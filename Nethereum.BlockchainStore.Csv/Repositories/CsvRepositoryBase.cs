using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Nethereum.BlockchainStore.Entities;

namespace Nethereum.BlockchainStore.Csv.Repositories
{
    public abstract class CsvRepositoryBase<TEntity>: IDisposable where TEntity: TableRow
    {
        private TextWriter _textWriter;
        private CsvHelper.CsvWriter _csvWriter;
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1);

        //hack - rather than store a last row index
        private long _rowIndex = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        public string FilePath { get; }

        protected CsvRepositoryBase(string csvFolderpath, string repositoryName)
        {
            FilePath = Path.Combine(csvFolderpath, $"{repositoryName}.csv");

            CreateFileIfNotExists();
        }

        private void CreateFileIfNotExists()
        {
            if (File.Exists(FilePath))
                return;

            using (var textWriter = File.CreateText(FilePath))
            {
                using(var csvWriter = new CsvWriter(textWriter))
                {
                    csvWriter.Configuration.RegisterClassMap(CreateClassMap());
                    csvWriter.WriteHeader<TEntity>();
                    csvWriter.NextRecord();
                    csvWriter.Flush();
                }
            }
        }

        protected abstract ClassMap<TEntity> CreateClassMap();

        protected async Task Write(TEntity entity)
        {
            await _lock.WaitAsync();
            try
            {
                if (_csvWriter == null)
                {
                    if (!File.Exists(FilePath))
                    {
                        _textWriter = File.CreateText(FilePath);
                        _csvWriter = new CsvWriter(_textWriter);
                        _csvWriter.Configuration.RegisterClassMap(CreateClassMap());
                        _csvWriter.WriteHeader<TEntity>();
                        _csvWriter.NextRecord();
                        _csvWriter.Flush();
                    }
                    else
                    {
                        _textWriter = File.AppendText(FilePath);
                        _csvWriter = new CsvWriter(_textWriter);
                        _csvWriter.Configuration.RegisterClassMap(CreateClassMap());
                    }
                }

                _rowIndex++;
                entity.RowIndex = (int)_rowIndex;
                _csvWriter.WriteRecord(entity);
                await _csvWriter.NextRecordAsync();
            }
            finally
            {
                _lock.Release();
            }
        }

        protected void CloseWriter()
        {
            if (_csvWriter != null)
            {
                _csvWriter.Flush();
                _csvWriter.Dispose();
                _csvWriter = null;
            }

            if (_textWriter != null)
            {
                _textWriter.Close();
                _textWriter = null;
            }

        }

        protected async Task<TEntity> FindAsync(Func<TEntity, bool> criteria)
        {
            try
            {
                await _lock.WaitAsync();
                CloseWriter();

                return await Task.Run(() =>
                {
                    using (var reader = File.OpenText(FilePath))
                    {
                        using (var csvReader = new CsvReader(reader))
                        {
                            csvReader.Configuration.HasHeaderRecord = true;
                            csvReader.Configuration.RegisterClassMap(CreateClassMap());
                            var entity = Activator.CreateInstance<TEntity>();
                            return csvReader.EnumerateRecords(entity).FirstOrDefault(criteria);
                        }
                    }
                });
            }
            finally
            {
                _lock.Release();
            }
        }

        protected async Task EnumerateAsync(Action<TEntity> rowAction)
        {
            try
            {
                await _lock.WaitAsync();

                CloseWriter();

                await Task.Run(() =>
                {
                    using (var reader = File.OpenText(FilePath))
                    {
                        using (var csvReader = new CsvReader(reader))
                        {
                            csvReader.Configuration.HasHeaderRecord = true;
                            csvReader.Configuration.RegisterClassMap(CreateClassMap());
                            var entity = Activator.CreateInstance<TEntity>();
                            foreach (var record in csvReader.EnumerateRecords(entity))
                            {
                                rowAction(record);
                            }
                        }
                    }
                });
            }
            finally
            {
                _lock.Release();
            }
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    CloseWriter();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion

    }
}
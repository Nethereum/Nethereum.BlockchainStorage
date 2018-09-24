using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Nethereum.BlockchainStore.Entities;
using Nethereum.Util;

namespace Nethereum.BlockchainStore.Csv.Repositories
{
    public abstract class CsvRepositoryBase<TEntity>: IDisposable where TEntity: TableRow
    {
        private TextWriter _textWriter;
        private CsvWriter _csvWriter;
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1);
        private readonly HashSet<string> _savedEntityHashes = new HashSet<string>();
        private readonly Sha3Keccack _hasher = new Sha3Keccack();

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
        
        private string CreateHash(TEntity entity)
        {
            var values = entity.GetType().GetProperties()
                .Where(p => p.Name != nameof(entity.RowIndex))
                .Where(p => p.Name != nameof(entity.RowCreated))
                .Where(p => p.Name != nameof(entity.RowUpdated))
                .Select(p => p.GetValue(entity))
                .Where(v => v != null)
                .Select(v => v.ToString());

            return _hasher.CalculateHash(string.Join("~", values));
        }

        protected async Task Write(TEntity entity)
        {
            await _lock.WaitAsync();
            try
            {
                var hash = CreateHash(entity);
                if (_savedEntityHashes.Contains(hash))
                    return;

                InitWriter();

                _rowIndex++;
                entity.RowIndex = (int)_rowIndex;
                _csvWriter.WriteRecord(entity);
                await _csvWriter.NextRecordAsync();
                _savedEntityHashes.Add(hash);
            }
            finally
            {
                _lock.Release();
            }
        }

        private void InitWriter()
        {
            if (_csvWriter != null) return;

            if (!File.Exists(FilePath))
            {
                _textWriter = File.CreateText(FilePath);
                _csvWriter = CreateCsvWriter(_textWriter);
                _csvWriter.WriteHeader<TEntity>();
                _csvWriter.NextRecord();
                _csvWriter.Flush();
            }
            else
            {
                LoadExistingRecordHashes();
                _textWriter = File.AppendText(FilePath);
                _csvWriter = CreateCsvWriter(_textWriter);
            }
        }

        private CsvWriter CreateCsvWriter(TextWriter textWriter)
        {
            var csvWriter = new CsvWriter(textWriter);
            csvWriter.Configuration.RegisterClassMap(CreateClassMap());
            return csvWriter;
        }

        private void LoadExistingRecordHashes()
        {
            Enumerate(entity =>
            {
                _savedEntityHashes.Add(CreateHash(entity));
            });
        }

        protected void CloseWriter()
        {
            _csvWriter?.Flush();
            _textWriter?.Flush();

            _csvWriter?.Dispose();
            _csvWriter = null;

            _textWriter?.Dispose();
            _textWriter = null;

        }

        protected async Task<TEntity> FindAsync(Func<TEntity, bool> criteria)
        {
            try
            {
                await _lock.WaitAsync();
                CloseWriter();

                TEntity row = null;
                WithCsvReader((csvReader) =>
                {
                    var entity = Activator.CreateInstance<TEntity>();
                    row = csvReader.EnumerateRecords(entity).FirstOrDefault(criteria);
                });

                return row;
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
                    Enumerate(rowAction);
                });
            }
            finally
            {
                _lock.Release();
            }
        }

        private void Enumerate(Action<TEntity> rowAction)
        {
            WithCsvReader((csvReader) =>
            {
                var entity = Activator.CreateInstance<TEntity>();
                foreach (var record in csvReader.EnumerateRecords(entity))
                {
                    rowAction(record);
                }
            });
        }

        private void WithCsvReader(Action<CsvReader> readerAction)
        {
            using (var reader = File.OpenText(FilePath))
            {
                using (var csvReader = new CsvReader(reader))
                {
                    csvReader.Configuration.HasHeaderRecord = true;
                    csvReader.Configuration.RegisterClassMap(CreateClassMap());
                    readerAction(csvReader);
                }
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
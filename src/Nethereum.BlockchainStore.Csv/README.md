# Nethereum.BlockchainStore.Csv

## CSV Repository Configuration Settings

[Common Configuration](../)

* --CsvOutputPath  (The directory where the CSV files will be written)

### CSV Specific Info

All of the repository storage implementations perform Upsert operations - they will update an existing record if it already exists else insert a new one.
This allows the storage processor to be re-run for the same block range.  This may be necessary if the a process crashed whilst writing transactions or if there has been a fork in the chain.
For CSV a true upsert is not really pragmatic or performant.  However it is still desirable to avoid writing duplicate records to a file.
If the CSV file already exists, the repo will read all records in the file and store a hash for each in memory.  
Before writing new records, the hash is compared to the in-memory store and if the hash is found the record is presumed to be a duplicate and is not written.

# Building Nethereum.BlockchainStore.EF.Hana.Tests

Note that the file `App.config` is hidden from the build process since it is named `App.config.EXAMPLE`. 
This project will build as-is using default EF data provider settings, not HANA.

To build for HANA:

1. Install HANA data provider on the same PC that build will happen. Instructions here: https://help.sap.com/viewer/1efad1691c1f496b8b580064a6536c2d/Cloud/en-US/7017cce72a054111b71cc713762e365c.html
2. Rename  `App.config.EXAMPLE` to be `App.config`.
3. Build project `Nethereum.BlockchainStore.EF.Hana.Tests`.

The build should complete successfully.


# Running Nethereum.BlockchainStore.EF.Hana.Tests

1. A HANA 2.x system is required.
2. Ensure that the schemas `dbo` and `DEMO` exist on the HANA system. (Edit `TestHanaContextFactory.cs` to write blockchain data to a schema other than `DEMO`, but `dbo` is always required for EF).
3. Edit the `App.config` file to have the correct connection string.

The tests should complete successfully.




using System;
using System.Linq;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.BlockchainStore;
using Nethereum.BlockchainStore.Handlers;
using Nethereum.BlockchainStore.Processing;
using Nethereum.BlockchainStore.Processors.Transactions;
using Nethereum.BlockchainStore.Web3Abstractions;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.ABI.Model;
using Nethereum.Hex.HexConvertors.Extensions;

namespace Nethereum.BlockchainProcessing.InMemory.Console
{
    class Program
    {
        public static string ByteCode = "60806040526040805190810160405280600481526020017f48312e30000000000000000000000000000000000000000000000000000000008152506006908051906020019062000051929190620001bf565b503480156200005f57600080fd5b5064174876e8006000803373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002081905550673a4965bf58a400006002819055506040805190810160405280600881526020017f4e617368436173680000000000000000000000000000000000000000000000008152506003908051906020019062000104929190620001bf565b506008600460006101000a81548160ff021916908360ff1602179055506040805190810160405280600581526020017f4e53435348000000000000000000000000000000000000000000000000000000815250600590805190602001906200016e929190620001bf565b506103e860078190555033600960006101000a81548173ffffffffffffffffffffffffffffffffffffffff021916908373ffffffffffffffffffffffffffffffffffffffff1602179055506200026e565b828054600181600116156101000203166002900490600052602060002090601f016020900481019282601f106200020257805160ff191683800117855562000233565b8280016001018555821562000233579182015b828111156200023257825182559160200191906001019062000215565b5b50905062000242919062000246565b5090565b6200026b91905b80821115620002675760008160009055506001016200024d565b5090565b90565b61139f806200027e6000396000f3006080604052600436106100d0576000357c0100000000000000000000000000000000000000000000000000000000900463ffffffff16806306fdde0314610390578063095ea7b31461042057806318160ddd146104855780632194f3a2146104b057806323b872dd14610507578063313ce5671461058c57806354fd4d50146105bd57806365f2bc2e1461064d57806370a0823114610678578063933ba413146106cf57806395d89b41146106fa578063a9059cbb1461078a578063cae9ca51146107ef578063dd62ed3e1461089a575b600034600854016008819055506007543402905080600080600960009054906101000a900473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff168152602001908152602001600020541015151561015357600080fd5b80600080600960009054906101000a900473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000205403600080600960009054906101000a900473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002081905550806000803373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002054016000803373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff168152602001908152602001600020819055503373ffffffffffffffffffffffffffffffffffffffff16600960009054906101000a900473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff167fddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef836040518082815260200191505060405180910390a3600960009054906101000a900473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff166108fc349081150290604051600060405180830381858888f1935050505015801561038c573d6000803e3d6000fd5b5050005b34801561039c57600080fd5b506103a5610911565b6040518080602001828103825283818151815260200191508051906020019080838360005b838110156103e55780820151818401526020810190506103ca565b50505050905090810190601f1680156104125780820380516001836020036101000a031916815260200191505b509250505060405180910390f35b34801561042c57600080fd5b5061046b600480360381019080803573ffffffffffffffffffffffffffffffffffffffff169060200190929190803590602001909291905050506109af565b604051808215151515815260200191505060405180910390f35b34801561049157600080fd5b5061049a610aa1565b6040518082815260200191505060405180910390f35b3480156104bc57600080fd5b506104c5610aa7565b604051808273ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200191505060405180910390f35b34801561051357600080fd5b50610572600480360381019080803573ffffffffffffffffffffffffffffffffffffffff169060200190929190803573ffffffffffffffffffffffffffffffffffffffff16906020019092919080359060200190929190505050610acd565b604051808215151515815260200191505060405180910390f35b34801561059857600080fd5b506105a1610d46565b604051808260ff1660ff16815260200191505060405180910390f35b3480156105c957600080fd5b506105d2610d59565b6040518080602001828103825283818151815260200191508051906020019080838360005b838110156106125780820151818401526020810190506105f7565b50505050905090810190601f16801561063f5780820380516001836020036101000a031916815260200191505b509250505060405180910390f35b34801561065957600080fd5b50610662610df7565b6040518082815260200191505060405180910390f35b34801561068457600080fd5b506106b9600480360381019080803573ffffffffffffffffffffffffffffffffffffffff169060200190929190505050610dfd565b6040518082815260200191505060405180910390f35b3480156106db57600080fd5b506106e4610e45565b6040518082815260200191505060405180910390f35b34801561070657600080fd5b5061070f610e4b565b6040518080602001828103825283818151815260200191508051906020019080838360005b8381101561074f578082015181840152602081019050610734565b50505050905090810190601f16801561077c5780820380516001836020036101000a031916815260200191505b509250505060405180910390f35b34801561079657600080fd5b506107d5600480360381019080803573ffffffffffffffffffffffffffffffffffffffff16906020019092919080359060200190929190505050610ee9565b604051808215151515815260200191505060405180910390f35b3480156107fb57600080fd5b50610880600480360381019080803573ffffffffffffffffffffffffffffffffffffffff16906020019092919080359060200190929190803590602001908201803590602001908080601f016020809104026020016040519081016040528093929190818152602001838380828437820191505050505050919291929050505061104f565b604051808215151515815260200191505060405180910390f35b3480156108a657600080fd5b506108fb600480360381019080803573ffffffffffffffffffffffffffffffffffffffff169060200190929190803573ffffffffffffffffffffffffffffffffffffffff1690602001909291905050506112ec565b6040518082815260200191505060405180910390f35b60038054600181600116156101000203166002900480601f0160208091040260200160405190810160405280929190818152602001828054600181600116156101000203166002900480156109a75780601f1061097c576101008083540402835291602001916109a7565b820191906000526020600020905b81548152906001019060200180831161098a57829003601f168201915b505050505081565b600081600160003373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060008573ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff168152602001908152602001600020819055508273ffffffffffffffffffffffffffffffffffffffff163373ffffffffffffffffffffffffffffffffffffffff167f8c5be1e5ebec7d5bd14f71427d1e84f3dd0314c0f7b2291e5b200ac8c7c3b925846040518082815260200191505060405180910390a36001905092915050565b60025481565b600960009054906101000a900473ffffffffffffffffffffffffffffffffffffffff1681565b6000816000808673ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000205410158015610b99575081600160008673ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060003373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000205410155b8015610ba55750600082115b15610d3a57816000808573ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060008282540192505081905550816000808673ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000206000828254039250508190555081600160008673ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060003373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff168152602001908152602001600020600082825403925050819055508273ffffffffffffffffffffffffffffffffffffffff168473ffffffffffffffffffffffffffffffffffffffff167fddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef846040518082815260200191505060405180910390a360019050610d3f565b600090505b9392505050565b600460009054906101000a900460ff1681565b60068054600181600116156101000203166002900480601f016020809104026020016040519081016040528092919081815260200182805460018160011615610100020316600290048015610def5780601f10610dc457610100808354040283529160200191610def565b820191906000526020600020905b815481529060010190602001808311610dd257829003601f168201915b505050505081565b60075481565b60008060008373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff168152602001908152602001600020549050919050565b60085481565b60058054600181600116156101000203166002900480601f016020809104026020016040519081016040528092919081815260200182805460018160011615610100020316600290048015610ee15780601f10610eb657610100808354040283529160200191610ee1565b820191906000526020600020905b815481529060010190602001808311610ec457829003601f168201915b505050505081565b6000816000803373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020019081526020016000205410158015610f395750600082115b1561104457816000803373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060008282540392505081905550816000808573ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff168152602001908152602001600020600082825401925050819055508273ffffffffffffffffffffffffffffffffffffffff163373ffffffffffffffffffffffffffffffffffffffff167fddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef846040518082815260200191505060405180910390a360019050611049565b600090505b92915050565b600082600160003373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060008673ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff168152602001908152602001600020819055508373ffffffffffffffffffffffffffffffffffffffff163373ffffffffffffffffffffffffffffffffffffffff167f8c5be1e5ebec7d5bd14f71427d1e84f3dd0314c0f7b2291e5b200ac8c7c3b925856040518082815260200191505060405180910390a38373ffffffffffffffffffffffffffffffffffffffff1660405180807f72656365697665417070726f76616c28616464726573732c75696e743235362c81526020017f616464726573732c627974657329000000000000000000000000000000000000815250602e01905060405180910390207c01000000000000000000000000000000000000000000000000000000009004338530866040518563ffffffff167c0100000000000000000000000000000000000000000000000000000000028152600401808573ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff1681526020018481526020018373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff168152602001828051906020019080838360005b83811015611290578082015181840152602081019050611275565b50505050905090810190601f1680156112bd5780820380516001836020036101000a031916815260200191505b509450505050506000604051808303816000875af19250505015156112e157600080fd5b600190509392505050565b6000600160008473ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff16815260200190815260200160002060008373ffffffffffffffffffffffffffffffffffffffff1673ffffffffffffffffffffffffffffffffffffffff168152602001908152602001600020549050929150505600a165627a7a723058203ae89336cd7fa50ae946738a781cd60d9e8d92a2ea6959eb2414b1f4867fe84b0029";
        public static string Abi = "[{\"constant\":true,\"inputs\":[],\"name\":\"getWarriorChestAvailable\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[{\"name\":\"_interfaceId\",\"type\":\"bytes4\"}],\"name\":\"supportsInterface\",\"outputs\":[{\"name\":\"\",\"type\":\"bool\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"name\",\"outputs\":[{\"name\":\"\",\"type\":\"string\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[{\"name\":\"_tokenId\",\"type\":\"uint256\"}],\"name\":\"getApproved\",\"outputs\":[{\"name\":\"\",\"type\":\"address\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"_to\",\"type\":\"address\"},{\"name\":\"_tokenId\",\"type\":\"uint256\"}],\"name\":\"approve\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"totalSupply\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"InterfaceId_ERC165\",\"outputs\":[{\"name\":\"\",\"type\":\"bytes4\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"getWarriorChestPrice\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"_from\",\"type\":\"address\"},{\"name\":\"_to\",\"type\":\"address\"},{\"name\":\"_tokenId\",\"type\":\"uint256\"}],\"name\":\"transferFrom\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"_region\",\"type\":\"uint256\"}],\"name\":\"buyApprenticeChest\",\"outputs\":[],\"payable\":true,\"stateMutability\":\"payable\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[{\"name\":\"_owner\",\"type\":\"address\"},{\"name\":\"_index\",\"type\":\"uint256\"}],\"name\":\"tokenOfOwnerByIndex\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"getClosingTime\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"getWarlordChestPrice\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"_from\",\"type\":\"address\"},{\"name\":\"_to\",\"type\":\"address\"},{\"name\":\"_tokenId\",\"type\":\"uint256\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"isOpen\",\"outputs\":[{\"name\":\"\",\"type\":\"bool\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"getWarlordChestAvailable\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[{\"name\":\"_tokenId\",\"type\":\"uint256\"}],\"name\":\"exists\",\"outputs\":[{\"name\":\"\",\"type\":\"bool\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[{\"name\":\"_index\",\"type\":\"uint256\"}],\"name\":\"tokenByIndex\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"getApprenticeChestAvailable\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"getGladiatorChestAvailable\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"_identifier\",\"type\":\"uint256\"}],\"name\":\"openChest\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[{\"name\":\"_tokenId\",\"type\":\"uint256\"}],\"name\":\"ownerOf\",\"outputs\":[{\"name\":\"\",\"type\":\"address\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"_emitter\",\"type\":\"address\"},{\"name\":\"_administrator\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"_to\",\"type\":\"address\"},{\"name\":\"_class\",\"type\":\"uint256\"},{\"name\":\"_type\",\"type\":\"uint256\"},{\"name\":\"_rarity\",\"type\":\"uint256\"},{\"name\":\"_tier\",\"type\":\"uint256\"},{\"name\":\"_name\",\"type\":\"uint256\"},{\"name\":\"_region\",\"type\":\"uint256\"}],\"name\":\"addItemTo\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[{\"name\":\"_owner\",\"type\":\"address\"}],\"name\":\"balanceOf\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"symbol\",\"outputs\":[{\"name\":\"\",\"type\":\"string\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"_to\",\"type\":\"address\"},{\"name\":\"_approved\",\"type\":\"bool\"}],\"name\":\"setApprovalForAll\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"_from\",\"type\":\"address\"},{\"name\":\"_to\",\"type\":\"address\"},{\"name\":\"_tokenId\",\"type\":\"uint256\"},{\"name\":\"_data\",\"type\":\"bytes\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"getApprenticeChestPrice\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"_region\",\"type\":\"uint256\"}],\"name\":\"buyGladiatorChest\",\"outputs\":[],\"payable\":true,\"stateMutability\":\"payable\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"_region\",\"type\":\"uint256\"}],\"name\":\"buyTokenPack\",\"outputs\":[],\"payable\":true,\"stateMutability\":\"payable\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[{\"name\":\"_tokenId\",\"type\":\"uint256\"}],\"name\":\"tokenURI\",\"outputs\":[{\"name\":\"\",\"type\":\"string\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"getGladiatorChestPrice\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"getOpeningTime\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"getTokenPackPrice\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[{\"name\":\"_owner\",\"type\":\"address\"},{\"name\":\"_operator\",\"type\":\"address\"}],\"name\":\"isApprovedForAll\",\"outputs\":[{\"name\":\"\",\"type\":\"bool\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"_uri\",\"type\":\"string\"}],\"name\":\"setTokenUriPref\",\"outputs\":[],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"_region\",\"type\":\"uint256\"}],\"name\":\"buyWarriorChest\",\"outputs\":[],\"payable\":true,\"stateMutability\":\"payable\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"_region\",\"type\":\"uint256\"}],\"name\":\"buyWarlordChest\",\"outputs\":[],\"payable\":true,\"stateMutability\":\"payable\",\"type\":\"function\"},{\"constant\":false,\"inputs\":[{\"name\":\"_region\",\"type\":\"uint256\"}],\"name\":\"buyInvestorPack\",\"outputs\":[],\"payable\":true,\"stateMutability\":\"payable\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"getInvestorPacksAvailable\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"getInvestorPackPrice\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"constant\":true,\"inputs\":[],\"name\":\"getTokenPacksAvailable\",\"outputs\":[{\"name\":\"\",\"type\":\"uint256\"}],\"payable\":false,\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"name\":\"_emitter\",\"type\":\"address\"},{\"name\":\"_administrator\",\"type\":\"address\"},{\"name\":\"_gameCoin\",\"type\":\"address\"},{\"name\":\"_openingTime\",\"type\":\"uint256\"},{\"name\":\"_closingTime\",\"type\":\"uint256\"}],\"payable\":false,\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"name\":\"_from\",\"type\":\"address\"},{\"indexed\":true,\"name\":\"_to\",\"type\":\"address\"},{\"indexed\":true,\"name\":\"_tokenId\",\"type\":\"uint256\"}],\"name\":\"Transfer\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"name\":\"_owner\",\"type\":\"address\"},{\"indexed\":true,\"name\":\"_approved\",\"type\":\"address\"},{\"indexed\":true,\"name\":\"_tokenId\",\"type\":\"uint256\"}],\"name\":\"Approval\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"name\":\"_owner\",\"type\":\"address\"},{\"indexed\":true,\"name\":\"_operator\",\"type\":\"address\"},{\"indexed\":false,\"name\":\"_approved\",\"type\":\"bool\"}],\"name\":\"ApprovalForAll\",\"type\":\"event\"}]\r\n";
        
        static void Main(string[] args)
        {
            System.Console.WriteLine(string.Join(" ", args));

            var appConfig = ConfigurationUtils.Build(args).AddConsoleLogging();

            var targetBlockchain = BlockchainSourceConfigurationFactory.Get(appConfig);

            System.Console.WriteLine($"Target Blockchain: {targetBlockchain.Name}, {targetBlockchain.BlockchainUrl}");

            var web3 = new Web3.Web3(targetBlockchain.BlockchainUrl);

            //var filters = new FilterContainer();

            //Example Filters 
            //These are singular - but the filter container can accept multiple filters
            //When using multiple filters, only one has to match 
            //var filters = new FilterContainer(new TransactionFilter((tx) => tx.Value.Value > 0 && tx.From == "<some address>"));
            //var filters = new FilterContainer(TransactionReceiptFilter.OnlyNewContracts());
            //var filters = new FilterContainer(TransactionFilter.FromAndTo("<from>", "<to>"));
            //var filters = new FilterContainer(TransactionReceiptFilter.OnlyNewContracts());

            var contractAddress = "0xC03cDD393C89D169bd4877d58f0554f320f21037";
            var contract = web3.Eth.GetContract(Abi, contractAddress);

            //contract.ContractBuilder.ContractABI.Functions[0].Sha3Signature

            var transactionHandler = new TransferTransactionHandler(contract);

            var filters = new FilterContainer(
                TransactionFilter.To(contractAddress));

            var strategy = new ProcessingStrategy
            {
                Filters = filters,
                BlockHandler = new InMemoryBlockHandler(System.Console.WriteLine),
                //TransactionHandler = new InMemoryTransactionHandler(System.Console.WriteLine),
                TransactionHandler = transactionHandler,
                TransactionLogHandler = new InMemoryTransactionLogHandler(System.Console.WriteLine),
                TransactionVmStackHandler = new InMemoryTransactionVmStackHandler(System.Console.WriteLine),
                ContractHandler = new InMemoryContractHandler(System.Console.WriteLine),
                MinimumBlockConfirmations = 6 //wait for 6 block confirmations before processing block
            };

            var web3Wrapper = new Web3Wrapper(web3);
            
            var blockProcessorFactory = new BlockProcessorFactory();
            var blockProcessor = blockProcessorFactory.Create(
                web3Wrapper, strategy, processTransactionsInParallel: false);

            var blockchainProcessor = new BlockchainProcessor(strategy, blockProcessor);

            blockchainProcessor.ExecuteAsync
                (targetBlockchain.FromBlock, targetBlockchain.ToBlock)
                .Wait();
        }

        public class TransferTransactionHandler : ITransactionHandler
        {
            private readonly Contracts.Contract _contract;
            private readonly Contracts.Event _transferEvent;
            private readonly Contracts.Event _approvalEvent;

            public TransferTransactionHandler(Contracts.Contract contract)
            {
                _transferEvent = contract.GetEvent("Transfer");
                _approvalEvent = contract.GetEvent("Approval");
                this._contract = contract;
            }

            public Task HandleContractCreationTransactionAsync(ContractCreationTransaction contractCreationTransaction)
            {
                return Task.CompletedTask;
            }

            public Task HandleTransactionAsync(TransactionWithReceipt txn)
            {
                if (!txn.HasLogs()) return Task.CompletedTask;
               
                var transferLogs = txn.GetEvents<TransferEvent>(_transferEvent);
                var approvalLogs = txn.GetEvents<ApprovalEvent>(_approvalEvent);

                //0x24f3c639
                var function = txn.GetFunction(_contract);

                System.Console.WriteLine($"Function: {function?.NameAndParameters() ?? "unknown"}");

                var testEvent = txn.Logs().First().DecodeEvent<TransferEvent>(_transferEvent);

                foreach (var log in transferLogs)
                {
                    System.Console.WriteLine($"from:{log.Event.From},to:{log.Event.To},value:{log.Event.Value}");
                }

                foreach (var log in approvalLogs)
                {
                    System.Console.WriteLine($"owner:{log.Event.Owner},spender:{log.Event.Spender},value:{log.Event.Value}");
                }

                return Task.CompletedTask;
            }
        }

        /*
 *     event Transfer(address indexed _from, address indexed _to, uint256 indexed _value);
 */
        public class TransferEvent
        {
            [Parameter("address", "_from", 1, true)]
            public string From {get; set;}

            [Parameter("address", "_to", 2, true)]
            public string To {get; set;}

            [Parameter("uint256", "_tokenId", 3, true)]
            public BigInteger Value {get; set;}
        }

        /*
         * event Approval(address indexed _owner, address indexed _spender, uint256 _value);
         */
        public class ApprovalEvent
        {
            [Parameter("address", "_owner", 1, true)]
            public string Owner {get; set;}

            [Parameter("address", "_spender", 2, true)]
            public string Spender {get; set;}

            [Parameter("uint256", "_value", 3, false)]
            public BigInteger Value {get; set;}
        }
    }
}

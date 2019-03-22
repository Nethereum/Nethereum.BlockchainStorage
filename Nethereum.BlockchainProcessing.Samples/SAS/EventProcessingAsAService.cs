using Moq;
using Nethereum.BlockchainProcessing.BlockchainProxy;
using Nethereum.BlockchainProcessing.Processing;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainProcessing.Samples.SAS
{
    public class EventProcessingAsAService
    {
        private static readonly string StandardContractAbi = "[{'constant':true,'inputs':[],'name':'name','outputs':[{'name':'','type':'string'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'name':'_spender','type':'address'},{'name':'_value','type':'uint256'}],'name':'approve','outputs':[{'name':'success','type':'bool'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[],'name':'totalSupply','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'name':'_from','type':'address'},{'name':'_to','type':'address'},{'name':'_value','type':'uint256'}],'name':'transferFrom','outputs':[{'name':'success','type':'bool'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[{'name':'','type':'address'}],'name':'balances','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'decimals','outputs':[{'name':'','type':'uint8'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[{'name':'','type':'address'},{'name':'','type':'address'}],'name':'allowed','outputs':[{'name':'','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[{'name':'_owner','type':'address'}],'name':'balanceOf','outputs':[{'name':'balance','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':true,'inputs':[],'name':'symbol','outputs':[{'name':'','type':'string'}],'payable':false,'stateMutability':'view','type':'function'},{'constant':false,'inputs':[{'name':'_to','type':'address'},{'name':'_value','type':'uint256'}],'name':'transfer','outputs':[{'name':'success','type':'bool'}],'payable':false,'stateMutability':'nonpayable','type':'function'},{'constant':true,'inputs':[{'name':'_owner','type':'address'},{'name':'_spender','type':'address'}],'name':'allowance','outputs':[{'name':'remaining','type':'uint256'}],'payable':false,'stateMutability':'view','type':'function'},{'inputs':[{'name':'_initialAmount','type':'uint256'},{'name':'_tokenName','type':'string'},{'name':'_decimalUnits','type':'uint8'},{'name':'_tokenSymbol','type':'string'}],'payable':false,'stateMutability':'nonpayable','type':'constructor'},{'anonymous':false,'inputs':[{'indexed':true,'name':'_from','type':'address'},{'indexed':true,'name':'_to','type':'address'},{'indexed':false,'name':'_value','type':'uint256'}],'name':'Transfer','type':'event'},{'anonymous':false,'inputs':[{'indexed':true,'name':'_owner','type':'address'},{'indexed':true,'name':'_spender','type':'address'},{'indexed':false,'name':'_value','type':'uint256'}],'name':'Approval','type':'event'}]";
        private static readonly string TransferEventSignature = "ddf252ad1be2c89b69c2b068fc378daa952ba7f163c4a11628f55a4df523b3ef";

        private const string BlockchainUrl = "https://rinkeby.infura.io/v3/25e7b6dfc51040b3bfc0e47317d38f60";

        [Fact]
        public async Task DynamicContractQuerying_NoParams_Returns_uint256()
        {
            var signature_totalSupply = "18160ddd";
            var contractAddress = "0x78c1301520edff0bb14314c64987a71fa5efa407";
            var blockchainProxyService = new BlockchainProxyService(BlockchainUrl);
            var decoded = await blockchainProxyService.Query(contractAddress, StandardContractAbi, signature_totalSupply);
            Assert.IsType<BigInteger>(decoded);

           //  Nethereum.Util.UnitConversion.Convert.FromWei(accountTotal, UnitConversion.EthUnit.Ether)
        }

        [Fact]
        public async Task DynamicContractQuerying_NoParams_Returns_string()
        {
            var signature_name = "06fdde03";
            var contractAddress = "0x78c1301520edff0bb14314c64987a71fa5efa407";

            var blockchainProxyService = new BlockchainProxyService(BlockchainUrl);
            var decoded = await blockchainProxyService.Query(contractAddress, StandardContractAbi, signature_name);
            Assert.Equal("JGX", decoded);  
        }

        [Fact]
        public async Task DynamicContractQuerying_AddressParam_Returns_uint256()
        {
            var signature_balanceOf = "70a08231"; 
            var contractAddress = "0x78c1301520edff0bb14314c64987a71fa5efa407";
            var functionInput = new object[]{"0xa13210c21fbbed075ec210a71b477a81cb3da7d8"}; // _owner parameter - type: address

            var blockchainProxyService = new BlockchainProxyService(BlockchainUrl);
            var decoded = await blockchainProxyService.Query(contractAddress, StandardContractAbi, signature_balanceOf, functionInput);
            Assert.IsType<BigInteger>(decoded);
        }

        [Fact]
        public async Task WebJobExample()
        {
            const long PartitionId = 1;
            string JsonProgressFilePath = Path.Combine(Path.GetTempPath(), "WebJobExampleBlockProcess.json");
            if(File.Exists(JsonProgressFilePath)) File.Delete(JsonProgressFilePath);
            const ulong MinimumBlockNumber = 4063361;
            const uint MaxBlocksPerBatch = 10;

            IEventProcessingConfigurationDb configDb = CreateMockDb();

            var blockchainProxy = new BlockchainProxyService(new Web3.Web3(BlockchainUrl));
            var eventHandlerFactory = new DecodedEventHandlerFactory(configDb, blockchainProxy);
            var processorFactory = new EventProcessorFactory(configDb, eventHandlerFactory);
            var eventProcessors = await processorFactory.GetLogProcessorsAsync(PartitionId);
            var logProcessor = new BlockchainLogProcessor(blockchainProxy, eventProcessors);
            var jsonProgressRepository = new JsonBlockProgressRepository(JsonProgressFilePath);
            var progressService = new BlockProgressService(blockchainProxy, MinimumBlockNumber, jsonProgressRepository);
            var batchProcessorService = new BlockchainBatchProcessorService(logProcessor, progressService, MaxBlocksPerBatch);

            var ctx = new System.Threading.CancellationTokenSource();
            var rangeProcessed = await batchProcessorService.ProcessLatestBlocksAsync(ctx.Token);

            await eventHandlerFactory.SaveStateAsync();

            Assert.NotNull(rangeProcessed);
            Assert.Equal((ulong)11, rangeProcessed.Value.BlockCount);
        }

        private static IEventProcessingConfigurationDb CreateMockDb()
        {
            var configDb = new Mock<IEventProcessingConfigurationDb>();

            const long Partition1 = 1;

            var subscribers = new[]
            {
                new SubscriberDto
                {
                    Id = 1,
                    Disabled = false,
                    OrganisationName = "Harry",
                    PartitionId = Partition1
                },
                new SubscriberDto
                {
                    Id = 2,
                    Disabled = false,
                    OrganisationName = "George",
                    PartitionId = Partition1
                },
                new SubscriberDto
                {
                    Id = 3,
                    Disabled = false,
                    OrganisationName = "Nosey Parker",
                    PartitionId = Partition1
                },
            };

            var contracts = new[]
            {
                subscribers[0].CreateContract(1, "StandardContract", StandardContractAbi),
                subscribers[1].CreateContract(2, "StandardContract", StandardContractAbi)
            };

            var eventSubscriptions = new[]
            {
                subscribers[0].CreateEventSubscription(1, contracts[0].Id, TransferEventSignature),
                subscribers[1].CreateEventSubscription(2, contracts[1].Id, TransferEventSignature),
                subscribers[2].CreateEventSubscription(3) // catch all
            };

            var eventAddresses = new EventAddressDto[]
            {
                new EventAddressDto{Id = 1, EventSubscriptionId = 3, Address = "0x924442a66cfd812308791872c4b242440c108e19"}
            };

            var eventHandlers = new DecodedEventHandlerDto[]
            {
                new DecodedEventHandlerDto
                {
                    Id = 1, Disabled = false, EventSubscriptionId = 1, HandlerType = EventHandlerType.Aggregate, Order = 1
                },

                new DecodedEventHandlerDto
                {
                    Id = 2, Disabled = false, EventSubscriptionId = 1, HandlerType = EventHandlerType.Queue, Order = 2
                },

                new DecodedEventHandlerDto
                {
                    Id = 3, Disabled = false, EventSubscriptionId = 2, HandlerType = EventHandlerType.Aggregate, Order = 1
                },

                new DecodedEventHandlerDto
                {
                    Id = 4, Disabled = false, EventSubscriptionId = 2, HandlerType = EventHandlerType.Queue, Order = 2
                },

                new DecodedEventHandlerDto
                {
                    Id = 5, Disabled = false, EventSubscriptionId = 3, HandlerType = EventHandlerType.Queue, Order = 1
                }
            };

            var parameterConditions = new ParameterConditionDto[]{};

            configDb
                .Setup(d => d.GetSubscribersAsync(It.IsAny<long>()))
                .Returns<long>((partitionId) => Task.FromResult(subscribers.Where(s => s.PartitionId == partitionId).ToArray()));

            configDb
                .Setup(d => d.GetEventSubscriptionsAsync(It.IsAny<long>()))
                .Returns<long>((subscriberId) => Task.FromResult(eventSubscriptions.Where(s => s.SubscriberId == subscriberId).ToArray()));

            configDb
                .Setup(d => d.GetContractAsync(It.IsAny<long>()))
                .Returns<long>((contractId) => Task.FromResult(contracts.Where(s => s.Id == contractId).FirstOrDefault()));

            configDb
                .Setup(d => d.GetEventAddressesAsync(It.IsAny<long>()))
                .Returns<long>((eventSubscriptionId) => Task.FromResult(eventAddresses.Where(s => s.EventSubscriptionId == eventSubscriptionId).ToArray()));

            configDb
                .Setup(d => d.GetParameterConditionsAsync(It.IsAny<long>()))
                .Returns<long>((eventSubscriptionId) => Task.FromResult(parameterConditions.Where(s => s.EventSubscriptionId == eventSubscriptionId).ToArray()));

            configDb
                .Setup(d => d.GetDecodedEventHandlers(It.IsAny<long>()))
                .Returns<long>((eventSubscriptionId) => Task.FromResult(eventHandlers.Where(s => s.EventSubscriptionId == eventSubscriptionId).ToArray()));

            Dictionary<long, EventSubscriptionStateDto> eventSubscriptionStateContainer = new Dictionary<long, EventSubscriptionStateDto>();

            configDb
                .Setup(d => d.GetEventSubscriptionStateAsync(It.IsAny<long>()))
                .Returns<long>((eventSubscriptionId) => 
                {
                    if (!eventSubscriptionStateContainer.ContainsKey(eventSubscriptionId))
                    {
                        eventSubscriptionStateContainer.Add(eventSubscriptionId, new EventSubscriptionStateDto(eventSubscriptionId));
                    }
                    return Task.FromResult(eventSubscriptionStateContainer[eventSubscriptionId]);
                });

            configDb
                .Setup(d => d.SaveAsync(It.IsAny<EventSubscriptionStateDto>()))
                .Callback<EventSubscriptionStateDto>((state) =>  eventSubscriptionStateContainer[state.EventSubscriptionId] = state)
                .Returns(Task.CompletedTask);

            return configDb.Object;

        }
    }
}

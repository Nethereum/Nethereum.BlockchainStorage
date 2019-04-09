using Moq;
using Nethereum.BlockchainProcessing.Processing.Logs;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling;
using Nethereum.BlockchainProcessing.Processing.Logs.Handling.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Nethereum.BlockchainProcessing.Tests.Processing.Logs.Handlers.Queues
{
    public class QueueHandlerTests
    {
        [Fact]
        public async Task TransformsAndSendsToQueue() 
        {
            var subscription = new Mock<IEventSubscription>();
            var queue = new Mock<IQueue>();
            var handler = new QueueHandler(subscription.Object, 99, queue.Object);
            var decodedLog = DecodedEvent.Empty();

            EventLogQueueMessage actualQueueMessage = null;
            queue
                .Setup(q => q.AddMessageAsync(It.IsAny<object>()))
                .Callback<object>((msg) => actualQueueMessage = msg as EventLogQueueMessage)
                .Returns(Task.CompletedTask);

            var result = await handler.HandleAsync(decodedLog);

            Assert.True(result);
            Assert.NotNull(actualQueueMessage);
        }

        [Fact]
        public void DecodedEventToQueueMessage()
        {
            var log = TestData.Contracts.StandardContract.SampleTransferLog();
            var decodedLog = log.ToDecodedEvent(TestData.Contracts.StandardContract.TransferEventAbi);

            decodedLog.Transaction = new RPC.Eth.DTOs.Transaction();
            decodedLog.State["test"] = "test";

            var queueMessage = decodedLog.ToQueueMessage();

            Assert.NotNull(queueMessage);
            Assert.Equal(decodedLog.Key, queueMessage.Key);
            Assert.Equal(decodedLog.State["test"], queueMessage.State["test"]);

            Assert.Equal(decodedLog.Event.Count, queueMessage.ParameterValues.Count);

            foreach(var parameter in decodedLog.Event)
            {
                var copy = queueMessage.ParameterValues.FirstOrDefault(p => p.Order == parameter.Parameter.Order);
                Assert.NotNull(copy);
                Assert.Equal(parameter.Result, copy.Value);
                Assert.Equal(parameter.Parameter.Indexed, copy.Indexed);
                Assert.Equal(parameter.Parameter.Name, copy.Name);
                Assert.Equal(parameter.Parameter.ABIType.Name, copy.AbiType);
            }
        }
        
    }
}

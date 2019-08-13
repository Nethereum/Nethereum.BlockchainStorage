using Moq;
using Nest;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Nethereum.BlockchainStore.Search.Tests.Elastic
{
    public class MockElasticClient
    {
        Mock<IElasticClient> _mockElasticClient = new Mock<IElasticClient>();
        Mock<IBulkResponse> _mockBulkResponse = new Mock<IBulkResponse>();

        public MockElasticClient()
        {
            _mockBulkResponse.Setup(b => b.IsValid).Returns(true);

            _mockElasticClient.Setup(s => s.BulkAsync(It.IsAny<IBulkRequest>(), default(CancellationToken)))
                .Callback<IBulkRequest, CancellationToken>((req, ctx) => BulkRequests.Add(req))
                .ReturnsAsync(_mockBulkResponse.Object);

        }

        public IElasticClient ElasticClient => _mockElasticClient.Object;

        public List<IBulkRequest> BulkRequests { get; } = new List<IBulkRequest>();

        public IBulkOperation GetFirstBulkOperation() => BulkRequests.FirstOrDefault().Operations.FirstOrDefault();

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using Xunit;

namespace Nethereum.BlockchainStore.Search.Tests
{
    public class SearcherFactoryTests
    {
        private readonly Mock<ISearcher> _blockSearcher = new Mock<ISearcher>();
        private readonly Mock<ISearcher> _transactionSearcher = new Mock<ISearcher>();
        private readonly Mock<ISearcher> _contractSearcher = new Mock<ISearcher>();
        private readonly Mock<ISearcher> _addressSearcher = new Mock<ISearcher>();

        private readonly SearcherFactory _searcherFactory;

        public SearcherFactoryTests()
        {
            _searcherFactory = new SearcherFactory(
                _blockSearcher.Object,
                _transactionSearcher.Object,
                _contractSearcher.Object,
                _addressSearcher.Object);
        }

        [Fact]
        public void ReturnsBlockSearcher()
        {
            var searchers = _searcherFactory.FindSearchers("7081049");
            Assert.Single(searchers, _blockSearcher.Object);
        }

        [Fact]
        public void ReturnsTransactionSearcher()
        {
            var searchers = _searcherFactory.FindSearchers("0x51dfdaa8e7273075af4fc03fb04111b445b3d5312c51bd48e7c7b8e077ceeff8");
            Assert.Single(searchers, _transactionSearcher.Object);
        }

        [Fact]
        public void WhenQueryIsAnAddress_ReturnsContractAndAddressSearchers()
        {
            var searchers = _searcherFactory.FindSearchers("0x53b04999C1FF2d77fCddE98935BB936A67209E4C");
            Assert.Equal(2, searchers.Length);
            Assert.Same(_contractSearcher.Object, searchers[0]);
            Assert.Same(_addressSearcher.Object, searchers[1]);
        }

        [Fact]
        public void WhenQueryIsUnrecognised_ReturnsContractAndAddressSearchers()
        {
            var searchers = _searcherFactory.FindSearchers("garbage");
            Assert.Single(searchers);
            Assert.Equal(typeof(UnknownSearchTypeSearcher), searchers[0].GetType());
        }
    }
}

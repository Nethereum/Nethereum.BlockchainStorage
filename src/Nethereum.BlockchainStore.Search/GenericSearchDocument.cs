using System;
using System.Collections.Generic;
using System.Text;

namespace Nethereum.BlockchainStore.Search
{

    public class GenericSearchDocument : Dictionary<string, object>, IHasId
    {
        private string _id = null;

        public string GetId() => _id;

        public string SetId(string id) => this._id = id;
    }
}

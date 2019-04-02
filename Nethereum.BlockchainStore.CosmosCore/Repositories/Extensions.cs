using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Microsoft.Azure.Documents;

namespace Nethereum.BlockchainStore.CosmosCore.Repositories
{
    public static class Extensions
    {
        public static bool IsNotFound(this DocumentClientException dEx)
        {
            return dEx.StatusCode == HttpStatusCode.NotFound;
        }
    }
}

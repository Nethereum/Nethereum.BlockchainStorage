using System.Linq;

namespace Nethereum.RPC.Eth.DTOs
{
    public class OpCodes
    {
        public const string Call = "CALL";
        public const string DelegateCall = "DELEGATECALL";
        public const string Create = "CREATE";
        public const string Return = "RETURN";

        public static readonly string[] InterContract = new []{
            Call, Create, DelegateCall
        };

        public static bool IsInterContract(string opCode)
        {
            return InterContract.Contains(opCode);
        }
    }
}
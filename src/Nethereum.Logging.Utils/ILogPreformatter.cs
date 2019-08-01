namespace Nethereum.Microsoft.Logging.Utils
{
    public interface ILogPreformatter
    {
        bool TryPreformat(string templateString, object[] args, out string newTemplate, out object[] newArgs);
    }
}

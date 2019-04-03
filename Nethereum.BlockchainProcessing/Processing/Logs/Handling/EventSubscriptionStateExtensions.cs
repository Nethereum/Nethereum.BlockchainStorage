namespace Nethereum.BlockchainProcessing.Processing.Logs.Handling
{
    public static class EventSubscriptionStateExtensions
    {
        public static int GetInt(this EventSubscriptionStateDto state, string key, int defaultIfNull = 0)
        {
            if (state.Values.ContainsKey(key))
            {
                return (int)state.Values[key];
            }
            return defaultIfNull;
        }

        public static void SetInt(this EventSubscriptionStateDto state, string key, int value)
        {
            state.Values[key] = value;
        }

        public static void Set(this EventSubscriptionStateDto state, string key, object value)
        {
            state.Values[key] = value;
        }

        public static void Increment(this EventSubscriptionStateDto state, string key)
        {
            var initialValue = state.GetInt(key);
            state.SetInt(key, initialValue + 1);
        }

        public static object Get(this EventSubscriptionStateDto state, string key)
        {
            if(state.Values.TryGetValue(key, out object val))
            {
                return val;
            }
            return null;
        }
    }
}

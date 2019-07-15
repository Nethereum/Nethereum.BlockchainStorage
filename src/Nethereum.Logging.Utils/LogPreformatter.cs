using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Microsoft.Logging.Utils
{
    public class LogPreformatter : ILogPreformatter
    {
        static readonly Regex _numericFormattedRegex = new Regex(@"{(\d{1,})}", RegexOptions.Compiled);

        public bool TryPreformat(string templateString, object[] args, out string newTemplate, out object[] newArgs)
        {
            newTemplate = templateString;
            newArgs = args;

            if (string.IsNullOrEmpty(templateString))
            {
                return true;
            }

            if (args == null || !args.Any())
            {
                return true;
            }

            var templateBuilder = new StringBuilder(templateString);
            var numericArgPositions = new HashSet<int>();
            var matches = _numericFormattedRegex.Matches(templateString);

            for (var i = matches.Count - 1; i >= 0; i--)
            {
                var currentMatcher = matches[i];
                var argPosition = GetArgumentPosition(currentMatcher);
                var currentArg = args[argPosition];

                templateBuilder.Remove(currentMatcher.Index, currentMatcher.Length);
                templateBuilder.Insert(currentMatcher.Index, currentArg);

                numericArgPositions.Add(argPosition);
            }

            newTemplate = templateBuilder.ToString();
            newArgs = RemoveItemsAtPositions(args, numericArgPositions);
            return true;
        }

        static int GetArgumentPosition(Match currentMatcher)
        {
            if (currentMatcher == null)
                throw new ArgumentNullException(nameof(currentMatcher));

            return int.Parse(currentMatcher.Groups.OfType<Group>().Skip(1).First().Value);
        }

        private static object[] RemoveItemsAtPositions(object[] args, ISet<int> numericArgPositions)
        {
            var result = new List<object>();
            for (int position = 0; position < args.Length; position++)
            {
                if (!numericArgPositions.Contains(position))
                {
                    result.Add(args[position]);
                }
            }
            return result.ToArray();
        }
    }
}
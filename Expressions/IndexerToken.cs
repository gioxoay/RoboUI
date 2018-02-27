using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RoboUI.Filters;

namespace RoboUI.Expressions
{
    internal class IndexerToken : IMemberAccessToken
    {
        public IndexerToken(IEnumerable<object> arguments)
        {
            Arguments = new ReadOnlyCollection<object>(arguments.ToArray());
        }

        public IndexerToken(params object[] arguments)
            : this((IEnumerable<object>)arguments)
        {
        }

        public ReadOnlyCollection<object> Arguments { get; }
    }
}

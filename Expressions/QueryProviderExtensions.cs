using System.Linq;

namespace RoboUI.Expressions
{
    public static class QueryProviderExtensions
    {
        public static bool IsLinqToObjectsProvider(IQueryProvider provider)
        {
            return provider.GetType().FullName.Contains("EnumerableQuery");
        }
    }
}

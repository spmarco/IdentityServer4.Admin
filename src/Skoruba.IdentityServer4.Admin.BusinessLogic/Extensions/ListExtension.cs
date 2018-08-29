using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Skoruba.IdentityServer4.Admin.BusinessLogic.Extensions
{
    public static class ListExtension
    {
        public static void Map<TFirst, TSecond, TKey>(this IEnumerable<TFirst> list,
                                                      IEnumerable<TSecond> child,
                                                      Func<TFirst, TKey> firstKey,
                                                      Func<TSecond, TKey> secondKey,
                                                      Action<TFirst, IEnumerable<TSecond>> addChildren,
                                                      Action<TFirst> inicializar = null)
        {
            var childMap = child.GroupBy(secondKey).ToDictionary(g => g.Key, g => g.AsEnumerable());
            var stackTrace = new StackTrace();

            Parallel.ForEach(list, item =>
            {
                inicializar?.Invoke(item);

                if (!childMap.Any()) return;
                IEnumerable<TSecond> children;

                var first = firstKey(item);

                if (first != null && childMap.TryGetValue(first, out children))
                    addChildren(item, children);
            });
        }

        public static IList<T> Clone<T>(this IList<T> listToClone) where T : ICloneable
        {
            return listToClone.Select(item => (T)item.Clone()).ToList();
        }
    }
}

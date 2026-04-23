using System;
using System.Linq;


namespace ADTOSharp.Extensions
{
    /// <summary>
    /// 将 IEnumerable 转成 OrderedDictionary（保留顺序）
    /// </summary>
    public static class OrderedDictionaryExtensions
    {
        /// <summary>
        /// 将 IEnumerable 转成 OrderedDictionary（保留顺序）
        /// </summary>
        public static System.Collections.Generic.OrderedDictionary<TKey, TValue> ToOrderedDictionary<T, TKey, TValue>(
            this System.Collections.Generic.IEnumerable<T> source,
            Func<T, TKey> keySelector,
            Func<T, TValue> valueSelector)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));
            if (keySelector == null) throw new ArgumentNullException(nameof(keySelector));
            if (valueSelector == null) throw new ArgumentNullException(nameof(valueSelector));

            var list = source.ToList(); // 保证多次迭代顺序一致
            var dict = new System.Collections.Generic.OrderedDictionary<TKey, TValue>(list.Count);

            foreach (var item in list)
            {
                dict[keySelector(item)] = valueSelector(item);
            }

            return dict;
        }
    }
}

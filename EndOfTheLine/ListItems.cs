using System.Collections.Generic;

namespace EndOfTheLine
{
    static class ListItems
    {
        internal static T PreviousItemOrDefault<T>(IList<T> lineCollection, T first)
        {
            var prevIndex = lineCollection.IndexOf(first) - 1;
            return prevIndex >= 0 ? lineCollection[prevIndex] : default(T);
        }
    }
}
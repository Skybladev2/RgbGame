using System.Collections.Generic;

public static class Extensions
{
    public static void AddRange<T1, T2>(this Dictionary<T1, T2> dictionary, Dictionary<T1, T2> other)
    {
        foreach (var item in other)
        {
            dictionary.Add(item.Key, item.Value);
        }
    }

}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Magnets
{
    public class BoolInverterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return (!(bool)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return (!(bool)value);
        }
    }
    public static class Helpers
    {
        public static IEnumerable<TValue> All<TKey, TValue>(this MultiValueDictionary<TKey,TValue> dictonary, TKey key)
        {
            if (dictonary.ContainsKey(key))
            {
                return dictonary[key];
            }else
            {
                return Enumerable.Empty<TValue>();
            }
        }
        public static string ToHex(this byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
    }
}

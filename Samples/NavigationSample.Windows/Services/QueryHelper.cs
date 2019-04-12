using System;
using System.Collections.Generic;
using System.Text;

namespace NavigationSample.Windows.Services
{
    public class QueryHelper
    {
        public static string ToQueryString(IEnumerable<KeyValuePair<string, object>> keyValuePairs)
        {
            var sb = new StringBuilder();
            sb.Append("?");
            bool first = true;
            foreach (var navigationParameter in keyValuePairs)
            {
                if (!first)
                {
                    sb.Append("&");
                }
                else
                {
                    first = false;
                }
                sb.Append(navigationParameter.Key);
                sb.Append('=');
                var value = navigationParameter.Value != null ? navigationParameter.Value.ToString() : "";
                sb.Append(Uri.EscapeDataString(value));
            }
            var queryString = sb.ToString();
            return queryString;
        }

        public static Dictionary<string, string> FromQueryString(string url)
        {
            var result = new Dictionary<string, string>();
            var splits = url.Split("?");
            if (splits.Length == 2)
            {
                var queryString = splits[1];
                var keyValuePairs = queryString.Split("&");
                foreach (var keyValuePair in keyValuePairs)
                {
                    var KeyAndValue = keyValuePair.Split("=");
                    if (KeyAndValue.Length == 2)
                    {
                        result.Add(KeyAndValue[0], Uri.UnescapeDataString(KeyAndValue[1]));
                    }
                }
            }
            return result;
        }
    }
}

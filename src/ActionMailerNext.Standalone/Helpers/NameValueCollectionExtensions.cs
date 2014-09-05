using System.Collections.Specialized;
using System.Text;
using System.Web;

namespace ActionMailerNext.Standalone.Helpers
{
    public static class NameValueCollectionExtensions
    {
        public static string ToQueryString(this NameValueCollection collection, bool includeQuestionmark = true, bool splitCommas = true)
        {
            var stringBuilder = new StringBuilder();
            foreach (string k in collection.Keys)
            {
                var entries = splitCommas ? collection[k].Split(',') : new[] { collection[k] };
                foreach (var value in entries)
                {
                    stringBuilder.AppendFormat("&{0}={1}", HttpUtility.UrlEncode(k), HttpUtility.UrlEncode(value));
                }
            }

            var queryString = stringBuilder.ToString();
            if (queryString.Length == 0)
            {
                return string.Empty;
            }

            return (includeQuestionmark ? "?" : string.Empty) + queryString.Substring(1);
        }
    }
}

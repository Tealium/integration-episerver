using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using log4net;

namespace Tealium.EPiServerTagManagement.Business.Extensions
{
    public static class StringExtensions
    {
        public const char SemicolonDelimiter = ';';
        public const char ColonDelimiter = ':';

        private static ILog log = LogManager.GetLogger(typeof(StringExtensions));

        public static bool IsNotNullOrEmpty(this string value)
        {
            return !string.IsNullOrWhiteSpace(value);
        }

        public static bool IsNullOrEmpty(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static Dictionary<string, string> TagsStringToDictionary(this string value)
        {
            var result = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(value))
            {
                return new Dictionary<string, string>();
            }

            var properties = value.Trim(SemicolonDelimiter).Trim().Split(SemicolonDelimiter);
            foreach (var property in properties)
            {
                try
                {
                    var keyValue = property.Split(ColonDelimiter);
                    result.Add(keyValue[0], keyValue[1]);
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    log.ErrorFormat(CultureInfo.InvariantCulture, "[UTAG] {0}", ex);
                }
                catch (IndexOutOfRangeException ex)
                {
                    log.ErrorFormat(CultureInfo.InvariantCulture, "[UTAG] {0}", ex);
                }
            }

            return result;
        }

        public static IEnumerable<KeyValuePair<string, string>> TagsStringToKeyValuePairs(this string value)
        {
            return value.TagsStringToDictionary().Select(x => new KeyValuePair<string, string>(x.Key, x.Value));
        }

        public static Dictionary<string, string> TagsCollectionToDictionary(this IEnumerable<string> collection, bool replaceSpaces = false)
        {
            var result = new Dictionary<string, string>();

            if (collection == null || !collection.Any())
            {
                return new Dictionary<string, string>();
            }

            foreach (var property in collection)
            {
                try
                {
                    var keyValue = property.Split(ColonDelimiter);
                    if (replaceSpaces)
                    {
                        result.Add(keyValue[0].Replace(' ', '_'), keyValue[1].Replace(' ', '_'));
                    }
                    else
                    {
                        result.Add(keyValue[0], keyValue[1]);
                    }
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    log.ErrorFormat(CultureInfo.InvariantCulture, "[UTAG] {0}", ex);
                }
                catch (IndexOutOfRangeException ex)
                {
                    log.ErrorFormat(CultureInfo.InvariantCulture, "[UTAG] {0}", ex);
                }
            }

            return result;
        }

        public static string TagsDictionaryToString(this Dictionary<string, string> tags)
        {
            StringBuilder result = new StringBuilder();

            foreach (var item in tags)
            {
                result.Append(string.Format(CultureInfo.InvariantCulture, "{0}:{1}", item.Key, item.Value));
                result.Append(SemicolonDelimiter);
            }

            return result.ToString().Trim(SemicolonDelimiter);
        }

        public static string TagsDictionaryToString(this IEnumerable<KeyValuePair<string, string>> tags)
        {
            return tags.ToDictionary(x => x.Key, x => x.Value).TagsDictionaryToString();
        }

        public static string ToJsonFormat(this object source)
        {
            if (source == null)
            {
                return "\"\"";
            }

            if (source is IEnumerable && !(source is string) && !(source is IEnumerable<char>))
            {
                return "[" +
                       string.Join(",", ((IEnumerable) source).Cast<object>().Select(x => "\"" + x.ToString() + "\"")) +
                       "]";
            }

            return "\"" + source + "\"";
        }
    }
}

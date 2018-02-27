using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace RoboUI.Extensions
{
    internal static class RoboUIExtensions
    {
        public static string ToHtmlString(this TagBuilder tagBuilder)
        {
            var stringWriter = new System.IO.StringWriter();
            tagBuilder.WriteTo(stringWriter, HtmlEncoder.Default);
            return stringWriter.ToString();
        }

        public static string ToHtmlString(this IHtmlContent htmlContent)
        {
            var stringWriter = new System.IO.StringWriter();
            htmlContent.WriteTo(stringWriter, HtmlEncoder.Default);
            return stringWriter.ToString();
        }

        public static string RemoveBetween(this string str, char begin, char end)
        {
            var regex = new Regex(string.Format("\\{0}.*?\\{1}", begin, end));
            return regex.Replace(str, string.Empty);
        }

        public static bool IsNullOrEmpty(this Dictionary<string, string> dictionary)
        {
            return dictionary == null || dictionary.Count == 0;
        }

        public static string JsonSerialize(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static string JsEncode(this string value, bool appendQuotes = true)
        {
            if (value == null)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();

            if (appendQuotes)
            {
                sb.Append("\"");
            }

            foreach (char c in value)
            {
                switch (c)
                {
                    case '\"': sb.Append("\\\""); break;
                    case '\\': sb.Append("\\\\"); break;
                    case '\b': sb.Append("\\b"); break;
                    case '\f': sb.Append("\\f"); break;
                    case '\n': sb.Append("\\n"); break;
                    case '\r': sb.Append("\\r"); break;
                    case '\t': sb.Append("\\t"); break;
                    default:
                        var i = (int)c;
                        if (i < 32 || i > 127)
                        {
                            sb.AppendFormat("\\u{0:X04}", i);
                        }
                        else { sb.Append(c); }
                        break;
                }
            }

            if (appendQuotes)
            {
                sb.Append("\"");
            }

            return sb.ToString();
        }

        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("request");

            if (request.Headers != null)
                return request.Headers["X-Requested-With"] == "XMLHttpRequest";
            return false;
        }

        /// <summary>
        /// Converts a string that has been HTML-encoded for HTTP transmission into a decoded string.
        /// </summary>
        /// <param name="s">The string to decode.</param>
        /// <returns>A decoded string</returns>
        public static string HtmlDecode(this string s)
        {
            return WebUtility.HtmlDecode(s);
        }

        /// <summary>
        /// Converts a string to an HTML-encoded string.
        /// </summary>
        /// <param name="s">The string to encode.</param>
        /// <returns>An encoded string.</returns>
        public static string HtmlEncode(this string s)
        {
            return WebUtility.HtmlEncode(s);
        }
    }
}
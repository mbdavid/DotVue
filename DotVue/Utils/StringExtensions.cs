using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Newtonsoft.Json.Linq;

namespace DotVue
{
    internal static class StringExtensions
    {
        public static string EncodeJavascript(this string str, char stringDefinition = '\'')
        {
            return (str ?? "")
                .Replace("\\", "\\\\")
                .Replace("\r", "\\r")
                .Replace("\n", "\\n")
                .Replace(stringDefinition.ToString(), "\\" + stringDefinition);
        }

        public static string MD5(this string input)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                var data = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder();

                for (var i = 0; i < data.Length; i++)
                {
                    sb.Append(data[i].ToString("x2"));
                }

                return sb.ToString();
            }
        }

        public static string ToCamelCase(this string str)
        {
            if (str.Length < 2) return str.ToLower();

            return str[0].ToString().ToLower() +
                str.Substring(1);
        }
    }
}

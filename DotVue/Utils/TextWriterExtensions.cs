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
    internal static class TextWriterExtensions
    {
        public static void WriteFormat(this TextWriter writer, string format, params object[] args)
        {
            writer.Write(string.Format(format, args));
        }
    }
}

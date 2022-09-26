using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using assignment1;

namespace app1
{
    internal static class Program
    {
        private static readonly Dictionary<string, Table> Converters = new()
        {
            { ".json", new JsonConverter() },
            { ".html", new HtmlConverter() },
            { ".csv", new CsvConverter() },
            { ".md", new MdConverter() }
        };
        public static void Main(string[] args)
        {
            var options = OptionParser.GetInstance(args);
            options.DisplayMeta();
            string iExt = Path.GetExtension(options.Input), oExt = Path.GetExtension(options.Output);
            if(!Converters.ContainsKey(iExt))
                throw new ArgumentException($"{iExt} is not a supported input format");
            if (!Converters.ContainsKey(oExt))
                throw new ArgumentException($"{oExt} is not a supported input format");
            File.WriteAllText(options.Output!, Converters[oExt].Deserialise(
                Converters[iExt].Serialise(File.OpenText(options.Input!).ReadToEnd().Trim())));
        }
    }
}
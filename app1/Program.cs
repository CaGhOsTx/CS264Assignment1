using System;
using System.Collections.Generic;
using System.IO;

namespace app1;
internal static class Program
{
    private static readonly Dictionary<string, Table> Converters = new()
    {
        { ".json", JsonConverter.Instance},
        { ".html", HtmlConverter.Instance },
        { ".csv", CsvConverter.Instance },
        { ".md", MdConverter.Instance }
    };
    public static void Main(string[] args)
    {
        var parser = ArgumentParser.Instance(args);
        parser.ExecuteFlags();
        string inputExtension = Path.GetExtension(parser.Input), outputExtension = Path.GetExtension(parser.Output);
        if(!Converters.ContainsKey(inputExtension))
            throw new ArgumentException($"{inputExtension} is not a supported input format");
        if (!Converters.ContainsKey(outputExtension))
            throw new ArgumentException($"{outputExtension} is not a supported output format");
        File.WriteAllText(parser.Output!, 
            Converters[outputExtension].Deserialise(
                Converters[inputExtension].Serialise(File.OpenText(parser.Input!).ReadToEnd().Trim())
            )
        );
    }
}
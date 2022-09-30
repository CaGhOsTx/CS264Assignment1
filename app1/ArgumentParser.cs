using System;
using System.Collections.Generic;


public class ArgumentParser
{
    private static ArgumentParser _instance;
    private readonly byte _flags;
    public string Input { get; }
    public string Output { get; }

    private static readonly Dictionary<string, Flag> Map = new()
    {
        { "-v", Flag.Verbose },
        { "--verbose", Flag.Verbose },
        { "-h", Flag.Help },
        { "--help", Flag.Help },
        { "-i", Flag.Info },
        { "--info", Flag.Info },
        { "-o", Flag.Output },
        { "--output", Flag.Output },
        { "-l", Flag.List},
        { "--list-formats", Flag.List}
    };

    private enum Flag
    {
        Output = 1 << 0, 
        Verbose = 1 << 1, 
        Info = 1 << 2, 
        Help = 1 << 3, 
        List = 1 << 4
    }
    
    public bool Verbose => Active(Flag.Verbose);

    public static ArgumentParser Instance(IEnumerable<string> args) =>_instance ??= new ArgumentParser(args);
    
    private ArgumentParser(IEnumerable<string> args)
    {
        Flag last = 0;
        foreach (var s in args)
        {
            if (s.StartsWith("--output"))
                Output = s.Split('=')[1];
            else if (Map.ContainsKey(s))
            {
                var f = Map[s];
                _flags |= (byte) f;
                last = f;
            }
            else if (last == Flag.Output)
                Output = s;
            else if (Input == null) Input = s;
            else throw new ArgumentException(s + " is not a valid argument");
        }
        if (Input == null && Output == null) return;
        if(!Active(Flag.Output))
            throw new ApplicationException("No output file specified");
        if (Input == null)
            throw new ApplicationException("No input file specified");
    }

    public void ExecuteFlags()
    {
        if (Active(Flag.Info))
            Console.WriteLine("TableConverter\nVersion 1.0\nAuthor: Carlos Milkovic");
        if (Active(Flag.Help))
            Console.WriteLine(
                "HELP:\n" +
                "This program is used to convert between table formats.\n" +
                "flags:\n" +
                "\t verbose mode: -v, --verbose\n" +
                "\t specify output: -o <file>, --output=<file>\n" +
                "\t list formats: -l, --list-formats\n" +
                "\t show help: -h, --help\n" +
                "\t show version information: -i, --info"
            );
        if(Active(Flag.List))
            Console.WriteLine("Supported formats: \n\t-CSV\n\t-JSON\n\t-MD\n\t-HTTP");
        Table.Debug = Active(Flag.Verbose);
    }

    private bool Active(Flag flag)
    {
        return (_flags & (byte) flag) > 0;
    }
}
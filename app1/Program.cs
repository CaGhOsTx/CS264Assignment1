using System;
using System.Collections.Generic;
using System.IO;

namespace assignment1
{
    internal class Program
    {

        public static void Main(string[] args)
        {
            var options = OptionParser.GetInstance(args);
            options.DisplayMeta();
            new JsonTable().fromString(File.OpenText(options.Input).ReadToEnd());
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace app1
{
    public sealed class JsonConverter : Table
    {
        private static JsonConverter _instance;
        public static JsonConverter Instance => _instance ??= new JsonConverter();
        public override IndexedTable Serialise(string s)
        {
            if(s == null) throw new ArgumentNullException(nameof(s));
            s = s.Replace("\r", "");
            if(!s.StartsWith("[") || !s.EndsWith("]"))
                throw new FormatException("JSON should be an array of objects");
            var table = new Dictionary<string, SortedDictionary<int, string>>();
            var stack = new Stack<char>();
            int row = 0, min = int.MaxValue, max = int.MinValue;
            for (var i = 0; i < s.Length; i++)
                switch (s[i])
                {
                    //bracket validity check
                    case '[':
                    case '{':
                    {
                        if(stack.Count != 0 && stack.Peek() == '{') 
                            throw new FormatException("Invalid character at position " + i + ", Array and object values are not supported");
                        stack.Push(s[i]);
                        row++;
                        break;
                    }
                    case ']' when stack.Count != 0 && stack.Peek() == '[':
                        stack.Pop();
                        break;
                    case ']':
                        throw new FormatException("Invalid character at position " + i);
                    case '}' when stack.Count != 0 && stack.Peek() == '{':
                        stack.Pop();
                        break;
                    case '}':
                        throw new FormatException("Invalid character at position " + i);
                    case ',' when stack.Count == 0:
                        throw new FormatException("Invalid character at position " + i);
                    //copy values
                    case '\"':
                    {
                        var key = ParseKey(s, ref i);
                        if (!table.ContainsKey(key))
                        {
                            min = Math.Min(min, row);
                            table[key] = new SortedDictionary<int, string>();
                        }
                        var val = ParseValue(s, ref i);
                        max = Math.Max(max, row);
                        table[key][row] = val;
                        if (s[i] == '}') i--;
                        break;
                    }
                }
            return new IndexedTable(table, min, max);
        }

        private static string ParseValue(string s, ref int i)
        {
            var buf = new StringBuilder();
            var quotes = 0;
            while (s[i] != ',' && s[i] != '}')
            {
                if (s[i] == '\"')
                {
                    i++;
                    quotes++;
                }
                else buf.Append(s[i++]);
            }
            if (quotes != 2 && quotes != 0) throw new FormatException("Invalid quoting at position " + i);
            var val = buf.ToString().Trim();
            return val;
        }

        private static string ParseKey(string s, ref int i)
        {
            var buf = new StringBuilder();
            i++;
            while (s[i] != '\"') buf.Append(s[i++]);
            if (s[++i] != ':') throw new FormatException("Invalid character at position " + i);
            var key = buf.ToString().Trim();
            i++;
            return key;
        }

        protected override string WriteCell(string key, string value)
        {
            var qKey = key.StartsWith("\"") && key.EndsWith("\"");
            if (value == null) return $"{(qKey? key : $"\"{key}\"")}:\"undefined\"";
            var qVal = value.StartsWith("\"") && value.EndsWith("\"");
            return $"{(qKey? key : $"\"{key}\"")}:{(qVal? value : $"\"{value}\"")}".Replace("\r", "");
        }

        protected override string TableHeader(IEnumerable<string> headers) => string.Empty;

        protected override string RowPrefix() => "{\n\t\t";

        protected override string RowSuffix() => "\n\t}";
        protected override string TablePrefix() => "[\n\t";
        protected override string TableSuffix() => "\n]";

        private JsonConverter() : base(",\n\t\t", ",\n\t", true) {}
    }
    
    
}
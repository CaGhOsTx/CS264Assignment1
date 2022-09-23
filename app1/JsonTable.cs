using System;
using System.Collections.Generic;
using System.Text;

namespace assignment1
{
    public class JsonTable : Table
    {
        public Dictionary<string, Dictionary<int, string>> fromString(string s)
        {
            if(!s.StartsWith("[") || !s.EndsWith("]"))
            {
                throw new FormatException("JSON should be an array of objects");
            }
            
            var table = new Dictionary<string, Dictionary<int, string>>();
            var stack = new Stack<char>();
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                //bracket validity check
                if (c == '[' || c == '{')
                {
                    if(stack.Count != 0 && stack.Peek() == '{') 
                        throw new FormatException("Invalid character at position " + i + ", Array and object values are not supported");
                    stack.Push(c);
                }
                else if (c == ']')
                {
                    if (stack.Count != 0 && stack.Peek() == '[') stack.Pop();
                    else throw new FormatException("Invalid character at position " + i);
                }
                else if (c == '}')
                {
                    if (stack.Count != 0 && stack.Peek() == '{') stack.Pop(); 
                    else throw new FormatException("Invalid character at position " + i);
                }
                else if (c == ',' && stack.Count == 0) throw new FormatException("Invalid character at position " + i);
                //copy values
                else if (c == '\"')
                {
                    var buf = new StringBuilder();
                    i++;
                    while (s[i] != ',' && s[i] != '\"' && s[i] != '}')buf.Append(s[i++]);
                    if (s[i] == '\"') i++;
                    var key = buf.ToString().Trim();
                    if (!table.ContainsKey(key)) table[key] = new Dictionary<int, string>();
                    if(s[i] != '}' && s[i] != ':') throw new FormatException("Invalid character at position " + i + "( " + s[i] + " )");
                    buf = new StringBuilder();
                    var val = "";
                    table[key][stack.Count] = val;
                    while (s[i] != ',' && s[i] != '\"' && s[i] != '}') buf.Append(s[i++]);
                    val = buf.ToString().Trim();
                    Console.WriteLine(key + " " + val);
                    if (s[i] == '}') i--;
                }
            }
            return table;
        }

        public string toTable(Dictionary<string, Dictionary<int, string>> table)
        {
            return "";
        }
    }
}
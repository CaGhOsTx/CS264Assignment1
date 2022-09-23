using System.Collections.Generic;

namespace assignment1
{
    public interface Table
    {
        Dictionary<string, Dictionary<int, string>> fromString(string s);
        string toTable(Dictionary<string, Dictionary<int, string>> table);
    }
}
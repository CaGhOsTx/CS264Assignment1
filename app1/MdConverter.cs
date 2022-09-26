using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using assignment1;

namespace app1;

public class MdConverter : Table
{
    public MdConverter() : base('|', '\n')
    {
    }

    private bool NotWhiteSpace(string s)
    {
        return !(string.IsNullOrEmpty(s) || s.Trim().Length == 0);
    }

    public override IndexedTable Serialise(string s)
    {
        s = s.Replace("\r", "");
        if (s == null) throw new ArgumentNullException(nameof(s));
        var table = new Dictionary<string, SortedDictionary<int, string>>();
        var rows = s.Split('\n').ToList();
        var headers = rows[0].Split(Delimiter).Where(NotWhiteSpace).Select(x => x.Trim()).ToList();
        headers.ForEach(h => table[h] = new SortedDictionary<int, string>());
        if (rows[1].Split(Delimiter).Where(NotWhiteSpace).Any(token => !Regex.Match(token, "-{3,}").Success))
            throw new FormatException("Invalid Markdown file, headers not aligned");
        for (var i = 2; i < rows.Count; i++)
        {
            var cells = rows[i].Split(Delimiter).Where(NotWhiteSpace).ToList();
            for (var j = 0; j < headers.Count; j++)
                table[headers[j]][i] = j < cells.Count ? cells[j] != "" ? cells[j].Trim() : null : null;
        }

        return new IndexedTable(table, 2, rows.Count - 1);
    }

    protected override string WriteCell(string key, string value)
    {
        return value ?? " ";
    }

    protected override string RowPrefix(IndexedTable table)
    {
        return "|";
    }

    protected override string RowSuffix(IndexedTable table)
    {
        return "|";
    }

    protected override string TablePrefix(IndexedTable table)
    {
        return Delimiter + table.Data.Keys.Aggregate((a, b) => a + Delimiter + b) + Delimiter + RowDelimiter
               + Delimiter + Enumerable.Repeat("---", table.Data.Keys.Count)
                   .Aggregate((a, b) => a + Delimiter + b) + Delimiter + RowDelimiter;
    }

    protected override string TableSuffix(IndexedTable table)
    {
        return "";
    }
}
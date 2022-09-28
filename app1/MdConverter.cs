using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace app1;

public sealed class MdConverter : Table
{
    private static MdConverter _instance;
    public static MdConverter Instance => _instance ??= new MdConverter();

    private MdConverter() : base("|", "\n") {}

    private static bool NotWhiteSpace(string s) => !(string.IsNullOrEmpty(s) || s.Trim().Length == 0);

    public override IndexedTable Serialise(string s)
    {
        s = s.Replace("\r", "");
        if (s == null) throw new ArgumentNullException(nameof(s));
        var table = new Dictionary<string, SortedDictionary<int, string>>();
        var rows = s.Split('\n').ToList();
        var headers = rows[0].Split(new[] {Delimiter}, StringSplitOptions.RemoveEmptyEntries).Where(NotWhiteSpace).Select(x => x.Trim()).ToList();
        headers.ForEach(h => table[h] = new SortedDictionary<int, string>());
        if (rows[1].Split(new[] {Delimiter}, StringSplitOptions.RemoveEmptyEntries)
            .Where(NotWhiteSpace)
            .Any(token => !Regex.Match(token, "-{3,}").Success)
        ) throw new FormatException("Invalid Markdown file, headers not aligned");
        for (var i = 2; i < rows.Count; i++)
        {
            var cells = rows[i].Split(new[] {Delimiter}, StringSplitOptions.RemoveEmptyEntries ).Where(NotWhiteSpace).ToList();
            for (var j = 0; j < headers.Count; j++)
                table[headers[j]][i] = j < cells.Count ? cells[j] != "" ? cells[j].Trim() : null : null;
        }
        return new IndexedTable(table, 2, rows.Count - 1);
    }
    
    protected override string WriteCell(string key, string value) => value ?? string.Empty;
    protected override string TableHeader(IEnumerable<string> headers)
    {
        var l = headers.ToList();
        return l.Aggregate((a, b) => a + Delimiter + b) + Delimiter + RowDelimiter + HeaderSeparator(l);
    }

    private string HeaderSeparator(ICollection<string> l)
        => Delimiter + Enumerable.Repeat("---", l.Count)
            .Aggregate((a, b) => a + Delimiter + b) + Delimiter + RowDelimiter;

    protected override string RowPrefix() => Delimiter;

    protected override string RowSuffix() => Delimiter;

    protected override string TablePrefix() => Delimiter;

    protected override string TableSuffix() => string.Empty;
}
using System;
using System.Collections.Generic;
using System.Linq;

public sealed class CsvConverter : Table
{
    private static CsvConverter _instance;
    public static CsvConverter Instance => _instance ??= new CsvConverter();
    public override IndexedTable Serialise(string s)
    {
        if(s == null) throw new ArgumentNullException(nameof(s));
        s = s.Replace("\r", "");
        var table = new Dictionary<string, SortedDictionary<int, string>>();
        var rows = s.Split('\n');
        var headers = rows[0].Split(',').Select(h => h.Trim()).ToList();
        InitRowMaps(headers, table);
        ParseCells(rows, headers, table);
        return new IndexedTable(table, 1, rows.Length);
    }

    private static void InitRowMaps(List<string> headers, IDictionary<string, SortedDictionary<int, string>> table)
    {
        headers.ForEach(h => table[h] = new SortedDictionary<int, string>());
    }

    private static void ParseCells(string[] rows, List<string> headers, Dictionary<string, SortedDictionary<int, string>> table)
    {
        for (var i = 1; i < rows.Length; i++)
        {
            var cells = rows[i].Split(',');
            for (var j = 0; j < headers.Count; j++)
                table[headers[j]][i] = j < cells.Length ? cells[j] != "" ? cells[j] : null : null;
        }
    }

    protected override string WriteCell(string key, string value) => value ?? string.Empty;
    protected override string TableHeader(IEnumerable<string> headers) 
        => headers.Aggregate((a, b) => a + Delimiter + b) + RowDelimiter;
    protected override string RowPrefix() => string.Empty;
    protected override string RowSuffix() => string.Empty;
    protected override string TablePrefix() => string.Empty;
    protected override string TableSuffix() => string.Empty;

    private CsvConverter() : base(",", "\n") { }
}


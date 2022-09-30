using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;


public sealed class HtmlConverter : Table
{
    private static HtmlConverter _instance;
    private class HtmlCell
    {
        public enum HtmlTag { Td, Th };

        private static HtmlTag Determine(string tag)
        {
            if (tag.StartsWith("<td>") && tag.EndsWith("</td>"))
                return HtmlTag.Td;
            if (tag.StartsWith("<th>") && tag.EndsWith("</th>"))
                return HtmlTag.Th;
            throw new ArgumentException("Invalid tag");
        }

        public string Value { get; }
        public HtmlTag Tag { get; }

        public HtmlCell(string value)
        {
            Console.WriteLine(value);
            this.Tag = Determine(value);
            this.Value = value.Substring(4, value.Length - 9);
        }
    }
        
    private HtmlConverter() : base("\n\t\t", "\n\t") { }
        
    public static HtmlConverter Instance => _instance ??= new HtmlConverter();

    public override IndexedTable Serialise(string s)
    {
        s = Regex.Replace(s, @"\s+", "");
        if(!s.StartsWith("<table>") || !s.EndsWith("</table>"))
            throw new FormatException("Invalid HTML table");
        var regex = Regex.Match(s, "<tr>(<t[dh]>.+?</t[dh]>)+?</tr>", RegexOptions.Singleline);
        var tmp = new List<CaptureCollection>();
        while (regex.Success)
        {
            tmp.Add(regex.Groups[1].Captures);
            regex = regex.NextMatch();
        }
        var table = new Dictionary<string, SortedDictionary<int, string>>();
        var rows = tmp.Select(row => (from object value in row 
                    select new HtmlCell(Regex.Replace(value.ToString(), @"\s+", "")))
                .ToList())
            .ToList();
        if(rows[0].Any(cell => cell.Tag != HtmlCell.HtmlTag.Th))
            throw new FormatException("Invalid HTML table");
        rows[0].ForEach(header => table[header.Value] = new SortedDictionary<int, string>());
        var i = 1;
        for (;i < rows.Count; i++)
        {
            var row = rows[i];
            if (row.Any(cell => cell.Tag != HtmlCell.HtmlTag.Td))
                throw new FormatException("Invalid HTML table at " + i);
            if (row.Count != table.Count)
                throw new FormatException("Invalid HTML table at " + i);
            var j = 0;
            foreach (var cell in row)
            {
                table[table.Keys.ElementAt(j)][i] = cell.Value;
                j++;
            }
        }
        return new IndexedTable(table, 1, i);
    }

    protected override string WriteCell(string key, string value) => $"<td>{value ?? ""}</td>";
    protected override string TableHeader(IEnumerable<string> headers)
        => RowPrefix() 
           + headers.Select(h => $"<th>{h}</th>")
               .Aggregate((a, b) => a + Delimiter +  b)
           + RowSuffix() 
           + RowDelimiter;

    protected override string RowPrefix() => "<tr>" + Delimiter;

    protected override string RowSuffix() => RowDelimiter + "</tr>";

    protected override string TablePrefix() => "<table>" + RowDelimiter;

    protected override string TableSuffix() => "\n</table>";
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text.RegularExpressions;

namespace app1
{
    public class HtmlConverter : Table
    {
        private class HtmlCell
        {
            public enum Tag { Td, Th };

            public static Tag Determine(string tag)
            {
                if (tag.StartsWith("<td>") && tag.EndsWith("</td>"))
                    return HtmlCell.Tag.Td;
                if (tag.StartsWith("<th>") && tag.EndsWith("</th>"))
                    return HtmlCell.Tag.Th;
                throw new ArgumentException("Invalid tag");
            }

            public string Value { get; }
            public Tag tag { get; }

            public HtmlCell(string value)
            {
                Console.WriteLine(value);
                this.tag = Determine(value);
                this.Value = value.Substring(4, value.Length - 9);
            }
        }
        
        public HtmlConverter() : base('\n', '\t')
        {
        }

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
            if(rows[0].Any(cell => cell.tag != HtmlCell.Tag.Th))
                throw new FormatException("Invalid HTML table");
            rows[0].ForEach(header => table[header.Value] = new SortedDictionary<int, string>());
            var i = 1;
            for (;i < rows.Count; i++)
            {
                var row = rows[i];
                if (row.Any(cell => cell.tag != HtmlCell.Tag.Td))
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
            return new IndexedTable(table, 0, i);
        }

        protected override string WriteCell(string key, string value)
        {
            return $"<td>{value ?? ""}</td>";
        }

        protected override string RowPrefix(IndexedTable table)
        {
            return "<tr>";
        }

        protected override string RowSuffix(IndexedTable table)
        {
            return "</tr>";
        }

        protected override string TablePrefix(IndexedTable table)
        {
            return "<table>";
        }

        protected override string TableSuffix(IndexedTable table)
        {
            return "</table>";
        }
    }
}


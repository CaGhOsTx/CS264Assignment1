using System;
using System.Collections.Generic;
using System.Linq;

namespace app1
{
    public class CsvConverter : Table
    {
        public override IndexedTable Serialise(string s)
        {
            if(s == null) throw new ArgumentNullException(nameof(s));
            s = s.Replace("\r", "");
            var table = new Dictionary<string, SortedDictionary<int, string>>();
            var rows = s.Split('\n');
            var headers = rows[0].Split(',').Select(x => x.Trim()).ToList();
            headers.ForEach(h => table[h] = new SortedDictionary<int, string>());
            for (var i = 1; i < rows.Length; i++)
            {
                var cells = rows[i].Split(',');
                for (var j = 0; j < headers.Count; j++)
                    table[headers[j]][i] = j < cells.Length ? cells[j] != "" ? cells[j] : null : null;
            }
            return new IndexedTable(table, 1, rows.Length);
        }
    
        protected override string WriteCell(string key, string value)
        {
            return value ?? "";
        }
    
        protected override string RowPrefix(IndexedTable table)
        {
            return "";
        }
    
        protected override string RowSuffix(IndexedTable table)
        {
            return "";
        }
    
        protected override string TablePrefix(IndexedTable table)
        {
            return table.Data.Keys.Aggregate((a, b) => a + Delimiter + b) + RowDelimiter;
        }
    
        protected override string TableSuffix(IndexedTable table)
        {
            return "";
        }
    
        public CsvConverter() : base(',', '\n')
        {
        }
    }
}


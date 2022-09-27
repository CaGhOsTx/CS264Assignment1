using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace app1
{
    public abstract class Table
    {
        protected readonly char Delimiter;
        protected readonly char RowDelimiter;

        protected Table(char delimiter, char rowDelimiter)
        {
            this.Delimiter = delimiter;
            this.RowDelimiter = rowDelimiter;
        }

        public class IndexedTable
        {
            public IndexedTable(Dictionary<string, SortedDictionary<int, string>> table, int start, int end)
            {
                this.Data = table;
                this.Start = start;
                this.End = end;
            }

            public Dictionary<string, SortedDictionary<int, string>> Data { get; }
            public int Start { get; }
            public int End { get; }
        }

        public abstract IndexedTable Serialise(string s);


        public string Deserialise(IndexedTable table)
        {
            return TablePrefix(table)
                   + Enumerable.Range(table.Start, table.End - 1)
                       .Select(row => table.Data.Keys.Select(head => 
                               WriteCell(head, table.Data[head].ContainsKey(row)
                                   ? table.Data[head][row]
                                   : null))
                           .Aggregate((a, b) => a + Delimiter + b)
                       ).Select(row => RowPrefix(table) + row + RowSuffix(table))
                       .Aggregate((a, b) => a + RowDelimiter + b)
                   + TableSuffix(table);
        }

        protected abstract string WriteCell(string key, string value);
        protected abstract string RowPrefix(IndexedTable table);
        protected abstract string RowSuffix(IndexedTable table);

        protected abstract string TablePrefix(IndexedTable table);
        protected abstract string TableSuffix(IndexedTable table);
    }
}
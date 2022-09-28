using System.Collections.Generic;
using System.Linq;

namespace app1;

public abstract class Table
{
    protected readonly string Delimiter;
    protected readonly string RowDelimiter;
    private readonly bool _filterNull;
    public static bool Debug;
    protected Table(string delimiter, string rowDelimiter, bool filterNull = false)
    {
        _filterNull = filterNull;
        Delimiter = delimiter;
        RowDelimiter = rowDelimiter;
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
        => TablePrefix() + TableHeader(table.Data.Keys)
                         + Enumerable.Range(table.Start, table.End - 1)
                             .Select(row => table.Data.Keys
                                 .Where(key => 
                                       !_filterNull
                                       || table.Data[key][row] != null
                                 ).Select(head =>
                                     WriteCell(head, table.Data[head].ContainsKey(row) 
                                         ? table.Data[head][row]
                                         : null))
                                 .Aggregate((a, b) => a + Delimiter + b)
                             ).Select(row => RowPrefix() + row + RowSuffix())
                             .Aggregate((a, b) => a + RowDelimiter + b)
                         + TableSuffix();

    protected abstract string WriteCell(string key, string value);

    protected abstract string TableHeader(IEnumerable<string> headers);
    protected abstract string RowPrefix();
    protected abstract string RowSuffix();

    protected abstract string TablePrefix();
    protected abstract string TableSuffix();
}
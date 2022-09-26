using assignment1;

namespace app1;

public class HtmlConverter : Table
{
    public HtmlConverter() : base('\n', '\t')
    {
    }

    public override IndexedTable Serialise(string s)
    {
        var table = new IndexedTable(null, 0, 0);
        // var row = new IndexedRow();
        // var cell = new IndexedCell();
        // cell.Value = s;
        // row.Cells.Add(cell);
        // table.Rows.Add(row);
        return table;
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
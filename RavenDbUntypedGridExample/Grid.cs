namespace RavenDbUntypedGridExample
{
    using System.Collections.Generic;

    public class Grid
    {
        public Grid()
        {
            this.Id = "grids/";
            this.Columns = new List<Column>();
            this.Rows = new List<Row>();
        }

        public string Id { get; set; }

        public IList<Column> Columns { get; set; }

        public IList<Row> Rows { get; set; } 
    }

    public class Column
    {
        public string Header { get; set; }

        public int Position { get; set; }
    }

    public class Row
    {
        public IList<object> Cells { get; set; }
    }
}

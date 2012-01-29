namespace RavenDbUntypedGridExample
{
    using System.Linq;

    using Raven.Abstractions.Indexing;
    using Raven.Client.Indexes;

    public class GridColumnData_ByGridAndColumn : AbstractIndexCreationTask<Grid, GridColumnData_ByGridAndColumn.IndexEntry>
    {
        public class IndexEntry
        {
            public string GridId { get; set; }

            public string ColumnHeader { get; set; }

            public object ColumnCells { get; set; }
        }
        
        public GridColumnData_ByGridAndColumn()
        {
            this.Map = grids => from grid in grids
                                from column in grid.Columns
                                from row in grid.Rows
                                select
                                    new IndexEntry
                                        {
                                            GridId = grid.Id,
                                            ColumnHeader = column.Header,
                                            ColumnCells = row.Cells[column.Position]
                                        };

            this.Reduce = results => from result in results
                                     group result by new { result.GridId, result.ColumnHeader }
                                     into g
                                     select
                                         new IndexEntry
                                             {
                                                 GridId = g.Key.GridId,
                                                 ColumnHeader = g.Key.ColumnHeader,
                                                 ColumnCells = g.Select(x => x.ColumnCells).ToList()
                                             };

            this.Store(x => x.GridId, FieldStorage.Yes);
            this.Store(x => x.ColumnHeader, FieldStorage.Yes);
            this.Store(x => x.ColumnCells, FieldStorage.Yes);
        }
    }
}

namespace RavenDbUntypedGridExample
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    using NUnit.Framework;

    using Raven.Abstractions.Linq;
    using Raven.Client;
    using Raven.Client.Document;

    [TestFixture]
    public class GridTests
    {
        private IDocumentStore documentStore;

        [TestFixtureSetUp]
        public void InitializeDocumentStore()
        {
            this.documentStore = new DocumentStore
                { Url = "http://localhost:8080", DefaultDatabase = "RavenDbUntypedGridExample" };
            this.documentStore.Initialize();

            Raven.Client.Indexes.IndexCreation.CreateIndexes(
                typeof(GridColumnData_ByGridAndColumn).Assembly, this.documentStore);
        }

        [Test]
        public void AccessingGridDataDirectlyReturnsProperlyTypedObjects()
        {
            using (var session = this.documentStore.OpenSession())
            {
                var grid = this.CreateSampleGrid();

                session.Store(grid);
                session.SaveChanges();

                grid = session.Load<Grid>(grid.Id);

                foreach (var row in grid.Rows)
                {
                    Assert.IsInstanceOf<string>(row.Cells[0]);
                    Assert.IsInstanceOf<DateTime>(row.Cells[1]);
                    Assert.IsInstanceOf<double>(row.Cells[2]);
                    Assert.IsInstanceOf<bool>(row.Cells[3]);
                }
            }
        }

        [Test]
        public void AccessingGridDataThroughTheIndexReturnsStrings()
        {
            using (var session = this.documentStore.OpenSession())
            {
                var grid = this.CreateSampleGrid();

                session.Store(grid);
                session.SaveChanges();

                foreach (var column in grid.Columns)
                {
                    var data =
                        session.Query<GridColumnData_ByGridAndColumn.IndexEntry, GridColumnData_ByGridAndColumn>().
                            SingleOrDefault(x => x.GridId == grid.Id && x.ColumnHeader == column.Header);

                    Assert.IsNotNull(data);
                    Assert.IsInstanceOf<IEnumerable<object>>(data.ColumnCells);

                    foreach (var cell in data.ColumnCells as IEnumerable<object>)
                    {
                        Assert.IsInstanceOf<string>(cell);
                    }
                }
            }
        }

        private Grid CreateSampleGrid()
        {
            var grid = new Grid();

            grid.Columns.Add(new Column { Header = "MyString", Position = 0 });
            grid.Columns.Add(new Column { Header = "MyDateTime", Position = 1 });
            grid.Columns.Add(new Column { Header = "MyDouble", Position = 2 });
            grid.Columns.Add(new Column { Header = "MyBool", Position = 3 });

            grid.Rows.Add(new Row { Cells = new List<object> { "Cat, Fish, Dog", DateTime.UtcNow, Math.PI, true }});
            grid.Rows.Add(new Row { Cells = new List<object> { "Man, Bear, Pig", DateTime.Now, Math.E, false } });

            return grid;
        }
    }
}

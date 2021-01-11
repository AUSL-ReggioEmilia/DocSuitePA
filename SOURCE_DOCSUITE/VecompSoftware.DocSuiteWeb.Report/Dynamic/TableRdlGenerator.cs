using System.Collections.Generic;

namespace VecompSoftware.DocSuiteWeb.Report.Dynamic
{
    class TableRdlGenerator
    {
        private IList<Column> _mFields;

        public IList<Column> Fields
        {
            get { return _mFields; }
            set { _mFields = value; }
        }

        public Rdl.TableType CreateTable()
        {
            var table = new Rdl.TableType
                            {
                                Name = "Table1",
                                Items = new object[]
                                            {
                                                CreateTableColumns(),
                                                CreateHeader(),
                                                CreateDetails()
                                            },
                                ItemsElementName = new[]
                                                       {
                                                           Rdl.ItemsChoiceType21.TableColumns,
                                                           Rdl.ItemsChoiceType21.Header,
                                                           Rdl.ItemsChoiceType21.Details
                                                       }
                            };
            return table;
        }

        private Rdl.HeaderType CreateHeader()
        {
            var header = new Rdl.HeaderType
                             {
                                 Items = new object[]
                                             {
                                                 CreateHeaderTableRows()
                                             },
                                 ItemsElementName = new[]
                                                        {
                                                            Rdl.ItemsChoiceType20.TableRows
                                                        }
                             };
            return header;
        }

        private Rdl.TableRowsType CreateHeaderTableRows()
        {
            var headerTableRows = new Rdl.TableRowsType {TableRow = new[] {CreateHeaderTableRow()}};
            return headerTableRows;
        }

        private Rdl.TableRowType CreateHeaderTableRow()
        {
            var headerTableRow = new Rdl.TableRowType {Items = new object[] {CreateHeaderTableCells(), "0.25in"}};
            return headerTableRow;
        }

        private Rdl.TableCellsType CreateHeaderTableCells()
        {
            var headerTableCells = new Rdl.TableCellsType {TableCell = new Rdl.TableCellType[_mFields.Count]};
            for (var i = 0; i < _mFields.Count; i++)
            {
                headerTableCells.TableCell[i] = CreateHeaderTableCell(_mFields[i]);
            }
            return headerTableCells;
        }

        private Rdl.TableCellType CreateHeaderTableCell(Column fieldColumn)
        {
            var headerTableCell = new Rdl.TableCellType { Items = new object[] { CreateHeaderTableCellReportItems(fieldColumn) } };
            return headerTableCell;
        }

        private Rdl.ReportItemsType CreateHeaderTableCellReportItems(Column fieldColumn)
        {
            var headerTableCellReportItems = new Rdl.ReportItemsType { Items = new object[] { CreateHeaderTableCellTextbox(fieldColumn) } };
            return headerTableCellReportItems;
        }

        private Rdl.TextboxType CreateHeaderTableCellTextbox(Column fieldColumn)
        {
            var headerTableCellTextbox = new Rdl.TextboxType
                                             {
                                                 Name = fieldColumn.OriginalName + "_Header",
                                                 Items = new object[]
                                                             {
                                                                 fieldColumn.DisplayName,
                                                                 CreateHeaderTableCellTextboxStyle(),
                                                                 true
                                                             },
                                                 ItemsElementName = new[]
                                                                        {
                                                                            Rdl.ItemsChoiceType14.Value,
                                                                            Rdl.ItemsChoiceType14.Style,
                                                                            Rdl.ItemsChoiceType14.CanGrow
                                                                        }
                                             };
            return headerTableCellTextbox;
        }

        private Rdl.StyleType CreateHeaderTableCellTextboxStyle()
        {
            var headerTableCellTextboxStyle = new Rdl.StyleType
                                                  {
                                                      Items = new object[]
                                                                  {
                                                                      "700",
                                                                      "14pt"
                                                                  },
                                                      ItemsElementName = new[]
                                                                             {
                                                                                 Rdl.ItemsChoiceType5.FontWeight,
                                                                                 Rdl.ItemsChoiceType5.FontSize
                                                                             }
                                                  };
            return headerTableCellTextboxStyle;
        }

        private Rdl.DetailsType CreateDetails()
        {
            var details = new Rdl.DetailsType {Items = new object[] {CreateTableRows()}};
            return details;
        }

        private Rdl.TableRowsType CreateTableRows()
        {
            var tableRows = new Rdl.TableRowsType {TableRow = new[] {CreateTableRow()}};
            return tableRows;
        }

        private Rdl.TableRowType CreateTableRow()
        {
            var tableRow = new Rdl.TableRowType {Items = new object[] {CreateTableCells(), "0.25in"}};
            return tableRow;
        }

        private Rdl.TableCellsType CreateTableCells()
        {
            var tableCells = new Rdl.TableCellsType {TableCell = new Rdl.TableCellType[_mFields.Count]};
            for (var i = 0; i < _mFields.Count; i++)
            {
                tableCells.TableCell[i] = CreateTableCell(_mFields[i]);
            }
            return tableCells;
        }

        private Rdl.TableCellType CreateTableCell(Column fieldColumn)
        {
            var tableCell = new Rdl.TableCellType {Items = new object[] {CreateTableCellReportItems(fieldColumn.OriginalName)}};
            return tableCell;
        }

        private Rdl.ReportItemsType CreateTableCellReportItems(string fieldName)
        {
            var reportItems = new Rdl.ReportItemsType {Items = new object[] {CreateTableCellTextbox(fieldName)}};
            return reportItems;
        }

        private Rdl.TextboxType CreateTableCellTextbox(string fieldName)
        {
            var textbox = new Rdl.TextboxType
                              {
                                  Name = fieldName,
                                  Items = new object[]
                                              {
                                                  "=Fields!" + fieldName + ".Value",
                                                  CreateTableCellTextboxStyle(),
                                                  true
                                              },
                                  ItemsElementName = new[]
                                                         {
                                                             Rdl.ItemsChoiceType14.Value,
                                                             Rdl.ItemsChoiceType14.Style,
                                                             Rdl.ItemsChoiceType14.CanGrow
                                                         }
                              };
            return textbox;
        }

        private Rdl.StyleType CreateTableCellTextboxStyle()
        {
            var style = new Rdl.StyleType
                            {
                                Items = new object[]
                                            {
                                                "=iif(RowNumber(Nothing) mod 2, \"AliceBlue\", \"White\")",
                                                "Left"
                                            },
                                ItemsElementName = new[]
                                                       {
                                                           Rdl.ItemsChoiceType5.BackgroundColor,
                                                           Rdl.ItemsChoiceType5.TextAlign
                                                       }
                            };
            return style;
        }

        private Rdl.TableColumnsType CreateTableColumns()
        {
            var tableColumns = new Rdl.TableColumnsType {TableColumn = new Rdl.TableColumnType[_mFields.Count]};
            for (var i = 0; i < _mFields.Count; i++)
            {
                tableColumns.TableColumn[i] = CreateTableColumn();
            }
            return tableColumns;
        }

        private Rdl.TableColumnType CreateTableColumn()
        {
            var tableColumn = new Rdl.TableColumnType {Items = new object[] {"2in"}};
            return tableColumn;
        }
    }
}

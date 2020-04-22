using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace VecompSoftware.DocSuiteWeb.Report.Dynamic
{
    public class RdlGenerator
    {
        private IList<Column> _mAllFields;
        private IList<Column> _mSelectedFields;

        public IList<Column> AllFields
        {
            get { return _mAllFields; }
            set { _mAllFields = value; }
        }

        public IList<Column> SelectedFields
        {
            get { return _mSelectedFields; }
            set { _mSelectedFields = value; }
        }

        private Rdl.Report CreateReport()
        {
            var report = new Rdl.Report
                             {
                                 Items = new object[]
                                             {
                                                 CreateDataSources(),
                                                 CreateBody(),
                                                 CreateDataSets(),
                                                 "6.5in"
                                             },
                                 ItemsElementName = new[]
                                                        {
                                                            Rdl.ItemsChoiceType37.DataSources,
                                                            Rdl.ItemsChoiceType37.Body,
                                                            Rdl.ItemsChoiceType37.DataSets,
                                                            Rdl.ItemsChoiceType37.Width
                                                        }
                             };
            return report;
        }

        private Rdl.DataSourcesType CreateDataSources()
        {
            var dataSources = new Rdl.DataSourcesType {DataSource = new[] {CreateDataSource()}};
            return dataSources;
        }

        private Rdl.DataSourceType CreateDataSource()
        {
            var dataSource = new Rdl.DataSourceType
                                 {Name = "DummyDataSource", Items = new object[] {CreateConnectionProperties()}};
            return dataSource;
        }

        private Rdl.ConnectionPropertiesType CreateConnectionProperties()
        {
            var connectionProperties = new Rdl.ConnectionPropertiesType
                                           {
                                               Items = new object[]
                                                           {
                                                               "",
                                                               "SQL"
                                                           },
                                               ItemsElementName = new[]
                                                                      {
                                                                          Rdl.ItemsChoiceType.ConnectString,
                                                                          Rdl.ItemsChoiceType.DataProvider
                                                                      }
                                           };
            return connectionProperties;
        }

        private Rdl.BodyType CreateBody()
        {
            var body = new Rdl.BodyType
                           {
                               Items = new object[]
                                           {
                                               CreateReportItems(),
                                               "1in"
                                           },
                               ItemsElementName = new[]
                                                      {
                                                          Rdl.ItemsChoiceType30.ReportItems,
                                                          Rdl.ItemsChoiceType30.Height
                                                      }
                           };
            return body;
        }

        private Rdl.ReportItemsType CreateReportItems()
        {
            var reportItems = new Rdl.ReportItemsType();
            var tableGen = new TableRdlGenerator {Fields = _mSelectedFields};
            reportItems.Items = new object[] { tableGen.CreateTable() };
            return reportItems;
        }

        private Rdl.DataSetsType CreateDataSets()
        {
            var dataSets = new Rdl.DataSetsType {DataSet = new[] {CreateDataSet()}};
            return dataSets;
        }

        private Rdl.DataSetType CreateDataSet()
        {
            var dataSet = new Rdl.DataSetType {Name = "MyData", Items = new object[] {CreateQuery(), CreateFields()}};
            return dataSet;
        }

        private Rdl.QueryType CreateQuery()
        {
            var query = new Rdl.QueryType
                            {
                                Items = new object[]
                                            {
                                                "DummyDataSource",
                                                ""
                                            },
                                ItemsElementName = new[]
                                                       {
                                                           Rdl.ItemsChoiceType2.DataSourceName,
                                                           Rdl.ItemsChoiceType2.CommandText
                                                       }
                            };
            return query;
        }

        private Rdl.FieldsType CreateFields()
        {
            var fields = new Rdl.FieldsType {Field = new Rdl.FieldType[_mAllFields.Count]};

            for (var i = 0; i < _mAllFields.Count; i++)
            {
                fields.Field[i] = CreateField(_mAllFields[i]);
            }

            return fields;
        }

        private Rdl.FieldType CreateField(Column fieldColumn)
        {
            var field = new Rdl.FieldType
                            {
                                Name = fieldColumn.OriginalName,
                                Items = new object[] { fieldColumn.OriginalName },
                                ItemsElementName = new[] {Rdl.ItemsChoiceType1.DataField}
                            };
            return field;
        }

        public void WriteXml(Stream stream)
        {
            var serializer = new XmlSerializer(typeof(Rdl.Report));
            serializer.Serialize(stream, CreateReport());
        }
    }
}

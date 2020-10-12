using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using VecompSoftware.Commons.Interfaces.DocumentGenerator.Models;
using VecompSoftware.DocSuiteWeb.Common.Exceptions;
using VecompSoftware.DocSuiteWeb.Model.DocumentGenerator.Parameters;

namespace VecompSoftware.DocSuite.Document.Generator.OpenXml.Word
{
    public class WordGeneratorBuilder
    {
        #region [ Fields ]
        #endregion

        #region [ Properties ]
        #endregion

        #region [ Constructor ]
        public WordGeneratorBuilder()
        {
        }
        #endregion

        #region [ Methods ]

        public byte[] GenerateWordDocument(byte[] content, ICollection<IDocumentGeneratorParameter> parameters)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                stream.Write(content, 0, content.Length);
                using (WordprocessingDocument doc = WordprocessingDocument.Open(stream, true))
                {
                    Body body = doc.MainDocumentPart.Document.Body;
                    IEnumerable<Text> textTokens = body.Elements<Paragraph>().SelectMany(f => f.Elements<Run>().SelectMany(x => x.Elements<Text>()));
                    IEnumerable<KeyValuePair<string, string>> parametersParsered = parameters.Select(s => ParseParameter(s));
                    Text foundedText;
                    foreach (KeyValuePair<string, string> param in parametersParsered)
                    {
                        foundedText = textTokens.SingleOrDefault(f => f.Text.Equals(param.Key));
                        if (foundedText != null)
                        {
                            foundedText.Text = foundedText.Text.Replace(param.Key, param.Value);
                        }
                    }
                }
                return stream.ToArray();
            }
        }

        public Table BuildWordTable(ICollection<IDocumentGeneratorParameter> parameters)
        {
            if (parameters == null || !parameters.Any())
            {
                return null;
            }

            Table wordTable = new Table();
            AddTableStyle(wordTable);

            IEnumerable<KeyValuePair<string, string>> tableHeaderNames = parameters.Where(x => !x.Name.Contains("_")).Select(x => ParseParameter(x));
            TableRow wordTableHeader = new TableRow();
            wordTableHeader.Append(tableHeaderNames.Select(x => new TableCell(new Paragraph(new Run(new Text(x.Value))))));
            AddTableHeaderStyle(wordTableHeader);
            wordTable.Append(wordTableHeader);

            int tableRowLength = parameters.Where(x => x.Name.Contains("_")).Count() / tableHeaderNames.Count();
            for (int tableRowIndex = 0; tableRowIndex < tableRowLength; tableRowIndex++)
            {
                TableRow wordTableRow = new TableRow();
                wordTableRow.Append(parameters.Where(x => x.Name.EndsWith($"_{tableRowIndex}")).Select(x => ParseParameter(x)).Select(x => new TableCell(new Paragraph(new Run(new Text(x.Value))))));
                wordTable.Append(wordTableRow);
            }

            return wordTable;
        }
        #endregion

        #region [ Helpers]
        private KeyValuePair<string, string> ParseParameter(IDocumentGeneratorParameter visitable)
        {
            if (visitable is BooleanParameter)
            {
                BooleanParameter b = visitable as BooleanParameter;
                return new KeyValuePair<string, string>(b.LookingTag, Convert.ToString(b.Value));
            }
            if (visitable is CharParameter)
            {
                CharParameter ch = visitable as CharParameter;
                return new KeyValuePair<string, string>(ch.LookingTag, ch.Value.ToString());
            }
            if (visitable is FloatParameter)
            {
                FloatParameter f = visitable as FloatParameter;
                return new KeyValuePair<string, string>(f.LookingTag, f.Value.ToString("00.000"));
            }
            if (visitable is GuidParameter)
            {
                GuidParameter g = visitable as GuidParameter;
                return new KeyValuePair<string, string>(g.LookingTag, g.Value.ToString());
            }
            if (visitable is IntParameter)
            {
                IntParameter i = visitable as IntParameter;
                return new KeyValuePair<string, string>(i.LookingTag, i.Value.ToString());
            }
            if (visitable is StringParameter)
            {
                StringParameter s = visitable as StringParameter;
                return new KeyValuePair<string, string>(s.LookingTag, HttpUtility.HtmlDecode(s.Value));
            }
            if (visitable is DateTimeParameter)
            {
                DateTimeParameter d = visitable as DateTimeParameter;
                return new KeyValuePair<string, string>(d.LookingTag, d.Value.ToShortDateString());
            }
            throw new DSWException($"Parameter '{visitable.GetType().Name}' is not correct", null, DSWExceptionCode.Invalid);
        }

        private void AddTableStyle(Table wordTable)
        {
            TableProperties borderProperty = new TableProperties(
                new TableBorders(
                    new TopBorder()
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                        Size = 12
                    },
                    new BottomBorder()
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                        Size = 12
                    },
                    new LeftBorder()
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                        Size = 12
                    },
                    new RightBorder()
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                        Size = 12
                    },
                    new InsideHorizontalBorder()
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                        Size = 12
                    },
                    new InsideVerticalBorder()
                    {
                        Val = new EnumValue<BorderValues>(BorderValues.Single),
                        Size = 12
                    }
                )
            );
            wordTable.AppendChild(borderProperty);
        }

        private void AddTableHeaderStyle(TableRow wordTableHeader)
        {
            foreach (TableCell tableCell in wordTableHeader.ChildElements)
            {
                Shading shading = new Shading()
                {
                    Color = "Automatic",
                    Fill = "C8C8C8", //Background color
                    Val = ShadingPatternValues.Clear
                };
                TableCellProperties tableCellProperties = new TableCellProperties();
                tableCellProperties.AppendChild(shading);
                tableCell.TableCellProperties = tableCellProperties;
            }
        }
        #endregion
    }
}

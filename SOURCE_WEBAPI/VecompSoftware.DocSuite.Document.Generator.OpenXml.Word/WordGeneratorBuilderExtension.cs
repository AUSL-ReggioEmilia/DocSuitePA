using System;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Model.DocumentGenerator;

namespace VecompSoftware.DocSuite.Document.Generator.OpenXml.Word
{
    public static class WordGeneratorBuilderExtension
    {
        public static IWordOpenXmlDocumentGenerator WordOpenXmlDocumentGenerator { get; set; }

        public static async Task<WordDocumentModel> GetLatestVersionAsync(Guid idTemplate)
        {
            return new WordDocumentModel()
            {
                Stream = await WordOpenXmlDocumentGenerator.GetLatestVersionAsync(idTemplate)
            };
        }

        public static WordDocumentModel AppendTable(this WordDocumentModel wordDocument, DocumentGeneratorModel extraContentParameter)
        {
            wordDocument.Stream = WordOpenXmlDocumentGenerator.AppendTable(extraContentParameter, wordDocument.Stream);
            return wordDocument;
        }

        public static async Task<WordDocumentModel> GenerateDocumentAsync(this WordDocumentModel wordDocument, Guid idTemplate, DocumentGeneratorModel documentGeneratorModel)
        {
            wordDocument.Stream = await WordOpenXmlDocumentGenerator.GenerateDocumentAsync(idTemplate, documentGeneratorModel, wordDocument.Stream);
            return wordDocument;
        }
    }
}

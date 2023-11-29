using System.Threading.Tasks;
using VecompSoftware.Commons.Interfaces.DocumentGenerator;

namespace VecompSoftware.DocSuite.Document.Generator.PDF
{
    public interface IPDFDocumentGenerator : IDocumentGenerator
    {
        Task<byte[]> GeneratePdfAsync(byte[] content, string filename, string signature);
    }
}
using System;
using System.Threading.Tasks;
using VecompSoftware.Commons.Interfaces.DocumentGenerator.Models;

namespace VecompSoftware.Commons.Interfaces.DocumentGenerator
{
    public interface IDocumentGenerator
    {
        Task<byte[]> GenerateDocumentAsync(Guid idTemplate, IDocumentGeneratorModel source, byte[] content = null);
        Task<byte[]> GetLatestVersionAsync(Guid idTemplate);
        byte[] AppendTable(IDocumentGeneratorModel source, byte[] content);
    }
}

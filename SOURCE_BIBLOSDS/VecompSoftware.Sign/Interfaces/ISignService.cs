using System.Collections.Generic;
using VecompSoftware.Sign.ArubaSignService;
using VecompSoftware.Sign.Models;

namespace VecompSoftware.Sign.Interfaces
{
    public interface ISignService
    {
        ICollection<FileModel> SignDocuments(ArubaSignModel signModel, IEnumerable<FileModel> documents, bool requiredMark);
        byte[] SignDocument(ArubaSignModel signModel, byte[] document, string filename, bool requiredMark);    
    }
}

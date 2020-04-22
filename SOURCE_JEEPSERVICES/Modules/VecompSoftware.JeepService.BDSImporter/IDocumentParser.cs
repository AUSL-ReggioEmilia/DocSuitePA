
namespace VecompSoftware.JeepService
{
    public interface IDocumentParser
    {

        int StackSize { get; }
        void ImportDocuments();
        void DisposeLocalCopies();

    }
}

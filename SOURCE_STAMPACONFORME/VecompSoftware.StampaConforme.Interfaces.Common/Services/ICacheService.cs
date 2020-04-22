namespace VecompSoftware.StampaConforme.Interfaces.Common.Services
{
    public interface ICacheService
    {
        byte[] FindDocument(byte[] documentContentToFind, string fileName);

        void CreateDocument(byte[] documentContent, byte[] referenceContent, string fileName);
    }
}

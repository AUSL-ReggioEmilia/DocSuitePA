namespace VecompSoftware.ServiceBus.BiblosDS
{
    public class BiblosClient
    {
        private readonly BiblosDS.DocumentsClient _document;
        private readonly BiblosDSManagement.AdministrationClient _administration;

        public BiblosDS.DocumentsClient Document => _document;

        public BiblosDSManagement.AdministrationClient Administration => _administration;

        public BiblosClient()
        {
            _document = new BiblosDS.DocumentsClient();
            _administration = new BiblosDSManagement.AdministrationClient();
        }


    }
}

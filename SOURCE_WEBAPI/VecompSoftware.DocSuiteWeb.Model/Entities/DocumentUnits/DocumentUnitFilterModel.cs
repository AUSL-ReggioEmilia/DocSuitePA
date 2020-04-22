using VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles;

namespace VecompSoftware.DocSuiteWeb.Model.Entities.DocumentUnits
{
    public class DocumentUnitFilterModel
    {
        public DocumentUnitFilterModel() { }

        public FascicleModel Fascicle { get; set; }

        public string DocumentUnitName { get; set; }

        public ReferenceType? ReferenceType { get; set; }

        public string Title { get; set; }
    }
}

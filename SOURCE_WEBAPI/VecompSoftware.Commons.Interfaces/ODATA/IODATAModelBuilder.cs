using System.Collections.Generic;

namespace VecompSoftware.Commons.Interfaces.ODATA
{
    public interface IODATAModelBuilder
    {
        ICollection<IODATAModel> MapEntityOData();
    }
}

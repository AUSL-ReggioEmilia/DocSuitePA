using System.Collections.Generic;

namespace VecompSoftware.JeepService.Common
{
    public interface IJeepParameter
    {
        void Initialize(List<Parameter> parameters);
        void DefaultInitialization();
        List<Parameter> Serialize();
    }
}

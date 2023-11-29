using System;
using VecompSoftware.DocSuiteWeb.DTO.UDS;

namespace VecompSoftware.DocSuiteWeb.Facade.NHibernate.UDS
{
    public interface IUDSInitializer
    {
        UDSDto GetUDSInitializer();
        Guid? GetIdFascicle();
    }
}

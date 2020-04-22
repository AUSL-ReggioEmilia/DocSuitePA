using System;
using System.Collections.Generic;
using VecompSoftware.DocSuiteWeb.Data.Entity.Commons;
using VecompSoftware.DocSuiteWeb.Data.NHibernate.Dao.Commons;

namespace VecompSoftware.DocSuiteWeb.Facade.NHibernate.Commons
{
    public class JeepServiceHostFacade : BaseProtocolFacade<JeepServiceHost, Guid, JeepServiceHostDao>
    {
        #region [ Fields ]
          
        #endregion [ Fields ]

        #region [ Properties ]

        #endregion [ Properties ]

        #region [ Constructor ]
        public JeepServiceHostFacade()
            : base()
        {

        }
      
        #endregion [ Constructor ]

        #region [ Methods ]

        public JeepServiceHost GetByHostName(string hostName)
        {
            return _dao.GetByHostName(hostName);
        }

        public JeepServiceHost GetDefaultHost()
        {
            return _dao.GetDefault();
        }

        public void SetDefaultHost(Guid idHost)
        {
            this.RemoveDefaultHost();
            JeepServiceHost jeepHost = base.GetById(idHost);
            jeepHost.IsDefault = true;
            _dao.UpdateOnly(ref jeepHost);
        }

        public void RemoveDefaultHost()
        {
            JeepServiceHost defaultHost = this.GetDefaultHost();
            if (defaultHost == null) return;
            defaultHost.IsDefault = false;
            _dao.UpdateOnly(ref defaultHost);
        }

        public void ActivateJeepServiceHost(IList<Guid> idHosts)
        {
            if (idHosts.Count <= 0) return;
            foreach (Guid idHost in idHosts)
            {
                this.ActivateJeepServiceHost(idHost);
            }
        }

        public void ActivateJeepServiceHost(Guid idHost)
        {
            JeepServiceHost jeepHost = base.GetById(idHost);
            jeepHost.IsActive = Convert.ToInt16(true);
            _dao.UpdateOnly(ref jeepHost);
        }

        public void DisableJeepServiceHost(IList<Guid> idHosts)
        {
            if (idHosts.Count <= 0) return;
            foreach (Guid idHost in idHosts)
            {
                this.DisableJeepServiceHost(idHost);
            }
        }

        public void DisableJeepServiceHost(Guid idHost)
        {
            JeepServiceHost jeepHost = base.GetById(idHost);
            jeepHost.IsActive = Convert.ToInt16(false);
            _dao.UpdateOnly(ref jeepHost);
        }

        public bool HostExist(Guid idHost)
        {
            return _dao.ExistHost(idHost);
        }
        #endregion [ Methods ]

    }
}

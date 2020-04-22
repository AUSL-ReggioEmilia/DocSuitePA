using System.ComponentModel;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Facade;
using VecompSoftware.JeepService.Common;
using VecompSoftware.JeepService.LogConservation.Models;

namespace VecompSoftware.JeepService.LogConservation
{
    public class LogConservationParameters : JeepParametersBase
    {
        [Category("Services")]
        [Description("Indica se abilitare il servizio di conservazione dei log di protocollo.")]
        [DefaultValue(false)]
        [LogService(LogEntityNameDefinition.PROTOCOLLOG_NAME)]
        [DisplayName("Abilita conservazione log protocollo")]
        public bool ProtocolLogServiceEnabled { get; set; }

        [Category("Services")]
        [Description("Indica se abilitare il servizio di conservazione dei log di serie documentali.")]
        [DefaultValue(false)]
        [LogService(LogEntityNameDefinition.DOCUMENTSERIESITEMLOG_NAME)]
        [DisplayName("Abilita conservazione log serie documentali")]
        public bool DocumentSeriesItemLogServiceEnabled { get; set; }

        [Category("Services")]
        [Description("Indica se abilitare il servizio di conservazione dei log di archivio.")]
        [DefaultValue(false)]
        [LogService(LogEntityNameDefinition.UDSLOG_NAME)]
        [DisplayName("Abilita conservazione log archivio")]
        public bool UDSLogServiceEnabled { get; set; }

        [Category("Services")]
        [Description("Indica se abilitare il servizio di conservazione dei log di amministrazione.")]
        [DefaultValue(false)]
        [LogService(LogEntityNameDefinition.TABLELOG_NAME)]
        [DisplayName("Abilita conservazione log amministrazione")]
        public bool TableLogServiceEnabled { get; set; }

        [Category("Services")]
        [Description("Indica se abilitare il servizio di conservazione dei log di fascicolo.")]
        [DefaultValue(false)]
        [LogService(LogEntityNameDefinition.FASCICLELOG_NAME)]
        [DisplayName("Abilita conservazione log fascicolo")]
        public bool FascicleLogServiceEnabled { get; set; }

        [Category("Services")]
        [Description("Indica se abilitare il servizio di conservazione dei log PEC.")]
        [DefaultValue(false)]
        [LogService(LogEntityNameDefinition.PECMAILLOG_NAME)]
        [DisplayName("Abilita conservazione log PEC")]
        public bool PECMailLogServiceEnabled { get; set; }

        [Category("Services")]
        [Description("Indica se abilitare il servizio di conservazione dei log di dossier.")]
        [DefaultValue(false)]
        [LogService(LogEntityNameDefinition.DOSSIERLOG_NAME)]
        [DisplayName("Abilita conservazione log dossier")]
        public bool DossierLogServiceEnabled { get; set; }

        private Location _logConservationLocation;
        internal Location LogConservationLocation
        {
            get
            {
                if (_logConservationLocation == null)
                {
                    int logConservationLocationId = DocSuiteContext.Current.ProtocolEnv.LogConservationLocation;
                    if (logConservationLocationId == -1)
                    {
                        return null;
                    }
                    _logConservationLocation = FacadeFactory.Instance.LocationFacade.GetById(logConservationLocationId);
                }
                return _logConservationLocation;
            }
        }
    }
}

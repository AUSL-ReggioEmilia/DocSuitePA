using System;
using System.Collections.Generic;
using System.Linq;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Facade.Report;

namespace VecompSoftware.JeepService
{
    class ProtJournalPrint
    {
        #region Private
        private ProtocolReport _report;
        private IList<Protocol> _protocols;
        #endregion


        #region Properties
        public IList<Protocol> Protocols
        {
            get
            {
                return _protocols ?? new List<Protocol>();
            }
            set { _protocols = value; }
        }

        public bool Emergenza { get; set; }
        public string ContainersName { get; set; }
        public ProtocolJournalLog JournalLog { get; set; }
        public string RdlcPrint { get; set; }
        public ProtocolReport Report
        {
            get { return _report ?? (_report = new ProtocolReport()); }
        }
        public int ANumero { get; private set; }
        public int DaNumero { get; private set; }

        #endregion

        #region Public
        public void DoPrint()
        {
            DaNumero = Protocols.First().Number;
            ANumero = Protocols.Last().Number;
            Report.AddRange(Protocols);
            Report.RdlcPrint = RdlcPrint;
            Report.AddParameter("Emergenza", Emergenza.ToString());
            foreach (var protocol in Protocols.Where(protocol => protocol.IdStatus != -3))
            {
                switch (protocol.IdStatus)
                {
                    case -2:
                        JournalLog.ProtocolCancelled++;
                        break;
                    case 0:
                        JournalLog.ProtocolActive++;
                        break;
                    default:
                        JournalLog.ProtocolOthers++;
                        break;
                }
                JournalLog.ProtocolTotal++;
                protocol.JournalDate = DateTime.Now;
                protocol.JournalLog = JournalLog;
            }
            JournalLog.LogDescription = JournalLog.ProtocolError != null && JournalLog.ProtocolError.Value == 0 ? "Creazione registro avvenuto correttamente" : String.Format("Protocolli non inseriti: {0}", JournalLog.LogDescription);
        }
        #endregion

    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace BiblosDS.Library.Common
{
    public partial class EventLog : Component
    {
        public EventLog()
        {
            InitializeComponent();
        }

        public EventLog(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }

    }

    public class EventLogHelper
    {
        private static volatile EventLogHelper g_UniqueInstance = null;
        private static object syncObj = new object();

        private EventLog _EventLog  ;

        private EventLogHelper()
		{
            _EventLog = new EventLog();
            _EventLog.eventLogBiblosDS = new System.Diagnostics.EventLog();
            _EventLog.eventLogBiblosDS.Log = "Application";
            _EventLog.eventLogBiblosDS.Source = "BiblosDS";
		}

        public static EventLogHelper GetUniqueInstance
        {
            get
            {
                if (g_UniqueInstance == null)
                {
                    lock (syncObj)
                    {
                        if (g_UniqueInstance == null)
                            g_UniqueInstance = new EventLogHelper();
                    }

                    return g_UniqueInstance;
                }
                else
                {
                    return g_UniqueInstance;
                }
            }
        }

        public void Write(string Message, EventLogEntryType eventLogEntryType, int IdEvent, short Category) 
        {
            _EventLog.eventLogBiblosDS.WriteEntry(Message, eventLogEntryType, IdEvent, Category) ;  
        }

        public void Write(string Message, EventLogEntryType eventLogEntryType)
        {
            _EventLog.eventLogBiblosDS.WriteEntry(Message, eventLogEntryType); 
        }
    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VecompSoftware.BiblosDS.WindowsService.WebAPI.Helpers
{
    public class ConnectionShared
    {
        public readonly static ConcurrentDictionary<string, string> Connections = new ConcurrentDictionary<string, string>();
    }
}

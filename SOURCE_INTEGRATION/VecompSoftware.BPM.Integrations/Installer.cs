using System.ComponentModel;

namespace VecompSoftware.BPM.Integrations
{
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        public Installer()
        {
            InitializeComponent();
        }

        private void RetrieveServiceName()
        {
            var serviceName = Context.Parameters["servicename"];
            if (!string.IsNullOrEmpty(serviceName))
            {
                serviceInstaller1.ServiceName = serviceName;
                serviceInstaller1.DisplayName = serviceName;
            }
        }

        public override void Install(System.Collections.IDictionary stateSaver)
        {
            RetrieveServiceName();
            base.Install(stateSaver);
        }

        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            RetrieveServiceName();
            base.Uninstall(savedState);
        }
    }
}

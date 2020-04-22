using System.ComponentModel;

namespace VecompSoftware.ServiceBus.Receiver
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
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

        public ProjectInstaller()
        {
            InitializeComponent();
        }
    }
}

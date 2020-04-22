namespace VecompSoftware.BPM.Integrations
{
    partial class Installer
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            serviceInstaller1 = new System.ServiceProcess.ServiceInstaller();
            serviceProcessInstaller1 = new System.ServiceProcess.ServiceProcessInstaller();
            // 
            // serviceInstaller1
            // 
            serviceInstaller1.DelayedAutoStart = true;
            serviceInstaller1.Description = "VecompSoftware.BPM.Integrations";
            serviceInstaller1.DisplayName = "VecompSoftware.BPM.Integrations";
            serviceInstaller1.ServiceName = "VecompSoftware.BPM.Integrations";
            serviceInstaller1.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // serviceProcessInstaller1
            // 
            serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            serviceProcessInstaller1.Password = null;
            serviceProcessInstaller1.Username = null;
            // 
            // Installer
            // 
            Installers.AddRange(new System.Configuration.Install.Installer[] {
            serviceInstaller1,
            serviceProcessInstaller1});

        }

        #endregion

        private System.ServiceProcess.ServiceInstaller serviceInstaller1;
        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller1;
    }
}
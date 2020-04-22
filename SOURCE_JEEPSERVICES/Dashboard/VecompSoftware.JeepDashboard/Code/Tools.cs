using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using VecompSoftware.JeepService.Common;
using Vecompsoftware.FileServer.Services;
using Module = VecompSoftware.JeepService.Common.Module;

namespace VecompSoftware.JeepDashboard.Code
{
    static class Tools
    {
        public static void RefreshModuleNode(TreeNode node)
        {
            var m = node.Tag as Module;
            node.ImageKey = m != null && m.Enabled ? "Module" : "Disabled";
            node.SelectedImageKey = node.ImageKey;
        }

        public static void RefreshSpoolNode(TreeNode node)
        {
            var m = node.Tag as SpoolXml;
            if (m != null) node.Text = m.Id;
        }

        public static IJeepParameter ParamBuilder(IFileRepositoryService liveUpdateClient, string assembly, string classname, List<Parameter> parameters)
        {
            IJeepParameter item;
            try
            {
                Assembly myAssembly;
                if (liveUpdateClient != null)
                {
                    var assemblyStream = liveUpdateClient.GetFile(assembly);
                    using (var ms = new MemoryStream())
                    {
                        assemblyStream.CopyTo(ms);
                        myAssembly = Assembly.Load(ms.ToArray());
                    }
                }
                else
                {
                    myAssembly = Assembly.LoadFile(assembly);
                }

                if (String.IsNullOrEmpty(assembly))
                {
                    throw new FileNotFoundException("Impossibile trovare l'assembly relativo ai parametri del modulo.", assembly);
                }

                if (String.IsNullOrEmpty(classname))
                {
                    throw new ArgumentNullException("classname", "Classe dei parametri non specificata");
                }


                var instance = myAssembly.CreateInstance(classname, false, BindingFlags.CreateInstance, null, null, null, null);
                if (instance == null || !(instance is IJeepParameter))
                {
                    throw new ArgumentException("Impossibile istanziare la classe.");
                }

                item = (IJeepParameter)instance;
                if (parameters != null && parameters.Count > 0)
                {
                    item.Initialize(parameters);
                }
                else
                {
                    item.DefaultInitialization();
                }

            }
            catch (Exception exception)
            {
                item = new EmptyJeepParameter("Errore in caricamento parametri. Verificare il valore della classe del modulo e dei parametri.", exception, classname);
            }
            return item;
        }

        public static void BackupConfiguration()
        {
            var programsFolder = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            var jeepService8ConfigFilePath = Path.Combine(programsFolder, "Vecomp Software", "JeepService8", "SERVICE", "Config", "JeepConfig.xml");
            if (!File.Exists(jeepService8ConfigFilePath)) return;
            // Creo la directory temporanea
            var jeepService8ConfigTempPath = String.Format("C:\\Temp\\Vecomp Software\\JeepService8\\Backup_{0}", DateTime.Now.ToString("yyyyMMdd"));
            Directory.CreateDirectory(jeepService8ConfigTempPath);
            // Copio il file di configurazione in una directory temporanea
            File.Copy(jeepService8ConfigFilePath, Path.Combine(jeepService8ConfigTempPath, "JeepConfig.xml"), true);
        }

        public static string FullStackTrace(Exception ex)
        {
            var currentException = ex;
            var fullStracktrace = String.Empty;
            while (currentException != null)
            {
                fullStracktrace += String.Format("\n\n{0}\n{1}", currentException.Message, currentException.StackTrace);
                currentException = currentException.InnerException;
            }
            return fullStracktrace;
        }
    }
}

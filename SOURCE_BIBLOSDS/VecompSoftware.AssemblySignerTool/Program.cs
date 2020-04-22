using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace VecompSoftware.AssemblySignerTool
{
    class Program
    {

        static void Main(string[] args)
        {
            if (Directory.Exists("Output"))
                Directory.Delete("Output", true);
            Directory.CreateDirectory("Output");

            //SignAssembly("GhostscriptSharp.dll");
            //SignAssembly("Tesseract.dll");
            SignAssembly("Magick.NET-x86.dll");

            
        }

        #region [ Constants ]

        private const string ILDASMEXECUTABLE = "ildasm.exe";
        private const string ILASMEXECUTABLE = "ilasm.exe";
        private const string STRONGNAMEKEYFILE = "BiblosDS.snk";

        #endregion

        #region [ Fields ]

        private static string _windowsSdkPath;
        private static string _dotNetFrameworkPath;

        #endregion

        #region [ Properties ]

        private static string WindowsSdkPath
        {
            get
            {
                if (string.IsNullOrEmpty(_windowsSdkPath))
                    _windowsSdkPath = SdkPathHelper.FindPathForWindowsSdk();
                return _windowsSdkPath;
            }
        }
        private static string DotNetFrameworkPath
        {
            get
            {
                if (string.IsNullOrEmpty(_dotNetFrameworkPath))
                    _dotNetFrameworkPath = SdkPathHelper.FindPathForDotNetFramework();
                return _dotNetFrameworkPath;
            }
        }

        #endregion

        #region [ Methods ]

        private static void RunCommand(string fileName, string arguments)
        {
            var startInfo = new ProcessStartInfo(fileName, arguments) { WindowStyle = ProcessWindowStyle.Maximized };
            var process = new Process() { StartInfo = startInfo };
            process.Start();
        }

        private static string Disassemble(string inputPath)
        {
            var inputFilename = Path.GetFileName(inputPath);
            var outputPath = Path.ChangeExtension(inputFilename, ".il");
            if (File.Exists(outputPath))
                File.Delete(outputPath);
            var arguments = string.Format("/all /out={0} {1}", outputPath, inputPath);
            var ildasmPath = Path.Combine(WindowsSdkPath, ILDASMEXECUTABLE);
            RunCommand(ildasmPath, arguments);
            return outputPath;
        }
        private static string Build(string inputPath)
        {
            var outputPath = string.Format(@"Output\{0}.dll", Path.GetFileNameWithoutExtension(inputPath));
            if (File.Exists(outputPath))
                File.Delete(outputPath);
            var arguments = string.Format("/dll /output={0} /key={1} {2}", outputPath, STRONGNAMEKEYFILE, inputPath);
            var ilasmPath = Path.Combine(DotNetFrameworkPath, ILASMEXECUTABLE);
            RunCommand(ilasmPath, arguments);
            return outputPath;
        }
        private static string SignAssembly(string inputPath)
        {
            var disassembled = Disassemble(inputPath);
            return Build(disassembled);
        }

        #endregion

    }
}

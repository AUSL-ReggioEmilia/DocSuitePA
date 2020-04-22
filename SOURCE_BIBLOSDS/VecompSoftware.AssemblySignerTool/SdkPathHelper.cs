using System;
using System.IO;

namespace VecompSoftware.AssemblySignerTool
{
    public static class SdkPathHelper
    {

        // ILDasm.exe will be somewhere in here
        public static string FindPathForWindowsSdk()
        {
            string[] windowsSdkPaths = new[]
            {
                @"Microsoft SDKs\Windows\v8.0A\bin\NETFX 4.0 Tools\",
                @"Microsoft SDKs\Windows\v8.0A\bin\",
                @"Microsoft SDKs\Windows\v8.0\bin\NETFX 4.0 Tools\",
                @"Microsoft SDKs\Windows\v8.0\bin\",
                @"Microsoft SDKs\Windows\v7.1A\bin\NETFX 4.0 Tools\",
                @"Microsoft SDKs\Windows\v7.1A\bin\",
                @"Microsoft SDKs\Windows\v7.0A\bin\NETFX 4.0 Tools\",
                @"Microsoft SDKs\Windows\v7.0A\bin\",
                @"Microsoft SDKs\Windows\v6.1A\bin\",        
                @"Microsoft SDKs\Windows\v6.0A\bin\",
                @"Microsoft SDKs\Windows\v6.0\bin\",
                @"Microsoft.NET\FrameworkSDK\bin"
            };

            foreach (var possiblePath in windowsSdkPaths)
            {
                string fullPath = string.Empty;

                // Check alternate program file paths as well as 64-bit versions.
                if (Environment.Is64BitProcess)
                {
                    fullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), possiblePath, "x64");
                    if (Directory.Exists(fullPath))
                    {
                        return fullPath;
                    }

                    fullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), possiblePath, "x64");
                    if (Directory.Exists(fullPath))
                    {
                        return fullPath;
                    }
                }

                fullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), possiblePath);
                if (Directory.Exists(fullPath))
                {
                    return fullPath;
                }

                fullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), possiblePath);
                if (Directory.Exists(fullPath))
                {
                    return fullPath;
                }
            }

            return null;
        }

        // ILAsm.exe will be somewhere in here
        public static string FindPathForDotNetFramework()
        {
            string[] frameworkPaths = new[]
            {
                @"Microsoft.NET\Framework\v4.0.30319",
                @"Microsoft.NET\Framework\v2.0.50727"
            };

            foreach (var possiblePath in frameworkPaths)
            {
                string fullPath = string.Empty;

                if (Environment.Is64BitProcess)
                {
                    fullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), possiblePath.Replace(@"\Framework\", @"\Framework64\"));
                    if (Directory.Exists(fullPath))
                    {
                        return fullPath;
                    }
                }

                fullPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), possiblePath);
                if (Directory.Exists(fullPath))
                {
                    return fullPath;
                }
            }

            return null;
        }

    }
}

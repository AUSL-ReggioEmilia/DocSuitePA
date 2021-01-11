using Microsoft.Build.Execution;
using Microsoft.Build.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VecompSoftware.Service.Roslyn
{
    public class CompileProject
    {
        public CompileProject()
        {
        }

        public async Task<IDictionary<string, TargetResult>> ExecuteAsync(string projectFilePath, IDictionary<string, string> BuildOptions, BasicFileLogger logger)
        {
            return await Task.Run(() =>
            {
                BuildManager manager = BuildManager.DefaultBuildManager;
                string[] targetToBuild = new string[] { "Build" };
                BuildParameters parameter = new BuildParameters()
                {
                    DetailedSummary = true,
                    Loggers = new List<ILogger>() { logger }
                };
                BuildRequestData dataRequest = new BuildRequestData(projectFilePath, BuildOptions, null, targetToBuild, null);
                BuildResult result = manager.Build(parameter, dataRequest);
                return result.ResultsByTarget;
            });
        }

        public IDictionary<string, string> BuildOption(string outputPath, CompilationType compilationType = CompilationType.Release)
        {
            if (string.IsNullOrEmpty(outputPath))
            {
                throw new Exception("OuptupPath non può essere vuoto");
            }

            IDictionary<string, string> buildOption = new Dictionary<string, string>
            {
                { "Configuration", compilationType.ToString() },
                { "OutputPath", outputPath }
            };
            return buildOption;
        }
    }
}

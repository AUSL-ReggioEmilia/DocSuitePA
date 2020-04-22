using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System.Threading.Tasks;

namespace VecompSoftware.Service.Roslyn
{
    public static class MSBuildWorkspaceExtensions
    {
        public static async Task<Solution> OpenSolutionByWorkspaceAsync(this MSBuildWorkspace workspace, string solutionPath)
        {
            return await workspace.OpenSolutionAsync(solutionPath);
        }
    }
}

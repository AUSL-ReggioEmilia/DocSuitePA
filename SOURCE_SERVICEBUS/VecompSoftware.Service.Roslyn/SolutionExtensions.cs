using Microsoft.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace VecompSoftware.Service.Roslyn
{
    public static class SolutionExtensions
    {
        public static async Task<Project> GetProjectByNameAsync(this Solution solution, string projectName)
        {
            return await Task.Run(() => solution.Projects.Where(x => x.Name.CompareTo(projectName) == 0).FirstOrDefault());
        }
    }
}

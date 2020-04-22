using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Evaluation = Microsoft.Build.Evaluation;

namespace VecompSoftware.Service.Roslyn
{
    public static class WriteDocument
    {
        public static async Task WriteToDiskAsync(Project prj, CompilationUnitSyntax syntaxToWrite, string namespaceDir, string className)
        {
            await Task.Run(() =>
            {
                DirectoryInfo baseDir = new DirectoryInfo(Path.Combine(new FileInfo(prj.FilePath).Directory.FullName, namespaceDir));
                if (!baseDir.Exists)
                {
                    baseDir.Create();
                }

                string fileName = string.Concat(className, ".", Languages.cs.ToString());
                string pathToSave = Path.Combine(baseDir.FullName, fileName);
                using (StreamWriter file = File.CreateText(pathToSave))
                {
                    file.Write(syntaxToWrite.ToString());
                    file.Flush();
                }
            });
        }

        public static async Task WriteToProjectAsync(string fileNamespace, string fileClass, Project prj, MSBuildWorkspace workspace)
        {
            await Task.Run(() =>
            {
                if (workspace.CanApplyChange(ApplyChangesKind.AddDocument))
                {
                    string evaluated = Path.Combine(fileNamespace, string.Concat(fileClass, ".", Languages.cs.ToString()));
                    Evaluation.Project projectEditor;
                    if (!new Evaluation.Project().ProjectCollection.LoadedProjects.Where(p => p.FullPath.Equals(prj.FilePath)).Any())
                    {
                        projectEditor = new Evaluation.Project(prj.FilePath);
                    }
                    else
                    {
                        projectEditor = new Evaluation.Project().ProjectCollection.LoadedProjects.Where(p => p.FullPath.Equals(prj.FilePath)).FirstOrDefault();
                    }

                    // Creo il folder
                    if (!projectEditor.GetItems(ProjectItemToInclude.Folder.ToString()).Where(p => p.EvaluatedInclude.Equals(fileNamespace)).Any())
                    {
                        projectEditor.AddItem(ProjectItemToInclude.Folder.ToString(), fileNamespace);
                    }
                    // Creo il file
                    if (!projectEditor.GetItemsByEvaluatedInclude(evaluated).Any())
                    {
                        projectEditor.AddItem(ProjectItemToInclude.Compile.ToString(), evaluated);
                    }

                    projectEditor.Save();
                }
            });
        }
    }
}

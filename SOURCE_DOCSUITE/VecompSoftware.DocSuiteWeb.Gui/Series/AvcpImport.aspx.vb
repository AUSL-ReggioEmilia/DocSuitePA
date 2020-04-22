Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Helpers
Imports System.IO
Imports VecompSoftware.Services.Biblos.Models

Namespace Series

    Public Class AvcpImport
        Inherits CommonBasePage

#Region " Fields "

        Public Const FilePathParameterKey As String = "FilePath"
        Public Const OriginalFileNameParameterKey As String = "OriginalFileName"

#End Region

#Region " Properties "

        Private ReadOnly Property TaskType As TaskTypeEnum
            Get
                Return CType([Enum].Parse(GetType(TaskTypeEnum), Request("TaskType")), TaskTypeEnum)
            End Get
        End Property

#End Region

#Region " Events "

        Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
            InitializeAjax()
        End Sub

        Private Sub cmdNew_Click(sender As Object, e As EventArgs) Handles cmdNew.Click
            Dim dir As New DirectoryInfo(ProtocolEnv.TaskTempFolder)

            For Each doc As DocumentInfo In uscDocument.DocumentInfosAdded
                Dim filename As String = FileHelper.UniqueFileNameFormat(doc.Name, DocSuiteContext.Current.User.UserName)
                Dim fileinfo As FileInfo = doc.SaveToDisk(dir, filename)

                Dim task As New TaskHeader
                task.Code = doc.Name
                task.Title = "Importazione AVCP"
                task.TaskType = Me.TaskType
                task.Status = TaskStatusEnum.Queued

                task.AddParameter(New TaskParameter(FilePathParameterKey, fileinfo.FullName))
                task.AddParameter(New TaskParameter(OriginalFileNameParameterKey, doc.Name))

                Dim facade As New TaskHeaderFacade
                facade.Save(task)
            Next

            Response.RedirectLocation = "parent"
            Response.Redirect("../Task/TaskHeaderGrid.aspx?" & CommonShared.AppendSecurityCheck(String.Format("Type=Series&TaskType={0}", Request("TaskType"))))

        End Sub

#End Region

#Region " Methods "

        Private Sub InitializeAjax()
            AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, uscDocument)
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdNew, MainPanel, MasterDocSuite.AjaxDefaultLoadingPanel)
            AjaxManager.AjaxSettings.AddAjaxSetting(cmdNew, pnlButtons, MasterDocSuite.AjaxFlatLoadingPanel)
        End Sub

#End Region

    End Class

End Namespace
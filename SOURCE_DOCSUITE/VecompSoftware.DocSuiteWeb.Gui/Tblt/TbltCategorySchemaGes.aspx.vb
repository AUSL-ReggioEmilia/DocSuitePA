Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Logging


Partial Public Class TbltCategorySchemaGes
    Inherits CommonBasePage


#Region " Fields "

    Private _categorySchema As CategorySchema
    Private _idCategorySchema As Guid
    Private Const CLOSE_WINDOW_SCRIPT As String = "CloseWindow('{0}');"
#End Region

#Region " Properties "

    Private ReadOnly Property Version As Short
    Private ReadOnly Property IdCategorySchema As Guid
        Get
            Return Request.QueryString.GetValue(Of Guid)("IdCategorySchema")
        End Get
    End Property

    Private Property CurrentCategorySchema As CategorySchema
        Get
            If _categorySchema Is Nothing Then
                If Not IdCategorySchema.Equals(Guid.Empty) Then
                    _categorySchema = Facade.CategorySchemaFacade.GetById(IdCategorySchema)
                End If
            End If
            Return _categorySchema
        End Get
        Set(value As CategorySchema)
            _categorySchema = value
        End Set
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        If Not CommonShared.HasGroupTblCategoryRight Then
            Throw New DocSuiteException("Utente non autorizzato alla visualizzazione della Versione del Classificatore.")
        End If

        InitializeAjax()
        If Not Page.IsPostBack Then
            InitializePage()
        End If
    End Sub


    Protected Sub btnConferma_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnConferma.Click
        Dim deleted As Boolean = False

        Try
            Select Case Action
                Case "Edit"
                    With CurrentCategorySchema
                        .Note = txtNewNote.Text
                    End With
                    Facade.CategorySchemaFacade.Update(CurrentCategorySchema)
                Case "Delete"
                    If Not CurrentCategorySchema.StartDate <= New DateTimeOffset(rdpNewEndDate.SelectedDate.Value, DateTimeOffset.UtcNow.Offset) Then
                        AjaxAlert("Data di Disattivazione non può essere inferiore alla Data di attivazione della Versione Corrente")
                        Exit Sub
                    End If
                    CurrentCategorySchema.EndDate = New DateTimeOffset(rdpNewEndDate.SelectedDate.Value, DateTimeOffset.UtcNow.Offset)
                    If CurrentCategorySchema.Categories IsNot Nothing AndAlso CurrentCategorySchema.Categories.Count > 0 Then
                        CreateNewSchema()
                    Else
                        Facade.CategorySchemaFacade.DeleteSchema(CurrentCategorySchema)
                    End If
            End Select

            AjaxManager.ResponseScripts.Add(String.Format(CLOSE_WINDOW_SCRIPT, Action))
        Catch ex As Exception
            FileLogger.Warn(LoggerName, ex.Message, ex)
            Dim jsScript As String = "alert('{0}');"
            jsScript = String.Format(jsScript, ex.Message)

            AjaxManager.ResponseScripts.Add(jsScript)
        End Try
    End Sub

#End Region

#Region " Methods "
    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, btnConferma, MasterDocSuite.AjaxFlatLoadingPanel)
    End Sub

    Private Sub InitializePage()
        MasterDocSuite.TitleVisible = False

        txtOldVersion.Text = CurrentCategorySchema.Version.ToString()
        txtOldVersion.ReadOnly = True
        txtOldNote.Text = CurrentCategorySchema.Note
        txtOldNote.ReadOnly = True

        Select Case Action
            Case "Edit"
                Title = "Versione del Classificatore - Modifica"
                rowNewNote.Visible = True
                txtNewNote.Text = CurrentCategorySchema.Note
                rowOldStartDate.Visible = False
                rowNewEndDate.Visible = False
            Case "Delete"
                Title = "Versione del Classificatore - Elimina"
                rdpOldStartDate.Enabled = False
                rdpOldStartDate.SelectedDate = CurrentCategorySchema.StartDate.Date
                rdpNewEndDate.MinDate = CurrentCategorySchema.StartDate.Date
                rowNewEndDate.Visible = True
                rowNewNote.Visible = False
                rowOldStartDate.Visible = True
        End Select
    End Sub

    Private Function CreateNewSchema() As Boolean
        Dim categorySchema As CategorySchema = New CategorySchema With {
            .StartDate = New DateTimeOffset(rdpNewEndDate.SelectedDate.Value, DateTimeOffset.UtcNow.Offset)
        }
        Facade.CategorySchemaFacade.CreateSchema(categorySchema)
        CurrentCategorySchema = categorySchema
        Return True
    End Function
#End Region

End Class
Imports System.Collections.Generic
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Commons
Imports System.Linq

Partial Public Class DocmFascicolo
    Inherits DocmBasePage

#Region " Properties "

    Protected ReadOnly Property Incremental() As Short
        Get
            Return Request.QueryString.GetValue(Of Short)("Incremental")
        End Get
    End Property

    Protected ReadOnly Property IncrementalObject() As String
        Get
            Return Request.QueryString("IncrementalObject")
        End Get
    End Property
    Protected ReadOnly Property Add() As String
        Get
            Return Request.QueryString("Add")
        End Get
    End Property
    Protected ReadOnly Property RefreshDocument() As String
        Get
            Return Request.QueryString("Refresh")
        End Get
    End Property

    Private Property IdRole As Integer
        Get
            Return CType(ViewState("idRole"), Integer)
        End Get
        Set(value As Integer)
            ViewState("idRole") = value
        End Set
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        InitializeAjax()

        If Not Page.IsPostBack Then
            Initialize()
        End If

        If Action.Eq("Modify") Then
            uscFascicleSelect.ShowPreviewMode()
        End If
    End Sub

    ''' <summary>
    ''' Seleziona un Fascicolo e ne visualizza l'anteprima
    ''' </summary>
    Private Sub btnSeleziona_Click(ByVal sender As Object, ByVal e As EventArgs) Handles uscFascicleSelect.FascicleSelected
        Search()
    End Sub

    ''' <summary>
    ''' Scatena l'inserimento del link di un fascicolo in una folder
    ''' </summary>
    Private Sub btnInserimento_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnInserimento.Click
        If uscFascicleSelect.SelectedFascicle IsNot Nothing Then
            Insert(uscFascicleSelect.SelectedFascicle.CalculatedLink, "LF", txtNote.Text, txtNote.Text)
        End If
    End Sub

    Private Sub btnCancella_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnCancella.Click
        Delete()
    End Sub

    Private Sub btnModifica_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnModifica.Click
        Dim selectedNode As RadTreeNode = uscDocumentFolderProt.Destination.SelectedNode()
        If selectedNode IsNot Nothing Then
            Change(selectedNode.Value, txtReason.Text, txtNote.Text)
        End If
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnModifica, btnModifica)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnCancella, btnCancella)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnInserimento, btnInserimento)
        AjaxManager.AjaxSettings.AddAjaxSetting(uscFascicleSelect, btnInserimento)
    End Sub

    Private Sub Initialize()
        Dim titolo As String = String.Empty
        Dim fullIncremental As String = String.Empty
        Dim role As Integer
        Dim roleIncremental As Integer
        DocUtil.FncCalcolaPath(role, roleIncremental, fullIncremental, titolo, CurrentDocumentYear, CurrentDocumentNumber, Incremental, True)
        IdRole = role

        pnlCartella.Visible = False
        btnInserimento.Visible = False
        btnModifica.Visible = False
        btnCancella.Visible = False

        Select Case Action
            Case "Insert"
                Title = "Inserimento Collegamento Fasicolo"
                btnInserimento.Visible = True
            Case "Modify"
                If DocSuiteContext.IsFullApplication AndAlso Add = "ON" Then
                    Title = "Modifica Collegamento Fascicolo"
                Else
                    Title = "Visualizza Collegamento Fascicolo"
                End If
                InitGraphics(uscDocumentFolderProt, pnlCartella, btnModifica, btnCancella)
        End Select

        If Not String.IsNullOrEmpty(titolo) Then
            Title &= " - " & titolo
        End If
    End Sub

    Private Sub Search()
        btnInserimento.Enabled = False

        Const description As String = "Fascicolo"

        'verifica se l'atto esiste
        If uscFascicleSelect.SelectedFascicle Is Nothing Then
            AjaxAlert(description & " inesistente")
            Exit Sub
        End If

        'verifica se collegamento è già stato inserito
        Dim sLink As String = uscFascicleSelect.SelectedFascicle.CalculatedLink
        If Action.Eq("Insert") Then
            If DocumentUtil.DocLinkVerify(Me, sLink, Incremental, CurrentDocumentYear, CurrentDocumentNumber) Then
                AjaxAlert("Collegamento " & description & "Già inserito nella Pratica")
                Exit Sub
            End If
        End If

        'Verifica sicurezza
        Dim firstCategoryWithCategoryFascicle As String() = New CategoryFascicleFacade().GetFirstIdCategoryWithProcedureCategoryFascicle(uscFascicleSelect.SelectedFascicle.Category.Id).ToArray
        AjaxAlert(description & "Mancano i diritti necessari")
        Exit Sub

        btnInserimento.Enabled = True
    End Sub

    Private Sub BindData(ByVal docmObject As DocumentObject)
        Dim link As String() = docmObject.Link.Split("|"c)
        uscFascicleSelect.TextBoxYear.Text = link(0)
        uscFascicleSelect.SelectedCategory = Facade.CategoryFacade.GetById(Int32.Parse(link(1)))
        uscFascicleSelect.TextBoxNumber.Text = link(2)
        txtReason.Text = docmObject.Reason
        txtNote.Text = docmObject.Note
    End Sub

    Private Sub Insert(ByVal link As String, ByVal objectType As String, ByVal note As String, ByVal reason As String)
        Dim _documentTokens As IList(Of DocumentToken) = Facade.DocumentTokenFacade.DocumentTokenStep(CurrentDocumentYear, CurrentDocumentNumber, IdRole)
        If _documentTokens.Count <> 1 Then
            Exit Sub
        End If

        Dim _documentObject As New DocumentObject()
        With _documentObject
            .Year = CurrentDocumentYear
            .Number = CurrentDocumentNumber
            .Incremental = Facade.DocumentObjectFacade.GetMaxId(CurrentDocumentYear, CurrentDocumentNumber)
            .IncrementalFolder = Incremental
            .OrdinalPosition = _documentObject.Incremental
            .DocStep = _documentTokens(0).DocStep
            .SubStep = _documentTokens(0).SubStep
            .idObjectType = objectType
            .Reason = reason
            .Note = note
            .idBiblos = 0
            .Link = link
        End With
        Facade.DocumentObjectFacade.Save(_documentObject)

        MasterDocSuite.AjaxManager.ResponseScripts.Add("CloseWindow('');")
        RegisterFolderRefreshParentScript()
    End Sub

    Private Sub Change(ByVal destinationFolder As String, ByVal reason As String, ByVal note As String)
        Dim documentobject As DocumentObject = Facade.DocumentObjectFacade.GetById(New YearNumberIncrCompositeKey(CurrentDocumentYear, CurrentDocumentNumber, IncrementalObject))

        Try
            Dim newIncrementalFolder As Short
            If Not String.IsNullOrEmpty(destinationFolder) Then
                newIncrementalFolder = destinationFolder
                If documentobject IsNot Nothing Then
                    If documentobject.IncrementalFolder <> newIncrementalFolder Then
                        Facade.DocumentObjectFacade.UpdateFolder(documentobject, newIncrementalFolder)
                    End If
                End If
            End If

            Facade.DocumentObjectFacade.UpdateDescription(documentobject, Nothing, "", reason, note)

            If newIncrementalFolder = 0 Then
                AjaxManager.ResponseScripts.Add("CloseWindow('" & RefreshDocument & "')")

            Else
                AjaxManager.ResponseScripts.Add("CloseWindow('')")
            End If
            RegisterFolderRefreshScript(newIncrementalFolder)
        Catch ex As Exception
            Throw New Exception("DocmProtocollo: " & ex.Message)
        End Try
    End Sub

    Private Sub InitGraphics(ByRef userControlFolder As uscDocumentFolder, ByRef pnlFolder As Panel, ByRef btnChange As Button, ByRef btnDetele As Button)
        Dim documentObject As DocumentObject = Facade.DocumentObjectFacade.GetById(New YearNumberIncrCompositeKey(CurrentDocumentYear, CurrentDocumentNumber, IncrementalObject))
        If documentObject IsNot Nothing Then
            BindData(documentObject)
            If DocSuiteContext.IsFullApplication AndAlso Add.Eq("ON") AndAlso documentObject.idObjectStatus <> "A" Then
                btnChange.Visible = True
                btnDetele.Visible = True
                txtReason.Focus()
            End If
        End If
        pnlFolder.Visible = True
        userControlFolder.Year = CurrentDocumentYear
        userControlFolder.Number = CurrentDocumentNumber
        userControlFolder.Incremental = Incremental
        userControlFolder.Document = documentObject.Document
        userControlFolder.IncrementalFolder = documentObject.IncrementalFolder
    End Sub

    Private Sub Delete()
        Dim documentObject As DocumentObject = Facade.DocumentObjectFacade.GetById(New YearNumberIncrCompositeKey(CurrentDocumentYear, CurrentDocumentNumber, IncrementalObject))
        Facade.DocumentObjectFacade.UpdateStatus(documentObject)

        RegisterFolderRefreshFullScript()
    End Sub

#End Region

End Class
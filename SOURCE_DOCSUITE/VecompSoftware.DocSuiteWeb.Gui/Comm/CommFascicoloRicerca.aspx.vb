Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data

Partial Public Class CommFascicoloRicerca
    Inherits CommBasePage

#Region " Fields "

    Dim _selectedContacts As List(Of Integer)

#End Region

#Region " Properties "

    Private ReadOnly Property SelectedContacts As List(Of Integer)
        Get
            If _selectedContacts.IsNullOrEmpty() Then
                _selectedContacts = New List(Of Integer)
                For Each contact As ContactDTO In uscContact.GetContacts(False)
                    _selectedContacts.AddRange(contact.Contact.FullIncrementalPath.Split({"|"c}, StringSplitOptions.RemoveEmptyEntries).Select(Function(stringId) Integer.Parse(stringId)))
                Next
                _selectedContacts = _selectedContacts.Distinct().ToList()
            End If
            Return _selectedContacts
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()

        If Not Page.IsPostBack Then
            btnSearch.Focus()
            pnClaim.Visible = DocSuiteContext.Current.ProtocolEnv.IsClaimEnabled
            pnlCategory.Visible = False
        End If
    End Sub

    Private Sub btnSearch_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSearch.Click
        DoSearch()
    End Sub

    Private Sub btnSearchContactCode_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSearchContactCode.Click
        If Len(txtSearchContact.Text) < 1 Then
            AjaxAlert("Impossibile effetuare la ricerca.{0}Inserire almeno un carattere.", Environment.NewLine)
            Exit Sub
        End If

        Dim contactActive As Boolean = Not CommonShared.HasGroupAdministratorRight OrElse CommonShared.HasGroupTblContactRight
        Dim contacts As IList(Of Contact) = Facade.ContactFacade.GetContactBySearchCode(txtSearchContact.Text, contactActive)
        If contacts.IsNullOrEmpty() Then
            AjaxAlert("Codice Inesistente.")
            Exit Sub
        End If
        uscContact.DataSource = Createcontactlist(contacts(0))
        uscContact.DataBind()
    End Sub

    Private Sub btnSearchContact_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnSearchContact.Click
        If Len(txtSearchContact.Text) < 2 Then
            AjaxAlert("Impossibile effetuare la ricerca.{0}Inserire almeno due caratteri.", Environment.NewLine)
            Exit Sub
        End If

        Dim contacts As IList(Of Contact) = Facade.ContactFacade.GetContactByDescription(txtSearchContact.Text, NHibernateContactDao.DescriptionSearchType.Contains, New List(Of Integer)())
        If contacts.IsNullOrEmpty() Then
            AjaxAlert("Nessun contatto trovato.")
            Exit Sub
        End If

        If contacts.Count = 1 Then
            uscContact.DataSource = Createcontactlist(contacts)
            uscContact.DataBind()
            Exit Sub
        End If

        AjaxManager.ResponseScripts.Add(uscContact.GetOpenContactWindowScript(txtSearchContact.Text))
    End Sub

    Private Sub uscCategory_CategoryAdded(ByVal sender As Object, ByVal e As EventArgs) Handles uscCategory.CategoryAdded
        pnlCategory.Visible = True
    End Sub

    Private Sub uscCategory_CategoryRemoved(ByVal sender As Object, ByVal e As EventArgs) Handles uscCategory.CategoryRemoved
        If Not uscCategory.HasSelectedCategories Then
            pnlCategory.Visible = False
        End If
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(uscCategory, pnlCategory)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSearchContactCode, uscContact)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSearchContact, uscContact)
    End Sub

    Private Function GetProtocolFinderByForm() As NHibernateProtocolFinder
        Dim protocolfinder As New NHibernateProtocolFinder()
        protocolfinder.EnableFetchMode = False
        protocolfinder.EnableTableJoin = False
        ' Data registrazione da
        protocolfinder.RegistrationDateFrom = rdpRegistrationDateFrom.SelectedDate
        ' Data registrazione a
        protocolfinder.RegistrationDateTo = rdpRegistrationDateTo.SelectedDate
        ' classificatore
        If uscCategory.HasSelectedCategories AndAlso uscCategory.SelectedCategories.Count > 0 Then
            protocolfinder.Classifications = uscCategory.SelectedCategories.First().FullIncrementalPath
            protocolfinder.IncludeChildClassifications = False
        End If

        If pnClaim.Visible AndAlso Not String.IsNullOrEmpty(rblClaim.SelectedValue) AndAlso (Int16.Parse(rblClaim.SelectedValue) < 2) Then
            protocolfinder.IsClaim = (Int16.Parse(rblClaim.SelectedValue) = 0)
        End If
        ' oggetto
        protocolfinder.ProtocolObject = txtObjectProtocol.Text.Trim()
        Select Case rblObjectSearch.SelectedValue
            Case "AND"
                protocolfinder.ProtocolObjectSearch = NHibernateBaseFinder(Of Protocol, ProtocolHeader).ObjectSearchType.AllWords
            Case "OR"
                protocolfinder.ProtocolObjectSearch = NHibernateBaseFinder(Of Protocol, ProtocolHeader).ObjectSearchType.AtLeastWord
        End Select
        ' contatti
        If Not SelectedContacts.IsNullOrEmpty() Then
            ' TODO: trasformare in vera disjunction
            Dim sqlQuery As String = String.Format(
                "(EXISTS (SELECT * FROM ProtocolContact PC WHERE PC.Year = {{alias}}.Year AND PC.Number = {{alias}}.Number AND PC.IDContact IN({0})) OR (EXISTS (SELECT * FROM ProtocolContactIssue PCI WHERE PCI.Year = {{alias}}.Year AND PCI.Number = {{alias}}.Number AND PCI.IDContact IN({0}))))",
                String.Join(",", SelectedContacts))
            protocolfinder.SQLExpressions.Remove("ProtContact")
            protocolfinder.SQLExpressions.Add("ProtContact", New SQLExpression(sqlQuery))
        End If
        ' classificazioni figlie
        protocolfinder.IncludeChildClassifications = chbCategoryChild.Checked
        ' page size
        If (CommonUtil.GetInstance().ApplyProtocolFinderSecurity(protocolfinder, SecurityType.Read, CurrentTenant.TenantAOO.UniqueId, True)) AndAlso DocSuiteContext.Current.ProtocolEnv.SearchMaxRecords <> 0 Then
            protocolfinder.PageSize = DocSuiteContext.Current.ProtocolEnv.SearchMaxRecords
        End If
        Return protocolfinder
    End Function

    Private Function GetDocumentFinderByForm() As NHibernateDocumentFinder
        Dim documentFinder As New NHibernateDocumentFinder("DocmDB")
        'Data registrazione da
        documentFinder.DocumentRegDate_From = rdpRegistrationDateFrom.SelectedDate
        'Data registrazione a
        documentFinder.DocumentRegDate_To = rdpRegistrationDateTo.SelectedDate
        'classificatore
        If uscCategory.HasSelectedCategories AndAlso uscCategory.SelectedCategories.Count > 0 Then
            documentFinder.IDCategory = uscCategory.SelectedCategories.First().FullIncrementalPath
            documentFinder.IncludeChildCategories = False
        End If
        'oggetto
        documentFinder.DocumentObject = txtObjectProtocol.Text.Trim()
        Select Case rblObjectSearch.SelectedValue
            Case "AND"
                documentFinder.DocumentObjectSearch = NHibernateBaseFinder(Of Document, DocumentHeader).ObjectSearchType.AllWords
            Case "OR"
                documentFinder.DocumentObjectSearch = NHibernateBaseFinder(Of Document, DocumentHeader).ObjectSearchType.AtLeastWord
        End Select
        'contatti
        documentFinder.DocumentContactIds = SelectedContacts
        ' classificazioni figlie
        documentFinder.IncludeChildCategories = chbCategoryChild.Checked
        ' page size
        If DocSuiteContext.Current.DocumentEnv.SearchMaxRecords <> 0 Then
            documentFinder.PageSize = DocSuiteContext.Current.DocumentEnv.SearchMaxRecords
        End If

        Return documentFinder
    End Function

    Private Function Createcontactlist(ByVal contacts As IList(Of Contact)) As IList(Of ContactDTO)
        Dim existContacts As IList(Of ContactDTO) = uscContact.GetContacts(False)
        For Each contact As Contact In contacts
            existContacts.Add(New ContactDTO(contact, ContactDTO.ContactType.Address))
        Next
        Return existContacts
    End Function

    Private Function CreateContactList(ByVal contact As Contact) As IList(Of ContactDTO)
        Dim existContacts As IList(Of ContactDTO) = uscContact.GetContacts(False)
        existContacts.Add(New ContactDTO(contact, ContactDTO.ContactType.Address))

        Return existContacts
    End Function

    Private Sub DoSearch()
        ' Finder pratiche
        Dim docmFinder As NHibernateDocumentFinder
        If CommonInstance.DocmEnabled Then
            docmFinder = GetDocumentFinderByForm()
            If Not CommonUtil.GetInstance().ApplyDocumentFinderSecurity(docmFinder, CurrentTenant.TenantAOO.UniqueId) Then
                Throw New DocSuiteException("Pratiche", "Diritti insufficienti per la ricerca nel modulo Pratiche")
            End If
            SessionSearchController.SaveSessionFinder(docmFinder, SessionSearchController.SessionFinderType.CommDocmFascFinderType)
        End If

        ' Finder protocollo
        SessionSearchController.SaveSessionFinder(GetProtocolFinderByForm(), SessionSearchController.SessionFinderType.CommProtFascFinderType)

        ' TODO: Accrocchio retrocompatibilità da eliminare
        Dim params As New StringBuilder("Type=Prot")
        If Not String.IsNullOrEmpty(tblType.SelectedValue) AndAlso Not String.IsNullOrEmpty(rblOrder.SelectedValue) Then
            params.AppendFormat("&SortBy={0}", tblType.SelectedValue)
            params.AppendFormat("&SortDirection={0}", rblOrder.SelectedValue)
        End If
        Response.Redirect(String.Format("../Comm/CommFascicoloRisultati.aspx?{0}", params))
    End Sub

#End Region

End Class
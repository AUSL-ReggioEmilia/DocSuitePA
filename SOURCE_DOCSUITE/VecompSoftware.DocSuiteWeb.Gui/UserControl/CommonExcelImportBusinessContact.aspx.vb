Imports System.Linq
Imports System.Text
Imports System.Collections.Generic
Imports System.IO
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers
Imports VecompSoftware.Helpers.ExtensionMethods
Imports Telerik.Web.UI
Imports System.Web
Imports VecompSoftware.Services.Biblos.Models

Public Class CommonExcelImportBusinessContact
    Inherits CommonBasePage

    Dim _contactsValidationError As Dictionary(Of Integer, Contact)
#Region "Properties"

    Private ReadOnly Property IdRef() As Integer?
        Get
            Return Request.QueryString.GetValueOrDefault(Of Integer?)("IdRef", Nothing)
        End Get
    End Property

    Private _contactFacade As ContactFacade

    Public ReadOnly Property ContactFacade() As ContactFacade
        Get
            If (_contactFacade Is Nothing) Then
                _contactFacade = New ContactFacade()
            End If
            Return _contactFacade
        End Get
    End Property


    Private Property ContactsToInsert As List(Of Contact)
        Get
            Dim contacts As List(Of Contact) = CType(Cache("contactsToInsert"), List(Of Contact))
            If contacts Is Nothing Then
                contacts = New List(Of Contact)()
            End If
            Return contacts
        End Get
        Set(value As List(Of Contact))
            Cache.Insert("contactsToInsert", value)
        End Set
    End Property

    Private Property ContactsValidationError As Dictionary(Of Integer, Contact)
        Get
            If _contactsValidationError Is Nothing Then
                _contactsValidationError = New Dictionary(Of Integer, Contact)()
            End If
            Return _contactsValidationError
        End Get
        Set(value As Dictionary(Of Integer, Contact))
            _contactsValidationError = value
        End Set
    End Property

    Public ReadOnly Property Filename As String
        Get
            Dim file As String = DirectCast(uscDocument.SelectedDocumentInfo, TempFileDocumentInfo).FileInfo.FullName
            If String.IsNullOrEmpty(file) Then
                Throw New DocSuiteException("Importazione Excel", "Nessun filename passato come argomento.")
            End If
            Return file
        End Get
    End Property

    Public ReadOnly Property Columns As List(Of String)
        Get
            Return New List(Of String)() From {"CodiceRicerca", "RagioneSociale", "PEC", "Cognome", "Nome", "e-mail", "CodFisc/PIVA", "TitoloStudio", "Via/Piazza/Corso", "Indirizzo", "NCivico", "CAP", "Citta", "Provincia", "Telefono", "Fax", "Note"}
        End Get
    End Property

#End Region

#Region "Events"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack() Then
            MasterDocSuite.TitleVisible = False
            AjaxManager.ResponseScripts.Add("return CommonExcelImportContactSend('importContacts');")
        End If
    End Sub

    Private Sub btnClose_Click(ByVal sender As Object, ByVal e As EventArgs)
        AjaxManager.ResponseScripts.Add("CloseWindow();")
        ClearCacheData()
    End Sub

    Private Sub btnIgnore_Click(ByVal sender As Object, ByVal e As EventArgs)
        Dim temp As String = JsonConvert.SerializeObject(ContactsToInsert)
        Dim serialized As String = HttpUtility.HtmlEncode(temp)
        Dim jsScript As String = "var jsonRes= ""{0}""; ReturnValuesJson(jsonRes, '{1}');"
        jsScript = String.Format(jsScript, serialized, True.ToString().ToLowerInvariant())
        AjaxManager.ResponseScripts.Add(jsScript)
        ClearCacheData()
    End Sub

    Protected Sub CommonExcelImportContact_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)
        Dim arg As String = e.Argument
        Select Case arg
            Case "importContacts"
                cmdImport_Click(sender, e)
            Case "btnClose"
                btnClose_Click(sender, e)
            Case "btnIgnore"
                btnIgnore_Click(sender, e)
        End Select
    End Sub
#End Region

#Region "Methods"
    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, MasterDocSuite.AjaxDefaultLoadingPanel)
        AddHandler AjaxManager.AjaxRequest, AddressOf CommonExcelImportContact_AjaxRequest
    End Sub



    Private Sub ImportContacts()

        'Leggo il file excel
        Dim excelHelper As New ExcelHelper(Filename)
        Dim dtExcel As DataTable = excelHelper.ReadFile(Columns)
        Dim toInsert As New List(Of Contact)
        ' Se l'xml è vuoto
        If dtExcel.Rows.Count = 0 Then
            Throw New DocSuiteException("Importazione Excel", "Foglio excel vuoto.")
        End If

        ' Controllo colonne mancanti
        Dim columnErrors As New StringBuilder()
        For Each column As String In Columns
            Dim columnExist As Boolean = dtExcel.Columns.Contains(column)
            If Not columnExist Then
                columnErrors.AppendFormat("La colonna [{0}] non è presente nello schema.{1}", column, Environment.NewLine)
            End If
        Next

        If columnErrors.Length > 0 Then
            Throw New DocSuiteException("Importazione Excel", columnErrors.ToString())
        End If

        Dim index As Integer = 1
        Dim parentContactId As Integer = ProtocolEnv.AVCPIdBusinessContact
        If (IdRef.HasValue) Then
            parentContactId = IdRef.Value
        End If
        Dim parentContact As Contact = ContactFacade.GetById(parentContactId)

        For Each row As DataRow In dtExcel.Rows
            Try
                index += 1
                Dim tipologia As ContactType = New ContactType(ContactType.Aoo)

                'Verifico i dati
                ValidationRow(row, index, tipologia)
                Dim descrizione As String = row.Item("RagioneSociale").ToString()

                'Inserimento
                Dim contact As New Contact()
                contact.ContactType = tipologia
                Dim nome As String = If(row.Item("Nome") Is Nothing, String.Empty, String.Concat("|", Trim$(row.Item("Nome").ToString())))
                contact.Description = If(String.IsNullOrEmpty(descrizione), Left(Trim$(row.Item("Cognome").ToString()) & nome, 60), descrizione)

                'Codice di ricerca
                Dim searchcode As String = row.Item("CodiceRicerca").ToString()
                contact.SearchCode = searchcode

                'Codice Fiscale
                contact.FiscalCode = row.Item("CodFisc/PIVA").ToString()

                'Indirizzo
                contact.Address = New Address()
                Dim contactPlaceNameString As String = row.Item("Via/Piazza/Corso").ToString()
                Dim contactPlaceNameSelected As IList(Of ContactPlaceName)
                contactPlaceNameSelected = Facade.ContactPlaceNameFacade.GetByDescription(contactPlaceNameString)
                With contact.Address
                    If (contactPlaceNameSelected.Count > 0) Then
                        .PlaceName = contactPlaceNameSelected(0)
                    End If
                    .Address = row.Item("Indirizzo").ToString()
                    .CivicNumber = row.Item("NCivico").ToString()
                    .ZipCode = row.Item("CAP").ToString()
                    .City = row.Item("Citta").ToString()
                    .CityCode = row.Item("Provincia").ToString()
                End With

                contact.TelephoneNumber = row.Item("Telefono").ToString()
                contact.FaxNumber = row.Item("Fax").ToString()
                contact.Note = row.Item("Note").ToString()
                'Gestisco gli indirizzi mail e PEC
                Dim emailAddress As String = row.Item("e-mail").ToString()
                If Not String.IsNullOrEmpty(emailAddress) Then
                    If Not RegexHelper.IsValidEmail(emailAddress) Then
                        ContactsValidationError.Add(index, contact)
                        Continue For
                    Else
                        contact.EmailAddress = emailAddress
                    End If
                End If
                Dim pecAddress As String = row.Item("PEC").ToString()
                If Not String.IsNullOrEmpty(pecAddress) Then
                    If Not RegexHelper.IsValidEmail(pecAddress) Then
                        ContactsValidationError.Add(index, contact)
                        Continue For
                    Else
                        contact.CertifiedMail = pecAddress
                    End If
                End If

                contact.Parent = parentContact
                contact.IsActive = 1

                If Facade.ContactFacade.ContactDuplicationCheck(contact) Then
                    AjaxManager.Alert("Sono presenti contatti Doppi")
                    Exit Sub
                End If

                Facade.ContactFacade.Save(contact)
                'aggiungo il contatto
                toInsert.Add(contact)
            Catch ex As DocSuiteException
                Throw New DocSuiteException("Importazione Excel", String.Format("Riga [{0}]: {1}{2}", index, ex.Titolo, Environment.NewLine))
            Catch ex As Exception
                Throw
            End Try
        Next

        If toInsert.Any() Then
            ContactsToInsert = toInsert
        End If
        If ContactsValidationError.Any() Then
            PopulateErrorValidationTable()
            Exit Sub
        End If


        Dim temp As String = JsonConvert.SerializeObject(ContactsToInsert)
        Dim serialized As String = HttpUtility.HtmlEncode(temp)
        Dim jsScript As String = "ReturnValuesJson('{0}');"
        jsScript = String.Format(jsScript, serialized)
        AjaxManager.ResponseScripts.Add(jsScript)

    End Sub

    Private Sub ValidationRow(ByVal row As DataRow, ByVal index As Integer, ByRef tipologia As ContactType)
        'Controllo dei campi obbligatori
        If Not String.IsNullOrEmpty(row.Item("Cognome").ToString()) AndAlso Not String.IsNullOrEmpty(row.Item("RagioneSociale").ToString()) Then
            Throw New DocSuiteException("Importazione Excel", String.Format("Riga [{0}]: campi 'RagioneSociale' e 'Cognome' entrambi valorizzati", index))
        ElseIf Not String.IsNullOrEmpty(row.Item("Cognome").ToString()) Then
            tipologia = New ContactType(ContactType.Person)
        ElseIf Not String.IsNullOrEmpty(row.Item("RagioneSociale").ToString()) Then
            tipologia = New ContactType(ContactType.Aoo)
        Else
            Throw New DocSuiteException("Importazione Excel", String.Format("Riga [{0}]: campo 'RagioneSociale' o 'Cognome' obbligatorio", index))
        End If
    End Sub

    Private Sub PopulateErrorValidationTable()
        errorValidationTable.Rows.AddRaw(Nothing, {4}, Nothing, {String.Format("Contatti corretti da inserire: {0}", ContactsToInsert.Count)}, {"head"})
        errorValidationTable.Rows.AddRaw(Nothing, {4}, Nothing, {String.Format("Contatti con PEC o email errati: {0}", ContactsValidationError.Count)}, {"head"})
        For Each contact As KeyValuePair(Of Integer, Contact) In ContactsValidationError
            errorValidationTable.Rows.AddRaw(Nothing, {3}, {20, 80}, {String.Format("Indice: {0}", contact.Key), String.Format("Contatto: {0}", contact.Value.FullDescription), ""}, {"label"})
        Next
    End Sub
    Private Sub cmdImport_Click(ByVal sender As Object, ByVal e As EventArgs) Handles cmdImport.Click
        ImportContacts()
    End Sub
    Private Sub ClearCacheData()
        Cache.Remove("contactsToInsert")
    End Sub
#End Region
End Class
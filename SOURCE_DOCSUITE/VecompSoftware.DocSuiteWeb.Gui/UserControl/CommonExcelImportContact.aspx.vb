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

Partial Public Class CommonExcelImportContact
    Inherits CommonBasePage

    Dim _contactsLinesValidationError As Dictionary(Of Integer, String)

#Region "Properties"
    Protected ReadOnly Property CallerId() As String
        Get
            Return Request.QueryString("ParentID")
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

    Private Property ContactsLinesValidationError As Dictionary(Of Integer, String)
        Get
            If _contactsLinesValidationError Is Nothing Then
                _contactsLinesValidationError = New Dictionary(Of Integer, String)()
            End If
            Return _contactsLinesValidationError
        End Get
        Set(value As Dictionary(Of Integer, String))
            _contactsLinesValidationError = value
        End Set
    End Property

    Public ReadOnly Property Filename As String
        Get
            Dim file As String = HttpContext.Current.Request.QueryString("FileName")
            If String.IsNullOrEmpty(file) Then
                Throw New DocSuiteException("Importazione Excel", "Nessun filename passato come argomento.")
            End If
            Return Path.Combine(CommonUtil.GetInstance().AppTempPath, file)
        End Get
    End Property

    Public ReadOnly Property Columns As List(Of String)
        Get
            Return New List(Of String)() From {"RagioneSociale", "PEC", "Cognome", "Nome", "DataNascita", "e-mail", "CodFisc/PIVA", "TitoloStudio", "Via/Piazza/Corso", "Indirizzo", "NCivico", "CAP", "Citta", "Provincia", "Telefono", "Fax", "Note"}
        End Get
    End Property

#End Region

#Region "Events"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack() Then
            MasterDocSuite.TitleVisible = False
        End If
    End Sub

    Private Sub btnClose_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnClose.Click
        AjaxManager.ResponseScripts.Add("CloseWindow();")
        ClearCacheData()
    End Sub

    Private Sub btnIgnore_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnIgnore.Click
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
                ImportContacts()
        End Select
    End Sub
#End Region

#Region "Methods"
    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, ajaxPanel, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnClose, ajaxPanel, MasterDocSuite.AjaxDefaultLoadingPanel)
        AjaxManager.AjaxSettings.AddAjaxSetting(btnIgnore, ajaxPanel, MasterDocSuite.AjaxDefaultLoadingPanel)

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

        Dim ErrorList As New List(Of String)
        Dim index As Integer = 1
        For Each row As DataRow In dtExcel.Rows
            Try
                index += 1

                Dim descrizione As String
                Dim birthDate As Date? = Nothing
                Dim tipologia As ContactType
                'Verifico i dati
                If Not ValidationRow(row, index, tipologia) Then
                    Continue For
                End If

                Select Case tipologia.Id
                    Case ContactType.Aoo
                        descrizione = HttpUtility.HtmlEncode(Left(row.Item("RagioneSociale").ToString(), 256))
                    Case ContactType.Person
                        descrizione = HttpUtility.HtmlEncode(Left(Trim$(row.Item("Cognome").ToString()) & "|" & Trim$(row.Item("Nome").ToString()), 256))
                        If (row.Item("DataNascita") IsNot DBNull.Value) Then
                            Dim dataNascita As Date
                            Date.TryParse(row.Item("DataNascita").ToString(), dataNascita)
                            If dataNascita = Date.MinValue Then
                                ContactsLinesValidationError.Add(index, String.Format("Importazione Excel Riga [{0}]: Formato della data di nascita non corretto.", index))
                                Continue For
                            End If
                            birthDate = dataNascita
                        End If
                End Select

                'Inserimento
                Dim contact As New Contact()
                contact.ContactType = tipologia
                contact.Description = descrizione
                If (birthDate.HasValue) Then
                    contact.BirthDate = birthDate.Value
                End If

                'Codice Fiscale
                contact.FiscalCode = Left(row.Item("CodFisc/PIVA").ToString(), 16)

                'Titolo di Studio
                Dim contactStudyTitleString As String = Left(row.Item("TitoloStudio").ToString(), 20)
                Dim contactStudyTitleSelected As IList(Of ContactTitle) = Facade.ContactTitleFacade.GetByDescription(contactStudyTitleString)
                If contactStudyTitleSelected.Count > 0 Then
                    contact.StudyTitle = contactStudyTitleSelected(0)
                End If

                'Indirizzo
                contact.Address = New Address()
                Dim contactPlaceNameString As String = Left(row.Item("Via/Piazza/Corso").ToString(), 15)
                Dim contactPlaceNameSelected As IList(Of ContactPlaceName)
                contactPlaceNameSelected = Facade.ContactPlaceNameFacade.GetByDescription(contactPlaceNameString)
                With contact.Address
                    If (contactPlaceNameSelected.Count > 0) Then
                        .PlaceName = contactPlaceNameSelected(0)
                    End If
                    .Address = Left(row.Item("Indirizzo").ToString(), 60)
                    .CivicNumber = Left(row.Item("NCivico").ToString(), 10)
                    .ZipCode = Left(row.Item("CAP").ToString(), 20)
                    .City = Left(row.Item("Citta").ToString(), 50)
                    .CityCode = Left(row.Item("Provincia").ToString(), 2)
                End With

                contact.TelephoneNumber = Left(row.Item("Telefono").ToString(), 50)
                contact.FaxNumber = Left(row.Item("Fax").ToString(), 50)
                contact.Note = Left(row.Item("Note").ToString(), 255)
                'Gestisco gli indirizzi mail e PEC
                Dim emailAddress As String = Left(row.Item("e-mail").ToString(), 50)
                If Not String.IsNullOrEmpty(emailAddress) Then
                    If Not RegexHelper.IsValidEmail(emailAddress) Then
                        ContactsLinesValidationError.Add(index, String.Format("Importazione Excel Riga [{0}]: Indirizzo mail non corretto.", index))
                        Continue For
                    Else
                        contact.EmailAddress = emailAddress
                    End If
                End If
                Dim pecAddress As String = Left(row.Item("PEC").ToString(), 100)
                If Not String.IsNullOrEmpty(pecAddress) Then
                    If Not RegexHelper.IsValidEmail(pecAddress) Then
                        ContactsLinesValidationError.Add(index, String.Format("Importazione Excel Riga [{0}]: Indirizzo PECMail non corretto.", index))
                        Continue For
                    Else
                        contact.CertifiedMail = pecAddress
                    End If
                End If

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
        If ContactsLinesValidationError.Any() Then
            PopulateErrorValidationTable()
            Exit Sub
        End If

        Dim temp As String = JsonConvert.SerializeObject(ContactsToInsert)
        Dim serialized As String = HttpUtility.HtmlEncode(temp)
        Dim jsScript As String = "var jsonRes= ""{0}""; ReturnValuesJson(jsonRes, '{1}');"
        jsScript = String.Format(jsScript, serialized, True.ToString().ToLowerInvariant())
        AjaxManager.ResponseScripts.Add(jsScript)

    End Sub

    Private Function ValidationRow(ByVal row As DataRow, ByVal index As Integer, ByRef tipologia As ContactType) As Boolean

        'Controllo dei campi obbligatori
        If Not String.IsNullOrEmpty(row.Item("Cognome").ToString()) AndAlso Not String.IsNullOrEmpty(row.Item("RagioneSociale").ToString()) Then
            ContactsLinesValidationError.Add(index, String.Format("Importazione Excel - Riga [{0}]: campi 'RagioneSociale' e 'Cognome' entrambi valorizzati", index))
            Return False
        ElseIf Not String.IsNullOrEmpty(row.Item("Cognome").ToString()) Then
            tipologia = New ContactType(ContactType.Person)
            Return True
        ElseIf Not String.IsNullOrEmpty(row.Item("RagioneSociale").ToString()) Then
            tipologia = New ContactType(ContactType.Aoo)
            Return True
        Else
            ContactsLinesValidationError.Add(index, String.Format("Importazione Excel Riga [{0}]: campo 'RagioneSociale' o 'Cognome' obbligatorio", index))
            Return False
        End If

    End Function

    Private Sub PopulateErrorValidationTable()
        errorValidationTable.Rows.AddRaw(Nothing, {4}, Nothing, {String.Format("Contatti corretti da inserire: {0}", ContactsToInsert.Count)}, {"head"})
        errorValidationTable.Rows.AddRaw(Nothing, {4}, Nothing, {String.Format("Contatti con PEC o email errati: {0}", ContactsLinesValidationError.Count)}, {"head"})
        For Each contact As KeyValuePair(Of Integer, String) In ContactsLinesValidationError
            errorValidationTable.Rows.AddRaw(Nothing, {3}, {20, 80}, {String.Format("Indice: {0}", contact.Key), String.Format("Contatto: {0}", contact.Value), ""}, {"label"})
        Next
    End Sub

    Private Sub ClearCacheData()
        Cache.Remove("contactsToInsert")
    End Sub

#End Region

End Class
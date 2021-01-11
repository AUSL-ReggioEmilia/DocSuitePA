Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade

Partial Public Class uscDocument
    Inherits DocSuite2008BaseControl

#Region " Fields "

    Dim _document As Document

#End Region

#Region " Properties "

    Public ReadOnly Property DocumentEnv() As DocumentEnv
        Get
            Return DocSuiteContext.Current.DocumentEnv
        End Get
    End Property

    Public Property CurrentDocument() As Document
        Get
            Return _document
        End Get
        Set(ByVal value As Document)
            _document = value
        End Set
    End Property

    Public Property VisiblePratica() As Boolean
        Get
            Return tblPratica.Visible
        End Get
        Set(ByVal value As Boolean)
            tblPratica.Visible = value
        End Set
    End Property

    Public Property VisibleGenerale() As Boolean
        Get
            Return tblGenerale.Visible
        End Get
        Set(ByVal value As Boolean)
            tblGenerale.Visible = value
        End Set
    End Property

    Public Property VisibleDate() As Boolean
        Get
            Return tblDate.Visible
        End Get
        Set(ByVal value As Boolean)
            tblDate.Visible = value
        End Set
    End Property

    Public Property VisibleContatti() As Boolean
        Get
            Return uscContatti.Visible
        End Get
        Set(ByVal value As Boolean)
            uscContatti.Visible = value
            uscContatti.ReadOnly = Not value
        End Set
    End Property

    Public Property VisibleDati() As Boolean
        Get
            Return tblDati.Visible
        End Get
        Set(ByVal value As Boolean)
            tblDati.Visible = value
        End Set
    End Property

    Public Property VisibleClassificazione() As Boolean
        Get
            Return tblClassificazione.Visible
        End Get
        Set(ByVal value As Boolean)
            tblClassificazione.Visible = value
        End Set
    End Property

    Public Property VisibleAltri() As Boolean
        Get
            Return tblAltri.Visible
        End Get
        Set(ByVal value As Boolean)
            tblAltri.Visible = value
        End Set
    End Property

    Public Property VisiblePubblicazione() As Boolean
        Get
            Return tblPubblicazione.Visible
        End Get
        Set(ByVal value As Boolean)
            tblPubblicazione.Visible = value
        End Set
    End Property

    Public Property VisiblePubblicazioneModifica() As Boolean
        Get
            Return tblPubblicazioneModifica.Visible
        End Get
        Set(ByVal value As Boolean)
            tblPubblicazioneModifica.Visible = value
        End Set
    End Property

    Public Property VisibleDateModifica() As Boolean
        Get
            Return tblDateModifica.Visible
        End Get
        Set(ByVal value As Boolean)
            tblDateModifica.Visible = value
        End Set
    End Property

    Public Property VisibleDatiModifica() As Boolean
        Get
            Return tblDatiModifica.Visible
        End Get
        Set(ByVal value As Boolean)
            tblDatiModifica.Visible = value
        End Set
    End Property

    Public Property VisibleDateSovrapposte() As Boolean
        Get
            Return tblDateSovrapposte.Visible
        End Get
        Set(ByVal value As Boolean)
            tblDateSovrapposte.Visible = value
        End Set
    End Property

    Public Property VisibleClassificatoreModifica() As Boolean
        Get
            Return tblClassificatoreModifica.Visible
        End Get
        Set(ByVal value As Boolean)
            tblClassificatoreModifica.Visible = value
        End Set
    End Property

    Public ReadOnly Property GetStartDate() As Date?
        Get
            Return txtStartDateMod.DateInput.SelectedDate
        End Get
    End Property

    Public ReadOnly Property GetExpiryDate() As Date?
        Get
            Return txtExpiryDateMod.DateInput.SelectedDate
        End Get
    End Property

    Public ReadOnly Property GetContacts() As IList(Of ContactDTO)
        Get
            Return uscContatti.GetContacts(False)
        End Get
    End Property

    Public ReadOnly Property GetServiceNumber() As String
        Get
            Return txtDocPosizione.Text
        End Get
    End Property

    Public ReadOnly Property GetName() As String
        Get
            Return txtDocNome.Text
        End Get
    End Property

    Public ReadOnly Property GetDocumentObject() As String
        Get
            Return txtDocOggetto.Text
        End Get
    End Property

    Public ReadOnly Property GetManager() As String
        Get
            Return uscManager.GetContactText()
        End Get
    End Property

    Public ReadOnly Property GetNote() As String
        Get
            Return txtDocNote.Text
        End Get
    End Property

    Public ReadOnly Property GetClassificazione() As Category
        Get
            Return uscClassificatore.SelectedCategories.FirstOrDefault()
        End Get
    End Property

    Public ReadOnly Property GetSottoClassificazione() As Category
        Get
            Return uscSottoClassificatore.SelectedCategories.FirstOrDefault()
        End Get
    End Property

    Public ReadOnly Property GetCheckPubblication() As Boolean
        Get
            Return chkPubblication.Checked
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Init(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init
        If _document Is Nothing Then
            _document = New Document()
        End If
        VisibleAltri = False
        VisibleClassificazione = False
        VisibleDate = False
        VisibleDati = False
        VisibleGenerale = False
        VisiblePratica = False
        VisibleDateModifica = False
        ' Visualizza/nasconde lo stato della pubblicazione
        VisiblePubblicazione = DocumentEnv.IsPubblicationEnabled
        uscManager.ContactRoot = DocumentEnv.ContactRoot
    End Sub

#End Region

#Region " Methods "

    Public Sub Show()
        VisibleAltri = True
        VisibleClassificazione = True
        LoadContacts()
        VisibleDate = True
        VisibleDati = True
        VisibleGenerale = True
        VisiblePratica = True

        FillControls()
        uscContatti.HideManualButtons()
        uscContatti.ReadOnly = True
        uscContatti.EnableCC = False

        lblRegistrationUser.Text = String.Format("{0} {1:dd/MM/yyyy HH.mm.ss}", CommonAD.GetDisplayName(CurrentDocument.RegistrationUser), CurrentDocument.RegistrationDate.ToLocalTime())
        If Not String.IsNullOrEmpty(CurrentDocument.LastChangedUser) AndAlso CurrentDocument.LastChangedDate.HasValue Then
            lblLastChangedUser.Text = String.Format("{0} {1:dd/MM/yyyy HH.mm.ss}", CommonAD.GetDisplayName(CurrentDocument.LastChangedUser), CurrentDocument.LastChangedDate.Value.ToLocalTime())
        End If

        ' Motivo di annullamento
        LoadStatus()
    End Sub

    ''' <summary> Compone (se il nodo ha parent) il codice completo del classificatore ( es. 1.2.3 ) </summary>
    ''' <returns>Il codice del classificatore</returns>
    Public Function ComposeClassificatoreCode() As String

        Dim sReturn As String = String.Empty

        If CurrentDocument.SubCategory IsNot Nothing Then
            sReturn = CalculateClassificatoreCode(CurrentDocument.SubCategory)
        Else
            If CurrentDocument.Category IsNot Nothing Then
                sReturn = (CurrentDocument.Category.Code.ToString())
            End If
        End If

        Return sReturn

    End Function

    ''' <summary> Funzione di supporto per comporre il codice completo del classificatore </summary>
    Private Function CalculateClassificatoreCode(ByVal subCategory As Category) As String
        ' TODO: verificare se duplicata
        Dim tmp As String = String.Empty

        Dim s As String = subCategory.Code

        If subCategory.Parent IsNot Nothing Then
            Dim cat As Category
            tmp &= s.Insert(0, ".")
            s = tmp
            cat = subCategory.Parent
            tmp = s.Insert(0, CalculateClassificatoreCode(cat))
        Else
            tmp = subCategory.Code
        End If


        Return tmp
    End Function

    ''' <summary> Compone (se il nodo ha parent) il nome completo del classificatore ( es. 1.PREVIDENZA, 2.PENSIONI ) </summary>
    ''' <returns>Il nome completo del classificatore</returns>
    Public Function ComposeClassificatoreDescription() As String

        Dim sReturn As String = String.Empty

        If CurrentDocument.SubCategory IsNot Nothing Then
            sReturn = (CalculateClassificatoreName(CurrentDocument.SubCategory))
        Else
            If CurrentDocument.Category IsNot Nothing Then
                sReturn = (CurrentDocument.Category.Name)
            End If
        End If

        Return sReturn

    End Function

    ''' <summary> Funzione di supporto per comporre il nome completo del classificatore  </summary>
    Public Function CalculateClassificatoreName(ByVal subCategory As Category) As String
        ' TODO: se lo fosse (verificare) spostare nella facade (e controllare i duplicati)
        Dim s As String = " " & subCategory.GetFullName()

        Dim tmp As String = String.Empty
        If subCategory.Parent Is Nothing Then
            tmp = subCategory.GetFullName()
        Else
            Dim cat As Category
            tmp &= s.Insert(0, ",")
            s = tmp
            cat = subCategory.Parent
            tmp = s.Insert(0, CalculateClassificatoreName(cat))
        End If

        Return tmp
    End Function

    Public Sub LoadContacts()
        If Not DocumentEnv.IsInteropEnabled Then
            uscContatti.Visible = False
            Exit Sub
        End If

        If CurrentDocument.Contacts.Count <= 0 Then
            Exit Sub
        End If

        Dim contacts As New List(Of ContactDTO)()
        For Each c As DocumentContact In CurrentDocument.Contacts
            Dim contact As New ContactDTO
            contact.Contact = c.Contact
            contact.Type = ContactDTO.ContactType.Address
            contacts.Add(contact)
        Next
        uscContatti.DataSource = contacts
        uscContatti.DataBind()
        uscContatti.Visible = True
    End Sub

    Public Sub FillControls()

        txtStartDateMod.SelectedDate = CurrentDocument.StartDate
        txtExpiryDateMod.SelectedDate = CurrentDocument.ExpiryDate
        txtDocPosizione.Text = CurrentDocument.ServiceNumber
        txtDocNome.Text = CurrentDocument.Name
        txtDocOggetto.Text = CurrentDocument.DocumentObject
        uscManager.DataSource = CurrentDocument.Manager
        txtDocNote.Text = CurrentDocument.Note

        If Not CurrentDocument.SubCategory Is Nothing Then
            uscClassificatore.DataSource.Add(CurrentDocument.SubCategory)
        ElseIf Not CurrentDocument.Category Is Nothing Then
            uscClassificatore.DataSource.Add(CurrentDocument.Category)
        End If
        uscClassificatore.DataBind()

        If DocSuiteContext.Current.DocumentEnv.IsPubblicationEnabled Then
            chkPubblication.Checked = CurrentDocument.CheckPublication = "1"
        End If
    End Sub

    ''' <summary> Imposta il layout del controllo per la parte di modifica, nascondendo i controlli di visualizzazione </summary>
    Public Sub SetLayoutPerModifica()

        VisiblePratica = True
        VisibleGenerale = True
        VisibleDateModifica = True
        VisibleContatti = True
        VisibleDate = False
        VisibleClassificazione = True
        VisibleClassificazione = False
        VisibleClassificatoreModifica = True
        VisibleDateSovrapposte = True
        VisibleDati = False
        VisibleDatiModifica = True
        VisibleAltri = False

        trCreazione.Visible = False
        trLocazione.Visible = False

        uscContatti.ReadOnly = False
        uscContatti.Caption = "Riferimento"
        uscContatti.EnableCC = False
        uscContatti.HideManualButtons()
        uscContatti.IsRequired = False

        uscClassificatore.Caption = "Classificazione"
        uscClassificatore.ReadOnly = True

        If Not CurrentDocument.SubCategory Is Nothing Then
            uscClassificatore.DataSource.Add(CurrentDocument.SubCategory)
        Else
            uscClassificatore.DataSource.Add(CurrentDocument.Category)
        End If
        uscClassificatore.DataBind()

        uscSottoClassificatore.Caption = ""
        uscSottoClassificatore.SubCategoryMode = True
        uscSottoClassificatore.Required = False
        uscSottoClassificatore.CategoryID = CurrentDocument.Category.Id

        If CurrentDocument.SubCategory IsNot Nothing Then
            uscSottoClassificatore.SubCategory = CurrentDocument.SubCategory
        End If

        ' Visualizza/nasconde lo stato della pubblicazione
        VisiblePubblicazione = False
        VisiblePubblicazioneModifica = DocumentEnv.IsPubblicationEnabled
    End Sub

    ''' <summary> Motivo di annullamento </summary>
    Public Sub LoadStatus()
        If CurrentDocument.Status IsNot Nothing AndAlso CurrentDocument.Status.Id = "PA" Then
            tblAnnullamento.Visible = True
        Else
            tblAnnullamento.Visible = False
        End If
    End Sub

#End Region

End Class
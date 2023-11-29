Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Collections.Generic
Imports Telerik.Web.UI

''' <summary> User Control per la ricerca dei Documenti all'interno della DocSuite. </summary>
Partial Public Class uscDocumentFinder
    Inherits DocSuite2008BaseControl

#Region " Fields "

    Dim _finder As NHibernateDocumentFinder = New NHibernateDocumentFinder("DocmDB")

#End Region

#Region " Properties "

    ''' <summary> Restituisce il numero di documenti trovati </summary>
    ''' <returns>Numero di documenti trovati</returns>
    Public ReadOnly Property Count() As Long
        Get
            Return _finder.Count()
        End Get
    End Property

    ''' <summary> Restituisce o imposta il numero di pagina correntemente visualizzata nella grid </summary>
    ''' <value>Numero di pagina correntemente visualizzata nella grid</value>
    ''' <returns>Numero di pagina correntemente visualizzata nella grid</returns>
    Public Property PageIndex() As Integer
        Get
            Return _finder.PageIndex
        End Get
        Set(ByVal value As Integer)
            _finder.PageIndex = value
        End Set
    End Property

    ''' <summary> Restituisce o imposta il numero di record per pagina visualizzati nella grid </summary>
    ''' <value>Numero di record per pagina visualizzati nella grid</value>
    ''' <returns>Numero di record per pagina visualizzati nella grid</returns>
    Public Property PageSize() As Integer
        Get
            Return _finder.PageSize
        End Get
        Set(ByVal value As Integer)
            _finder.PageSize = value
        End Set
    End Property

    ''' <summary> Restituisce il finder interno popolato </summary>
    Public ReadOnly Property Finder() As NHibernateBaseFinder(Of Document, DocumentHeader)
        Get
            BindData()
            Return _finder
        End Get
    End Property

    ''' <summary> Restituisce o imposta la visibilità della sezione Year </summary>
    Public Property VisibleYear() As Boolean
        Get
            Return trYear.Visible
        End Get
        Set(ByVal value As Boolean)
            trYear.Visible = value
        End Set
    End Property

    ''' <summary> Restituisce o imposta la visibilità della sezione Number </summary>
    Public Property VisibleNumber() As Boolean
        Get
            Return trNumber.Visible
        End Get
        Set(ByVal value As Boolean)
            trNumber.Visible = value
        End Set
    End Property

    ''' <summary> Restituisce o imposta la visibilità della sezione StartDate </summary>
    Public Property VisibleStartDate() As Boolean
        Get
            Return trStartDate.Visible
        End Get
        Set(ByVal value As Boolean)
            trStartDate.Visible = value
        End Set
    End Property

    ''' <summary> Restituisce o imposta la visibilità della sezione ExpiryDate </summary>
    Public Property VisibleExpiryDate() As Boolean
        Get
            Return trExpiryDate.Visible
        End Get
        Set(ByVal value As Boolean)
            trExpiryDate.Visible = value
        End Set
    End Property

    ''' <summary> Restituisce o imposta la visibilità della sezione ServiceNumber </summary>
    Public Property VisibleServiceNumber() As Boolean
        Get
            Return trServiceNumber.Visible
        End Get
        Set(ByVal value As Boolean)
            trServiceNumber.Visible = value
        End Set
    End Property

    ''' <summary> Restituisce o imposta la visibilità della sezione Name </summary>
    Public Property VisibleName() As Boolean
        Get
            Return trName.Visible
        End Get
        Set(ByVal value As Boolean)
            trName.Visible = value
        End Set
    End Property

    ''' <summary> Restituisce o imposta la visibilità della sezione ObjectD </summary>
    Public Property VisibleObjectD() As Boolean
        Get
            Return trObjectD.Visible
        End Get
        Set(ByVal value As Boolean)
            trObjectD.Visible = value
        End Set
    End Property

    ''' <summary> Restituisce o imposta la visibilità della sezione Manager </summary>
    Public Property VisibleManager() As Boolean
        Get
            Return trManager.Visible
        End Get
        Set(ByVal value As Boolean)
            trManager.Visible = value
        End Set
    End Property

    ''' <summary> Restituisce o imposta la visibilità della sezione Note. </summary>
    Public Property VisibleNote() As Boolean
        Get
            Return trNote.Visible
        End Get
        Set(ByVal value As Boolean)
            trNote.Visible = value
        End Set
    End Property

    ''' <summary> Restituisce o imposta la visibilità della sezione Contact. </summary>
    Public Property VisibleContact() As Boolean
        Get
            Return trContact.Visible
        End Get
        Set(ByVal value As Boolean)
            trContact.Visible = value
        End Set
    End Property

    ''' <summary> Restituisce o imposta la visibilità della sezione ContactChild. </summary>
    Public Property VisibleContactChild() As Boolean
        Get
            Return trContactChild.Visible
        End Get
        Set(ByVal value As Boolean)
            trContactChild.Visible = value
        End Set
    End Property

    ''' <summary> Restituisce o imposta la visibilità della sezione Container </summary>
    Public Property VisibleContainer() As Boolean
        Get
            Return trContainer.Visible
        End Get
        Set(ByVal value As Boolean)
            trContainer.Visible = value
        End Set
    End Property

    ''' <summary> Restituisce o imposta la visibilità della sezione Roles </summary>
    Public Property VisibleRoles() As Boolean
        Get
            Return trRoles.Visible
        End Get
        Set(ByVal value As Boolean)
            trRoles.Visible = value
        End Set
    End Property

    ''' <summary> Restituisce o imposta la visibilità della sezione Category </summary>
    Public Property VisibleCategory() As Boolean
        Get
            Return trCategory.Visible
        End Get
        Set(ByVal value As Boolean)
            trCategory.Visible = value
        End Set
    End Property

    ''' <summary> Restituisce o imposta la visibilità della sezione CategoryChild </summary>
    Public Property VisibleCategoryChild() As Boolean
        Get
            If trCategoryChild.Style("display") = "none" Then
                Return False
            Else
                Return True
            End If
            'Return _hasCategorySearch
        End Get
        Set(ByVal value As Boolean)
            If value = True Then
                trCategoryChild.Style.Add("display", "")
            Else
                trCategoryChild.Style.Add("display", "none")
            End If
            trCategoryChild.Visible = value
        End Set
    End Property


    ''' <summary> Restituisce o imposta la visibilità della sezione DocumentDescription </summary>
    Public Property VisibleDocumentDescription() As Boolean
        Get
            Return trDocumentDescription.Visible
        End Get
        Set(ByVal value As Boolean)
            trDocumentDescription.Visible = value
        End Set
    End Property

    ''' <summary> Restituisce o imposta la visibilità della sezione DocumentDate </summary>
    Public Property VisibleDocumentDate() As Boolean
        Get
            Return trDocumentDate.Visible
        End Get
        Set(ByVal value As Boolean)
            trDocumentDate.Visible = value
        End Set
    End Property

    ''' <summary> Restituisce o imposta la visibilità della sezione DocumentObject </summary>
    Public Property VisibleDocumentObject() As Boolean
        Get
            Return trDocumentObject.Visible
        End Get
        Set(ByVal value As Boolean)
            trDocumentObject.Visible = value
        End Set
    End Property

    ''' <summary> Restituisce o imposta la visibilità della sezione DocumentReason </summary>
    Public Property VisibleDocumentReason() As Boolean
        Get
            Return trDocumentReason.Visible
        End Get
        Set(ByVal value As Boolean)
            trDocumentReason.Visible = value
        End Set
    End Property

    ''' <summary> Restituisce o imposta la visibilità della sezione DocumentNote </summary>
    Public Property VisibleDocumentNote() As Boolean
        Get
            Return trDocumentNote.Visible
        End Get
        Set(ByVal value As Boolean)
            trDocumentNote.Visible = value
        End Set
    End Property

    ''' <summary> Restituisce o imposta la visibilità della sezione DocumentRegDate </summary>
    Public Property VisibleDocumentRegDate() As Boolean
        Get
            Return trDocumentRegDate.Visible
        End Get
        Set(ByVal value As Boolean)
            trDocumentRegDate.Visible = value
        End Set
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        BindContainers()
        If Not IsPostBack Then
            If DocSuiteContext.Current.ProtocolEnv.IsEnvSearchDefaultEnabled Then
                txtYear.Text = DateTime.Now.Year.ToString()
                txtNumber.Focus()
            Else
                txtYear.Focus()
            End If
            trContactChild.Attributes.Add("style", "display:none")
            trCategoryChild.Attributes.Add("style", "display:none")
        End If
        pnlInterop.Visible = DocSuiteContext.Current.DocumentEnv.IsInteropEnabled
    End Sub

    Private Sub Search_Click(ByVal sender As System.Object, ByVal e As EventArgs) Handles Search.Click
        DoSearch()
    End Sub

    Protected Sub OnContactAdded(ByVal sender As Object, ByVal e As EventArgs) Handles uscContattiSel1.ContactAdded
        AjaxManager.ResponseScripts.Add("VisibleContactChild()")
    End Sub

    Protected Sub OnContactRemoved(ByVal sender As Object, ByVal e As EventArgs) Handles uscContattiSel1.ContactRemoved
        If uscContattiSel1.TreeViewControl.Nodes(0).Nodes.Count = 0 Then
            AjaxManager.ResponseScripts.Add("HideContactChild()")
        End If
    End Sub

    Protected Sub OnCategoryAdded(ByVal sender As Object, ByVal e As EventArgs) Handles uscClassificatore1.CategoryAdded
        VisibleCategoryChild = True
        AjaxManager.ResponseScripts.Add("VisibleCategoryChild()")
    End Sub

    Protected Sub OnCategoryRemoved(ByVal sender As Object, ByVal e As EventArgs) Handles uscClassificatore1.CategoryRemoved
        If Not uscClassificatore1.HasSelectedCategories Then
            VisibleCategoryChild = False
            AjaxManager.ResponseScripts.Add("HideCategoryChild()")
        End If
    End Sub

    Protected Sub uscClassificatore1_CategoryAdding(sender As Object, args As EventArgs) Handles uscClassificatore1.CategoryAdding
        uscClassificatore1.Year = Nothing
        If txtYear.Value.HasValue Then
            uscClassificatore1.Year = Convert.ToInt32(txtYear.Value)
        End If

        uscClassificatore1.FromDate = Nothing
        If dtStartDate_From.SelectedDate.HasValue Then
            uscClassificatore1.FromDate = dtStartDate_From.SelectedDate.Value
        End If

        If dtExpiryDate_From.SelectedDate.HasValue Then
            If uscClassificatore1.FromDate Is Nothing OrElse uscClassificatore1.FromDate > dtExpiryDate_From.SelectedDate Then
                uscClassificatore1.FromDate = dtExpiryDate_From.SelectedDate
            End If
        End If

        uscClassificatore1.ToDate = Nothing
        If dtStartDate_To.SelectedDate.HasValue Then
            uscClassificatore1.ToDate = dtStartDate_To.SelectedDate.Value
        End If

        If dtExpiryDate_To.SelectedDate.HasValue Then
            If uscClassificatore1.FromDate Is Nothing OrElse uscClassificatore1.ToDate > dtExpiryDate_To.SelectedDate Then
                uscClassificatore1.ToDate = dtExpiryDate_To.SelectedDate
            End If
        End If
    End Sub

#End Region

#Region " Methods "

    Private Sub BindData()
        If Not String.IsNullOrEmpty(txtYear.Text) Then
            _finder.Anno = Short.Parse(txtYear.Text)
        End If
        If Not String.IsNullOrEmpty(txtNumber.Text) Then
            _finder.Numero = Integer.Parse(txtNumber.Text)
        End If
        _finder.DataInizioFrom = dtStartDate_From.SelectedDate
        _finder.DataInizioTo = dtStartDate_To.SelectedDate
        _finder.DataScadenzaFrom = dtExpiryDate_From.SelectedDate
        _finder.DataScadenzaTo = dtExpiryDate_To.SelectedDate

        'CLASSIFICATORE
        If uscClassificatore1.HasSelectedCategories AndAlso String.IsNullOrEmpty(_finder.IDCategory) Then
            Dim categories As IList(Of Category) = uscClassificatore1.SelectedCategories
            For Each cat As Category In categories
                _finder.IDCategory &= cat.FullIncrementalPath & ","
            Next
            'Tolgo l'ultima virgola
            _finder.IDCategory = _finder.IDCategory.Substring(0, _finder.IDCategory.Length - 1)
        End If

        'SOTTOCLASSIFICATORE
        If VisibleCategoryChild Then
            _finder.IncludeChildCategories = chbCategoryChild.Checked
        End If

        'CONTATTI
        Dim contacts As IList(Of ContactDTO) = uscContattiSel1.GetContacts(False)
        If Not contacts.IsNullOrEmpty() Then
            For Each c As ContactDTO In contacts
                _finder.IDContact = (c.Contact.FullIncrementalPath & ",")
            Next
            _finder.IDContact = _finder.IDContact.Substring(0, _finder.IDContact.Length - 1)
            'SOTTOCONTATTI
            _finder.IncludeChildContacts = chbContactChild.Checked
        End If

        'SETTORI
        Dim roles As IList(Of Role) = uscSettore1.GetRoles()
        If Not roles.IsNullOrEmpty() Then
            _finder.IDRole = String.Join(",", roles.Select(Function(x) x.Id.ToString()).ToArray())
        End If

        'CONTENITORE
        _finder.IDContainer = ddlIdContainer.SelectedValue

        _finder.Manager = txtManager.Text.Trim()
        _finder.Nome = txtName.Text.Trim()
        _finder.Note = txtNote.Text.Trim()
        _finder.NumeroServizio = txtServiceNumber.Text.Trim()

        'OGGETTO
        _finder.Oggetto = txtObjectD.Text.Trim()
        '_finder.SearchObject = txtseNumber.Text.Trim()
        Select Case rblClausola.SelectedValue
            Case "AND"
                _finder.DocumentObjectSearch = NHibernateProtocolFinder.ObjectSearchType.AllWords
            Case "OR"
                _finder.DocumentObjectSearch = NHibernateProtocolFinder.ObjectSearchType.AtLeastWord
        End Select

        'STATO
        Select Case ddlStatus.SelectedValue
            Case 0
                _finder.Status = NHibernateDocumentFinder.SearchStatus.All
            Case 1
                _finder.Status = NHibernateDocumentFinder.SearchStatus.Open
            Case 2
                _finder.Status = NHibernateDocumentFinder.SearchStatus.Closed
            Case 3
                _finder.Status = NHibernateDocumentFinder.SearchStatus.Canceled
            Case 4
                _finder.Status = NHibernateDocumentFinder.SearchStatus.Archived
        End Select

        _finder.DocumentDescription = txtDocumentDescription.Text.Trim()
        _finder.DocumentDate_From = dtDocumentDate_From.SelectedDate
        _finder.DocumentDate_To = dtDocumentDate_To.SelectedDate
        _finder.DocumentObject = txtDocumentObject.Text.Trim()
        _finder.DocumentReason = txtDocumentReason.Text.Trim()
        _finder.DocumentNote = txtDocumentNote.Text.Trim()
        _finder.DocumentRegDate_From = dtDocumentRegDate_From.SelectedDate
        _finder.DocumentRegDate_To = dtDocumentRegDate_To.SelectedDate
    End Sub

    ''' <summary> Esegue la ricerca in base ai parametri impostati </summary>
    Public Function DoSearch() As IList(Of Document)
        BindData()
        Return _finder.DoSearch()
    End Function

    ''' <summary> Esegue l'ordinamento in base ai parametri impostati dalla grid </summary>
    Public Function DoSort(ByVal source As Object, ByVal e As GridSortCommandEventArgs) As IList(Of Document)
        For Each gse As GridSortExpression In source.MasterTableView.SortExpressions
            _finder.SortExpressions.Add(gse.FieldName, gse.SortOrder.ToString())
        Next
        If _finder.SortExpressions.ContainsKey(e.SortExpression) Then
            _finder.SortExpressions(e.SortExpression) = e.NewSortOrder.ToString()
        Else
            _finder.SortExpressions.Add(e.SortExpression, e.NewSortOrder.ToString())
        End If
        BindData()
        Return _finder.DoSearch()
    End Function

    Private Sub BindContainers()
        Dim containers As IList(Of Container) = Facade.ContainerFacade.GetAllRightsDistinct("Docm", Nothing)
        'PER EVITARE DI FARE 2 QUERY NE FACCIO UNA SOLA CHE MI RITORNA TUTTI I CONTENITORI
        'INVECIE DI CICLARE 2 VOLTE SU TUTTI I CONTENITORI SI POTEVANO FILTRARE.. OPPURE CON UN PO' DI FANTASIA
        'SFRUTTANDO IL METODO ITEMS.INSERT SI POTREBBE FARE TUTTO CON UN CICLO SOLO... 
        'MA VISTO CHE I CONTENITORI NON SONO TANTISSIMI, PER IL MOMENTO FACCIO COSI'
        For Each container As Container In containers
            If container.IsActive AndAlso container.IsActiveRange() Then
                ddlIdContainer.Items.Add(New ListItem(container.Name, container.Id.ToString()))
            End If
        Next
        For Each container As Container In containers
            If Not container.IsActive OrElse Not container.IsActiveRange() Then
                Dim li As New ListItem(container.Name, container.Id.ToString())
                li.Attributes.Add("class", "disabled")
                ddlIdContainer.Items.Add(li)
            End If
        Next
    End Sub

#End Region

End Class
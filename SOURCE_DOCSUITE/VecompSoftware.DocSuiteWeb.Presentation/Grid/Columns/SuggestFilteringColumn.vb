Imports System.Web.UI.WebControls
Imports System.Web.UI
Imports Telerik.Web.UI

''' <summary> Colonna in stile google che implementa un filtro con combobox e autosuggest </summary>
Public Class SuggestFilteringColumn
    Inherits GridBoundColumn

#Region " Fields "

    Private _comboFilter As DropDownList
    Private _dataSourceCombo As Object
    Private _dataTextCombo As String
    Private _dataFieldCombo As String
    Private _propertyBind As String

#End Region

#Region " Properties "

    Public Property DataSourceCombo() As Object
        Get
            Return _dataSourceCombo
        End Get
        Set(ByVal value As Object)
            _dataSourceCombo = value
        End Set
    End Property

    Public Property DataFieldCombo() As String
        Get
            Return _dataFieldCombo
        End Get
        Set(ByVal value As String)
            _dataFieldCombo = value
        End Set
    End Property

    Public Property DataTextCombo() As String
        Get
            Return _dataTextCombo
        End Get
        Set(ByVal value As String)
            _dataTextCombo = value
        End Set
    End Property

    Public ReadOnly Property ComboBoxControl() As DropDownList
        Get
            Return _comboFilter
        End Get
    End Property

    Public Property PropertyBind() As String
        Get
            Return _propertyBind
        End Get
        Set(ByVal value As String)
            _propertyBind = value
        End Set
    End Property

#End Region

#Region " Methods "

    ''' <summary> Inizializza il controllo che identifica il filtro di una colonna </summary>
    Protected Overrides Sub SetupFilterControls(ByVal cell As TableCell)
        MyBase.SetupFilterControls(cell)

        cell.Controls.RemoveAt(0)

        _comboFilter = New DropDownList()
        _comboFilter.ID = ("RadComboBox1" + DataField)
        _comboFilter.AutoPostBack = True
        _comboFilter.Width = Unit.Percentage(98)
        _comboFilter.Height = Unit.Percentage(100)
        AddHandler _comboFilter.SelectedIndexChanged, AddressOf list_SelectedIndexChanged
        AddHandler _comboFilter.DataBound, AddressOf list_DataBound

        _comboFilter.DataTextField = DataTextCombo
        _comboFilter.DataValueField = DataFieldCombo
        _comboFilter.DataSource = DataSourceCombo
        _comboFilter.DataBind()

        Dim table As New DSTable(False)
        table.CSSClass = "filterControlTable"
        table.CreateEmptyRow(False)
        table.CurrentRow.CreateEmpytCell(False)
        table.CurrentRow.CurrentCell.AddCellControl(_comboFilter)

        cell.Controls.AddAt(0, table.Table)
        cell.Controls.RemoveAt(1)
        'imposto il valore del filtro
        If Not cell.Page.IsPostBack Then
            FilterHelper.SetSessionFilterClient(cell.Page.Session, _comboFilter.DataTextField, New FilterClient(_comboFilter.DataTextField, _comboFilter.ClientID, _comboFilter.SelectedIndex))
        End If
    End Sub

    ''' <summary> Imposta il valore del controllo che identifica il filtro </summary>
    Protected Overrides Sub SetCurrentFilterValueToControl(ByVal cell As TableCell)
        MyBase.SetCurrentFilterValueToControl(cell)
        _comboFilter = FilterHelper.GetFilterControl(Of DropDownList)(cell.Controls(0))
        If Not String.IsNullOrEmpty(Me.CurrentFilterValue) Then
            _comboFilter.Text = Me.CurrentFilterValue
        End If
    End Sub

    ''' <summary> Restituisce il valore del controllo che identifica il filtro </summary>
    Protected Overrides Function GetCurrentFilterValueFromControl(ByVal cell As TableCell) As String
        Return FilterHelper.GetFilterControl(Of DropDownList)(cell.Controls(0)).Text
    End Function

    Private Sub list_SelectedIndexChanged(ByVal o As Object, ByVal e As EventArgs)
        Dim filter As Pair
        If Me.DataType Is GetType(String) Then
            'Filtro per colonne di stringhe
            filter = New Pair("Contains", Me.UniqueName)
        Else
            'Filtro per colonne di interi
            filter = New Pair("EqualTo", Me.UniqueName)
        End If
        Dim filterItem As GridFilteringItem = DirectCast(DirectCast(o, DropDownList).NamingContainer, GridFilteringItem)
        filterItem.FireCommandEvent("Filter", filter)
    End Sub

    Private Sub list_DataBound(ByVal o As Object, ByVal e As EventArgs)
        _comboFilter.Items.Insert(0, New ListItem("", ""))
    End Sub

#End Region

End Class

Imports System.Web.UI.WebControls
Imports System.Web.UI
Imports Telerik.Web.UI

Public Class SuggestFilteringTemplateColumn
    Inherits GridTemplateColumn

#Region " Fields "

    Private _comboFilter As DropDownList
    Private _dataSourceCombo As Object
    Private _dataTextCombo As String
    Private _dataFieldCombo As String

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

#End Region

#Region " Methods "

    ''' <summary> Inizializza il controllo che identifica il filtro di una colonna </summary>
    Protected Overrides Sub SetupFilterControls(ByVal cell As TableCell)
        MyBase.SetupFilterControls(cell)

        cell.Controls.RemoveAt(0)

        _comboFilter = New DropDownList()
        _comboFilter.ID = ("RadComboBox1" + Me.DataField)
        _comboFilter.AutoPostBack = True
        _comboFilter.Width = Unit.Percentage(98)
        _comboFilter.Height = Unit.Percentage(100)
        AddHandler _comboFilter.SelectedIndexChanged, AddressOf Me.list_SelectedIndexChanged
        AddHandler _comboFilter.DataBound, AddressOf Me.list_DataBound

        _comboFilter.DataTextField = DataTextCombo
        _comboFilter.DataValueField = DataFieldCombo
        _comboFilter.DataSource = DataSourceCombo
        _comboFilter.DataBind()

        Dim table As DSTable = New DSTable()
        table.Width = Unit.Percentage(100)
        table.Table.BorderStyle = WebControls.BorderStyle.None
        table.CreateEmptyRow()
        table.CurrentRow.CreateEmpytCell()
        table.CurrentRow.CurrentCell.Width = Unit.Percentage(100)
        table.CurrentRow.CurrentCell.TableCell.Style.Add("border", "0px !important")
        table.CurrentRow.CurrentCell.AddCellControl(_comboFilter)

        cell.Controls.AddAt(0, table.Table)
        cell.Controls.RemoveAt(1)

        'imposto il valore del filtro
        If Not cell.Page.IsPostBack Then
            FilterHelper.SetSessionFilterClient(cell.Page.Session, _comboFilter.DataTextField, New FilterClient(_comboFilter.DataTextField, _comboFilter.ClientID, _comboFilter.SelectedIndex))
        End If
    End Sub

    Public Overrides Function Clone() As GridColumn
        Return MyBase.Clone()
    End Function

    Protected Overrides Function GetFilterDataField() As String
        Return Me.DataField
    End Function

    Protected Overrides Sub SetCurrentFilterValueToControl(ByVal cell As TableCell)
        MyBase.SetCurrentFilterValueToControl(cell)

        If Not String.IsNullOrEmpty(Me.CurrentFilterValue) Then
            FilterHelper.GetFilterControl(Of TextBox)(cell.Controls(0)).Text = Me.CurrentFilterValue
        End If
    End Sub

    Protected Overrides Function GetCurrentFilterValueFromControl(ByVal cell As TableCell) As String
        Return FilterHelper.GetFilterControl(Of TextBox)(cell.Controls(0)).Text
    End Function

    Private Sub list_DataBound(ByVal o As Object, ByVal e As EventArgs)
        _comboFilter.Items.Insert(0, New ListItem("", ""))
    End Sub

    Protected Overridable Sub list_SelectedIndexChanged(ByVal o As Object, ByVal e As EventArgs)
        Dim filter As Pair
        If (Me.DataType Is GetType(System.String)) Then
            'Filtro per colonne di stringhe
            filter = New Pair("Contains", Me.UniqueName)
        Else
            'Filtro per le altre colonne
            filter = New Pair("EqualTo", Me.UniqueName)
        End If
        Dim filterItem As GridFilteringItem = CType(CType(o, DropDownList).NamingContainer, GridFilteringItem)
        filterItem.FireCommandEvent("Filter", filter)
    End Sub

#End Region

End Class

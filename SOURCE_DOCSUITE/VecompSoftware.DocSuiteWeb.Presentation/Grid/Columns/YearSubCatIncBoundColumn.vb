Imports System.Web.UI.WebControls
Imports System.Web.UI
Imports Telerik.Web.UI

Public Class YearSubCatIncBoundColumn
    Inherits GridTemplateColumn

#Region " Fields "

    Protected _textBox As TextBox = New TextBox()
    Protected _maskTextBox As RadMaskedTextBox = New RadMaskedTextBox()
    Private _allowFiltering As Boolean

#End Region

#Region " Methods "

    Protected Overrides Sub SetupFilterControls(ByVal cell As TableCell)
        MyBase.SetupFilterControls(cell)

        cell.Controls.RemoveAt(0)

        _maskTextBox = New RadMaskedTextBox()
        _maskTextBox.ResetCaretOnFocus = True
        _maskTextBox.Mask = "####-#######-#######"
        _maskTextBox.DisplayMask = "####-#######-#######"
        _maskTextBox.Width = Unit.Percentage(100)
        _maskTextBox.Height = Unit.Percentage(100)
        AddHandler _maskTextBox.TextChanged, AddressOf MaskText_TextChanged

        Dim table As DSTable = New DSTable()
        table.Width = Unit.Percentage(100)
        table.Table.BorderStyle = WebControls.BorderStyle.None
        table.CreateEmptyRow()
        table.CurrentRow.CreateEmpytCell()
        table.CurrentRow.CurrentCell.Width = Unit.Percentage(100)
        table.CurrentRow.CurrentCell.TableCell.Style.Add("border", "0px !important")
        table.CurrentRow.CurrentCell.AddCellControl(_maskTextBox)

        cell.Controls.AddAt(0, table.Table)
        cell.Controls.RemoveAt(1)
        If Not cell.Page.IsPostBack Then
            FilterHelper.SetSessionFilterClient(cell.Page.Session, _maskTextBox.ClientID, New FilterClient(_maskTextBox.ClientID, _maskTextBox.ClientID + "_text", _maskTextBox.TextWithLiterals))
        End If
    End Sub

    Public Overrides Function Clone() As GridColumn
        Return MyBase.Clone()
    End Function

    Protected Overrides Function GetFilterDataField() As String
        Return Me.DataField
    End Function

    Public Overrides Property AllowFiltering() As Boolean
        Get
            Return _allowFiltering
        End Get
        Set(ByVal value As Boolean)
            _allowFiltering = value
            If Not _maskTextBox Is Nothing Then
                _maskTextBox.Visible = value
            End If
            If Not _textBox Is Nothing Then
                _textBox.Visible = value
            End If
            MyBase.AllowFiltering = value
        End Set
    End Property


    Protected Overrides Sub SetCurrentFilterValueToControl(ByVal cell As TableCell)
        MyBase.SetCurrentFilterValueToControl(cell)

        If Not String.IsNullOrEmpty(Me.CurrentFilterValue) Then
            FilterHelper.GetFilterControl(Of TextBox)(cell.Controls(0)).Text = Me.CurrentFilterValue
        End If
    End Sub

    Protected Overrides Function GetCurrentFilterValueFromControl(ByVal cell As TableCell) As String
        Return FilterHelper.GetFilterControl(Of TextBox)(cell.Controls(0)).Text
    End Function

    Public Overrides Function SupportsFiltering() As Boolean
        Return True
    End Function

    Protected Sub MaskText_TextChanged(ByVal sender As Object, ByVal e As EventArgs)
        Dim filterItem As GridFilteringItem = DirectCast(DirectCast(sender, RadMaskedTextBox).NamingContainer, GridFilteringItem)
        'Filtro per colonne di stringhe
        filterItem.FireCommandEvent("Filter", New Pair("EqualTo", Me.UniqueName))
    End Sub

    Public Overridable Function GetSQLExpression(ByVal sqlParameter As String)
        Return "(convert(varchar,{alias}.Year) + '-' + convert(varchar,{alias}.IdCategory) + '-' + convert(varchar,{alias}.Incremental)) LIKE '" & sqlParameter & "'"
    End Function

#End Region

End Class

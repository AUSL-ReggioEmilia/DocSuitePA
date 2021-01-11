Imports System.Web.UI.WebControls
Imports System.Web.UI
Imports Telerik.Web.UI

Public Class YearNumberBoundColumn
    Inherits GridTemplateColumn

#Region " Fields "

    Protected _textBox As TextBox = Nothing
    Protected _maskTextBox As RadMaskedTextBox = Nothing

#End Region

#Region " Methods "

    Protected Overrides Sub SetupFilterControls(ByVal cell As TableCell)
        MyBase.SetupFilterControls(cell)

        cell.Controls.RemoveAt(0)

        _maskTextBox = New RadMaskedTextBox()
        _maskTextBox.ResetCaretOnFocus = True
        _maskTextBox.Mask = "####/#######"
        _maskTextBox.DisplayMask = "####/#######"
        _maskTextBox.CssClass = "filterBox"
        AddHandler _maskTextBox.TextChanged, AddressOf MaskText_TextChanged

        Dim table As DSTable = FilterHelper.CreateTable(_maskTextBox, Nothing)

        cell.Controls.AddAt(0, table.Table)
        cell.Controls.RemoveAt(1)
        If Not cell.Page.IsPostBack Then
            FilterHelper.SetSessionFilterClient(cell.Page.Session, _maskTextBox.ClientID, New FilterClient(_maskTextBox.ClientID, _maskTextBox.ClientID + "_text", _maskTextBox.Text))
        End If
    End Sub

    Public Overrides Function Clone() As GridColumn
        Return MyBase.Clone()
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

    Public Overrides Function SupportsFiltering() As Boolean
        Return True
    End Function

    Protected Sub MaskText_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs)
        If String.IsNullOrEmpty(_maskTextBox.Text) Then
            Exit Sub
        End If

        Dim arrHlp() As String = _maskTextBox.TextWithPromptAndLiterals.Split("/")

        Dim i As Integer
        If Not Int32.TryParse(arrHlp(1).TrimEnd("_").Replace("_", "0"), i) Then
            i = 0
        End If
        _maskTextBox.Text = String.Format("{0}/{1:0000000}", arrHlp(0).Replace("_", "0"), i)

        ' Filtro per colonne di stringhe
        Dim filterItem As GridFilteringItem = DirectCast(DirectCast(sender, RadMaskedTextBox).NamingContainer, GridFilteringItem)
        filterItem.FireCommandEvent("Filter", New Pair("EqualTo", Me.UniqueName))
    End Sub

    Public Overridable Function GetSQLExpression(ByVal sqlParameter As String)
        Return "(convert(varchar,{alias}.Year) + '/' + right('0000000'+convert(varchar,{alias}.Number),7)) LIKE '" & sqlParameter & "'"
    End Function

#End Region

End Class

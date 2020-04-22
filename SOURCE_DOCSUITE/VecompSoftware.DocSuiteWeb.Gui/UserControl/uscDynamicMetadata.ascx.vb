Imports System.Collections.Generic
Imports System.Globalization
Imports System.Linq
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.Commons
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Model.Metadata
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web
Imports VecompSoftware.Helpers.Web.HtmlStructure
Imports VecompSoftware.Services.Logging

Public Class uscDynamicMetadata
    Inherits DocSuite2008BaseControl


    Public Const DYNAMIC_LABEL_NAME_FORMAT As String = "lbl_"
    Public Const DYNAMIC_FIELD_NAME_FORMAT As String = "field_"
    Public Const DYNAMIC_VALIDATOR_NAME_FORMAT As String = "validator_"

    Private Shared _labelTypeName As String = GetType(Label).Name
    Private Shared _checkBoxTypeName As String = GetType(CheckBox).Name
    Private Shared _radDropDownListTypeName As String = GetType(RadDropDownList).Name
    Private Shared _radComboBoxTypeName As String = GetType(RadComboBox).Name
    Private Shared _radTextBoxTypeName As String = GetType(RadTextBox).Name
    Private Shared _radDatePickerTypeName As String = GetType(RadDatePicker).Name
    Private Shared _radNumericTextBoxTypeName As String = GetType(RadNumericTextBox).Name

    Public ReadOnly Property PageContentDiv As Control
        Get
            Return pnlDynamicContent
        End Get
    End Property

    Public Property IsReadOnly As Boolean

    Private Property CurrentMetadataModel As MetadataModel
        Get
            If ViewState(String.Format("{0}_CurrentMetadataModel", ID)) IsNot Nothing Then
                Return DirectCast(ViewState(String.Format("{0}_CurrentMetadataModel", ID)), MetadataModel)
            End If
            Return Nothing
        End Get
        Set(ByVal value As MetadataModel)
            ViewState(String.Format("{0}_CurrentMetadataModel", ID)) = value
        End Set
    End Property

    Public Property ValidationDisabled As Boolean

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            CurrentMetadataModel = Nothing
        End If

        If CurrentMetadataModel IsNot Nothing Then
            LoadControls(CurrentMetadataModel)
        End If
    End Sub

    Public Sub LoadControls(metadataModel As MetadataModel)
        If metadataModel Is Nothing Then
            Exit Sub
        End If

        dynamicPageContent.Controls.Clear()
        Dim layout As RadPageLayout = New RadPageLayout()

        For Each textItem As TextFieldModel In metadataModel.TextFields
            CreateTextControl(layout, textItem)
        Next
        For Each numberItem As BaseFieldModel In metadataModel.NumberFields
            CreateNumberControl(layout, numberItem)
        Next
        For Each dateItem As BaseFieldModel In metadataModel.DateFields
            CreateDateControl(layout, dateItem)
        Next
        For Each boolItem As BaseFieldModel In metadataModel.BoolFields
            CreateBoolControl(layout, boolItem)
        Next
        For Each enumItem As EnumFieldModel In metadataModel.EnumFields
            CreateEnumControl(layout, enumItem)
        Next
        For Each discussionItem As DiscussionFieldModel In metadataModel.DiscussionFields
            CreateDiscussionControl(layout, discussionItem)
        Next
        dynamicPageContent.Controls.Add(layout)
        CurrentMetadataModel = metadataModel
    End Sub

    Private Sub CreateControl(layout As RadPageLayout, control As Control, fieldControlId As String, item As BaseFieldModel, Optional validationEnabled As Boolean = True)
        Dim row As LayoutRow = New LayoutRow()
        row.CssClass = "layout-row"
        Dim labelColumn As LayoutColumn = CreateLabelColumn()
        Dim label As Control = New LabelStructure().GetStructure(String.Concat(DYNAMIC_LABEL_NAME_FORMAT, item.Label, item.Position), GetLabelName(item.Label), "strongRiLabel")
        labelColumn.Controls.Add(label)
        row.Columns.Add(labelColumn)

        Dim fieldColumn As LayoutColumn = CreateFieldColumn()
        fieldColumn.Controls.Add(control)
        If item.Required AndAlso validationEnabled Then
            CreateValidatorControl(fieldColumn, fieldControlId, item.Label)
        End If
        row.Columns.Add(fieldColumn)
        layout.Rows.Add(row)
    End Sub

    Private Sub CreateTextControl(layout As RadPageLayout, textItem As TextFieldModel)
        Dim fieldControlId As String = WebHelper.SafeControlIdName(String.Concat(DYNAMIC_FIELD_NAME_FORMAT, textItem.Label, textItem.Position))
        Dim control As Control
        If IsReadOnly Then
            control = New LabelStructure().GetStructure(fieldControlId, textItem.Value, String.Empty)
        Else
            control = New TextStructure().GetRadStructure(fieldControlId, New KeyValuePair(Of Integer, UnitType)(100, UnitType.Percentage), Nothing,
                                                                  Nothing, If(textItem.Multiline, InputMode.MultiLine, InputMode.SingleLine), String.Empty)
        End If
        CreateControl(layout, control, fieldControlId, textItem, Not IsReadOnly)
    End Sub

    Private Sub CreateDiscussionControl(layout As RadPageLayout, discussionItem As DiscussionFieldModel)
        Dim fieldControlId As String = WebHelper.SafeControlIdName(String.Concat(DYNAMIC_FIELD_NAME_FORMAT, discussionItem.Label, discussionItem.Position))
        Dim control As Control
        If IsReadOnly Then
            control = New HtmlGenericControl("div")
            If discussionItem.Comments IsNot Nothing AndAlso discussionItem.Comments.Count > 0 Then
                Dim lastComment As CommentFieldModel = discussionItem.Comments.OrderByDescending(Function(o) o.RegistrationDate).First()
                Dim authorDiv As HtmlGenericControl = New HtmlGenericControl("div")
                Dim author As String = If(Not String.IsNullOrEmpty(lastComment.Author), CommonAD.GetDisplayName(lastComment.Author), String.Empty)
                Dim authorLabel As Control = New LabelStructure().GetStructure(String.Concat(fieldControlId, "_author"), String.Concat(author, " - ", String.Format("{0:dd/MM/yyyy}", lastComment.RegistrationDate)), "metadata-author")
                authorDiv.Controls.Add(authorLabel)
                Dim imageControl As ImageButton = New ImageButton()
                imageControl.ImageUrl = "../App_Themes/DocSuite2008/imgset16/search.png"
                imageControl.OnClientClick = String.Concat("return OpenCommentsWindow('", discussionItem.Label, "')")
                authorDiv.Controls.Add(imageControl)
                Dim commentDiv As HtmlGenericControl = New HtmlGenericControl("div")
                Dim commentLabel As Control = New LabelStructure().GetStructure(String.Concat(fieldControlId, "_comment"), lastComment.Comment, String.Empty)
                commentDiv.Controls.Add(commentLabel)
                control.Controls.Add(authorDiv)
                control.Controls.Add(commentDiv)
            End If
        Else
            control = New TextStructure().GetRadStructure(fieldControlId, New KeyValuePair(Of Integer, UnitType)(100, UnitType.Percentage), Nothing,
            Nothing, InputMode.MultiLine, String.Empty)
        End If
        CreateControl(layout, control, fieldControlId, discussionItem, Not IsReadOnly)
    End Sub

    Private Sub CreateNumberControl(layout As RadPageLayout, numberItem As BaseFieldModel)
        Dim fieldControlId As String = WebHelper.SafeControlIdName(String.Concat(DYNAMIC_FIELD_NAME_FORMAT, numberItem.Label, numberItem.Position))
        Dim control As Control
        If IsReadOnly Then
            Dim numberValueFormatted As String = String.Empty
            Dim n As Double
            If Double.TryParse(numberItem.Value, n) Then
                numberValueFormatted = n.ToString("G", CultureInfo.InvariantCulture)
            End If
            control = New LabelStructure().GetStructure(fieldControlId, numberValueFormatted, String.Empty)
        Else
            control = New NumericStructure().GetRadStructure(fieldControlId, New KeyValuePair(Of Integer, UnitType)(100, UnitType.Pixel), Nothing, Nothing,
                                                             String.Empty, 0)
        End If
        CreateControl(layout, control, fieldControlId, numberItem, Not IsReadOnly)
    End Sub

    Private Sub CreateDateControl(layout As RadPageLayout, dateItem As BaseFieldModel)
        Dim fieldControlId As String = WebHelper.SafeControlIdName(String.Concat(DYNAMIC_FIELD_NAME_FORMAT, dateItem.Label, dateItem.Position))
        Dim control As Control
        If IsReadOnly Then
            Dim formattedDate As String = String.Empty
            Dim d As DateTime
            If Not String.IsNullOrEmpty(dateItem.Value) AndAlso DateTime.TryParse(dateItem.Value, d) Then
                formattedDate = d.ToString("dd/MM/yyyy")
            End If
            control = New LabelStructure().GetStructure(fieldControlId, formattedDate, String.Empty)
        Else
            control = New DateTimeStructure().GetRadStructure(fieldControlId, New KeyValuePair(Of Integer, UnitType)(150, UnitType.Pixel),
                                                                      Nothing, String.Empty)
        End If
        CreateControl(layout, control, fieldControlId, dateItem, Not IsReadOnly)
    End Sub

    Private Sub CreateBoolControl(layout As RadPageLayout, boolItem As BaseFieldModel)
        Dim fieldControlId As String = WebHelper.SafeControlIdName(String.Concat(DYNAMIC_FIELD_NAME_FORMAT, boolItem.Label, boolItem.Position))
        Dim boolValue As Boolean
        If IsReadOnly AndAlso Not String.IsNullOrEmpty(boolItem.Value) Then
            Boolean.TryParse(boolItem.Value, boolValue)
        End If
        Dim control As Control = New CheckBoxStructure().GetStructure(fieldControlId, Nothing, boolValue, String.Empty, Not IsReadOnly)
        CreateControl(layout, control, fieldControlId, boolItem, False)
    End Sub

    Private Sub CreateEnumControl(layout As RadPageLayout, enumItem As EnumFieldModel)
        Dim comboDictionary As IDictionary(Of String, String) = New Dictionary(Of String, String)
        If enumItem.Options IsNot Nothing Then
            comboDictionary = enumItem.Options.OrderBy(Function(x) x.Key).ToDictionary(Function(p) p.Value, Function(p) p.Key.ToString())
        End If

        Dim fieldColumn As LayoutColumn = CreateFieldColumn()
        Dim fieldControlId As String = WebHelper.SafeControlIdName(String.Concat(DYNAMIC_FIELD_NAME_FORMAT, enumItem.Label, enumItem.Position))
        Dim control As Control
        If IsReadOnly Then
            control = New LabelStructure().GetStructure(fieldControlId, enumItem.Value, String.Empty)
        Else
            control = New ComboStructure().GetRadStructure(fieldControlId, New KeyValuePair(Of Integer, UnitType)(200, UnitType.Pixel), comboDictionary,
                                                                   String.Empty)
        End If
        CreateControl(layout, control, fieldControlId, enumItem, Not IsReadOnly)
    End Sub

    Private Function GetLabelName(name As String) As String
        Return String.Concat(name, ":")
    End Function

    Private Function CreateLabelColumn() As LayoutColumn
        Dim labelColumn As LayoutColumn = New LayoutColumn()
        labelColumn.Span = If(IsReadOnly, 1, 2)
        Dim cssClass As String = "dsw-text-right"
        If IsReadOnly Then
            cssClass = String.Concat(cssClass, " col-dsw-16-important")
        End If
        labelColumn.CssClass = cssClass
        Return labelColumn
    End Function

    Private Function CreateFieldColumn() As LayoutColumn
        Dim fieldColumn As LayoutColumn = New LayoutColumn()
        fieldColumn.Span = If(IsReadOnly, 1, 9)
        Dim cssClass As String = "t-col-left-padding"
        If IsReadOnly Then
            cssClass = String.Concat(cssClass, " t-col-right-padding col-dsw-84-important")
        End If
        fieldColumn.CssClass = cssClass
        Return fieldColumn
    End Function

    Private Sub CreateValidatorControl(column As LayoutColumn, parentId As String, parentName As String)
        If Not ValidationDisabled Then
            Dim tdControl As Control = New Control()
            Dim validator As Control = New ValidatorStructure().GetStructure(String.Concat(DYNAMIC_VALIDATOR_NAME_FORMAT, parentId), String.Format("Campo {0} Obbligatorio", parentName), parentId, ValidatorDisplay.Dynamic, String.Empty)
            Dim row As LayoutRow = New LayoutRow()
            column.Controls.Add(validator)
        End If
    End Sub

    Public Sub SetControlValue(metadataModel As MetadataModel, Optional isDefault As Boolean = False)
        For Each textItem As TextFieldModel In metadataModel.TextFields
            SetControlValue(WebHelper.SafeControlIdName(String.Concat(DYNAMIC_FIELD_NAME_FORMAT, textItem.Label, textItem.Position)), If(isDefault, textItem.DefaultValue, textItem.Value))
        Next
        For Each numberItem As BaseFieldModel In metadataModel.NumberFields
            SetControlValue(WebHelper.SafeControlIdName(String.Concat(DYNAMIC_FIELD_NAME_FORMAT, numberItem.Label, numberItem.Position)), If(isDefault, numberItem.DefaultValue, numberItem.Value))
        Next
        For Each dateItem As BaseFieldModel In metadataModel.DateFields
            SetControlValue(WebHelper.SafeControlIdName(String.Concat(DYNAMIC_FIELD_NAME_FORMAT, dateItem.Label, dateItem.Position)), If(isDefault, dateItem.DefaultValue, dateItem.Value))
        Next
        For Each boolItem As BaseFieldModel In metadataModel.BoolFields
            SetControlValue(WebHelper.SafeControlIdName(String.Concat(DYNAMIC_FIELD_NAME_FORMAT, boolItem.Label, boolItem.Position)), If(isDefault, boolItem.DefaultValue, boolItem.Value))
        Next
        For Each enumItem As EnumFieldModel In metadataModel.EnumFields
            SetControlValue(WebHelper.SafeControlIdName(String.Concat(DYNAMIC_FIELD_NAME_FORMAT, enumItem.Label, enumItem.Position)), If(isDefault, enumItem.DefaultValue, enumItem.Value))
        Next
        For Each discussionItem As DiscussionFieldModel In metadataModel.DiscussionFields
            SetControlValue(WebHelper.SafeControlIdName(String.Concat(DYNAMIC_FIELD_NAME_FORMAT, discussionItem.Label, discussionItem.Position)), If(isDefault, discussionItem.DefaultValue, discussionItem.Value))
        Next
    End Sub

    Private Sub SetControlValue(controlId As String, value As String)
        If String.IsNullOrEmpty(controlId) OrElse String.IsNullOrEmpty(value) Then
            Exit Sub
        End If

        Dim ctrl As Control = dynamicPageContent.FindControl(controlId)
        If ctrl IsNot Nothing Then
            Dim controlName As String = ctrl.GetType().Name
            Select Case controlName

                Case _labelTypeName
                    Dim control As Label = DirectCast(ctrl, Label)
                    Dim valueDate As DateTime
                    If DateTime.TryParse(value, valueDate) Then
                        value = String.Empty
                        If Not valueDate.Year.Equals(DateTime.MinValue.Year) Then
                            value = valueDate.ToString("dd/MM/yyyy")
                        End If
                    End If
                    control.Text = value

                Case _checkBoxTypeName
                    Dim control As CheckBox = DirectCast(ctrl, CheckBox)
                    Dim boolValue As Boolean
                    Boolean.TryParse(value, boolValue)
                    control.Checked = boolValue

                Case _radDropDownListTypeName

                    Dim control As RadDropDownList = DirectCast(ctrl, RadDropDownList)
                    Dim selectectItem As DropDownListItem = control.Items.SingleOrDefault(Function(f) f.Text.Eq(value))
                    If (selectectItem IsNot Nothing) Then
                        selectectItem.Selected = True
                        control.SelectedText = selectectItem.Text
                        control.SelectedValue = selectectItem.Value
                    End If
                    control.DataBind()
                Case _radComboBoxTypeName

                    Dim control As RadComboBox = DirectCast(ctrl, RadComboBox)
                    Dim selectectItem As RadComboBoxItem = control.Items.SingleOrDefault(Function(f) f.Text.Eq(value))
                    If (selectectItem IsNot Nothing) Then
                        selectectItem.Selected = True
                        control.SelectedValue = selectectItem.Value
                        control.Text = selectectItem.Text
                    End If
                    control.DataBind()
                Case _radTextBoxTypeName
                    Dim control As RadTextBox = DirectCast(ctrl, RadTextBox)
                    control.Text = value

                Case _radDatePickerTypeName
                    Dim control As RadDatePicker = DirectCast(ctrl, RadDatePicker)
                    Dim dateValue As DateTime
                    If DateTime.TryParse(value, dateValue) AndAlso Not dateValue.Year.Equals(DateTime.MinValue.Year) Then
                        control.SelectedDate = dateValue
                    End If

                Case _radNumericTextBoxTypeName
                    Dim control As RadNumericTextBox = DirectCast(ctrl, RadNumericTextBox)
                    Dim numberValue As Double
                    If Double.TryParse(value, numberValue) Then
                        control.Value = numberValue
                    End If
            End Select
        End If
    End Sub

    Public Function GetControlValues() As MetadataModel
        If CurrentMetadataModel Is Nothing Then
            Return Nothing
        End If

        For Each item As TextFieldModel In CurrentMetadataModel.TextFields
            FillPageControl(item)
        Next
        For Each item As BaseFieldModel In CurrentMetadataModel.NumberFields
            FillPageControl(item)
        Next
        For Each item As BaseFieldModel In CurrentMetadataModel.BoolFields
            FillPageControl(item)
        Next
        For Each item As BaseFieldModel In CurrentMetadataModel.DateFields
            FillPageControl(item)
        Next
        For Each item As EnumFieldModel In CurrentMetadataModel.EnumFields
            FillPageControl(item)
        Next
        For Each item As DiscussionFieldModel In CurrentMetadataModel.DiscussionFields
            FillPageControl(item)
        Next

        Return CurrentMetadataModel
    End Function

    Private Sub FillPageControl(ByRef element As Object)
        Dim control As Control = dynamicPageContent.FindControl(WebHelper.SafeControlIdName(String.Concat(DYNAMIC_FIELD_NAME_FORMAT, element.Label, element.Position)))

        If control IsNot Nothing Then
            If TypeOf element Is DiscussionFieldModel Then
                element = DirectCast(element, DiscussionFieldModel)
            Else
                element = DirectCast(element, BaseFieldModel)
            End If

            Dim controlName As String = control.GetType().Name
            Select Case controlName

                Case _checkBoxTypeName
                    element.Value = DirectCast(control, CheckBox).Checked.ToString()

                Case _radDropDownListTypeName
                    element.Value = DirectCast(control, RadDropDownList).SelectedText
                Case _radComboBoxTypeName
                    element.Value = DirectCast(control, RadComboBox).SelectedItem.Text

                Case _radTextBoxTypeName
                    Dim value As String = DirectCast(control, RadTextBox).Text
                    Dim discussion As DiscussionFieldModel
                    element.Value = value

                    If TypeOf element Is DiscussionFieldModel Then
                        discussion = DirectCast(element, DiscussionFieldModel)
                        If discussion.Comments Is Nothing Then
                            discussion.Comments = New List(Of CommentFieldModel)
                        End If
                        If Not String.IsNullOrEmpty(value) Then
                            Dim comment As CommentFieldModel = New CommentFieldModel()
                            comment.Author = DocSuiteContext.Current.User.FullUserName
                            comment.RegistrationDate = DateTimeOffset.UtcNow
                            comment.Comment = value
                            discussion.Comments.Add(comment)
                            discussion.Comments = discussion.Comments.OrderByDescending(Function(c) c.RegistrationDate).ToList()
                            discussion.Value = String.Empty
                        End If

                        For Each item As CommentFieldModel In discussion.Comments
                            item.Author = item.Author.Replace("\", "\\")
                        Next
                    End If


                Case _radDatePickerTypeName
                    If DirectCast(control, RadDatePicker).SelectedDate.HasValue AndAlso DirectCast(control, RadDatePicker).SelectedDate.Value > DateTime.MinValue Then
                        element.Value = DirectCast(control, RadDatePicker).SelectedDate.Value.ToString()
                    End If

                Case _radNumericTextBoxTypeName
                    If DirectCast(control, RadNumericTextBox).Value.HasValue Then
                        element.Value = DirectCast(control, RadNumericTextBox).Value.Value.ToString()
                    End If

            End Select
        End If
    End Sub

    Private Sub InitializeAjax()
        AddHandler AjaxManager.AjaxRequest, AddressOf UscDynamicMetadata_AjaxRequest
        AjaxManager.AjaxSettings.AddAjaxSetting(AjaxManager, dynamicPageContent)
    End Sub

    Protected Sub UscDynamicMetadata_AjaxRequest(ByVal sender As Object, ByVal e As AjaxRequestEventArgs)

        Dim ajaxModel As AjaxModel = Nothing
        Try
            ajaxModel = JsonConvert.DeserializeObject(Of AjaxModel)(e.Argument)
        Catch
            Exit Sub
        End Try

        If ajaxModel IsNot Nothing Then
            Select Case ajaxModel.ActionName
                Case "ClearControls"
                    dynamicPageContent.Controls.Clear()
                    CurrentMetadataModel = Nothing
                    Exit Select
                Case "LoadControls"
                    Try
                        If ajaxModel.Value IsNot Nothing AndAlso ajaxModel.Value.Count > 0 AndAlso Not String.IsNullOrEmpty(ajaxModel.Value(0)) Then
                            FileLogger.Info(LoggerName, "Caricamento metadati dinamici.")
                            Dim metadataModel As MetadataModel = JsonConvert.DeserializeObject(Of MetadataModel)(ajaxModel.Value(0))
                            LoadControls(metadataModel)
                            SetControlValue(metadataModel, True)
                        End If

                    Catch ex As Exception
                        FileLogger.Error(LoggerName, "Impossibile caricare i metadati dinamici del fascicolo.", ex)
                        AjaxManager.Alert("Impossibile caricare i metadati dinamici del fascicolo.")
                        Return
                    End Try

                    Exit Select
            End Select
        End If
    End Sub


End Class
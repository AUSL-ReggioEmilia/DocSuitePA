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
Imports VecompSoftware.Services.AnagraficaANAS.Models
Imports VecompSoftware.Services.Logging

Public Class uscDynamicMetadata
    Inherits DocSuite2008BaseControl

    Public Const DYNAMIC_LABEL_NAME_FORMAT As String = "lbl_"
    Public Const DYNAMIC_FIELD_NAME_FORMAT As String = "field_"
    Public Const DYNAMIC_VALIDATOR_NAME_FORMAT As String = "validator_"
    Public Const CONTROL_BOOL_ITEM = "boolItem"
    Public Const CONTROL_ENUM_ITEM = "enumItem"
    Public Const CONTROL_TEXT_ITEM = "textItem"
    Public Const CONTROL_DATE_ITEM = "dateItem"
    Public Const CONTROL_NUMBER_ITEM = "numberItem"
    Public Const CONTROL_DISCUSSION_ITEM = "discussionItem"
    Private Shared metadataModel As MetadataDesignerModel
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

    Private Property CurrentMetadataDesignerModel As MetadataDesignerModel
        Get
            If ViewState(String.Format("{0}_CurrentMetadataDesignerModel", ID)) IsNot Nothing Then
                Return DirectCast(ViewState(String.Format("{0}_CurrentMetadataDesignerModel", ID)), MetadataDesignerModel)
            End If
            Return Nothing
        End Get
        Set(ByVal value As MetadataDesignerModel)
            ViewState(String.Format("{0}_CurrentMetadataDesignerModel", ID)) = value
        End Set
    End Property

    Private Property CurrentMetadataValueModels As ICollection(Of MetadataValueModel)
        Get
            If ViewState(String.Format("{0}_CurrentMetadataValueModels", ID)) IsNot Nothing Then
                Return JsonConvert.DeserializeObject(Of ICollection(Of MetadataValueModel))(ViewState(String.Format("{0}_CurrentMetadataValueModels", ID)).ToString())
            End If
            Return Nothing
        End Get
        Set(ByVal value As ICollection(Of MetadataValueModel))
            ViewState(String.Format("{0}_CurrentMetadataValueModels", ID)) = JsonConvert.SerializeObject(value)
        End Set
    End Property

    Public Property ValidationDisabled As Boolean

    Public Property FascicleInsertCommonIdEvent As String

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            CurrentMetadataDesignerModel = Nothing
        End If

        If CurrentMetadataDesignerModel IsNot Nothing Then
            LoadControls(CurrentMetadataDesignerModel, CurrentMetadataValueModels)
        End If
    End Sub

    Public Sub LoadControls(metadataModel As MetadataDesignerModel, metadataValues As ICollection(Of MetadataValueModel))
        If metadataModel Is Nothing Then
            Exit Sub
        End If
        CurrentMetadataValueModels = metadataValues
        LoadControlsWithPosition(metadataModel)
    End Sub

    Private Sub LoadControlsWithPosition(metadataModel As MetadataDesignerModel)
        dynamicPageContent.Controls.Clear()
        Dim fields As List(Of KeyValuePair(Of String, BaseFieldModel)) = OrderControlsByPosition(metadataModel)
        Dim layout As RadPageLayout = New RadPageLayout()
        For Each field As KeyValuePair(Of String, BaseFieldModel) In fields
            Dim controlName As String = field.Key
            Select Case controlName
                Case CONTROL_BOOL_ITEM
                    CreateBoolControl(layout, field.Value)
                Case CONTROL_ENUM_ITEM
                    Dim enumField As EnumFieldModel = metadataModel.EnumFields.FirstOrDefault(Function(x) x.KeyName = field.Value.KeyName)
                    CreateEnumControl(layout, enumField)
                Case CONTROL_TEXT_ITEM
                    Dim textField As TextFieldModel = metadataModel.TextFields.FirstOrDefault(Function(x) x.KeyName = field.Value.KeyName)
                    CreateTextControl(layout, textField)
                Case CONTROL_DATE_ITEM
                    CreateDateControl(layout, field.Value)
                Case CONTROL_NUMBER_ITEM
                    CreateNumberControl(layout, field.Value)
                Case CONTROL_DISCUSSION_ITEM
                    Dim discussionField As DiscussionFieldModel = metadataModel.DiscussionFields.FirstOrDefault(Function(x) x.KeyName = field.Value.KeyName)
                    CreateDiscussionControl(layout, discussionField)
            End Select
        Next
        dynamicPageContent.Controls.Add(layout)
        CurrentMetadataDesignerModel = metadataModel

    End Sub

    Private Function OrderControlsByPosition(metadataModel As MetadataDesignerModel) As List(Of KeyValuePair(Of String, BaseFieldModel))
        Dim fields As List(Of KeyValuePair(Of String, BaseFieldModel)) = New List(Of KeyValuePair(Of String, BaseFieldModel))()
        For Each textItem As TextFieldModel In metadataModel.TextFields
            fields.Add(New KeyValuePair(Of String, BaseFieldModel)(CONTROL_TEXT_ITEM, textItem))
        Next
        For Each numberItem As BaseFieldModel In metadataModel.NumberFields
            fields.Add(New KeyValuePair(Of String, BaseFieldModel)(CONTROL_NUMBER_ITEM, numberItem))
        Next
        For Each dateItem As BaseFieldModel In metadataModel.DateFields
            fields.Add(New KeyValuePair(Of String, BaseFieldModel)(CONTROL_DATE_ITEM, dateItem))
        Next
        For Each boolItem As BaseFieldModel In metadataModel.BoolFields
            fields.Add(New KeyValuePair(Of String, BaseFieldModel)(CONTROL_BOOL_ITEM, boolItem))
        Next
        For Each enumItem As EnumFieldModel In metadataModel.EnumFields
            fields.Add(New KeyValuePair(Of String, BaseFieldModel)(CONTROL_ENUM_ITEM, enumItem))
        Next
        For Each discussionItem As DiscussionFieldModel In metadataModel.DiscussionFields
            fields.Add(New KeyValuePair(Of String, BaseFieldModel)(CONTROL_DISCUSSION_ITEM, discussionItem))
        Next

        Return fields.OrderBy(Function(x) x.Value.Position).ToList()
    End Function

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
            control = New LabelStructure().GetStructure(fieldControlId, FindControlValue(textItem), String.Empty)
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
            If Not String.IsNullOrEmpty(FindControlValue(numberItem)) AndAlso Double.TryParse(FindControlValue(numberItem), n) Then
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
            If Not String.IsNullOrEmpty(FindControlValue(dateItem)) AndAlso DateTime.TryParse(FindControlValue(dateItem), d) Then
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
        If IsReadOnly AndAlso Not String.IsNullOrEmpty(FindControlValue(boolItem)) Then
            Boolean.TryParse(FindControlValue(boolItem), boolValue)
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
            control = New LabelStructure().GetStructure(fieldControlId, FindControlValue(enumItem), String.Empty)
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

    Public Sub SetControlValue(metadataModel As MetadataDesignerModel, Optional isDefault As Boolean = False)
        For Each textItem As TextFieldModel In metadataModel.TextFields
            SetControlValue(WebHelper.SafeControlIdName(String.Concat(DYNAMIC_FIELD_NAME_FORMAT, textItem.Label, textItem.Position)), If(isDefault, textItem.DefaultValue, FindControlValue(textItem)))
            AddAttributeToControl(WebHelper.SafeControlIdName(String.Concat(DYNAMIC_FIELD_NAME_FORMAT, textItem.Label, textItem.Position)), textItem.KeyName)
        Next
        For Each numberItem As BaseFieldModel In metadataModel.NumberFields
            SetControlValue(WebHelper.SafeControlIdName(String.Concat(DYNAMIC_FIELD_NAME_FORMAT, numberItem.Label, numberItem.Position)), If(isDefault, numberItem.DefaultValue, FindControlValue(numberItem)))
            AddAttributeToControl(WebHelper.SafeControlIdName(String.Concat(DYNAMIC_FIELD_NAME_FORMAT, numberItem.Label, numberItem.Position)), numberItem.KeyName)
        Next
        For Each dateItem As BaseFieldModel In metadataModel.DateFields
            SetControlValue(WebHelper.SafeControlIdName(String.Concat(DYNAMIC_FIELD_NAME_FORMAT, dateItem.Label, dateItem.Position)), If(isDefault, dateItem.DefaultValue, FindControlValue(dateItem)))
            AddAttributeToControl(WebHelper.SafeControlIdName(String.Concat(DYNAMIC_FIELD_NAME_FORMAT, dateItem.Label, dateItem.Position)), dateItem.KeyName)
        Next
        For Each boolItem As BaseFieldModel In metadataModel.BoolFields
            SetControlValue(WebHelper.SafeControlIdName(String.Concat(DYNAMIC_FIELD_NAME_FORMAT, boolItem.Label, boolItem.Position)), If(isDefault, boolItem.DefaultValue, FindControlValue(boolItem)))
            AddAttributeToControl(WebHelper.SafeControlIdName(String.Concat(DYNAMIC_FIELD_NAME_FORMAT, boolItem.Label, boolItem.Position)), boolItem.KeyName)
        Next
        For Each enumItem As EnumFieldModel In metadataModel.EnumFields
            SetControlValue(WebHelper.SafeControlIdName(String.Concat(DYNAMIC_FIELD_NAME_FORMAT, enumItem.Label, enumItem.Position)), If(isDefault, enumItem.DefaultValue, FindControlValue(enumItem)))
            AddAttributeToControl(WebHelper.SafeControlIdName(String.Concat(DYNAMIC_FIELD_NAME_FORMAT, enumItem.Label, enumItem.Position)), enumItem.KeyName)
        Next
        For Each discussionItem As DiscussionFieldModel In metadataModel.DiscussionFields
            SetControlValue(WebHelper.SafeControlIdName(String.Concat(DYNAMIC_FIELD_NAME_FORMAT, discussionItem.Label, discussionItem.Position)), If(isDefault, discussionItem.DefaultValue, FindControlValue(discussionItem)))
            AddAttributeToControl(WebHelper.SafeControlIdName(String.Concat(DYNAMIC_FIELD_NAME_FORMAT, discussionItem.Label, discussionItem.Position)), discussionItem.KeyName)
        Next
    End Sub

    Private Function FindControlValue(field As BaseFieldModel) As String
        If CurrentMetadataValueModels Is Nothing Then
            Return String.Empty
        End If
        Dim metadataValue As MetadataValueModel = CurrentMetadataValueModels.SingleOrDefault(Function(x) x.KeyName = field.KeyName)
        Return If(metadataValue Is Nothing OrElse String.IsNullOrEmpty(metadataValue.Value), String.Empty, metadataValue.Value)
    End Function

    Private Sub AddAttributeToControl(controlId As String, keyname As String)
        If String.IsNullOrEmpty(controlId) OrElse String.IsNullOrEmpty(keyname) Then
            Exit Sub
        End If

        Dim ctrl As Control = dynamicPageContent.FindControl(controlId)
        If ctrl IsNot Nothing Then
            Dim controlName As String = ctrl.GetType().Name
            Select Case controlName
                Case _labelTypeName
                    Dim control As Label = DirectCast(ctrl, Label)
                    control.Attributes.Add("KeyName", keyname)
                Case _checkBoxTypeName
                    Dim control As CheckBox = DirectCast(ctrl, CheckBox)
                    control.Attributes.Add("KeyName", keyname)
                Case _radDropDownListTypeName
                    Dim control As RadDropDownList = DirectCast(ctrl, RadDropDownList)
                    control.Attributes.Add("KeyName", keyname)
                Case _radComboBoxTypeName
                    Dim control As RadComboBox = DirectCast(ctrl, RadComboBox)
                    control.Attributes.Add("KeyName", keyname)
                    control.DataBind()
                Case _radTextBoxTypeName
                    Dim control As RadTextBox = DirectCast(ctrl, RadTextBox)
                    control.Attributes.Add("KeyName", keyname)
                Case _radDatePickerTypeName
                    Dim control As RadDatePicker = DirectCast(ctrl, RadDatePicker)
                    control.Attributes.Add("KeyName", keyname)

                Case _radNumericTextBoxTypeName
                    Dim control As RadNumericTextBox = DirectCast(ctrl, RadNumericTextBox)
                    control.Attributes.Add("KeyName", keyname)
            End Select
        End If
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

    Public Function GetControlValues() As Tuple(Of MetadataDesignerModel, ICollection(Of MetadataValueModel))
        If CurrentMetadataDesignerModel Is Nothing Then
            Return Nothing
        End If

        If CurrentMetadataValueModels Is Nothing Then
            CurrentMetadataValueModels = New List(Of MetadataValueModel)()
        End If

        Dim tmpValues As ICollection(Of MetadataValueModel) = CurrentMetadataValueModels
        For Each item As TextFieldModel In CurrentMetadataDesignerModel.TextFields
            FillPageControl(item, tmpValues)
        Next
        For Each item As BaseFieldModel In CurrentMetadataDesignerModel.NumberFields
            FillPageControl(item, tmpValues)
        Next
        For Each item As BaseFieldModel In CurrentMetadataDesignerModel.BoolFields
            FillPageControl(item, tmpValues)
        Next
        For Each item As BaseFieldModel In CurrentMetadataDesignerModel.DateFields
            FillPageControl(item, tmpValues)
        Next
        For Each item As EnumFieldModel In CurrentMetadataDesignerModel.EnumFields
            FillPageControl(item, tmpValues)
        Next
        For Each item As DiscussionFieldModel In CurrentMetadataDesignerModel.DiscussionFields
            FillPageControl(item, tmpValues)
        Next

        Return New Tuple(Of MetadataDesignerModel, ICollection(Of MetadataValueModel))(CurrentMetadataDesignerModel, tmpValues)
    End Function

    Private Sub FillPageControl(element As BaseFieldModel, dynamicValues As ICollection(Of MetadataValueModel))
        Dim control As Control = dynamicPageContent.FindControl(WebHelper.SafeControlIdName(String.Concat(DYNAMIC_FIELD_NAME_FORMAT, element.Label, element.Position)))
        If control IsNot Nothing Then
            Dim controlName As String = control.GetType().Name
            Dim dynamicValue As MetadataValueModel = dynamicValues.SingleOrDefault(Function(x) x.KeyName = element.KeyName)
            If dynamicValue Is Nothing Then
                dynamicValue = New MetadataValueModel()
                dynamicValue.KeyName = element.KeyName
                dynamicValues.Add(dynamicValue)
            End If
            Select Case controlName
                Case _checkBoxTypeName
                    dynamicValue.Value = DirectCast(control, CheckBox).Checked.ToString()
                Case _radDropDownListTypeName
                    dynamicValue.Value = DirectCast(control, RadDropDownList).SelectedText
                Case _radComboBoxTypeName
                    dynamicValue.Value = DirectCast(control, RadComboBox).SelectedItem.Text
                Case _radTextBoxTypeName
                    Dim value As String = DirectCast(control, RadTextBox).Text
                    Dim discussion As DiscussionFieldModel
                    dynamicValue.Value = value

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
                        End If

                        For Each item As CommentFieldModel In discussion.Comments
                            item.Author = item.Author.Replace("\", "\\")
                        Next
                    End If


                Case _radDatePickerTypeName
                    If DirectCast(control, RadDatePicker).SelectedDate.HasValue AndAlso DirectCast(control, RadDatePicker).SelectedDate.Value > DateTime.MinValue Then
                        dynamicValue.Value = DirectCast(control, RadDatePicker).SelectedDate.Value.ToString("o")
                    End If

                Case _radNumericTextBoxTypeName
                    If DirectCast(control, RadNumericTextBox).Value.HasValue Then
                        dynamicValue.Value = DirectCast(control, RadNumericTextBox).Value.Value.ToString()
                    End If

            End Select
        End If
    End Sub

    Public Sub PopulateControlValues(metadataModel As MetadataDesignerModel, setiContact As AnagraficaAnAsModel)
        For Each textItem As TextFieldModel In metadataModel.TextFields
            Dim ctrl As Control = dynamicPageContent.FindControl(WebHelper.SafeControlIdName(String.Concat(DYNAMIC_FIELD_NAME_FORMAT, textItem.Label, textItem.Position)))
            Dim control As RadTextBox = DirectCast(ctrl, RadTextBox)

            If (control.Attributes("KeyName") = textItem.KeyName) Then
                SetControlValue(WebHelper.SafeControlIdName(String.Concat(DYNAMIC_FIELD_NAME_FORMAT, textItem.Label, textItem.Position)), FillValue(setiContact, textItem.KeyName))
            End If

        Next

        For Each dateItem As BaseFieldModel In metadataModel.DateFields
            Dim ctrl As Control = dynamicPageContent.FindControl(WebHelper.SafeControlIdName(String.Concat(DYNAMIC_FIELD_NAME_FORMAT, dateItem.Label, dateItem.Position)))
            Dim control As RadDatePicker = DirectCast(ctrl, RadDatePicker)

            If (control.Attributes("KeyName") = dateItem.KeyName) Then
                SetControlValue(WebHelper.SafeControlIdName(String.Concat(DYNAMIC_FIELD_NAME_FORMAT, dateItem.Label, dateItem.Position)), FillValue(setiContact, dateItem.KeyName))
            End If

        Next
        For Each numberItem As BaseFieldModel In metadataModel.NumberFields
            Dim ctrl As Control = dynamicPageContent.FindControl(WebHelper.SafeControlIdName(String.Concat(DYNAMIC_FIELD_NAME_FORMAT, numberItem.Label, numberItem.Position)))
            Dim control As RadNumericTextBox = DirectCast(ctrl, RadNumericTextBox)

            If (control.Attributes("KeyName") = numberItem.KeyName) Then
                SetControlValue(WebHelper.SafeControlIdName(String.Concat(DYNAMIC_FIELD_NAME_FORMAT, numberItem.Label, numberItem.Position)), FillValue(setiContact, numberItem.KeyName))
            End If

        Next
        For Each boolItem As BaseFieldModel In metadataModel.BoolFields
            Dim ctrl As Control = dynamicPageContent.FindControl(WebHelper.SafeControlIdName(String.Concat(DYNAMIC_FIELD_NAME_FORMAT, boolItem.Label, boolItem.Position)))
            Dim control As CheckBox = DirectCast(ctrl, CheckBox)

            If (control.Attributes("KeyName") = boolItem.KeyName) Then
                SetControlValue(WebHelper.SafeControlIdName(String.Concat(DYNAMIC_FIELD_NAME_FORMAT, boolItem.Label, boolItem.Position)), FillValue(setiContact, boolItem.KeyName))
            End If

        Next
        For Each enumItem As EnumFieldModel In metadataModel.EnumFields
            Dim ctrl As Control = dynamicPageContent.FindControl(WebHelper.SafeControlIdName(String.Concat(DYNAMIC_FIELD_NAME_FORMAT, enumItem.Label, enumItem.Position)))

            Dim controlName As String = ctrl.GetType().Name
            Select Case controlName
                Case _radDropDownListTypeName
                    Dim control As RadDropDownList = DirectCast(ctrl, RadDropDownList)
                    If (control.Attributes("KeyName") = enumItem.KeyName) Then
                        SetControlValue(WebHelper.SafeControlIdName(String.Concat(DYNAMIC_FIELD_NAME_FORMAT, enumItem.Label, enumItem.Position)), FillValue(setiContact, enumItem.KeyName))
                    End If
                Case _radComboBoxTypeName
                    Dim control As RadComboBox = DirectCast(ctrl, RadComboBox)
                    If (control.Attributes("KeyName") = enumItem.KeyName) Then
                        SetControlValue(WebHelper.SafeControlIdName(String.Concat(DYNAMIC_FIELD_NAME_FORMAT, enumItem.Label, enumItem.Position)), FillValue(setiContact, enumItem.KeyName))
                    End If
            End Select
        Next
        For Each discussionItem As DiscussionFieldModel In metadataModel.DiscussionFields
            Dim ctrl As Control = dynamicPageContent.FindControl(WebHelper.SafeControlIdName(String.Concat(DYNAMIC_FIELD_NAME_FORMAT, discussionItem.Label, discussionItem.Position)))
            Dim control As RadTextBox = DirectCast(ctrl, RadTextBox)

            If (control.Attributes("KeyName") = discussionItem.KeyName) Then
                SetControlValue(WebHelper.SafeControlIdName(String.Concat(DYNAMIC_FIELD_NAME_FORMAT, discussionItem.Label, discussionItem.Position)), FillValue(setiContact, discussionItem.KeyName))
            End If
        Next
    End Sub

    Private Function FillValue(setiContact As AnagraficaAnAsModel, keyName As String) As String
        Try
            Dim objType As Type = setiContact.GetType()
            Dim pInfo As Reflection.PropertyInfo = objType.GetProperty(keyName)
            Dim PropValue As Object = pInfo.GetValue(setiContact, Reflection.BindingFlags.GetProperty, Nothing, Nothing, Nothing)
            Return CType(PropValue, String)
        Catch ex As Exception
            Return String.Empty
        End Try
    End Function

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
                    CurrentMetadataDesignerModel = Nothing
                    Exit Select
                Case "LoadControls"
                    Try
                        If ajaxModel.Value IsNot Nothing AndAlso ajaxModel.Value.Count > 0 AndAlso Not String.IsNullOrEmpty(ajaxModel.Value(0)) Then
                            FileLogger.Info(LoggerName, "Caricamento metadati dinamici.")
                            metadataModel = JsonConvert.DeserializeObject(Of MetadataDesignerModel)(ajaxModel.Value(0))
                            LoadControls(metadataModel, Nothing)
                            SetControlValue(metadataModel, True)
                        End If

                    Catch ex As Exception
                        FileLogger.Error(LoggerName, "Impossibile caricare i metadati dinamici del fascicolo.", ex)
                        AjaxManager.Alert("Impossibile caricare i metadati dinamici del fascicolo.")
                        Return
                    End Try
                Case "PopulateFields"
                    Dim setiContact As AnagraficaAnAsModel = JsonConvert.DeserializeObject(Of AnagraficaAnAsModel)(ajaxModel.Value(0))

                    PopulateControlValues(metadataModel, setiContact)
                    Exit Select
            End Select
        End If
    End Sub

End Class
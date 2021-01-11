Imports System.ComponentModel
Imports System.Web.UI
Imports System.Web.UI.WebControls
Imports System.Drawing
Imports System.Web.UI.HtmlControls
Imports VecompSoftware.Helpers.ExtensionMethods

<ToolboxBitmap(GetType(Button)), DefaultEvent("Click"), Obsolete("Usare RadButton")>
Public Class PromptClickOnceButton
    Inherits Button
    Implements IPostBackEventHandler

#Region " Fields "

    Private Const OnceClickBtnName As String = "__onceClickBtn"

#End Region

#Region " Properties "

    <Browsable(True), Category("Behavior")>
    Public Property DisableAfterClick() As Boolean
        Get
            Dim o As Object = ViewState("DisableAfterClick")
            Return (o IsNot Nothing) AndAlso CBool(o)
        End Get
        Set(ByVal value As Boolean)
            ViewState("DisableAfterClick") = value
        End Set
    End Property

    <Browsable(True), Category("Behavior")>
    Public Property ConfirmBeforeSubmit() As Boolean
        Get
            Dim o As Object = ViewState("ConfirmBeforeSubmit")
            Return (o Is Nothing) OrElse CBool(o)
        End Get
        Set(ByVal value As Boolean)
            ViewState("ConfirmBeforeSubmit") = value
        End Set
    End Property

    <Browsable(True), Category("Appearance")>
    Public Property ConfirmationMessage() As String
        Get
            Dim o As Object = ViewState("ConfirmationMessage")
            Return If(o Is Nothing, "", CStr(o))
        End Get
        Set(ByVal value As String)
            ViewState("ConfirmationMessage") = value
        End Set
    End Property

    <Browsable(False)>
    Friend ReadOnly Property GetClientValidate() As String
        Get
            Return "if (typeof(Page_ClientValidate) == 'function') Page_ClientValidate(); "
        End Get
    End Property

    <Browsable(False)>
    Friend ReadOnly Property GetClickOnceJavascript() As String
        Get
            Return String.Format("this.disabled = true;if(typeof window.external.AutoCompleteSaveForm == 'function') {{ window.external.AutoCompleteSaveForm({0}) }};__doPostBack('{1}','');", Page.Form.UniqueID, UniqueID)
        End Get
    End Property

    <Browsable(False)>
    Friend ReadOnly Property GetClickOnceClientValidate() As String
        Get
            Return String.Format("if (typeof(Page_ClientValidate) == 'function') {{ if(Page_ClientValidate()) {{ {0} }}}} else {{ {0} }}", GetClickOnceJavascript)
        End Get
    End Property

    <Browsable(False)>
    Friend ReadOnly Property GetConfirmJavascript() As String
        Get
            Return String.Format("return confirm('{0}');", EscapedConfirmationMessage())
        End Get
    End Property

    <Browsable(False)>
    Friend ReadOnly Property GetConfirmClientValidate() As String
        Get
            Dim confirmReturn As String = String.Format("return confirm('{0}')", EscapedConfirmationMessage)
            Return String.Format("if (typeof(Page_ClientValidate) === 'function') {{ if(Page_ClientValidate()) {{ {0} }}}} else {{ {0} }}", confirmReturn)
        End Get
    End Property

    <Browsable(False)>
    Friend ReadOnly Property GetConfirmClickOnce() As String
        Get
            Return String.Format(" if(confirm('{0}')) {{ {1} }} else {{return false;}}", EscapedConfirmationMessage, GetClickOnceJavascript)
        End Get
    End Property

    <Browsable(False)>
    Friend ReadOnly Property GetConfirmClickOnceClientValidate() As String
        Get
            Return String.Format("if (typeof(Page_ClientValidate) === 'function') {{ if(Page_ClientValidate()) {{ {0} }}}} else {{ {0} }}", GetConfirmClickOnce)
        End Get
    End Property

#End Region

#Region " Constructors "

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

#Region " Methods "

    Protected Overrides Sub RenderContents(ByVal writer As HtmlTextWriter)
    End Sub

    Protected Overrides Sub AddAttributesToRender(ByVal writer As HtmlTextWriter)
        Dim strOnClick As String = String.Empty

        If IsNothing(Page) Then
            Page.VerifyRenderingInServerForm(Me)
        End If

        writer.AddAttribute(HtmlTextWriterAttribute.Type, "submit")
        writer.AddAttribute(HtmlTextWriterAttribute.Name, UniqueID)
        writer.AddAttribute(HtmlTextWriterAttribute.Value, Text)

        If Not IsNothing(Page) And CausesValidation And Page.Validators.Count > 0 Then
            If DisableAfterClick Then
                If ConfirmBeforeSubmit Then
                    strOnClick = GetConfirmClickOnceClientValidate
                Else
                    strOnClick = GetClickOnceClientValidate
                End If
            Else
                If ConfirmBeforeSubmit Then
                    strOnClick = GetConfirmClientValidate
                Else
                    strOnClick = GetClientValidate
                End If
            End If
            If Attributes.Count > 0 And Attributes("onclick") IsNot Nothing Then
                strOnClick = String.Concat(Attributes("onclick"), strOnClick)
                Attributes.Remove("onclick")
            End If

            writer.AddAttribute("language", "javascript")
            writer.AddAttribute(HtmlTextWriterAttribute.Onclick, strOnClick)

        ElseIf DisableAfterClick Or ConfirmBeforeSubmit Then
            If DisableAfterClick Then
                If ConfirmBeforeSubmit Then
                    strOnClick = GetConfirmClickOnce
                Else
                    strOnClick = GetClickOnceJavascript
                End If
            Else
                If ConfirmBeforeSubmit Then
                    strOnClick = GetConfirmJavascript
                End If
            End If

            If Attributes.Count > 0 And Attributes("onclick") IsNot Nothing Then
                strOnClick = String.Concat(Attributes("onclick"), strOnClick)
                Attributes.Remove("onclick")
            End If

            writer.AddAttribute("language", "javascript")
            writer.AddAttribute(HtmlTextWriterAttribute.Onclick, strOnClick)
        End If

        MyBase.AddAttributesToRender(writer)

    End Sub

    Protected Overrides Sub OnInit(ByVal e As EventArgs)
        If DisableAfterClick And Not IsHiddenFieldRegistered() Then
            Page.ClientScript.RegisterHiddenField(OnceClickBtnName, "")
        End If
        MyBase.OnInit(e)
    End Sub

    Private Function IsHiddenFieldRegistered() As Boolean
        For Each ctl As Control In Page.Controls
            If TypeOf ctl Is HtmlInputHidden AndAlso ctl.ID.Eq(OnceClickBtnName) Then
                Return True
            End If
        Next
        Return False
    End Function

    Private Function EscapedConfirmationMessage() As String
        Return Replace(ConfirmationMessage, "'", "\'")
    End Function

#End Region

End Class
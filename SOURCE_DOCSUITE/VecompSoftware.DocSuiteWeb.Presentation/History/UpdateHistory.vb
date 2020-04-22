Imports System
Imports System.Collections
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Diagnostics
Imports System.Text
Imports System.Web
Imports System.Web.UI

#Region "Web Resources"
<Assembly: WebResource("VecompSoftware.DocSuiteWeb.Presentation.UpdateControls.js", "text/javascript")> 
<Assembly: WebResource("VecompSoftware.DocSuiteWeb.Presentation.UpdateControls.debug.js", "text/javascript")> 
#End Region

<DefaultEvent("Navigate"), DefaultProperty("HistoryPagePath"), Designer(GetType(UpdateHistoryDesigner)), NonVisualControl()> _
Public Class UpdateHistory
    Inherits Control
    Implements IScriptControl
    Implements IPostBackEventHandler

    Private Shared ReadOnly NavigateEventKey As New Object()

    Private _scriptManager As ScriptManager
    Private _historyPagePath As String

    Private _historyEntry As String
    Private _initialEntry As String

    <Category("Behavior"), DefaultValue("~/History.htm"), Description("The path of an empty HTML page used to navigate to when adding history entries to the browser's navigation stack.")> _
    Public Property HistoryPagePath() As String
        Get
            If [String].IsNullOrEmpty(_historyPagePath) Then
                Return "~/History.htm"
            End If
            Return _historyPagePath
        End Get
        Set(ByVal value As String)
            _historyPagePath = value
        End Set
    End Property

    <Browsable(False), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)> _
    Public Overloads Overrides Property Visible() As Boolean
        Get
            Return MyBase.Visible
        End Get
        Set(ByVal value As Boolean)
            Throw New NotImplementedException()
        End Set
    End Property

    <Category("Behavior"), Description("The navigate event occurs any time the user navigates to logical view.")> _
    Public Custom Event Navigate As HistoryEventHandler
        AddHandler(ByVal value As HistoryEventHandler)
            Events.[AddHandler](NavigateEventKey, value)
        End AddHandler
        RemoveHandler(ByVal value As HistoryEventHandler)
            Events.[RemoveHandler](NavigateEventKey, value)
        End RemoveHandler
        RaiseEvent(ByVal sender As Object, ByVal e As HistoryEventArgs)
            Dim handler As HistoryEventHandler = DirectCast(Events(NavigateEventKey), HistoryEventHandler)
            If handler IsNot Nothing Then
                handler.Invoke(Me, e)
            End If
        End RaiseEvent
    End Event

    Public Sub AddEntry(ByVal entryName As String)
        If [String].IsNullOrEmpty(entryName) Then
            Throw New ArgumentNullException("entryName")
        End If

        Dim scriptManager__1 As ScriptManager = ScriptManager.GetCurrent(Page)
        If _scriptManager Is Nothing Then
            Throw New InvalidOperationException("You must add a ScriptManager to the page to use the UpdateHistory control.")
        End If

        If scriptManager__1.IsInAsyncPostBack Then
            _historyEntry = entryName
        End If
    End Sub

    Public Shared Function GetCurrent(ByVal page As Page) As UpdateHistory
        If page Is Nothing Then
            Throw New ArgumentNullException("page")
        End If

        Return TryCast(page.Items(GetType(UpdateHistory)), UpdateHistory)
    End Function

    Protected Overridable Function GetScriptDescriptors() As IEnumerable(Of ScriptDescriptor) Implements IScriptControl.GetScriptDescriptors
        Dim optionsJSONBuilder As New StringBuilder()
        optionsJSONBuilder.Append("{ dataID: '")
        optionsJSONBuilder.Append(ClientID)
        optionsJSONBuilder.Append("', initialEntry: ")
        If _initialEntry Is Nothing Then
            optionsJSONBuilder.Append("null")
        Else
            optionsJSONBuilder.Append("'")
            optionsJSONBuilder.Append(_initialEntry)
            optionsJSONBuilder.Append("'")
        End If
        optionsJSONBuilder.Append(", postbackID: '")
        optionsJSONBuilder.Append(UniqueID)
        optionsJSONBuilder.Append("'}")

        Dim componentDescriptor As New ScriptComponentDescriptor("nStuff.UpdateHistory")
        componentDescriptor.AddScriptProperty("options", optionsJSONBuilder.ToString())

        Return New ScriptDescriptor() {componentDescriptor}
    End Function

    Protected Overridable Function GetScriptReferences() As IEnumerable(Of ScriptReference) Implements IScriptControl.GetScriptReferences
        Dim assembly As String = GetType(UpdateHistory).Assembly.FullName
        Return New ScriptReference() {New ScriptReference(Page.ClientScript.GetWebResourceUrl(Me.GetType(), "VecompSoftware.DocSuiteWeb.Presentation.UpdateControls.js"))}
    End Function

    Protected Overloads Overrides Sub OnInit(ByVal e As EventArgs)
        MyBase.OnInit(e)

        If DesignMode Then
            Exit Sub
        End If

        If UpdateHistory.GetCurrent(Page) IsNot Nothing Then
            Throw New InvalidOperationException("You must add only a single UpdateHistory control to the page.")
        End If
        Page.Items(GetType(UpdateHistory)) = Me

        _scriptManager = ScriptManager.GetCurrent(Page)
        If _scriptManager Is Nothing Then
            Throw New InvalidOperationException("You must add a ScriptManager to the page to use the UpdateHistory control.")
        End If

        If Not Page.IsPostBack Then
            Dim url As String = Page.Request.RawUrl
            Dim hashIndex As Integer = url.LastIndexOf("#"c)
            If (hashIndex > 0) AndAlso (hashIndex < (url.Length - 1)) Then
                _initialEntry = url.Substring(hashIndex + 1)

                Dim he As New HistoryEventArgs(_initialEntry)
                OnNavigate(he)
            End If
        End If
    End Sub

    Protected Overridable Sub OnNavigate(ByVal e As HistoryEventArgs)
        Dim handler As HistoryEventHandler = DirectCast(Events(NavigateEventKey), HistoryEventHandler)
        If handler IsNot Nothing Then
            handler.Invoke(Me, e)
            'RaiseEvent Navigate()
        End If
    End Sub

    Protected Overloads Overrides Sub OnPreRender(ByVal e As EventArgs)
        MyBase.OnPreRender(e)

        Dim parent__1 As Control = Parent
        While parent__1 IsNot Nothing
            If TypeOf parent__1 Is UpdatePanel Then
                Throw New InvalidOperationException("An UpdateHistory control should not be placed within an UpdatePanel.")
            End If
            parent__1 = parent__1.Parent
        End While

        _scriptManager.RegisterScriptControl(Me)
        _scriptManager.RegisterAsyncPostBackControl(Me)

        If [String].IsNullOrEmpty(_historyEntry) = False Then
            _scriptManager.RegisterDataItem(Me, _historyEntry)
        End If
    End Sub

    Protected Overloads Overrides Sub Render(ByVal writer As HtmlTextWriter)
        _scriptManager.RegisterScriptDescriptors(Me)

        If Page.Request.Browser.Browser.Equals("IE", StringComparison.OrdinalIgnoreCase) Then
            writer.AddAttribute(HtmlTextWriterAttribute.Id, "__historyFrame")
            writer.AddAttribute(HtmlTextWriterAttribute.Src, ResolveClientUrl(HistoryPagePath))
            writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none")
            writer.RenderBeginTag(HtmlTextWriterTag.Iframe)
            writer.RenderEndTag()
        End If

        ' Render an empty span representing this control... otherwise
        ' PageRequestManager complains when it can't find the element causing
        ' the postback.
        writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID)
        writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none")
        writer.RenderBeginTag(HtmlTextWriterTag.Span)
        writer.RenderEndTag()
    End Sub

#Region "IScriptControl Members"
    'Private Function GetScriptDescriptors() As IEnumerable(Of ScriptDescriptor) Implements IScriptControl.GetScriptDescriptors
    '    Return GetHistoryScriptDescriptors()
    'End Function

    'Private Function GetScriptReferences() As IEnumerable(Of ScriptReference) Implements IScriptControl.GetScriptReferences
    '    Return GetHistoryScriptReferences()
    'End Function
#End Region

#Region "IPostBackEventHandler Members"
    Private Sub RaisePostBackEvent(ByVal eventArgument As String) Implements IPostBackEventHandler.RaisePostBackEvent
        Dim e As New HistoryEventArgs(eventArgument)
        OnNavigate(e)
    End Sub
#End Region
End Class


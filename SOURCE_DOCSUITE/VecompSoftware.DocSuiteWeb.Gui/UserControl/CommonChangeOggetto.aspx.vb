Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Web

Partial Public Class CommonChangeOggetto
    Inherits CommBasePage

#Region " Properties "

    Private ReadOnly Property Year() As String
        Get
            Return Request.QueryString("Year")
        End Get
    End Property

    Private ReadOnly Property Number() As String
        Get
            Return Request.QueryString("Number")
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        MasterDocSuite.TitleVisible = False
        InitializeAjax()
        If Not IsPostBack Then
            InitializePage()
        End If
    End Sub

    Private Sub btnConferma_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnConferma.Click
        Dim [object] As String = HttpUtility.JavaScriptStringEncode(uscObject.Text)
        Dim reason As String = HttpUtility.JavaScriptStringEncode(txtObjectReason.Text)

        If txtObjectOld.Text = uscObject.Text Then
            AjaxAlert("Il campo oggetto non è stato modificato")
            Exit Sub
        End If

        AjaxManager.ResponseScripts.Add("ReturnValue('" & [object] & "','" & reason & "');")
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConferma, btnConferma)
    End Sub

    Private Sub InitializePage()
        Dim protocol As Protocol = Facade.ProtocolFacade.GetById(Year, Number)

        If protocol Is Nothing Then
            Throw New DocSuiteException("Protocollo n. " & ProtocolFacade.ProtocolFullNumber(Year, Number), "Protocollo Inesistente")
        End If

        txtObjectOld.MaxLength = ProtocolEnv.ObjectMaxLength
        txtObjectOld.Text = protocol.ProtocolObject
        txtObjectReasonOld.Text = protocol.ObjectChangeReason
        uscObject.Text = protocol.ProtocolObject
        uscObject.MaxLength = ProtocolEnv.ObjectMaxLength
        txtObjectReason.Text = protocol.ObjectChangeReason
    End Sub

#End Region

End Class
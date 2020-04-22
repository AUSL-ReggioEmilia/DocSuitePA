Imports System.Collections.Generic
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Data.Formatter
Imports VecompSoftware.DocSuiteWeb.Data

Public Class UtltTestSignature
    Inherits SuperAdminPage

#Region " Events "


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not Page.IsPostBack Then
            txtSignatureType.Text = DocSuiteContext.Current.ProtocolEnv.SignatureType.ToString()
            txtProtocolSignatureFormat.Text = DocSuiteContext.Current.ProtocolEnv.ProtocolSignatureFormat
            txtAttachmentSignatureFormat.Text = DocSuiteContext.Current.ProtocolEnv.AttachmentSignatureFormat
            txtAnnexedSignatureFormat.Text = DocSuiteContext.Current.ProtocolEnv.AnnexedSignatureFormat
            txtSignatureString.Text = DocSuiteContext.Current.ProtocolEnv.SignatureString
        End If
    End Sub

    Private Sub btnTest_Click(sender As Object, e As EventArgs) Handles btnTest.Click
        ' Salvo parametri originali
        Dim currentParameter As Integer = DocSuiteContext.Current.ProtocolEnv.SignatureType
        Dim currentProtocolSignatureFormat As String = DocSuiteContext.Current.ProtocolEnv.ProtocolSignatureFormat
        Dim currentAttachmentSignatureFormat As String = DocSuiteContext.Current.ProtocolEnv.AttachmentSignatureFormat
        Dim currentAnnexedSignatureFormat As String = DocSuiteContext.Current.ProtocolEnv.AnnexedSignatureFormat
        Dim currentSignatureString As String = DocSuiteContext.Current.ProtocolEnv.SignatureString
        Try
            ' Imposto nuovi parametri
            DocSuiteContext.Current.ProtocolEnv.SetParameter("SignatureType", txtSignatureType.Text)
            DocSuiteContext.Current.ProtocolEnv.SetParameter("ProtocolSignatureFormat", txtProtocolSignatureFormat.Text)
            DocSuiteContext.Current.ProtocolEnv.SetParameter("AttachmentSignatureFormat", txtAttachmentSignatureFormat.Text)
            DocSuiteContext.Current.ProtocolEnv.SetParameter("AnnexedSignatureFormat", txtAnnexedSignatureFormat.Text)
            DocSuiteContext.Current.ProtocolEnv.SetParameter("SignatureString", txtSignatureString.Text)

            ' Test vero e proprio
            Dim protocol As Protocol = Facade.ProtocolFacade.GetById(CType(txtYear.Value, Short), CType(txtNumber.Value, Integer))

            Dim tableSource As New List(Of Tuple(Of String, String))
            tableSource.Add(New Tuple(Of String, String)("Nullo", Facade.ProtocolFacade.GenerateSignature(protocol, protocol.RegistrationDate.ToLocalTime().DateTime, New ProtocolSignatureInfo() With {.DocumentType = ProtocolDocumentType.None})))
            tableSource.Add(New Tuple(Of String, String)("Documento principale", Facade.ProtocolFacade.GenerateSignature(protocol, protocol.RegistrationDate.ToLocalTime().DateTime, New ProtocolSignatureInfo() With {.DocumentType = ProtocolDocumentType.Main})))
            tableSource.Add(New Tuple(Of String, String)("Allegato", Facade.ProtocolFacade.GenerateSignature(protocol, protocol.RegistrationDate.ToLocalTime().DateTime, New ProtocolSignatureInfo() With {.DocumentType = ProtocolDocumentType.Attachment})))
            tableSource.Add(New Tuple(Of String, String)("Annesso", Facade.ProtocolFacade.GenerateSignature(protocol, protocol.RegistrationDate.ToLocalTime().DateTime, New ProtocolSignatureInfo() With {.DocumentType = ProtocolDocumentType.Annexed})))

            result.DataSource = tableSource
            result.DataBind()
        Catch ex As Exception
            FileLogger.Error(LoggerName, "Errore test segnatura", ex)
            AjaxAlert("Errore loggato.")
        Finally
            ' Ripristino parametri originali
            DocSuiteContext.Current.ProtocolEnv.SetParameter("SignatureType", currentParameter.ToString())
            DocSuiteContext.Current.ProtocolEnv.SetParameter("ProtocolSignatureFormat", currentProtocolSignatureFormat)
            DocSuiteContext.Current.ProtocolEnv.SetParameter("AttachmentSignatureFormat", currentAttachmentSignatureFormat)
            DocSuiteContext.Current.ProtocolEnv.SetParameter("AnnexedSignatureFormat", currentAnnexedSignatureFormat)
            DocSuiteContext.Current.ProtocolEnv.SetParameter("SignatureString", currentSignatureString)
        End Try
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnTest, result)
    End Sub

#End Region

End Class
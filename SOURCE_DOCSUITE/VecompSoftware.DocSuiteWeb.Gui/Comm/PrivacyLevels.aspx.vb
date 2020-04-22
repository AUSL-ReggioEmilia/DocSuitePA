Imports System.Collections.Generic
Imports System.Collections.Specialized
Imports System.Linq
Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models

Public Class PrivacyLevels
    Inherits CommonBasePage

#Region " Fields "
#End Region

#Region " Properties "
    Private ReadOnly Property Caption As String
        Get
            Return Request.QueryString.GetValueOrDefault("Caption", String.Empty)
        End Get
    End Property

    Private ReadOnly Property PrivacyLevel As Integer?
        Get
            Return Request.QueryString.GetValueOrDefault(Of Integer)("Level", Nothing)
        End Get
    End Property
    Private ReadOnly Property MinLevel As Integer?
        Get
            Return Request.QueryString.GetValueOrDefault(Of Integer)("Min", Nothing)
        End Get
    End Property

    Private ReadOnly Property DocumentInfoAttributes As String
        Get
            Return Request.QueryString.GetValueOrDefault("Doc", String.Empty)
        End Get
    End Property
#End Region

#Region " Events "
    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack() Then
            Initialize()
        End If
    End Sub

    Protected Sub btnConfirm_OnClick(sender As Object, e As EventArgs) Handles btnConfirm.Click
        Dim document As DocumentInfo = uscDocument.DocumentInfos.First()
        If document IsNot Nothing AndAlso document.Attributes.Any(Function(a) a.Key.Eq(BiblosFacade.DOCUMENT_POSITION_ATTRIBUTE)) Then
            Dim privacyLevel As String = document.Attributes(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE)
            AjaxManager.ResponseScripts.Add(String.Concat("CloseWindow('", privacyLevel, "');"))
        End If
    End Sub
#End Region

#Region " Methods "

    Protected Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnConfirm, btnConfirm, MasterDocSuite.AjaxFlatLoadingPanel)
    End Sub

    Protected Sub Initialize()
        Title = String.Concat("Livelli di ", PRIVACY_LABEL)
        MasterDocSuite.TitleVisible = False
        uscDocument.Caption = Caption
        If Not String.IsNullOrEmpty(DocumentInfoAttributes) Then
            Dim temp As IDictionary(Of String, String) = JsonConvert.DeserializeObject(Of IDictionary(Of String, String))(DocumentInfoAttributes)
            Dim collection As NameValueCollection = temp.Aggregate(New NameValueCollection(), Function(seed, current)
                                                                                                  seed.Add(current.Key, current.Value)
                                                                                                  Return seed
                                                                                              End Function)
            Dim doc As DocumentInfo = DocumentInfoFactory.BuildDocumentInfo(collection)
            If PrivacyLevel.HasValue Then
                doc.AddAttribute(BiblosFacade.PRIVACYLEVEL_ATTRIBUTE, PrivacyLevel.Value.ToString())
            End If
            uscDocument.LoadDocumentInfo(doc)
            If MinLevel.HasValue Then
                uscDocument.MinPrivacyLevel = MinLevel.Value
                uscDocument.RefreshDdlPrivacyLevel(True)
            End If
        End If
    End Sub
#End Region

End Class
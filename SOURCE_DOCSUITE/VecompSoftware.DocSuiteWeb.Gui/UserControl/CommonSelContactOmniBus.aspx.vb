Imports System.Collections.Generic
Imports System.Web
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.Services.OmniBus

Public Class CommonSelContactOmniBus
    Inherits CommBasePage

#Region " Fields "
    Private _currentService As Service
#End Region

#Region " Properties "
    Private ReadOnly Property CurrentService As Service
        Get
            If _currentService Is Nothing Then
                _currentService = New Service()
            End If
            Return _currentService
        End Get
    End Property

    Public ReadOnly Property CallerId() As String
        Get
            Return Request.QueryString("ParentID").Trim()
        End Get
    End Property
#End Region

#Region " Events "
    Protected Sub BtnFind_OnClick(sender As Object, e As EventArgs) Handles btnFind.Click
        Try
            Dim finderModel As OmniBusFinderModel = CreateFinder()
            Dim contacts As ICollection(Of OmniBusContactModel) = CurrentService.FindContacts(finderModel)
            rtvResults.Nodes()(0).Nodes.Clear()
            InsertNodes(contacts)
        Catch ex As Exception
            FileLogger.Error(LoggerName, String.Format("Errore in ricerca contatti: {0}", ex.Message), ex)
            AjaxAlert("Errore in ricerca contatti")
        End Try
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjax()
        If Not IsPostBack Then
            Initialize()
        End If
    End Sub
#End Region

#Region " Methods "
    Private Sub Initialize()

    End Sub

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnFind, rtvResults, MasterDocSuite.AjaxDefaultLoadingPanel)
    End Sub

    Private Function CreateFinder() As OmniBusFinderModel
        Dim finderModel As OmniBusFinderModel = New OmniBusFinderModel With {
            .CodiceApplicativo = ProtocolEnv.OmniBusApplicationKey
        }
        If Not String.IsNullOrEmpty(txtNome.Text) Then
            finderModel.Nome = txtNome.Text
        End If

        If Not String.IsNullOrEmpty(txtCognome.Text) Then
            finderModel.Cognome = txtCognome.Text
        End If

        If Not String.IsNullOrEmpty(txtCdc.Text) Then
            finderModel.CentroDiCosto = txtCdc.Text
        End If

        If Not String.IsNullOrEmpty(rcbDistretto.SelectedValue) Then
            finderModel.Distretto = rcbDistretto.SelectedValue
        End If
        Return finderModel
    End Function

    Private Sub InsertNodes(contacts As ICollection(Of OmniBusContactModel))
        If contacts.IsNullOrEmpty() Then
            Exit Sub
        End If

        For Each contact As OmniBusContactModel In contacts
            Dim dswContact As Contact = CreateContact(contact)
            Dim node As RadTreeNode = New RadTreeNode(String.Format("{0} ({1})", dswContact.DescriptionFormatByContactType, dswContact.EmailAddress), dswContact.FiscalCode)
            dswContact = Data.Contact.EscapingJSON(dswContact, Function(f) HttpUtility.HtmlEncode(f))
            node.Attributes.Add("serializedModel", HttpUtility.HtmlEncode(JsonConvert.SerializeObject(dswContact).Replace("'", "\'").Replace("\", "")))
            rtvResults.Nodes()(0).Nodes.Add(node)
        Next
    End Sub

    Private Function CreateContact(model As OmniBusContactModel) As Contact
        Dim contact As Contact = New Contact() With {.ContactType = New ContactType(ContactType.Person)}
        contact.Description = String.Concat(model.Cognome, "|", model.Nome)
        contact.BirthDate = model.DataDiNascita
        contact.EmailAddress = model.EmailUfficio
        contact.FiscalCode = model.CodiceFiscale
        contact.IsActive = 1
        contact.TelephoneNumber = If(String.IsNullOrEmpty(model.Telefono1), model.Telefono2, model.Telefono1)
        contact.Address = New Address() With
        {
            .Address = model.Via,
            .City = model.Città,
            .ZipCode = model.CAP
        }
        Return contact
    End Function
#End Region

End Class
Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data

Partial Class UtltIPAd
    Inherits UtltBasePage

#Region " Fields "

    Protected WithEvents TblProperty As Table

#End Region

#Region " Properties "

    ReadOnly Property Path() As String
        Get
            Return Request.QueryString("Path")
        End Get
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        MasterDocSuite.TitleVisible = False
        If Not Page.IsPostBack Then
            Initialize()
        End If
    End Sub

#End Region

#Region " Methods "

    Private Sub Initialize()
        ' Recupero i dati dell'IPA
        Dim ipaEntity As IPA = IPARetriever.GetIpaEntitieByPath(ProtocolEnv.LdapIndicePa, Path)
        If ipaEntity Is Nothing Then
            Exit Sub
        End If



        tableProperty.Rows.AddRaw(Nothing, Nothing, {50, 50}, {"Proprietà", "Valore"}, {"head", "head"})
        ' Parametri default
        tableProperty.Rows.AddRaw(Nothing, Nothing, {50, 50}, {"Descrizione", ipaEntity.Description}, {"label"})
        tableProperty.Rows.AddRaw(Nothing, Nothing, {50, 50}, {"Indirizzo", ipaEntity.Indirizzo}, {"label"})
        tableProperty.Rows.AddRaw(Nothing, Nothing, {50, 50}, {"CAP", ipaEntity.PostalCode}, {"label"})
        tableProperty.Rows.AddRaw(Nothing, Nothing, {50, 50}, {"Provincia", ipaEntity.Provincia}, {"label"})
        tableProperty.Rows.AddRaw(Nothing, Nothing, {50, 50}, {"Regione", ipaEntity.Regione}, {"label"})
        tableProperty.Rows.AddRaw(Nothing, Nothing, {50, 50}, {"Telefono", ipaEntity.TelephoneNumber}, {"label"})
        tableProperty.Rows.AddRaw(Nothing, Nothing, {50, 50}, {"eMail", ipaEntity.Mail}, {"label"})
        tableProperty.Rows.AddRaw(Nothing, Nothing, {50, 50}, {"Sito web", ipaEntity.SitoIstituzionale}, {"label"})
        tableProperty.Rows.AddRaw(Nothing, Nothing, {50, 50}, {"Responsabile", (ipaEntity.TitoloResponsabile & " " & ipaEntity.CognomeResponsabile & " " & ipaEntity.NomeResponsabile).Trim()}, {"label"})
        tableProperty.Rows.AddRaw(Nothing, Nothing, {50, 50}, {"Aoo", ipaEntity.Aoo}, {"label"})
        tableProperty.Rows.AddRaw(Nothing, Nothing, {50, 50}, {"Codice amministrazione", ipaEntity.CodiceAmministrazione}, {"label"})
        tableProperty.Rows.AddRaw(Nothing, Nothing, {50, 50}, {"Tipo amministrazione", ipaEntity.TipoAmministazione}, {"label"})
        tableProperty.Rows.AddRaw(Nothing, Nothing, {50, 50}, {"Stato amministrazione", ipaEntity.StatoAmministrazione}, {"label"})

        If CommonUtil.HasGroupAdministratorRight() Then
            tableProperty.Rows.AddRaw(Nothing, {2}, Nothing, {"Avanzate"}, {"head"})
            ' Parametri per amministratori
            tableProperty.Rows.AddRaw(Nothing, Nothing, {50, 50}, {"Path", ipaEntity.ADSPath}, {"label"})
            tableProperty.Rows.AddRaw(Nothing, Nothing, {50, 50}, {"ObjectClass", ipaEntity.ObjectClass}, {"label"})
            If Not String.IsNullOrEmpty(ipaEntity.LogoAmministrazione) Then
                Dim text As New LiteralControl("Logo")
                Dim image As New Image() With {.ImageUrl = "http://www.indiceipaEntity.gov.it/logodir/" & ipaEntity.LogoAmministrazione}
                tableProperty.Rows.AddRaw(Nothing, Nothing, Nothing, {50, 50}, {text, image}, {"label"})
            End If
            tableProperty.Rows.AddRaw(Nothing, Nothing, {50, 50}, {"L", ipaEntity.L}, {"label"})
        End If

    End Sub

#End Region

End Class

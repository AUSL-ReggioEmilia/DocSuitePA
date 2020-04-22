Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Helpers.ExtensionMethods
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data

Partial Class UtltConfig
    Inherits UtltBasePage

#Region " Fields "

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        InitializeAjax()
        If Not Page.IsPostBack Then
            If DocSuiteContext.Current.IsDocumentEnabled Then
                cmdDocument.Visible = True
                cmdDocument.CommandArgument = DSWEnvironment.Document.ToString()
            End If
            If DocSuiteContext.Current.IsProtocolEnabled Then
                cmdProtocol.Visible = True
                cmdProtocol.CommandArgument = DSWEnvironment.Protocol.ToString()
            End If
            If DocSuiteContext.Current.IsProtocolEnabled AndAlso ProtocolEnv.DocumentSeriesEnabled Then
                cmdDocumentSeries.Visible = True
                cmdDocumentSeries.Text = ProtocolEnv.DocumentSeriesName
                cmdDocumentSeries.CommandArgument = DSWEnvironment.DocumentSeries.ToString()
            End If
            If DocSuiteContext.Current.IsResolutionEnabled Then
                cmdResolution.Visible = True
                cmdResolution.Text = Facade.TabMasterFacade.TreeViewCaption
                cmdResolution.CommandArgument = DSWEnvironment.Resolution.ToString()
            End If
            CreateConfigurationSection()
        End If
    End Sub

    Private Sub locations_Click(sender As Object, e As EventArgs) Handles cmdDocument.Click, cmdProtocol.Click, cmdDocumentSeries.Click, cmdResolution.Click
        CreateRightsSection(DirectCast(sender, RadButton).CommandArgument)
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjax()
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdDocument, tblDiritti)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdProtocol, tblDiritti)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdDocumentSeries, tblDiritti)
        AjaxManager.AjaxSettings.AddAjaxSetting(cmdResolution, tblDiritti)
    End Sub

    Private Sub CreateConfigurationSection()
        tblConfig.Rows.AddRaw(Nothing, {2}, Nothing, {"Application Server"}, {"head"})
        tblConfig.Rows.AddRaw(Nothing, Nothing, {20}, {"Nome", CommonInstance.AppServerName}, {"label"})

        tblConfig.Rows.AddRaw(Nothing, {2}, Nothing, {"Client"}, {"head"})
        tblConfig.Rows.AddRaw(Nothing, Nothing, {20}, {"Nome", CommonInstance.UserComputer}, {"label"})
        tblConfig.Rows.AddRaw(Nothing, Nothing, {20}, {"Tipo", Request.UserAgent}, {"label"})

        tblConfig.Rows.AddRaw(Nothing, {2}, Nothing, {"Utente"}, {"head"})
        tblConfig.Rows.AddRaw(Nothing, Nothing, {20}, {"FullName", DocSuiteContext.Current.User.UserName}, {"label"})
        tblConfig.Rows.AddRaw(Nothing, Nothing, {20}, {"Dominio", CommonUtil.UserDomain}, {"label"})
        tblConfig.Rows.AddRaw(Nothing, Nothing, {20}, {"Nome", DocSuiteContext.Current.User.UserName}, {"label"})
        tblConfig.Rows.AddRaw(Nothing, Nothing, {20}, {"Sessione", CommonUtil.UserSessionId}, {"label"})

        Dim output As String = String.Empty
        For Each sg As SecurityGroups In Facade.SecurityUsersFacade.GetGroupsByAccount(DocSuiteContext.Current.User.UserName)
            output &= String.Format("{0} - {1}, ", sg.Id, sg.GroupName)
        Next
        tblConfig.Rows.AddRaw(Nothing, Nothing, {20}, {"SecurityGroups", output}, {"label"})

        tblConfig.Rows.AddRaw(Nothing, {2}, Nothing, {"Configurazione Tool Client"}, {"head"})
        tblConfig.Rows.AddRaw(Nothing, Nothing, {20}, {"Visualizzatore", CommonShared.AdvancedViewer.ToString()}, {"label"})

        tblConfig.Rows.AddRaw(Nothing, {2}, Nothing, {"Autorizzazione"}, {"head"})
        tblConfig.Rows.AddRaw(Nothing, Nothing, {20}, {"Identity", Page.User.Identity.Name}, {"label"})
        tblConfig.Rows.AddRaw(Nothing, Nothing, {20}, {"AuthenticationType", Page.User.Identity.AuthenticationType}, {"label"})
        tblConfig.Rows.AddRaw(Nothing, Nothing, Nothing, {20}, {New LiteralControl("AuthenticationType"), New Image() With {.ImageUrl = If(Page.User.Identity.IsAuthenticated, ImagePath.SmallAccept, ImagePath.SmallCross)}}, {"label"})

        tblConfig.Rows.AddRaw(Nothing, {2}, Nothing, {"Abilitazione Gruppi"}, {"head"})
        tblConfig.Rows.AddRaw(Nothing, Nothing, Nothing, {20}, {New LiteralControl("Amministratore"), New Image() With {.ImageUrl = If(CommonShared.HasGroupAdministratorRight, ImagePath.SmallAccept, ImagePath.SmallCross)}}, {"label"})
        tblConfig.Rows.AddRaw(Nothing, Nothing, Nothing, {20}, {New LiteralControl("Statistiche"), New Image() With {.ImageUrl = If(CommonShared.HasGroupStatisticsRight, ImagePath.SmallAccept, ImagePath.SmallCross)}}, {"label"})
        tblConfig.Rows.AddRaw(Nothing, Nothing, Nothing, {20}, {New LiteralControl("Sospensione"), New Image() With {.ImageUrl = If(CommonShared.HasGroupSuspendRight, ImagePath.SmallAccept, ImagePath.SmallCross)}}, {"label"})
        tblConfig.Rows.AddRaw(Nothing, Nothing, Nothing, {20}, {New LiteralControl("Contatti"), New Image() With {.ImageUrl = If(CommonShared.HasGroupTblContactRight, ImagePath.SmallAccept, ImagePath.SmallCross)}}, {"label"})
        tblConfig.Rows.AddRaw(Nothing, Nothing, Nothing, {20}, {New LiteralControl("Classificatore"), New Image() With {.ImageUrl = If(CommonShared.HasGroupTblCategoryRight, ImagePath.SmallAccept, ImagePath.SmallCross)}}, {"label"})
        tblConfig.Rows.AddRaw(Nothing, Nothing, Nothing, {20}, {New LiteralControl("Log"), New Image() With {.ImageUrl = If(CommonShared.HasGroupLogViewRight, ImagePath.SmallAccept, ImagePath.SmallCross)}}, {"label"})
        tblConfig.Rows.AddRaw(Nothing, Nothing, Nothing, {20}, {New LiteralControl("Concorsi"), New Image() With {.ImageUrl = If(CommonShared.HasGroupConcourseRight, ImagePath.SmallAccept, ImagePath.SmallCross)}}, {"label"})

        tblConfig.Rows.AddRaw(Nothing, {2}, Nothing, {"Ambiente"}, {"head"})
        tblConfig.Rows.AddRaw(Nothing, Nothing, {20}, {"Server", CommonShared.MachineName}, {"label"})
        tblConfig.Rows.AddRaw(Nothing, Nothing, {20}, {"Home", CommonInstance.HomeDirectory}, {"label"})
        tblConfig.Rows.AddRaw(Nothing, Nothing, {20}, {"Application Http", DocSuiteContext.Current.CurrentTenant.DSWUrl}, {"label"})
        tblConfig.Rows.AddRaw(Nothing, Nothing, {20}, {"Application Path", CommonInstance.AppPath}, {"label"})
        tblConfig.Rows.AddRaw(Nothing, Nothing, {20}, {"Temp Path", CommonInstance.AppTempPath}, {"label"})
        tblConfig.Rows.AddRaw(Nothing, Nothing, {20}, {"Temp Directory", Path.GetTempPath & "tmp*.tmp"}, {"label"})


        tblConfig.Rows.AddRaw(Nothing, {2}, Nothing, {"Dominio"}, {"head"})
        tblConfig.Rows.AddRaw(Nothing, Nothing, {20}, {"Dominio Path AD", DocSuiteContext.DomainPath}, {"label"})
        tblConfig.Rows.AddRaw(Nothing, Nothing, {20}, {"Utente Accesso Dominio", DocSuiteContext.Current.CurrentTenant.DomainUser}, {"label"})

        tblConfig.Rows.AddRaw(Nothing, {2}, Nothing, {"Dominio nei Moduli"}, {"head"})

        If DocSuiteContext.Current.IsDocumentEnabled Then
            tblConfig.Rows.AddRaw(Nothing, Nothing, {20}, {"Pratiche", DocSuiteContext.Current.DocumentEnv.Domain}, {"label"})
        End If
        If DocSuiteContext.Current.IsProtocolEnabled Then
            tblConfig.Rows.AddRaw(Nothing, Nothing, {20}, {"Protocollo", DocSuiteContext.Current.CurrentDomainName}, {"label"})
        End If
        If DocSuiteContext.Current.IsResolutionEnabled Then
            tblConfig.Rows.AddRaw(Nothing, Nothing, {20}, {Facade.TabMasterFacade.TreeViewCaption, DocSuiteContext.Current.ResolutionEnv.ResolutionDomain}, {"label"})
        End If
    End Sub

    Private Sub CreateRightsSection(environmentValue As String)
        Dim environment As DSWEnvironment? = Nothing
        If Not String.IsNullOrEmpty(environmentValue) Then
            environment = DirectCast([Enum].Parse(GetType(DSWEnvironment), environmentValue, True), DSWEnvironment)
        End If

        If Not environment.HasValue Then
            Exit Sub
        End If

        Dim enumType As Type = GetType(ProtocolContainerRightPositions)
        Select Case environment
            Case DSWEnvironment.Document
                enumType = GetType(DocumentContainerRightPositions)

            Case DSWEnvironment.Protocol
                enumType = GetType(ProtocolContainerRightPositions)

            Case DSWEnvironment.Resolution
                enumType = GetType(ResolutionRightPositions)

            Case DSWEnvironment.DocumentSeries
                enumType = GetType(DocumentSeriesContainerRightPositions)

        End Select

        tblDiritti.Rows.AddRaw("", Nothing, {20, 80}, {"Diritto", "Contenitore"}, {"head", "head"})
        For Each value As [Enum] In [Enum].GetValues(enumType)
            Dim containers As IList(Of Container) = Facade.ContainerFacade.GetContainers(DocSuiteContext.Current.CurrentDomainName, DocSuiteContext.Current.User.UserName, environment.Value, Convert.ToInt32(value), True)
            Dim list As String = "Nessun contenitore"
            If Not containers.IsNullOrEmpty() Then
                list = String.Join(", ", containers.Select(Function(c) c.Name))
            End If
            tblDiritti.Rows.AddRaw("", Nothing, {20, 80}, {value.GetDescription(), list}, {"label"})
        Next
    End Sub

#End Region

End Class


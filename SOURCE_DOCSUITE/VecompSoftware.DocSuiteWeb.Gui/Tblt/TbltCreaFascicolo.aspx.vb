Imports System.Collections.Generic
Imports System.Linq
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Commons
Imports VecompSoftware.DocSuiteWeb.Data.Entity.Fascicles
Imports VecompSoftware.DocSuiteWeb.Data.Entity.UDS
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Commons
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.Fascicles
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Commons
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.Fascicles
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.UDS
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods
Imports VecompSoftware.Services.Logging
Imports WebAPIUDS = VecompSoftware.DocSuiteWeb.Entity.UDS
Imports VecompSoftware.DocSuiteWeb.Data.WebAPI.Finder.UDS
Imports VecompSoftware.DocSuiteWeb.DTO.WebAPI

Public Class TbltCreaFascicolo
    Inherits CommonBasePage

#Region " Fields "
    Private _udsRepositoryFacade As UDSRepositoryFacade
    Private _fasciclePeriodFacade As FasciclePeriodFacade
    Private _category As Entity.Commons.Category
    Private Const TITLE_LABEL As String = "Piano di fascicolazione - {0}"

    Private Const TBLT_INSERT_CALLBACK As String = "tbltCreaFascicolo.insertCallback('{0}');"
#End Region

#Region " Properties "

    Public ReadOnly Property IdCategory As String
        Get
            Return Request.QueryString("IdCategory").GetValueOrEmpty()
        End Get
    End Property

    Public ReadOnly Property Environment As String
        Get
            Return Request.QueryString("Environment").GetValueOrEmpty()
        End Get
    End Property

    Private ReadOnly Property CurrentFasciclePeriodFacade As FasciclePeriodFacade
        Get
            If _fasciclePeriodFacade Is Nothing Then
                _fasciclePeriodFacade = New FasciclePeriodFacade()
            End If
            Return _fasciclePeriodFacade
        End Get
    End Property


#End Region

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        InitializeAjaxSettings()
        If Not IsPostBack Then
            InitializePeriods()
        End If
    End Sub

#End Region

#Region " Methods "

    Private Sub InitializeAjaxSettings()
        AjaxManager.AjaxSettings.AddAjaxSetting(btnSave, btnSave, MasterDocSuite.AjaxFlatLoadingPanel)

    End Sub

    Private Sub InitializePeriods()
        Dim periods As IList(Of FasciclePeriod) = CurrentFasciclePeriodFacade.GetAll()
        ddlPeriods.Items.AddRange(periods.Select(Function(x) New RadComboBoxItem(x.PeriodName, x.Id.ToString())))
        Dim udsRepositories As IList(Of UDSRepository) = New UDSRepositoryFacade(DocSuiteContext.Current.User.FullUserName).GetActiveRepositories(String.Empty)
        ddlUDS.Items.Clear()
        ddlUDS.Items.Add(New RadComboBoxItem(String.Empty, String.Empty))
        ddlUDS.Items.Add(New RadComboBoxItem("Protocollo", Convert.ToInt32(DSWEnvironment.Protocol).ToString()))
        ddlUDS.Items.Add(New RadComboBoxItem(FacadeFactory.Instance.TabMasterFacade.TreeViewCaption, Convert.ToInt32(DSWEnvironment.Resolution).ToString()))
        ddlUDS.Items.Add(New RadComboBoxItem(DocSuiteContext.Current.ProtocolEnv.DocumentSeriesName, Convert.ToInt32(DSWEnvironment.DocumentSeries).ToString()))
        For Each repository As UDSRepository In udsRepositories
            ddlUDS.Items.Add(New RadComboBoxItem(repository.Name, repository.DSWEnvironment.ToString()))
        Next
    End Sub

#End Region

End Class
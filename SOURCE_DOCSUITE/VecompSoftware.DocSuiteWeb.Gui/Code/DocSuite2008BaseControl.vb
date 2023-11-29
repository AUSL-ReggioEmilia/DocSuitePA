Imports System.Collections.Generic
Imports Newtonsoft.Json
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Entity.Commons
Imports VecompSoftware.DocSuiteWeb.Entity.Tenants
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Facade.Common.Commons
Imports VecompSoftware.DocSuiteWeb.Facade.NHibernate.UDS

''' <summary> Controllo base della docsuiteweb. </summary>
''' <remarks> Deve essere ospitato da una <see cref="CommonBasePage"/>. </remarks>
Public Class DocSuite2008BaseControl
    Inherits BaseControl

#Region " Field "
    Private _udsRepositoryFacade As UDSRepositoryFacade
    Private _windowBuilder As WindowBuilder = Nothing
    Private _fileExtensionBlackList As List(Of String)
    Protected _type As String
    Protected _facade As FacadeFactory
    Protected Const SCANNER_LIGHT_PATH As String = "../UserControl/ScannerLight.aspx"
    Protected Const SCANNER_LIGHT_PATH_REST As String = "../UserControl/ScannerRest.aspx"
#End Region

#Region " Properties "

    ''' <summary> Id del controllo chiamante. </summary>
    ''' <remarks> Serve a chiamare il javascript corretto nella pagina del controllo chiamante. </remarks>
    Protected ReadOnly Property CallerId() As String
        Get
            Return Request.QueryString("ParentID")
        End Get
    End Property

    <Obsolete("Togliere ad ogni costo. Se la usate vi taglio le mani")>
    Protected ReadOnly Property WindowBuilder() As WindowBuilder
        Get
            If _windowBuilder Is Nothing Then
                _windowBuilder = New WindowBuilder(AjaxManager)
            End If
            Return _windowBuilder
        End Get
    End Property

    ''' <summary> Restituisce una istanza della common util. </summary>
    Public ReadOnly Property CommonInstance() As CommonUtil
        Get
            Return CommonUtil.GetInstance()
        End Get
    End Property

    ''' <summary> LoggerName del Appender sul quale inserire i messaggi di log. </summary>
    Public Shared ReadOnly Property LoggerName As String
        Get
            Return CommonBasePage.LoggerName
        End Get
    End Property

    ''' <summary> Tipo che identifica l'area del controllo. </summary>
    ''' <returns> Di default è il tipo della pagina che contiene il controllo. </returns>
    ''' <remarks> Modifica stile della pagina e connessione al DB della <see cref="DocSuite2008BaseControl.Facade"/>. </remarks>
    Public Property Type As String
        Get
            If String.IsNullOrEmpty(_type) Then
                _type = BasePage.Type
            End If
            Return _type
        End Get
        Set(value As String)
            _type = value
        End Set
    End Property

    ''' <summary> Nome della sessione NHibernate predefinita per la pagina. </summary>
    Protected ReadOnly Property DbName As String
        Get
            Select Case Type
                Case DocmBasePage.DefaultType
                    Return "DocmDB"
                Case ReslBasePage.DefaultType
                    Return "ReslDB"
                Case Else
                    Return "ProtDB"
            End Select
        End Get
    End Property

    ''' <summary> Istanza predefinita per la pagina della <see cref="FacadeFactory"/>. </summary>
    ''' <remarks> A meno che non venga indicato diversamente, il controllo usa la connessione predefinita della pagina. </remarks>
    Public ReadOnly Property Facade(Optional ByVal databaseName As String = Nothing) As FacadeFactory
        Get
            If _facade Is Nothing Then
                _facade = New FacadeFactory(If(databaseName Is Nothing, DbName, databaseName))
            End If
            Return _facade
        End Get
    End Property
    Public ReadOnly Property CurrentUDSRepositoryFacade As UDSRepositoryFacade
        Get
            If _udsRepositoryFacade Is Nothing Then
                _udsRepositoryFacade = New UDSRepositoryFacade(DocSuiteContext.Current.User.FullUserName)
            End If
            Return _udsRepositoryFacade
        End Get
    End Property

    Protected ReadOnly Property ProtocolEnv() As ProtocolEnv
        Get
            Return DocSuiteContext.Current.ProtocolEnv
        End Get
    End Property

    Protected ReadOnly Property DocumentEnv() As DocumentEnv
        Get
            Return DocSuiteContext.Current.DocumentEnv
        End Get
    End Property

    Protected ReadOnly Property ResolutionEnv() As ResolutionEnv
        Get
            Return DocSuiteContext.Current.ResolutionEnv
        End Get
    End Property

    Protected ReadOnly Property CurrentUser As String
        Get
            Return JsonConvert.SerializeObject(DocSuiteContext.Current.User.FullUserName)
        End Get
    End Property


    ''' <summary> Se true salva in una variabile in sessione gli elementi dello usercontrol.
    ''' Per ora è implementato solo per gli user control di settore e classificatore</summary>
    Public Property UseSessionStorage As Boolean

    Protected ReadOnly Property CurrentPrivacyLevels() As ICollection(Of PrivacyLevel)
        Get
            If Not DocSuiteContext.Current.HasPrivacyLevels Then
                Dim privacyLevelFacade As PrivacyLevelFacade = New PrivacyLevelFacade()
                DocSuiteContext.Current.RefreshPrivacyLevel(privacyLevelFacade.GetCurrentPrivacyLevels())
            End If
            Return DocSuiteContext.Current.CurrentPrivacyLevels
        End Get
    End Property

    Protected ReadOnly Property FileExtensionBlackList As List(Of String)
        Get
            If _fileExtensionBlackList Is Nothing AndAlso Not String.IsNullOrEmpty(DocSuiteContext.Current.ProtocolEnv.FileExtensionBlackList) Then

                Dim splitted As String() = DocSuiteContext.Current.ProtocolEnv.FileExtensionBlackList.ToLowerInvariant().Split("|"c)
                _fileExtensionBlackList = New List(Of String)(splitted)
            End If
            Return _fileExtensionBlackList
        End Get
    End Property
    Public Property CurrentTenant As Tenant
        Get
            If Session(CommonShared.USER_CURRENT_TENANT) IsNot Nothing Then
                Return DirectCast(Session(CommonShared.USER_CURRENT_TENANT), Tenant)
            End If
            Return Nothing
        End Get
        Set(value As Tenant)
            Session(CommonShared.USER_CURRENT_TENANT) = value
        End Set
    End Property

    Public Property CurrentDomainUser As Model.Securities.DomainUserModel
        Get
            If Session("CurrentDomainUser") IsNot Nothing Then
                Return DirectCast(Session("CurrentDomainUser"), Model.Securities.DomainUserModel)
            End If
            Return Nothing
        End Get
        Set(value As Model.Securities.DomainUserModel)
            Session("CurrentDomainUser") = value
        End Set
    End Property
#End Region

#Region " Methods "

    Protected Function GetPropertyValue(Of V)(ByVal propertyName As String, ByVal nullValue As V) As V
        If (ViewState(propertyName) Is Nothing) Then
            Return Nothing
        End If
        Return CType(ViewState(propertyName), V)
    End Function

    Protected Sub SetPropertyValue(Of V)(ByVal propertyName As String, ByVal value As V)
        ViewState(propertyName) = value
    End Sub

#End Region

End Class


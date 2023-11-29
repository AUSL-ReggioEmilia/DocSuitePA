Imports System.Collections.Generic
Imports Newtonsoft.Json
Imports Telerik.Web.UI
Imports VecompSoftware.DocSuiteWeb.Data

Public Class uscRoleUserSelRest
    Inherits DocSuite2008BaseControl

#Region " Fields "
    Private _roleUserTypeLabels As String
    Private _environments As String
#End Region

#Region "Properties"
    Public ReadOnly Property PageContent As RadDropDownTree
        Get
            Return rddtRoleUser
        End Get
    End Property

    Public ReadOnly Property FascicleEnabled As Boolean
        Get
            Return DocSuiteContext.Current.ProtocolEnv.FascicleEnabled
        End Get
    End Property

    Public ReadOnly Property ProtocolEnabled As Boolean
        Get
            Return DocSuiteContext.Current.PrivacyEnabled
        End Get
    End Property

    Public ReadOnly Property CollaborationEnabled As Boolean
        Get
            Return DocSuiteContext.Current.ProtocolEnv.IsCollaborationEnabled AndAlso DocSuiteContext.Current.ProtocolEnv.IsCollaborationGroupEnabled
        End Get
    End Property

    Public ReadOnly Property CollaborationRightsEnabled As Boolean
        Get
            Return DocSuiteContext.Current.ProtocolEnv.CollaborationRightsEnabled
        End Get
    End Property

    Public ReadOnly Property RoleUserTypeLabels As String
        Get
            If _roleUserTypeLabels Is Nothing Then
                Dim labels As IDictionary(Of String, String) = New Dictionary(Of String, String)
                labels.Add(RoleUserType.RP.ToString(), DocSuiteContext.Current.ProtocolEnv.FascicleRoleRPLabel)
                labels.Add(RoleUserType.SP.ToString(), DocSuiteContext.Current.ProtocolEnv.FascicleRoleSPLabel)
                labels.Add(RoleUserType.MP.ToString(), CommonBasePage.PRIVACY_LABEL)
                labels.Add(RoleUserType.D.ToString(), DocSuiteContext.Current.ProtocolEnv.NomeDirigentiCollaborazione)
                labels.Add(RoleUserType.V.ToString(), DocSuiteContext.Current.ProtocolEnv.NomeViceCollaborazione)
                labels.Add(RoleUserType.S.ToString(), DocSuiteContext.Current.ProtocolEnv.NomeSegreteria)
                _roleUserTypeLabels = JsonConvert.SerializeObject(labels)
            End If
            Return _roleUserTypeLabels
        End Get
    End Property

    Public ReadOnly Property Environments As String
        Get
            If _environments Is Nothing Then
                Dim envList As New List(Of DSWEnvironment)
                If DocSuiteContext.Current.IsProtocolEnabled Then
                    envList.Add(DSWEnvironment.Protocol)
                End If
                If DocSuiteContext.Current.IsResolutionEnabled Then
                    envList.Add(DSWEnvironment.Resolution)
                End If
                If DocSuiteContext.Current.ProtocolEnv.DocumentSeriesEnabled Then
                    envList.Add(DSWEnvironment.DocumentSeries)
                End If
                Dim a As String = DSWEnvironment.DocumentSeries.ToString()
                _environments = JsonConvert.SerializeObject(envList)
            End If
            Return _environments
        End Get
    End Property
#End Region

#Region "Events"
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub
#End Region

#Region " Methods "
#End Region

End Class
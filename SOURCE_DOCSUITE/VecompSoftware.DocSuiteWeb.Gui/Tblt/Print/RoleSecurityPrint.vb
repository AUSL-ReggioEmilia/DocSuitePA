Imports System.Collections.Generic
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.Data

''' <summary> Stampa dei settori con visualizzazione sicurezza </summary>
Public Class RoleSecurityPrint
    Inherits SecurityPrint

#Region "Fields"
    Private _roles As IList(Of Integer)
#End Region

#Region "Properties"
    Public Property RolesID() As IList(Of Integer)
        Get
            If _roles Is Nothing Then
                _roles = New List(Of Integer)
            End If
            Return _roles
        End Get
        Set(ByVal value As IList(Of Integer))
            _roles = value
        End Set
    End Property
#End Region
    Public Sub New()
        TraslitteraDiritti = New TraslitteraDelegate(AddressOf RoleGroupFacade.TraslitteraDiritti)
    End Sub

    Public Overrides Sub DoPrint()
        'Setto il titolo della stampa
        TitlePrint = "Stampa dei Settori con Sicurezza"
        StampaRuoliSecurity()
    End Sub

#Region "Funzioni di Stampa"
    Private Sub StampaRuoliSecurity()
        Dim lblResl As String = String.Empty

        If DocSuiteContext.Current.IsResolutionEnabled Then
            lblResl = Facade.TabMasterFacade.TreeViewCaption
        End If

        Dim roles As IList(Of Role) = New List(Of Role)()

        For Each id As Integer In RolesID
            roles.Add(Facade.RoleFacade.GetById(id))
        Next

        If roles.Count > 0 Then
            For Each role As Role In roles
                PrintRole(role, lblResl)
            Next
        End If
    End Sub

    Protected Sub PrintRole(ByVal role As Role, ByVal lblResl As String)
        'ruolo
        PrintRoleDetail(role, lblResl)
        '--Sottoruolo
        Dim roles As IList(Of Role) = Facade.RoleFacade.GetItemsByParentId(role.Id)
        If roles.Count > 0 Then
            For Each childrenRole As Role In roles
                PrintRole(childrenRole, lblResl)
            Next
        End If
    End Sub

    Protected Sub PrintRoleDetail(ByVal role As Role, ByVal lblResl As String)
        'Aggiungi Spazio
        CreateSpaceRow(TablePrint)
        'Crea titolo principale Settore
        CreateSecurityRow(TablePrint, "Settore: " & String.Empty.PadLeft(role.Level, "."c) & " " & role.Name)
        'Aggiungi Spazio
        CreateSpaceRow(TablePrint)
        'Gruppi
        For Each group As RoleGroup In role.RoleGroups
            CreateGroupsSection(TablePrint, group)
        Next
    End Sub
#End Region

End Class

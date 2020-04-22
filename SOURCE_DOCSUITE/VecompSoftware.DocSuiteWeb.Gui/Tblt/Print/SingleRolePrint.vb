Imports VecompSoftware.DocSuiteWeb.Data

Public Class SingleRolePrint
    Inherits RoleSecurityPrint

#Region "Fields"
    Private _role As Role = Nothing
#End Region

#Region "Properties"
    Public Property Role() As Role
        Get
            Return _role
        End Get
        Set(ByVal value As Role)
            _role = value
        End Set
    End Property
#End Region

#Region "Constructor"
    Public Sub New(ByVal idRole As Integer)
        MyBase.New()
        _role = Facade.RoleFacade.GetById(idRole)
    End Sub
#End Region

#Region "DoPrint"
    Public Overrides Sub DoPrint()
        'Setto il titolo della stampa
        TitlePrint = "Stampa Settore con Sicurezza"

        Dim reslCaption As String = String.Empty

        If DocSuiteContext.Current.IsResolutionEnabled Then
            'TODO: atti non implementati
        End If
        MyBase.PrintRole(Role, reslCaption)
    End Sub
#End Region
    
End Class

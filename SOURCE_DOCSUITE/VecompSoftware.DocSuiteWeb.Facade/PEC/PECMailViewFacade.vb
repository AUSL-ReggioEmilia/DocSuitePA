Imports System.ComponentModel
Imports System.Linq
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods

<DataObject()> _
Public Class PECMailViewFacade
    Inherits BaseProtocolFacade(Of PECMailView, Integer, NHibernatePECMailViewDao)

#Region " Fields "
#End Region

#Region " Constructor "
    Public Sub New()
        MyBase.New()
    End Sub
#End Region

#Region " Methods "

    ''' <summary> Ritorna tutte le viste sulle quali l'utente ha autorizzazione di utilizzo. </summary>
    Public Function GetByRight() As IList(Of PECMailView)
        Return CheckIfAuthorizationOnPecMailView(GetAll())
    End Function

    ''' <summary> Ritorna tutte le viste sulle quali l'utente ha autorizzazione di utilizzo e che non hanno vincoli</summary>
    Public Function GetByRightAndPageType() As IList(Of PECMailView)
        Return GetByRightAndPageType(String.Empty)
    End Function

    ''' <summary> Ritorna tutte le viste sulle quali l'utente ha autorizzazione di utilizzo e che non hanno vincoli oppure il tipo di pagina corrisponde</summary>
    Public Function GetByRightAndPageType(ByVal allowedPageType As String) As IList(Of PECMailView)
        Return CheckIfAuthorizationOnPecMailView(_dao.GetByAllowedPageType(allowedPageType))
    End Function

    ''' <summary> Ritorna la vista di default tra quelle passate </summary>
    Public Function GetDefault(ByRef pecMailViews As IList(Of PECMailView)) As PECMailView
        Dim pecMailView As PECMailView = Nothing

        '' Se ho 1 elemento solo è certamente quello di default
        If pecMailViews.Count = 1 Then
            Return pecMailViews(0)
        End If

        '' Alternativamente cerco quello di default
        For Each pmView As PECMailView In From pmView1 In pecMailViews Where pmView1.MailViewDefaults.Count > 0
            Return pmView.MailViewDefaults(0).DefaultView
        Next

        '' Se non ho 1 default e la lista ha almeno 1 elemento, ritorno il primo
        If pecMailView Is Nothing AndAlso pecMailViews.Count > 0 Then
            Return pecMailViews(0)
        End If

        '' Se sono qui ho ricevuto necessariamente un nothing
        Return pecMailView
    End Function

    ''' <summary> Ritorna la vista di default per l'utente corrente </summary>
    Public Function GetDefault() As PECMailView
        Return GetDefault(GetByRightAndPageType())
    End Function

    Private Function IsCurrentUserEnabled(view As PECMailView) As Boolean
        Dim roleGroupPECRightEnabled As Boolean = DocSuiteContext.Current.ProtocolEnv.RoleGroupPECRightEnabled
        'se le abilitazioni (pecmailview role) non sono definite, di default non ho restrizioni
        Return view.Roles.IsNullOrEmpty() OrElse view.Roles.SelectMany(Function(r) r.RoleGroups) _
            .Any(Function(g) g.ProtocolRights.IsRoleEnabled AndAlso (Not roleGroupPECRightEnabled OrElse (roleGroupPECRightEnabled AndAlso g.ProtocolRights.IsRolePEC)) AndAlso FacadeFactory.Instance.RoleFacade.CurrentUserBelongsToRoles(DSWEnvironment.Protocol, g.Role))
    End Function

    Private Function CheckIfAuthorizationOnPecMailView(ByVal mailViews As IList(Of PECMailView)) As IList(Of PECMailView)
        If CommonShared.HasGroupAdministratorRight Then
            Return mailViews
        End If

        Return mailViews.Where(Function(v) IsCurrentUserEnabled(v)).ToList()
    End Function

#End Region

End Class


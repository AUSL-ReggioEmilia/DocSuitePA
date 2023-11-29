Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade

Partial Public Class TbltSettorePropagation
    Inherits CommonBasePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not (CommonShared.HasGroupAdministratorRight OrElse CommonShared.HasGroupTblRoleAdminRight) Then
            Throw New DocSuiteException("Sono necessari diritti amministrativi per vedere la pagina.")
        End If


        Dim count_d As Integer = 0
        Dim count_v As Integer = 0
        Dim count_s As Integer = 0

        ' Recupero i direttori
        For Each r As Role In Facade.RoleFacade.GetAll()
            For Each ru As RoleUser In Facade.RoleUserFacade.GetByRoleId(r.Id)
                ' Verifico la tipologi
                Select Case ru.Type
                    Case RoleUserType.D.ToString()
                        ' Direttore da propagare in Vice e Segreteria
                        count_d = count_d + 1
                        ' Verifico se esiste nei Vice
                        If Facade.RoleUserFacade.GetByRoleIdAndAccount(r.Id, ru.Account, RoleUserType.V).Count = 0 Then
                            ' Non esiste, aggiungo
                            Dim usr As New RoleUser()
                            usr.Role = r
                            usr.Type = RoleUserType.V.ToString()
                            usr.Description = ru.Description
                            usr.Account = ru.Account
                            usr.Enabled = True
                            usr.Email = ""
                            usr.DSWEnvironment = Env
                            Facade.RoleUserFacade.Save(usr)
                        End If
                        ' Verifico se esiste nei Segretari
                        If Facade.RoleUserFacade.GetByRoleIdAndAccount(r.Id, ru.Account, RoleUserType.S).Count = 0 Then
                            ' Non esiste, aggiungo
                            Dim usr As New RoleUser()
                            usr.Role = r
                            usr.Type = RoleUserType.S.ToString()
                            usr.Description = ru.Description
                            usr.Account = ru.Account
                            usr.Enabled = True
                            usr.Email = ""
                            usr.DSWEnvironment = Env
                            Facade.RoleUserFacade.Save(usr)
                        End If
                    Case RoleUserType.V.ToString()
                        ' Vice da propagare in Segreteria
                        count_v = count_v + 1
                        ' Verifico se esiste nei Segretari
                        If Facade.RoleUserFacade.GetByRoleIdAndAccount(r.Id, ru.Account, RoleUserType.S).Count = 0 Then
                            ' Non esiste, aggiungo
                            Dim usr As New RoleUser()
                            usr.Role = r
                            usr.Type = RoleUserType.S.ToString()
                            usr.Description = ru.Description
                            usr.Account = ru.Account
                            usr.Enabled = True
                            usr.Email = ""
                            usr.DSWEnvironment = Env
                            Facade.RoleUserFacade.Save(usr)
                        End If
                    Case RoleUserType.S.ToString()
                        ' Segreteria
                        count_s = count_s + 1
                End Select
            Next
        Next

        Message.Text = "Propagazione gerarchica collaborazione conclusa con successo."

    End Sub

End Class
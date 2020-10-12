Imports System.Linq
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.DocSuiteWeb.Data

<ComponentModel.DataObject()>
Public Class CollaborationStatusRecipientFacade
    Inherits BaseProtocolFacade(Of CollaborationStatusRecipient, String, NHibernateCollaborationStatusRecipientDao)

#Region " Enumerators "
    Public Enum Code
        DirectWorkers
        AllSecretaries
        CheckedSecretaries
        MainSecretaries
        CheckedMainSecretaries
        CollaborationProposer
        AllDirectors
        AllViceDirector
        ActiveSigner
        AllSigners
        AllPrecedentSigners
        AllFollowingSigners
        CurrentUser
        FirstSigner
        ActiveSignerAntecedent
        CurrentSignerRole
        AllSignersRole
        SecretariesRole
    End Enum
#End Region

#Region " Properties "

    Public Overrides ReadOnly Property LoggerName As String
        Get
            Return "Collaboration"
        End Get
    End Property

#End Region

#Region " Methods "
    ''' <summary>
    ''' Carica una lista di contatti a partire da un particolare codice ricevuto e una collaborazione di riferimento
    ''' </summary>
    ''' <param name="collaborationStatusRecipientCode">Codice che identifica la tipologia di contatto</param>
    ''' <param name="idCollaboration">Collaborazione di riferimento</param>
    ''' <param name="removeNullEmailContacts">Definisce se devono essere rimossi eventuali contatti che risultassero senza e-mail impostata</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function ResolveAddresses(ByVal collaborationStatusRecipientCode As String, ByVal idCollaboration As Integer, ByVal removeNullEmailContacts As Boolean) As IList(Of Contact)
        Dim contacts As New List(Of Contact)
        Select Case DirectCast([Enum].Parse(GetType(Code), collaborationStatusRecipientCode), Code)
            Case Code.DirectWorkers
                ''Contatti diretti per la protocollazione
                contacts.AddRange((From collaborationUser In Factory.CollaborationUsersFacade.GetDirectWorkers(idCollaboration) Select New Contact() With {.Description = collaborationUser.DestinationName, .EmailAddress = collaborationUser.DestinationEMail}).ToList())

            Case Code.AllSecretaries
                ''Tutti gli utenti segretari di tutti i settori segreteria
                contacts.AddRange((From roleUser In Factory.RoleUserFacade.GetByCollaboration(idCollaboration, RoleUserType.S, Nothing, Nothing) Select New Contact() With {.Description = roleUser.Description, .EmailAddress = roleUser.Email}).ToList())

            Case Code.CheckedSecretaries
                ''Tutti gli utenti segretari di tutti i settori segreteria che hanno il checkbox a 1
                contacts.AddRange((From roleUser In Factory.RoleUserFacade.GetByCollaboration(idCollaboration, RoleUserType.S, True, Nothing) Select New Contact() With {.Description = roleUser.Description, .EmailAddress = roleUser.Email}).ToList())

            Case Code.MainSecretaries
                ''Tutti gli utenti segretari di tutti i settori segreteria che hanno la proprietà Main impostata a True
                contacts.AddRange((From roleUser In Factory.RoleUserFacade.GetByCollaboration(idCollaboration, RoleUserType.S, Nothing, True) Select New Contact() With {.Description = roleUser.Description, .EmailAddress = roleUser.Email}).ToList())

            Case Code.CheckedMainSecretaries
                ''Tutti gli utenti segretari di tutti i settori segreteria che hanno la proprietà Main impostata a True e che hanno il chekcbox a 1
                contacts.AddRange((From roleUser In Factory.RoleUserFacade.GetByCollaboration(idCollaboration, RoleUserType.S, True, True) Select New Contact() With {.Description = roleUser.Description, .EmailAddress = roleUser.Email}).ToList())

            Case Code.CollaborationProposer
                ''L'utente che ha inserito la collaborazione
                Dim collaboration As Collaboration = Factory.CollaborationFacade.GetById(idCollaboration)
                contacts.Add(New Contact() With {.Description = collaboration.RegistrationName, .EmailAddress = collaboration.RegistrationEMail})
            Case Code.AllDirectors
                ''Tutti gli utenti direttori di tutti i settori segreteria
                contacts.AddRange((From roleUser In Factory.RoleUserFacade.GetByCollaboration(idCollaboration, RoleUserType.D, Nothing, Nothing) Select New Contact() With {.Description = roleUser.Description, .EmailAddress = roleUser.Email}).ToList())

            Case Code.AllViceDirector
                ''Tutti gli utenti vice direttori di tutti i settori segreteria
                contacts.AddRange((From roleUser In Factory.RoleUserFacade.GetByCollaboration(idCollaboration, RoleUserType.V, Nothing, Nothing) Select New Contact() With {.Description = roleUser.Description, .EmailAddress = roleUser.Email}).ToList())

            Case Code.ActiveSigner
                ''Il firmatario attivo
                contacts.Add((From sign In Factory.CollaborationSignsFacade.GetCollaborationSignsBy(idCollaboration, 1)).Select(Function(s) New Contact() With {.Description = s.SignName, .EmailAddress = s.SignEMail}).First())

            Case Code.AllSigners
                ''Tutti i firmatari
                contacts.AddRange((From collaborationSign In Factory.CollaborationSignsFacade.GetByIdCollaboration(idCollaboration) Select New Contact() With {.Description = collaborationSign.SignName, .EmailAddress = collaborationSign.SignEMail}).ToList())

            Case Code.AllPrecedentSigners
                ''Tutti i firmatari precedenti a quello attivo
                For Each collaborationSign As CollaborationSign In Factory.CollaborationSignsFacade.GetByIdCollaboration(idCollaboration)
                    If collaborationSign.IsActive = 0 Then
                        contacts.Add(New Contact() With {.Description = collaborationSign.SignName, .EmailAddress = collaborationSign.SignEMail})
                    Else
                        Exit For
                    End If
                Next

            Case Code.AllFollowingSigners
                ''Tutti i firmatari successivi a quello attivo
                contacts.AddRange((From collaborationSign In Factory.CollaborationSignsFacade.GetCollaborationSignsByGeActiveIncremental(idCollaboration) Select New Contact() With {.Description = collaborationSign.SignName, .EmailAddress = collaborationSign.SignEMail}).ToList())

            Case Code.CurrentUser
                '' L'utente corrente
                contacts.Add(New Contact() With {.Description = CommonUtil.GetInstance.UserDescription, .EmailAddress = CommonUtil.GetInstance.UserMail})

            Case Code.FirstSigner
                '' Il primo firmatario
                Dim firstSigner As CollaborationSign = Factory.CollaborationSignsFacade.GetByIdCollaboration(idCollaboration).First()
                contacts.Add(New Contact() With {.Description = firstSigner.SignName, .EmailAddress = firstSigner.SignEMail})

            Case Code.ActiveSignerAntecedent
                '' L'ultimo firmatario prima di quello attivo
                Dim activeSignerIncremental As Short = Factory.CollaborationSignsFacade.GetCollaborationSignsBy(idCollaboration, 1).First().Incremental
                contacts.Add((From signer In Factory.CollaborationSignsFacade.SearchFull(idCollaboration, False, activeSignerIncremental - 1S) Select New Contact() With {.Description = signer.SignName, .EmailAddress = signer.SignEMail}).First())

            Case Code.CurrentSignerRole
                ''il settore del firmatario attivo
                Dim currentSigner As CollaborationSign = Factory.CollaborationSignsFacade.GetCollaborationSignsBy(idCollaboration, 1).FirstOrDefault()
                If currentSigner IsNot Nothing Then
                    Dim currentRoleUser As RoleUser = Factory.RoleUserFacade.GetByAccountsAndNotType(currentSigner.SignUser, Nothing, True).FirstOrDefault()
                    If currentRoleUser IsNot Nothing Then
                        For Each roleMailAddress As String In currentRoleUser.Role.EMailAddress.Split(";"c)
                            contacts.Add(New Contact() With {.Description = currentRoleUser.Role.Name, .EmailAddress = roleMailAddress})
                        Next
                    End If
                End If

            Case Code.AllSignersRole
                ''I settori di tutti i firmatari
                Dim currentSigners As IList(Of CollaborationSign) = Factory.CollaborationSignsFacade.GetByIdCollaboration(idCollaboration)
                If currentSigners IsNot Nothing AndAlso currentSigners.Count > 0 Then
                    For Each currentSigner As CollaborationSign In currentSigners
                        Dim currentRoleUser As RoleUser = Factory.RoleUserFacade.GetByAccountsAndNotType(currentSigner.SignUser, Nothing, True).FirstOrDefault()
                        If currentRoleUser IsNot Nothing Then
                            For Each roleMailAddress As String In currentRoleUser.Role.EMailAddress.Split(";"c)
                                contacts.Add(New Contact() With {.Description = currentRoleUser.Role.Name, .EmailAddress = roleMailAddress})
                            Next
                        End If
                    Next
                End If

            Case Code.SecretariesRole
                ''I settori di tutte le segreterie
                Dim collaborationUsers As IList(Of CollaborationUser) = Factory.CollaborationUsersFacade.GetByCollaboration(idCollaboration, True, DestinatonType.S)
                If collaborationUsers IsNot Nothing AndAlso collaborationUsers.Count > 0 Then
                    For Each currentUser As CollaborationUser In collaborationUsers
                        Dim currentRole As Role = Factory.RoleFacade.GetById(currentUser.IdRole.Value)
                        For Each roleMailAddress As String In currentRole.EMailAddress.Split(";"c)
                            contacts.Add(New Contact() With {.Description = currentRole.Name, .EmailAddress = roleMailAddress})
                        Next
                    Next
                End If

        End Select

        '' Rimuovo i contatti senza mail e tengo traccia
        '' Eseguo l'operazione in 2 step altrimenti sbaglia la cancellazione
        If removeNullEmailContacts Then
            For Each contact As Contact In contacts.FindAll(Function(c) String.IsNullOrEmpty(c.EmailAddress))
                FileLogger.Warn(LoggerName, String.Format("{0}: CollaborationFacade.SendMail - Collaborazione [{1}] : Scelta contatto tipologia {2} - Contatto ""{3}"" rimosso dall'elenco perchè senza indirizzo mail.", DocSuiteContext.Current.User.FullUserName, idCollaboration, collaborationStatusRecipientCode, contact.Description))
            Next
            contacts.RemoveAll(Function(c) String.IsNullOrEmpty(c.EmailAddress) OrElse c.EmailAddress = "mancante")
        End If

        '' Rimuovo i contatti con la stessa mail (prendendone il primo) --> Corrisponde ad un distinct su specifica proprietà
        Return contacts.GroupBy(Function(c) New With {Key c.EmailAddress}).Select(Function(c1) c1.First()).ToList()
    End Function

#End Region

#Region " Ctor/init "

    Public Sub New()
        MyBase.New()
    End Sub

#End Region
End Class

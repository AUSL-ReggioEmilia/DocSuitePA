Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Services.Logging
Imports VecompSoftware.NHibernateManager
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Model.Parameters
Imports VecompSoftware.DocSuiteWeb.DTO.Collaborations
Imports Newtonsoft.Json

<ComponentModel.DataObject()>
Public Class CollaborationSignsFacade
    Inherits BaseProtocolFacade(Of CollaborationSign, CollaborationSign, NHibernateCollaborationSignsDao)

#Region " Constructor "

    Public Sub New()
        MyBase.New()
    End Sub

#End Region

#Region " Method "

    ''' <summary> Controlla che una collaborazione sia già firmata dall'utente </summary>
    Public Function IsSigned(ByVal idCollaboration As Integer, ByVal user As String) As Boolean
        Return _dao.IsSigned(idCollaboration, Nothing, user)
    End Function

    ''' <summary> Controlla che una collaborazione nell'incrementale passato sia firmata dall'utente </summary>
    Public Function IsSigned(ByVal idCollaboration As Integer, ByVal incremental As Short, ByVal user As String) As Boolean
        Dim incrementals As New List(Of Short)({incremental})
        Return _dao.IsSigned(idCollaboration, incrementals, user)
    End Function

    Public Function SearchFull(ByVal idCollaboration As Integer, Optional ByVal isActive As Boolean = False, Optional ByVal incremental As Short = 0) As IList(Of CollaborationSign)
        Return _dao.SearchFull(idCollaboration, isActive, incremental)
    End Function

    Public Function GetNext(ByVal idCollaboration As Integer) As IList(Of CollaborationSign)
        Return _dao.GetNext(idCollaboration)
    End Function

    Public Function HasNext(ByVal idCollaboration As Integer) As Boolean
        Dim subsequents As IList(Of CollaborationSign) = _dao.GetNext(idCollaboration)
        Return Not subsequents.IsNullOrEmpty()
    End Function

    Public Function GetPrev(ByVal idCollaboration As Integer) As IList(Of CollaborationSign)
        Return _dao.GetPrev(idCollaboration)
    End Function

    Public Function HasPrev(ByVal idCollaboration As Integer) As Boolean
        Dim previous As IList(Of CollaborationSign) = _dao.GetPrev(idCollaboration)
        Return Not previous.IsNullOrEmpty()
    End Function

    ''' <summary> Avanza al destinatario successivo </summary>
    Public Sub UpdateForward(ByRef coll As Collaboration)
        FileLogger.Info(LoggerName, String.Format("Avanzamento destinatario successivo per collaborazione [{0}]", coll.Id))

        Dim unitOfWork As New NHibernateUnitOfWork("ProtDB")
        Try
            unitOfWork.BeginTransaction()

            Dim activeSigner As CollaborationSign = coll.GetFirstCollaborationSignActive()
            activeSigner.IsActive = 0S
            UpdateOnly(activeSigner)

            Dim signers As List(Of CollaborationSign) = coll.CollaborationSigns.OrderBy(Function(s) s.Incremental).ToList()
            For Each item As CollaborationSign In signers
                If item.Incremental > activeSigner.Incremental AndAlso (Not item.IsAbsent.HasValue OrElse Not item.IsAbsent.Value) Then
                    item.IsActive = 1S
                    UpdateOnly(item)
                    Exit For
                End If
            Next

            unitOfWork.Commit()
        Catch ex As Exception
            unitOfWork.Rollback()
            FileLogger.Error(LoggerName, String.Format("Impossibile avanzare collaborazione [{0}]", coll.Id), ex)
            Throw New DocSuiteException("Errore avanzamento", "Impossibile avanzare a destinatario successivo", ex)
        End Try
    End Sub

    Public Sub Insert(ByVal idColl As Integer, ByVal isActive As Short, ByVal idStatus As String, ByVal signUser As String, ByVal signName As String, ByVal signEMail As String, ByVal isRequired As Boolean)
        FileLogger.Info(LoggerName, String.Format("Inserimento firmatario collaborazione [{0}]", idColl))
        Dim collSign As New CollaborationSign()
        collSign.IdCollaboration = idColl
        collSign.Incremental = _dao.GetNextIncremental(idColl)
        collSign.IsActive = isActive
        collSign.IdStatus = idStatus
        collSign.SignUser = signUser
        collSign.SignName = signName
        collSign.SignEMail = signEMail
        collSign.RegistrationUser = DocSuiteContext.Current.User.FullUserName
        collSign.RegistrationDate = DateTimeOffset.UtcNow
        collSign.IsRequired = isRequired
        Save(collSign)
    End Sub


    Public Sub UpdateForwardSecretary(ByRef coll As Collaboration, ByVal idStatus As String, ByVal contactForward As List(Of CollaborationContact))
        FileLogger.Info(LoggerName, String.Format("Inoltro segreteria collaborazione [{0}]", coll.Id))
        'Inizia transazione
        Dim unitOfWork As New NHibernateUnitOfWork("ProtDB")
        Try
            unitOfWork.BeginTransaction()

            coll.IdStatus = idStatus
            Factory.CollaborationFacade.UpdateOnly(coll)

            ' Disattivo collaborationSigns
            Dim collSignList As IList(Of CollaborationSign) = SearchFull(coll.Id, True)
            For Each collSign As CollaborationSign In collSignList
                collSign.IsActive = 0
                UpdateOnly(collSign)
            Next

            Insert(coll.Id, 1, "", contactForward(0).Account, contactForward(0).DestinationName, contactForward(0).DestinationEMail, False)

            unitOfWork.Commit()
        Catch ex As Exception
            unitOfWork.Rollback()
            Throw New DocSuiteException("Errore Inoltro Segreteria", "Verificare destinatari di inoltro.", ex)
        End Try
    End Sub

    ''' <summary> Inserisce un nuovo Sign dopo l'ex-firmatario ed esegue uno shift degli altri firmatari, mantenendo l'ordine di firma. </summary>
    ''' <remarks> Tale funzione viene richiamata solo in presenza di piu' firmatari </remarks>
    Public Sub ShiftUsers(ByVal idCollaboration As Integer, ByVal contact As CollaborationContact, ByVal destination As String, ByVal userConnected As String)
        Dim unitOfWork As New NHibernateUnitOfWork(ProtDB)
        Try
            unitOfWork.BeginTransaction()
            Dim coll As Collaboration = Factory.CollaborationFacade.GetById(idCollaboration)
            Dim shiftedSigns As IList(Of CollaborationSign) = GetCollaborationSignsByGeActiveIncremental(idCollaboration)
            Dim activeSign As CollaborationSign = shiftedSigns.Item(0)
            With activeSign
                .IsActive = 0
                .SignName &= " (D)"
            End With
            ' Disattivo il precedente firmatario
            UpdateOnly(activeSign)

            Dim delta As Short = shiftedSigns.Item(shiftedSigns.Count - 1).Incremental
            shiftedSigns.RemoveAt(0) ' Rimuovo il firmatario disattivato dalla lista delle firme da spostare
            For Each item As CollaborationSign In shiftedSigns
                Dim shifted As CollaborationSign = getNewCollaborationSigns(item)
                shifted.Incremental += delta
                Save(shifted)
                coll.CollaborationSigns.Remove(item) ' Vedi inverse="true" su mapping
                Delete(item)
            Next

            ' Inserisco il nuovo firmatario in posizione del precedente firmatario +1
            Dim newActiveSign As New CollaborationSign
            With newActiveSign
                .IdCollaboration = activeSign.IdCollaboration
                .Incremental = activeSign.Incremental + 1S
                .IsActive = 1
                .IdStatus = String.Empty
                .SignUser = contact.Account
                .SignName = contact.DestinationName
                .SignEMail = contact.DestinationEMail
                .IsRequired = activeSign.IsRequired
                .RegistrationUser = userConnected
                .RegistrationDate = _dao.GetServerDate()
            End With
            Save(newActiveSign)

            unitOfWork.Commit()
        Catch ex As Exception
            unitOfWork.Rollback()
            FileLogger.Warn(LoggerName, "Errore su CollaborationSignsFacade.ShiftUsers : " & idCollaboration, ex)
            Throw New DocSuiteException("Errore ShiftUsers", "Aggiornamento Record CollaborationSigns", ex)
        End Try
    End Sub

    Private Function getNewCollaborationSigns(oldCollSign As CollaborationSign) As CollaborationSign
        Dim newSign As New CollaborationSign
        With newSign
            .IdCollaboration = oldCollSign.IdCollaboration
            .Incremental = oldCollSign.Incremental
            .IsActive = oldCollSign.IsActive
            .IdStatus = oldCollSign.IdStatus
            .SignUser = oldCollSign.SignUser
            .SignName = oldCollSign.SignName
            .SignEMail = oldCollSign.SignEMail
            .RegistrationUser = oldCollSign.RegistrationUser
            .RegistrationDate = oldCollSign.RegistrationDate
            .IsRequired = oldCollSign.IsRequired
        End With
        Return newSign
    End Function

    Public Function GetByIdCollaboration(ByVal idCollaboration As Integer) As IList(Of CollaborationSign)
        Return _dao.GetByIdCollaboration(idCollaboration)
    End Function
    Public Function GetByCollaboration(collaboration As Collaboration) As IList(Of CollaborationSign)
        Return GetByIdCollaboration(collaboration.Id)
    End Function

    Public Function GetCollaborationSignsBy(ByVal idCollaboration As Integer, ByVal signUser As String, ByVal isActive As Short) As IList(Of CollaborationSign)
        Return _dao.GetCollaborationSignsBy(idCollaboration, signUser, isActive)
    End Function

    Public Function GetCollaborationSignsBy(ByVal idCollaboration As Integer, ByVal isActive As Short) As IList(Of CollaborationSign)
        Return _dao.GetCollaborationSignsBy(idCollaboration, isActive)
    End Function

    ''' <summary> Ottiene l'incrementale successivo </summary>
    Public Function GetNextIncremental(ByVal idCollaboration As Integer) As Short
        Return _dao.GetNextIncremental(idCollaboration)
    End Function

    Public Function GetCollaborationSignsByGeActiveIncremental(ByVal idCollaboration As Integer) As IList(Of CollaborationSign)
        Return _dao.GetCollaborationSignsByGeActiveIncremental(idCollaboration)
    End Function

    Public Function IsCollaborationSignedByActiveSigner(ByVal idCollaboration As Integer) As Boolean
        Return _dao.IsCollaborationSignedByActiveSigner(idCollaboration)
    End Function

    Public Function GetByAccount(username As String) As IList(Of CollaborationSign)
        Return _dao.GetByAccount(username)
    End Function

    Public Function GetSigns(idCollaborations As Integer()) As IList(Of CollaborationSign)
        Return _dao.GetSigns(idCollaborations)
    End Function

    Public Function GetSignsDictionary(idCollaborations As Integer()) As IDictionary(Of Integer, List(Of CollaborationSign))
        Dim signs As IList(Of CollaborationSign) = GetSigns(idCollaborations)
        Return idCollaborations.Distinct().ToDictionary(Function(id) id, Function(id) signs.Distinct().Where(Function(cs) cs.IdCollaboration = id).ToList())
    End Function

    Public Function DisableActiveRequiredSigns(idCollaboration As Integer) As List(Of CollaborationSign)
        Return _dao.DisableActiveRequiredSigns(idCollaboration)
    End Function
    Public Function SkipRequiredSigns(collaboration As Collaboration) As List(Of CollaborationSign)
        Dim edited As List(Of CollaborationSign) = DisableActiveRequiredSigns(collaboration.Id)
        If Not edited.IsNullOrEmpty() Then
            Dim friendlyNames As IEnumerable(Of String) = edited.Select(Function(s) s.SignUser)
            Dim joined As String = String.Join(", ", friendlyNames.ToArray())
            FacadeFactory.Instance.CollaborationLogFacade.Insert(collaboration, String.Format("Rimossa obbligatorietà di firma da: {0}", joined))
        End If
        Return edited
    End Function

    Public Sub ForcedDelete(collSign As CollaborationSign)
        _dao.Delete(collSign)
    End Sub


    ''' <summary>
    ''' Il metodo restituisce i direttori (indicati da parametro) presenti come firmatari della collaborazione
    ''' </summary>
    ''' <param name="collaboration"></param>
    ''' <returns></returns>
    Public Function GetManagerSigners(collaboration As Collaboration) As IList(Of CollaborationSign)
        Dim managers As IList(Of String) = DocSuiteContext.Current.ProtocolEnv.AbsentManagersCertificates.Managers.Select(Function(m) m.Value.Account).ToList()
        Return collaboration.CollaborationSigns.Where(Function(s) managers.Any(Function(m) m.Eq(s.SignUser))).ToList()
    End Function

    Public Function SetAbsentManagers(collaboration As Collaboration, managersAccounts As AbsentManager()) As Boolean
        Dim currentCollaborationSign As CollaborationSign = collaboration.CollaborationSigns.Where(Function(s) s.IsActive = 1).FirstOrDefault()
        If currentCollaborationSign Is Nothing Then
            Return False
        End If
        Dim absentSigners As IEnumerable(Of CollaborationSign) = collaboration.CollaborationSigns.Where(Function(s) managersAccounts.Any(Function(m) m.Manager.Eq(s.SignUser)) AndAlso
                                                                                                            (s.Incremental >= currentCollaborationSign.Incremental OrElse s.SignName.Contains("(D)")) AndAlso
                                                                                                            (Not s.IsAbsent.HasValue OrElse Not s.IsAbsent.Value))
        For Each item As CollaborationSign In absentSigners
            item.IsAbsent = True
            item.IsRequired = False
            If managersAccounts.Any(Function(f) item.SignUser.Eq(f.Manager) AndAlso Not String.IsNullOrEmpty(f.Substitution)) Then
                item.SignName = String.Concat(item.SignName, " (D)")
            End If
            Update(item)
        Next

        Dim message As String = String.Empty
        For Each item As AbsentManager In managersAccounts
            message = String.Concat("Direttore ", item, " segnato come assente ", If(String.IsNullOrEmpty(item.Substitution), String.Empty, String.Concat("e vicariato da ", item.Substitution)), " .")
            Factory.CollaborationLogFacade.Insert(collaboration, String.Concat("Collaborazione n.", collaboration.Id, " - ", message))
        Next
        Factory.CollaborationLogFacade.Insert(collaboration, 0, 0, 0, CollaborationLogType.JS, JsonConvert.SerializeObject(managersAccounts))
        Return True
    End Function

    Public Function GetCollaborationSignDescription(signName As String, isAbsent As Boolean?) As String
        If isAbsent.HasValue AndAlso isAbsent.Value Then
            Return String.Concat(signName, " (ASSENTE)")
        End If
        Return signName
    End Function

    Public Function GetEffectiveSigners(idCollaboration As Integer) As ICollection(Of CollaborationSign)
        Return _dao.GetEffectiveSigners(idCollaboration)
    End Function
#End Region

End Class
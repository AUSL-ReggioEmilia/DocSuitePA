Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports NHibernate
Imports VecompSoftware.NHibernateManager
Imports VecompSoftware.DocSuiteWeb.Data


<DataObject()> _
Public Class DocumentFolderFacade
    Inherits BaseDocumentFacade(Of DocumentFolder, YearNumberIncrCompositeKey, NHibernateDocumentFolderDao)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Function CreateDocumentFolder(ByVal year As Short, ByVal number As Integer) As DocumentFolder
        Dim documentFolder As New DocumentFolder()
        documentFolder.Year = year
        documentFolder.Number = number
        documentFolder.IsActive = True
        documentFolder.Incremental = GetMaxId(year, number)

        Return documentFolder
    End Function

    ''' <summary>
    ''' Crea la gerarchia di cartelle per la pratica in base al contenitore selezionato
    ''' </summary>
    ''' <param name="document">Pratica di cui creare le cartelle</param>
    ''' <param name="contExtList">Lista di estensione del contenitore</param>
    ''' <returns>True se la creazione è andata a buon fine, false altrimenti.</returns>
    Public Function CreateFromContainerExtension(ByVal document As Document, ByVal contExtList As IList(Of ContainerExtension)) As Boolean
        Dim transaction As ITransaction = NHibernateSessionManager.Instance.GetSessionFrom(Me._dbName).BeginTransaction(IsolationLevel.ReadCommitted)
        Try
            Dim numberOfRole As Short = GetMaxId(document.Year, document.Number) - 1S
            For Each extension As ContainerExtension In contExtList
                'crea la cartella
                Dim _folder As DocumentFolder = CreateDocumentFolder(document.Year, document.Number)
                'imposta la gerarchia
                If (extension.IncrementalFather + numberOfRole > 0) Then
                    _folder.IncrementalFather = extension.IncrementalFather + numberOfRole
                Else
                    _folder.IncrementalFather = Nothing
                End If
                'imposta il numero di documenti richiesti
                Dim keyString As String() = extension.KeyValue.Split("|"c)
                If keyString.Length > 1 Then
                    _folder.DocumentsRequired = Integer.Parse(keyString(1))
                Else
                    _folder.DocumentsRequired = 0
                End If
                _folder.FolderName = keyString(0)

                'Salva la cartella
                SaveWithoutTransaction(_folder)
            Next

            transaction.Commit()
            Return True
        Catch ex As Exception
            transaction.Rollback()
            Return False
        End Try
    End Function

    Public Overloads Function GetById(ByVal year As Short, ByVal number As Integer, ByVal incremental As Short, Optional ByVal shoudLock As Boolean = False) As DocumentFolder
        Dim id As YearNumberIncrCompositeKey = New YearNumberIncrCompositeKey(year, number, incremental)
        Return MyBase.GetById(id, shoudLock)
    End Function

    Function GetByYearAndNumber(ByVal year As Short, ByVal number As Integer, ByVal incrementalFather As Short?) As IList(Of DocumentFolder)
        Return _dao.GetByYearAndNumber(year, number, incrementalFather)
    End Function

    Public Function GetByRole(ByVal year As Short, ByVal number As Integer, ByVal role As Role) As IList(Of DocumentFolder)
        Return _dao.GetByRole(year, number, role)
    End Function

    Public Function GetMaxId(ByVal year As Short, ByVal number As Integer) As Short
        Return _dao.GetMaxId(year, number)
    End Function

    Public Function GetRoot(ByVal year As Short, ByVal number As Integer, ByVal idroleincremental As Short) As IList(Of DocumentFolder)
        Return _dao.GetRoot(year, number, idroleincremental)
    End Function

    ''' <summary>
    ''' Inserisce la gerarchia di cartelle per un determinato settore
    ''' </summary>
    ''' <param name="year">Anno della pratica</param>
    ''' <param name="number">Numero della pratica</param>
    ''' <param name="FullIncrementalPath">IncrementalPath della Role di destinazione</param>
    ''' <param name="ParentFolder">Cartella padre, inizialmente impostata a Nothing</param>
    Public Sub InsertFoldersRole(ByVal year As Short, ByVal number As Integer, ByVal fullIncrementalPath As String, ByVal parentFolder As DocumentFolder)
        Dim roleDao As New NHibernateRoleDao("DocmDB")
        Dim foldersRole As IList(Of DocumentFolder) = Nothing
        Dim incremetalPath As String() = Nothing
        Dim parentRole As Role = Nothing
        Dim newIncrementalPath As String = String.Empty

        'condizione uscita ricorsione
        If fullIncrementalPath Is String.Empty Then
            Exit Sub
        End If

        'recupera gli id dei settori di tutta la gerarchia del settore passato
        incremetalPath = Split(fullIncrementalPath, "|")
        'rimuove il nodo root dal
        If incremetalPath.Length > 1 Then
            newIncrementalPath = Mid$(fullIncrementalPath, InStr(fullIncrementalPath, "|") + 1)
        Else
            newIncrementalPath = String.Empty
        End If

        'recupera il settore root
        parentRole = roleDao.GetById(Convert.ToInt32(incremetalPath(0)), False)
        'se esiste una cartella per quel settore allora passa alla sottocartella
        'altrimenti crea la cartella per quel settore e poi passa alla sottocartella
        foldersRole = GetByRole(year, number, parentRole)
        If foldersRole.Count > 0 Then
            InsertFoldersRole(year, number, newIncrementalPath, foldersRole(0))
        Else
            Dim newFolder As DocumentFolder = CreateDocumentFolder(year, number)
            If parentFolder Is Nothing Then
                newFolder.IncrementalFather = Nothing
            Else
                newFolder.IncrementalFather = parentFolder.Incremental
            End If
            newFolder.Role = parentRole
            newFolder.DocumentsRequired = 0
            Save(newFolder)
            InsertFoldersRole(year, number, newIncrementalPath, newFolder)
        End If
    End Sub

    ''' <summary> Restituisce la descrizione della scadenza di una cartella </summary>
    Public Shared Function GetExpiryDescription(ByRef folder As DocumentFolder, Optional ByRef isExpired As Boolean = False) As String
        If folder Is Nothing OrElse Not folder.ExpiryDate.HasValue Then
            Return String.Empty
        End If

        Return String.Format(" ({0} il {1:dd/MM/yyyy} - {2})", If(folder.ExpiryDate.Value <= DateTime.Now, "Scaduta", "Scadenza"), folder.ExpiryDate.Value, folder.Description)
    End Function

    ''' <summary>
    ''' Cancellazione logica delle cartelle
    ''' </summary>
    ''' <param name="folder"></param>
    ''' <remarks></remarks>
    Public Overloads Sub Delete(ByRef folder As DocumentFolder)
        folder.IsActive = False
        UpdateOnly(folder)
    End Sub

    Public Overloads Sub Delete(ByVal year As Short, ByVal number As Integer, ByVal incremental As Short)
        Dim CurrentDocumentFolder As DocumentFolder = Me.GetById(year, number, incremental)
        Delete(CurrentDocumentFolder)
    End Sub

    Public Overloads Sub Delete(ByVal YearNumberIncrCompositeKey As YearNumberIncrCompositeKey)
        Dim CurrentDocumentFolder As DocumentFolder = Me.GetById(YearNumberIncrCompositeKey)
        Delete(CurrentDocumentFolder)
    End Sub

End Class

Imports System.IO
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers
Imports VecompSoftware.Services.Biblos.Models

<ComponentModel.DataObject()>
Public Class FileResolutionFacade
    Inherits BaseResolutionFacade(Of FileResolution, Integer, NHibernateFileResolutionDao)

#Region " Constructor "

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal DbName As String)
        MyBase.New(DbName)
    End Sub

#End Region

#Region " Methods "

    Public Function GetByResolution(ByVal resl As Resolution) As IList(Of FileResolution)
        Return _dao.GetByResolution(resl)
    End Function

    Public Function Extract(ByVal resolutionList As List(Of Resolution), ByRef tempDirectory As DirectoryInfo, ByRef inDirectory As DirectoryInfo, ByRef outDirectory As DirectoryInfo, ByRef fieldName As String) As String

        Dim errorMessage As String = Nothing
        ' Prendo i permessi per manipolare cartelle sul server
        Dim impersonator As Impersonator = Nothing
        Try
            impersonator = CommonAD.ImpersonateSuperUser()

            ' directory temporanea
            Dim shareDirectory As String = DocSuiteContext.Current.ProtocolEnv.FDQMultipleShare.Replace("%ServerName%", CommonShared.MachineName)
            If Not Directory.Exists(shareDirectory) Then
                Throw New DirectoryNotFoundException("Impossibile trovare la directory temporanea: reimpostare FDQMultipleShare.")
            End If

            ' Directory temporanea utente
            Dim tmpDirPath As String = Path.Combine(shareDirectory, String.Format("{0}{1:hhmmss}", DocSuiteContext.Current.User.UserName, Now))

            ' Mi assicuro che la directory sia vuota
            tempDirectory = New DirectoryInfo(tmpDirPath)
            If (tempDirectory.Exists) Then
                tempDirectory.Delete(True)
            End If

            tempDirectory.Create()

            ' Creo la sottodirectory per i documenti da salvare
            inDirectory = tempDirectory.CreateSubdirectory("In")
            If Not inDirectory.Exists Then
                Throw New IOException("Impossibile creare la directory temporanea: " & inDirectory.FullName)
            End If

            ' Creo la directory per i documenti firmati
            outDirectory = tempDirectory.CreateSubdirectory("Out")
            If Not outDirectory.Exists Then
                Throw New DirectoryNotFoundException("Impossibile trovare la directory: " & outDirectory.FullName)
            End If

            ' Estraggo i documenti per la firma
            For Each resolution As Resolution In resolutionList
                ' Salvo nella directory temporanea il file rinominandolo nel suo id (per vecchie logiche di GUI)
                Extract(resolution, fieldName, inDirectory)
            Next

        Catch ex As Exception
            ' Se esiste ancora la directory temporanea la cancello
            If tempDirectory IsNot Nothing Then
                tempDirectory.Refresh()
                If tempDirectory.Exists Then
                    tempDirectory.Delete(True)
                End If
            End If

            errorMessage = "Errore firma multipla: " & ex.Message

        Finally
            ' Rilascio i permessi per manipolare cartelle sul server
            impersonator.ImpersonationUndo()
        End Try

        Return errorMessage
    End Function

    ''' <summary>
    ''' Estrae il file indicato
    ''' </summary>
    ''' <param name="resolution">Atto</param>
    ''' <param name="fieldName">Nome del campo contente l'id catena necessario</param>
    ''' <param name="directory">Directory dove salvare il file</param>
    ''' <returns>Nome del file generato</returns>
    Public Function Extract(ByVal resolution As Resolution, ByVal fieldName As String, ByVal directory As DirectoryInfo) As String
        Return Extract(resolution.File, resolution.Location.ReslBiblosDSDB, fieldName, directory)
    End Function

    ''' <summary>
    ''' Estrae il file indicato
    ''' </summary>
    ''' <param name="fileResolution">File dell'Atto</param>
    ''' <param name="server">Server biblos</param>
    ''' <param name="archive">Archivio biblos</param>
    ''' <param name="fieldName">Nome del campo contente l'id catena necessario</param>
    ''' <param name="directory">Directory dove salvare il file</param>
    ''' <returns>Nome del file generato</returns>
    Public Function Extract(ByVal fileResolution As FileResolution, ByVal archive As String, ByVal fieldName As String, ByVal directory As DirectoryInfo) As String
        ' id catena del campo desiderato
        Dim chainId As Integer = Integer.Parse(ReflectionHelper.GetPropertyCase(fileResolution, fieldName).ToString())

        Dim doc As New BiblosDocumentInfo(archive, chainId)

        Dim filename As String = String.Format("{0}{1}{2}", fileResolution.Resolution.Id, ResolutionJournalFacade.Separator, doc.Name)
        doc.SaveToDisk(directory, filename)

        Return filename
    End Function

#End Region

End Class
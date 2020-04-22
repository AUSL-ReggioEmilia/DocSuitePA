Imports System.IO
Imports VecompSoftware.Services.Logging

Public Class Service
    Implements IWebService

    Protected Db As DbAdmin

    Public Sub New()
        Db = New DbAdmin

        'Inizializzo il logger
        FileLogger.Initialize()
    End Sub

    ''' <summary>
    ''' Permette l'inserimento iniziale della signature di un documento
    ''' </summary>
    ''' <param name="tipoDocumento"></param>
    ''' <param name="titolo"></param>
    ''' <param name="dataAdozione"></param>
    ''' <param name="oggetto"></param>
    ''' <param name="revocato"></param>
    ''' <returns>N_pubblicazione</returns>
    ''' <remarks></remarks>
    Public Function Inserisci(ByVal tipoDocumento As String, ByVal titolo As String, ByVal dataAdozione As DateTime, ByVal oggetto As String, ByVal revocato As Short) As Long Implements IWebService.Inserisci

        Dim id As Long

        FileLogger.Debug("testLogger", "Inserimento documento iniziale")
        Try
            id = Db.Inserisci(tipoDocumento, titolo, dataAdozione, oggetto, revocato)

            FileLogger.Info("testLogger", "Inserimento documento iniziale id: " & id)
        Catch ex As Exception
            FileLogger.Error("testLogger", "Inserimento documento iniziale id: " & id, ex)
            Throw New Exception("Errore in Inserimento del documento id: & id", ex)
        End Try

        Return id

    End Function


    ''' <summary>
    ''' Permette di pubblicare un documento
    ''' </summary>
    ''' <param name="attoPath"></param>
    ''' <param name="nPubblicazione"></param>
    ''' <param name="ritirato"></param>
    ''' <param name="dataCaricamento"></param>
    ''' <returns>nPubblicazione, nothing se non funziona</returns>
    Public Function PubblicaPath(ByVal attoPath As String, ByVal nPubblicazione As Long, ByVal ritirato As Short, ByVal dataCaricamento As DateTime, ByVal oggetto As String) As Long Implements IWebService.PubblicaPath
        Dim fStream As FileStream
        Dim br As BinaryReader

        'Inizialize
        Dim fi As FileInfo
        fStream = Nothing
        br = Nothing

        FileLogger.Debug("testLogger", "Pubblica Atto")

        Try
            fi = New FileInfo(attoPath)
            Dim numBytes As Long = fi.Length
            fStream = New FileStream(attoPath, FileMode.Open, FileAccess.Read)
            br = New BinaryReader(fStream)
            Dim data As Byte() = br.ReadBytes(CInt(numBytes))

            Db.Pubblica(data, nPubblicazione, ritirato, dataCaricamento, oggetto)

            FileLogger.Info("testLogger", "Pubblica Atto path: " & attoPath & " id: " & nPubblicazione)
        Catch ex As Exception
            FileLogger.Error("testLogger", "Pubblica Atto path: " & attoPath & " id: " & nPubblicazione, ex)
            Throw New Exception("Errore nella pubblicazione del documento path: " & attoPath & " id: " & nPubblicazione, ex)
        Finally
            If fStream IsNot Nothing Then
                fStream.Close()
                fStream.Dispose()
            End If
            If br IsNot Nothing Then
                br.Close()
                br.Dispose()
            End If
        End Try

        Return nPubblicazione
    End Function


    ''' <summary>
    ''' Revoca un documento precedentemente caricato
    ''' </summary>
    ''' <param name="nPubblicazione"></param>
    ''' <remarks></remarks>
    Public Sub Revoca(ByVal nPubblicazione As Long) Implements IWebService.Revoca
        FileLogger.Debug("testLogger", "Revoca Atto")
        Try
            Db.Revoca(nPubblicazione)

            FileLogger.Info("testLogger", "Revoca Atto id: " & nPubblicazione)

        Catch ex As Exception
            FileLogger.Error("testLogger", "Revoca Atto id: " & nPubblicazione, ex)
            Throw New Exception("Errore nella revoca del documento id: " & nPubblicazione, ex)
        End Try

    End Sub

End Class

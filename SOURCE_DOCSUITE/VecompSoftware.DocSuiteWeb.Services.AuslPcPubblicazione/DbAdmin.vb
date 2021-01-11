Imports System.Data.SqlClient
Imports VecompSoftware.Services.Logging

''' <summary>
''' Classe per interfacciarsi al db 
''' </summary>
''' <remarks></remarks>
Public Class DbAdmin

#Region "Attribute"

    Private ReadOnly _connectionString As String

#End Region

#Region "New"

    Public Sub New()
        'per default prendo la stringa di connessione in Web.config
        _connectionString = ConfigurationManager.ConnectionStrings("TestConnection").ConnectionString

        If (_connectionString = Nothing) Then
            Throw New Exception("can't get the connectionString")
        End If
    End Sub

    Public Sub New(ByVal strConn As String)
        _connectionString = strConn
    End Sub

#End Region

#Region "Property"

    Public ReadOnly Property ConnectionString() As String
        Get
            Return _connectionString
        End Get
    End Property

#End Region

#Region "Method"


    ''' <summary>
    ''' Crea un adapter per interfacciarsi col db nelle operazioni di inserimento pubblicazione o revoca dell'atto
    ''' </summary>
    ''' <param name="connection">SqlConnection per connessione al db</param>
    ''' <param name="isRevoca">identifica se l'adapter deve essere creato per un operazione di revoca o pubblicazione atto</param>
    ''' <returns>adapter</returns>
    ''' <remarks></remarks>
    Public Function CreateSqlDataAdapter(ByVal connection As SqlConnection, ByVal isRevoca As Boolean) As SqlDataAdapter

        Dim adapter As SqlDataAdapter = New SqlDataAdapter
        adapter.MissingSchemaAction = MissingSchemaAction.AddWithKey

        'SELECT
        adapter.SelectCommand = New SqlCommand("SELECT * FROM dbo.Documenti where ID_documento in (SELECT MAX(ID_documento) FROM dbo.Documenti)", connection)

        'INSERIMENTO
        adapter.InsertCommand = New SqlCommand("INSERT INTO dbo.Documenti " &
        "(Tipo_documento, Titolo, Data_adozione, Oggetto, Revocato)VALUES" &
        "(@Tipo_documento, @Titolo, @Data_adozione, @Oggetto, @Revocato)", connection)

        adapter.InsertCommand.Parameters.Add("@Tipo_documento", SqlDbType.NChar, 10, "Tipo_documento")
        adapter.InsertCommand.Parameters.Add("@Titolo", SqlDbType.NVarChar, 100, "Titolo")
        adapter.InsertCommand.Parameters.Add("@Data_adozione", SqlDbType.DateTime, 20, "Data_adozione")
        adapter.InsertCommand.Parameters.Add("@Oggetto", SqlDbType.NVarChar, 255, "Oggetto")
        adapter.InsertCommand.Parameters.Add("@Revocato", SqlDbType.Int, 10, "Revocato")

        'UPDATE
        If isRevoca Then
            'Se è un update di revoca
            adapter.UpdateCommand = New SqlCommand(
             "UPDATE dbo.Documenti SET Revocato = @Revocato WHERE ID_documento = @N_Pubblicazione", connection)

            adapter.UpdateCommand.Parameters.Add("@N_Pubblicazione", SqlDbType.Int, 10, "N_Pubblicazione")
            adapter.UpdateCommand.Parameters.Add("@Revocato", SqlDbType.Int, 15, "Revocato")

        Else
            'update di pubblicazione Atto
            adapter.UpdateCommand = New SqlCommand(
                "UPDATE dbo.Documenti " &
                "SET Atto = @Atto, N_Pubblicazione = @N_Pubblicazione, " &
                "Ritirato = @Ritirato, " &
                "Data_caricamento = @Data_caricamento, " &
                "Dimensione_file = @Dimensione_file, " &
                "Data_fine_pubb = @Data_fine_pubb, " &
                "Oggetto = @Oggetto " &
                "WHERE ID_documento = @N_Pubblicazione", connection)

            adapter.UpdateCommand.Parameters.Add("@Atto", SqlDbType.Image, Integer.MaxValue, "Atto")
            adapter.UpdateCommand.Parameters.Add("@N_Pubblicazione", SqlDbType.Int, 15, "N_Pubblicazione")
            adapter.UpdateCommand.Parameters.Add("@Ritirato", SqlDbType.Int, 15, "Ritirato")
            adapter.UpdateCommand.Parameters.Add("@Data_caricamento", SqlDbType.DateTime, 20, "Data_caricamento")
            adapter.UpdateCommand.Parameters.Add("@Dimensione_file", SqlDbType.NVarChar, 50, "Dimensione_file")
            adapter.UpdateCommand.Parameters.Add("@Data_fine_pubb", SqlDbType.DateTime, 20, "Data_fine_pubb")
            adapter.UpdateCommand.Parameters.Add("@Oggetto", SqlDbType.NVarChar, 255, "Oggetto")
        End If

        Return adapter
    End Function

    Public Sub ExecuteStoreProcedure(ByVal connection As SqlConnection, ByVal nomeStoreProcedure As String, ByVal idDocumento As Integer, ByVal azione As Integer)

        If Not String.IsNullOrEmpty(nomeStoreProcedure) Then
            FileLogger.Debug("testLogger", "ExecuteStroreProcedure azione: " & azione)
            Using cmd As New SqlCommand(nomeStoreProcedure, connection)
                Try
                    cmd.CommandType = CommandType.StoredProcedure

                    'The param names are exactly the same with SP WriteData's
                    cmd.Parameters.AddWithValue("@idDocumento", idDocumento)
                    cmd.Parameters.AddWithValue("@azione", azione)

                    cmd.ExecuteNonQuery()
                    FileLogger.Info("testLogger", "ExecuteStroreProcedure azione: " & azione)
                Catch ex As StoreProcedureException
                    FileLogger.Error("testLogger", "ExecuteStroreProcedure azione: " & azione, ex)
                    Throw New StoreProcedureException("Errore nel'esecuzione della store procedure ", ex)
                End Try
            End Using
        End If
    End Sub

    ''' <summary>
    ''' Inserice la tupla iniziale del documento
    ''' </summary>
    ''' <param name="tipoDocumento"></param>
    ''' <param name="titolo"></param>
    ''' <param name="dataAdozione"></param>
    ''' <param name="oggetto"></param>
    ''' <param name="revocato"></param>
    ''' <returns>Identity del record creato</returns>
    ''' <remarks></remarks>
    Public Function Inserisci(ByVal tipoDocumento As String, ByVal titolo As String, ByVal dataAdozione As DateTime, ByVal oggetto As String, ByVal revocato As Short) As Long

        Using connection As New SqlConnection(ConnectionString())
            Dim adapter As SqlDataAdapter = Nothing
            Try
                connection.Open()

                adapter = CreateSqlDataAdapter(connection, False)

                Dim ds As New DataSet
                adapter.Fill(ds, "dbo.Documenti")

                ' Get the Data Table
                Dim dt As DataTable = ds.Tables("dbo.Documenti")

                ' Add A Row
                Dim newRow As DataRow = dt.NewRow()
                newRow("Tipo_documento") = tipoDocumento
                newRow("Titolo") = titolo
                newRow("Data_adozione") = dataAdozione
                newRow("Oggetto") = oggetto
                newRow("Revocato") = revocato
                dt.Rows.Add(newRow)

                ' 3. Insert
                adapter.Update(ds, "dbo.Documenti")

                dt.AcceptChanges()

                Dim newId As Integer = 1
                If newRow("ID_documento") > 0 Then
                    newId = newRow("ID_documento")
                End If
                'esegue la store procedure
                ExecuteStoreProcedure(connection, ConfigurationManager.AppSettings.Get("InserimentoStoreProcedure"), newId, 0)

                Return newId
            Catch ex As Exception
                Throw New PubblicationException("Errore nel inserimento record nella tabella ", ex)
            Finally
                connection.Close()
                If adapter IsNot Nothing Then
                    adapter.Dispose()
                End If
            End Try

        End Using


    End Function


    Public Function GetAtto(nPubblicazione As Long) As Byte()

        Dim atto As Byte()

        Using connection As New SqlConnection(ConnectionString)
            Dim adapter As SqlDataAdapter = Nothing
            Try
                connection.Open()

                adapter = CreateSqlDataAdapter(connection, False)

                adapter.SelectCommand = New SqlCommand("SELECT Atto FROM dbo.Documenti where ID_documento = @N_Pubblicazione", connection)
                adapter.SelectCommand.Parameters.Add("@N_Pubblicazione", SqlDbType.Int).Value = nPubblicazione

                Dim ds As New DataSet
                adapter.Fill(ds, "dbo.Documenti")

                If ds.Tables(0).Rows.Count > 1 Then
                    Throw New PubblicationException("Errore duplicazione numero pubblicazione")
                End If

                atto = ds.Tables(0).Rows(0).Item(0)

            Catch ex As Exception
                Throw New Exception(String.Format("Atto con pubblicazione numero {0} non trovato", nPubblicazione.ToString()), ex)
            Finally
                connection.Close()
                If adapter IsNot Nothing Then
                    adapter.Dispose()
                End If
            End Try
        End Using

        Return atto
    End Function

    ''' <summary>
    ''' Permette la pubblicazione del atto
    ''' </summary>
    ''' <param name="atto">array di byte contenente il documento</param>
    ''' <param name="nPubblicazione"></param>
    ''' <param name="ritirato"></param>
    ''' <param name="dataCaricamento"></param>
    ''' <remarks></remarks>
    Public Sub Pubblica(ByRef atto() As Byte, ByVal nPubblicazione As Long, ByVal ritirato As Short, ByVal dataCaricamento As DateTime, ByVal oggetto As String)


        Using connection As New SqlConnection(ConnectionString)
            Dim adapter As SqlDataAdapter = Nothing
            Try
                connection.Open()

                adapter = CreateSqlDataAdapter(connection, False)

                Dim ds As New DataSet
                adapter.Fill(ds, "dbo.Documenti")

                ' Get the Data Table
                Dim dt As DataTable = ds.Tables("dbo.Documenti")

                dt.Rows(0)("Atto") = atto
                dt.Rows(0)("N_Pubblicazione") = nPubblicazione
                dt.Rows(0)("Oggetto") = oggetto
                dt.Rows(0)("Ritirato") = ritirato
                dt.Rows(0)("Data_Caricamento") = dataCaricamento
                dt.Rows(0)("Dimensione_file") = atto.Length.ToString()
                ' Tengo in considerazione la tempisticaPubblicazione del web.config, ed imposto la mezzanotte del giorno successivo per comprendere n-esimo giorno
                dt.Rows(0)("Data_fine_pubb") = dataCaricamento.AddDays(Integer.Parse(ConfigurationManager.AppSettings.Get("tempisticaPubblicazione")) + 1).Date.Add(New TimeSpan(0, 0, 0))

                ' 3. Update
                adapter.Update(ds, "dbo.Documenti")

                dt.AcceptChanges()

                'esegue la store procedure
                ExecuteStoreProcedure(connection, ConfigurationManager.AppSettings.Get("PubblicazioneStoreProcedure"), nPubblicazione, 1)
            Catch ex As Exception
                Throw New Exception("Errore nella pubblicazione dell'atto", ex)
            Finally
                connection.Close()
                If adapter IsNot Nothing Then
                    adapter.Dispose()
                End If
            End Try
        End Using

    End Sub

    ''' <summary>
    ''' Permette la revoca di un atto dato il suo identificativo N_Pubblicazione
    ''' </summary>
    ''' <param name="nPubblicazione"></param>
    ''' <remarks></remarks>
    Public Sub Revoca(ByVal nPubblicazione As Long)

        Using connection As New SqlConnection(ConnectionString)
            Dim adapter As SqlDataAdapter = Nothing
            Try
                connection.Open()

                adapter = CreateSqlDataAdapter(connection, True)

                Dim ds As New DataSet
                adapter.Fill(ds, "dbo.Documenti")

                ' Get the Data Table
                Dim dt As DataTable = ds.Tables("dbo.Documenti")

                dt.Rows(0)("N_Pubblicazione") = nPubblicazione
                dt.Rows(0)("Revocato") = 1

                ' 3. Update
                adapter.Update(ds, "dbo.Documenti")

                dt.AcceptChanges()

                'esegue la store procedure
                ExecuteStoreProcedure(connection, ConfigurationManager.AppSettings.Get("RevocaStoreProcedure"), nPubblicazione, 2)
            Catch ex As Exception
                Throw New Exception("Errore nella revoca dell'atto", ex)
            Finally
                connection.Close()
                If adapter IsNot Nothing Then
                    adapter.Dispose()
                End If
            End Try
        End Using

    End Sub

#End Region



End Class

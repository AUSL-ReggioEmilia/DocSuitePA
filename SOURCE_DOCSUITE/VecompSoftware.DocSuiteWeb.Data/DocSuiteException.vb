''' <summary> Eccezione applicativa gestita </summary>
''' <remarks> Usare per comunicare le eccezioni all'utente finale, dopo che sono state gestite </remarks>
Public Class DocSuiteException
    Inherits Exception

#Region "[ Property ]"

    Property Titolo() As String
        Get
            Dim temp As Object = Data.Item("Titolo")
            If temp Is Nothing Then
                Return String.Empty
            Else
                Return temp.ToString()
            End If
        End Get
        Set(value As String)
            Data.Item("Titolo") = value
        End Set
    End Property

    Property Descrizione() As String
        Get
            Dim temp As Object = Data.Item("Descrizione")
            If temp Is Nothing Then
                Return String.Empty
            Else
                Return temp.ToString()
            End If
        End Get
        Set(value As String)
            Data.Item("Descrizione") = value
        End Set
    End Property

    Property Url() As String
        Get
            Dim temp As Object = Data.Item("Url")
            If temp Is Nothing Then
                Return String.Empty
            Else
                Return temp.ToString()
            End If
        End Get
        Set(value As String)
            Data.Item("Url") = value
        End Set
    End Property

    Property User() As String
        Get
            Dim temp As Object = Data.Item("User")
            If temp Is Nothing Then
                Return String.Empty
            Else
                Return temp.ToString()
            End If
        End Get
        Set(value As String)
            Data.Item("User") = value
        End Set
    End Property

    ''' <summary>
    ''' Definisce un indirizzo di redirect da raggiungere dopo la visualizzazione (dopo l'esecuzione del timer)
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property RedirectUriAddress As Uri

    ''' <summary>
    ''' Definisce dopo quanto tempo (secondi) effettuare il redirect alla pagina definita da RedirectAddress
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Property RedirectUriLatency As Integer

#End Region

#Region "[ Constructor ]"

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal descrizione As String)
        MyBase.New(descrizione)
        Me.Descrizione = descrizione
    End Sub

    Public Sub New(ByVal descrizione As String, ByVal inner As Exception)
        MyBase.New(descrizione, inner)
    End Sub

    Public Sub New(ByVal titolo As String, ByVal descrizione As String)
        MyBase.New(descrizione)
        Me.Titolo = titolo
        Me.Descrizione = descrizione
    End Sub

    Public Sub New(ByVal titolo As String, ByVal descrizione As String, ByVal inner As Exception)
        MyBase.New(descrizione, inner)
        Me.Titolo = titolo
        Me.Descrizione = descrizione
    End Sub

    Public Sub New(ByVal titolo As String, ByVal descrizione As String, ByVal url As String, ByVal user As String)
        Me.New(titolo, descrizione)
        Me.Url = url
        Me.User = user
    End Sub

    Public Sub New(ByVal titolo As String, ByVal descrizione As String, ByVal redirectUriAddress As Uri, ByVal redirectUriLatency As Integer)
        MyBase.New(descrizione)
        Me.Titolo = titolo
        Me.Descrizione = descrizione
        Me.RedirectUriAddress = redirectUriAddress
        Me.RedirectUriLatency = redirectUriLatency
    End Sub

#End Region

End Class

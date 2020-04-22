Imports VecompSoftware.DocSuiteWeb.Data

Public Class ParerDetail
    Inherits CommonBasePage

#Region " Properties "

    Private ReadOnly Property Year() As Short
        Get
            Return Short.Parse(Request("Year"))
        End Get
    End Property

    Private ReadOnly Property Number() As Integer
        Get
            Return Integer.Parse(Request("Number"))
        End Get
    End Property

    Private ReadOnly Property IdResolution() As Integer
        Get
            Return Integer.Parse(Request("Id"))
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        Title = "Dettaglio archiviazione"
        MasterDocSuite.TitleVisible = False

        Select Case Type
            Case "Prot"
                LoadProtocolParer()
            Case "Resl"
                LoadResolutionParer()
        End Select
    End Sub

#End Region

#Region " Methods "

    Private Sub LoadResolutionParer()
        If Not Facade.ResolutionParerFacade.Exists(IdResolution) Then
            Exit Sub
        End If

        Dim resolution As Resolution = Facade.ResolutionFacade.GetById(IdResolution)
        Dim item As ResolutionParer = Facade.ResolutionParerFacade.GetByResolution(resolution)

        lblCaption.Text = Facade.ResolutionTypeFacade.GetDescription(resolution.Type)
        lblProtocol.Text = Facade.ResolutionFacade.CalculateFullNumber(resolution, resolution.Type.Id, False)
        If item.ArchivedDate.HasValue Then
            lblArchivedDate.Text = item.ArchivedDate.Value.ToString("dd/MM/yyyy HH:mm:ss")
        End If
        lblParerUri.Text = item.ParerUri
        lblIsForArchive.Text = item.IsForArchive.ToString()
        lblHasError.Text = item.HasError.ToString()
        lblLastError.Text = item.LastError
        If item.LastSendDate.HasValue Then
            lblLastSendDate.Text = item.LastSendDate.Value.ToString("dd/MM/yyyy HH:mm:ss")
        End If
    End Sub

    Private Sub LoadProtocolParer()
        If Not Facade.ProtocolParerFacade.Exists(Year, Number) Then
            Exit Sub
        End If

        lblCaption.Text = "Protocollo"

        Dim item As ProtocolParer = Facade.ProtocolParerFacade.GetById(New YearNumberCompositeKey(Year, Number))

        lblProtocol.Text = String.Format("{0}/{1:000000}", Year, Number)
        If item.ArchivedDate.HasValue Then
            lblArchivedDate.Text = item.ArchivedDate.Value.ToString("dd/MM/yyyy HH:mm:ss")
        End If
        lblParerUri.Text = item.ParerUri
        lblIsForArchive.Text = BoolToString(item.IsForArchive)
        If item.LastSendDate.HasValue Then
            lblLastSendDate.Text = item.LastSendDate.Value.ToString("dd/MM/yyyy HH:mm:ss")
        End If
        lblHasError.Text = BoolToString(item.HasError)
        lblLastError.Text = item.LastError
    End Sub

    Private Function BoolToString(item As Boolean) As String
        If item Then
            Return "Sì"
        Else
            Return "No"
        End If
    End Function

#End Region

End Class
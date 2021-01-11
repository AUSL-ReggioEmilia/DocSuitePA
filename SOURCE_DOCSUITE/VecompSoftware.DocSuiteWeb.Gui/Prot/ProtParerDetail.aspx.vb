Imports VecompSoftware.DocSuiteWeb.Data

Public Class ProtParerDetail
    Inherits ProtBasePage

#Region " Properties "

    Private ReadOnly Property Year() As Integer
        Get
            Return Integer.Parse(Request("Year"))
        End Get
    End Property

    Private ReadOnly Property Number() As Integer
        Get
            Return Integer.Parse(Request("Number"))
        End Get
    End Property

#End Region

#Region " Events "

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Facade.ProtocolParerFacade.Exists(Year, Number) Then

            Me.Title = "Dettaglio archiviazione"
            Me.MasterDocSuite.TitleVisible = False

            Dim item As ProtocolParer = Facade.ProtocolParerFacade.GetByProtocol(Year, Number)

            lblProtocol.Text = String.Format("{0}/{1:000000}", Year, Number)
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

        End If

        ' Gestione dell'errore
    End Sub

#End Region

End Class
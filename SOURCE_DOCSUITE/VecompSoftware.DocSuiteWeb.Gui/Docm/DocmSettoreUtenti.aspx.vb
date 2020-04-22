Public Class DocmSettoreUtenti
    Inherits DocmBasePage

#Region " Fields "

    Private _publication As String

#End Region

#Region " Properties "

    Public Property Publication() As String
        Get
            Return _publication
        End Get
        Set(ByVal value As String)
            _publication = value
        End Set
    End Property

#End Region

#Region " Events "

    Private Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        uscSelezioneSettoreUtenti.Year = CurrentDocumentYear
        uscSelezioneSettoreUtenti.Number = CurrentDocumentNumber
        uscSelezioneSettoreUtenti.FullManagerS = Request.QueryString("txtIdRoleRightM")
        uscSelezioneSettoreUtenti.RoleS = Request.QueryString("txtPIdOwner")
        uscSelezioneSettoreUtenti.StepP = Request.QueryString("txtPStep")
        If Not Page.IsPostBack Then
            uscSelezioneSettoreUtenti.Initialize()
        End If
    End Sub


    Protected Sub btnConferma_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnConferma.Click
        uscSelezioneSettoreUtenti.TvwCheck(uscSelezioneSettoreUtenti.Treeview.Nodes(0))
        RegisterFolderRefreshFullScript()
    End Sub

#End Region

End Class

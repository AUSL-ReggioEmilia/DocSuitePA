Imports System.Collections.Generic
Imports System.Linq
Imports System.Web
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.Helpers.ExtensionMethods


Public Class uscInteropInfo
    Inherits DocSuite2008BaseControl

    Private _segnautreReader As SegnaturaReader

    Private _source As String
    Public Property Source As String
        Get
            If String.IsNullOrEmpty(_source) Then
                _source = ViewState("Source").ToString()
            End If
            Return _source
        End Get
        Set(value As String)
            ViewState("Source") = value
            _source = value
        End Set
    End Property

    Public ReadOnly Property SegnaturaReader As SegnaturaReader
        Get
            If _segnautreReader Is Nothing Then
                Dim defaultIdContactParent As Integer = Nothing
                If ProtocolEnv.DefaultContactParentForInteropSender > 0 Then
                    defaultIdContactParent = ProtocolEnv.DefaultContactParentForInteropSender
                End If

                _segnautreReader = New SegnaturaReader(Source, defaultIdContactParent)
                _segnautreReader.LoadDocument()
            End If

            Return _segnautreReader
        End Get
    End Property


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load

        If Not IsPostBack And Visible Then

            If String.IsNullOrEmpty(Source) Then
                Return
            End If

            If Not SegnaturaReader.HasErrors Then
                tblValidationOk.Visible = True
                If Not SegnaturaReader.IsValid Then
                    ' Validata con Warning
                    lblValidazione.Text = "Validata dopo correzione automatica di errori"
                Else
                    lblValidazione.Text = "Valida"
                End If

                lblOggetto.Text = SegnaturaReader.GetOggetto()
                lblDocumentPrincipale.Text = SegnaturaReader.GetNomeDocumentoPrincipale()

                Dim contacts As List(Of ContactDTO) = SegnaturaReader.GetMittenteForAddressBook(False)
                If Not contacts.IsNullOrEmpty() Then
                    Dim descriptions As IEnumerable(Of String) = contacts.Select(Function(c) c.Contact.Description)
                    lblMittente.Text = String.Join(", ", descriptions)
                End If

                lblProtocolloMittente.Text = SegnaturaReader.GetNumeroRegistrazione()

            Else
                tblValidationError.Visible = True
                lblValidazioneErrore.Text = "Errore in lettura Segnatura."
            End If

        End If
    End Sub

End Class
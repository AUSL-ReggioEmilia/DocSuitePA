Imports System.Collections.Generic
Imports System.Web
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.DTO.Collaborations
Imports VecompSoftware.Helpers.Web.ExtensionMethods

''' <summary> Classe base per pagine che eseguono firme multiple </summary>
Public Class MultipleSignBasePage
    Inherits CommonBasePage

#Region " Fields "

    Private _originalDocuments As New List(Of MultiSignDocumentInfo)
    Private _signedDocuments As New List(Of MultiSignDocumentInfo)

#End Region

#Region " Properties "

    ''' <summary> Documenti originali </summary>
    Protected Property OriginalDocuments As List(Of MultiSignDocumentInfo)
        Get
            If _originalDocuments.Count <> 0 Then
                Return _originalDocuments
            End If
            ' serializzo a manina per mancanza di tempo
            Dim objects() As String = DirectCast(ViewState("OriginalDocuments"), String())
            For Each serial As String In objects
                _originalDocuments.Add(New MultiSignDocumentInfo(HttpUtility.ParseQueryString(serial)))
            Next
            Return _originalDocuments
        End Get
        Set(value As List(Of MultiSignDocumentInfo))
            _originalDocuments = value
            Dim objects(value.Count - 1) As String
            For i As Integer = 0 To value.Count - 1
                objects(i) = value.Item(i).ToQueryString().AsEncodedQueryString()
            Next
            ViewState("OriginalDocuments") = objects
        End Set
    End Property

    ''' <summary> Documenti firmati </summary>
    Public Property SignedDocuments As List(Of MultiSignDocumentInfo)
        Get
            If _signedDocuments.Count <> 0 Then
                Return _signedDocuments
            End If
            If ViewState("SignedDocuments") IsNot Nothing Then
                ' serializzo a manina per mancanza di tempo
                Dim objects() As String = DirectCast(ViewState("SignedDocuments"), String())
                For Each serial As String In objects
                    _signedDocuments.Add(New MultiSignDocumentInfo(HttpUtility.ParseQueryString(serial)))
                Next
            End If
            Return _signedDocuments
        End Get
        Set(value As List(Of MultiSignDocumentInfo))
            _signedDocuments = value
            Dim objects(value.Count - 1) As String
            For i As Integer = 0 To value.Count - 1
                objects(i) = value.Item(i).ToQueryString().AsEncodedQueryString()
            Next
            ViewState("SignedDocuments") = objects
        End Set
    End Property

    Protected Property PostBackUrl As String
        Get
            Return ViewState("PostBackUrl").ToString()
        End Get
        Set(value As String)
            ViewState("PostBackUrl") = value
        End Set
    End Property
    Protected Property SignAction As String
        Get
            Return ViewState("SignAction").ToString()
        End Get
        Set(value As String)
            ViewState("SignAction") = value
        End Set
    End Property
    Protected Property SignedComplete As Boolean
        Get
            Dim sessionParam As Object = Session("signedComplete")
            If sessionParam IsNot Nothing Then
                Return DirectCast(sessionParam, Boolean)
            End If
            Return False
        End Get
        Set(value As Boolean)
            If value = Nothing Then
                Session.Remove("signedComplete")
            Else
                Session("signedComplete") = value
            End If
        End Set
    End Property

    Protected ReadOnly Property SaveResponseToSession As Boolean
        Get
            Dim param As String = Request.QueryString("saveResponse")
            If Not String.IsNullOrEmpty(param) Then
                Return True
            End If
            Return False
        End Get
    End Property
#End Region

#Region " Methods "

    ''' <summary> Torna l'url per il tipo di firma multipla selezionata </summary>
    ''' <remarks> Le pagine chiamanti devono usare ISignMultipleDocuments </remarks>
    Public Shared Function GetMultipleSignUrl() As String
        Select Case DocSuiteContext.Current.ProtocolEnv.MultipleSignType
            Case 0
                Return "~/Comm/MultipleSign.aspx"
            Case 1
                Return "~/Comm/MultipleSingleSign.aspx"
        End Select
        Throw New NotImplementedException("Caso non previsto")
    End Function

#End Region
End Class

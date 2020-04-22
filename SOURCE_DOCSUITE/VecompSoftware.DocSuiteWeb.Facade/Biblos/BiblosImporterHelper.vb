
Imports VecompSoftware.Helpers
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models

Public Class BiblosImporterHelper

#Region "Fields"
#End Region

#Region "Constructore"
    Public Sub New()
    End Sub
#End Region

#Region " Private methods "
    ''' <summary>
    ''' Verifica se il filename passato ha come estensione una di quelle utili per la conservazione
    ''' </summary>
    ''' <param name="filename">Nome file</param>
    ''' <param name="fileExtension">Estensioni supportate</param>
    ''' <returns>True se il file ha come estensione una di quelle utili per la conservazione, False altrimenti</returns>
    ''' <remarks></remarks>
    Private Function CheckFileExtension(ByVal filename As String, ByVal ParamArray fileExtension() As String) As Boolean
        If fileExtension Is Nothing OrElse fileExtension.Length = 0 Then
            Return True
        End If

        For Each ext As String In fileExtension
            If FileHelper.MatchExtension(filename, ext) Then
                Return True
            End If
        Next

        Return False
    End Function

    Private Function CheckAttribute(doc As BiblosDocumentInfo, attribute As String) As Boolean
        Dim temp As String = doc.GetAttributeValue(attribute).ToString()
        Return Not String.IsNullOrEmpty(temp)
    End Function
#End Region



#Region "Import/Export Documents"
    ''' <summary>
    ''' Restituisce la lista di documenti con l'estensione indicata
    ''' </summary>
    ''' <param name="server">Nome Server Biblos</param>
    ''' <param name="archive">Nome Archivio Biblos</param>
    ''' <param name="idChain">Id Documento</param>
    ''' <param name="fileExtension">Estensioni valide</param>
    ''' <returns>Lista di oggetti conteneti chainObject e documento espresso come array di byte</returns>
    ''' <remarks></remarks>
    Public Function GetValidDocuments(ByVal server As String, ByVal archive As String, ByVal idChain As Integer?, ByVal ParamArray fileExtension() As String) As IList(Of BiblosDocumentInfo)
        Dim tor As New List(Of BiblosDocumentInfo)
        Dim docs As List(Of BiblosDocumentInfo) = BiblosDocumentInfo.GetDocuments(server, archive, idChain.Value)
        For Each doc As BiblosDocumentInfo In docs
            If (CheckAttribute(doc, "Filename") AndAlso _
                        CheckAttribute(doc, "Signature") AndAlso _
                        Not CheckAttribute(doc, "ConservatedDocumentLink") AndAlso _
                        CheckFileExtension(doc.Name, fileExtension)) Then
                tor.Add(doc)
            End If
        Next

        Return tor
    End Function

#End Region

End Class

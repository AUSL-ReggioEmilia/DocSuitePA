Imports System.Collections.Generic
Imports VecompSoftware.Services.Biblos
Imports VecompSoftware.Services.Biblos.Models

Namespace Viewers
    Public Interface IHaveViewerLight

        ReadOnly Property CheckedDocuments() As List(Of DocumentInfo)
        ReadOnly Property SelectedDocument() As DocumentInfo

    End Interface
End Namespace
Imports System.Runtime.CompilerServices
Imports VecompSoftware.DocSuiteWeb.API
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.Helpers.ExtensionMethods

Public Module ProtocolDTOEx

    <Extension()>
    Public Function CopyFrom(source As ProtocolDTO, protocol As Protocol) As ProtocolDTO
        source.UniqueId = protocol.Id
        source.Year = protocol.Year
        source.Number = protocol.Number

        If protocol.IdDocument.HasValue Then
            source.AddBiblosDocument(protocol.Location.ProtBiblosDSDB, protocol.IdDocument.Value)
        End If

        If source.IdProtocolKind.HasValue AndAlso ProtocolKind.FatturePA = CType(source.IdProtocolKind.Value, ProtocolKind) Then
            Return source
        End If

        If protocol.IdAttachments.HasValue Then
            Dim attachmentLocation As Location = protocol.Location
            If DocSuiteContext.Current.ProtocolEnv.IsProtocolAttachLocationEnabled AndAlso protocol.AttachLocation IsNot Nothing Then
                attachmentLocation = protocol.AttachLocation
            End If
            source.AddBiblosAttachment(attachmentLocation.ProtBiblosDSDB, protocol.IdAttachments.Value)
        End If

        source.AddBiblosAnnexed(protocol.IdAnnexed)
        Return source
    End Function

End Module
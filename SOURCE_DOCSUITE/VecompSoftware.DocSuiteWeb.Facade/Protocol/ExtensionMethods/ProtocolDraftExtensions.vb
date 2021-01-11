Imports System.Runtime.CompilerServices
Imports VecompSoftware.DocSuiteWeb.Data

''' <summary>
''' Carica i dati da protocollo a xml
''' </summary>
''' <remarks></remarks>
Public Module ProtocolDraftExtensions
    <Extension()>
    Public Sub Load(ByVal xmlData As ResolutionXML, ByVal prot As Protocol)
        If xmlData.UpdateProposerProtocolLink Then
            xmlData.ProposerProtocolLink = ProtocolFacade.GetCalculatedLink(prot)
        End If
        If xmlData.UpdateCollegioSindacaleProtocolLink Then
            xmlData.CollegioSindacaleProtocolLink = ProtocolFacade.GetCalculatedLink(prot)
            xmlData.CollegioSindacaleDate = prot.RegistrationDate.DateTime
        End If
        If xmlData.UpdateInvioPubbLetteraFirmaDigitaleProtocolLink Then
            xmlData.InvioPubbLetteraFirmaDigitaleProtocolLink = ProtocolFacade.GetCalculatedLink(prot)
            xmlData.InvioPubbLetteraFirmaDigitaleDate = prot.RegistrationDate.DateTime
        End If
    End Sub
End Module
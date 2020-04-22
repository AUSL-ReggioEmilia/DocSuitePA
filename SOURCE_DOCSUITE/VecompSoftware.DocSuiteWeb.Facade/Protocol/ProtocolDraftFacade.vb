Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Xml
Imports System.Xml.Linq
Imports System.Linq
Imports VecompSoftware.Services.Logging

Public Class ProtocolDraftFacade
    Inherits BaseProtocolFacade(Of ProtocolDraft, Integer, NHibernateProtocolDraftDao)

    Public Function AddProtocolDraft(collaboration As Collaboration, description As String, data As String) As ProtocolDraft
        Dim protocolXml As CollaborationXmlData
        Try
            protocolXml = SerializationHelper.SerializeFromString(Of CollaborationXmlData)(data)
        Catch ex As Exception
            Throw New ArgumentException("Errore in lettura XML.", "data", ex)
        End Try
        Return AddProtocolDraft(collaboration, description, protocolXml)
    End Function

    Public Function AddProtocolDraft(collaboration As Collaboration, description As String, xmlCollaborationData As CollaborationXmlData) As ProtocolDraft
        Dim data As String = SerializationHelper.SerializeToStringWithoutNamespace(xmlCollaborationData)
        Dim draftType As Integer = -1
        Select Case xmlCollaborationData.GetType()
            Case GetType(ProtocolXML)
                draftType = 0
            Case GetType(ResolutionXML)
                draftType = 1
        End Select
        Dim draft As New ProtocolDraft() With {.Collaboration = collaboration, .Description = description, .Data = data, .DraftType = draftType, .IsActive = 1, .RegistrationDate = DateTime.Now, .LastChangedUser = "", .RegistrationUser = "", .LastChangedDate = DateTimeOffset.UtcNow}
        _dao.Save(draft)
        Return draft
    End Function

    Public Function GetDataFromCollaboration(collaboration As Collaboration) As CollaborationXmlData
        Dim protocolDraft As ProtocolDraft = _dao.GetByCollaboration(collaboration)
        If protocolDraft IsNot Nothing Then
            Select Case protocolDraft.DraftType
                Case 0
                    Return SerializationHelper.SerializeFromString(Of ProtocolXML)(protocolDraft.Data)
                Case 1
                    Return SerializationHelper.SerializeFromString(Of ResolutionXML)(protocolDraft.Data)
            End Select
        End If
        Return Nothing
    End Function

    Public Sub DeleteFromCollaboration(collaboration As Collaboration)
        Dim protocolDraft As ProtocolDraft = _dao.GetByCollaboration(collaboration)
        If protocolDraft IsNot Nothing Then
            FileLogger.Info(LoggerName, String.Format("Delete ProtocolDraft -> id: {0}", protocolDraft.Id))
            Delete(protocolDraft)
        End If
    End Sub

    ''' <summary>
    ''' Estraggo l'informazione del contatto dal file XML di Uoia.
    ''' La descrizione finirà nella sezione "Spettabile" del report Uoia.
    ''' Il file XML esempio è come segue:
    ''' <code>
    ''' <ContactBag sourceType="0">
    '''     <Contact type = "A" >
    '''         <Description>Legale rappresentante di ZEN SISTEMI S.R.L.</Description>
    '''         <StandardMail> afantini@zensistemi.com</StandardMail>
	'''			<CertifiedMail> zensistemiDitta@gmail.com</CertifiedMail>
	'''			<Address name="VIA I. PIZZETTI 2" cap="42124" city="REGGIO NELL'EMILIA" prov="RE"/>
	'''			<Fax/>
	'''		</Contact>
	'''	</ContactBag>
    ''' </code>
    ''' 
    ''' </summary>
    ''' <param name="collaboration"></param>
    ''' <returns></returns>
    Public Function GetContactDescriptionFromCollaborationUoia(collaboration As Collaboration) As String
        Dim protocolDraft As ProtocolDraft = _dao.GetByCollaboration(collaboration)
        Const contactString As String = "Contact"
        Const descriptionString As String = "Description"

        If protocolDraft IsNot Nothing Then
            Select Case protocolDraft.DraftType
                Case 0 'Protocollo
                    Dim document As XDocument = XDocument.Parse(protocolDraft.Data)
                    Dim contacts As IEnumerable(Of XElement) = document.Descendants(contactString)
                    If contacts IsNot Nothing AndAlso contacts.Any() Then
                        Dim contact As XElement = contacts.FirstOrDefault()
                        If contact IsNot Nothing AndAlso contact.Elements(descriptionString).Any() Then
                            Return contact.Element(descriptionString).Value
                        End If
                    End If
            End Select
        End If

        Return String.Empty
    End Function

End Class

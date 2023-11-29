Imports VecompSoftware.Helpers
Imports VecompSoftware.DocSuiteWeb.Data
Imports System.Xml
Imports System.Xml.Linq
Imports System.Linq
Imports VecompSoftware.Services.Logging

Public Class CollaborationDraftFacade
    Inherits BaseProtocolFacade(Of CollaborationDraft, Guid, NHibernateCollaborationDraftDao)

    Public Function AddCollaborationDraft(collaboration As Collaboration, description As String, data As String) As CollaborationDraft
        Dim collaborationXml As CollaborationXmlData
        Try
            collaborationXml = SerializationHelper.SerializeFromString(Of CollaborationXmlData)(data)
        Catch ex As Exception
            Throw New ArgumentException("Errore in lettura XML.", "data", ex)
        End Try
        Return AddCollaborationDraft(collaboration, description, collaborationXml)
    End Function

    Public Function AddCollaborationDraft(collaboration As Collaboration, description As String, xmlCollaborationData As CollaborationXmlData, Optional idDocumentUnit As Guid? = Nothing) As CollaborationDraft
        Dim data As String = SerializationHelper.SerializeToStringWithoutNamespace(xmlCollaborationData)
        Dim draftType As Integer = -1
        Select Case xmlCollaborationData.GetType()
            Case GetType(ProtocolXML)
                draftType = 0
            Case GetType(ResolutionXML)
                draftType = 1
        End Select
        Dim draft As CollaborationDraft = New CollaborationDraft() With {
            .collaboration = collaboration,
            .description = description,
            .data = data,
            .draftType = draftType,
            .IsActive = 1,
            .RegistrationDate = DateTime.Now,
            .LastChangedUser = "",
            .RegistrationUser = "",
            .LastChangedDate = DateTimeOffset.UtcNow,
            .idDocumentUnit = idDocumentUnit
        }
        _dao.Save(draft)
        Return draft
    End Function

    Public Function GetDataFromCollaboration(collaboration As Collaboration) As CollaborationXmlData
        Dim collaborationDraft As CollaborationDraft = _dao.GetByCollaboration(collaboration)
        If collaborationDraft IsNot Nothing Then
            Select Case collaborationDraft.DraftType
                Case 0
                    Return SerializationHelper.SerializeFromString(Of ProtocolXML)(collaborationDraft.Data)
                Case 1
                    Return SerializationHelper.SerializeFromString(Of ResolutionXML)(collaborationDraft.Data)
            End Select
        End If
        Return Nothing
    End Function

    Public Sub DeleteFromCollaboration(collaboration As Collaboration)
        Dim collaborationDraft As CollaborationDraft = _dao.GetByCollaboration(collaboration)
        If collaborationDraft IsNot Nothing Then
            FileLogger.Info(LoggerName, String.Format("Delete CollaborationDraft -> id: {0}", collaborationDraft.Id))
            Delete(collaborationDraft)
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
        Dim collaborationDraft As CollaborationDraft = _dao.GetByCollaboration(collaboration)
        Const contactString As String = "Contact"
        Const descriptionString As String = "Description"

        If collaborationDraft IsNot Nothing Then
            Select Case collaborationDraft.DraftType
                Case 0 'Protocollo
                    Dim document As XDocument = XDocument.Parse(collaborationDraft.Data)
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

    Public Function ExistResolutionLink(resolutionUniqueId As Guid) As Boolean
        Return _dao.GetByIdDocumentUnit(resolutionUniqueId) IsNot Nothing
    End Function

    Public Function GetByIdDocumentUnit(idDocumentUnit As Guid) As CollaborationDraft
        Return _dao.GetByIdDocumentUnit(idDocumentUnit)
    End Function

    Public Function GetFromCollaboration(collaboration As Collaboration) As CollaborationDraft
        Return _dao.GetByCollaboration(collaboration)
    End Function

End Class

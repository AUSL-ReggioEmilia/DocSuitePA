Imports System.Xml.Serialization

Namespace OrganizationChart.xml
    ''' <summary> classe per il parsing XML dell'oggetto ProtocolXmlData. </summary>
    <XmlRootAttribute("ProtocolXmlData")>
    Public Class ProtocolXmlData

        <XmlAttribute("Year")>
        Public Property Year() As Integer

        <XmlAttribute("Number")>
        Public Property Number() As Integer

        <XmlElement("Date")>
        Public Property ProtocolDate() As Date?

        <XmlElement("ProtocolType")>
        Public Property ProtocolType() As ProtocolTypeXml

        <XmlElement("DocumentType")>
        Public Property DocumentType() As DocumentTypeXml

        <XmlArray("Senders"), XmlArrayItem("FullContact")>
        Public Property Senders() As List(Of FullContact)

        <XmlArray("Recipients"), XmlArrayItem("FullContact")>
        Public Property Recipients() As List(Of FullContact)

        <XmlElement("ProtocolObject")>
        Public Property ProtocolObject() As String

        <XmlElement("Category")>
        Public Property Category() As FullCategoryXml

        <XmlElement("Notes")>
        Public Property Notes() As String

        <XmlArray("MainDocuments"), XmlArrayItem("DocumentXml")>
        Public Property MainDocuments() As List(Of DocumentXml)

        <XmlArray("Attachments"), XmlArrayItem("DocumentXml")>
        Public Property Attachments() As List(Of DocumentXml)

        <XmlArray("Annexes"), XmlArrayItem("DocumentXml")>
        Public Property Annexes() As List(Of DocumentXml)

        <XmlArray("Authorizations"), XmlArrayItem("FullAuthorizationRole")>
        Public Property Authorizations() As List(Of FullAuthorizationRole)
    End Class
End Namespace
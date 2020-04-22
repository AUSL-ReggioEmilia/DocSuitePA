Imports System.Xml.Serialization

Namespace OrganizationChart.xml
    ''' <summary> classe per il parsing XML dell'oggetto ProtocolXmlData. </summary>
    <XmlRoot("FullContact")>
    Public Class FullContact

        <XmlAttribute("FullIncrementalPath")>
        Public Property FullIncrementalPath() As Int32

        <XmlArray("ContactList"), XmlArrayItem("ContactXml")>
        Public Property ContactList() As List(Of ContactXML)
    End Class
End Namespace
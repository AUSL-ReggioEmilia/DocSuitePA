Imports System.Xml.Serialization

<XmlRoot("WorkflowMetadata")>
Public Class WorkflowMetadataXml

    <XmlElement("Key")>
    Public Property Key As String

    <XmlElement("Value")>
    Public Property Value As String
End Class

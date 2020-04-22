Imports System.Xml.Serialization

Namespace OrganizationChart.xml
    <XmlRoot("ProtocolTypeXml")>
    Public Class ProtocolTypeXml

        <XmlAttribute("IdType")>
        Public Property IdType As Int32

        <XmlAttribute("Description")>
        Public Property Description As String

        <XmlAttribute("ShortDescription")>
        Public Property ShortDescription As String
    End Class
End Namespace
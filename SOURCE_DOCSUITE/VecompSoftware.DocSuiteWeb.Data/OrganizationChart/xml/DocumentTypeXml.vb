Imports System.Xml.Serialization

Namespace OrganizationChart.xml
    <XmlRoot("DocumentTypeXml")>
    Public Class DocumentTypeXml

        <XmlAttribute("Code")>
        Public Property Code As String

        <XmlAttribute("Description")>
        Public Property Description As String
    End Class
End Namespace
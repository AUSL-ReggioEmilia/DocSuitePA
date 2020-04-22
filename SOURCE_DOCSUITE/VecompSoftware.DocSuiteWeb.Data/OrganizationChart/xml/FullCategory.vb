Imports System.Xml.Serialization

Namespace OrganizationChart.xml
    <XmlRoot("FullCategoryXml")>
    Public Class FullCategoryXml

        <XmlAttribute("FullCode")>
        Public Property FullCode() As String

        <XmlArray("CategoryXmlList"), XmlArrayItem("CategoryXml")>
        Public Property CategoryXmlList() As List(Of CategoryXml)
    End Class

    <XmlRoot("CategoryXml")>
    Public Class CategoryXml

        <XmlAttribute("Code")>
        Public Property Code As Integer

        <XmlAttribute("Name")>
        Public Property Name As String

        <XmlAttribute("FullCode")>
        Public Property FullCode As String
    End Class
End Namespace
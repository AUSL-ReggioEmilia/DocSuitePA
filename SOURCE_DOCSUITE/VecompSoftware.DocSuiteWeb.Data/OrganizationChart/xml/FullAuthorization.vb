Imports System.Xml.Serialization

Namespace OrganizationChart.xml
    <XmlRoot("FullAuthorizationRole")>
    Public Class FullAuthorizationRole

        <XmlAttribute("FullIncrementalPath")>
        Public Property FullIncrementalPath() As String

        <XmlArray("RoleList"), XmlArrayItem("RoleXml")>
        Public Property RoleList() As List(Of RoleXml)
    End Class

    <XmlRoot("RoleXml")>
    Public Class RoleXml

        <XmlAttribute("IdRole")>
        Public Property IdRole As Int32

        <XmlAttribute("Name")>
        Public Property Name As String

        <XmlAttribute("IsActive")>
        Public Property IsActive As Short

        <XmlAttribute("ServiceCode")>
        Public Property ServiceCode As String
    End Class
End Namespace
Imports System.Xml.Serialization
Imports VecompSoftware.DocSuiteWeb.Data.OrganizationChart.interfaces

Namespace OrganizationChart.xml
    <XmlRoot("OChartProtocolXml")>
    Public Class OChartProtocolXml
        Inherits OChartCommunicationDataBase

        <XmlAttribute("OChartItemFullCode")>
        Public OChartItemFullCode As String

        <XmlElement("ProtocolXmlData")>
        Public ProtocolXmlData As ProtocolXmlData
    End Class

    <XmlRoot("DocumentXml")>
    Public Class DocumentXml

        <XmlElement("DocumentId")>
        Public DocumentId As String

        <XmlElement("Caption")>
        Public Caption As String
    End Class
End Namespace
Imports System.Xml.Serialization

Namespace OrganizationChart.interfaces
    Public MustInherit Class OChartCommunicationDataBase
        Implements IOChartCommunicationData

        <XmlIgnore>
        Public Property OChartItem() As OChartItem
    End Class
End Namespace


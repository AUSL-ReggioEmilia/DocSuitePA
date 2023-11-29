Imports System.Collections.Generic
Imports System.Xml
Imports VecompSoftware.DocSuiteWeb.Data
Imports VecompSoftware.DocSuiteWeb.Facade
Imports VecompSoftware.DocSuiteWeb.UDSDesigner

Public Class DownloadXml
    Inherits CommonBasePage

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not CommonShared.HasGroupAdministratorRight Then
            Throw New DocSuiteException("Sono necessari diritti amministrativi per vedere la pagina.")
        End If

        Response.AddHeader("Content-Disposition", "attachment; filename=uds.xml")
        Response.ContentType = "application/xml"
        Response.Clear()

        Dim jsModel As JsModel = DirectCast(Me.Session("tempModel"), JsModel)
        If jsModel IsNot Nothing Then
            Dim model As UDSConverter = New UDSConverter()
            Dim errors As List(Of String)
            Dim xml As String = model.JsToXml(jsModel, errors)

            Dim xDoc As XmlDocument = New XmlDocument()
            xDoc.LoadXml(xml)

            xDoc.Save(Response.OutputStream)
            Response.Flush()
        End If
    End Sub

End Class
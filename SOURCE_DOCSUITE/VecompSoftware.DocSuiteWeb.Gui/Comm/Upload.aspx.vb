Public Partial Class Upload
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Me.Request.InputStream.Length > 0 Then
            Dim reader As New System.IO.StreamReader(Request.InputStream)
            Dim encodedString As String = reader.ReadToEnd()
            Dim decoded As String = System.Web.HttpUtility.UrlDecode(encodedString)
        End If
        Session("User_ID") = 1 'Demo
        Dim Upload As Object = Server.CreateObject("LEAD.Upload")
        'Dim LeadRaster As Object = Server.CreateObject("LEADRaster.LEADRaster.130")
        'Dim LeadRasterIO As Object = Server.CreateObject("LEADRasterIO.LEADRasterIO.130")

        'Dim files As Object = Upload.Upload

        'Dim rp As String = "C:\Prodotti\DocSuite\DocSuite2008\Src\DocSuite.Web\Temp\"

        'For Each file As Object In files
        '    Dim result As Object = LeadRasterIO.LoadArray(LeadRaster, file.Data, 0, 1, 1, file.size)
        '    If result = 0 Then
        '        'ImageName= ImageName&".jpg"
        '        'LeadRasterIO.Save LeadRaster, rp & ImageName, 10, 24, 75, 0
        '        Dim ImageName As Object = Upload.Form("idfile")
        '        LeadRasterIO.Save(LeadRaster, rp & ImageName, 29, 1, 75, 1)
        '    End If
        'Next

    End Sub

End Class
<%@ Page Language="vb" AutoEventWireup="false" Theme=""%>
<%@ Import Namespace="VecompSoftware.DocSuiteWeb.Facade" %>
<%
        Dim files As HttpFileCollection = HttpContext.Current.Request.Files
        Dim uploadfile As HttpPostedFile = files("RemoteFile")

        Dim filePath As String = CommonUtil.GetInstance().AppTempPath & uploadfile.FileName

        uploadfile.SaveAs(filePath)
%>
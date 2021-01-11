<%@ Language="vb" AutoEventWireup="false" CodeBehind="PrivacyLevels.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.PrivacyLevels" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscDocumentUpload.ascx" TagPrefix="usc" TagName="Documents" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="radScriptBlock">
        <script language="javascript" type="text/javascript">
            function CloseWindow(argument) {
                var oWindow = GetRadWindow();
                oWindow.close(argument);
            }

        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <asp:Panel runat="server" ID="pageContent">

        <div runat="server" id="mainDiv">
            <usc:Documents runat="server" ID="uscDocument" PrivacyLevelVisible="true" ButtonFileEnabled="false" ButtonRemoveEnabled="false" DocumentDeletable="false" MultipleDocuments="true" IsDocumentRequired="false" ButtonSharedFolederEnabled="false" SignButtonEnabled="false" ButtonScannerEnabled="false" ButtonLibrarySharepointEnabled="false" ModifiyPrivacyLevelEnabled="true" />
        </div>
    </asp:Panel>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <telerik:RadButton runat="server" ID="btnConfirm" Text="Conferma"></telerik:RadButton>
</asp:Content>

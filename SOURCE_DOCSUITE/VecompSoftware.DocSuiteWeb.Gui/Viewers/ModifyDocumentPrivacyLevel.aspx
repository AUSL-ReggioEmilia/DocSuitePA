<%@ Language="vb" AutoEventWireup="false" CodeBehind="ModifyDocumentPrivacyLevel.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ModifyDocumentPrivacyLevel" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

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
        <div runat="server" id="mainDocumentDiv" visible="false">
            <usc:Documents runat="server" ID="uscDocument" ButtonFileEnabled="false" ButtonRemoveEnabled="false" DocumentDeletable="false" MultipleDocuments="true" IsDocumentRequired="false" Caption="Documento Principale" ButtonSharedFolederEnabled="false" SignButtonEnabled="false" ButtonScannerEnabled="false" ButtonLibrarySharepointEnabled="false" PrivacyLevelVisible="true" />
        </div>
        <div runat="server" id="adoptedDocumentDiv" visible="false">
            <usc:Documents runat="server" ID="uscAdopted" ButtonFileEnabled="false" ButtonRemoveEnabled="false" DocumentDeletable="false" MultipleDocuments="true" IsDocumentRequired="false" Caption="Doc. Adottato" ButtonSharedFolederEnabled="false" SignButtonEnabled="false" ButtonScannerEnabled="false" ButtonLibrarySharepointEnabled="false" PrivacyLevelVisible="true" />
        </div>
        <div runat="server" id="attachmentsDiv" visible="false">
            <usc:Documents runat="server" ID="uscAttachments" ButtonFileEnabled="false" ButtonRemoveEnabled="false" DocumentDeletable="false" MultipleDocuments="true" IsDocumentRequired="false" Caption="Allegati" ButtonSharedFolederEnabled="false" SignButtonEnabled="false" ButtonScannerEnabled="false" ButtonLibrarySharepointEnabled="false" PrivacyLevelVisible="true" />
        </div>
        <div runat="server" id="annexedDiv" visible="false">
            <usc:Documents runat="server" ID="uscAnnexed" ButtonFileEnabled="false" ButtonRemoveEnabled="false" DocumentDeletable="false" MultipleDocuments="true" IsDocumentRequired="false" Caption="Annessi" ButtonSharedFolederEnabled="false" SignButtonEnabled="false" ButtonScannerEnabled="false" ButtonLibrarySharepointEnabled="false" PrivacyLevelVisible="true" />
        </div>
        <div runat="server" id="unpublishedAnnexedDiv" visible="false">
            <usc:Documents runat="server" ID="uscUnpublishedAnnexed" ButtonFileEnabled="false" ButtonRemoveEnabled="false" DocumentDeletable="false" MultipleDocuments="true" IsDocumentRequired="false" Caption="Annessi non pubblicati" ButtonSharedFolederEnabled="false" SignButtonEnabled="false" ButtonScannerEnabled="false" ButtonLibrarySharepointEnabled="false" PrivacyLevelVisible="true" />
        </div>
        <div runat="server" id="proposedDocumentDiv" visible="false">
            <usc:Documents runat="server" ID="uscProposed" ButtonFileEnabled="false" ButtonRemoveEnabled="false" DocumentDeletable="false" MultipleDocuments="true" IsDocumentRequired="false" Caption="Doc. Proposto" ButtonSharedFolederEnabled="false" SignButtonEnabled="false" ButtonScannerEnabled="false" ButtonLibrarySharepointEnabled="false" PrivacyLevelVisible="true" />
        </div>
        <div runat="server" id="supervisoryBoardDiv" visible="false">
            <usc:Documents runat="server" ID="uscSupervisoryBoard" ButtonFileEnabled="false" ButtonRemoveEnabled="false" DocumentDeletable="false" MultipleDocuments="true" IsDocumentRequired="false" Caption="Collegio ograno sindacale" ButtonSharedFolederEnabled="false" SignButtonEnabled="false" ButtonScannerEnabled="false" ButtonLibrarySharepointEnabled="false" PrivacyLevelVisible="true" />
        </div>
    </asp:Panel>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <telerik:RadButton runat="server" ID="btnConfirm" Text="Conferma"></telerik:RadButton>
</asp:Content>

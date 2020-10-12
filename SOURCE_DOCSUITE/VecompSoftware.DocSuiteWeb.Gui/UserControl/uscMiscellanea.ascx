<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscMiscellanea.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscMiscellanea" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server">
    <script type="text/javascript">
        var uscMiscellanea;
        require(["UserControl/uscMiscellanea"], function (UscMiscellanea) {
            $(function () {
                uscMiscellanea = new UscMiscellanea();
                uscMiscellanea.pageId = "<%= pageContent.ClientID%>";
                uscMiscellanea.ajaxManagerId = "<%= AjaxManager.ClientID%>";
                uscMiscellanea.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                uscMiscellanea.ajaxLoadingPanelId = "<%= BasePage.MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";                
                uscMiscellanea.managerWindowsId = "<%= manager.ClientID %>";
                uscMiscellanea.managerUploadDocumentId = "<%= managerUploadDocument.ClientID %>";
                uscMiscellanea.managerId = "<%= BasePage.MasterDocSuite.DefaultWindowManager.ClientID %>";
                uscMiscellanea.miscellaneaGridId = "<%= miscellaneaGrid.ClientID %>"; 
                uscMiscellanea.type = "<%= Type %>"; 
                uscMiscellanea.initialize();
            });
        });

        function onRequestStart(sender, args) {
            args.set_enableAjax(false);
        }

        function OnGridDataBound(sender, args) {
            uscMiscellanea.onGridDataBound();
        }

        function OnRowDataBound(sender, args) {
            uscMiscellanea.onRowDataBound(sender, args);
        }

        function getDocumentExtension(name) {
            return uscMiscellanea.getDocumentExtension(name);
        }

        function openPreviewWindow(serializedDoc) {
            uscMiscellanea.openPreviewWindow(serializedDoc);
        }

        function openEditWindow(idDocument, idArchiveChain, locationId) {
            uscMiscellanea.openEditWindow(idDocument, idArchiveChain, locationId);
        }

        function openDeleteWindow(idDocument, idArchiveChain) {
            uscMiscellanea.openDeleteWindow(idDocument, idArchiveChain);
        }

        function initializeSign(idDocument) {
            uscMiscellanea.initializeSign(idDocument);
        }

        function closeSignWindow(sender, args) {
            uscMiscellanea.closeSignWindow(sender, args);            
        }

    </script>
</telerik:RadScriptBlock>
<telerik:RadWindowManager EnableViewState="False" ID="manager" runat="server">
    <Windows>
        <telerik:RadWindow Height="600" ID="managerUploadDocument" runat="server" Title="Gestione inserti" Width="750" />
        <telerik:RadWindow Behaviors="Maximize,Close,Resize,Reload" DestroyOnClose="True" ID="windowPreviewDocument" ReloadOnShow="false" runat="server" Title="Gestione inserti" />
        <telerik:RadWindow Behaviors="Maximize,Close,Resize" Height="500px" ID="signWindow" OnClientClose="closeSignWindow" ReloadOnShow="true" runat="server" Title="Firma documento" Width="600px" />
    </Windows>
</telerik:RadWindowManager>

<asp:Panel runat="server" ID="pageContent" Width="100%" Height="100%">
    <div class="radGridWrapper">
        <telerik:RadGrid runat="server" ID="miscellaneaGrid" GridLines="None" AllowPaging="False" Height="100%" AllowMultiRowSelection="False" AllowFilteringByColumn="False">
            <ClientSettings>
                <Resizing AllowColumnResize="false" />
                <ClientEvents OnDataBound="OnGridDataBound"/>
            </ClientSettings>
            <MasterTableView CommandItemDisplay="None" AutoGenerateColumns="False" Width="100%" TableLayout="fixed" DataKeyNames="UniqueId" NoMasterRecordsText="Nessun documento selezionato.">
                <Columns>
                    <telerik:GridTemplateColumn HeaderText="Nome" AllowFiltering="false" UniqueName="Name" DataField="Name" HeaderStyle-Width="45%" ItemStyle-VerticalAlign="Middle">
                        <ClientItemTemplate>
                            <img src="#=getDocumentExtension(Name)#" title="Visualizza anteprima" alt="Anteprima documento" onclick="openPreviewWindow('#=Serialized#')"></img>
                            <label>#=Name#</label>                        
                        </ClientItemTemplate>
                    </telerik:GridTemplateColumn>
                    <%--<telerik:GridBoundColumn DataField="Version" UniqueName="Version" HeaderText="Versione" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="130px" />--%>
                    <telerik:GridBoundColumn DataField="Note" UniqueName="Note" HeaderText="Note" HeaderStyle-Width="25%" />
                    <telerik:GridTemplateColumn DataField="RegistrationDate" UniqueName="RegistrationDate" HeaderText="Data" HeaderStyle-Width="10%">
                        <ClientItemTemplate>
                            <label>#=moment(RegistrationDate).format("DD/MM/YYYY")#</label>
                        </ClientItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridBoundColumn DataField="RegistrationUser" UniqueName="RegistrationUser" HeaderText="Utente" HeaderStyle-Width="20%" />
                    <telerik:GridTemplateColumn UniqueName="Actions" AllowFiltering="false" HeaderText="Azioni" HeaderStyle-Width="10%" >
                        <ClientItemTemplate>
                            # if(EditEnabled){ #                          
                                <img src="../App_Themes/DocSuite2008/imgset16/pencil.png" style="cursor:pointer" title="Modifica" alt="Modifica" onclick="openEditWindow('#=IdDocument#', '#=IdChain#', '#=LocationId#')" />
                                <img src="../App_Themes/DocSuite2008/imgset16/delete.png" title="Elimina" alt="Elimina" style="cursor:pointer" onclick="openDeleteWindow('#=IdDocument#', '#=IdChain#')" />
                            # }#                            
                            # if(!IsSigned){ #
                                <img src="../App_Themes/DocSuite2008/imgset16/card_chip_gold.png" title="Firma documento" style="cursor:pointer" onclick="initializeSign('#=IdDocument#')" />
                            # }#                            
                        </ClientItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </telerik:RadGrid>
    </div>

</asp:Panel>

<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
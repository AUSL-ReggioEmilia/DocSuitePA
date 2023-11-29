<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscDossierGrid.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscDossierGrid" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server" EnableViewState="false">
    <script type="text/javascript">
        var uscDossierGrid;
        require(["UserControl/uscDossierGrid"], function (UscDossierGrid) {
            $(function () {
                uscDossierGrid = new UscDossierGrid(tenantModelConfiguration.serviceConfiguration);
                uscDossierGrid.dossierGridId = "<%= dossierGrid.ClientID %>";
                uscDossierGrid.pageId = "<%= pageContent.ClientID %>";
                uscDossierGrid.isWindowPopupEnable = <%= IsWindowPopupEnable.ToString().ToLower() %>;
                uscDossierGrid.initialize();
            });
        });

        function onRequestStart(sender, args) {
            args.set_enableAjax(false);
        }

        function OnGridCommand(sender, args) {
            if (args.get_commandName() == "Page") {
                args.set_cancel(true);
                uscDossierGrid.onPageChanged();
            }
        }

        function OnGridDataBound(sender, args) {
            uscDossierGrid.onGridDataBound();
        }

    </script>
</telerik:RadScriptBlock>

<telerik:RadAjaxPanel ClientEvents-OnRequestStart="onRequestStart" runat="server" CssClass="radGridWrapper" ID="pageContent">
    <telerik:RadGrid runat="server" CssClass="dossierGrid" ID="dossierGrid" Skin="Office2010Blue" PageSize="30" GridLines="None" Height="100%" AllowPaging="True" AllowMultiRowSelection="True" AllowFilteringByColumn="False" RenderMode="Lightweight">
        <ClientSettings>
            <Scrolling AllowScroll="true" UseStaticHeaders="true" ScrollHeight="100%" />
            <Resizing AllowColumnResize="false" />
            <ClientEvents OnCommand="OnGridCommand" OnDataBound="OnGridDataBound" />
        </ClientSettings>
        <MasterTableView CommandItemDisplay="None" AutoGenerateColumns="False" TableLayout="fixed" DataKeyNames="UniqueId" NoMasterRecordsText="Nessun dossier trovato." PagerStyle-PagerTextFormat="{4} Visualizzati {3} su {5}">
            <Columns>
                <telerik:GridTemplateColumn HeaderText="Dossier" AllowFiltering="false" UniqueName="Title" HeaderStyle-Width="120px">
                    <ClientItemTemplate>
                         <center>
                             # if(!uscDossierGrid.isWindowPopupEnable){#
                                 <a href="../Dossiers/DossierVisualizza.aspx?Type=Dossier&IdDossier=#= UniqueId #&DossierTitle=#= Title#" class="dossierLink">#= Title #</a>
                            #} else{#
                                 <a href="" onClick="uscDossierGrid.closeResultWindow('#= UniqueId #'); return false;" class="dossierLink">#= Title #</a>
                            #}#
                         </center>
                    </ClientItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn DataField="StartDate" UniqueName="StartDate" HeaderText="Data apertura" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="130px" />
                <telerik:GridBoundColumn DataField="ContainerName" UniqueName="Container" HeaderText="Contenitore" FilterControlWidth="87%" />
                <telerik:GridBoundColumn DataField="MasterRoleName" UniqueName="MasterRole" HeaderText="Settore responsabile" FilterControlWidth="87%" />
                <telerik:GridBoundColumn DataField="Subject" UniqueName="Subject" HeaderText="Oggetto" HeaderStyle-Width="35%" FilterControlWidth="87%" />
            </Columns>
        </MasterTableView>
        <PagerStyle Mode="NextPrevAndNumeric" Position="Bottom" AlwaysVisible="true"></PagerStyle>
    </telerik:RadGrid>
</telerik:RadAjaxPanel>

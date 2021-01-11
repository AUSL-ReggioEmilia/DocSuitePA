<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscReportDesigner.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscReportDesigner" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server">
    <script type="text/javascript">
        var uscReportDesigner;
        require(["UserControl/uscReportDesigner"], function (UscReportDesigner) {
            $(function () {
                uscReportDesigner = new UscReportDesigner();
                uscReportDesigner.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                uscReportDesigner.pnlContentId = "<%= pnlContent.ClientID %>";
                uscReportDesigner.rgvPropertiesId = "<%= rgvProperties.ClientID %>";
                uscReportDesigner.rgvConditionId = "<%= rgvCondition.ClientID %>";
                uscReportDesigner.rgvSortId = "<%= rgvSort.ClientID %>";
                uscReportDesigner.rtsDesignerId = "<%= rtsDesigner.ClientID %>";
                uscReportDesigner.editable = <%= IsEditable.ToString().ToLower() %>;
                uscReportDesigner.initialize();
            });
        });
    </script>
</telerik:RadScriptBlock>

<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
<asp:Panel runat="server" ID="pnlContent">
    <telerik:RadTabStrip ID="rtsDesigner" runat="server" MultiPageID="rmpDesigner"
        SelectedIndex="0">
        <Tabs>
            <telerik:RadTab Text="Proprietà" Value="projectionsview" runat="server" Selected="True" ImageUrl="../App_Themes/DocSuite2008/imgset16/extended_property.png">
            </telerik:RadTab>
            <telerik:RadTab Text="Condizioni" Value="conditionsview" runat="server" ImageUrl="../App_Themes/DocSuite2008/imgset16/conditions_editor.png">
            </telerik:RadTab>
            <%--<telerik:RadTab Text="Ordinamento" runat="server">
            </telerik:RadTab>--%>
        </Tabs>
    </telerik:RadTabStrip>
    <telerik:RadMultiPage runat="server" ID="rmpDesigner" SelectedIndex="0">
        <telerik:RadPageView runat="server">
            <div class="radGridWrapper">
                <telerik:RadGrid runat="server" ID="rgvProperties" Width="100%">
                    <MasterTableView Caption="Trascina una o più proprietà dalla colonna di destra nella colonna Elementi" NoMasterRecordsText="Nessun template caricato">
                        <Columns>
                            <telerik:GridBoundColumn HeaderText="Elementi template" DataField="TagName" UniqueName="tagName"></telerik:GridBoundColumn>
                            <%--<telerik:GridTemplateColumn HeaderText="Titolo da sostituire" DataField="Alias" HeaderStyle-Width="100px" UniqueName="alias">
                                <ClientItemTemplate>
                                    <span class="riSingle RadInput RadInput_Office2007">
                                        <input type="text" class="riTextBox riEnabled"></input>
                                    </span>                                    
                                </ClientItemTemplate>
                            </telerik:GridTemplateColumn>--%>
                            <telerik:GridTemplateColumn HeaderText="Elementi" UniqueName="projection" ItemStyle-CssClass="drop"></telerik:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
            </div>
        </telerik:RadPageView>
        <telerik:RadPageView runat="server">
            <div class="radGridWrapper">
                <telerik:RadGrid runat="server" ID="rgvCondition" Width="100%">
                    <MasterTableView Caption="Trascina dalla colonna di destra al massimo una condizione per ogni riga" NoMasterRecordsText="Nessun template caricato">
                        <Columns>
                            <telerik:GridTemplateColumn HeaderText="Condizioni" UniqueName="condition" ItemStyle-CssClass="drop">
                            </telerik:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
            </div>
        </telerik:RadPageView>
        <telerik:RadPageView runat="server">
            <div class="radGridWrapper">
                <telerik:RadGrid runat="server" ID="rgvSort" Width="100%">
                    <MasterTableView>
                        <Columns>
                            <telerik:GridTemplateColumn HeaderText="Ordinamento" ItemStyle-CssClass="drop" />
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
            </div>
        </telerik:RadPageView>
    </telerik:RadMultiPage>
</asp:Panel>

<div id="controls" style="display: none;">
    <div id="control-template" class="report-element">
        <div style="left: 150px;">
            <div class="wrap" style="width: 200px;">
                <div class="report-element-content">
                    <div><img class="template-icon" /><i class="small-description"></i></div>
                    <div class="template-text"></div>
                    <span>
                        <a href="#" title="Rimuovi" onclick="uscReportDesigner.removeControl(this);">
                            <img class="template-remove-icon" src="../App_Themes/DocSuite2008/imgset16/cross.png" /></a>
                    </span>
                    <i class="long-description"></i>
                </div>
            </div>
        </div>
    </div>
</div>

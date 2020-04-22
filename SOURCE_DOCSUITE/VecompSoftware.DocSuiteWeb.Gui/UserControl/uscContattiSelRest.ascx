<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscContattiSelRest.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscContattiSelRest" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscContactRest.ascx" TagPrefix="uc1" TagName="uscContactREST" %>

        <style type="text/css">
            div.RadGrid_Office2007 input {
                background-color: white;
            }
            .RadWindow .rwIcon{
                margin:3px 5px 0 0;
                position: static;
            }
            .RadWindow .rwIcon::before{
                content: unset;
            }
        </style>

<telerik:RadScriptBlock runat="server">
    <script type="text/javascript">
        var <%= Me.ClientID %>_uscContattiSelRest;
        require(["UserControl/uscContattiSelRest"], function (uscContattiSelRest) {
            $(function () {
                <%= Me.ClientID %>_uscContattiSelRest = new uscContattiSelRest(tenantModelConfiguration.serviceConfiguration);
                <%= Me.ClientID %>_uscContattiSelRest.treeContactId = "<%= treeContact.ClientID%>";
                <%= Me.ClientID %>_uscContattiSelRest.pnlContentId = "<%= pnlContent.ClientID%>";
                <%= Me.ClientID %>_uscContattiSelRest.tbContactsControlId = "<%= tbContactsControl.ClientID%>";
                <%= Me.ClientID %>_uscContattiSelRest.rwContactSelectorId = "<%= rwContactSelector.ClientID %>";
                <%= Me.ClientID %>_uscContattiSelRest.btnContactConfirmId = "<%= btnContactConfirm.ClientID%>";
                <%= Me.ClientID %>_uscContattiSelRest.btnContactConfirmAndNewId = "<%= btnContactConfirmAndNew.ClientID%>";
                <%= Me.ClientID %>_uscContattiSelRest.uscContactRestId = "<%= uscContactRest.PanelContent.ClientID %>";
                <%= Me.ClientID %>_uscContattiSelRest.validatorAnyNodeId = "<%= AnyNodeCheck.ClientID%>";
                <%= Me.ClientID %>_uscContattiSelRest.filterByParentId = "<%= If(FilterByParentId.HasValue, FilterByParentId.Value, "undefined")  %>";
                <%= Me.ClientID %>_uscContattiSelRest.requiredValidationEnabled = "<%= Required %>";

                <%= Me.ClientID %>_uscContattiSelRest.initialize();
            });
        });
    </script>
</telerik:RadScriptBlock>

<asp:Panel runat="server" Height="100%" Width="100%" ID="pnlContent">

    <table class="datatable">
        <tr>
            <td>
                <telerik:RadToolBar runat="server" ID="tbContactsControl" CssClass="ToolBarContainer"
                                    RenderMode="Lightweight" EnableRoundedCorners="False" EnableShadows="False" Width="100%">
                    <Items>
                        <telerik:RadToolBarButton runat="server" CommandName="ADD" ImageUrl="~/App_Themes/DocSuite2008/imgset16/account-circle.png" ToolTip="Aggiungi contatti esistente"/>
                        <telerik:RadToolBarButton runat="server" CommandName="REMOVE" ImageUrl="~/App_Themes/DocSuite2008/imgset16/delete.png" ToolTip="Elimina contatti selezionato"></telerik:RadToolBarButton>
                    </Items>
                </telerik:RadToolBar>
            </td>

        </tr>
        <tr>
            <td>
                <telerik:RadTreeView ID="treeContact" LoadingStatusPosition="BeforeNodeText" PersistLoadOnDemandNodes="false" runat="server" Style="margin-top: 10px;" Width="100%">
                    <Nodes>
                        <telerik:RadTreeNode runat="server" Text="Contatti collegati" Expanded="true" EnableViewState="false" Value="Root" />
                    </Nodes>
                </telerik:RadTreeView>
            </td>
        </tr>
        <tr>
            <td>
            <asp:CustomValidator runat="server" ID="AnyNodeCheck" 
                                 ControlToValidate="treeContact" 
                                 ValidateEmptyText="true"
                                 EnableClientScript="true"
                                 Enabled="false"
                                 ClientValidationFunction="anyNodeCheck" 
                                 Display="Dynamic" ErrorMessage="Campo Responsabile di procedimento Obbligatorio" />
        </td>
        </tr>
    </table>

        <telerik:RadWindow  runat="server" ID="rwContactSelector" Title="Seleziona contatti" Width="650" Height="600">
        <ContentTemplate>
            <asp:UpdatePanel runat="server" ID="UpdatePanelContact" UpdateMode="Conditional">
                <ContentTemplate>
                    <div style="width:100%; height:95%">
                        <uc1:uscContactREST runat="server" ID="uscContactREST"/>
                    </div>
                    <telerik:RadToolBarButton>
                        <ItemTemplate>
                            <telerik:RadButton runat="server" ID="btnContactConfirm" Text="Conferma" Width="150px" AutoPostBack="false"></telerik:RadButton>
                            <telerik:RadButton runat="server" ID="btnContactConfirmAndNew" AutoPostBack="false" Text="Conferma e nuovo" Width="150px"></telerik:RadButton>
                        </ItemTemplate>
                    </telerik:RadToolBarButton>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>

</asp:Panel>
<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
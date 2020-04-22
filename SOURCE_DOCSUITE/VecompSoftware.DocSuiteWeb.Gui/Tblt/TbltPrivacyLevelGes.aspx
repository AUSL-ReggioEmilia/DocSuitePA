<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltPrivacyLevelGes" CodeBehind="TbltPrivacyLevelGes.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script type="text/javascript">
            var tbltPrivacyLevelGes;
            require(["Tblt/TbltPrivacyLevelGes"], function (TbltPrivacyLevelGes) {
                $(function () {
                    tbltPrivacyLevelGes = new TbltPrivacyLevelGes(tenantModelConfiguration.serviceConfiguration);
                    tbltPrivacyLevelGes.btnConfirmId = "<%= btnConfirm.ClientID %>";
                    tbltPrivacyLevelGes.txtDescriptionId = "<%= txtDescription.ClientID %>";                   
                    tbltPrivacyLevelGes.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                    tbltPrivacyLevelGes.action = "<%= Action%>";
                    tbltPrivacyLevelGes.currentPrivacyLevelId = "<%= IdPrivacyLevel%>";
                    tbltPrivacyLevelGes.txtLevelId = "<%= txtLevel.ClientID%>";
                    tbltPrivacyLevelGes.rcpColorId = "<%= rcpColor.ClientID%>";
                    tbltPrivacyLevelGes.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>


    <usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
    <telerik:RadPageLayout runat="server" ID="pageContent" HtmlTag="Div" Width="100%" Height="90%">
        <Rows>
            <telerik:LayoutRow Style="margin-top: 5px;">
                <Columns>
                    <telerik:LayoutColumn Span="3" Height="30px">
                        <b>Descrizione: </b>
                    </telerik:LayoutColumn>
                    <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding t-col-right-padding" Height="30px">
                        <telerik:RadTextBox ID="txtDescription" runat="server" Width="98%" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtDescription" EnableViewState="false" ErrorMessage="Il campo descrizione è obbligatorio" Display="Dynamic" ID="rfvDescription" />
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
            <telerik:LayoutRow Style="margin-top: 5px;" runat="server">
                <Columns>
                    <telerik:LayoutColumn Span="3" Height="30px">
                        <b>Livello: </b>
                    </telerik:LayoutColumn>
                    <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding t-col-right-padding" Height="30px">
                        <telerik:RadTextBox ID="txtLevel" runat="server" Width="98%" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="txtLevel" EnableViewState="false" ErrorMessage="Il campo livello è obbligatorio" Display="Dynamic" ID="rfvName" />
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
             <telerik:LayoutRow Style="margin-top: 5px;" runat="server">
                <Columns>
                    <telerik:LayoutColumn Span="3" Height="30px">
                        <b>Colore: </b>
                    </telerik:LayoutColumn>
                    <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding t-col-right-padding" Height="30px">
                        <telerik:RadColorPicker RenderMode="Lightweight" ID="rcpColor" runat="server" PaletteModes="All" ShowIcon="true" Preset="default">
                        <Localization ApplyButtonText="Applica" />
                            </telerik:RadColorPicker>
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>    
        </Rows>
    </telerik:RadPageLayout>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <telerik:RadButton ID="btnConfirm" runat="server" AutoPostBack="false" Text="Conferma"></telerik:RadButton>
</asp:Content>


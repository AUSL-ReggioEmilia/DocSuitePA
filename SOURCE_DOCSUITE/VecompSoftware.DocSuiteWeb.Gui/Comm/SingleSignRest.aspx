<%@ Page Title="Firma documento" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="SingleSignRest.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.SingleSignRest" %>

<%@ Register Src="~/Viewers/ViewerLight.ascx" TagPrefix="uc1" TagName="ViewerLight" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadCodeBlock runat="server" ID="RadScriptBlock1">
         <script type="text/javascript">
             var singleSign;
             require(["Comm/SingleSignRest"], function (SingleSignRest) {
                 $(function () {
                     singleSign = new SingleSignRest(tenantModelConfiguration.serviceConfiguration);

                     singleSign.typeOfSign = "<%=TypeOfSign%>";
                     singleSign.ToolBarId = "<%=ToolBar.ClientID %>";
                     singleSign.storageType = "<%=StorageInformationType%>";
                     singleSign.dswSignalR = "<%=SignalRAddress%>";
                     singleSign.currentUserDomain = "<%=CurrentUserDomain%>";
                     singleSign.currentUserTenantName = ("<%= CurrentUserTenantName %>");
                     singleSign.currentUserTenantId = ("<%= CurrentUserTenantId %>");
                     singleSign.currentUserTenantAOOId = ("<%= CurrentUserTenantAOOId %>");

                     singleSign.initialize();
                 });
             });
         </script>
    </telerik:RadCodeBlock>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <asp:Panel ID="signContainer" runat="server">
        <telerik:RadToolBar AutoPostBack="false" CssClass="ToolBarContainer" EnableRoundedCorners="False" EnableShadows="False" ID="ToolBar" runat="server" Width="100%">
            <Items>
                <telerik:RadToolBarButton>
                    <ItemTemplate>
                        <span class="templateText">Tipo:</span>
                    </ItemTemplate>
                </telerik:RadToolBarButton>
                <telerik:RadToolBarButton CheckOnClick="true" Group="file" Text="CAdES" ToolTip="Firma il file trasformandolo in un P7M" Value="CAdES" />
                <telerik:RadToolBarButton CheckOnClick="true" Group="file" Text="PAdES" ToolTip="Firma PDF che non cambia l'estensione del File" Value="PAdES" />
                <telerik:RadToolBarButton IsSeparator="true" />

                <telerik:RadToolBarButton Value="pinText">
                    <ItemTemplate>
                        <span class="templateText">Pin:</span>
                    </ItemTemplate>
                </telerik:RadToolBarButton>
                <telerik:RadToolBarButton Value="pinContainer2">
                    <ItemTemplate>
                        <span class="templateText">Pin:</span>
                    </ItemTemplate>
                </telerik:RadToolBarButton>
                <telerik:RadToolBarButton Value="pinContainer">
                    <ItemTemplate>
                        <telerik:RadTextBox ID="pin" runat="server" TextMode="Password" Width="50px" />
                    </ItemTemplate>
                </telerik:RadToolBarButton>

                <telerik:RadToolBarButton CommandName="requestOtp" Text="Richiedi OTP" ToolTip="Richiesta OTP" Value="requestOtp" />
                <telerik:RadToolBarButton Value="otpContainer">
                    <ItemTemplate>
                        <span class="templateText">OTP:</span>
                    </ItemTemplate>
                </telerik:RadToolBarButton>
                <telerik:RadToolBarButton Value="otpContainer2">
                    <ItemTemplate>
                        <telerik:RadTextBox ID="proxyOtp" runat="server" TextMode="Password" Width="50px" />
                    </ItemTemplate>
                </telerik:RadToolBarButton>

                <telerik:RadToolBarButton CommandName="sign" Text="Firma" ToolTip="Firma il documento visualizzato." Value="sign" ImageUrl="~/App_Themes/DocSuite2008/imgset16/text_signature.png" />

            </Items>
        </telerik:RadToolBar>

        <object classid="clsid:CBBABF89-D183-11D2-819C-00001C011F1D" style="height: 1px; width: 1px; border: none 0 transparent;" id="signOcx" name="signOcx">
        </object>
    </asp:Panel>
    <asp:Panel runat="server" Width="100%" Style="overflow: hidden; height: 100%;">
        <uc1:ViewerLight runat="server" ID="uscViewerLight" ToolBarVisible="false" />
    </asp:Panel>
</asp:Content>

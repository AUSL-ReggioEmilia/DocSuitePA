<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommIntro" MasterPageFile="~/MasterPages/Base.Master" Title="DSW" CodeBehind="CommIntro.aspx.vb" %>

<asp:Content runat="server" ContentPlaceHolderID="cphMain">
    <telerik:RadPageLayout Width="100%" Height="100%" runat="server" HtmlTag="Div">
        <Rows>
            <telerik:LayoutRow runat="server" HtmlTag="Div" CssClass="topLogo">
                <Content>
                    <asp:Image ID="imgLogo" runat="server" />
                </Content>
            </telerik:LayoutRow>
            <telerik:LayoutRow runat="server" HtmlTag="Div" CssClass="intro-panel dsw-text-center">
                <Content>
                    <asp:Label Font-Bold="True" ID="Version" runat="server" />
                </Content>
            </telerik:LayoutRow>
            <telerik:LayoutRow runat="server" ID="rowVersions" HtmlTag="Div" CssClass="intro-panel dsw-text-center">
                <Content>
                    <div class="dsw-display-inline-block dsw-text-left" style="width: 430px;">
                        <asp:PlaceHolder ID="phVersions" runat="server" />
                    </div>
                </Content>
            </telerik:LayoutRow>
            <telerik:LayoutRow runat="server" HtmlTag="Div" CssClass="intro-panel dsw-text-center">
                <Content>
                    <p>
                        <asp:Label Font-Bold="True" ID="UserConnected" runat="server" />
                    </p>
                    <p>
                        <asp:Label Font-Bold="True" ID="UserMail" runat="server" />
                    </p>
                    <p>
                        <asp:Label Font-Bold="True" ID="UserDomain" runat="server" />
                    </p>
                </Content>
            </telerik:LayoutRow>
            <telerik:LayoutRow runat="server" HtmlTag="Div" ID="rowRecoverProtocols" CssClass="intro-panel dsw-text-center">
                <Content>
                    <p>
                        <asp:Label ID="lblRecoveringProtocols" runat="server" Visible="False" Style="color: Red; font-weight: bold" /></p>                    
                    <telerik:RadButton runat="server" ID="btnProtocolCorrect" Text="Correggi" Style="margin-top: 5px;" Visible="false" PostBackUrl="../Prot/ProtRecupera.aspx" />
                </Content>
            </telerik:LayoutRow>
            <telerik:LayoutRow runat="server" HtmlTag="Div" ID="rowRecoverTemplateCollaborations" CssClass="intro-panel dsw-text-center">
                <Content>
                    <p>
                        <asp:Label ID="lblRecoveringTemplateCollaborations" runat="server" Visible="False" Style="color: Red; font-weight: bold" /></p>                    
                        <telerik:RadButton runat="server" ID="btnTemplateCollaborationCorrects" Text="Correggi Template" Style="margin-top: 5px;" Visible="false" PostBackUrl="../Tblt/TbltTemplateCollaborationManager.aspx?Type=Comm&ViewNotActive=true" />
                </Content>
            </telerik:LayoutRow>
            <telerik:LayoutRow runat="server" HtmlTag="Div" ID="rowRecoverResolutionActivities" CssClass="intro-panel dsw-text-center">
                <Content>
                    <p>
                        <asp:Label ID="lblRecoverResolutionActivities" runat="server" Visible="False" Style="color: Red; font-weight: bold" />
                    </p> 
                    <telerik:RadButton ButtonType="LinkButton" runat="server" ID="btnRecoverResolutionActivities" Text="Vedi attività in errore" Style="margin-top: 5px;" Visible="false" PostBackUrl="../Resl/ReslActivityResults.aspx?Status=2&Type=Resl" />
                </Content>
            </telerik:LayoutRow>
            <telerik:LayoutRow runat="server" HtmlTag="None" WrapperCssClass="intro-button" WrapperHtmlTag="Div">
                <Content>
                    <telerik:RadButton ButtonType="LinkButton" Image-EnableImageButton="true" ID="imbUtente" runat="server" Width="16px" Height="16px" />
                </Content>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>
</asp:Content>

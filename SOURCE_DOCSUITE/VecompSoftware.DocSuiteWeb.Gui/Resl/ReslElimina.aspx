<%@ Page AutoEventWireup="false" Codebehind="ReslElimina.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ReslElimina" Language="vb" MasterPageFile="~/MasterPages/Base.Master" Title="Elimina Proposta" %>

<asp:Content runat="server" ContentPlaceHolderID="cphMain">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script language="javascript" type="text/javascript">
            //restituisce un riferimento alla radwindow
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function CloseWindow(args) {
                var oWindow = GetRadWindow();
                oWindow.close(args);
            }
        </script>
    </telerik:RadScriptBlock>
      
    <telerik:RadAjaxLoadingPanel ID="DefaultLoadingPanel" runat="server">
    </telerik:RadAjaxLoadingPanel>

    <asp:Panel runat="server" ID="pnlMain">
     <telerik:RadPageLayout runat="server" HtmlTag="Div" Width="100%">
        <Rows>
            <telerik:LayoutRow HtmlTag="Div" Style="margin-bottom: 5px;">
                <Columns>
                    <telerik:LayoutColumn HtmlTag="Div">                       
                     <asp:Label ID="lblTitolo" runat="server" Font-Bold="True">Label</asp:Label>
                     </telerik:LayoutColumn>                    
                </Columns>
            </telerik:LayoutRow>
             <telerik:LayoutRow HtmlTag="Div" Style="margin-bottom: 5px;">
                <Columns>
                    <telerik:LayoutColumn HtmlTag="Div">                       
                     <telerik:RadTextBox runat="server" TextMode="MultiLine" Height="90px" Width="100%" ID="rtbAnnulmentReason" />
                        <asp:RequiredFieldValidator ID="rfvAnnulmentReason" ControlToValidate="rtbAnnulmentReason" runat="server" Display="Dynamic" ErrorMessage="Campo obbligatorio" />
                     </telerik:LayoutColumn>                    
                </Columns>
            </telerik:LayoutRow>
             <telerik:LayoutRow HtmlTag="Div" Style="margin-bottom: 5px;">
                <Columns>
                    <telerik:LayoutColumn HtmlTag="Div">                       
                        <asp:button id="btnConferma" runat="server" Text="Conferma" />
                     </telerik:LayoutColumn>                    
                </Columns>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>
    </asp:Panel>  
  
</asp:Content>


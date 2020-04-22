<%@ Page Title="Selezione documenti da esportare" Language="vb" AutoEventWireup="false" CodeBehind="ProtToSeries.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.Prot.ProtToSeries" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>
<%@ Register Src="../UserControl/UscToSeries.ascx" TagName="uscToSeries" TagPrefix="usc" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock ID="radScriptBlock" runat="server" EnableViewState="false">
        <script language="javascript" type="text/javascript">
            
        </script>
    </telerik:RadScriptBlock>    
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadPageLayout runat="server" CssClass="dataform">
                <telerik:LayoutRow HtmlTag="Div">
                <Columns>
                    <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                        <b>
                            <asp:Label ID="lblName" runat="server">Protocollo:</asp:Label>
                        </b>
                    </telerik:LayoutColumn>
                    <telerik:LayoutColumn Span="8" CssClass="t-col-left-padding">
                        <asp:Label ID="lblNumber" runat="server"></asp:Label>
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
    </telerik:RadPageLayout>        
    <usc:uscToSeries id="uscToSeries" runat="server"></usc:uscToSeries>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter" Visible="true">
    <asp:Button runat="server" ID="btnConfirm" Text="Conferma" visible ="true"/>
</asp:Content>

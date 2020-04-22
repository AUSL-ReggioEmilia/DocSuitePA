<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.UserScrivaniaD" CodeBehind="UserScrivaniaD.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscProtGrid.ascx" TagName="uscProtGrid" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/uscDocmGrid.ascx" TagName="uscDocmGrid" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/uscReslGrid.ascx" TagName="uscReslGrid" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/uscDocmGridBar.ascx" TagName="uscDocmGridBar" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/uscProtGridBar.ascx" TagName="uscProtGridBar" TagPrefix="uc1" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadAjaxPanel runat="server" ID="ajaxHeader">
        <div class="titolo" id="divTitolo" runat="server" visible="true">
            <asp:Label ID="lblHeader" runat="server" />
        </div>
    </telerik:RadAjaxPanel>

    <telerik:RadPageLayout ID="headerTable" HtmlTag="None" runat="server" CssClass="col-dsw-10">
        <Rows>
            <telerik:LayoutRow CssClass="col-dsw-10 form-group" HtmlTag="Div" ID="pnlCC" runat="server" Visible="false">
                <Columns>
                    <telerik:LayoutColumn CssClass="control-label" Width="20%" Span="3">
                        <asp:Label runat="server" Text="Visualizza solo Protocolli:" />
                    </telerik:LayoutColumn>
                    <telerik:LayoutColumn Width="80%" Span="9">
                        <asp:RadioButtonList AutoPostBack="true" ID="rbCC" RepeatDirection="Horizontal" runat="server">
                            <asp:ListItem Text="Per Competenza" Value="PC" />
                            <asp:ListItem Text="In Copia Conoscenza" Value="CC" />
                        </asp:RadioButtonList>
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>

            <telerik:LayoutRow CssClass="col-dsw-10 form-group" HtmlTag="Div" ID="rowProtocolType" runat="server" Visible="false">
                <Columns>
                    <telerik:LayoutColumn CssClass="control-label" Width="20%" Span="3">
                        <asp:Label runat="server" Text="Tipologia:" />
                    </telerik:LayoutColumn>
                    <telerik:LayoutColumn  Width="80%" Span="9">
                        <asp:DropDownList AutoPostBack="True" ID="ddlProtocolTypes" runat="server">
                        </asp:DropDownList>
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>

            <telerik:LayoutRow CssClass="col-dsw-10 form-group" HtmlTag="Div" ID="rowResolutionTypes" runat="server" Visible="false">
                <Columns>
                    <telerik:LayoutColumn CssClass="control-label" Width="20%" Span="3">
                        <asp:Label runat="server" Text="Tipologia:" />
                    </telerik:LayoutColumn>
                    <telerik:LayoutColumn Width="80%" Span="9">
                        <telerik:RadAjaxPanel runat="server" ID="pnlResolutionType">
                            <asp:RadioButtonList AutoPostBack="True" ID="rblResolutionTypes" RepeatDirection="Horizontal" runat="server"></asp:RadioButtonList>
                        </telerik:RadAjaxPanel>
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>

            <telerik:LayoutRow CssClass="col-dsw-10 form-group" HtmlTag="Div" ID="pnlContainer" runat="server" Visible="False">
                <Columns>
                    <telerik:LayoutColumn CssClass="control-label" Width="20%" Span="3">
                        <asp:Label runat="server" Text="Filtra per contenitore:" />
                    </telerik:LayoutColumn>
                    <telerik:LayoutColumn  Width="80%" Span="9">
                        <telerik:RadDropDownList AutoPostBack="True" ID="ddlProtContainer" runat="server" Width="400px" DropDownHeight="200px"/>
                        <telerik:RadDropDownList AutoPostBack="True" ID="ddlDocmContainer" runat="server" Width="400px" DropDownHeight="200px"/>
                        <telerik:RadDropDownList AutoPostBack="True" ID="ddlReslContainer" runat="server" Width="400px" DropDownHeight="200px"/>
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>

            <telerik:LayoutRow CssClass="col-dsw-10 form-group" HtmlTag="Div" ID="rowRoles" runat="server" Visible="False">
                <Columns>
                    <telerik:LayoutColumn CssClass="control-label" Width="20%" Span="3">
                        <asp:Label runat="server" Text="Settore proponente:" />
                    </telerik:LayoutColumn>
                    <telerik:LayoutColumn Width="80%" Span="9">
                        <asp:DropDownList AutoPostBack="True" ID="ddlRoles" runat="server" />
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>

            <telerik:LayoutRow CssClass="col-dsw-10 form-group" HtmlTag="Div" ID="pnlCalendar" runat="server" Visible="false">
                <Columns>
                    <telerik:LayoutColumn CssClass="control-label" Width="20%" Span="3">
                        <asp:Label runat="server" ID="lblCalendar" Font-Bold="true"></asp:Label>
                    </telerik:LayoutColumn>
                    <telerik:LayoutColumn Width="80%" Span="9">
                        <telerik:RadDatePicker AutoPostBack="True" ID="rdpCalendar" runat="server" />
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>

            <telerik:LayoutRow CssClass="col-dsw-10 form-group" HtmlTag="Div" ID="pnlDateFilter" runat="server" Visible="false">
                <Columns>
                    <telerik:LayoutColumn CssClass="control-label" Width="10%" Span="3">
                        <asp:Label runat="server" Font-Bold="true">Filtro date</asp:Label>
                    </telerik:LayoutColumn>
                    <telerik:LayoutColumn Span="9">
                        <telerik:RadDatePicker AutoPostBack="True" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="Da" ID="rdpDateFilterFrom" runat="server" />
                        <telerik:RadDatePicker AutoPostBack="True" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="A" ID="rdpDateFilterTo" runat="server" />
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>

        </Rows>
    </telerik:RadPageLayout>

    <asp:Panel runat="server" ID="pnlAlert" Visible="False">
        <div style="margin: 3px; padding: 3px; border: solid 1px red;">
            <asp:Label runat="server" ID="lblAlert" Font-Bold="true" ForeColor="Red" />
        </div>
    </asp:Panel>

</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server" style="overflow:hidden;width:100%;height:100%;">
    <asp:Panel ID="pnlProtocollo" Visible="false" runat="server" Width="100%" Height="100%">
        <uc1:uscProtGrid ColumnClientSelectVisible="true" ColumnContainerVisible="true" ColumnDocSignVisible="false" ColumnFascicleVisible="false" ColumnLinkVisible="false" ColumnObjectVisible="true" ColumnProtocolVisible="true" ColumnRegistrationDateVisible="true" ColumnStatusVisible="false" ColumnTipoVisible="true" ColumnTypeVisible="true" ColumnUnreadVisible="true" ID="uscProtocolGrid" runat="server" />
    </asp:Panel>
    <asp:Panel ID="pnlPratiche" Visible="false" runat="server" Width="100%" Height="100%">
        <uc1:uscDocmGrid ColumnClientSelectVisible="false" ID="uscDocmGrid" runat="server" />
    </asp:Panel>
    <asp:Panel ID="pnlAtti" Visible="false" runat="server" Width="100%" Height="100%">
        <uc1:uscReslGrid ID="uscReslGrid" runat="server" />
    </asp:Panel>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Panel ID="pnlUpdate" runat="server" Visible="False" Style="margin-left: 2px;">
        <asp:Button runat="server" Text="Lavorato" ID="btnUpdate" />
    </asp:Panel>
    <asp:Panel ID="pnlMultiAutorizza" runat="server" Visible="false" Style="margin-left: 2px;">
        <div style="border: 0; float: left; clear: left">
            <asp:Button ID="cmdMultiAutorizza" runat="server" Text="Autorizza selezionati" />
        </div>
    </asp:Panel>
    <asp:Panel ID="pnlMultiDistribuzione" runat="server" Visible="false">
        <asp:Button ID="cmdMultiDistribuzione" runat="server" Text="Distribuzione multipla" />
    </asp:Panel>

    <asp:Panel ID="pnlHighlight" runat="server" Visible="false">
        <asp:Button ID="btnRemoveHighlight" runat="server" Text="Rimuovi evidenza" />
    </asp:Panel>

    <asp:Panel ID="pnlButtonBar" runat="server" Visible="False" Style="margin-left: 2px;">
        <table id="tblSegnaLetti" width="100%">
            <tr>
                <td>
                    <asp:Panel runat="server" ID="pnlGridBarProt">
                        <uc1:uscProtGridBar runat="server" ID="uscProtocolGridBar" />
                    </asp:Panel>
                    <asp:Panel runat="server" ID="pnlGridBarDocm">
                        <uc1:uscDocmGridBar runat="server" ID="uscDocumentGridBar" />
                    </asp:Panel>
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>

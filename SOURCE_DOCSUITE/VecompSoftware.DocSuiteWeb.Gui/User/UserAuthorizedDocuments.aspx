<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.UserAuthorizedDocuments" CodeBehind="UserAuthorizedDocuments.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="../UserControl/uscProtGrid.ascx" TagName="uscProtGrid" TagPrefix="uc1" %>
<%@ Register Src="~/UserControl/uscProtGridBar.ascx" TagName="uscProtGridBar" TagPrefix="uc1" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">

   <telerik:RadPageLayout runat="server" HtmlTag="None" CssClass="col-dsw-10">
        <Rows>            
            <telerik:LayoutRow CssClass="col-dsw-10 form-group" HtmlTag="Div" ID="rowDate" runat="server">
                <Columns>
                    <telerik:LayoutColumn HtmlTag="Div" CssClass="control-label" Span="3">
                        <asp:Label ID="lblRegistrationDate" runat="server" Text="Data registrazione:" />
                    </telerik:LayoutColumn>
                    <telerik:CompositeLayoutColumn HtmlTag="Div" CssClass="form-control dsw-vertical-middle" Span="9">
                        <Content>
                            <telerik:RadDatePicker ID="rdpDateFrom" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="Da" runat="server" />
                            <asp:CompareValidator ControlToValidate="rdpDateFrom" Display="Dynamic" ErrorMessage="Errore formato" ID="cfvDateFrom" Operator="DataTypeCheck" runat="server" Type="Date" />
                            <asp:RequiredFieldValidator ControlToValidate="rdpDateFrom" Display="Dynamic" ErrorMessage="Campo Obbligatorio" ID="rfvDateFrom" runat="server" />

                            <telerik:RadDatePicker ID="rdpDateTo" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important; margin-left: 5px;" Width="200" DateInput-Label="A" runat="server" />
                            <asp:CompareValidator ControlToValidate="rdpDateTo" Display="Dynamic" ErrorMessage="Errore formato" ID="cfvDateTo" Operator="DataTypeCheck" runat="server" Type="Date" />
                            <asp:RequiredFieldValidator ControlToValidate="rdpDateTo" Display="Dynamic" ErrorMessage="Campo Obbligatorio" ID="rfvDateTo" runat="server" />
                        </Content>
                    </telerik:CompositeLayoutColumn>
                </Columns>
            </telerik:LayoutRow>
            <telerik:LayoutRow CssClass="col-dsw-10 form-group" HtmlTag="Div" ID="rowContainer" runat="server" Visible="false">
                <Columns>
                    <telerik:LayoutColumn HtmlTag="Div" CssClass="control-label" Span="3">
                        <asp:Label ID="lblContainer" runat="server" Text="Filtra per contenitore:" />
                    </telerik:LayoutColumn>
                    <telerik:LayoutColumn HtmlTag="Div" CssClass="form-control" Span="9">                        
                        <telerik:RadDropDownList ID="ddlContainer" runat="server" Width="400px" EnableVirtualScrolling="true" DropDownHeight="200px"/>                        
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
            <telerik:LayoutRow CssClass="col-dsw-10 form-group" HtmlTag="Div" WrapperHtmlTag="None">
                <Content>
                    <asp:Button ID="btnSearch" runat="server" Text="Cerca" />
                </Content>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>
</asp:Content>


<asp:Content ContentPlaceHolderID="cphContent" ID="ProtGridContent" runat="server" style="overflow:hidden;width:100%;height:100%;">
    <div style="height:100%">
        <uc1:uscProtGrid ColumnClientSelectVisible="true" ColumnContainerVisible="true" ColumnCategoryNameVisible="false" ColumnProtocolContactVisible="false" ColumnDocSignVisible="false" ColumnFascicleVisible="false" ColumnLinkVisible="false" ColumnObjectVisible="true" ColumnProtocolVisible="true" ColumnRegistrationDateVisible="true" ColumnStatusVisible="false" ColumnTipoVisible="true" ColumnTypeVisible="true" ColumnUnreadVisible="true" ID="uscProtocolGrid" runat="server" />
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    
</asp:Content>

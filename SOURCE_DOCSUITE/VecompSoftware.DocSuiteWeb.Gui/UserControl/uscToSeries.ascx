<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscToSeries.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscToSeries" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server">
    <script type="text/javascript">
        function OpenGenericWindow(button, args) {
            if (args._commandArgument === null)
                return false;

            var wnd = DSWOpenGenericWindow(args._commandArgument, WindowTypeEnum.NORMAL);
            wnd.set_visibleStatusbar(false);
            wnd.set_modal(true);
            wnd.set_showOnTopWhenMaximized(false);
            wnd.set_destroyOnClose(true);
            return wnd;
        }
    </script>
</telerik:RadScriptBlock>

    <telerik:RadPageLayout runat="server" HtmlTag="Div" Width="100%" CssClass="dataform">
        <Rows>
            <telerik:LayoutRow HtmlTag="Div">
                <Columns>
                    <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                        <b><asp:Label ID="DocumentSeries" runat="server" /></b>
                    </telerik:LayoutColumn>
                    <telerik:LayoutColumn Span="8" CssClass="t-col-left-padding">
                        <asp:DropDownList runat="server" ID="ddlDocumentSeries" Visible="True" Width="300px" AutoPostBack="true" EnableViewState="True" />
                        <asp:RequiredFieldValidator ID="rfvDocumentSeries" ControlToValidate="ddlDocumentSeries" runat="server" />
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>
        <telerik:RadGrid runat="server" ID="documentListGrid" Width="100%" AllowMultiRowSelection="True" EnableViewState="true">
            <MasterTableView AutoGenerateColumns="False" DataKeyNames="Serialized">
                <Columns>
                    <telerik:GridClientSelectColumn UniqueName="selectColumn" HeaderText="Imp." HeaderTooltip="Documenti da esportare" ItemStyle-Width="20px" />
                    <telerik:GridTemplateColumn UniqueName="Type" HeaderText="Tipo">
                        <ItemTemplate>
                            <asp:Label runat="server" ID="fileType" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="Description" HeaderText="Documento">
                        <ItemTemplate>
                            <telerik:RadButton Height="16px" ID="fileImage" runat="server" ToolTip="Visualizza anteprima" Width="16px" />
                            <asp:Label runat="server" ID="fileName" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="exportType" HeaderText="Copia conforme">
                        <ItemTemplate>
                            <telerik:RadButton AutoPostBack="false" ButtonType="ToggleButton" Checked="True" ID="pdf" runat="server" ToggleType="CheckBox" ToolTip="Seleziona per la Copia conforme, altrimenti verrà usato l'originale" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="chainType" HeaderText="Catena documentale">
                        <ItemTemplate>
                            <asp:DropDownList runat="server" ID="chainTypes" Visible="False" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
            <ClientSettings EnableRowHoverStyle="true" Selecting-AllowRowSelect="true" Selecting-UseClientSelectColumnOnly="true" />
        </telerik:RadGrid>    


<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>

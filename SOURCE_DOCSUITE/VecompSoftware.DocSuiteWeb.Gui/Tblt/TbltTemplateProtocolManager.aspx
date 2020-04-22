<%@ Page Title="Gestione Template di Protocollo" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="TbltTemplateProtocolManager.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltTemplateProtocolManager" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" EnableViewState="false">
        <script type="text/javascript">
            function CloseWindow() {
                var oWindow = GetRadWindow();
                oWindow.close();
                LoadPageData();
            }

            function pageLoad(sender, eventArgs) {
                if (!eventArgs.get_isPartialLoad()) {
                    LoadPageData();
                }
            }

            function addNewTemplate() {
                TemplateProtocolManagerPageRequest("add");
                return false;
            }

            function deleteTemplate() {
                TemplateProtocolManagerPageRequest("delete");
                return false;
            }

            function setDefault() {
                TemplateProtocolManagerPageRequest("setDefault");
                return false;
            }

            function LoadPageData() {
                TemplateProtocolManagerPageRequest("loadData");
            }

            function TemplateProtocolManagerPageRequest(action) {
                var manager = $find("<%= AjaxManager.ClientID %>");
                manager.ajaxRequest(action);
            }

            function SelectMeOnly(objRadioButton, grdName) {

                var i, obj;
                for (i = 0; i < document.all.length; i++) {
                    obj = document.all(i);

                    if (obj.type == "radio") {
                        if (objRadioButton.id == obj.id)
                            obj.checked = true;
                        else
                            obj.checked = false;
                    }
                }
            }
        </script>
    </telerik:RadScriptBlock>
</asp:Content>
<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <div style="overflow:hidden;width:100%;height:100%;">
        <DocSuite:BindGrid ID="grdTemplateProtocol" runat="server" AutoGenerateColumns="false" AllowMultiRowSelection="false" GridLines="Both">
        <MasterTableView TableLayout="Auto" DataKeyNames="Id" AllowFilteringByColumn="true" GridLines="Both">
            <Columns>
                <telerik:GridTemplateColumn UniqueName="Select" HeaderImageUrl="../Comm/Images/PriorityHigh16.gif" HeaderTooltip="Selezione del Template" AllowFiltering="false" Groupable="false">
                    <HeaderStyle HorizontalAlign="Center" Width="5%" />
                    <ItemStyle HorizontalAlign="Center" Width="5%" />
                    <ItemTemplate>
                        <asp:RadioButton ID="rdbSelect" runat="server" GroupName="selectTemplate" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="RemoveDefault" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/remove_star.png" HeaderTooltip="Rimuovi Default" AllowFiltering="false" Groupable="false">
                    <HeaderStyle HorizontalAlign="Center" Width="5%" />
                    <ItemStyle HorizontalAlign="Center" Width="5%" />
                    <ItemTemplate>
                        <asp:ImageButton runat="server" ID="imgRemoveDefault" ToolTip="Rimuovi Default" CommandName="removeDefault" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="Default" HeaderText="Default" AllowFiltering="false" Groupable="false">
                    <HeaderStyle HorizontalAlign="Center" Width="5%" />
                    <ItemStyle HorizontalAlign="Center" Width="5%" />
                    <ItemTemplate>
                        <asp:Image runat="server" ID="imgDefault" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="TemplateName" HeaderText="Descrizione" AllowFiltering="false" Groupable="false" AllowSorting="True" SortExpression="TemplateName">
                    <HeaderStyle HorizontalAlign="Center" Width="25%" />
                    <ItemStyle HorizontalAlign="Center" Width="25%" Height="40px" />
                    <ItemTemplate>
                        <asp:LinkButton runat="server" ID="lblTemplateName" CommandName="editTemplate"></asp:LinkButton>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridDateTimeColumn UniqueName="RegistrationDate" DataField="RegistrationDate" HeaderText="Data registrazione" AllowFiltering="false" Groupable="false" DataFormatString="{0:dd/MM/yyyy}" SortExpression="RegistrationDate">
                    <HeaderStyle HorizontalAlign="Center" Width="15%" />
                    <ItemStyle HorizontalAlign="Center" Width="15%" />
                </telerik:GridDateTimeColumn>
                <telerik:GridDateTimeColumn UniqueName="RegistrationUser" DataField="RegistrationUser" HeaderText="Utente Registrazione" AllowFiltering="false" Groupable="false" SortExpression="RegistrationUser">
                    <HeaderStyle HorizontalAlign="Center" Width="15%" />
                    <ItemStyle HorizontalAlign="Center" Width="15%" />
                </telerik:GridDateTimeColumn>
                <telerik:GridDateTimeColumn UniqueName="LastChangedDate" DataField="LastChangedDate" HeaderText="Data Ultima Modifica" AllowFiltering="false" Groupable="false" DataFormatString="{0:dd/MM/yyyy}" SortExpression="LastChangedDate">
                    <HeaderStyle HorizontalAlign="Center" Width="15%" />
                    <ItemStyle HorizontalAlign="Center" Width="15%" />
                </telerik:GridDateTimeColumn>
                <telerik:GridDateTimeColumn UniqueName="LastChangedUser" DataField="LastChangedUser" HeaderText="Utente Ultima Modifica" AllowFiltering="false" Groupable="false" SortExpression="LastChangedUser">
                    <HeaderStyle HorizontalAlign="Center" Width="15%" />
                    <ItemStyle HorizontalAlign="Center" Width="15%" />
                </telerik:GridDateTimeColumn>
            </Columns>
        </MasterTableView>
        <ClientSettings>
            <Selecting AllowRowSelect="true" />
        </ClientSettings>
    </DocSuite:BindGrid>
    </div>
</asp:Content>
<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <div class="footer-buttons-wrapper">
        <asp:Button Width="150" runat="server" ID="btnNewTemplate" Text="Nuovo" OnClientClick="return addNewTemplate();" ToolTip="Nuovo Template di Protocollo" />
        <asp:Button Width="150" runat="server" ID="btnDeleteTemplate" Text="Elimina" OnClientClick="return deleteTemplate();" ToolTip="Elimina Template" />
        <asp:Button Width="150" runat="server" ID="btnDefault" Text="Imposta Default" OnClientClick="return setDefault();" ToolTip="Imposta il Template come default" />
    </div>
</asp:Content>

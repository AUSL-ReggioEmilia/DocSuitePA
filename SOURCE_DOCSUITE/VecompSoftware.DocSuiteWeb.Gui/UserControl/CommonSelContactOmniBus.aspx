<%@ Page Title="Seleziona contatti" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="CommonSelContactOmniBus.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.CommonSelContactOmniBus" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">
            var commonSelContactOmniBus;
            require(["UserControl/CommonSelContactOmniBus"], function (CommonSelContactOmniBus) {
                $(function () {
                    commonSelContactOmniBus = new CommonSelContactOmniBus();
                    commonSelContactOmniBus.txtNomeId = "<%= txtNome.ClientID %>";
                    commonSelContactOmniBus.txtCognomeId = "<%= txtCognome.ClientID %>";
                    commonSelContactOmniBus.rcbDistrettoId = "<%= rcbDistretto.ClientID %>";
                    commonSelContactOmniBus.txtCdcId = "<%= txtCdc.ClientID %>";
                    commonSelContactOmniBus.btnFindId = "<%= btnFind.ClientID %>";
                    commonSelContactOmniBus.btnFindUniqueId = "<%= btnFind.UniqueID %>";
                    commonSelContactOmniBus.rtvResultsId = "<%= rtvResults.ClientID %>";
                    commonSelContactOmniBus.btnConfirmId = "<%= btnConfirm.ClientID %>";
                    commonSelContactOmniBus.btnConfirmAndNewId = "<%= btnConfirmAndNew.ClientID %>";
                    commonSelContactOmniBus.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                    commonSelContactOmniBus.callerId = "<%= CallerId %>";
                    commonSelContactOmniBus.initialize();
                });
            });
        </script>
    </telerik:RadScriptBlock>

    <asp:Panel runat="server" DefaultButton="btnFind">
        <table class="datatable">
            <tr>
                <td class="label labelPanel col-dsw-2">Nome:</td>
                <td>
                    <telerik:RadTextBox runat="server" ID="txtNome" Width="350px"></telerik:RadTextBox>
                </td>
            </tr>
            <tr>
                <td class="label labelPanel col-dsw-2">Cognome:</td>
                <td>
                    <telerik:RadTextBox runat="server" ID="txtCognome" Width="350px"></telerik:RadTextBox>
                </td>
            </tr>
            <tr>
                <td class="label labelPanel col-dsw-2">Centro di costo:</td>
                <td>
                    <telerik:RadTextBox runat="server" ID="txtCdc" Width="200px"></telerik:RadTextBox>
                </td>
            </tr>
            <tr>
                <td class="label labelPanel col-dsw-2">Distretto/Ospedale:</td>
                <td>
                    <telerik:RadComboBox runat="server" ID="rcbDistretto" Width="300px" Sort="Ascending" Filter="Contains">
                        <Items>
                            <telerik:RadComboBoxItem Text="" Value="" Selected="true" />
                            <telerik:RadComboBoxItem Text="C.MONTI" Value="CAS" />
                            <telerik:RadComboBoxItem Text="CORREGGIO" Value="COR" />
                            <telerik:RadComboBoxItem Text="GUASTALLA" Value="GUA" />
                            <telerik:RadComboBoxItem Text="MONTECCHIO" Value="MON" />
                            <telerik:RadComboBoxItem Text="OSPEDALE S.MARIA" Value="OSP" />
                            <telerik:RadComboBoxItem Text="REGGIO" Value="REG" />
                            <telerik:RadComboBoxItem Text="SCANDIANO" Value="SCA" />
                        </Items>
                    </telerik:RadComboBox>
                </td>
            </tr>
            <tr>
                <td class="labelPanel col-dsw-2"></td>
                <td>
                    <telerik:RadButton ID="btnFind" AutoPostBack="false" runat="server" Text="Cerca">
                        <Icon PrimaryIconUrl="../App_Themes/DocSuite2008/images/search-transparent.png" />
                    </telerik:RadButton>
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <telerik:RadTreeView runat="server" ID="rtvResults" CheckBoxes="false" Height="100%">
        <Nodes>
            <telerik:RadTreeNode Value="root" Checkable="false" Expanded="true" Text="Contatti"></telerik:RadTreeNode>
        </Nodes>
    </telerik:RadTreeView>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <telerik:RadButton runat="server" ID="btnConfirm" AutoPostBack="false" Text="Conferma"></telerik:RadButton>
    <telerik:RadButton runat="server" ID="btnConfirmAndNew" AutoPostBack="false" Text="Conferma e nuovo"></telerik:RadButton>
</asp:Content>

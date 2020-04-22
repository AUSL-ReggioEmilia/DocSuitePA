<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="ReslActivityEdit.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ReslActivityEdit" Title="Modifica data attività" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script type="text/javascript">

            function CloseWindow(action) {
                var window = GetRadWindow();
                window.close(action);
            }

        </script>
    </telerik:RadScriptBlock>

<asp:Panel ID="pnlData" runat="server">
            <table class="datatable" width="100%">
                <tr>
                    <th colspan="2">Modifica data attività
                    </th>
                </tr>
                <tr class="Chiaro">
                    <td class="label" style="width: 20%">Data:
                    </td>
                    <td style="width: 80%">
                        <telerik:RadDatePicker ID="txtDate" runat="server" />
                        <asp:RequiredFieldValidator ControlToValidate="txtDate" Display="Dynamic" ErrorMessage="Data obbligatoria" ID="rfvData" runat="server" />
                        <br />
                        <asp:CompareValidator ID="cvCompareData" runat="server" Type="Date" Operator="GreaterThan"
                            ControlToValidate="txtDate" ErrorMessage="La data deve essere superiore alla data di oggi."
                            Display="Dynamic"></asp:CompareValidator>
                    </td>
                </tr>
                <tr class="Spazio">
                    <td></td>
                </tr>
            </table>
        </asp:Panel>
  </asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <telerik:RadButton runat="server" id="btnSave" Text="Conferma" Enabled="False"></telerik:RadButton>
</asp:Content>
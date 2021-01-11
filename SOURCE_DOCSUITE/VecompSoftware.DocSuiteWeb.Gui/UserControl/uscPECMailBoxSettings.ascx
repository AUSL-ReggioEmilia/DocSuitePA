<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscPECMailBoxSettings.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscPECMailBoxSettings" %>

<telerik:RadScriptBlock runat="server" EnableViewState="False">
    <script type="text/javascript">
        var uscPECMailBoxSettings;
        require(["UserControl/uscPECMailBoxSettings"], function (UscPECMailBoxSettings) {
            $(function () {
                uscPECMailBoxSettings = new UscPECMailBoxSettings(tenantModelConfiguration.serviceConfiguration);
                uscPECMailBoxSettings.rwPECMailBoxSettingsId = "<%= rwPECMailBoxSettings.ClientID %>";
                uscPECMailBoxSettings.lblPECMailBoxIdId = "<%= lblPECMailBoxId.ClientID %>";
                uscPECMailBoxSettings.txtMailBoxNameId = "<%= txtMailboxName.ClientID %>";
                uscPECMailBoxSettings.txtUsernameId = "<%= txtUsername.ClientID %>";
                uscPECMailBoxSettings.txtPasswordId = "<%= txtPassword.ClientID %>";
                uscPECMailBoxSettings.txtPasswordRequireValidatorId = "<%= txtPasswordRequireValidator.ClientID %>";
                uscPECMailBoxSettings.chkIsInteropId = "<%= chkIsInterop.ClientID %>";
                uscPECMailBoxSettings.chkIsProtocolId = "<%= chkIsProtocol.ClientID %>";
                uscPECMailBoxSettings.chkIsPublicProtocolId = "<%= chkIsPublicProtocol.ClientID %>";
                uscPECMailBoxSettings.ddlLocationId = "<%= ddlLocation.ClientID %>";
                uscPECMailBoxSettings.ddlINServerTypeId = "<%= ddlINServerType.ClientID %>";
                uscPECMailBoxSettings.txtINServerNameId = "<%= txtINServerName.ClientID %>";
                uscPECMailBoxSettings.txtINPortId = "<%= txtINPort.ClientID %>";
                uscPECMailBoxSettings.chkINSslId = "<%= chkINSsl.ClientID %>";
                uscPECMailBoxSettings.txtOUTServerNameId = "<%= txtOUTServerName.ClientID %>";
                uscPECMailBoxSettings.txtOUTPortId = "<%= txtOUTPort.ClientID %>";
                uscPECMailBoxSettings.chkOUTSslId = "<%= chkOUTSsl.ClientID %>";
                uscPECMailBoxSettings.chkManagedId = "<%= chkManaged.ClientID %>";
                uscPECMailBoxSettings.chkUnmanagedId = "<%= chkUnmanaged.ClientID %>";
                uscPECMailBoxSettings.chkIsHandleEnabledId = "<%= chkIsHandleEnabled.ClientID %>";
                uscPECMailBoxSettings.ddlProfileAddId = "<%= ddlProfileAdd.ClientID %>";
                uscPECMailBoxSettings.ddlJeepServiceInId = "<%= ddlJeepServiceIn.ClientID %>";
                uscPECMailBoxSettings.ddlJeepServiceOutId = "<%= ddlJeepServiceOut.ClientID %>";
                uscPECMailBoxSettings.ddlInvoiceTypeId = "<%= ddlInvoiceType.ClientID %>";
                uscPECMailBoxSettings.chkLoginErrorId = "<%= chkLoginError.ClientID %>";
                uscPECMailBoxSettings.invoiceTypes = JSON.parse('<%= InvoiceTypes %>');

                uscPECMailBoxSettings.initialize();
            });
        });

        function save() {
            if (document.getElementById("<%= lblPECMailBoxId.ClientID %>").innerText !== "")
                ValidatorEnable(document.getElementById('<%=txtPasswordRequireValidator.ClientID%>'), false);
            if (Page_ClientValidate("PecMailBoxes")) {
                uscPECMailBoxSettings.savePECMailBox();
            }
        }
    </script>
</telerik:RadScriptBlock>

<telerik:RadWindow runat="server" ID="rwPECMailBoxSettings" Title="PEC - Gestione caselle PEC" Width="700" Height="560px" Behaviors="Close">
    <ContentTemplate>

        <asp:Panel runat="server" ID="pnlPageContent">
            <table class="datatable">
                <tr>
                    <th colspan="2" class="tabella">Modifica impostazioni casella PEC
                    </th>
                </tr>
                <tr>
                    <td>
                        <asp:HiddenField runat="server" ID="lblPECMailBoxId" />
                    </td>
                </tr>
                <tr>
                    <td class="label">Nome casella</td>
                    <td>
                        <asp:TextBox runat="server" ID="txtMailboxName" Width="240" ValidationGroup="PecMailBoxes" />
                        <asp:RequiredFieldValidator ID="txtMailboxNameRequireValidator" runat="server" ControlToValidate="txtMailboxName"
                            ErrorMessage="Il campo Nome Casella è Obbligatorio" Display="Dynamic" ValidationGroup="PecMailBoxes"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="label" style="width: 30%">Nome utente</td>
                    <td>
                        <asp:TextBox runat="server" ID="txtUsername" Width="240" ValidationGroup="PecMailBoxes" />
                        <asp:RequiredFieldValidator ID="txtUsernameRequireValidator" runat="server" ControlToValidate="txtUsername"
                            ErrorMessage="Il campo Nome Utente è Obbligatorio" Display="Dynamic" ValidationGroup="PecMailBoxes"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="label" style="width: 30%">Password</td>
                    <td>
                        <asp:TextBox runat="server" ID="txtPassword" Width="240" TextMode="Password" ValidationGroup="PecMailBoxes" />
                        <asp:RequiredFieldValidator ID="txtPasswordRequireValidator" runat="server" ControlToValidate="txtPassword"
                            ErrorMessage="Il campo Password è Obbligatorio" Display="Dynamic" ValidationGroup="PecMailBoxes"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="label" style="width: 30%">Casella interoperabile</td>
                    <td>
                        <asp:CheckBox runat="server" ID="chkIsInterop" />
                    </td>
                </tr>
                <tr>
                    <td class="label" style="width: 30%">Casella di protocollazione</td>
                    <td>
                        <asp:CheckBox runat="server" ID="chkIsProtocol" /></td>
                </tr>
                <tr>
                    <td class="label" style="width: 30%">Casella di protocollazione pubblica</td>
                    <td>
                        <asp:CheckBox runat="server" ID="chkIsPublicProtocol" /></td>
                </tr>
                <tr>
                    <td class="label" style="width: 30%">Location</td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlLocation" DataTextField="Name" DataValueField="Id" AppendDataBoundItems="true" Width="240">
                            <asp:ListItem Text="" Value="" />
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="ddlLocationRequireValidator" runat="server"
                            ErrorMessage="Il campo Location è Obbligatorio" ControlToValidate="ddlLocation"
                            ValidationGroup="PecMailBoxes" InitialValue=""></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="label" style="width: 30%">IN: Tipo server</td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlINServerType" Width="240">
                            <Items>
                                <asp:ListItem Text="" Value="-1" />
                                <asp:ListItem Text="POP3" Value="0" />
                                <asp:ListItem Text="IMAP" Value="1" />
                            </Items>
                        </asp:DropDownList>
                    </td>
                </tr>
                <tr>
                    <td class="label" style="width: 30%">IN: Nome server</td>
                    <td>
                        <asp:TextBox runat="server" ID="txtINServerName" Width="240" ValidationGroup="PecMailBoxes" />
                    </td>
                </tr>
                <tr>
                    <td class="label" style="width: 30%">IN: Porta</td>
                    <td>
                        <asp:TextBox runat="server" ID="txtINPort" Width="240" ValidationGroup="PecMailBoxes" />
                        <asp:RequiredFieldValidator ID="txtINPortRequireValidator" runat="server" ControlToValidate="txtINPort"
                            ErrorMessage="Il campo IN: Porta è Obbligatorio" Display="Dynamic" ValidationGroup="PecMailBoxes"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="label" style="width: 30%">IN: SSL</td>
                    <td>
                        <asp:CheckBox runat="server" ID="chkINSsl" /></td>
                </tr>

                <tr>
                    <td class="label" style="width: 30%">OUT: Nome server</td>
                    <td>
                        <asp:TextBox runat="server" ID="txtOUTServerName" Width="240" ValidationGroup="PecMailBoxes" />
                    </td>
                </tr>
                <tr>
                    <td class="label" style="width: 30%">OUT: Porta</td>
                    <td>
                        <asp:TextBox runat="server" ID="txtOUTPort" Width="240" ValidationGroup="PecMailBoxes" />
                        <asp:RequiredFieldValidator ID="txtOUTPortRequireValidator" runat="server" ControlToValidate="txtOUTPort"
                            ErrorMessage="Il campo OUT: Porta è Obbligatorio" Display="Dynamic" ValidationGroup="PecMailBoxes"></asp:RequiredFieldValidator>
                    </td>
                </tr>
                <tr>
                    <td class="label" style="width: 30%">OUT: SSL</td>
                    <td>
                        <asp:CheckBox runat="server" ID="chkOUTSsl" /></td>
                </tr>
                <tr>
                    <td class="label" style="width: 30%">Gestita</td>
                    <td>
                        <asp:CheckBox runat="server" ID="chkManaged" /></td>
                </tr>
                <tr>
                    <td class="label" style="width: 30%">Non gestita</td>
                    <td>
                        <asp:CheckBox runat="server" ID="chkUnmanaged" /></td>
                </tr>
                <tr>
                    <td class="label" style="width: 30%">Abilita presa in carico</td>
                    <td>
                        <asp:CheckBox runat="server" ID="chkIsHandleEnabled" /></td>
                </tr>
                <tr>
                    <td class="label" style="width: 30%">Profilo</td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlProfileAdd" DataTextField="Name" DataValueField="Id" Width="240" /></td>
                </tr>
                <tr>
                    <td class="label" style="width: 30%">IN: JeepService Associato</td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlJeepServiceIn" DataTextField="Hostname" AppendDataBoundItems="true" DataValueField="Id" Width="240">
                            <asp:ListItem Text="" Value="" />
                        </asp:DropDownList></td>
                </tr>
                <tr>
                    <td class="label" style="width: 30%">OUT: JeepService Associato</td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlJeepServiceOut" DataTextField="Hostname" AppendDataBoundItems="true" DataValueField="Id" Width="240">
                            <asp:ListItem Text="" Value="" />
                        </asp:DropDownList></td>
                </tr>
                <tr>
                    <td class="label" style="width: 30%">Tipologia di fatturazione elettronica</td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlInvoiceType" DataTextField="InvoiceType" AppendDataBoundItems="true" DataValueField="Id" Width="240">
                            <asp:ListItem Text="" Value="" />
                        </asp:DropDownList></td>
                </tr>
                <tr>
                    <td class="label" style="width: 30%">Visualizza PEC nelle viste</td>
                    <td>
                        <asp:CheckBox runat="server" ID="chkLoginError" /></td>
                </tr>
                <tr>
                    <td>
                        <asp:Button ID="btnSave" runat="server" Text="Salva" Width="150" ValidationGroup="PECMailBoxes" OnClientClick="save(); return false;" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </ContentTemplate>
</telerik:RadWindow>

<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscPECMailBoxSettings.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscPECMailBoxSettings" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server" EnableViewState="False">
    <script type="text/javascript">
        var <%= Me.ClientID %>_uscPECMailBoxSettings;
        require(["UserControl/uscPECMailBoxSettings"], function (UscPECMailBoxSettings) {
            $(function () {
                <%= Me.ClientID %>_uscPECMailBoxSettings = new UscPECMailBoxSettings(tenantModelConfiguration.serviceConfiguration, "<%= Me.ClientID %>");
                <%= Me.ClientID %>_uscPECMailBoxSettings.ajaxLoadingPanelId = "<%= BasePage.MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                <%= Me.ClientID %>_uscPECMailBoxSettings.txtMailBoxNameId = "<%= txtMailboxName.ClientID %>";
                <%= Me.ClientID %>_uscPECMailBoxSettings.txtUsernameId = "<%= txtUsername.ClientID %>";
                <%= Me.ClientID %>_uscPECMailBoxSettings.txtPasswordId = "<%= txtPassword.ClientID %>";
                <%= Me.ClientID %>_uscPECMailBoxSettings.txtPasswordRequireValidatorId = "<%= txtPasswordRequireValidator.ClientID %>";
                <%= Me.ClientID %>_uscPECMailBoxSettings.chkIsInteropId = "<%= chkIsInterop.ClientID %>";
                <%= Me.ClientID %>_uscPECMailBoxSettings.chkIsProtocolId = "<%= chkIsProtocol.ClientID %>";
                <%= Me.ClientID %>_uscPECMailBoxSettings.chkIsPublicProtocolId = "<%= chkIsPublicProtocol.ClientID %>";
                <%= Me.ClientID %>_uscPECMailBoxSettings.ddlLocationId = "<%= ddlLocation.ClientID %>";
                <%= Me.ClientID %>_uscPECMailBoxSettings.ddlINServerTypeId = "<%= ddlINServerType.ClientID %>";
                <%= Me.ClientID %>_uscPECMailBoxSettings.txtINServerNameId = "<%= txtINServerName.ClientID %>";
                <%= Me.ClientID %>_uscPECMailBoxSettings.txtINPortId = "<%= txtINPort.ClientID %>";
                <%= Me.ClientID %>_uscPECMailBoxSettings.chkINSslId = "<%= chkINSsl.ClientID %>";
                <%= Me.ClientID %>_uscPECMailBoxSettings.txtOUTServerNameId = "<%= txtOUTServerName.ClientID %>";
                <%= Me.ClientID %>_uscPECMailBoxSettings.txtOUTPortId = "<%= txtOUTPort.ClientID %>";
                <%= Me.ClientID %>_uscPECMailBoxSettings.chkOUTSslId = "<%= chkOUTSsl.ClientID %>";
                <%= Me.ClientID %>_uscPECMailBoxSettings.chkManagedId = "<%= chkManaged.ClientID %>";
                <%= Me.ClientID %>_uscPECMailBoxSettings.chkUnmanagedId = "<%= chkUnmanaged.ClientID %>";
                <%= Me.ClientID %>_uscPECMailBoxSettings.chkIsHandleEnabledId = "<%= chkIsHandleEnabled.ClientID %>";
                <%= Me.ClientID %>_uscPECMailBoxSettings.ddlProfileAddId = "<%= ddlProfileAdd.ClientID %>";
                <%= Me.ClientID %>_uscPECMailBoxSettings.ddlJeepServiceInId = "<%= ddlJeepServiceIn.ClientID %>";
                <%= Me.ClientID %>_uscPECMailBoxSettings.ddlJeepServiceOutId = "<%= ddlJeepServiceOut.ClientID %>";
                <%= Me.ClientID %>_uscPECMailBoxSettings.ddlInvoiceTypeId = "<%= ddlInvoiceType.ClientID %>";
                <%= Me.ClientID %>_uscPECMailBoxSettings.chkLoginErrorId = "<%= chkLoginError.ClientID %>";
                <%= Me.ClientID %>_uscPECMailBoxSettings.invoiceTypes = JSON.parse('<%= InvoiceTypes %>');
                <%= Me.ClientID %>_uscPECMailBoxSettings.isValidEncryptionKey = <%= IsValidEncryptionKey.ToString().ToLower() %>;
                <%= Me.ClientID %>_uscPECMailBoxSettings.ajaxManagerId = "<%= AjaxManager.ClientID %>";
                <%= Me.ClientID %>_uscPECMailBoxSettings.btnSaveId = "<%= btnSave.ClientID%>";
                <%= Me.ClientID %>_uscPECMailBoxSettings.pnlDetailsId = "<%= pnlDetails.ClientID%>";
                <%= Me.ClientID %>_uscPECMailBoxSettings.rpbDetailsId = "<%= rpbDetails.ClientID %>";
                <%= Me.ClientID %>_uscPECMailBoxSettings.isInsertAction = <%= IsInsertAction.ToString().ToLower()%>;
                <%= Me.ClientID %>_uscPECMailBoxSettings.validationGroupName = "<%= ValidationGroupName %>";
                <%= Me.ClientID %>_uscPECMailBoxSettings.initialize();
            });
        });
    </script>
</telerik:RadScriptBlock>

<asp:Panel runat="server" ID="pnlDetails" CssClass="dsw-panel">
    <div class="dsw-panel-content">
        <telerik:RadPanelBar runat="server" AllowCollapseAllItems="true" ExpandMode="MultipleExpandedItems" Width="100%" ID="rpbDetails">
            <Items>
                <telerik:RadPanelItem Text="Modifica impostazioni casella PEC" Expanded="true" Value="pnlInformations">
                    <ContentTemplate>
                        <asp:Panel runat="server" ID="pnlPageContent" CssClass="dsw-panel">
                            <table class="datatable">
                                <tr>
                                    <td class="label">Nome casella</td>
                                    <td>
                                        <asp:TextBox runat="server" ID="txtMailboxName" Width="240" />
                                        <asp:RequiredFieldValidator ID="txtMailboxNameRequireValidator" runat="server" ControlToValidate="txtMailboxName"
                                            ErrorMessage="Il campo Nome Casella è Obbligatorio" Display="Dynamic"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="label" style="width: 30%">Nome utente</td>
                                    <td>
                                        <asp:TextBox runat="server" ID="txtUsername" Width="240" />
                                        <asp:RequiredFieldValidator ID="txtUsernameRequireValidator" runat="server" ControlToValidate="txtUsername"
                                            ErrorMessage="Il campo Nome Utente è Obbligatorio" Display="Dynamic"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="label" style="width: 30%">Password</td>
                                    <td>
                                        <asp:TextBox runat="server" ID="txtPassword" Width="240" TextMode="Password" />
                                        <asp:RequiredFieldValidator ID="txtPasswordRequireValidator" runat="server" ControlToValidate="txtPassword"
                                            ErrorMessage="Il campo Password è Obbligatorio" Display="Dynamic" ></asp:RequiredFieldValidator>
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
                                            InitialValue=""></asp:RequiredFieldValidator>
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
                                        <asp:TextBox runat="server" ID="txtINServerName" Width="240"/>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="label" style="width: 30%">IN: Porta</td>
                                    <td>
                                        <asp:TextBox runat="server" ID="txtINPort" Width="240"/>
                                        <asp:RequiredFieldValidator ID="txtINPortRequireValidator" runat="server" ControlToValidate="txtINPort"
                                            ErrorMessage="Il campo IN: Porta è Obbligatorio" Display="Dynamic" ></asp:RequiredFieldValidator>
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
                                        <asp:TextBox runat="server" ID="txtOUTServerName" Width="240"/>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="label" style="width: 30%">OUT: Porta</td>
                                    <td>
                                        <asp:TextBox runat="server" ID="txtOUTPort" Width="240"/>
                                        <asp:RequiredFieldValidator ID="txtOUTPortRequireValidator" runat="server" ControlToValidate="txtOUTPort"
                                            ErrorMessage="Il campo OUT: Porta è Obbligatorio" Display="Dynamic"></asp:RequiredFieldValidator>
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
                                    <td class="label" style="width: 30%">Marca casella "in errore"</td>
                                    <td>
                                        <asp:CheckBox runat="server" ID="chkLoginError" /></td>
                                </tr>
                                <tr>
                                    <td></td>
                                </tr>
                            </table>
                        </asp:Panel>
                        <asp:Panel runat="server">
                            <telerik:RadButton ID="btnSave" runat="server" Text="Salva" Width="150" AutoPostBack="false" />
                        </asp:Panel>
                    </ContentTemplate>
                </telerik:RadPanelItem>
            </Items>
        </telerik:RadPanelBar>
    </div>
</asp:Panel>

<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
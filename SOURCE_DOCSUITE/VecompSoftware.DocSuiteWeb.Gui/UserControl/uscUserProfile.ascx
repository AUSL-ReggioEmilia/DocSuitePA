<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscUserProfile.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscUserProfile" %>

<%@ Register Src="~/UserControl/uscProtRicercaPreview.ascx" TagName="uscProtRicerca" TagPrefix="usc" %>

<asp:Panel runat="server" ID="WarningPanel" CssClass="hiddenField">
    <asp:Label ID="WarningLabel" runat="server" />
</asp:Panel>
<telerik:RadCodeBlock runat="server" ID="RadCodeBlock1">
    <script type="text/javascript">
        function changeCompanyDisplay(makeReload) {
            alert("Dati salvati correttamente");
            if (makeReload === "True") {
                parent.location.reload();
            }
        }

        function rlbSignType_SelectedIndexChanged(sender, args) {
            var selectedValue = sender._selectedValue;
            toggleSignBoxes(selectedValue);
        }

        function rlbSignType_OnLoad(sender) {
            var selectedValue = sender._selectedValue;
            toggleSignBoxes(selectedValue);
        }

        function toggleSignBoxes(selectedValue) {
            switch (selectedValue) {
                case "0":
                    $('#<%= ClientID %>_aliasContainer').hide();
                    $('#<%= ClientID %>_pinContainer').hide();
                    $('#<%= ClientID %>_passwordContainer').hide();
                    $('#<%= ClientID %>_saveTypeContainer').hide();
                    $('#<%= ClientID %>_arubaAutoContainer').hide();
                    $('#<%= ClientID %>_OTPTypeContainer').hide();
                    break;
                case "1":
                    $('#<%= ClientID %>_aliasContainer').show();
                    $('#<%= ClientID %>_pinContainer').show();
                    $('#<%= ClientID %>_passwordContainer').hide();
                    $('#<%= ClientID %>_arubaAutoContainer').show();
                    $('#<%= ClientID %>_OTPTypeContainer').hide();
                    break;
                case "2":
                    $('#<%= ClientID %>_aliasContainer').show();
                    $('#<%= ClientID %>_pinContainer').show();
                    $('#<%= ClientID %>_passwordContainer').hide();
                    $('#<%= ClientID %>_arubaAutoContainer').hide();
                    $('#<%= ClientID %>_OTPTypeContainer').show();
                    break;
                case "3":
                    $('#<%= ClientID %>_aliasContainer').hide();
                    $('#<%= ClientID %>_pinContainer').hide();
                    $('#<%= ClientID %>_passwordContainer').show();
                    $('#<%= ClientID %>_arubaAutoContainer').show();
                    $('#<%= ClientID %>_OTPTypeContainer').hide();
                    break;

                case "4":
                    $('#<%= ClientID %>_aliasContainer').show();
                    $('#<%= ClientID %>_pinContainer').hide();
                    $('#<%= ClientID %>_passwordContainer').show();
                    $('#<%= ClientID %>_arubaAutoContainer').hide();
                    $('#<%= ClientID %>_OTPTypeContainer').hide();
                    break;
                default:
            }
        }

        function saveToSession(value) {
            localStorage.setItem("DocumentSignPassword", value);
        }
    </script>
</telerik:RadCodeBlock>

<style>
    
.localSplitterWrapper {
    width: 100% !important;
    height: 95%;
    overflow: hidden;
}

</style>

<div class="localSplitterWrapper">
    <telerik:RadSplitter runat="server" Width="100%" ResizeWithParentPane="False" Height="100%" Orientation="Horizontal">
        <telerik:RadPane runat="server" Width="100%" Height="40px" Scrolling="None">
            <telerik:RadTabStrip runat="server" RenderMode="Lightweight" ID="radTabStrip" MultiPageID="radMultiPage" SelectedIndex="0" Skin="Silk">
                <Tabs>
                    <telerik:RadTab Text="Configurazione Utente" Value="UserConfigurationTab" Width="200px"></telerik:RadTab>
                    <telerik:RadTab Text="Configurazione ricerca Protocollo" Value="AdaptiveProtSearchConfigurationTab" Width="250px"></telerik:RadTab>
                </Tabs>
            </telerik:RadTabStrip>
        </telerik:RadPane>
        <telerik:RadPane runat="server" Width="100%" Height="100%" Scrolling="None">
            <telerik:RadMultiPage runat="server" ID="radMultiPage" SelectedIndex="0" Width="100%" Height="100%">
                <telerik:RadPageView runat="server" ID="RadPageView1">
                    <asp:Panel runat="server" ID="pnlConsoleUserContainer">
                        <table class="datatable">
                            <tr>
                                <th colspan="2">Dati Generali</th>
                            </tr>
                            <tr>
                                <td class="label" style="width: 30%;">Nome utente:
                                </td>
                                <td>
                                    <asp:Label runat="server" ID="lblUser" />
                                </td>
                            </tr>
                            <tr>
                                <td class="label" style="width: 30%;">Indirizzo e-mail:
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtEmail" Width="300px" />
                                    <telerik:RadButton ID="btnRetrieveADEmail" runat="server" Text="Recupera da AD" CausesValidation="false" />
                                    <asp:RegularExpressionValidator ID="regexpMail" runat="server"
                                        ValidationGroup="consoleUserValidationGroup"
                                        ErrorMessage="Inserire una mail valida."
                                        ControlToValidate="txtEmail"
                                        ValidationExpression="^[a-zA-Z][\w\.-]*[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$" />
                                </td>
                            </tr>
                            <tr runat="server" id="trMultiTenant">
                                <td class="label" style="width: 30%;">Seleziona AOO di lavoro:
                                </td>
                                <td>
                                    <telerik:RadDropDownList runat="server" ID="rlbSelectCompany" Width="300px" AutoPostBack="true" selected="true" DropDownHeight="200px" OnItemSelected="ChangeCompany" Visible="false">
                                    </telerik:RadDropDownList>
                                </td>
                            </tr>                         
                            <tr runat="server" id="trFiscalCode" visible="false">
                                <td class="label" style="width:30%;">Codice fiscale:</td>
                                <td>
                                    <telerik:RadTextBox runat="server" ID="txtFiscalCode" Width="300px" AutoPostBack="true" />
                                </td>
                            </tr>
                        </table>

                        <table class="datatable" id="trSignInfo" runat="server">
                            <tr>
                                <th colspan="2">Dati di firma digitale</th>
                            </tr>
                            <tr>
                                <td class="label" style="width: 30%;">Tipo di firma
                                </td>
                                <td>
                                    <telerik:RadDropDownList runat="server" ID="rlbSignType" Width="300px" AutoPostBack="true" selected="true" DropDownHeight="200px" OnClientSelectedIndexChanged="rlbSignType_SelectedIndexChanged" OnClientLoad="rlbSignType_OnLoad">
                                        <Items>
                                            <telerik:DropDownListItem Selected="true" Text="Smartcard" Value="0" />
                                            <telerik:DropDownListItem Text="Firma remota di Aruba" Value="1" />
                                            <telerik:DropDownListItem Text="Firma remota di Infocert" Value="2" />
                                            <telerik:DropDownListItem Text="Firma automatica di Aruba" Value="3" />
                                            <telerik:DropDownListItem Text="Firma automatica di Infocert" Value="4" />
                                        </Items>
                                    </telerik:RadDropDownList>
                                </td>
                            </tr>
                            <tr id="aliasContainer">
                                <td class="label" style="width: 30%;">Alias
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtSignAlias" Width="300px" />

                                </td>
                            </tr>
                            <tr id="passwordContainer">
                                <td class="label" style="width: 30%;">Pin
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtSignPassword" Width="300px" TextMode="Password" />

                                </td>
                            </tr>
                            <tr id="pinContainer">
                                <td class="label" style="width: 30%;">Pin
                                </td>
                                <td>
                                    <asp:TextBox runat="server" ID="txtSignPin" Width="100px" TextMode="Password" />
                                </td>
                            </tr>
                            <tr id="saveTypeContainer">
                                <td class="label" style="width: 30%;">Modalità di salvataggio
                                </td>
                                <td>
                                    <telerik:RadDropDownList runat="server" ID="rlbSignSaveModality" Width="300px" AutoPostBack="true" DropDownHeight="200px">
                                        <Items>
                                            <telerik:DropDownListItem Selected="true" Text="Memorizza nel profilo utente" Value="2" />
                                            <telerik:DropDownListItem Text="Salva in sessione" Value="1" />
                                            <telerik:DropDownListItem Text="Dimenticare" Value="0" />
                                        </Items>
                                    </telerik:RadDropDownList>
                                </td>
                            </tr>
                            <tr runat="server" id="OTPTypeContainer">
                                <td class="label" style="width: 30%;">Tipo di OTP
                                </td>
                                <td>
                                    <telerik:RadDropDownList runat="server" ID="rlbOTPType" Width="300px" AutoPostBack="true" selected="true" DropDownHeight="40px">
                                        <Items>
                                            <telerik:DropDownListItem Selected="true" Text="Automatico" Value="0" />
                                            <telerik:DropDownListItem Text="Manuale" Value="1" />
                                        </Items>
                                    </telerik:RadDropDownList>
                                </td>
                            </tr>   
                            <tr id="arubaAutoContainer">
                                <td class="label" style="width: 30%;">Dati dinamici di aruba:
                                </td>
                                <td>
                                    <table class="datatable">
                                        <tr>
                                            <td class="label" style="width: 11%;">Delegated Domain:
                                            </td>
                                            <td>
                                                <asp:TextBox runat="server" ID="txtArubaDelegatedDomain" Width="172px" />

                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="label" style="width: 11%;">Delegated User:
                                            </td>
                                            <td>
                                                <asp:TextBox runat="server" ID="txtArubaDelegatedUser" Width="172px" />

                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="label" style="width: 11%;">OTP Password:
                                            </td>
                                            <td>
                                                <asp:TextBox runat="server" ID="txtArubaOTPPassword" Width="172px" />

                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="label" style="width: 11%;">OTP AuthType:
                                            </td>
                                            <td>
                                                <asp:TextBox runat="server" ID="txtArubaOTPAuthType" Width="172px" />

                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="label" style="width: 11%;">User:
                                            </td>
                                            <td>
                                                <asp:TextBox runat="server" ID="txtArubaUser" Width="172px" />

                                            </td>
                                        </tr>
                                        <tr>
                                            <td class="label" style="width: 11%;">Certificate Id:
                                            </td>
                                            <td>
                                                <asp:TextBox runat="server" ID="txtArubaCertificateId" Width="172px" />

                                            </td>
                                        </tr>
                                    </table>
                                </td>
                            </tr>
                            <tr id="defaultContainer">
                                <td class="label" style="width: 30%;">Predefinito:
                                </td>
                                <td>
                                    <asp:CheckBox runat="server" ID="cbSignDefault" />
                                </td>
                            </tr>
                        </table>

                        <telerik:RadAjaxPanel runat="server" ID="pnlMobilePhone" Visible="false">
                            <table class="datatable">
                                <tr>
                                    <th colspan="2">Configurazione numero di cellulare</th>
                                </tr>
                                <tr>
                                    <td class="label" style="width: 30%;">Numero di Cellulare</td>
                                    <td>+39<telerik:RadTextBox MaxLength="15" runat="server" ID="txtMobilePhone"></telerik:RadTextBox>
                                        <p>
                                            <asp:RegularExpressionValidator runat="server" ID="mobilePhoneValidator" Display="Dynamic" ValidationGroup="consoleUserValidationGroup"
                                                ErrorMessage="Inserire un numero di cellulare valido."
                                                ControlToValidate="txtMobilePhone"
                                                ValidationExpression="^\d+$" />
                                        </p>
                                        <p>
                                            <asp:RegularExpressionValidator runat="server" ID="mobilePhoneRangeValidator" Display="Dynamic" ValidationGroup="consoleUserValidationGroup"
                                                ErrorMessage="Il numero di telefono deve essere compreso tra 6 e 15 caratteri."
                                                ControlToValidate="txtMobilePhone"
                                                ValidationExpression="\s*[0-9]{6,15}\s*" />
                                        </p>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="label" style="width: 30%;"></td>
                                    <td>Attenzione! Il numero di cellulare dovrà essere inserito SENZA il prefisso internazionale (es. 0039 o +39)
                                    </td>
                                </tr>
                            </table>
                        </telerik:RadAjaxPanel>

                        <asp:Panel runat="server" ID="pnlPasswordChange" Visible="false">
                            <table class="datatable">
                                <tr>
                                    <th colspan="2">Modifica Password utenza di dominio</th>
                                </tr>
                                <tr>
                                    <td class="label" style="width: 30%;">Password Attuale</td>
                                    <td>
                                        <asp:TextBox runat="server" TextMode="Password" ID="txtOldPassword" />
                                        <p>
                                            <asp:RequiredFieldValidator runat="server" ID="txtOldPasswordValidator" Display="Dynamic" ValidationGroup="changePassordValidatorGroup"
                                                ErrorMessage="Campo Password Attuale obbligatorio."
                                                ControlToValidate="txtOldPassword" />
                                        </p>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="label" style="width: 30%;">Nuova Password</td>
                                    <td>
                                        <asp:TextBox runat="server" TextMode="Password" ID="txtNewPassword" />
                                        <p>
                                            <asp:RequiredFieldValidator runat="server" ID="txtNewPasswordValidator" Display="Dynamic" ValidationGroup="changePassordValidatorGroup"
                                                ErrorMessage="Campo Nuova Password obbligatorio."
                                                ControlToValidate="txtNewPassword" />
                                        </p>
                                        <p>
                                            <asp:CompareValidator runat="server" ID="txtPasswordCompareValidator" Display="Dynamic" ValidationGroup="changePassordValidatorGroup"
                                                ErrorMessage="La Password Attuale e la Nuova Password non devono coincidere"
                                                ControlToCompare="txtOldPassword" ControlToValidate="txtNewPassword" Operator="NotEqual" />
                                        </p>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="label" style="width: 30%;">Conferma Nuova Password</td>
                                    <td>
                                        <asp:TextBox runat="server" TextMode="Password" ID="txtConfirmNewPassword" />
                                        <p>
                                            <asp:RequiredFieldValidator runat="server" ID="txtConfirmNewPasswordValidator" Display="Dynamic" ValidationGroup="changePassordValidatorGroup"
                                                ErrorMessage="Campo Conferma Nuova Password obbligatorio."
                                                ControlToValidate="txtConfirmNewPassword" />
                                        </p>
                                        <p>
                                            <asp:CompareValidator runat="server" ID="txtNewPasswordCompareValidator" Display="Dynamic" ValidationGroup="changePassordValidatorGroup"
                                                ErrorMessage="La Nuova Password e la Conferma Nuova Password devono coincidere"
                                                ControlToCompare="txtNewPassword" ControlToValidate="txtConfirmNewPassword" />
                                        </p>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="label" style="width: 30%;"></td>
                                    <td>Attenzione! Una volta modificata la password è necessario eseguire nuovamente l'accesso al PC.</td>
                                </tr>
                                <tr>
                                    <td class="label" style="width: 30%;"></td>
                                    <td>
                                        <asp:Button runat="server" ID="btnChangePassword" Text="Conferma" ValidationGroup="changePassordValidatorGroup" /></td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </asp:Panel>
                </telerik:RadPageView>
                <telerik:RadPageView runat="server" ID="viewAdaptiveSearchConfiguration" Width="100%" Height="100%">
                    <telerik:RadSplitter ID="adaptiveSearchConfigurationSplitter" runat="server" Width="100%" ResizeWithParentPane="False"
                        Orientation="Vertical" Height="100%">
                        <telerik:RadPane runat="server" ID="paneControls" Width="30%" Height="100%" Scrolling="Y">
                            <telerik:RadTreeView runat="server" ID="treeControls" DataValueField="Id" DataTextField="Label" CheckBoxes="true"></telerik:RadTreeView>
                        </telerik:RadPane>
                        <telerik:RadSplitBar runat="server" />
                        <telerik:RadPane runat="server" ID="panePreview" Width="70%" Height="100%">
                            <usc:uscProtRicerca runat="server" Type="Prot" ID="uscProtRicerca"></usc:uscProtRicerca>
                        </telerik:RadPane>
                    </telerik:RadSplitter>
                </telerik:RadPageView>
            </telerik:RadMultiPage>
        </telerik:RadPane>
    </telerik:RadSplitter>
</div>

<telerik:RadButton ID="btnSalva" Text="Salva" ValidationGroup="consoleUserValidationGroup" CausesValidation="True" Width="100px" runat="server" Style="margin-left: 3px;" />

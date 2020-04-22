<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscContattiGes.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscContattiGes" %>

<%@ Register Src="~/UserControl/uscSettori.ascx" TagName="Settori" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
    <script type="text/javascript">
        function ReturnValuesJSon(action, idContact, contact, close) {
            if ($.parseJSON(close.toLowerCase())) {
                var contactGes = new Object();
                contactGes.Action = action;
                contactGes.IdContact = idContact;
                contactGes.Contact = contact;

                GetRadWindow().close(contactGes);
            } else {
                GetRadWindow().BrowserWindow.<%= CallerId%>_UpdateManual(contact, action);
            }
        }

        /*Se l'utente seleziona una data pregressa, non permetto di salvare! */
        function dateSelected(sender, eventArgs) {
            var datePicker = $find("<%= txtHistoryDate.ClientID %>");
            var dateTimePicker = datePicker.get_dateInput().get_selectedDate();
            var dateTimeNow = new Date();
            dateTimePicker.setHours(0, 0, 0, 0);
            dateTimeNow.setHours(0, 0, 0, 0);

            if (dateTimePicker >= dateTimeNow) {
                $("#<%= btnConferma.ClientID %>").attr('disabled', 'disabled');
                $("#<%= lbErrorHistoryDate.ClientID %>").show();
            }
            else {
                $("#<%= btnConferma.ClientID %>").removeAttr('disabled');
                $("#<%= lbErrorHistoryDate.ClientID %>").hide();
            }

        }

        function OnClientClicking(sender, args) {
                var windowManager = $find("<%= RadWindowManagerContattiGes.ClientID %>");
                var actionName = "<%= BasePage.Action.ToLower() %>";
                switch (actionName) {
                    case "recovery":
                    case "del":
                        var callBackFunction = Function.createDelegate(sender, function (shouldSubmit) {
                            if (shouldSubmit) {
                                if (actionName == "recovery") {
                                    sender.set_commandArgument('activateAllChildren');
                                } else {
                                    sender.set_commandArgument('disableAllChildren');
                                }
                            }
                            this.click();
                        });
                        windowManager.radconfirm("Vuoi che l'operazione sia gestita anche su tutti i figli del contatto selezionato?", callBackFunction, 300, 100, null, "Gestione");
                        args.set_cancel(true);
                        break;
                    case "clone":
                        var cloneCallBackFunction = Function.createDelegate(sender, function (shouldSubmit) {
                            if (shouldSubmit) {
                                this.click();
                            }
                        });
                        windowManager.radconfirm("Vuoi procedere a clonare massivamente il contatto selezionato?", cloneCallBackFunction, 300, 100, null, "Clona");
                        args.set_cancel(true);
                        break;
                }
            }

    </script>
    <style>
        table.datatable {
            background-color: #ffffff !important;
        }
    </style>
</telerik:RadScriptBlock>


 <telerik:RadAjaxLoadingPanel ID="DefaultLoadingPanel" runat="server">
 </telerik:RadAjaxLoadingPanel>

<telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerContattiGes" runat="server" />
<table class="datatable" style="height: 100%; margin-bottom: 0;">
    <tr id="trTrvContatto" class="Scuro" runat="server" style="height: 20px;">
        <td style="background-color: #ffffff">
            <telerik:RadTreeView Height="100%" ID="Tvw" runat="server" Width="100%" />
        </td>
    </tr>
    <tr id="pnlEditType" runat="server">
        <td style="height: 35px; background-color: #ffffff">
            <telerik:RadToolBar AutoPostBack="true" CssClass="ToolBarContainer" EnableRoundedCorners="False" EnableShadows="False" ID="ToolBar" runat="server" Width="100%">
                <Items>
                    <telerik:RadToolBarButton CausesValidation="False">
                        <ItemTemplate>
                            <span class="templateText">Tipologia Contatto:</span>
                        </ItemTemplate>
                    </telerik:RadToolBarButton>
                </Items>
            </telerik:RadToolBar>
        </td>
    </tr>
    <tr>
        <td style="background-color: #ffffff">
            <asp:Panel ID="pnlAggiungi" runat="server">
                <table class="dataform">
                    <asp:Panel ID="pnlPersona" runat="server">
                        <tr>
                            <td class="label" style="width: 30%;">Cognome/Nome:</td>
                            <td>
                                <div style="width: 50%; float: left;">
                                    <telerik:RadTextBox ID="txtLastName" MaxLength="30" runat="server" Width="100%" />
                                    <asp:RequiredFieldValidator ControlToValidate="txtLastName" Display="Dynamic" ErrorMessage="Campo Cognome Obbligatorio" ID="RequiredFieldValidator1" runat="server" />
                                </div>
                                <div style="width: 50%; float: left;">
                                    <telerik:RadTextBox ID="txtFirstName" MaxLength="30" runat="server" Width="100%" />
                                </div>
                            </td>
                        </tr>
                        <tr id="trBirthDay" runat="server">
                            <td class="label" style="width: 30%;">Data di nascita:</td>
                            <td>
                                <telerik:RadDatePicker ID="txtBirthDate" runat="server" />
                            </td>
                        </tr>
                        <asp:Panel ID="pnlBirthPlace" runat="server" Visible="false">
                        <tr>
                            <td class="label" style="width: 30%;">Luogo di nascita:</td>
                            <td>
                                <telerik:RadTextBox ID="txtBirthPlace" MaxLength="250" runat="server" Width="100%" />
                                
                            </td>
                        </tr>
                        </asp:Panel>
                        <asp:Panel ID="pnlTitoliStudio" runat="server">
                            <tr>
                                <td class="label" style="width: 30%;">Titolo di studio:</td>
                                <td>
                                    <asp:DropDownList AppendDataBoundItems="true" DataSourceID="ObjectDataSourceContactTitle" ID="ddlTitoliStudio" runat="server">
                                        <asp:ListItem Text="" Value="" />
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </asp:Panel>
                    </asp:Panel>
                    <asp:Panel ID="pnlDescrizione" runat="server">
                        <tr>
                            <td class="label" style="width: 30%;">Descrizione:</td>
                            <td>
                                <telerik:RadTextBox ID="txtDescription" MaxLength="250" runat="server" Width="100%" />
                                <asp:RequiredFieldValidator ControlToValidate="txtDescription" Display="Dynamic" ErrorMessage="Campo Descrizione Obbligatorio" ID="rfvDescription" runat="server" />
                            </td>
                        </tr>
                    </asp:Panel>
                    <asp:Panel ID="pnlHistory" runat="server" Visible="false">
                        <tr>
                            <td class="label" style="width: 30%;">Data storicizzazione:</td>
                            <td>
                                <telerik:RadDatePicker ID="txtHistoryDate" Width="30%" runat="server" enable="false" Visible="false">
                                    <ClientEvents OnDateSelected="dateSelected" />
                                </telerik:RadDatePicker>
                                <asp:Label runat="server" CssClass="hiddenField" Font-Bold="True" ForeColor="Red" Text="Data di storicizzazione posteriore a oggi" ID="lbErrorHistoryDate" />
                            </td>
                        </tr>
                    </asp:Panel>
                    <asp:Panel ID="pnlCertifiedMail" runat="server" Visible="False" BorderWidth="0">
                        <tr>
                            <td class="label" style="width: 30%;">
                                <asp:Label ID="lblCertifiedMail" runat="server" />
                            </td>
                            <td style="width: 70%;">
                                <telerik:RadTextBox ID="txtCertifiedMail" MaxLength="250" runat="server" Width="100%" />
                                <asp:RequiredFieldValidator ControlToValidate="txtCertifiedMail" Display="Dynamic" ErrorMessage="Indirizzo di invio obbligatorio" ID="rfvCertifiedMail" runat="server" />
                            </td>
                        </tr>
                    </asp:Panel>
                    <asp:Panel ID="pnlSDIIdentification" runat="server" Visible="False" BorderWidth="0">
                        <tr>
                            <td class="label" style="width: 30%;">
                                <asp:Label ID="lblSDIIdentification" runat="server" />
                            </td>
                            <td style="width: 70%;">
                                <telerik:RadTextBox ID="txtSDIIdentification" MaxLength="250" runat="server" Width="100%" />
                            </td>
                        </tr>
                    </asp:Panel>
                    <asp:Panel ID="pnlContactRubrica" runat="server">
                        <tr>
                            <td class="label" style="width: 30%;">Bloccato:</td>
                            <td style="width: 70%;">
                                <asp:CheckBox Checked="False" ID="chkLocked" runat="server" />
                                &nbsp;&nbsp;&nbsp;&nbsp;
                                    <asp:Label EnableViewState="False" Font-Bold="True" ForeColor="Red" ID="lblChildren" runat="server" Text="Propaga ai figli: " />
                                <asp:CheckBox Checked="False" ID="chkChildren" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label" style="width: 30%;">Codice AOO:</td>
                            <td style="width: 70%;">
                                <asp:TextBox ID="txtCode" MaxLength="8" runat="server" Width="100px" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label" style="width: 30%;">Codice ricerca:</td>
                            <td style="width: 70%;">
                                <asp:TextBox ID="txtSearchCode" MaxLength="255" runat="server" Width="250px" />
                            </td>
                        </tr>
                    </asp:Panel>
                    <asp:Panel ID="pnlFiscalCode" runat="server" Visible="False" BorderWidth="0">
                        <tr>
                            <td class="label" style="width: 30%;">Cod. Fisc./P. IVA:</td>
                            <td style="width: 70%;">
                                <asp:TextBox ID="txtFiscalCode" MaxLength="16" runat="server" Width="150px" />
                                <asp:RequiredFieldValidator ControlToValidate="txtFiscalCode" Display="Dynamic" Enabled="false" ErrorMessage="Campo Cod. Fisc./P. IVA Obbligatorio" ID="rfvFiscalCode" runat="server" />
                            </td>
                        </tr>
                    </asp:Panel>
                    <asp:Panel ID="pnlSimpleMode" runat="server">
                        <tr>
                            <td class="label" style="width: 30%;">Tipo:</td>
                            <td style="width: 70%;">
                                <asp:DropDownList AppendDataBoundItems="true" DataSourceID="ObjectDataSourcePlaceName" ID="ddlPlaceName" runat="server">
                                    <asp:ListItem Text="" Value="" />
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="label" style="width: 30%;">Indirizzo:</td>
                            <td style="width: 70%;">
                                <telerik:RadTextBox ID="txtAddress" MaxLength="256" runat="server" Width="100%" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label" style="width: 30%;">Numero:</td>
                            <td style="width: 70%;">
                                <asp:TextBox ID="txtCivicNumber" MaxLength="10" runat="server" Width="50px" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label" style="width: 30%;">CAP:</td>
                            <td style="width: 70%;">
                                <asp:TextBox ID="txtZipCode" MaxLength="20" runat="server" Width="100px" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label" style="width: 30%;">Città:</td>
                            <td style="width: 70%;">
                                <asp:TextBox ID="txtCity" MaxLength="50" runat="server" Width="250px" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label" style="width: 30%;">Provincia:</td>
                            <td style="width: 70%;">
                                <asp:TextBox ID="txtCityCode" MaxLength="2" runat="server" Width="50px" />
                            </td>
                        </tr>
                        <asp:Panel ID="pnlNationality" runat="server" BorderWidth="0">
                            <tr>
                                <td class="label" style="width: 30%;">Nazionalità:</td>
                                <td style="width: 70%;">
                                    <asp:TextBox ID="txtNationality" runat="server" Width="250px" />
                                </td>
                            </tr>
                            <tr>
                                <td class="label" style="width: 30%;">Lingua:</td>
                                <td>
                                    <asp:DropDownList ID="ddlLanguageType" runat="server">
                                        <asp:ListItem Text="" Value="" />
                                    </asp:DropDownList>
                                </td>
                            </tr>
                        </asp:Panel>
                        <tr>
                            <td class="label" style="width: 30%;">Telefono:</td>
                            <td style="width: 70%;">
                                <asp:TextBox ID="txtTelephoneNumber" MaxLength="50" runat="server" Width="250px" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label" style="width: 30%;">Fax:</td>
                            <td style="width: 70%;">
                                <asp:TextBox ID="txtFaxNumber" MaxLength="50" runat="server" Width="250px" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label" style="width: 30%;">Posta elettronica:</td>
                            <td style="width: 70%;">
                                <telerik:RadTextBox ID="txtEMailAddress" MaxLength="250" runat="server" Width="100%" />
                            </td>
                        </tr>
                        <tr>
                            <td class="label" style="width: 30%;">
                                <asp:Label ID="lblNote" runat="server" />:
                            </td>
                            <td style="width: 70%;">
                                <telerik:RadTextBox ID="txtNote" MaxLength="256" runat="server" Width="100%" />
                            </td>
                        </tr>
                    </asp:Panel>
                    <asp:Panel ID="pnlRole" runat="server" BorderWidth="0">
                        <tr>
                            <td class="label" style="width: 30%;">
                                <asp:Label Font-Bold="true" ID="lblSettoreAutorizzato" runat="server" Text="Settore Autorizzato" />:
                            </td>
                            <td style="width: 70%;">
                                <usc:Settori Caption="Autorizzazione" HeaderVisible="False" ID="uscAutorizza" MultipleRoles="false" MultiSelect="false" Required="false" runat="server" />
                            </td>
                        </tr>
                        <tr id="trSettoreRubrica" runat="server">
                            <td class="label" style="width: 30%;">Settore Rubrica:</td>
                            <td style="width: 70%;">
                                <usc:Settori Caption="Settore Rubrica" HeaderVisible="False" ID="uscSettoreRubrica" MultipleRoles="false" MultiSelect="false" Required="false" runat="server" />
                            </td>
                        </tr>
                    </asp:Panel>
                </table>
            </asp:Panel>
        </td>
    </tr>
    <tr id="pnlFooter" runat="server">
        <td style="background-color: #ffffff; vertical-align: bottom">
            <telerik:RadButton ID="btnConferma" runat="server" Text="Conferma" OnClientClicking="OnClientClicking" />
            <telerik:RadButton ID="btnConfermaNuovo" runat="server" Text="Conferma e Nuovo" Visible="False" OnClientClicking="OnClientClicking" />
            &nbsp;
            <asp:ValidationSummary DisplayMode="List" ID="ValidationSummary1" runat="server" ShowMessageBox="True" ShowSummary="False" />
        </td>
    </tr>
</table>
<asp:ObjectDataSource ID="ObjectDataSourcePlaceName" OldValuesParameterFormatString="original_{0}" runat="server" SelectMethod="GetAll" TypeName="VecompSoftware.DocSuiteWeb.Facade.ContactPlaceNameFacade" />
<asp:ObjectDataSource ID="ObjectDataSourceContactTitle" OldValuesParameterFormatString="original_{0}" runat="server" SelectMethod="GetAll" TypeName="VecompSoftware.DocSuiteWeb.Facade.ContactTitleFacade" />

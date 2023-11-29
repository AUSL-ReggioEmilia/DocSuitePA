<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ProtInserimento.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtInserimento" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Protocollo - Inserimento" %>

<%@ Register Src="~/UserControl/uscDocumentUpload.ascx" TagPrefix="usc" TagName="UploadDocument" %>
<%@ Register Src="~/UserControl/uscClassificatore.ascx" TagPrefix="usc" TagName="SelCategory" %>
<%@ Register Src="~/UserControl/uscSettori.ascx" TagPrefix="usc" TagName="Settori" %>
<%@ Register Src="~/UserControl/uscContattiSel.ascx" TagPrefix="usc" TagName="SelContatti" %>
<%@ Register Src="~/UserControl/uscOggetto.ascx" TagPrefix="usc" TagName="SelOggetto" %>
<%@ Register Src="~/UserControl/uscServiceCategory.ascx" TagPrefix="usc" TagName="SelServiceCategory" %>
<%@ Register Src="~/UserControl/uscContattiSelText.ascx" TagPrefix="usc" TagName="SelContattiTesto" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">

    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
        <script type="text/javascript" language="javascript">
            function OnRequestStart(sender, args) {
                document.getElementById('btnInserimentoV').disabled = true;
                ShowLoadingPanel();
            }


            function OnResponseEnd(sender, args) {
                document.getElementById('btnInserimentoV').disabled = false;
                HideLoadingPanel();
            }

            function btnInserimentoConfirmClick() {
                var hf_checkProtocolInMetadatas = $get("<%= hf_checkProtocolInMetadatas.ClientID %>");
                hf_checkProtocolInMetadatas.value = "0";
                btnInserimentoVClick();
            }

            function btnInserimentoVClick() {
                document.getElementById('btnInserimentoV').click();
            }

            function btnSelectProtocolV() {
                document.getElementById('<%= btnSelectProtocol.ClientID %>').click();
            }

            function Confirm(message, axajRequest) {
                if (confirm(message)) {
                    var ajaxManager = $find("<%= AjaxManager.ClientID %>");
                    ajaxManager.ajaxRequest(axajRequest);
                } else {
                    return false;
                }
                return true;
            }

            function <%= Me.ID %>_OpenWindow(name, width, height, parameters) {
                if (HasDocument()) parameters += '&Doc=1';

                var url = "../Prot/ProtSelezione.aspx";
                url += "?" + parameters;
                var manager = $find("<%=RadWindowManagerProtocollo.ClientID %>");
                var wnd = manager.open(url, name);
                wnd.setSize(width, height);
                wnd.center();

                return false;
            }

            function HasDocument() {
                try {
                    var twDoc = $find("<%= uscUploadDocumenti.TreeViewControl.ClientID%>");
                    var nodes = twDoc.get_nodes();

                    if (nodes.get_count() < 1)
                        return false;

                    nodes = nodes.getItem(0).get_nodes();
                    return nodes.get_count() >= 1;
                } catch (ex) {
                    return false;
                }
            }

            function SetRecoveryProtocol(v) {
                document.getElementById('<%= txtProtRecovery.ClientID %>').value = v;
            }

            function <%= Me.ID %>_CloseFunction(sender, args) {
                sender.remove_close(<%= Me.ID %>_CloseFunction);
                if (args.get_argument() !== null) {
                    var ajaxManager = $find("<%= AjaxManager.ClientID %>");
                    ajaxManager.ajaxRequest("Recovery" + "|" + args.get_argument());
                }
            }

            function <%= Me.ID %>_CloseContactFunction(sender, args) {
                sender.remove_close(<%= Me.ID %>_CloseContactFunction);
                if (args.get_argument() !== null) {
                    var ajaxManager = $find("<%= AjaxManager.ClientID %>");
                    ajaxManager.ajaxRequest("TextMode" + "|" + args.get_argument());
                }
            }
            function ShowLoadingPanel() {
                var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                var currentUpdatedControl = "<%= protocolContainer.ClientID%>";
                currentLoadingPanel.show(currentUpdatedControl);
            }

            function HideLoadingPanel() {
                var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                var currentUpdatedControl = "<%= protocolContainer.ClientID%>";
                currentLoadingPanel.hide(currentUpdatedControl);
            }

            function RedirectToPage(url) {
                window.location = url;
            }

            function BindingFromWorkflowUI() {
                ShowLoadingPanel();
                var workflowReferenceModel = sessionStorage.getItem("WorkflowReferenceModel");
                var workflowStartModel = sessionStorage.getItem("WorkflowStartModel");
                var fascicleFolderSelected = sessionStorage.getItem("SelectedFascicleFolderId");
                if (workflowReferenceModel && workflowStartModel) {
                    var ajaxManager = $find("<%= AjaxManager.ClientID%>");
                    var ajaxModel = {};
                    ajaxModel.Value = [];
                    ajaxModel.ActionName = "BindingFromWorkflowUI";
                    ajaxModel.Value.push(workflowReferenceModel);
                    ajaxModel.Value.push(workflowStartModel);
                    ajaxModel.Value.push(fascicleFolderSelected);
                    ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
                }
            }
        </script>
    </telerik:RadScriptBlock>

    <telerik:RadWindowManager EnableViewState="false" ID="RadWindowManagerProtocollo" runat="server">
        <Windows>
            <telerik:RadWindow ID="windowSelProtocollo" ReloadOnShow="false" runat="server" Title="Selezione Oggetto" />
            <telerik:RadWindow ID="windowSelContact" ReloadOnShow="false" runat="server" Title="Selezione Contatto" />
        </Windows>
    </telerik:RadWindowManager>

    <asp:TextBox AutoPostBack="True" ID="txtProtDate" runat="server" Width="16px" />
    <asp:TextBox AutoPostBack="True" ID="txtIdContact" runat="server" Width="16px" />
    <asp:TextBox ID="txtIdContactNew" runat="server" Width="16px" AutoPostBack="True" />

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <asp:HiddenField ID="hf_checkProtocolInMetadatas" Value="1" runat="server" />
    <%--Container trucco per correggere larghezza della textarea--%>
    <asp:Panel Style="width: 100%" runat="server" ID="protocolContainer">
        <table id="tblProtocollo" class="datatable" visible="false" runat="server">
            <tr>
                <th colspan="6">
                    <asp:Label ID="lbTitle" runat="server" />
                </th>
            </tr>
            <tr>
                <td class="label" style="width: 15%">Anno:
                </td>
                <td style="width: 15%">
                    <b>
                        <%=CurrentProtocol.Year %></b>
                </td>
                <td class="label" style="width: 15%">Numero:
                </td>
                <td style="width: 15%">
                    <b>
                        <%= CurrentProtocol.Number.ToString.PadLeft(7, "0"c)%>
                    </b>
                </td>
                <td class="label" style="width: 15%">Data:
                </td>
                <td style="width: 25%">
                    <b>
                        <%=String.Format("{0:dd/MM/yyyy}",CurrentProtocol.RegistrationDate) %>
                    </b>
                </td>
            </tr>
        </table>
        <%-- Sezione Note destinazione PEC --%>
        <asp:Panel ID="pnlDestinationNote" runat="server" Visible="False">
            <table class="datatable">
                <tr>
                    <td>
                        <table class="datatable">
                            <tr>
                                <th colspan="2">Note destinazione da PEC</th>
                            </tr>
                            <tr class="Chiaro">
                                <td style="width: 150px">&nbsp;
                                </td>
                                <td>
                                    <label style="font-weight: bold;">&nbsp;Note destinazione:</label>
                                    <asp:Label ID="lblDestinationNote" runat="server" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </asp:Panel>

        <asp:Panel runat="server" ID="pnlProtocolAlertNote" Visible="False">
            <table class="datatable">
                <tr>
                    <td>
                        <table class="datatable">
                            <tr>
                                <th colspan="2">
                                    <asp:Label runat="server" ID="ProtocolAlertNoteTitle" />
                                </th>
                            </tr>
                            <tr class="Chiaro">
                                <td style="width: 150px">&nbsp;
                                </td>
                                <td>
                                    <asp:Label ID="ProtocolAlertNoteMessage" runat="server" Style="color: Red; font-weight: bold" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <%-- Fine Sezione Note destinazione PEC --%>

        <%-- Sezione Documenti --%>
        <usc:UploadDocument runat="server" ID="uscUploadDocumenti" MultipleDocuments="false" IsDocumentRequired="true" />
        <usc:UploadDocument runat="server" ID="uscUploadAllegati" MultipleDocuments="true" HideScannerMultipleDocumentButton="true" IsDocumentRequired="false" ButtonSharedFolederEnabled="false" />
        <usc:UploadDocument runat="server" ID="uscUploadAnnexes" MultipleDocuments="true" HideScannerMultipleDocumentButton="true" IsDocumentRequired="false" ButtonSharedFolederEnabled="false" Prefix="" />
        <%-- Fine Sezione Documenti --%>

        <%-- Sezione Tipologia Protocollo --%>
        <telerik:RadAjaxPanel runat="server" ID="UpdatePanelProtocollo" ClientEvents-OnRequestStart="OnRequestStart" ClientEvents-OnResponseEnd="OnResponseEnd">
            <table class="datatable">
                <tr style="vertical-align: middle">
                    <th colspan="2">Tipologia del protocollo</th>
                    <th>
                        <asp:Label ID="lblProtocolloMittente" runat="server">Protocollo del Mittente</asp:Label></th>
                </tr>
                <tr class="Chiaro">
                    <td style="width: 150px; vertical-align: middle; font-size: 8pt; text-align: right;">
                        <b>Tipo di protocollo:</b>
                    </td>
                    <td style="vertical-align: middle; font-size: 8pt">
                        <asp:RadioButtonList ID="rblTipoProtocollo" runat="server" AutoPostBack="True" CssClass="autoWidth" Font-Names="Verdana" DataTextField="Description" DataValueField="Id" />
                    </td>
                    <td style="vertical-align: middle; font-size: 8pt;">
                        <asp:Panel ID="PanelProtocollo" runat="server" Style="vertical-align: middle;">
                            <span class="miniLabel">Protocollo:</span>
                            <asp:TextBox ID="txtDocumentProtocol" runat="server" MaxLength="80" Style="vertical-align: middle;" AutoPostBack="True"></asp:TextBox>
                            <span class="miniLabel">Data:</span>
                            <telerik:RadDatePicker ID="txtDocumentDate" runat="server" AutoPostBack="true" />
                        </asp:Panel>
                    </td>
                </tr>
            </table>
        </telerik:RadAjaxPanel>

        <asp:Panel runat="server" ID="pnlTemplateProtocol">
            <table class="datatable">
                <tr>
                    <th colspan="2">Template Protocollo</th>
                </tr>
                <tr class="Chiaro">
                    <td style="padding-left: 150px; padding-top: 5px; padding-bottom: 5px;">
                        <asp:DropDownList ID="ddlTemplateProtocol" runat="server" AutoPostBack="True"></asp:DropDownList>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel ID="pnlContenitore" runat="server">
            <table id="TblContenitore" class="datatable">
                <tr>
                    <th colspan="2">Contenitore</th>
                </tr>
                <tr>
                    <td>
                        <telerik:RadComboBox runat="server" CausesValidation="false"
                            ID="rcbContainer" AutoPostBack="false" EnableLoadOnDemand="true" MarkFirstMatch="true"
                            ItemRequestTimeout="500" Visible="false" Style="margin-left: 150px;" Width="500px">
                        </telerik:RadComboBox>
                        <asp:DropDownList ID="cboIdContainer" runat="server" Style="margin-left: 150px;" AutoPostBack="false" />
                        &nbsp;
                            <asp:RequiredFieldValidator ID="rfvContainer" runat="server" Display="Dynamic" ErrorMessage="Campo contenitore obbligatorio" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <asp:Panel runat="server" ID="pnlProtocolKind">
            <table class="datatable">
                <tr>
                    <th colspan="2">Modello del protocollo</th>
                </tr>
                <tr class="Chiaro">
                    <td style="padding-left: 150px; padding-top: 5px; padding-bottom: 5px;">
                        <asp:DropDownList ID="ddlProtKindList" runat="server" AutoPostBack="False"></asp:DropDownList>
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <%-- Sezione Tipo Documento --%>
        <asp:Panel ID="pnlIdDocType" runat="server">
            <table class="datatable">
                <%-- Sezione Scelta Tipo Documento --%>
                <tr>
                    <th colspan="2">
                        <span style="margin-right: 50px;">Tipologia spedizione</span>
                        <asp:CheckBox ID="cbClaim" Text="Reclamo" TextAlign="Left" runat="server" />
                    </th>
                </tr>
                <tr class="Chiaro">
                    <td style="padding-left: 150px; padding-top: 5px; padding-bottom: 5px;">
                        <asp:DropDownList ID="cboIdDocType" runat="server" AutoPostBack="False">
                            <asp:ListItem Value="" Text="" />
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator ID="rfvIdDocType" runat="server" ControlToValidate="cboIdDocType" ErrorMessage="Campo tipologia spedizione obbligatorio" Display="Dynamic" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <%-- Sezione Scelta Stato Protocollo --%>
        <asp:Panel ID="pnlProtocolStatus" runat="server">
            <table class="datatable">
                <tr>
                    <th colspan="2">Stato del protocollo</th>
                </tr>
                <tr class="Chiaro">
                    <td style="width: 150px">&nbsp;
                    </td>
                    <td>
                        <asp:DropDownList ID="cboProtocolStatus" DataValueField="Id" DataTextField="Description" runat="server" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <%-- Sezione Dati Fatture --%>
        <telerik:RadAjaxPanel runat="server" ID="radAjaxPnlInvoice">
            <asp:Panel ID="pnlInvoice" runat="server">
                <table class="datatable">
                    <tr>
                        <th colspan="2">Dati fattura</th>
                    </tr>
                    <tr class="Chiaro">
                        <td style="width: 146px; text-align: right; vertical-align: middle;">
                            <b>Fattura numero:</b>
                        </td>
                        <td>
                            <asp:TextBox ID="txtInvoiceNumber" runat="server" MaxLength="80" Style="vertical-align: middle;"></asp:TextBox>
                            <asp:CompareValidator ID="cvInvoiceNumber" runat="server" ControlToValidate="txtInvoiceNumber" ErrorMessage="Numero non Valido" Display="Dynamic" Type="Integer" Operator="GreaterThan" ValueToCompare="0"></asp:CompareValidator>
                            <asp:RequiredFieldValidator ID="rfvInvoiceNumber" runat="server" ControlToValidate="txtInvoiceNumber" ErrorMessage="Campo numero obbligatorio" Display="Dynamic"></asp:RequiredFieldValidator>&nbsp;<b>Data:</b>
                            <telerik:RadDatePicker ID="dtpInvoiceDate" runat="server" />
                            <asp:RequiredFieldValidator ID="rfvInvoiceDate" runat="server" ControlToValidate="dtpInvoiceDate" ErrorMessage="Campo data obbligatorio" Display="Dynamic"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                </table>
                <table class="datatable">
                    <tr>
                        <th colspan="2">Dati Contabilità</th>
                    </tr>
                    <telerik:RadAjaxPanel runat="server" ID="radAjaxPnlSectionalType">
                        <asp:Panel ID="pnlSectionalType" runat="server">
                            <tr class="Chiaro">
                                <td style="width: 148px; text-align: right; vertical-align: middle;">
                                    <b>Sezionale:</b>
                                </td>
                                <td>
                                    <asp:DropDownList ID="ddlAccountingSectional" runat="server" />
                                </td>
                            </tr>
                        </asp:Panel>
                    </telerik:RadAjaxPanel>
                    <telerik:RadAjaxPanel runat="server" ID="radAjaxPnlAccounting">
                        <asp:Panel ID="pnlAccounting" runat="server">
                            &nbsp;
                        <tr class="Chiaro">
                            &nbsp;&nbsp; &nbsp;&nbsp;
                            <td style="width: 150px; text-align: right; vertical-align: middle;">
                                <b>Data:</b>
                            </td>
                            <td>
                                <telerik:RadDatePicker ID="rdpAccountingDate" runat="server" />
                                <asp:RequiredFieldValidator ID="rfvAccountingDate" runat="server" ControlToValidate="rdpAccountingDate" ErrorMessage="Campo data obbligatorio" Display="Dynamic"></asp:RequiredFieldValidator>
                                &nbsp;<b>Numero:</b>&nbsp;
                                <asp:TextBox ID="txtAccountingNumber" runat="server" Style="vertical-align: middle; height: 20px;" />
                                <asp:CompareValidator ID="cvSectionalNumber" runat="server" ControlToValidate="txtAccountingNumber" ErrorMessage="Numero non valido" Display="Dynamic" ValueToCompare="0" Operator="GreaterThan" Type="Integer"></asp:CompareValidator>
                                <asp:RequiredFieldValidator ID="rfvAccountingNumber" runat="server" ControlToValidate="txtAccountingNumber" ErrorMessage="Campo data obbligatorio" Display="Dynamic"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        </asp:Panel>
                    </telerik:RadAjaxPanel>
                </table>
            </asp:Panel>
        </telerik:RadAjaxPanel>

        <%-- Scelta Contatti --%>
        <table class="datatable">
            <tr>
                <td id="MittCell" style="width: 50%; white-space: nowrap;" runat="server">
                    <usc:SelContatti ButtonImportManualVisible="true" Caption="Mittenti" EnableCC="false" ID="uscMittenti" Multiple="true" MultiSelect="true" ProtType="True" RequiredErrorMessage="Mittente obbligatorio" runat="server" TreeViewCaption="Mittenti" Type="Prot" />
                </td>
                <td id="DestCell" style="width: 50%; white-space: nowrap;" runat="server">
                    <usc:SelContatti ButtonImportManualVisible="true" Caption="Destinatari" EnableCC="true" ID="uscDestinatari" IsRequired="true" Multiple="true" MultiSelect="true" ProtType="True" RequiredErrorMessage="Destinatario obbligatorio" runat="server" TreeViewCaption="Destinatari" Type="Prot" />
                </td>
            </tr>
        </table>
        <%-- Sezione Scelta Fascicolo --%>
        <asp:Panel ID="panelFascicolo" runat="server">
            <table class="datatable">
                <tr>
                    <td style="width: 20%;"></td>
                    <td style="width: 60%;">
                        <usc:SelContatti runat="server" ID="uscFascicle" Type="Prot" Caption="Fascicolo" EnableCC="false" Multiple="true" MultiSelect="true" IsRequired="false" TreeViewCaption="Fascicolo" ButtonIPAVisible="false" ButtonManualVisible="false" />
                    </td>
                    <td style="width: 20%;"></td>
                </tr>
            </table>
        </asp:Panel>
        <%-- Sezione Package --%>
        <telerik:RadAjaxPanel runat="server" ID="radAjaxPnlPackage">
            <asp:Panel ID="pnlPackage" runat="server" Visible="False">
                <table class="datatable">
                    <tr>
                        <th>Dati Archiviazione</th>
                    </tr>
                    <tr>
                        <td width="90%">&nbsp;
                        <asp:Label ID="Label1" runat="server" EnableViewState="False" Font-Bold="True">Tipologia: </asp:Label>&nbsp;
                        <asp:TextBox ID="txtOrigin" runat="server" Enabled="False" Width="20px"></asp:TextBox>&nbsp;
                        <asp:Label ID="lblPackage" runat="server" EnableViewState="False" Font-Bold="True"> Scatolone: </asp:Label>&nbsp;
                        <asp:TextBox ID="txtPackage" runat="server" Enabled="False" Width="60px"></asp:TextBox>&nbsp;
                        <asp:Label ID="lblLot" runat="server" EnableViewState="False" Font-Bold="True"> Lotto: </asp:Label>&nbsp;
                        <asp:TextBox ID="txtLot" runat="server" Enabled="False" Width="60px"></asp:TextBox>&nbsp;
                        <asp:Label ID="lblIncremental" runat="server" EnableViewState="False" Font-Bold="True"> Progressivo: </asp:Label>&nbsp;
                        <asp:TextBox ID="txtIncremental" runat="server" Enabled="False" Width="60px"></asp:TextBox>&nbsp;
                        <asp:Button ID="btnNewPackage" runat="server" CausesValidation="False" Text="Nuovo scatolone" EnableViewState="False"></asp:Button>&nbsp;
                        <asp:Button ID="btnNewLot" runat="server" CausesValidation="False" Text="Nuovo lotto" EnableViewState="False"></asp:Button>
                        </td>
                    </tr>
                </table>
            </asp:Panel>
        </telerik:RadAjaxPanel>
        <%-- Sezione Autorizzazioni --%>
        <asp:Panel ID="pnlAutorizzazioni" runat="server" Visible="False">
            <usc:Settori Caption="Autorizzazioni" ID="uscAutorizzazioni" MultipleRoles="True" MultiSelect="True" RequiredMessage="Campo autorizzazioni obbligatorio" RoleRestictions="None" runat="server" Type="Prot" />
            <usc:Settori Caption="Autorizzazioni responsabile settore" ID="uscAutorizzazioniCc" ReadOnly="True" runat="server" />
        </asp:Panel>
        <%-- Sezione Oggetto --%>
        <table class="datatable">
            <tr>
                <th>Oggetto</th>
            </tr>
            <tr class="Chiaro">
                <td>
                    <usc:SelOggetto runat="server" ID="uscOggetto" EditMode="true" MultiLine="true" MaxLength="255" Required="true" Type="Prot" />
                </td>
            </tr>
        </table>
        <%-- Sezione Classificatore --%>
        <usc:SelCategory runat="server" ID="uscClassificatori" Type="Prot" Caption="Classificazione" Multiple="false" />
        <table class="datatable">
            <%-- Sezione Note --%>
            <tr class="Chiaro">
                <td style="width: 150px; vertical-align: middle; font-size: 11px; text-align: right">
                    <div id="divLblNote" runat="server" style="font-weight: bold;">Note:</div>
                </td>
                <td>
                    <div id="divTxtNote" style="margin-left: 2px;" runat="server">
                        <asp:TextBox ID="txtNote" runat="server" TextMode="MultiLine" Rows="3" Width="400px" onBlur="javascript:ChangeStrWithValidCharacter(this);"></asp:TextBox>
                    </div>
                </td>
            </tr>
            <%-- Sezione Assegnatario --%>
            <tr class="Chiaro">
                <td style="width: 150px; vertical-align: middle; font-size: 11px; text-align: right;">
                    <b>
                        <asp:Label ID="lblAssegnatario" runat="server">Assegnatario:</asp:Label></b>
                </td>
                <td>
                    <usc:SelContattiTesto ForceAddressBook="True" ID="uscContactAssegnatario" MaxLunghezzaTesto="1000" runat="server" TextBoxWidth="400px" Type="Prot" />
                </td>
            </tr>
            <%-- Sezione Categoria di servizio --%>
            <tr class="Chiaro">
                <td style="width: 150px; vertical-align: middle; font-size: 11px; text-align: right">
                    <div id="divLblServiceCategory" runat="server" style="font-weight: bold;">Categoria di servizio:</div>
                </td>
                <td>
                    <div id="divSelServiceCategory" runat="server">
                        <usc:SelServiceCategory runat="server" ID="SelServiceCategory" TextBoxWidth="400px" EditMode="true" MultiLine="false" MaxLength="100" Required="false" Type="Prot" />
                    </div>
                </td>
            </tr>
        </table>
        <%-- Sezione RECOVERY --%>
        <table class="datatable" id="RecoveryPanel" runat="server">
            <tr>
                <th colspan="2">Recupero protocollo</th>
            </tr>
            <tr class="Chiaro">
                <td style="width: 150px; vertical-align: middle; font-size: 11px; text-align: right">
                    <b>Protocollo: </b>
                </td>
                <td>
                    <asp:TextBox AutoPostBack="True" ID="txtProtRecovery" ReadOnly="true" runat="server" Width="150px" />
                    <asp:Button ID="btnSelectProtocol" runat="server" Text="Seleziona protocollo" />
                </td>
            </tr>
        </table>
          <div id="panelHiddenFileds" style="display:none" runat="server">
              <asp:HiddenField ID="hf_workflowAction_toFascicle" runat="server" Value="" />
              <asp:HiddenField ID="hf_workflowAction_toFascicleFolder" runat="server" Value="" />
          </div>
    </asp:Panel>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <input id="btnInserimentoV" onclick="document.getElementById('<%= btnInserimento.ClientID %>').click();" type="button" value="Conferma inserimento" />
    <asp:HiddenField ID="hfNeedClone" runat="server" Value="0" />
    <asp:Button ID="btnInserimento" runat="server" Text="Conferma inserimento" />
    <br />
    <asp:Panel ID="pnlValidationSummary" runat="server">
        <asp:ValidationSummary DisplayMode="List" ID="validationSummary" runat="server" ShowMessageBox="True" ShowSummary="False" />
    </asp:Panel>
</asp:Content>

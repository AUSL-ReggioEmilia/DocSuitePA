<%@ Control AutoEventWireup="false" CodeBehind="uscResolutionWorkflowFinder.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscResolutionWorkflowFinder" Language="vb" %>

<%@ Register Src="~/UserControl/uscContattiSel.ascx" TagName="uscContattiSel" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscClassificatore.ascx" TagName="uscClassificatore" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscProtocolSelTree.ascx" TagName="uscProtocolSelTree" TagPrefix="usc" %>

<asp:Panel runat="server" ID="pnlRadioButtons">
    <table id="table" class="dataform">
        <%-- Tipologia --%>
        <asp:Panel ID="pnlTipologia" runat="server">
            <tr>
                <td class="label" style="width: 30%">Tipologia:
                </td>
                <td class="DXChiaro" style="width: 70%; vertical-align: top">
                    <asp:RadioButtonList AutoPostBack="True" ID="rblTipologia" RepeatDirection="Horizontal" runat="server" />
                </td>
                <td class="DXChiaro" style="width: 30%"></td>
            </tr>
        </asp:Panel>
        <asp:Panel ID="pnlOCRegion" runat="server" Visible="false">
            <tr>
                <td class="label" style="width: 30%">Controllo della Regione:
                </td>
                <td class="DXChiaro" style="width: 70%; vertical-align: top">
                    <asp:CheckBox AutoPostBack="True" ID="chbOCRegion" RepeatDirection="Horizontal" runat="server" Checked="false" />
                </td>
                <td class="DXChiaro" style="width: 30%"></td>
            </tr>
        </asp:Panel>
        <%-- Passo del Flusso --%>
        <asp:Panel ID="pnlAzione" runat="server">
            <tr>
                <td class="label" style="width: 30%; vertical-align: top">Passo del flusso:
                </td>
                <td class="DXChiaro" style="width: 70%; vertical-align: top">
                    <asp:RadioButtonList ID="rblFlusso" runat="server" Width="100%" AutoPostBack="True">
                    </asp:RadioButtonList>
                    <asp:RadioButtonList ID="rblFlussoDet" runat="server" Width="100%" AutoPostBack="True">
                    </asp:RadioButtonList>
                </td>
                <td class="DXChiaro" style="width: 30%"></td>
            </tr>
        </asp:Panel>
    </table>
</asp:Panel>
<asp:Panel runat="server" ID="pnlControls">
    <table class="dataform" style="margin: 0px 0px 0px 0px">
        <asp:Panel ID="pnlOmissis" runat="server">
            <tr>
                <td class="label" width="30%">Privacy: </td>
                <td class="DXChiaro" width="70%">
                    <asp:CheckBox ID="chbOmissis" runat="server"></asp:CheckBox>
                </td>
            </tr>
        </asp:Panel>
        <%-- Proposta Da A --%>
        <asp:Panel runat="server" ID="pnlProposta">
            <tr>
                <td class="label" style="width: 30%">Proposta da data:
                </td>
                <td class="DXChiaro" style="width: 70%; height: 100%;" colspan="2">
                    <telerik:RadDatePicker ID="ProposeDate_From" runat="server" />
                    &nbsp;&nbsp;
                    <asp:Label ID="Label3" runat="server" Width="14px" Font-Bold="True" Height="100%" Style="vertical-align: middle;">a:</asp:Label>
                    <telerik:RadDatePicker ID="ProposeDate_To" runat="server" />
                    <asp:CompareValidator ID="cvCompareProposeDataTo" runat="server" ControlToValidate="ProposeDate_To"
                        ErrorMessage="La data di proposta finale non può essere antecedente a quella iniziale" Display="Dynamic"
                        Type="Date" Operator="GreaterThanEqual" ControlToCompare="ProposeDate_From"></asp:CompareValidator>
                </td>
            </tr>
        </asp:Panel>
        <%-- Adottata Il --%>
        <asp:Panel ID="pnlAdottata" runat="server">
            <tr>
                <td class="label" style="width: 30%">Adottata in data:
                </td>
                <td class="DXChiaro" style="width: 70%" colspan="2">
                    <telerik:RadDatePicker ID="AdoptionDate" runat="server" />
                    <asp:RequiredFieldValidator ControlToValidate="AdoptionDate" Display="Dynamic" ErrorMessage="Data Obbligatoria" ID="rfvAdoptionDate" runat="server" />
                </td>
            </tr>
        </asp:Panel>
        <%-- Adottata Da A Required --%>
        <asp:Panel ID="pnlAdottataIntervalloReq" runat="server">
            <tr>
                <td class="label" style="width: 30%">Adottata da data:
                </td>
                <td class="DXChiaro" style="width: 70%; height: 100%;" colspan="2">
                    <telerik:RadDatePicker ID="AdoptionDate_From_Req" runat="server" />
                    <asp:RequiredFieldValidator ControlToValidate="AdoptionDate_From_Req" Display="Dynamic" ErrorMessage="Data Obbligatoria" ID="rfvAdoptionDateFrom" runat="server" />
                    &nbsp;&nbsp;
                    <asp:Label ID="Label2" runat="server" Width="14px" Font-Bold="True" Height="100%" Style="vertical-align: middle;">a:</asp:Label>
                    <telerik:RadDatePicker ID="AdoptionDate_To_Req" runat="server" />
                    <asp:RequiredFieldValidator ControlToValidate="AdoptionDate_To_Req" Display="Dynamic" ErrorMessage="Data Obbligatoria" ID="rfvAdoptionDateTo" runat="server" />
                    <asp:CompareValidator ID="cvCompareAdoptionDataTo" runat="server" ControlToValidate="AdoptionDate_To_Req"
                        ErrorMessage="La data di adozione finale non può essere antecedente a quella iniziale" Display="Dynamic"
                        Type="Date" Operator="GreaterThanEqual" ControlToCompare="AdoptionDate_From_Req"></asp:CompareValidator>
                </td>
            </tr>
        </asp:Panel>
                <%-- Adottata Da A NonRequired --%>
        <asp:Panel ID="pnlAdottataIntervallo" runat="server">
            <tr>
                <td class="label" style="width: 30%">Adottata da data:
                </td>
                <td class="DXChiaro" style="width: 70%; height: 100%;" colspan="2">
                    <telerik:RadDatePicker ID="AdoptionDate_From" runat="server" />
                    &nbsp;&nbsp;
                    <asp:Label ID="Label1" runat="server" Width="14px" Font-Bold="True" Height="100%" Style="vertical-align: middle;">a:</asp:Label>
                    <telerik:RadDatePicker ID="AdoptionDate_To" runat="server" />
                    <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="AdoptionDate_To"
                        ErrorMessage="La data di adozione finale non può essere antecedente a quella iniziale" Display="Dynamic"
                        Type="Date" Operator="GreaterThanEqual" ControlToCompare="AdoptionDate_From"></asp:CompareValidator>
                </td>
            </tr>
        </asp:Panel>
        <asp:Panel ID="pnlAdoptionYearNumber" runat="server">
            <tr>
                <td class="label" style="width: 30%">Anno adozione:
                </td>
                <td class="DXChiaro" style="width: 70%; height: 100%;" colspan="2">
                    <asp:TextBox runat="server" ID="txtAdoptionYear"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="label" style="width: 30%">Dal numero:
                </td>
                <td class="DXChiaro" style="width: 70%; height: 100%;" colspan="2">
                    <asp:TextBox runat="server" ID="txtNumberFrom"></asp:TextBox>
                    <asp:Label runat="server" Font-Bold="True" Text="al numero:"></asp:Label>
                    <asp:TextBox runat="server" ID="txtNumberTo"></asp:TextBox>
                </td>
            </tr>
        </asp:Panel>
        <%-- Inviata Il --%>
        <asp:Panel ID="pnlOC" runat="server">
            <tr>
                <td class="label" style="width: 30%">Inviata al Collegio in data:
                </td>
                <td class="DXChiaro" style="width: 70%" colspan="2">
                    <telerik:RadDatePicker ID="CollegioWarningDate" runat="server" />
                    <asp:RequiredFieldValidator ControlToValidate="CollegioWarningDate" Display="Dynamic" ErrorMessage="Data Obbligatoria" ID="rfvCollegioWarningDate" runat="server" />
                </td>
            </tr>
        </asp:Panel>
        <%-- Pubblicata Il --%>
        <asp:Panel ID="pnlPubblicata" runat="server">
            <tr>
                <td class="label" style="width: 30%">Pubblicata in data:
                </td>
                <td class="DXChiaro" style="width: 70%" colspan="2">
                    <telerik:RadDatePicker ID="PublishingDate" runat="server" />
                    <asp:RequiredFieldValidator ControlToValidate="PublishingDate" Display="Dynamic" ErrorMessage="Data Obbligatoria" ID="rfvPublishingDate" runat="server" />
                </td>
            </tr>
        </asp:Panel>
        <%-- Esecutiva Il --%>
        <asp:Panel ID="pnlEsecutivita" runat="server">
            <tr>
                <td class="label" style="width: 30%">Esecutiva in data:
                </td>
                <td class="DXChiaro" style="width: 70%" colspan="2">
                    <telerik:RadDatePicker ID="EffectivenessDate" runat="server" />
                    <asp:RequiredFieldValidator ControlToValidate="EffectivenessDate" Display="Dynamic" ErrorMessage="Data Obbligatoria" ID="rfvEffectivenessDate" runat="server" />
                </td>
            </tr>
        </asp:Panel>
        <%-- Contenitore --%>
        <asp:Panel ID="pnlContenitore" runat="server">
            <tr>
                <td class="label" style="width: 30%">Contenitore:
                </td>
                <td class="DXChiaro" style="width: 40%">
                    <asp:DropDownList ID="ddlContainer" runat="server">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator ID="cvContainer" runat="server" Display="Dynamic" ControlToValidate="ddlContainer"
                        ErrorMessage="Campo contenitore obbligatorio"></asp:RequiredFieldValidator>
                </td>
                <td class="DXChiaro" style="width: 30%"></td>
            </tr>
        </asp:Panel>
        <telerik:RadAjaxPanel ID="pnlContainerMultiSelect" runat="server">
            <tr>
                <td class="label" style="width: 30%">Contenitore:
                </td>
                <td class="DXChiaro" style="width: 40%;">
                    <telerik:RadComboBox runat="server" ID="rcbContainerMultiSelect" CheckBoxes="True" EnableCheckAllItemsCheckBox="True" Width="350px">
                        <Localization CheckAllString="Seleziona tutti" AllItemsCheckedString="Tutti i contenitori selezionati" />
                    </telerik:RadComboBox>
                </td>
            </tr>
            <tr>
                <td class="label" style="width: 30%"></td>
                <td class="DXChiaro" style="width: 40%">
                    <asp:RequiredFieldValidator ID="rcbContainerMultiSelectValidator" runat="server" Display="Dynamic" ControlToValidate="rcbContainerMultiSelect"
                        ErrorMessage="Campo contenitore obbligatorio"></asp:RequiredFieldValidator>
                </td>
            </tr>
        </telerik:RadAjaxPanel>
        <%-- Protocollo --%>
        <asp:Panel ID="pnlProtocollo" runat="server" Visible="true">
            <tr>
                <td class="label" style="width: 30%">
                    <asp:Label ID="lblProtocollo" runat="server"></asp:Label>
                </td>
                <td class="DXChiaro" style="width: 40%">
                    <usc:uscProtocolSelTree runat="server" ID="uscProtocollo" ButtonDeleteVisible="true"
                        ButtonSelectVisible="true" IsRequired="true" RequiredErrorMessage="Protocollo Trasmissione Obbligatorio"
                        Multiple="false" TreeViewCaption="Protocollo trasmissione" />
                </td>
                <td class="DXChiaro" style="width: 30%"></td>
            </tr>
        </asp:Panel>
        <%-- Proponente --%>
        <asp:Panel ID="pnlAltre" runat="server">
            <tr>
                <td class="label" style="width: 30%">Proponente:
                </td>
                <td class="DXChiaro" style="width: 40%">
                    <asp:Panel ID="pnlPropInterop" runat="server">
                        <usc:uscContattiSel runat="server" ID="uscPropInterop" ButtonSelectVisible="true"
                            ButtonDeleteVisible="true" ButtonImportVisible="false" ButtonIPAVisible="false"
                            ButtonManualVisible="false" ButtonPropertiesVisible="false" TreeViewCaption="Proponente"
                            HeaderVisible="false" IsRequired="false" Type="Resl" />
                    </asp:Panel>
                </td>
                <td class="DXChiaro" style="width: 30%"></td>
            </tr>
        </asp:Panel>
        <%-- Category --%>
        <asp:Panel ID="pnlCategory" runat="server">
            <tr>
                <td class="label" style="width: 30%">Classificazione:
                </td>
                <td class="DXChiaro" style="width: 40%; white-space: nowrap; max-width: 500px">
                    <usc:uscClassificatore ID="uscCategory" runat="server" HeaderVisible="false" Required="false"
                        Type="Resl" />
                </td>
                <td class="DXChiaro" style="width: 30%"></td>
            </tr>
            <asp:Panel ID="pnlCategorySearch" runat="server">
                <tr>
                    <td class="label" style="height: 24px; width: 30%"></td>
                    <td class="DXChiaro" style="height: 24px; width: 40%">
                        <asp:CheckBox ID="chbCategoryChild" runat="server" Text="Estendi ricerca alle Sottocategorie"
                            AutoPostBack="true" />
                    </td>
                    <td class="DXChiaro" style="width: 30%"></td>
                </tr>
            </asp:Panel>
        </asp:Panel>
    </table>
</asp:Panel>

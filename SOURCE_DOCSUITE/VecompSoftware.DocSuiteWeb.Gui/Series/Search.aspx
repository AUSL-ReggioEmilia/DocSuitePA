<%@ Page AutoEventWireup="false" CodeBehind="Search.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.Series.Search" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<%@ Register Src="~/UserControl/uscClassificatore.ascx" TagName="Classificatore" TagPrefix="usc" %>
<%@ Register TagPrefix="usc" TagName="Settori" Src="~/UserControl/uscSettori.ascx" %>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
     <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript" language="javascript">
            function disableDynamicsControl(controlsId, disableControl) {
                for (i = 0; i < controlsId.length; i++)
                {
                    var inputElement = '#ctl00_cphContent_' + controlsId[i];
                    if (disableControl) {
                        // Resetto il testo inserito
                        $(inputElement).val("");
                        $(inputElement).hide();
                    }
                    else {
                        $(inputElement).show()
                    }
                }               
            }            
        </script>
    </telerik:RadCodeBlock>

    <asp:Panel runat="server" ID="MainContentWrapper">

        <table class="dataform">
            <tr>
                <td class="label" style="width: 30%;">Tipo:
                </td>
                <td style="width: 70%;">
                    <asp:DropDownList runat="server" CausesValidation="false" ID="ddlContainerArchive" AutoPostBack="True" Width="300px" />
                </td>
            </tr>
        </table>

        <div runat="server" id="ContentWrapper" style="height: 100%;">

            <table class="dataform">
                <tr>
                    <td class="label" style="width: 30%;">
                        <asp:Label ID="DocumentSeries" runat="server" />
                    </td>
                    <td style="width: 70%;">
                        <asp:DropDownList runat="server" CausesValidation="false" ID="ddlDocumentSeries" AutoPostBack="True" Width="300px" />
                    </td>
                </tr>
                <tr>
                    <td class="label" style="width: 30%;">Anno:
                    </td>
                    <td style="width: 70%;">
                        <telerik:RadNumericTextBox ID="txtYear" NumberFormat-DecimalDigits="0" NumberFormat-GroupSeparator="" IncrementSettings-InterceptArrowKeys="True" IncrementSettings-InterceptMouseWheel="True" MaxLength="4" Width="56px" runat="server" />
                    </td>
                </tr>
                <tr style="vertical-align: middle;">
                    <td class="label">Numero:
                    </td>
                    <td>
                        <telerik:RadNumericTextBox ID="txtNumberFrom" LabelCssClass="strongRiLabel" LabelWidth="22%" Label="Da" Style="margin-left: 2px;" NumberFormat-DecimalDigits="0" NumberFormat-GroupSeparator="" IncrementSettings-InterceptArrowKeys="True" IncrementSettings-InterceptMouseWheel="True" MaxLength="7" Width="150" runat="server" />
                        <telerik:RadNumericTextBox ID="txtNumberTo" LabelCssClass="strongRiLabel" LabelWidth="22%" Label="A" Style="margin-left: 5px;" NumberFormat-DecimalDigits="0" NumberFormat-GroupSeparator="" IncrementSettings-InterceptArrowKeys="True" IncrementSettings-InterceptMouseWheel="True" MaxLength="7" Width="150" runat="server" />
                    </td>
                </tr>
                <tr style="vertical-align: middle;">
                    <td class="label">Data registrazione:
                    </td>
                    <td>
                        <telerik:RadDatePicker ID="rdpRegistrationFrom" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="Da" runat="server" />
                        <telerik:RadDatePicker ID="rdpRegistrationTo" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="A" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td class="label">Oggetto:
                    </td>
                    <td>
                        <telerik:RadTextBox ID="txtSubject" runat="server" Width="300px" />
                        <asp:CheckBox ID="chkSubjectContains" runat="server" Text="Contiene" Checked="true" />
                    </td>
                </tr>
                <tr>
                    <td class="label">
                        Settori di appartenenza:
                    </td>
                    <td>
                        <usc:Settori Environment="DocumentSeries" HeaderVisible="false" ID="uscRoleOwner" MultipleRoles="true" LoadUsers="False" MultiSelect="true" Required="False" runat="server" ShowActive="True" Type="Prot" />
                    </td>
                </tr>
                <tr>
                    <td class="label">Classificazione:
                    </td>
                    <td>
                        <usc:Classificatore ID="uscCategory" runat="server" Action="Search" Required="false" HeaderVisible="false" />
                    </td>
                </tr>
                <tr runat="server" id="trIncludeCancelled" visible="false">
                    <td class="label">Includi registrazioni annullate:
                    </td>
                    <td style="vertical-align: bottom; height: 100%; width: 500px">
                        <asp:CheckBox runat="server" ID="chkIncludeCancelled" />
                    </td>
                </tr>
                <tr runat="server" id="trIncludeDraft">
                    <td class="label">Includi bozze:
                    </td>
                    <td style="vertical-align: bottom; height: 100%; width: 500px;">
                        <asp:CheckBox runat="server" Style="margin-left: 3px;" ID="chkIncludeDraft" />
                    </td>
                </tr>
                <tr>
                    <td class="label">Primo piano:
                    </td>
                    <td style="vertical-align: bottom; height: 100%; width: 500px;">
                        <asp:CheckBox runat="server" Style="margin-left: 3px;" ID="chkPriority" />
                    </td>
                </tr>
            </table>

            <asp:Panel runat="server" ID="pnlSubsectionArea">
                <table class="dataform" runat="server" id="tblSubsection" visible="False">
                    <tr>
                        <td class="label" style="width: 30%;">Sotto-sezione:
                        </td>
                        <td style="width: 70%;">
                            <asp:DropDownList runat="server" CausesValidation="false" ID="ddlSubsection" AutoPostBack="false" Visible="True" Width="300px" DataTextField="Description" DataValueField="Id" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <asp:Panel runat="server" ID="pnlPublicationArea">
                <table class="dataform" runat="server" id="tblPublication" visible="True">
                    <tr>
                        <td class="label" style="width: 30%;">Stato pubblicazione:
                        </td>
                        <td style="width: 70%;">
                            <asp:CheckBoxList runat="server" ID="cblPublicationStatus" RepeatLayout="Table">
                                <asp:ListItem Value="NONE">Nessuno</asp:ListItem>
                                <asp:ListItem Value="PUBLICATED">Pubblicato</asp:ListItem>
                                <asp:ListItem Value="RETIRED">Ritirato</asp:ListItem>
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr style="vertical-align: middle;">
                        <td class="label">Data pubblicazione:
                        </td>
                        <td>
                            <telerik:RadDatePicker ID="txtPublishingDateFrom" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="Da" runat="server" />
                            <telerik:RadDatePicker ID="txtPublishingDateTo" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="A" runat="server" />
                        </td>
                    </tr>
                    <tr style="vertical-align: middle;">
                        <td class="label">Data ritiro:
                        </td>
                        <td>
                            <telerik:RadDatePicker ID="txtRetireDateFrom" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="Da" runat="server" />                           
                            <telerik:RadDatePicker ID="txtRetireDateTo" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="A" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="label" style="width: 30%;">Tipologia provenzienza:
                        </td>
                        <td style="width: 70%;">
                            <asp:CheckBoxList runat="server" ID="clbOriginType" RepeatLayout="Table">
                                <asp:ListItem Value="0">Atto</asp:ListItem>
                                <asp:ListItem Value="1">Delibera</asp:ListItem>
                            </asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr>
                        <td class="label" style="width: 30%;">Dati della provenzienza:
                        </td>
                        <td style="width: 70%;">
                            <telerik:RadTextBox ID="txtOrginText" runat="server" Width="300px" />
                        </td>
                    </tr>
                </table>
            </asp:Panel>

            <asp:PlaceHolder runat="server" ID="DynamicControls"></asp:PlaceHolder>

        </div>

    </asp:Panel>

</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Panel runat="server" ID="ButtonsPanel">
        <asp:Button ID="btnSearch" runat="server" TabIndex="1" Text="Ricerca" />
        <asp:Button ID="btnClear" runat="server" TabIndex="2" Text="Svuota Ricerca" />
    </asp:Panel>
</asp:Content>

<%@ Page AutoEventWireup="false" CodeBehind="Export.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.Series.Export" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">

    <asp:Panel runat="server" ID="MainContentWrapper">

        <div runat="server" id="ContentWrapper" style="height: 100%;">

            <table class="dataform">
                <tr>
                    <td class="label" style="width: 30%;">
                        <asp:Label ID="DocumentSeries" runat="server" />
                    </td>
                    <td style="width: 70%;">
                        <asp:DropDownList runat="server" CausesValidation="false" ID="ddlDocumentSeries" AutoPostBack="True" Width="300px" ValidationGroup="FinderGroup" />
                        <asp:RequiredFieldValidator ID="rfvDdlDocumentSeries" ErrorMessage="Selezionare una serie" ForeColor="Red" runat="server" ControlToValidate="ddlDocumentSeries" InitialValue="-1" ValidationGroup ="FinderGroup"/>
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
                    <td class="label">Data registrazione:
                    </td>
                    <td>
                        <telerik:RadDatePicker ID="rdpRegistrationFrom" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="Da" runat="server" />
                        <telerik:RadDatePicker ID="rdpRegistrationTo" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="A" runat="server" />                        
                    </td>
                </tr>
                <tr runat="server" id="trIncludeDraft">
                    <td class="label">Includi bozze:
                    </td>
                    <td style="vertical-align: bottom; height: 100%; width: 500px;">
                        <asp:CheckBox runat="server" Style="margin-left: 3px;" ID="chkIncludeDraft" />
                    </td>
                </tr>
            </table>

            <asp:Panel runat="server" ID="pnlPublicationArea">
                <table class="dataform" runat="server" id="tblPublication" visible="False">
                    <tr>
                        <td class="label" style="width: 30%;">Stato pubblicazione:
                        </td>
                        <td style="width: 70%;">
                            <asp:RadioButtonList runat="server" ID="cblPublicationStatus" RepeatLayout="Table">
                                <asp:ListItem Value="NONE">Nessuno</asp:ListItem>
                                <asp:ListItem Value="PUBLICATED">Pubblicato</asp:ListItem>
                                <asp:ListItem Value="RETIRED">Ritirato</asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                </table>
            </asp:Panel>

        </div>

    </asp:Panel>

</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Panel runat="server" ID="ButtonsPanel">
        <asp:Button ID="btnExport" runat="server" Text="Esporta" visible="false" ValidationGroup="FinderGroup"/>
        <asp:Button ID="btnClear" runat="server" TabIndex="2" Text="Svuota Ricerca" />
    </asp:Panel>
</asp:Content>

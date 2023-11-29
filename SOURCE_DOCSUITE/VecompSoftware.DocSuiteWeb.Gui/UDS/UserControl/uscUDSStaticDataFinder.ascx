<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscUDSStaticDataFinder.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscUDSStaticDataFinder" %>
<%@ Register Src="~/UserControl/uscClassificatore.ascx" TagPrefix="usc" TagName="SelCategory" %>

<telerik:RadScriptBlock runat="server">
    <script type="text/javascript">
        function setBorderBottom() {
            $("#grdFinderStaticData").addClass("border-bottom-collapse");
        }

        function removeBorderBottom() {
            $("#grdFinderStaticData").removeClass("border-bottom-collapse");
        }
    </script>
</telerik:RadScriptBlock>

<telerik:RadPageLayout runat="server" GridType="Fluid" HtmlTag="None">
    <Rows>
        <%--Dati--%>
        <telerik:LayoutRow runat="server" CssClass="col-dsw-10" RowType="Container" HtmlTag="None">
            <Columns>
                <telerik:LayoutColumn HtmlTag="None">
                    <asp:Panel runat="server" ID="rowStaticData">
                        <table class="datatable 
                            
                            DataTable"
                            id="grdFinderStaticData">
                            <tr>
                                <th colspan="2">Dati</th>
                            </tr>
                            <tr id="rowYear" runat="server">
                                <td class="col-dsw-2 label">
                                    <label>Anno:</label>
                                </td>
                                <td class="col-dsw-8">
                                    <telerik:RadDateInput ID="txtYear" runat="server" Width="100px" DateFormat="yyyy"></telerik:RadDateInput>
                                </td>
                            </tr>
                            <tr id="rowNumber" runat="server">
                                <td class="col-dsw-2 label">
                                    <label>Numero:</label>
                                </td>
                                <td class="col-dsw-8">
                                    <telerik:RadNumericTextBox ID="txtNumber" runat="server" NumberFormat-DecimalDigits="0" NumberFormat-GroupSeparator="" Width="150px"></telerik:RadNumericTextBox>
                                </td>
                            </tr>
                            <tr class="dsw-vertical-middle">
                                <td class="col-dsw-2 label">Data:
                                </td>
                                <td class="col-dsw-8">
                                    <telerik:RadDatePicker ID="txtRegistrationDateFrom" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="Da" runat="server" />
                                    <telerik:RadDatePicker ID="txtRegistrationDateTo" DateInput-LabelWidth="20%" DateInput-LabelCssClass="strongRiLabel" Style="height: auto !important;" Width="200" DateInput-Label="A" runat="server" />
                                </td>
                            </tr>
                            <tr>
                                <td class="col-dsw-2 label" style="vertical-align: middle;">
                                    <label>Oggetto:</label>
                                </td>
                                <td class="col-dsw-8">
                                    <telerik:RadTextBox ID="txtSubject" runat="server" Width="100%" TextMode="SingleLine"></telerik:RadTextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="col-dsw-2 label" style="vertical-align: middle;">
                                    <label>Classificazione:</label>
                                </td>
                                <td class="col-dsw-8">
                                    <usc:SelCategory runat="server" ID="uscSelCategory" Type="Prot" Caption="Classificazione" Action="Search" Required="false" HeaderVisible="false" Multiple="false" />
                                </td>
                            </tr>
                            <tr>
                                <td class="col-dsw-2 label" style="vertical-align: middle;">
                                    <label>Nome documento:</label>
                                </td>
                                <td class="col-dsw-8">
                                    <telerik:RadTextBox ID="txtDocumentName" runat="server" Width="100%" TextMode="SingleLine"></telerik:RadTextBox>
                                </td>
                            </tr>
                            <tr>
                                <td class="col-dsw-2 label" style="vertical-align: middle;">
                                    <label>Privo di documenti:</label>
                                </td>
                                <td class="col-dsw-8">
                                    <telerik:RadDropDownList runat="server" ID="rddlGenericDocument" Width="140px" AutoPostBack="true">
                                        <Items>
                                            <telerik:DropDownListItem Selected="true" Text="Tutti" Value="0" />
                                            <telerik:DropDownListItem Text="Si" Value="1" />
                                            <telerik:DropDownListItem Text="No" Value="2" />
                                        </Items>
                                    </telerik:RadDropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td class="col-dsw-2 label" style="vertical-align: middle;">
                                    <label>Archivi annullati:</label>
                                </td>
                                <td class="col-dsw-8">
                                    <telerik:RadDropDownList runat="server" ID="chkStatus" Width="140px" AutoPostBack="true">
                                        <Items>
                                            <telerik:DropDownListItem Selected="true" Text="Tutti" Value="0" />
                                            <telerik:DropDownListItem Text="Si" Value="1" />
                                            <telerik:DropDownListItem Text="No" Value="2" />
                                        </Items>
                                    </telerik:RadDropDownList>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </telerik:LayoutColumn>
            </Columns>
        </telerik:LayoutRow>
    </Rows>
</telerik:RadPageLayout>

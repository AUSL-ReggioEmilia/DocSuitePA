<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscUDSStaticDataInsert.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscUDSStaticDataInsert" %>
<%@ Register Src="~/UserControl/uscClassificatore.ascx" TagPrefix="usc" TagName="SelCategory" %>

<telerik:RadPageLayout runat="server" GridType="Fluid" HtmlTag="None">
    <Rows>
        <%--Dati--%>
        <telerik:LayoutRow runat="server" CssClass="col-dsw-10" RowType="Container" HtmlTag="None">
            <Columns>
                <telerik:LayoutColumn HtmlTag="None">
                    <asp:Panel runat="server" ID="rowStaticData">
                        <table class="datatable udsDataTable border-bottom-collapse">
                            <tr>
                                <th colspan="2">Dati</th>
                            </tr>
                            <tr id="trSubject" runat="server">
                                <td class="col-dsw-2 label" style="vertical-align: middle;">
                                    <label>Oggetto:</label>
                                </td>
                                <td class="col-dsw-8">
                                    <telerik:RadAjaxPanel runat="server">
                                        <telerik:RadTextBox ID="txtSubject" runat="server" Width="100%" TextMode="MultiLine" Rows="3"></telerik:RadTextBox>
                                        <asp:RequiredFieldValidator ID="rfvSubject" runat="server" Display="Dynamic" ControlToValidate="txtSubject" ErrorMessage="Il campo Oggetto è obbligatorio" />
                                    </telerik:RadAjaxPanel>
                                </td>
                            </tr>
                        </table>
                    </asp:Panel>
                </telerik:LayoutColumn>
            </Columns>
        </telerik:LayoutRow>

        <%--Classificazione--%>
        <telerik:LayoutRow CssClass="col-dsw-10" runat="server" RowType="Container" HtmlTag="None">
            <Columns>
                <telerik:CompositeLayoutColumn HtmlTag="None">
                    <Content>
                        <asp:Panel runat="server" ID="rowCategory">
                            <table class="datatable udsDataTable border-top-collapse border-bottom-collapse">
                                <tr>
                                    <td class="col-dsw-2 label">Classificazione:
                                    </td>
                                    <td class="col-dsw-8">
                                        <usc:SelCategory runat="server" ID="uscSelCategory" Type="Prot" Caption="Classificazione" HeaderVisible="false" Multiple="false" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </Content>
                </telerik:CompositeLayoutColumn>
            </Columns>
        </telerik:LayoutRow>
    </Rows>
</telerik:RadPageLayout>

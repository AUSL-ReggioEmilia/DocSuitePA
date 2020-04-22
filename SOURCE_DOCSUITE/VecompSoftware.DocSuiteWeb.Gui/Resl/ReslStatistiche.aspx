<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="ReslStatistiche.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ReslStatistiche" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Statistiche - Delibere e Determine" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <asp:Panel runat="server" ID="pnlStatistics" DefaultButton="btnRicerca">
        <table class="dataform" width="100%">
            <tr>
                <td class="label">
                    Data Adozione da:</td>
                <td style="height:100%;">
                    <telerik:RadDatePicker ID="rdpAdoptionDateFrom" runat="server" />
                    <strong style="vertical-align:middle;height:100%;">&nbsp;a:&nbsp;</strong>
                    <telerik:RadDatePicker ID="rdpAdoptionDateTo" runat="server" />
                    <asp:CompareValidator ID="CompareValidator1" runat="server" ControlToValidate="rdpAdoptionDateFrom"
                        ControlToCompare="rdpAdoptionDateTo" Operator="LessThanEqual" ErrorMessage="La data di inizio è più recente della data di fine"
                        Display="Dynamic" />
                </td>
            </tr>
            <tr>
                <td class="label">
                    Data Proposta da:
                </td>
                <td style="height:100%;">
                    <telerik:RadDatePicker ID="rdpProposeDateFrom" runat="server" />
                    <strong style="vertical-align:middle;height:100%;">&nbsp;a:&nbsp;</strong>
                    <telerik:RadDatePicker ID="rdpProposeDateTo" runat="server" />
                    <asp:CompareValidator ID="cfDate" runat="server" ControlToValidate="rdpProposeDateFrom"
                        ControlToCompare="rdpProposeDateTo" Operator="LessThanEqual" ErrorMessage="La data di inizio è più recente della data di fine"
                        Display="Dynamic" />
                </td>
            </tr>
            <tr>
                <td class="label">
                    <b>Tipologia:</b>
                </td>
                <td>
                    <asp:DropDownList AppendDataBoundItems="True" DataSourceID="odsResolutionType" ID="ddlType" runat="server">
                        <asp:ListItem />
                    </asp:DropDownList>
                    <strong>&nbsp;Contenitore:&nbsp;</strong>
                    <asp:DropDownList ID="ddlContainer" runat="server" AppendDataBoundItems="true">
                        <asp:ListItem />
                    </asp:DropDownList>
                    &nbsp;&nbsp;
                    <asp:CheckBox Checked="true" Font-Bold="True" ID="chkShowContact" runat="server" Text="Visualizza dettagli Proponente\Destinatario" />
                </td>
        </table>
        <div style="display:inline-block;">
            <asp:Button ID="btnRicerca" runat="server" Text="Ricerca" />
        </div>
        <div style="display:inline-block; float:right;">
            <asp:Button ID="btnExcel" runat="server" Text="Esporta in Excel" Visible="True"></asp:Button>
            <asp:Button ID="btnStampa" runat="server" Text="Stampa" OnClientClick="window.print();return false;" />
        </div>
        <!-- datasource -->
        <asp:ObjectDataSource ID="odsResolutionType" OldValuesParameterFormatString="original_{0}" runat="server" SelectMethod="GetAll" TypeName="VecompSoftware.DocSuiteWeb.Facade.ResolutionTypeFacade" />
    </asp:Panel>        
</asp:Content>

<asp:Content ID="content" runat="server" ContentPlaceHolderID="cphContent">
    <asp:Panel ID="pnlResolution" runat="server" Visible="False" Width="100%">
        <table class="datatable" cellpadding="3" width="100%">
            <tr>
                <th colspan="3" nowrap="nowrap" align="left">
                    <asp:Label runat="server" ID="lblHeader" Text=""></asp:Label></th>
            </tr>
            <tr>
                <td style="width: 20%;" nowrap="nowrap">
                    <strong>Delibere e Determine</strong>
                </td>
                <td style="width:10%;text-align:right;">
                    <asp:Label ID="lblTotal" runat="server" Font-Bold="true" />
                </td>
                <td style="width:70%;">&nbsp;</td>
            </tr>
            <tr>
                <td style="width: 20%;" nowrap="nowrap">
                    <strong>Delibere e Determine (errata registrazione)</strong>
                </td>
                <td style="width:10%;text-align:right;">
                    <asp:Label ID="lblTotalRegisterError" runat="server" Font-Bold="true" />
                </td>
                <td style="width:70%;">&nbsp;</td>
            </tr>

            <!-- Dettaglio per tipologia -->
            <tr>
                <th colspan="3" align="left">
                    Dettaglio per Tipologia</th>
            </tr>            
            <%  If ResolutionByType.Count > 0 Then %>
            <asp:Repeater ID="rpType" runat="server">
                <ItemTemplate>
                    <tr>
                        <td style="width:20%;" nowrap="nowrap">
                            <strong>
                                <%#DataBinder.Eval(Container.DataItem,"[0]")%>
                            </strong>
                        </td>
                        <td style="width:10%;text-align:right;">
                            <strong>
                                <%#DataBinder.Eval(Container.DataItem,"[1]")%>
                            </strong>
                        </td>
                        <td style="width:70%;">&nbsp;</td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
            <%End If%>
            
            <!-- Dettaglio per contenitore -->
            <tr>
                <th colspan="3" align="left">
                    Dettaglio per Contenitore</th>
            </tr>
            <%If ResolutionByContainer.Count > 0 Then%>
            <asp:Repeater ID="rpContainer" runat="server">
                <ItemTemplate>
                    <tr>
                        <td style="width: 20%;" nowrap="nowrap">
                            <strong>
                                <%#DataBinder.Eval(Container.DataItem,"[1]")%>
                            </strong>
                        </td>
                        <td style="width:10%;text-align:right;">
                            <strong>
                                <%#DataBinder.Eval(Container.DataItem,"[2]")%>
                            </strong>
                        </td>
                        <td style="width:70%;">&nbsp;</td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
            <%End If%>
            
            <!-- dettaglio attivi per Organo di controllo -->            
            <tr>
                <th colspan="3" align="left">
                    Dettaglio per Organo di controllo</th>
            </tr>
            <%If ResolutionByOC.Count > 0 Then%>
            <asp:Repeater ID="rpOC" runat="server">
                <ItemTemplate>
                    <tr>
                        <td style="width: 20%;" nowrap="nowrap">
                            <strong>
                                <%#DataBinder.Eval(Container.DataItem,"[0]")%>
                            </strong>
                        </td>
                        <td style="width:10%;text-align:right;">
                            <strong>
                                <%#DataBinder.Eval(Container.DataItem,"[1]")%>
                            </strong>
                        </td>
                        <td style="width:70%;">&nbsp;</td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
            <%End If%>
            
            <%If Me.chkShowContact.Checked Then%>
            <!-- dettaglio attivi per proponente -->            
            <tr>
                <th colspan="3" align="left">
                    Dettaglio per Proponente</th>
            </tr>
            <%If ResolutionByProposer.Count > 0 Then%>
            <asp:Repeater ID="rpProposer" runat="server">
                <ItemTemplate>
                    <tr>
                        <td style="width: 20%;" nowrap="nowrap">
                            <strong>
                                <%#DataBinder.Eval(Container.DataItem,"[0]")%>
                            </strong>
                        </td>
                        <td style="width:10%;text-align:right;">
                            <strong>
                                <%#DataBinder.Eval(Container.DataItem,"[1]")%>
                            </strong>
                        </td>
                        <td style="width:70%;">&nbsp;</td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
            <%End If%>
            
            <!-- dettaglio attivi per Destinatario -->            
            <tr>
                <th colspan="3" align="left">
                    Dettaglio per Destinatario</th>
            </tr>
            <%If ResolutionByRecipient.Count > 0 Then%>
            <asp:Repeater ID="rpRecipient" runat="server">
                <ItemTemplate>
                    <tr>
                        <td style="width: 20%;" nowrap="nowrap">
                            <strong>
                                <%#DataBinder.Eval(Container.DataItem,"[0]")%>
                            </strong>
                        </td>
                        <td style="width:10%;text-align:right;">
                            <strong>
                                <%#DataBinder.Eval(Container.DataItem,"[1]")%>
                            </strong>
                        </td>
                        <td style="width:70%;">&nbsp;</td>
                    </tr>
                </ItemTemplate>
            </asp:Repeater>
            <%End If%>
            <%End If%>
        </table>
    </asp:Panel>
</asp:Content>

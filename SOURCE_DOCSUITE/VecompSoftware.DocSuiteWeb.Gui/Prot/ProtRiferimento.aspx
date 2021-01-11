<%@ Page AutoEventWireup="false" Codebehind="ProtRiferimento.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.ProtRiferimento" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Protocollo - Riferimento" %>

<%@ Register Src="../UserControl/uscProtocollo.ascx" TagName="uscProtocollo" TagPrefix="uc1" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <table ID="tblRiferimento" runat="server" class="datatable" width="100%">
        <tr>
            <td>
                <uc1:uscProtocollo ID="UscProtocollo" runat="server" />
            </td>
        </tr>
        <tr>
            <td>
                <table id="tblPratiche" class="datatable" runat="server">
                    <tr>
                      <th>Pratiche</th>
                    </tr>
                    <tr>
                        <td>
                            <table id="Table1" class="datatable" runat="server">
                                <tr>
                                    <td>
                                        <telerik:RadTreeView ID="tvwPratiche" Width="100%" runat="server" >                                      
                                            <Nodes>
                                                <telerik:RadTreeNode Text="Pratiche" Expanded="true" Font-Bold="true"></telerik:RadTreeNode>
                                            </Nodes>
                                        </telerik:RadTreeView>              
                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
</table>
</asp:Content>			




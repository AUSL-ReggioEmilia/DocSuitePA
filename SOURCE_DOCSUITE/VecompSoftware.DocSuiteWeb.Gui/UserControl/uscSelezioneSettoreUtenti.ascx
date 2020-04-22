<%@ Control AutoEventWireup="false" CodeBehind="uscSelezioneSettoreUtenti.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscSelezioneSettoreUtenti" Language="vb" %>

<div id="container" >
    <div id="spacerUp" style="height:1%"></div>
    <asp:DropDownList AutoPostBack="True" ID="ddlToken" runat="server" />
    <div id="spacer" style="height:1%"></div>
    <table cellSpacing="0" cellPadding="0" width="100%" border="0" bgcolor="#F0FFFF" style="height: 90%;">
        <tr class="Chiaro">
            <td valign="top">
                <telerik:RadTreeView CheckBoxes="true" EnableViewState="true" ExpandAnimation-Type="None" ID="Tvw" LoadingStatusPosition="BeforeNodeText" runat="server" Width="100%">
                    <CollapseAnimation Duration="100" Type="OutQuint"/>
                    <ExpandAnimation Duration="100" Type="None" />
                </telerik:RadTreeView>                
            </td>
        </tr>
    </table>
</div>
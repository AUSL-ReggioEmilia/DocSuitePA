<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscPECInfo.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscPECInfo" %>

<table class="datatable">
    <tr>
        <th colspan="2">
            <asp:Label ID="lblDetail" runat="server" />
        </th>
    </tr>
    <tr>
        <td class="label" style="width: 10%;">Da:</td>
        <td>
            <asp:Label ID="lblFrom" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="label">A:</td>
        <td>
            <asp:Label ID="lblTo" runat="server"/>
        </td>
    </tr>
    <tr>
        <td class="label">Casella:</td>
        <td>
            <asp:Label ID="lblMailBox" runat="server" />
        </td>
    </tr>
    <tr>
        <td class="label">Data:</td>
        <td>
            <asp:Label ID="lblDate" runat="server"/>
        </td>
    </tr>
    <tr>
        <td class="label">Oggetto:</td>
        <td>
            <asp:Label ID="lblSubject" runat="server" />
        </td>
    </tr>
</table>

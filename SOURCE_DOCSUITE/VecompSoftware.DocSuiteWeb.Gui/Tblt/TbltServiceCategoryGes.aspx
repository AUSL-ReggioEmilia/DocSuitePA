<%@ Page AutoEventWireup="false" Codebehind="TbltServiceCategoryGes.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltServiceCategoryGes" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script language="javascript" type="text/javascript">
            //restituisce un riferimento alla radwindow
            function GetRadWindow()
            {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }		
        	
            function CloseWindow(objOperation,objDescription,objCode,objID)
            {
                var oServiceCategory = new Object();
                oServiceCategory.Operation = objOperation;
                oServiceCategory.Name = objDescription;
                oServiceCategory.Code = objCode;
                oServiceCategory.ID = objID;
               
                var oWindow = GetRadWindow();
                oWindow.close(oServiceCategory);   
                
                return false; 
            }
        </script>

    </telerik:RadScriptBlock>
    
    <telerik:RadTreeView EnableViewState="false" ID="RadTreeViewSelectedServiceCategory" runat="server" />
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <asp:Panel ID="pnlRinomina" runat="server" Width="100%">
        <table id="Table4" class="dataform">

            <tr>
                <td class="label" width="20%">
                    <asp:Label runat="server" ID="lblOldCode" Text="Codice:" Font-Bold="true"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtOldCode" runat="server" MaxLength="100" Width="100px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="label" width="20%">
                    <asp:Label runat="server" ID="lblOldServiceCategory" Text="Descrizione:" Font-Bold="true"></asp:Label>
                </td>
                <td width="75%">
                    <telerik:RadTextBox ID="txtOldServiceCategory" runat="server" MaxLength="100" Width="100%"></telerik:RadTextBox>
                    <asp:RequiredFieldValidator ID="rfvOldServiceCategory" runat="server" ControlToValidate="txtOldServiceCategory"
                        ErrorMessage="Campo Descrizione Obbligatorio" Display="Dynamic"></asp:RequiredFieldValidator>
                </td>
            </tr>
                <tr>
                <td class="label" width="20%">
                    <asp:Label runat="server" ID="lblNewCode" Text="Codice:" Font-Bold="true"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtNewCode" runat="server" MaxLength="100" Width="100px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="label" width="20%">
                    <asp:Label runat="server" ID="lblNewServiceCategory" Text="Descrizione:" Font-Bold="true"></asp:Label>
                </td>
                <td width="75%">
                    <telerik:RadTextBox ID="txtNewServiceCategory" runat="server" MaxLength="100" Width="100%"></telerik:RadTextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="txtServiceCategory"
                        ErrorMessage="Campo Descrizione Obbligatorio" Display="Dynamic"></asp:RequiredFieldValidator>
                </td>
            </tr>
        </table>
    </asp:Panel>
    
    <asp:Panel ID="pnlInserimento" runat="server" Width="100%">
        <table class="dataform">
            <tr>
                <td class="label" width="20%">
                    <asp:Label runat="server" ID="lblCode" Text="Codice:" Font-Bold="true"></asp:Label>
                </td>
                <td>
                    <asp:TextBox ID="txtCode" runat="server" MaxLength="100" Width="100px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="label" width="20%">
                    <asp:Label runat="server" ID="lblServiceCategory" Text="Descrizione:" Font-Bold="true"></asp:Label>
                </td>
                <td width="75%">
                    <telerik:RadTextBox ID="txtServiceCategory" runat="server" MaxLength="100" Width="100%"></telerik:RadTextBox>
                    <asp:RequiredFieldValidator ControlToValidate="txtServiceCategory" Display="Dynamic" ErrorMessage="Campo Descrizione Obbligatorio" ID="rfvServiceCategory" runat="server" />
                </td>
            </tr>
        </table>
    </asp:Panel>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="btnConferma" runat="server" Text="Conferma" />
</asp:Content>

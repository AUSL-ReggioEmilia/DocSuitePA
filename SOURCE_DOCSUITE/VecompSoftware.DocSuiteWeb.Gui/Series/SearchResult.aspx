<%@ Page AutoEventWireup="false" CodeBehind="SearchResult.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.Series.SearchResult" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>
<%@ Import Namespace="VecompSoftware.DocSuiteWeb.Data" %>

<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadScriptBlock runat="server" ID="rsbContent" EnableViewState="false">
        <script language="javascript" type="text/javascript">
            //restituisce un riferimento alla radwindow
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function CloseWindow(argument) {
                var oWindow = GetRadWindow();
                oWindow.close(argument);
            }

            function pageLoad(sender, eventArgs) {
                if (!eventArgs.get_isPartialLoad()) {
                    $find("<%= AjaxManager.ClientID %>").ajaxRequest("InitialPageLoad");
                }
            }
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">
    <div runat="server" style="overflow:hidden;width:100%;height:100%;">
        <DocSuite:BindGrid ID="grdDocumentSeriesItem" runat="server" AutoGenerateColumns="false" AllowMultiRowSelection="false" GridLines="Both">
            <MasterTableView TableLayout="Auto" AllowFilteringByColumn="true" GridLines="Both">
                <Columns>
                   <telerik:GridTemplateColumn UniqueName="Select" AllowFiltering="false" Groupable="false">
                        <HeaderStyle HorizontalAlign="Center" Width="30"/>
                        <ItemStyle HorizontalAlign="Center" Width="30"/>
                        <ItemTemplate>
                            <asp:CheckBox ID="chkSelect" runat="server" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="MainDocument" HeaderText="Documento" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/card_chip_gold.png" AllowFiltering="false" Groupable="false">
                        <HeaderStyle HorizontalAlign="Center" CssClass="headerImage" />
                        <ItemStyle HorizontalAlign="Center" CssClass="cellImage"/>
                        <ItemTemplate>
                            <asp:ImageButton ID="ibtMainDocument" runat="server" AlternateText="Visualizza documenti"
                                 ImageUrl="~/App_Themes/DocSuite2008/imgset16/document_copies.png"
                                onmouseover="this.style.cursor='hand';" onmouseout="this.style.cursor='default';"/>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn UniqueName="YearNumber" HeaderText="Anno/Numero" AllowFiltering="false" Groupable="false" SortExpression="Id">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center"/>
                        <ItemTemplate>
                            <asp:LinkButton ID="lbtYearNumber" runat="server" CommandName="ViewDocumentSeriesItem" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridDateTimeColumn  UniqueName="RegistrationDate" DataField="RegistrationDate" HeaderText="Data registrazione" AllowFiltering="false" Groupable="false" DataFormatString="{0:dd/MM/yyyy}" SortExpression="RegistrationDate">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center"/>
                    </telerik:GridDateTimeColumn>
                    
                    <telerik:GridTemplateColumn  UniqueName="RegistrationUser" HeaderText="Utente" AllowFiltering="false" Groupable="false" SortExpression="RegistrationUser">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                          <ItemTemplate>
                            <asp:Label runat="server" ID="lblRegistrationUser"/>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>

                    <telerik:GridBoundColumn UniqueName="DocumentSeries" DataField="ContainerName" HeaderText="Serie" AllowFiltering="false" Groupable="false" SortExpression="ContainerName">
                        <HeaderStyle HorizontalAlign="Center" />
                    </telerik:GridBoundColumn>

                    <telerik:GridTemplateColumn UniqueName="OwnerRoles" HeaderText="Sett. appartenenza" HeaderTooltip="Settori di appartenenza" AllowFiltering="false" Groupable="false">
                        <HeaderStyle HorizontalAlign="Left" />
                        <ItemStyle HorizontalAlign="Left" />
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblRoles"/>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>

                    <telerik:GridBoundColumn UniqueName="Subject" DataField="Subject" HeaderText="Oggetto" AllowFiltering="false" Groupable="false" SortExpression="Subject">
                        <HeaderStyle HorizontalAlign="Center" />
                    </telerik:GridBoundColumn>
                    <telerik:GridDateTimeColumn UniqueName="PublishingDate" DataField="PublishingDate" HeaderText="Data pubblicazione" AllowFiltering="false" Groupable="false" DataFormatString="{0:dd/MM/yyyy}" SortExpression="PublishingDate">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </telerik:GridDateTimeColumn>
                    <telerik:GridDateTimeColumn UniqueName="RetireDate" DataField="RetireDate" HeaderText="Data ritiro" AllowFiltering="false" Groupable="false" DataFormatString="{0:dd/MM/yyyy}" SortExpression="RetireDate">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center"/>
                    </telerik:GridDateTimeColumn>
                </Columns>
            </MasterTableView>
            <ClientSettings>
                <Selecting AllowRowSelect="true" />
<%--                <Scrolling AllowScroll="true" UseStaticHeaders="true" />--%>
            </ClientSettings>
        </DocSuite:BindGrid>
    </div>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <telerik:RadScriptBlock runat="server">
        <script type="text/javascript">

            function ToggleCheckBoxes() {
                var checkboxes = $("input[type='checkbox']");
                if (checkboxes && checkboxes.length > 0)
                    checkboxes.attr("checked", !checkboxes.attr("checked"));
            }

            function ValidateCheckBoxes() {
                var checked = $("input[type='checkbox']:checked");
                if (checked && checked.length > 0)
                    return true;

                alert("E' necessario selezionare almeno una <%= DocsuiteContext.Current.ProtocolEnv.DocumentSeriesName%>.");
                return false;
            }

            function ConfirmSelection(message) {
                if (!ValidateCheckBoxes())
                    return false;

                if (!confirm(message))
                    return false;
            }

        </script>
    </telerik:RadScriptBlock>
    <div runat="server" id="ButtonsWrapper">
        <input type="button" id="cmdCheckAll" value="Seleziona/Deseleziona tutti" onclick="ToggleCheckBoxes();" />
        <asp:Button ID="cmdViewDocuments" runat="server" Text="Visualizza documenti" OnClientClick="return ValidateCheckBoxes();" />
        <asp:Button ID="cmdPublish" runat="server" Text="Pubblica" Width="150px" OnClientClick="ConfirmSelection('Si è scelto di eseguire una pubblicazione massiva, procedere?');" />
        <asp:Button ID="cmdRetire" runat="server" Text="Ritira" Width="150px" OnClientClick="ConfirmSelection('Si è scelto di eseguire un ritiro massivo, procedere?');" />
    </div>
</asp:Content>

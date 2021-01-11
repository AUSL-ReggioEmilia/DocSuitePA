<%@ Page Title="Gestione serie documentali" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="UtltDocumentSeries.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UtltDocumentSeries" %>
<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
       <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript" language="javascript">
        

            function getAjaxManager() {
                return $find("<%= RadAjaxManager.GetCurrent(Page).ClientID%>");
            }

            function AjaxRequest(request) {
                var manager = getAjaxManager();
                manager.ajaxRequest(request);
                return false;
            }

            function openYearAddWindow() {
                Sys.Application.remove_load(openYearSelectWindow);
                var oWndYear = $find("<%=windowsAddYear.ClientID%>");
                oWndYear.show();
                return false;
            }
               
               
            function clientCloseYearAddWindow(arg) {
                Sys.Application.remove_load(clientCloseYearSelectWindow);
                var oWndYear = $find("<%=windowsAddYear.ClientID%>");
                oWndYear.close(arg);
            }

        
            
        </script>
       
    </telerik:RadCodeBlock>
</asp:Content>
<asp:Content ContentPlaceHolderID="cphContent" runat="server">
        
        <telerik:RadWindow ID="windowsAddYear" Title="Anno di registrazione" runat="server" Behaviors="Close" Height="120" Width="350" >
               <ContentTemplate>
                   <div class="titolo">
                        <p>Aggiunta anno di registrazione</p>
                    </div>
                    <div>                     
                      <p>Verrà aggiunto l'anno di registrazione successivo all'ultimo</p>
                        <br />
                        <asp:Button ID="cmdYear" runat="server" Text="Aggiungi"  Width="100" />
                   </div>
                   
               </ContentTemplate>
           </telerik:RadWindow>
            <telerik:RadWindow ID="windowsJustAddYear" Title="Anno di registrazione" runat="server" Behaviors="Close" Height="120" Width="350" >
               <ContentTemplate>
                   <div class="titolo">
                        <p>Aggiunta anno di registrazione</p>
                    </div>
                    <div>                     
                      <p>L'anno di registrazione successivo al corrente è già stato aggiunto.</p>
                        <br />
                       
                   </div>
                   
               </ContentTemplate>
           </telerik:RadWindow>
        <telerik:RadWindow ID="UpdateDocumentalSeries" Title="Modifica serie documentale" runat="server" Behaviors="Close" Height="150" Width="350" >
               <ContentTemplate>
                   <div class="titolo">
                        <p>Modifica stato serie documentale</p>
                    </div>
                    <div>    
                         <br />
                                
                        <asp:Button ID="btnUpdateDocSeriesStatus" runat="server"  />
                   </div>
                   
               </ContentTemplate>
           </telerik:RadWindow>
     <telerik:RadWindow ID="RadWindowAlert" Title="Modifica serie documentale" runat="server" Behaviors="Close" Height="120" Width="350" >
               <ContentTemplate>
                   <div class="titolo">
                        <p>Errore</p>
                    </div>
                    <div>    
                         <p>E' possibile seleziona solo una riga</p>
                   </div>
                   
               </ContentTemplate>
           </telerik:RadWindow>

   <table class="datatable" runat="server" id="tblSeries" visible="true">
       
        <tr>
 
             <td class="label">Tipo:</td>
            <td>
                <asp:DropDownList runat="server" CausesValidation="false" ID="ddlContainerArchive" AutoPostBack="True" Visible="True" Width="300px" />
                
            </td>
        </tr>
         <tr>
            <td class="label">Archivi:</td>
            <td>
               <asp:DropDownList runat="server" CausesValidation="false" ID="ddlDocumentSeries" AutoPostBack="True" Visible="True" Width="300px" />
            </td>
        </tr>
    </table>
   
  <telerik:RadGrid runat="server" ID="dgDocSeries" Width="100%" EnableViewState="true" AllowMultiRowSelection="True" >
        <MasterTableView AutoGenerateColumns="False" DataKeyNames="Id" >     
      
            <Columns>
                 <telerik:GridClientSelectColumn UniqueName="ClientSelectColumn">
                     <ItemStyle width="20px" />                    
                </telerik:GridClientSelectColumn>
                   <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="Id" UniqueName="docSeriesId" Visible="false">
                    <HeaderStyle HorizontalAlign="Center" Wrap="false" />
                    <ItemStyle HorizontalAlign="left" />
                    <ItemTemplate>
                         <asp:Label runat="server" ID="lblId"/>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                  <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="Serie Documentale" UniqueName="docSeriesName">
                    <HeaderStyle HorizontalAlign="Center" Wrap="false" />
                    <ItemStyle HorizontalAlign="left" />
                    <ItemTemplate>
                         <asp:Label runat="server" ID="lblDocumentSeries"/>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                 <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="Anno riferimento" UniqueName="docSerieYear">
                    <HeaderStyle HorizontalAlign="Center"  Wrap="false" />
                    <ItemStyle HorizontalAlign="Center" Width="25px" />
                    <ItemTemplate>
                         <asp:Label runat="server" ID="lblYear"/>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="Ultimo numero usato" UniqueName="docSeriesLastUSedNumber">
                    <HeaderStyle HorizontalAlign="Center"  Wrap="false"  />
                    <ItemStyle HorizontalAlign="Center" Width="25px" />
                    <ItemTemplate>
                         <asp:Label runat="server" ID="lblLastUsedNumber"/>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false"  HeaderText="Aperto" UniqueName="docSeriesIsOpen">
                    <HeaderStyle HorizontalAlign="Center"   Wrap="false"  />
                    <ItemStyle HorizontalAlign="Center" Width="25px" />
                    <ItemTemplate>
                        <asp:Image ID="chkIsOpen" runat="server" ImageUrl="~/App_Themes/DocSuite2008/imgset16/accept.png" />
                          </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
     <ClientSettings EnablePostBackOnRowClick="true" Selecting-AllowRowSelect="true" /> 
    
       </telerik:RadGrid>

  
       
         
      
        
   
</asp:Content>
<asp:Content  ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Panel ID="pnlButtons" runat="server">
              <asp:Button ID="cmdUpdate" runat="server" Text="Modifica" Width="150" />
              <asp:Button ID="cmdAddYear" runat="server" Text="Aggiungi anno"  Width="150" />
    </asp:Panel>
      
</asp:Content>
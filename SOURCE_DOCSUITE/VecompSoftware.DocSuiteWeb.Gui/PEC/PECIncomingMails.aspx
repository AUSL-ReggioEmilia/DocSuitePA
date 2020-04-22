<%@ Page AutoEventWireup="false" CodeBehind="PECIncomingMails.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.PECIncomingMails" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">
            
           function pageLoad(sender, eventArgs) {
                if (!eventArgs.get_isPartialLoad()) {
                    $find("<%= AjaxManager.ClientID %>").ajaxRequest("InitialPageLoad");
                }
            }
            
            //<![CDATA[

            function OpenWindow(mailId) {
                OpenGenericWindow("<%=PecMailContentUrl() %>?Type=Pec&PECId=" + mailId);
            }

            function OpenGenericWindow(url) {
                var wnd = window.radopen(url, null);

                wnd.setSize(<%=WindowWidth() %>, <%=WindowHeight() %>);
                wnd.add_close(OnClientClose);

                wnd.set_behaviors(<%=WindowBehaviors() %>);
                wnd.set_visibleStatusbar(false);
                wnd.set_modal(true);
                wnd.set_iconUrl("images/mail.gif");
                wnd.set_showOnTopWhenMaximized(false);
                wnd.set_destroyOnClose(true);
                <%=WindowPosition() %>;
                return wnd;
            }

            function OnClientClose(sender, eventArgs) {
                var currentArgument = sender.argument;
                if (currentArgument && isNaN(currentArgument)) {
                    // Ricevuta stringa come argomento di chiusura
                    var tokens = currentArgument.split("|");
                    switch (tokens[0]) {
                        case 'REFRESH':
                            document.getElementById("<%= cmdAggiornaDgMail.ClientID %>").click();
                            break;
                        case 'NONE':
                        default:
                            break;
                    }
                }
                sender.remove_close(OnClientClose);
            }
          
            function UpdateGrid() {
                document.getElementById("<%= cmdAggiornaDgMail.ClientID %>").click();
        }


        function RefreshNode() {
            var ajaxManager = $find("<%= AjaxManager.ClientID %>");
                ajaxManager.ajaxRequest('Refresh');
        }

       
        
            function ShowLoadingPanel() {
                var currentLoadingPanel = $find("<%= MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                var currentUpdatedControl = "<%= dgMail.ClientID%>";
                currentLoadingPanel.show(currentUpdatedControl);


                var ajaxFlatLoadingPanel = $find("<%= MasterDocSuite.AjaxFlatLoadingPanel.ClientID%>");
                var pnlButtons = "<%= pnlButtons.ClientID%>";
                ajaxFlatLoadingPanel.show(pnlButtons);
            }

            function collapseHeader(){
                if ( $("#headerSection").is(":hidden")) {
                    $("#headerSection").slideDown("slow");
                    $("#collapseButton").removeClass("arrow-down").addClass("arrow-up");
                    var grid = $find("<%= dgMail.ClientID %>");
                    if(grid != undefined)
                    {
                        var masterTable = $find("<%= dgMail.ClientID %>").get_masterTableView();
                        masterTable.rebind();
                    }
                } else {
                    $("#headerSection").hide();
                    $("#collapseButton").removeClass("arrow-up").addClass("arrow-down");
                    var grid = $find("<%= dgMail.ClientID %>");
                    if(grid != undefined)
                    {
                        var masterTable = $find("<%= dgMail.ClientID %>").get_masterTableView();
                        masterTable.rebind();
                    }
                }
            }
            //]]> 
        </script>
    </telerik:RadCodeBlock>
    <%--Header--%>
    <table class="datatable">
         <tr class="Chiaro" id="headerSection">
            <td style="width: 50%; border-right: 1px solid">
                <asp:Panel runat="server" ID="pnlFilterLeft">
                    <table cellspacing="0" cellpadding="2" style="border: 0; width: 100%">
                        <tr>
                            <td style="width: 150px; vertical-align: middle; font-size: 8pt; text-align: right;">
                                <b>
                                    <label runat="server" id="ddlMailBoxLabel"></label>
                                </b>
                            </td>
                            <td style="vertical-align: middle; font-size: 8pt">
                                <asp:DropDownList AutoPostBack="true" DataTextField="MailBoxName" DataValueField="Id" ID="ddlMailbox" runat="server" Width="300" />
                            </td>
                        </tr>
                        <tr id="trFilterSender" runat="server">
                            <td style="vertical-align: middle; font-size: 8pt; text-align: right;">
                                <b>Mittente:</b>
                            </td>
                            <td style="vertical-align: middle; font-size: 8pt">
                                <asp:TextBox ID="txtFilterSender" runat="server" Width="300" />
                            </td>
                        </tr>
                        <tr>
                            <td style="vertical-align: middle; font-size: 8pt; text-align: right;">
                                <b>Oggetto:</b>
                            </td>
                            <td style="vertical-align: middle; font-size: 8pt">
                                <asp:TextBox ID="txtFilterSubject" runat="server" Width="300" />
                            </td>
                        </tr>
                        <tr>
                            <td style="vertical-align: middle; font-size: 8pt; text-align: right;">
                                <b>Ricevuto dal:</b>
                            </td>
                            <td style="vertical-align: middle; font-size: 8pt">
                                <telerik:RadDatePicker ID="dtpMailDateFrom" runat="server" />
                                <span class="miniLabel">al:</span>
                                <telerik:RadDatePicker ID="dtpMailDateTo" runat="server" />
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 150px; vertical-align: middle; font-size: 8pt; text-align: right;">
                                <b>Provider PEC:</b>
                            </td>
                            <td style="vertical-align: middle; font-size: 8pt">
                                <asp:CheckBox runat="server" ID="chkAnomalies" Text="Includi PEC rimaste nella posta del provider" Checked="true" />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <hr />
                            </td>
                        </tr>
                        <tr>
                            <td colspan="2">
                                <div>
                                    <asp:Button ID="cmdAggiornaDgMail" runat="server" Text="Aggiorna visualizzazione" Width="200" />
                                    <asp:Button ID="cmdClearFilters" runat="server" Text="Azzera filtri" />
                                    <asp:CheckBox AutoPostBack="True" ID="chkVisCestino" runat="server" Text="Visualizzazione cestino" />
                                </div>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
            <td style="width: 50%">
                <asp:Panel runat="server" ID="pnlFilterRight">
                    <table id="tblFilterVisualizza" runat="server" cellspacing="0" cellpadding="2" border="0" width="100%">
                        <tr>
                            <td style="width: 150px; vertical-align: middle; font-size: 8pt; text-align: right;">
                                <b>Visualizza:</b>
                            </td>
                            <td style="vertical-align: middle; font-size: 8pt">
                                <asp:RadioButtonList ID="rdbShowRecorded" runat="server" RepeatDirection="Horizontal">
                                    <asp:ListItem Text="Tutti i messaggi" />
                                    <asp:ListItem Text="Da gestire" />
                                    <asp:ListItem Text="Gestiti" />
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                    </table>
                    <table id="tblFilterVisualizzaDate" runat="server" cellspacing="0" cellpadding="2" border="0" width="100%">
                        <tr>
                            <td style="width: 150px; vertical-align: middle; font-size: 8pt; text-align: right;">
                                <b>Dal:</b>
                            </td>
                            <td style="vertical-align: middle; font-size: 8pt">
                                <telerik:RadDatePicker ID="dtpShowRecordedFrom" runat="server" />
                                <span><b>al:</b></span>
                                <telerik:RadDatePicker ID="dtpShowRecordedTo" runat="server" />
                            </td>
                        </tr>
                    </table>
                    <hr />
                    <table id="tblFilterDestinati" runat="server" cellspacing="0" cellpadding="2" border="0" width="100%">
                        <tr>
                            <td style="width: 150px; vertical-align: middle; font-size: 8pt; text-align: right;">
                                <b>Destinati:</b>
                            </td>
                            <td style="vertical-align: middle; font-size: 8pt;">
                                <asp:CheckBox runat="server" ID="chkDaDestinare" Text="Visualizza anche da destinare" Style="display: none" />
                                <asp:CheckBox runat="server" ID="chkDestinati" Text="Visualizza destinati" Style="display: none" />
                            </td>
                        </tr>
                    </table>
                    <table id="tblFilterHandler" runat="server" cellspacing="0" cellpadding="2" border="0" width="100%" style="display: none;">

                        <tr>
                            <td style="width: 150px; vertical-align: middle; font-size: 8pt; text-align: right;">
                                <b>In gestione:</b>
                            </td>
                            <td style="vertical-align: middle; font-size: 8pt">
                                <asp:CheckBox AutoPostBack="False" Checked="true" ID="chkShowHandled" runat="server" Text="(Visualizza anche PEC in carico ad altri operatori)" />
                            </td>
                        </tr>
                    </table>
                    <table cellspacing="0" cellpadding="2" border="0" width="100%">
                        <tr id="trFilterType" runat="server">
                            <td style="width: 150px; vertical-align: middle; font-size: 8pt; text-align: right;">
                                <b>Tipologia:</b>
                            </td>
                            <td style="vertical-align: middle; font-size: 8pt">
                                <asp:CheckBox runat="server" ID="chkTypePEC" Text="PEC" Checked="false" />
                                <asp:CheckBox runat="server" ID="chkTypeAnomalia" Text="Mail" Checked="false" />
                                <asp:CheckBox runat="server" ID="chkTypeNotify" Text="Notifica" Checked="false" />
                            </td>
                        </tr>
                        <tr id="trFilterOrder" runat="server">
                            <td style="width: 150px; vertical-align: middle; font-size: 8pt; text-align: right;">
                                <b>Ordinamento:</b>
                            </td>
                            <td style="vertical-align: middle; font-size: 8pt">
                                <asp:CheckBox runat="server" ID="chkInverted" Text="(Dalla meno recente)" Checked="false" />
                            </td>
                        </tr>
                        <tr id="tr1" runat="server">
                            <td style="width: 150px; vertical-align: middle; font-size: 8pt; text-align: right;">
                                <b>Ricevute altri invii:</b>
                            </td>
                            <td style="vertical-align: middle; font-size: 8pt">
                                <asp:CheckBox runat="server" ID="chkReceiptNotLinked" Checked="false" AutoPostBack="true" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
        </tr>        
         <tr class="Chiaro" runat="server" visible="false" id="collapseSection">
            <td colspan="2">
                <div id="collapseButton" style="float:right;margin-left:5px;margin-right:10px;" onclick="collapseHeader();" class="pec arrow-up"></div>
            </td>
        </tr>
    </table>
    <table class="datatable" width="100%">
       
    </table>
</asp:Content>
<%-- Content --%>
<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <div style="overflow: hidden; width: 100%; height: 100%;">
        <DocSuite:BindGrid AllowFilteringByColumn="false" AllowMultiRowSelection="true" AutoGenerateColumns="False" GridLines="Both" ID="dgMail" PageSize="20" runat="server" ShowGroupPanel="True">
            <MasterTableView CommandItemDisplay="None" CurrentResetPageIndexAction="SetPageIndexToFirst" DataKeyNames="Id,IsActive" Dir="LTR" TableLayout="Fixed">
                <ItemStyle CssClass="Scuro" />
                <AlternatingItemStyle CssClass="Chiaro" />
                <Columns>
                    <telerik:GridClientSelectColumn UniqueName="Select" HeaderStyle-CssClass="headerImage" ItemStyle-CssClass="cellImage" />
                    <%-- Visualizza sommario --%>
                    <telerik:GridTemplateColumn HeaderStyle-CssClass="headerImage" AllowFiltering="false" Groupable="false" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/mail_blue.png" HeaderText="Visualizza sommario" UniqueName="ViewSummary">
                        <ItemStyle CssClass="cellImage" />
                        <ItemTemplate>
                            <div style="position: relative">
                                <telerik:RadButton AlternateText="Visualizza sommario" runat="server" ID="cmdViewSummary" CommandName="ViewSummary" Height="16px" Width="16px"/>
                            </div>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <%-- Visualizza documenti --%>
                    <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/documents_email.png" HeaderText="Visualizza documenti" UniqueName="ViewDocs">
                        <HeaderStyle HorizontalAlign="Left" CssClass="headerImage" />
                        <ItemStyle HorizontalAlign="Left" CssClass="cellImage" />
                        <ItemTemplate>
                            <div style="position: relative">
                                <telerik:RadButton AlternateText="Visualizza documenti" CommandName="ViewDocs" ID="cmdViewDocs" runat="server" Height="16" Width="16"/>
                            </div>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <%-- Numero allegati --%>
                    <telerik:GridBoundColumn DataField="AttachmentsCount" Groupable="False" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/documents_email_attach.png" HeaderText="Numero allegati" UniqueName="AttachmentsCount">
                        <HeaderStyle HorizontalAlign="Center" Width="30" CssClass="headerImage" />
                        <ItemStyle HorizontalAlign="Center" Width="30" />
                    </telerik:GridBoundColumn>
                    <%-- ImgDirection --%>
                    <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderImageUrl="../Prot/Images/Mail16_IU.gif" HeaderText="Direzione" UniqueName="cDirection">
                        <HeaderStyle HorizontalAlign="Center" CssClass="headerImage" />
                        <ItemStyle HorizontalAlign="Center" CssClass="cellImage" />
                        <ItemTemplate>
                            <asp:Image runat="server" ID="imgDirection" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <%-- ImgPriority --%>
                    <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderImageUrl="../Comm/Images/Mails/highimportance.gif" HeaderText="Priorità" UniqueName="cPriority">
                        <HeaderStyle HorizontalAlign="Center" Width="30" CssClass="headerImage" />
                        <ItemStyle HorizontalAlign="Center" Width="30" CssClass="cellImage" />
                        <ItemTemplate>
                            <asp:Image runat="server" ID="imgPriority" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <%-- ImgSegnatura --%>
                    <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/mail_blue.png" HeaderText="Interoperabilità" UniqueName="cInterop">
                        <HeaderStyle HorizontalAlign="Center" Width="30" CssClass="headerImage" />
                        <ItemStyle HorizontalAlign="Center" Width="30" CssClass="cellImage" />
                        <ItemTemplate>
                            <asp:Image runat="server" ID="imgInterop" AlternateText="Interoperabilità" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <%-- Link Protocollo --%>
                    <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="Protocollo" UniqueName="cProtocol">
                        <HeaderStyle HorizontalAlign="Center" Width="130px" />
                        <ItemStyle HorizontalAlign="Center" Width="130px" />
                        <ItemTemplate>
                            <div style="position: relative">
                                <telerik:RadButton ButtonType="LinkButton" ID="cmdProtocol" runat="server" />
                            </div>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <%-- ImgIsPEC --%>
                    <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/document_signature.png" HeaderText="PEC" UniqueName="cIsPEC">
                        <HeaderStyle HorizontalAlign="Center" CssClass="headerImage" />
                        <ItemStyle HorizontalAlign="Center" CssClass="cellImage" />
                        <ItemTemplate>
                            <asp:Image runat="server" ID="imgIsPEC" AlternateText="Da PEC" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <%-- ImgMoved --%>
                    <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/move_to_folder.png" HeaderText="PEC Spostata" UniqueName="cMoved">
                        <HeaderStyle HorizontalAlign="Center" CssClass="headerImage" />
                        <ItemStyle HorizontalAlign="Center" CssClass="cellImage" />
                        <ItemTemplate>
                            <asp:Image runat="server" ID="imgMoved" AlternateText="Mail spostata" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <%-- Risposta --%>
                    <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/email_reply.png" HeaderText="Risposta spedita" UniqueName="Reply">
                        <HeaderStyle HorizontalAlign="Left" CssClass="headerImage" />
                        <ItemStyle HorizontalAlign="Left" CssClass="cellImage" />
                        <ItemTemplate>
                            <div style="position: relative">
                                <telerik:RadButton AlternateText="Risposta spedita" CommandName="ViewReplay" Height="16px" ID="imgViewReplay" runat="server" Width="16px" />
                            </div>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <%-- Inoltrata --%>
                    <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/email_forward.png" HeaderText="Inoltro eseguito" UniqueName="Forward">
                        <HeaderStyle HorizontalAlign="Left" CssClass="headerImage" />
                        <ItemStyle HorizontalAlign="Left" CssClass="cellImage" />
                        <ItemTemplate>
                            <div style="position: relative">
                                <telerik:RadButton AlternateText="Inoltro eseguito" CommandName="ViewForward" Height="16px" ID="imgViewForward" runat="server" Width="16px" />
                            </div>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <%-- In carico --%>
                    <telerik:GridTemplateColumn AllowFiltering="false" DataField="Handler" HeaderText="In carico a" SortExpression="Handler" UniqueName="Handler">
                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="100px" />
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="100px" />
                        <ItemTemplate>
                            <asp:Label ID="lblHandler" runat="server"></asp:Label>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <%-- Ricevuta come CC --%>
                    <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/mail_send.png" HeaderText="Ricezione come Copia Conoscenza" UniqueName="cIsCc">
                        <HeaderStyle HorizontalAlign="Center" CssClass="headerImage" />
                        <ItemStyle HorizontalAlign="Left" CssClass="cellImage" />
                        <ItemTemplate>
                            <div style="position: relative">
                            <asp:Image runat="server" ID="imgCc" Width="16px" Height="16px" AlternateText="Mail ricevuta come CC" />
                                </div>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <%-- Mittente --%>
                    <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="Da" UniqueName="Da">
                        <HeaderStyle HorizontalAlign="Center"  Width="200px"/>
                        <ItemStyle HorizontalAlign="Left" Width="200px" />
                        <ItemTemplate>
                            <asp:Label ID="lblAddress" runat="server"></asp:Label>
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                   <%--Destinatari--%>
                    <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="A" UniqueName="colMailBoxName" DataField="MailBoxName">
                        <HeaderStyle HorizontalAlign="Center" Width="210px"/>
                        <ItemStyle HorizontalAlign="Left"/>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblMailBoxName" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="A" UniqueName="colRecipients" DataField="MailRecipients">
                        <HeaderStyle HorizontalAlign="Center" Width="210px"/>
                        <ItemStyle HorizontalAlign="Left"/>
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblRecipient" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <%-- Oggetto --%>
                    <telerik:GridTemplateColumn DataField="MailSubject" Groupable="False" HeaderText="Oggetto" UniqueName="MailSubject">
                        <HeaderStyle HorizontalAlign="Center" Width="300px" />
                        <ItemStyle HorizontalAlign="Left" />
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblSubject" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                    <%-- Ricevuto --%>
                    <telerik:GridDateTimeColumn DataField="MailDate" DataFormatString="{0:dd/MM/yyyy HH:mm:ss tt}" DataType="System.DateTime" HeaderText="Ricevuto" SortExpression="MailDate" UniqueName="MailDate">
                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="80px" />
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle"/>
                    </telerik:GridDateTimeColumn>
                    <%-- Dimensione --%>
                    <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="Dimensione" UniqueName="PecSize">
                        <HeaderStyle HorizontalAlign="Center" Width="80px" />
                        <ItemStyle HorizontalAlign="Right" />
                        <ItemTemplate>
                            <asp:Label runat="server" ID="lblPecSize" />
                        </ItemTemplate>
                    </telerik:GridTemplateColumn>
                </Columns>
            </MasterTableView>
            <ClientSettings>
                <Selecting AllowRowSelect="True" CellSelectionMode="None" UseClientSelectColumnOnly="True" EnableDragToSelectRows="False" />                
            </ClientSettings>
            <SortingSettings SortedAscToolTip="Ordine Crescente" SortedDescToolTip="Ordine Descrescente" SortToolTip="Ordina" />
        </DocSuite:BindGrid>
    </div>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Panel runat="server" ID="pnlButtons">
        <div>
            <asp:Button ID="cmdDocuments" runat="server" Text="Visualizza" OnClientClick="ShowLoadingPanel();" Width="150" />
            <asp:Button ID="cmdNewMail" runat="server" OnClientClick="ShowLoadingPanel();" Width="150" />
            <asp:Button ID="cmdDelete" runat="server" Text="Elimina" OnClientClick="ShowLoadingPanel();" Width="150" />
            <asp:Button ID="cmdMove" runat="server" Text="Sposta" OnClientClick="ShowLoadingPanel();" Width="150" />
            <asp:Button ID="cmdRestore" runat="server" Text="Ripristina" Width="150" />
            <asp:Button ID="cmdSvuotaCestino" runat="server" Text="Svuota cestino" Width="150" />
            <asp:Button ID="cmdClonePec" runat="server" Width="150" OnClientClick="ShowLoadingPanel();" />
            <asp:Button ID="cmdForward" runat="server" Width="150" OnClientClick="ShowLoadingPanel();" />
            <asp:Button ID="cmdViewLog" runat="server" Text="Visualizza log" Width="150" OnClientClick="ShowLoadingPanel();" />
            <asp:Button ID="cmdAttach" runat="server" Text="Allega" Width="150" OnClientClick="ShowLoadingPanel();" PostBackUrl="../PEC/PECAttachToDocumentUnit.aspx?Type=PEC" />
            <asp:Button ID="cmdHandle" runat="server" Style="display: none" Text="Gestita" OnClientClick="ShowLoadingPanel();" Width="150" />
        </div>
        </asp:Panel>
</asp:Content>

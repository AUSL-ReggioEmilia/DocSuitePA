<%@ Page AutoEventWireup="false" CodeBehind="PECOutgoingMails.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.PECOutgoingMails" Language="vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
       
    <telerik:RadWindowManager EnableViewState="false" ID="RadWindowManager" runat="server">
        <Windows>
            <telerik:RadWindow Behaviors="None" Height="550" ID="wndProgress" runat="server" Width="700" />
        </Windows>
    </telerik:RadWindowManager>

    <%-- Javascript non ajaxified --%>
    <script type="text/javascript">
        $(document).ready(function () {
            $("#imgWait").hide(); 
        });

        function runModule(jeepUrl, sModuleName) {
            $.ajaxSetup({
                beforeSend: function () {
                    $("#btnLaunchAsyn").attr("disabled", "disabled");
                    $("#imgWait").show();
                },
                complete: function () {
                    $("#btnLaunchAsyn").attr("disabled", null);
                    $("#imgWait").hide();
                }
            });
            $.ajax({
                type: "POST",
                url: jeepUrl,
                data: '{"nameMod":"' + sModuleName + '"}',
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg) {
                    if (msg.d == false)
                        alert("PECMailService non eseguito");
                    else
                        __doPostBack('ctl00$cphContent$cmdRefreshGrid', '');
                },
                error: function (a, b, c) {
                    if (a.status == 500)
                        alert("Connessione a servizio jeep non riuscita");
                    else
                        alert("Operazione di aggiornamento già eseguita.");
                }
            });           
        }


    </script>
    <%--Ajaxified Javascript--%>
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
        <script type="text/javascript">

            function OpenSelectOpWindow(url, name) {
                var grid = $find("<%=dgMail.ClientID %>");
                var MasterTable = grid.get_masterTableView();
                var selectedRows = MasterTable.get_selectedItems();
                var keys = "" 
                for (var i = 0; i < selectedRows.length; i++) {
                    var row = selectedRows[i];
                    keys += row.getDataKeyValue("Id") +"|";
                }
                var wnd = window.radopen(url+"?keys="+keys, name);
                wnd.setSize(300, 250);
                wnd.set_behaviors(<%=WindowBehaviors() %>);
                wnd.set_visibleStatusbar(false);
                wnd.set_modal(true);
                wnd.set_iconUrl("images/mail.gif");
                wnd.set_destroyOnClose(true);
                <%=WindowPosition() %>;
                return wnd;
            }

            function OpenGenericWindow(url, name) {
                var wnd = window.radopen(url, name);
                wnd.setSize(<%=WindowWidth() %>, <%=WindowHeight() %>);
                wnd.set_behaviors(<%=WindowBehaviors() %>);
                wnd.set_visibleStatusbar(false);
                wnd.set_modal(true);
                wnd.set_iconUrl("images/mail.gif");
                wnd.add_close(SimpleClose);
                wnd.set_destroyOnClose(true);
                <%=WindowPosition() %>;
                return wnd;
            }

            function OpenWindow(mailId) {
                var url = "<%=PECMailViewerUrl() %>";
                var queryString = "?Type=Pec&PECId=" + mailId;

                window.setTimeout(function() {
                    var wnd = window.radopen(url + queryString, "windowViewMail");
                    wnd.setSize(700, 480);
                    wnd.set_behaviors(<%=WindowBehaviors() %>);
                    wnd.set_showOnTopWhenMaximized(false);
                    wnd.set_visibleStatusbar(false);
                    wnd.set_modal(true);
                    wnd.add_close(OnClientClose);
                    wnd.center();
                }, 0);
            }
            
            function SimpleClose(sender,args) {
                sender.remove_close(SimpleClose);
                var ajaxManager = $find("<%= AjaxManager.ClientID%>");
                ajaxManager.ajaxRequest("refresh");
            }

            function OnClientClose(sender, args) {
                sender.remove_close(OnClientClose);
                sender.SetUrl('about:blank');
                var currentArgument = sender.argument;
                // non ho ricevuto argomenti, eseguo il refresh
                if (!currentArgument || !isNaN(currentArgument)) {
                    document.getElementById("<%= cmdRefreshGrid.ClientID %>").click();
                    return true;
                }
                // Ricevuta stringa come argomento di chiusura
                var tokens = currentArgument.split("|");
                if (tokens.length < 2) {
                    window.location.href = currentArgument;
                    return true;
                }
                
                var action = tokens[0];
                var val = tokens[1];
                switch (action) {
                case 'REFRESH':
                    document.getElementById("<%= cmdRefreshGrid.ClientID %>").click();
                    break;
                case 'LINK':
                    window.location.href = val;
                    break;
                case 'NONE':
                    break;
                default:
                    // mando tutto l'argument come pagina di landing
                    window.location.href = currentArgument;
                    break;
                }
                return true;
            }

            var id_btnSendRaccomandata = '#<%= btnOpenModalRaccomandata.ClientID%>';
            var id_btnExport = '#<%= btnExport.ClientID%>';
            var id_cmdResend = '#<%= cmdResend.ClientID%>';
            function onRowDeselected(sender, args) {
                var grid = sender;
                var MasterTable = grid.get_masterTableView();
                var selectedRows = MasterTable.get_selectedItems();
                if (selectedRows.length <= 0) {
                    $(id_btnSendRaccomandata).attr('disabled', 'disabled');
                    $(id_btnExport).attr('disabled', 'disabled');
                    $(id_cmdResend).attr('disabled', 'disabled');
                }
            }

            function onRowSelected(sender, args) {
                var grid = sender;
                var MasterTable = grid.get_masterTableView();
                var selectedRows = MasterTable.get_selectedItems();
                if (selectedRows.length > 0) {
                    $(id_btnSendRaccomandata).removeAttr('disabled');                    
                    $(id_btnExport).removeAttr('disabled'); 
                    $(id_cmdResend).removeAttr('disabled');           
                }
            }

            $(document).ready(function() {
                $(id_btnSendRaccomandata).attr('disabled', 'disabled');
                $(id_btnExport).attr('disabled', 'disabled');
                $(id_cmdResend).attr('disabled', 'disabled');
            });

            function onRowDeselected(sender, args) {
                var grid = sender;
                var MasterTable = grid.get_masterTableView();
                var selectedRows = MasterTable.get_selectedItems();
                if (selectedRows.length <= 0) {
                    $(id_btnSendRaccomandata).attr('disabled', 'disabled');
                    $(id_btnExport).attr('disabled', 'disabled');
                }
            }

            function onRowSelected(sender, args) {
                var grid = sender;
                var MasterTable = grid.get_masterTableView();
                var selectedRows = MasterTable.get_selectedItems();
                if (selectedRows.length > 0) {
                    $(id_btnSendRaccomandata).removeAttr('disabled');                    
                    $(id_btnExport).removeAttr('disabled');
                }
            }

            function ShowLoadingPanelNew() { 
                var radgrid = $find("<%= dgMail.ClientID%>");
                var rows = radgrid.get_masterTableView().get_selectedItems().length;
                if (rows == 0) {
                    alert("Seleziona almeno una PECMail.");
                    window.event.returnValue = false;
                }
                if (rows > 0) {
                    window.event.returnValue = true; 
                }
            }

            function onTaskCompleted(sender, args) {
                var splitted = args.get_argument().split("|");
                if (Boolean(splitted[0])) {
                    if (splitted[1] * 1 > 0) {
                        var isok = confirm("Si sono verificati " + splitted[1] + " errori durante la fase di esportazione.");
                        if (isok) {
                            var ajaxManager = $find("<%= AjaxManager.ClientID %>");
                            ajaxManager.ajaxRequest("E");
                        }
                    } else {
                        alert("Esportazione eseguita correttamente.");
                    }
                } else {
                    alert("Si sono verificati degli errori durante il processo d\'importazione.");
                }
            }

            function collapseHeader(){
                if ( $( "#headerSection" ).is( ":hidden" ) ) {
                    $( "#headerSection" ).slideDown( "slow" );
                    $( "#collapseButton" ).removeClass("arrow-down").addClass("arrow-up");
                    var grid = $find("<%= dgMail.ClientID %>");
                    if(grid != undefined)
                    {
                        var masterTable = $find("<%= dgMail.ClientID %>").get_masterTableView();
                        masterTable.rebind();
                    }                  
                } else {
                    $( "#headerSection" ).hide();
                    $( "#collapseButton" ).removeClass("arrow-up").addClass("arrow-down");
                    var grid = $find("<%= dgMail.ClientID %>");
                    if(grid != undefined)
                    {
                        var masterTable = $find("<%= dgMail.ClientID %>").get_masterTableView();
                        masterTable.rebind();
                    }
                }
            }

            function pageLoad(sender, eventArgs) {
                if (!eventArgs.get_isPartialLoad()) {
                    $find("<%= AjaxManager.ClientID %>").ajaxRequest("refresh");
                }
            }

            function showImageOnSelectedItemChanging(sender, eventArgs) {
                var input = sender.get_inputDomElement();

                input.style.background = "url(" + eventArgs.get_item().get_imageUrl() + ") no-repeat";
                input.style.paddingLeft = "17px";
                input.style.backgroundPositionY = "bottom";
            }

            function OnClientLoad(sender) {
                var input = sender.get_inputDomElement();

                input.style.background = "url(" + sender.get_selectedItem().get_imageUrl() + ") no-repeat";
                input.style.paddingLeft = "17px";
                input.style.backgroundPositionY = "bottom";
            }
        </script>
    </telerik:RadCodeBlock>
    <%--Header--%>
    <table class="datatable">
        <tr class="Chiaro"  id="headerSection">
            <td style="width: 50%; border-right: 1px solid;">
                <table cellspacing="0" cellpadding="2" border="0" width="100%">
                    <tr>
                        <td style="width: 150px; vertical-align: middle; font-size: 8pt; text-align: right;">
                            <b>Casella PEC:</b>
                        </td>
                        <td style="vertical-align: middle; font-size: 8pt">
                                <telerik:RadComboBox runat="server" 
                                    RenderMode="Lightweight" 
                                    ID="ddlMailbox" 
                                    DataTextField="MailBoxName" 
                                    DataValueField="Id" 
                                    AutoPostBack="true" 
                                    Width="300px"
                                    OnClientSelectedIndexChanging="showImageOnSelectedItemChanging" 
                                    OnClientLoad="OnClientLoad"/>
                        </td>
                    </tr>
                    <tr>
                        <td style="vertical-align: middle; font-size: 8pt; text-align: right;">
                            <b>Destinatario:</b>
                        </td>
                        <td style="vertical-align: middle; font-size: 8pt">
                            <asp:TextBox runat="server" ID="txtFilterRecipient" Width="300"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="vertical-align: middle; font-size: 8pt; text-align: right;">
                            <b>Oggetto:</b>
                        </td>
                        <td style="vertical-align: middle; font-size: 8pt">
                            <asp:TextBox ID="txtFinderSubject" runat="server" Width="300"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="vertical-align: middle; font-size: 8pt; text-align: right;">
                            <b>Spedito dal:</b>
                        </td>
                        <td style="vertical-align: middle; font-size: 8pt">
                            <telerik:RadDatePicker ID="dtpShowSentFrom" runat="server" />
                            <span class="miniLabel">al:</span>
                            <telerik:RadDatePicker ID="dtpShowSentTo" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td style="vertical-align: middle; font-size: 8pt; text-align: right;">
                            <b>Mittente:</b>
                        </td>
                        <td style="vertical-align: middle; font-size: 8pt">
                            <asp:RadioButtonList ID="rdbSenderFilter" runat="server" RepeatDirection="Horizontal">
                                <asp:ListItem Text="tutti" Value="-1" Selected="True"/>
                                <asp:ListItem Text="Inviato da me" Value="0"/>
                                <asp:ListItem Text="Inviato dai miei settori" Value="1"/>
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 150px; vertical-align: middle; font-size: 8pt; text-align: right;">
                            <b>Provider PEC:</b>
                        </td>
                        <td style="vertical-align: middle; font-size: 8pt">
                            <asp:CheckBox runat="server" ID="chkAnomalies" Text="Includi PEC in errore o in fase di invio" Checked="true" />
                        </td>
                    </tr>
                    <tr ID="trIncludeMultiSended" runat="server" >
                        <td style="vertical-align: middle; font-size: 8pt; text-align: right;">
                            <b>Includi invii massivi:</b>
                        </td>
                        <td style="vertical-align: middle; font-size: 8pt">
                            <asp:CheckBox ID="cbIncludeMultiSended" runat="server" Checked="false"/>
                        </td>
                    </tr>
                    <tr ID="trMessageMultiSended" runat="server" visible="false">
                        <td style="vertical-align: middle; font-size: 8pt; text-align: left;" colspan="2">
                            <b><asp:Label ID="lnlMessageMultiSended" runat="server">Attenzione: filtro impostato in automatico dalla selezione effettuata in 'Invio Massivo'</asp:Label></b>
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
                                <asp:Button ID="cmdRefreshGrid" runat="server" Width="200px" Text="Aggiorna visualizzazione" />
                                <asp:Button ID="cmdClearFilters" runat="server" Text="Azzera filtri" />
                            </div>
                        </td>
                    </tr>
                </table>
            </td>
            <td style="width: 50%">
                <asp:Panel runat="server" ID="pnlFilterRight">
                    <table id="tblFilterVisualizza" runat="server" cellspacing="0" cellpadding="2" border="0" width="100%">
                        <tr>
                            <td style="width: 150px; vertical-align: middle; font-size: 8pt; text-align: right;">
                                <b>Visualizza:</b>
                            </td>
                            <td style="vertical-align: middle; font-size: 8pt">
                                <asp:CheckBox ID="chkShowRecorded" runat="server" AutoPostBack="true" Text="(Mail già protocollate)" />
                            </td>
                        </tr>
                        <tr>
                            <td style="width: 150px; vertical-align: middle; font-size: 8pt; text-align: right;">
                                <b>Protocollate dal:</b>
                            </td>
                            <td style="vertical-align: middle; font-size: 8pt">
                                <telerik:RadDatePicker ID="dtpShowRecordedFrom" runat="server" />
                                <span class="miniLabel">al:</span>
                                <telerik:RadDatePicker ID="dtpShowRecordedTo" runat="server" />
                            </td>
                        </tr>
                    </table>
                    <hr id="hrFilterEsito" runat="server" />
                    <table id="tblFilterEsito" runat="server" cellspacing="0" cellpadding="2" border="0" width="100%">
                        <tr>
                            <td style="width: 150px; vertical-align: middle; font-size: 8pt; text-align: right;">
                                <b>Esito invio:</b>
                            </td>
                            <td style="vertical-align: middle; font-size: 8pt">
                                <asp:RadioButtonList ID="rdbFilterPECResult" runat="server" RepeatDirection="Vertical">
                                    <asp:ListItem Text="tutti gli esiti" Value="-1" Selected="True" />
                                    <asp:ListItem Text="senza esito" Value="0" Selected="False" />
                                    <asp:ListItem Text="con solo ricevuta di accettazione" Value="1" Selected="False" />
                                    <asp:ListItem Text="con errore o ricevute di ritardo nella consegna" Value="2" Selected="False" />
                                </asp:RadioButtonList>
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
        </tr>
        <tr class="Chiaro" id="collapseSection" runat="server" visible="false">
            <td colspan="2">
                <div id="collapseButton" style="float:right;margin-left:5px;margin-right:10px;" onclick="collapseHeader();" class="pec arrow-up"></div>
            </td>
        </tr>
    </table>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <div style="overflow:hidden;width:100%;height:100%;">
        <DocSuite:BindGrid AllowFilteringByColumn="False" AllowMultiRowSelection="True" AutoGenerateColumns="False" GridLines="Both" ID="dgMail" PageSize="20" runat="server" ShowGroupPanel="True">

        <MasterTableView CommandItemDisplay="None" CurrentResetPageIndexAction="SetPageIndexToFirst" DataKeyNames="Id,IsActive" ClientDataKeyNames="Id"  Dir="LTR" TableLayout="Auto">
            <ItemStyle CssClass="Scuro" />
            <AlternatingItemStyle CssClass="Chiaro" />
            <Columns>
                    <telerik:GridClientSelectColumn UniqueName="Select" HeaderStyle-CssClass="headerImage" ItemStyle-CssClass="cellImage"/>
                <%-- Visualizza sommario --%>
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/mail_blue.png" HeaderText="Visualizza sommario" UniqueName="ViewSummary">
                        <HeaderStyle HorizontalAlign="Left" CssClass="headerImage"/>
                        <ItemStyle HorizontalAlign="Left" CssClass="cellImage"/>
                    <ItemTemplate>
                        <div style="position: relative">
                            <telerik:radButton AlternateText="Visualizza sommario" CommandName="ViewSummary" Height="16px" ID="cmdViewSummary" runat="server" Width="16px" />
                        </div>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <%-- Visualizza documenti --%>
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/documents_email.png" HeaderText="Visualizza documenti" UniqueName="ViewDocs">
                        <HeaderStyle HorizontalAlign="Left" CssClass="headerImage" />
                        <ItemStyle HorizontalAlign="Left" CssClass="cellImage"/>
                    <ItemTemplate>
                        <div style="position: relative">
                            <telerik:radButton AlternateText="Visualizza documenti" CommandName="ViewDocs" Height="16px" ID="cmdViewDocs" runat="server" Width="16px" />
                        </div>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <%-- Numero allegati --%>
                <telerik:GridBoundColumn DataField="AttachmentsCount" Groupable="False" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/documents_email_attach.png" HeaderText="Numero allegati" UniqueName="AttachmentsCount">
                        <HeaderStyle HorizontalAlign="Center" Width="30" />
                        <ItemStyle HorizontalAlign="Center" Width="30"/>
                </telerik:GridBoundColumn>
                <%-- ImgPriority --%>
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderImageUrl="../Comm/Images/Mails/highimportance.gif" HeaderText="Priorità" UniqueName="cPriority">
                        <HeaderStyle HorizontalAlign="Center" CssClass="headerImage" />
                        <ItemStyle HorizontalAlign="Center" CssClass="cellImage" />
                    <ItemTemplate>
                        <asp:Image ID="imgPriority" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <%-- ImgSegnatura --%>
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/mail_blue.png" HeaderText="Interoperabilità" UniqueName="cInterop">
                        <HeaderStyle HorizontalAlign="Center" CssClass="headerImage" />
                        <ItemStyle HorizontalAlign="Center" CssClass="cellImage"/>
                    <ItemTemplate>
                        <asp:Image runat="server" ID="imgInterop" AlternateText="Interoperabilità" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <%-- Link Protocollo --%>
                <DocSuite:CompositeTemplateExportableColumn AllowFiltering="false" Groupable="false" HeaderText="Protocollo" UniqueName="cProtocol">
                        <HeaderStyle HorizontalAlign="Center" Width="130px" />
                        <ItemStyle HorizontalAlign="Center" Width="130px" />
                    <ItemTemplate>
                        <div style="position: relative">
                            <telerik:RadButton ButtonType="LinkButton" ID="cmdProtocol" runat="server" />
                        </div>
                    </ItemTemplate>
                </DocSuite:CompositeTemplateExportableColumn>
                <%-- ImgIsPEC --%>
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/document_signature.png" HeaderText="PEC" UniqueName="cIsPEC">
                        <HeaderStyle HorizontalAlign="Center"  CssClass="headerImage" />
                        <ItemStyle HorizontalAlign="Center" CssClass="cellImage"/>
                    <ItemTemplate>
                        <asp:Image runat="server" ID="imgIsPEC" AlternateText="Da PEC" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <%--Destinatari--%>
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="A" UniqueName="colRecipients" DataField="MailRecipients">
                        <HeaderStyle HorizontalAlign="Center" Width="220px" />
                        <ItemStyle HorizontalAlign="Left" Width="220px" />
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblRecipient" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <%--Destinatari CC--%>
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="CC" UniqueName="colRecipientsCc" DataField="MailRecipientsCc">
                        <HeaderStyle HorizontalAlign="Center" Width="220px" />
                        <ItemStyle HorizontalAlign="Left" Width="220px" />
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblRecipientsCc" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <%--Oggetto--%>
                <telerik:GridTemplateColumn CurrentFilterFunction="Contains" DataField="MailSubject" Groupable="False" HeaderText="Oggetto" UniqueName="MailSubject">
                        <HeaderStyle HorizontalAlign="Center" Width="300px" />
                        <ItemStyle HorizontalAlign="Left" />
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblSubject" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <%--Data invio--%>
                <telerik:GridDateTimeColumn CurrentFilterFunction="EqualTo" DataField="RegistrationDate" DataFormatString="{0:dd/MM/yyyy HH:mm:ss tt}" DataType="System.DateTime" HeaderText="Inviato il" SortExpression="RegistrationDate" UniqueName="RegistrationDate">
                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="80"  />
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="80" />
                </telerik:GridDateTimeColumn>
                <%--Data spedizione--%>
                <telerik:GridDateTimeColumn CurrentFilterFunction="EqualTo" DataField="MailDate" DataFormatString="{0:dd/MM/yyyy HH:mm:ss tt}" DataType="System.DateTime" HeaderText="Spedito il" SortExpression="MailDate" UniqueName="MailDate">
                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="80" />
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="80" />
                </telerik:GridDateTimeColumn>
                <%--Stato spedizione--%>
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="Stato sped." UniqueName="colCurrentMailStatus">
                        <HeaderStyle HorizontalAlign="Center" Width="100px" />
                        <ItemStyle HorizontalAlign="Left" />
                    <ItemTemplate>                        
                        <asp:Label ID="lblStatus" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <%-- Dimensione --%>
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="Dimensione" UniqueName="PecSize">
                        <HeaderStyle HorizontalAlign="Center" Width="80px"/>
                        <ItemStyle HorizontalAlign="Right" Width="80px"/>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblPecSize" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
        <ClientSettings>
            <Selecting AllowRowSelect="True" CellSelectionMode="None" UseClientSelectColumnOnly="True" EnableDragToSelectRows="False" />
            <ClientEvents OnRowDeselected="onRowDeselected" OnRowSelected="onRowSelected" />
                
        </ClientSettings>
        <SortingSettings SortedAscToolTip="Ordine crescente" SortedDescToolTip="Ordine descrescente" SortToolTip="Ordina" />
    </DocSuite:BindGrid>
    </div>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <div>
        <asp:Button ID="cmdNewMail" runat="server" Width="150px" Text="Nuova PEC" />
        <asp:Button ID="cmdDelete" runat="server" Text="Elimina" Width="150" PostBackUrl="~/PEC/PECDelete.aspx?Type=PEC" />
        <asp:Button ID="cmdForward" runat="server" Text="Inoltra PEC" Width="150" />
        <asp:Button ID="cmdViewLog" runat="server" Width="150px" Text="Visualizza log" OnClientClick="ShowLoadingPanelNew();" />
        <asp:Button ID="cmdResend" runat="server" Width="150px" />
         <asp:Button ID="btnOpenModalRaccomandata" runat="server"  Width="150px" Text="Invia raccomandata" OnClientClick="OpenSelectOpWindow('SelectPolAccount.aspx');return false;" />
        <asp:Button ID="btnExport" runat="server" Text="Esporta Documenti" Visible="False" Width="120px" />
    </div>
</asp:Content>

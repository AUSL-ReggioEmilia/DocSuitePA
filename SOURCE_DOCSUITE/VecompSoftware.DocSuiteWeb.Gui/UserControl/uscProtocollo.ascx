<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscProtocollo.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscProtocollo" %>
<%@ Import Namespace="VecompSoftware.DocSuiteWeb.Data" %>
<%@ Register Src="uscSettori.ascx" TagName="uscSettori" TagPrefix="uc" %>
<%@ Register Src="~/UserControl/uscContattiSel.ascx" TagName="uscContatti" TagPrefix="uc" %>
<%@ Register Src="~/UDS/UserControl/uscUDSDynamics.ascx" TagPrefix="usc" TagName="UDSDynamics" %>
<%@ Register Src="~/UserControl/uscDocumentUnitReferences.ascx" TagName="uscDocumentUnitReferences" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscMulticlassificationRest.ascx" TagName="uscMulticlassificationRest" TagPrefix="usc" %>

<telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
    <script type="text/javascript">
        //<![CDATA[

        function OpenParerDetail(uniqueIdProtocol) {
            var wnd = window.radopen("<%=ProtocolParerDetailUrl() %>?Type=Prot&UniqueId=" + uniqueIdProtocol, "parerDetailWindow");
            wnd.setSize(400, 300);
            wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close);
            wnd.set_visibleStatusbar(false);
            wnd.set_modal(true);
            wnd.add_close(OnClientClose);
            wnd.center();
            return false;
        }

        function OnClientClose(sender, eventArgs) {
            var returnValue = sender.argument;
            if (returnValue) {
                window.location.href = returnValue;
            }
            sender.remove_close(OnClientClose);
        }

        function OpenGenericWindow(url, name) {
            var wnd = window.radopen(url, name);
            wnd.setSize(<%=WindowWidth() %>, <%=WindowHeight() %>);
            wnd.set_behaviors(<%=WindowBehaviors() %>);
            wnd.set_visibleStatusbar(false);
            wnd.set_modal(true);
            wnd.set_iconUrl("images/mail.gif");
            wnd.add_close(OnClientClose);
            wnd.set_destroyOnClose(true);
            <%=WindowPosition() %>;
            return wnd;
        }
        //]]>
    </script>
</telerik:RadCodeBlock>
<telerik:RadWindowManager EnableViewState="False" ID="RadWindowManagerViewMail" Modal="true" runat="server">
    <Windows>
        <telerik:RadWindow Height="500px" ID="windowPreviewDocument" Modal="true" ReloadOnShow="false" runat="server" ShowContentDuringLoad="false" Title="Anteprima documento" VisibleStatusbar="false" Width="700px" />
    </Windows>
</telerik:RadWindowManager>
<div id="divProtocollo" style="width: 100%;">
    <%-- protocollo --%>
    <table id="tblProtocollo" class="datatable" runat="server">
        <tr>
            <th colspan="6">
                <asp:Label ID="lblTitle" runat="server" />
            </th>
        </tr>
        <tr>
            <td class="label" style="width: 15%">Anno:
            </td>
            <td style="width: 15%">
                <b>
                    <%=CurrentProtocol.Year %></b>
            </td>
            <td class="label" style="width: 15%">Numero:
            </td>
            <td style="width: 15%">
                <b>
                    <%=CurrentProtocol.Number.ToString.PadLeft(7, "0"c)%></b>
            </td>
            <td class="label" style="width: 15%">Data:
            </td>
            <td style="width: 25%; font-weight: bold;">
                <%= String.Format(DocSuiteContext.Current.ProtocolEnv.ProtRegistrationDateFormat, CurrentProtocol.RegistrationDate.ToLocalTime.DateTime)%>
            </td>
        </tr>
    </table>
    <%-- Tipologia spedizione --%>
    <table id="tblTipoDocumento" class="datatable" runat="server" summary="Tipologia spedizione">
        <tr>
            <th colspan="2">
                <span style="margin-right: 50px;">Tipologia spedizione</span>
                <asp:CheckBox ID="cbClaim" Text="Reclamo" runat="server" Visible="false" />
            </th>
        </tr>
        <tr>
            <td class="label" style="width: 15%">Tipologia spedizione:
            </td>
            <td style="width: 85%">
                <asp:Label ID="TipoDocumentoDescription" runat="server" />
            </td>
        </tr>
    </table>
    <%-- fatturazione  --%>
    <table id="tblInvoice" class="datatable" runat="server">
        <tr id="trInvoiceTitle" runat="server">
            <th colspan="2">Dati fattura e contabilità
            </th>
        </tr>
        <tr id="trInvoiceNumber" runat="server">
            <td class="label" style="width: 15%">Fattura:
            </td>
            <td style="width: 85%" id="tdInvoiceNumber" runat="server"></td>
        </tr>
        <tr id="trAccountingSectional" runat="server">
            <td class="label" style="width: 15%">Contabilità:
            </td>
            <td style="width: 85%" runat="server" id="tdAccountingSectional"></td>
        </tr>
    </table>
    <%-- stato protocollo --%>
    <table id="tblStatusProt" runat="server" class="datatable">
        <tr>
            <th colspan="2">Stato del protocollo
            </th>
        </tr>
        <tr>
            <td class="label" style="width: 15%">Stato:
            </td>
            <td style="width: 85%">
                <b>
                    <%=GetProtocolStatusDescription()%></b>
            </td>
        </tr>
    </table>
    <%-- scatoloni --%>
    <table class="datatable" id="tblScatoloni" runat="server">
        <tr>
            <th colspan="8">Dati archiviazione
            </th>
        </tr>
        <tr>
            <td style="width: 15%;" class="label">Tipologia:
            </td>
            <td style="width: 10%;">
                <%=CurrentProtocol.PackageOrigin.ToString()%>
            </td>
            <td style="width: 10%;" class="label">Scatolone:
            </td>
            <td style="width: 10%;">
                <%=String.Format("{0:00000}", CurrentProtocol.Package)%>
            </td>
            <td style="width: 10%;" class="label">Lotto:
            </td>
            <td style="width: 10%;">
                <%=String.Format("{0:000}", CurrentProtocol.PackageLot)%>
            </td>
            <td style="width: 10%;" class="label">Progressivo:
            </td>
            <td style="width: 25%;">
                <%=String.Format("{0:00000}", CurrentProtocol.PackageIncremental)%>
            </td>
        </tr>
    </table>
    <%-- Protocollo del mittente --%>
    <table id="tblSenderProt" class="datatable" runat="server">
        <tr>
            <th colspan="4">Protocollo del mittente
            </th>
        </tr>
        <tr>
            <td class="label" style="width: 15%">Protocollo:
            </td>
            <td style="width: 15%">
                <%=CurrentProtocol.DocumentProtocol %>
            </td>
            <td class="label" style="width: 15%">Data:
            </td>
            <td style="width: 55%">
                <%=String.Format("{0:dd/MM/yyyy}", CurrentProtocol.DocumentDate) %>
            </td>
        </tr>
    </table>
    <%-- oggetto --%>
    <table id="tblOggetto" class="datatable" runat="server">
        <tr>
            <th colspan="2">Oggetto
            </th>
        </tr>
        <tr>
            <td class="label" style="width: 15%">Oggetto:
            </td>
            <td style="width: 85%">
                <%=CurrentProtocol.ProtocolObject %>
            </td>
        </tr>
        <tr id="trObjectChangeReason" runat="server" visible="false">
            <td class="label" style="width: 15%">Mot. Cambio Oggetto:
            </td>
            <td style="width: 85%">
                <%=CurrentProtocol.ObjectChangeReason %>
            </td>
        </tr>
        <tr id="trNote" runat="server">
            <td class="label" style="width: 15%">Note:
            </td>
            <td style="width: 85%">
                <%=CurrentProtocol.Note %>
            </td>
        </tr>
    </table>
    <%-- Scelta Contatti --%>
    <table class="datatable" id="tblMittentiDestinatari" runat="server">
        <tr>
            <td style="width: 50%; white-space: nowrap;">
                <uc:uscContatti ButtonDeleteVisible="false" ButtonImportVisible="false" ButtonManualVisible="false" ButtonPropertiesVisible="true" ButtonSelectAdamVisible="false" ButtonSelectDomainVisible="false" ButtonSelectOChartVisible="false" ButtonSelectVisible="false" Caption="Mittenti" EnableCompression="true" EnableCC="false" ID="uscMittenti" IsRequired="false" Multiple="true" MultiSelect="true" OnContactAdded="uscMittenti_ContactAdded" OnManualContactAdded="uscMittenti_ManualContactAdded" ProtType="true" ReadOnlyProperties="true" runat="server" Type="Prot" />
            </td>
            <td style="width: 50%; white-space: nowrap;">
                <uc:uscContatti ButtonDeleteVisible="false" ButtonImportVisible="false" ButtonManualVisible="false" ButtonPropertiesVisible="true" ButtonSelectAdamVisible="false" ButtonSelectDomainVisible="false" ButtonSelectOChartVisible="false" ButtonSelectVisible="false" Caption="Destinatari" EnableCompression="true" EnableCC="false" ID="uscDestinatari" IsRequired="false" Multiple="true" MultiSelect="true" OnContactAdded="uscDestinatari_ContactAdded" OnManualContactAdded="uscDestinatari_ManualContactAdded" ProtType="true" ReadOnlyProperties="true" runat="server" Type="Prot" />
            </td>
        </tr>
    </table>
    <%-- fascicoli --%>
    <table id="tblFascicoli" class="datatable" runat="server" visible="false">
        <tr>
            <th style="width: 20%;"></th>
            <th style="width: 60%;">Fascicoli
            </th>
            <th style="width: 20%;"></th>
        </tr>
        <tr>
            <td style="width: 20%;"></td>
            <td style="width: 60%;">
                <uc:uscContatti ID="uscFascicoli" runat="server" ReadOnly="true" HeaderVisible="false"
                    IsRequired="false" MultiSelect="true" Multiple="true" Caption="Fascicoli" EnableCC="false"
                    TreeViewCaption="Fascicoli" />
            </td>
            <td style="width: 20%;"></td>
        </tr>
    </table>
    <%-- <%-- Settori con autorizzazioni rifiutate --%>
    <table class="datatable" id="tbltRefusedAuthorizations" runat="server" visible="false">
        <tr>
            <th>Settori con autorizzazione rifiutata</th>
        </tr>
        <tr>
            <td>
                <telerik:RadTreeView runat="server" ID="TreeViewRefused" />
            </td>
        </tr>
    </table>
    <%-- settori con autorizzazione --%>
    <uc:uscSettori ID="uscSettori" ReadOnly="true" Required="false" runat="server" Visible="false" />
    <%-- Settori con Autorizzazione --%>
    <uc:uscSettori ID="uscProtocolRoleUser" Checkable="False" ReadOnly="true" Required="False" runat="server" Visible="false" />
    <%-- classificazione --%>
    <table id="tblClassificazione" class="datatable" runat="server">
        <tr>
            <th colspan="2">Classificazione
            </th>
        </tr>
        <tr>
            <td class="label" style="width: 15%">Codice:<br />
                Descrizione:
            </td>
            <td style="width: 85%">
                <%= Facade.ProtocolFacade.GetCategoryCode(CurrentProtocol)%><br />
                <%= Facade.ProtocolFacade.GetCategoryDescription(CurrentProtocol)%>
            </td>
        </tr>
    </table>

    <%-- multiclassificazione --%>
    <telerik:LayoutRow ID="rowDocumentUnitMulticlassifactionRest" runat="server" Visible="true">
        <Columns>
            <telerik:CompositeLayoutColumn>
                <Content>
                    <asp:Panel runat="server">
                        <usc:uscMulticlassificationRest ID="uscMulticlassificationRest" Visible="false" runat="server" />
                    </asp:Panel>
                </Content>
            </telerik:CompositeLayoutColumn>
        </Columns>
    </telerik:LayoutRow>

    <%-- Informazioni--%>
    <table id="tblAltri" class="datatable" runat="server">
        <tr>
            <th colspan="4">Informazioni
            </th>
        </tr>
        <tr>
            <td class="label" style="width: 15%">Locazione:
            </td>
            <td style="width: 35%">
                <asp:Label runat="server" ID="LocationName"></asp:Label>
            </td>
            <td class="label" style="width: 15%">Contenitore:
            </td>
            <td style="width: 35%">
                <asp:Label runat="server" ID="ContainerName"></asp:Label>
            </td>
        </tr>
        <tr id="rowProponente" runat="server">
            <td class="label" style="width: 15%">Proponente:
            </td>
            <td style="width: 35%">
                <%=CurrentProtocol.Subject%>
            </td>
            <td style="width: 15%">&nbsp;
            </td>
            <td style="width: 35%">&nbsp;
            </td>
        </tr>
        <tr id="rowAssegnatario" runat="server">
            <td class="label" style="width: 15%">Assegnatario:
            </td>
            <td style="width: 35%">
                <%=CurrentProtocol.Subject%>
            </td>
            <td>&nbsp;
            </td>
            <td>&nbsp;
            </td>
        </tr>
        <tr>
            <td class="label" style="width: 15%">Categoria:
            </td>
            <td style="width: 35%">
                <%=CurrentProtocol.ServiceCategory%>
            </td>
            <td class="label" style="width: 15%">
                <asp:Label runat="server" ID="lblPubblication" Text="Pubb. internet:"></asp:Label>
            </td>
            <td style="width: 35%">
                <asp:Label ID="lblCheckPublication" runat="server" />
            </td>
        </tr>
        <tr>
            <td class="label" style="width: 15%">Creato da:
            </td>
            <td style="width: 35%">
                <asp:Label ID="lblProtocolRegistrationUser" runat="server"></asp:Label>
            </td>
            <td class="label" style="width: 15%">Modificato da:
            </td>
            <td style="width: 15%">
                <asp:Label ID="lblProtocolLastChangedUser" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="label" style="width: 15%">Protocolli collegati:
            </td>
            <td style="width: 15%">
                <asp:Label ID="lblProtocolLink" runat="server"></asp:Label>
            </td>
        </tr>
        <tr id="trSourceCollaboration" runat="server">
            <td class="label" style="width: 15%">Collaborazione di origine:</td>
            <td colspan="3">
                <telerik:RadButton ID="cmdSourceCollaboration" runat="server" ButtonType="LinkButton" />
            </td>
        </tr>
    </table>
    <table class="datatable" id="tblUds" runat="server">
        <tr>
            <th>
                <telerik:RadButton ID="btnViewUDS" runat="server" ButtonType="LinkButton" AutoPostBack="false" />
            </th>
        </tr>
        <tr id="trSourceUDS" runat="server">
            <td>
                <asp:Panel runat="server" ID="dynamicData">
                    <usc:UDSDynamics runat="server" ID="udsDynamicControls" ActionType="View" ViewAuthorizations="false" />
                </asp:Panel>
            </td>
        </tr>
    </table>
    <%-- classificazione --%>
    <table id="tblAssegnatario" class="datatable" runat="server">
        <tr>
            <th colspan="2">Presa in carico
            </th>
        </tr>
        <tr>
            <td class="label" style="width: 15%">Assegnatario:<br />
                Data presa in carico:
            </td>
            <td style="width: 85%">
                <%= CurrentProtocol.Subject%><br />
                <%= CurrentProtocol.HandlerDate%>
            </td>
        </tr>
    </table>
    <%-- Motivo di annullamento --%>
    <table id="tblAnnullamento" class="datatable" runat="server">
        <tr>
            <th>Estremi del provvedimento di annullamento del protocollo
            </th>
        </tr>
        <tr>
            <td>
                <asp:Image ID="imgAnnullamento" ImageUrl="~/Comm/Images/Remove32.gif" runat="server" Height="32px" Width="32px" />
                <%=CurrentProtocol.LastChangedReason%>
            </td>
        </tr>
    </table>
    <%-- Rigetto --%>
    <table id="tblReject" class="datatable" runat="server">
        <tr>
            <th>Estremi del rigetto
            </th>
        </tr>
        <tr>
            <td>
                <asp:Image ID="imgReject" runat="server" Height="16px" Width="16px" />
                <asp:Label runat="server" ID="lblReject" />
            </td>
        </tr>
    </table>
    <%-- PARER --%>
    <table id="tblParer" class="datatable" runat="server" visible="false">
        <tr>
            <th>Stato di conservazione
                <asp:ImageButton runat="server" CssClass="dsw-align-right" Style="margin-top: 1px; margin-right: 2px;" ID="parerInfo" />
            </th>
        </tr>
        <tr>
            <td>
                <div style="margin: 5px">
                    <asp:Image runat="server" ID="parerIcon" Style="vertical-align: middle" />
                    <asp:Label runat="server" ID="parerLabel" Text=""></asp:Label>
                </div>
            </td>
        </tr>
    </table>
    <%-- Fatture PA --%>
    <table id="tblInvoicePA" class="datatable" runat="server" visible="false" border="1">
        <tr>
            <th>Stato fattura elettronica</th>
        </tr>
        <tr>
            <td>
                <div style="margin: 5px">
                    <asp:Image runat="server" ID="imgInvoicePAStatus" Style="vertical-align: middle" />
                    <asp:Label runat="server" ID="lblInvoicePAStatus" Text=""></asp:Label>
                </div>
            </td>
        </tr>
    </table>
    <%-- PosteWeb --%>
    <table id="tblPosteWeb" class="datatable" runat="server" visible="false">
        <tr>
            <th>Poste</th>
        </tr>
        <tr>
            <td>
                <table>
                    <asp:Repeater ID="rptPosteRequest" runat="server">
                        <ItemTemplate>
                            <tr>
                                <td class="label" style="width: 10%">Contenuto:
                                </td>
                                <td style="width: 20%">
                                    <%# GetContenutoRequest(Container.DataItem)%>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="6"></td>
                            </tr>
                            <tr>
                                <td colspan="6" style="height: 15px">&nbsp;
                                </td>
                            </tr>
                        </ItemTemplate>
                    </asp:Repeater>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <telerik:RadGrid AutoGenerateColumns="False" ID="dgPosteRequestContact" runat="server" Width="100%">
                    <MasterTableView CommandItemDisplay="None" CurrentResetPageIndexAction="SetPageIndexToFirst" Dir="LTR" Frame="Border" TableLayout="Auto">
                        <Columns>
                            <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="Tipo" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Image ID="imgType" runat="server" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn AllowFiltering="false" DataField="SenderName" Groupable="false" HeaderStyle-Width="100px" HeaderText="Mittente" />
                            <telerik:GridBoundColumn AllowFiltering="false" Groupable="false" HeaderText="Identif. Poste" DataField="GuidPoste" />
                            <telerik:GridBoundColumn AllowFiltering="false" Groupable="false" HeaderText="Identif. Richiesta" DataField="IdRichiesta" />
                            <telerik:GridBoundColumn AllowFiltering="false" Groupable="false" HeaderText="Nr. Ordine" DataField="IdOrdine" />
                            <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="Costo" ItemStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lblCost" />
                                </ItemTemplate>
                            </telerik:GridTemplateColumn>
                            <telerik:GridBoundColumn DataField="Name" HeaderText="Nome" HeaderStyle-Width="100px" />
                            <telerik:GridBoundColumn DataField="DataSpedizione" HeaderStyle-Width="105px" HeaderText="Data Sped." HeaderTooltip="Data Spedizione" ItemStyle-Width="105px" />
                            <telerik:GridBoundColumn DataField="IdRicevuta" HeaderText="Ricevuta Nr." />
                            <telerik:GridBoundColumn DataField="RequestStatusDescrition" HeaderText="Stato richiesta" />
                            <telerik:GridBoundColumn DataField="StatusDescrition" HeaderText="Stato consegna" />
                            <telerik:GridBoundColumn DataField="ErrorMsg" HeaderText="Errore" />
                        </Columns>
                    </MasterTableView>
                </telerik:RadGrid>
            </td>
        </tr>
    </table>

    <telerik:LayoutRow ID="rowDocumentUnitReference" runat="server" Visible="false">
        <Columns>
            <telerik:CompositeLayoutColumn>
                <Content>
                    <asp:Panel runat="server">
                        <usc:uscDocumentUnitReferences Visible="true" ID="uscDocumentUnitReferences" runat="server" ShowFascicleLinks="true" ShowProtocolRelationLinks="true" 
                            ShowProtocolDocumentSeriesLinks="true" ShowArchiveRelationLinks="true" ShowProtocolMessageLinks="true" ShowPECIncoming="true" ShowPECOutgoing="true"
                            ShowActiveWorkflowActivities="true"/>                        
                    </asp:Panel>
                </Content>
            </telerik:CompositeLayoutColumn>
        </Columns>
    </telerik:LayoutRow>    
</div>

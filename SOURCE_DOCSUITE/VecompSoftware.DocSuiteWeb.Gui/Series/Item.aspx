<%@ Page Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="Item.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.Series.Item" %>

<%@ Register Src="~/UserControl/uscDocumentUpload.ascx" TagPrefix="usc" TagName="UploadDocument" %>
<%@ Register TagPrefix="usc" TagName="Classificatore" Src="~/UserControl/uscClassificatore.ascx" %>
<%@ Register TagPrefix="usc" TagName="Settori" Src="~/UserControl/uscSettori.ascx" %>
<%@ Register Src="~/UserControl/uscAmmTraspMonitorLog.ascx" TagName="uscAmmTraspMonitorLog" TagPrefix="ucs" %>
<%@ Register Src="~/UserControl/uscMulticlassificationRest.ascx" TagName="uscMulticlassificationRest" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscDocumentUnitReferences.ascx" TagName="uscDocumentUnitReferences" TagPrefix="usc" %>


<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server" EnableViewState="false">
        <script type="text/javascript">
            var item;
            require(["Series/Item"], function (Item) {
                $(function () {
                    item = new Item(tenantModelConfiguration.serviceConfiguration);
                    item.btnNuovoMonitoraggioId = "<%= btnNuovoMonitoraggio.ClientID %>";
                    item.uscAmmTraspMonitorLogId = "<%= uscAmmTraspMonitorLog.PageContent.ClientID %>";

                    if (document.getElementById("<%= uscAmmTraspMonitorLog.DocumentUnitId.ClientID %>") != null) {
                        document.getElementById("<%= uscAmmTraspMonitorLog.DocumentUnitId.ClientID %>").value = "<%= If(CurrentDocumentSeriesItem Is Nothing, Guid.Empty, CurrentDocumentSeriesItem.UniqueId) %>";
                    }
                    if (document.getElementById("<%= uscAmmTraspMonitorLog.DocumentUnitName.ClientID %>") != null) {
                        document.getElementById("<%= uscAmmTraspMonitorLog.DocumentUnitName.ClientID %>").value ="<%= If(CurrentDocumentSeriesItem Is Nothing OrElse CurrentDocumentSeriesItem.DocumentSeries Is Nothing, String.Empty, CurrentDocumentSeriesItem.DocumentSeries.Name) %>";
                    }

                    item.initialize();
                });
            });

            function showWindow() {
                item.showWindow();
            }

            function getAjaxManager() {
                return $find("<%= RadAjaxManager.GetCurrent(Page).ClientID%>");
            }

            // Metodo per il lancio di una Request
            function AjaxRequest(request) {
                var manager = getAjaxManager();
                manager.ajaxRequest(request);
                return false;
            }

            function openCancelWindow() {
                var oWnd = $find("<%=CancelMotivation.ClientID%>");
                oWnd.show();
                return false;
            }


            function openSendToRoles(url) {
                OpenGenericWindow(url);
                return false;
            }

            function openYearSelectWindow() {
                Sys.Application.remove_load(openYearSelectWindow);
                var oWndYear = $find("<%=RadWindowsSelectYear.ClientID%>");
                oWndYear.show();
                return false;
            }


            function clientCloseYearSelectWindow(arg) {
                Sys.Application.remove_load(clientCloseYearSelectWindow);
                var oWndYear = $find("<%=RadWindowsSelectYear.ClientID%>");
                oWndYear.close(arg);
            }

            function OpenGenericWindow(url) {
                var wnd = window.radopen(url, null);
                wnd.setSize(<%= VecompSoftware.DocSuiteWeb.Data.DocSuiteContext.Current.ProtocolEnv.ModalWidth%>, <%=VecompSoftware.DocSuiteWeb.Data.DocSuiteContext.Current.ProtocolEnv.ModalWidth%>);
                wnd.set_behaviors("Telerik.Web.UI.WindowBehaviors.Close");
                wnd.set_visibleStatusbar(false);
                wnd.set_modal(true);
                wnd.DestroyOnClose = true;
                wnd.set_destroyOnClose(true);
                wnd.center();
                return wnd;
            }

            function onFlushMainDocument() {
                return confirm("Sei sicuro di voler eliminare i documenti?");
            }

            function onFlushAnnexedDocument() {
                return confirm("Sei sicuro di voler eliminare gli annessi?");
            }

            function onFlushUnpublishedAnnexedDocument() {
                return confirm("Sei sicuro di voler eliminare gli annessi da non pubblicare?");
            }

            function RemoveDraftLink(idSeriesItem) {
                AjaxRequest("removeDraftLink|" + idSeriesItem);
                return false;
            }

            function disableButton() {
                document.getElementById("<%= btnYearConfirm.ClientID %>").disabled = true;
            }

        </script>
        <style type="text/css" media="all">
            .BreakColumn {
                word-break: break-all;
                padding-right: 2px;
            }
        </style>
    </telerik:RadCodeBlock>

    <telerik:RadWindow runat="server" ID="RadWindowsSelectYear" Title="Anno di registrazione" Width="300" Height="200" Behaviors="Close">
        <ContentTemplate>
            <asp:UpdatePanel ID="UpdatePanelSelectYear" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="titolo">
                        <p>Seleziona anno di registrazione</p>
                    </div>
                    <div>
                        <br />

                        <telerik:RadDropDownList runat="server" ID="dropYears" Width="200px" AutoPostBack="False" />
                        <br />
                        <br />
                        <asp:CheckBox ID="chkSaveSession" runat="server" Text="Salva in sessione" />
                        <br />
                        <br />
                        <asp:Button runat="server" ID="btnYearConfirm" Text="Conferma" Width="100" OnClientClick="disableButton();" />
                        <br />
                        <br />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>

    <telerik:RadWindow runat="server" ID="CancelMotivation" Title="Motivazione annullamento" Width="480" Height="160" Behaviors="Close">
        <ContentTemplate>
            <asp:UpdatePanel ID="messagePanel" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="warningAreaLow">
                        <p>Motivazione per l'annullamento della registrazione.</p>
                    </div>
                    <div>
                        <telerik:RadTextBox runat="server" ID="txtCancelMotivation" ValidationGroup="Cancel" TextMode="MultiLine" Rows="3" Width="100%"></telerik:RadTextBox>
                    </div>
                    <div style="text-align: right">
                        <asp:RequiredFieldValidator ID="txtCancelRequired" runat="server" ValidationGroup="Cancel" ControlToValidate="txtCancelMotivation" ErrorMessage="Campo Motivazione Obbligatorio" Display="Dynamic" />
                        <asp:Button runat="server" ID="cmdCancelOk" Text="Conferma" Style="margin-top: 2px;" ValidationGroup="Cancel" />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </ContentTemplate>
    </telerik:RadWindow>

</asp:Content>

<asp:Content ContentPlaceHolderID="cphContent" runat="server">


    <table class="datatable" runat="server" id="tblSeries" visible="False">
        <tr>
            <th colspan="2">
                <asp:Label ID="DocumentSeriesSelection" runat="server" /></th>
        </tr>
        <tr>
            <td class="DocumentSeriesLabel">Tipo:</td>
            <td>
                <asp:DropDownList runat="server" CausesValidation="false" ID="ddlContainerArchive" AutoPostBack="True" Visible="True" Width="300px" />

            </td>
        </tr>
        <tr>
            <td class="DocumentSeriesLabel">
                <asp:Label ID="lblDocumentSeries" runat="server" />
            </td>
            <td>
                <asp:DropDownList runat="server" CausesValidation="false" ID="ddlDocumentSeries" AutoPostBack="True" Visible="True" Width="300px" />
                <asp:RequiredFieldValidator ID="rfvIdDocType" runat="server" ControlToValidate="ddlDocumentSeries" ErrorMessage="Campo Tipo Documento obbligatorio" Display="Dynamic" />
            </td>
        </tr>
    </table>

    <div runat="server" id="ContentWrapper" style="display: none; width: 100%">
        <table class="datatable" runat="server" id="tblSeriesView" visible="False">
            <tr>
                <th colspan="2">
                    <asp:Label ID="DocumentSeriesTitle" runat="server" /></th>
            </tr>
            <tr>
                <td class="DocumentSeriesLabel">Tipo:</td>
                <td>
                    <asp:Label runat="server" ID="lblContainerArchive" />
                </td>
            </tr>
            <tr>
                <td class="DocumentSeriesLabel">
                    <asp:Label ID="lblSeriesLabel" runat="server" Text="Serie Documentale" />:</td>
                <td>
                    <asp:Label runat="server" ID="lblSeries" />
                </td>
            </tr>
            <tr class="Chiaro" runat="server" id="trCollaboration" visible="False">
                <td class="DocumentSeriesLabel">Collaborazione di origine:</td>
                <td>
                    <asp:HyperLink runat="server" ID="collaborationLink"></asp:HyperLink>
                </td>
            </tr>
        </table>

        <table class="datatable" runat="server" id="tblIdentification" visible="False">
            <tr>
                <th colspan="6">Anno e numero</th>
            </tr>

            <tr id="rowActive">
                <td class="DocumentSeriesLabel" style="width: 15%">Anno:
                </td>
                <td style="width: 15%"><b>
                    <asp:Label runat="server" ID="lblYear"></asp:Label></b>
                </td>
                <td class="DocumentSeriesLabel" style="width: 15%">Numero:
                </td>
                <td style="width: 15%"><b>
                    <asp:Label runat="server" ID="lblNumber"></asp:Label></b>
                </td>
                <td class="DocumentSeriesLabel" style="width: 15%">Data:
                </td>
                <td style="width: 25%;"><b>
                    <asp:Label runat="server" ID="lblRegistrationDate"></asp:Label></b>
                </td>
            </tr>
            <tr id="rowDraft" class="Chiaro" visible="false">

                <td class="DocumentSeriesLabel">Bozza N:
                </td>
                <td>
                    <asp:Label runat="server" ID="lblDraftId"></asp:Label>
                </td>
            </tr>

        </table>
        <table class="datatable" runat="server" id="tblSubsection" visible="False">
            <tr>
                <th colspan="2">Sotto-sezione</th>
            </tr>
            <tr class="Chiaro">
                <td class="DocumentSeriesLabel">&nbsp;</td>
                <td>
                    <asp:DropDownList runat="server" CausesValidation="false" ID="ddlSubsection" AutoPostBack="false" Visible="True" Width="300px" DataTextField="Description" DataValueField="Id" />
                    <asp:RequiredFieldValidator ID="validatorSubsection" runat="server" ControlToValidate="ddlSubsection" ErrorMessage="Campo sotto-sezione Obbligatorio" Display="Dynamic" />
                </td>
            </tr>
        </table>
        <table class="datatable" runat="server" id="tblConstraints" visible="False">
            <tr>
                <th colspan="2">Obblighi trasparenza</th>
            </tr>
            <tr class="Chiaro">
                <td class="DocumentSeriesLabel">&nbsp;</td>
                <td>
                    <asp:DropDownList runat="server" CausesValidation="false" ID="ddlConstraints" AutoPostBack="false" Visible="True" Width="300px" DataTextField="Name" DataValueField="UniqueId" />
                </td>
            </tr>
        </table>
        <table class="datatable" runat="server" id="tblSubsectionView" visible="False">
            <tr>
                <th colspan="2">Sotto-sezione</th>
            </tr>
            <tr class="Chiaro">
                <td class="DocumentSeriesLabel">&nbsp;</td>
                <td>
                    <asp:Label runat="server" ID="lblSubsection"></asp:Label>
                </td>
            </tr>
        </table>
        <table class="datatable" runat="server" id="tblConstraintView" visible="False">
            <tr>
                <th colspan="2">Obblighi trasparenza</th>
            </tr>
            <tr class="Chiaro">
                <td class="DocumentSeriesLabel">&nbsp;</td>
                <td>
                    <asp:Label runat="server" ID="lblConstraint"></asp:Label>
                </td>
            </tr>
        </table>
        <asp:Panel runat="server" ID="DocumentsPanel">
            <usc:UploadDocument ButtonPreviewEnabled="True" ButtonScannerEnabled="False" DocumentDeletable="True" ID="uscUploadDocument" IsDocumentRequired="True" MultipleDocuments="true" HideScannerMultipleDocumentButton="true" runat="server" SignButtonEnabled="false"/>
        </asp:Panel>
        <asp:Panel runat="server" ID="AnnexedPanel" Visible="False">
            <usc:UploadDocument ButtonPreviewEnabled="True" ButtonScannerEnabled="False" DocumentDeletable="True" ID="uscUploadAnnexed" IsDocumentRequired="False" MultipleDocuments="True" HideScannerMultipleDocumentButton="true" runat="server" SignButtonEnabled="false" />
        </asp:Panel>
        <asp:Panel runat="server" ID="UnpublishedAnnexedPanel" Visible="False">
            <usc:UploadDocument ButtonPreviewEnabled="True" ButtonScannerEnabled="False" DocumentDeletable="True" ID="uscUnpublishedAnnexed" IsDocumentRequired="False" MultipleDocuments="True" HideScannerMultipleDocumentButton="true" runat="server" SignButtonEnabled="false" />
        </asp:Panel>

        <asp:Panel ID="pnlRoles" runat="server" Visible="True">
            <usc:Settori Caption="Settori di appartenenza" Environment="DocumentSeries" ID="uscRoleOwner" LoadUsers="False" MultiSelect="true" Required="False" RequiredMessage="Campo Obbligatorio" MultipleRoles="true" runat="server" RoleRestictions="None" />
            <usc:Settori Caption="Autorizzazioni di conoscenza" Environment="DocumentSeries" ID="uscRoleAuthorization" LoadUsers="False" MultiSelect="true" Required="False" RequiredMessage="Campo Obbligatorio" MultipleRoles="true" runat="server" />
        </asp:Panel>

        <table class="datatable" runat="server" id="ItemObjectTable" visible="False">
            <tr>
                <th>Oggetto/Descrizione
                </th>
            </tr>
            <tr>
                <td>
                    <telerik:RadTextBox runat="server" ID="ItemSubject" MaxLength="500" Width="100%" InputType="Text" TextMode="MultiLine" Rows="4"></telerik:RadTextBox>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="ItemSubject" ErrorMessage="Campo Oggetto Obbligatorio"></asp:RequiredFieldValidator>
                </td>
            </tr>
        </table>
        <table class="datatable" runat="server" id="ItemObjectTableView" visible="False">
            <tr>
                <th colspan="2">Oggetto
                </th>
            </tr>
            <tr>
                <td class="DocumentSeriesLabel"><b>Oggetto:</b></td>
                <td>
                    <asp:Label runat="server" ID="ItemSubjectLabel"></asp:Label>
                </td>
            </tr>
        </table>
        <%--Pubblicazione--%>
        <table class="datatable" runat="server" id="tblPubblication" visible="False">
            <tr>
                <th colspan="2"><b>Informazioni</b></th>
            </tr>
            <tr class="Chiaro">
                <td class="DocumentSeriesLabel">Data ultima modifica:</td>
                <td>
                    <asp:Label runat="server" ID="ItemDateEditLabelPubblicationArea"></asp:Label>
                </td>
            </tr>

            <tr class="Chiaro">
                <td class="DocumentSeriesLabel"><b>Data pubblicazione:</b></td>
                <td>
                    <telerik:RadDatePicker ID="ItemPublishingDate" runat="server" Visible="False" AutoPostBack="true" />
                    <telerik:RadLabel ID="lblItemPublishingDate" runat="server" Visible="False" Text="Seleziona una data maggiore o uguale alla data corrente" style="color: red;" />
                    <asp:Label ID="ItemPublishingDateLabel" runat="server" Visible="False" />
                </td>
            </tr>
            <tr class="Chiaro">
                <td class="DocumentSeriesLabel"><b>Data ritiro:</b></td>
                <td>
                    <telerik:RadDatePicker ID="ItemRetireDate" runat="server" Visible="False" />
                    <asp:Label ID="ItemRetireDateLabel" runat="server" Visible="False" />
                    <asp:CompareValidator runat="server" ControlToCompare="ItemPublishingDate" ControlToValidate="ItemRetireDate" Operator="GreaterThanEqual" ErrorMessage="La Data ritiro deve essere maggiore o uguale alla Data pubblicazione" />
                </td>
            </tr>
            <tr class="Chiaro">
                <td class="DocumentSeriesLabel">Pubblica in primo piano:</td>
                <td>
                    <asp:CheckBox Enabled="False" ID="chkPriority" runat="server" />
                </td>
            </tr>
        </table>
        <%--Info--%>
        <table class="datatable" runat="server" id="tblLastEditDate">
            <tr>
                <th colspan="2">Informazioni</th>
            </tr>
            <tr class="Chiaro">
                <td class="DocumentSeriesLabel">Data ultima modifica:</td>
                <td>
                    <asp:Label runat="server" ID="ItemDateEditLabel"></asp:Label>
                </td>
            </tr>
        </table>
        <asp:PlaceHolder runat="server" ID="DynamicControls"></asp:PlaceHolder>
        <asp:Panel runat="server" ID="CategoryPanel" Visible="False">
            <usc:Classificatore runat="server" ID="ItemCategory" Type="Series" Caption="Classificazione" Multiple="false" />
            <usc:Classificatore runat="server" ID="ItemSubCategory" Type="Series" Caption="Sotto Classificazione" Multiple="false" SubCategoryMode="True" Required="False" />
        </asp:Panel>
        <table class="datatable" runat="server" id="tblCategoryView" visible="False">
            <tr>
                <th colspan="2">Classificazione</th>
            </tr>
            <tr class="Chiaro">
                <td class="DocumentSeriesLabel">Codice:</td>
                <td>
                    <asp:Label runat="server" ID="ItemCategoryCodeLabel" />
                </td>
            </tr>
            <tr class="Chiaro">
                <td class="DocumentSeriesLabel">Descrizione:</td>
                <td>
                    <asp:Label runat="server" ID="ItemCategoryDescriptionLabel" />
                </td>
            </tr>
        </table>
        <asp:Panel runat="server" ID="MulticlassificationPanel">
            <usc:uscMulticlassificationRest runat="server" Visible="false" ID="uscMulticlassificationRest" />
        </asp:Panel>
        <table class="datatable" runat="server" id="pnlSeriesLink" visible="False">
            <tr>
                <th colspan="2">
                    <asp:Label ID="lblSeriesLinkTitle" runat="server" Text="Serie collegate"></asp:Label></th>
            </tr>
            <tr>
                <td class="DocumentSeriesLabel">Codice:</td>
                <td>
                    <asp:HyperLink runat="server" ID="documentSeriesLink" CausesValidation="False"></asp:HyperLink>
                </td>
            </tr>
        </table>
        <table class="datatable" runat="server" id="pnlCancel" visible="False">
            <tr>
                <th colspan="2">Estremi di annullamento</th>
            </tr>
            <tr class="Chiaro">
                <td class="DocumentSeriesLabel">Motivazione:</td>
                <td>
                    <asp:Image runat="server" ID="ItemCancelIcon" Style="vertical-align: middle" ImageUrl="../Comm/images/parer/red.png" />
                    <asp:Label runat="server" ID="ItemCancelLabel" />
                </td>
            </tr>
        </table>


        <asp:Panel runat="server" ID="pnlEditAmmTrasp" Visible="False">
            <table class="datatable">
                <tr>
                    <th>Elementi Collegati
                    </th>
                </tr>
                <tr class="Chiaro">
                    <td style="vertical-align: middle;">
                        <telerik:RadGrid runat="server" ID="dgvLinkedDocumentUnit" Skin="Office2010Blue" Width="100%" AllowMultiRowSelection="true">
                            <MasterTableView autogeneratecolumns="False" showheader="False">
                                <NoRecordsTemplate>
                                    <div>Nessun elemento collegato</div>
                                </NoRecordsTemplate>
                                <Columns>
                                    <telerik:GridTemplateColumn AllowFiltering="false">
                                        <HeaderStyle Width="75%"></HeaderStyle>
                                        <ItemTemplate>
                                            <asp:HyperLink runat="server" ID="documentUnitLink" NavigateUrl="javascript:void(0)" CausesValidation="False"></asp:HyperLink>
                                            <asp:ImageButton runat="server" ID="btnRemoveLink" Style="margin-left: 10px; cursor: pointer;" CausesValidation="False" ImageUrl="~/App_Themes/DocSuite2008/imgset16/delete.png" ToolTip="Rimuovi Bozza selezionata" />
                                        </ItemTemplate>
                                    </telerik:GridTemplateColumn>
                                </Columns>
                            </MasterTableView>
                        </telerik:RadGrid>
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </div>
    <asp:Panel runat="server" ID="pnlTransparentMonitoringLog">
        <ucs:uscAmmTraspMonitorLog runat="server" ID="uscAmmTraspMonitorLog" />
    </asp:Panel>
    <asp:ValidationSummary runat="server" ID="ValidatorSummary" DisplayMode="BulletList" ShowSummary="True" ShowMessageBox="True" />

    <asp:Panel runat="server" ID="pnlDocumentUnitReference">
        <usc:uscDocumentUnitReferences Visible="true" ID="uscDocumentUnitReferences" runat="server" ShowDocumentSeriesProtocolsLinks="true" ShowDocumentSeriesMessageLinks="true" ShowDocumentSeriesResolutionsLinks="true"/>
    </asp:Panel>
</asp:Content>

<asp:Content ContentPlaceHolderID="cphFooter" runat="server">
    <asp:Panel runat="server" ID="ButtonsPanel">
        <asp:PlaceHolder runat="server" ID="BehavioursPlaceHolder"></asp:PlaceHolder>
        <div>
            <asp:Button runat="server" ID="cmdViewDocuments" Text="Documenti" Visible="False" CausesValidation="False" Width="150" />
            <asp:Button runat="server" ID="cmdOk" Text="Conferma inserimento" CausesValidation="True" Visible="False" Enabled="False" Width="150" CommandArgument="SAVE" />
            <asp:Button runat="server" ID="cmdAssignNumber" Text="Conferma inserimento" CausesValidation="False" Width="150" Visible="False" CommandArgument="ASSIGN"></asp:Button>
            <asp:Button runat="server" ID="cmdOkEdit" Text="Conferma modifica" Visible="False" Width="150" CommandArgument="UPDATE" />
            <asp:Button runat="server" ID="cmdSaveDraft" Text="Conferma bozza" Visible="False" Enabled="False" Width="150" CommandArgument="DRAFT" />
            <asp:Button runat="server" ID="cmdEdit" Text="Modifica" Visible="False" CausesValidation="False" Width="150" />
            <asp:Button runat="server" ID="cmdCancel" Text="Annullamento" Visible="False" CausesValidation="False" Width="150" OnClientClick="return openCancelWindow();" />
            <asp:Button runat="server" ID="cmdDuplicate" Text="Duplica" Visible="False" CausesValidation="False" Width="150" PostBackUrl="Duplicate.aspx?Type=Series" />
            <asp:Button runat="server" ID="btnLog" Text="Log" CausesValidation="False" Width="150" Visible="False" />
            <asp:Button runat="server" ID="cmdFlushOwner" Text="Ripristina Settori" CausesValidation="False" Width="150" Visible="False" />
            <asp:Button runat="server" ID="btnNuovoMonitoraggio" Text="Monitoraggio" CausesValidation="False" Width="150" Visible="False" />

            <asp:Button runat="server" ID="cmdPublish" Text="Pubblica" Visible="False" CausesValidation="False" Width="150" />
            <asp:Button runat="server" ID="cmdRetire" Text="Ritira" Visible="False" CausesValidation="False" Width="150" />
            <asp:Button runat="server" ID="cmdFlushDocs" Text="Svuota documenti" OnClientClick="if(!onFlushMainDocument()) return false;" CausesValidation="False" Width="150" Visible="False" />
            <asp:Button runat="server" ID="cmdFlushAnnexed" Text="Svuota annessi" OnClientClick="if(!onFlushAnnexedDocument()) return false;" CausesValidation="False" Width="150" Visible="False" />
            <asp:Button CausesValidation="False" ID="cmdFlushUnpublishedAnnexed" OnClientClick="if(!onFlushUnpublishedAnnexedDocument()) return false;" runat="server" Text="Svuota annessi N.P." ToolTip="Svuota annessi non pubblicati" Visible="False" Width="150" />
            <asp:Button runat="server" ID="cmdMailSettori" Text="Invia a Settori" Visible="False" Width="150" />

            <asp:Button runat="server" ID="cmdAVCPEditor" Text="Modifica AVCP" Visible="False" Width="150" />
            <!-- click automatico server side-->
            <asp:Button runat="server" ID="cmdAVCPAutocomplete" Text="Completamento Automatico" Visible="False" Width="180" />
        </div>
    </asp:Panel>
</asp:Content>

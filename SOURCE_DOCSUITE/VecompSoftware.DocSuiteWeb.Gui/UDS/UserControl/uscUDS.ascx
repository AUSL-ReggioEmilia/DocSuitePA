<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscUDS.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscUDS" %>
<%@ Register Src="uscUDSDynamics.ascx" TagPrefix="usc" TagName="UDSDynamics" %>
<%@ Register Src="uscUDSStaticDataInsert.ascx" TagPrefix="usc" TagName="UDSDataInsert" %>
<%@ Register Src="uscUDSStaticDataFinder.ascx" TagPrefix="usc" TagName="UDSDataFinder" %>
<%@ Register Src="~/UserControl/uscDocumentUnitReferences.ascx" TagName="uscDocumentUnitReferences" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscMulticlassificationRest.ascx" TagName="uscMulticlassificationRest" TagPrefix="usc" %>

<telerik:RadScriptBlock runat="server">
    <script type="text/javascript">
        var UscUDSScripts = (function () {
            var currentLoadingPanel;
            var currentUpdatedControl;
            function UscUDSScripts() {
                currentLoadingPanel = null;
                currentUpdatedControl = null;
            }

            UscUDSScripts.prototype.showLoadingPanel = function () {
                if (currentLoadingPanel != null) {
                    return false;
                }
                currentLoadingPanel = $find("<%= BasePage.MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>");
                currentUpdatedControl = "<%= uscContent.ClientID%>";
                currentLoadingPanel.show(currentUpdatedControl);
            };

            UscUDSScripts.prototype.hideLoadingPanel = function () {
                if (currentLoadingPanel != null) {
                    currentLoadingPanel.hide(currentUpdatedControl);
                }
                currentUpdatedControl = null;
                currentLoadingPanel = null;
            };

            return UscUDSScripts;
        })();

        var currentUdsScript = new UscUDSScripts();

        function SetMetadataSessionStorage(metadatas, udsModel) {
            sessionStorage.setItem('DocumentMetadatas', metadatas);
            sessionStorage.setItem('UDSModel', udsModel);
        }

        function SetCurrentMetadataSessionStorage(metadatas) {
            var metadataModel = metadatas.$values;
            var dictMetadatas = [];
            var metadata;
            for (i = 0; i < metadataModel.length; i++) {
                metadata = metadataModel[i];
                dictMetadatas.push({ KeyName: metadata.Label, Value: metadata.Value != undefined ? metadata.Value : null });
            }
            sessionStorage.setItem('CurrentUDSMetadata', JSON.stringify(dictMetadatas));
        }
    </script>
</telerik:RadScriptBlock>

<asp:Panel runat="server" ID="uscContent" Height="100%">
    <telerik:RadPageLayout runat="server" GridType="Fluid" HtmlTag="None">
        <rows>
            <%--Selezione UDS--%>
            <telerik:LayoutRow CssClass="col-dsw-10" RowType="Container" HtmlTag="None">
                <Columns>
                    <telerik:LayoutColumn HtmlTag="None">
                        <asp:Panel runat="server" ID="rowUDSArchive">
                            <table class="datatable">
                                <tr id="rowTypology" runat="server">
                                    <td class="col-dsw-2 label">
                                        <label>Tipologia:</label>
                                    </td>
                                    <td class="col-dsw-8">
                                        <telerik:RadComboBox ID="ddlTypology" AllowCustomText="false" Width="300px" Height="200px" CausesValidation="false" AutoPostBack="true" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="col-dsw-2 label">
                                        <label>Archivio:</label>
                                    </td>
                                    <td class="col-dsw-8">
                                        <telerik:RadComboBox ID="ddlUds" AllowCustomText="false" Width="300px" Height="200px" CausesValidation="false" AutoPostBack="true" runat="server" />
                                    </td>
                                </tr>
                            </table>
                        </asp:Panel>
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>

            <%--Info UDS--%>
            <telerik:LayoutRow CssClass="col-dsw-10" RowType="Container" HtmlTag="None">
                <Columns>
                    <telerik:LayoutColumn HtmlTag="None">
                        <asp:Panel runat="server" ID="rowUDSInfo">

                            <%-- Registrazione del Documento --%>
                            <table class="datatable" id="tblUDSinformation" runat="server">
                                <tr>
                                    <th colspan="3">Registrazione del Documento</th>
                                </tr>
                                <tr>
                                    <td class="col-dsw-4">
                                        <asp:Label ID="lblYear" CssClass="col-dsw-9 label" Style="margin-left: 15%" Text="Anno:" runat="server"></asp:Label>
                                    </td>
                                    <td class="col-dsw-3">
                                        <asp:Label ID="lblNumer" CssClass="col-dsw-10 label" Text="Numero:" runat="server"></asp:Label>
                                    </td>
                                    <td class="col-dsw-3">
                                        <asp:Label ID="lblRegistrationDate" CssClass="col-dsw-10 label" Text="Data registrazione:" runat="server"></asp:Label>
                                    </td>
                                </tr>
                            </table>

                            <%-- Oggetto --%>
                            <table class="datatable" id="tblSubject" runat="server">
                                <tr>
                                    <th colspan="2">Oggetto
                                    </th>
                                </tr>
                                <tr>
                                    <td class="label" style="width: 15%">Oggetto:
                                    </td>
                                    <td style="width: 85%">
                                        <asp:Label ID="lblSubject" Text="Esempio di oggetto" runat="server"></asp:Label>
                                    </td>
                                </tr>
                                <tr>
                                    <td class="label" style="width: 15%">Note:
                                    </td>
                                    <td style="width: 85%">
                                        <asp:Label ID="lblNote" runat="server"></asp:Label>
                                    </td>
                                </tr>
                            </table>

                            <%-- Contatti --%>
                            <asp:Panel runat="server" ID="rowUDSContacts" Visible="false">
                                <usc:UDSDynamics runat="server" ID="udsDynamicContactsControl" />
                            </asp:Panel>

                            <%-- Autorizzazioni --%>
                            <asp:Panel runat="server" ID="rowUDSAuthorizations" Visible="false">
                                <usc:UDSDynamics runat="server" ID="udsDynamicAuthorizationsControl" />
                            </asp:Panel>

                            <%-- Classificazione --%>
                            <table id="tblClassificazione" class="datatable" runat="server" visible="false">
                                <tr>
                                    <th colspan="4">Classificazione
                                    </th>
                                </tr>
                                <tr>
                                    <td class="label" style="width: 15%">Codice:
                                    </td>
                                    <td style="width: 85%">
                                        <asp:Label ID="lblCode" Text="Esempio di codice" runat="server" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="label" style="width: 15%">Descrizione:
                                    </td>
                                    <td style="width: 85%">
                                        <asp:Label ID="lblDescription" Text="Esempio di descrizione" runat="server" />
                                    </td>
                                </tr>
                            </table>

                            <%-- Multiclassificazione --%>
                             <asp:Panel runat="server" ID="rowMulticlassificationRest">
                                <usc:uscMulticlassificationRest  runat="server" ID="uscMulticlassificationRest" Visible="false"/>
                            </asp:Panel>

                            <%-- Informazioni --%>
                            <table class="datatable">
                                <tr>
                                    <th colspan="4">Informazioni</th>
                                </tr>
                                <tr>
                                    <td class="label" style="width: 15%">
                                        <label class="label">Archivio:</label>
                                    </td>
                                    <td style="width: 35%">
                                        <asp:Label ID="lblDddlUds" CssClass="readValue" runat="server" />

                                    </td>
                                    <td class="label" style="width: 15%">
                                        <label class="label">Contenitore:</label>
                                    </td>
                                    <td style="width: 35%">
                                        <asp:Label ID="lblContainer" CssClass="readValue" runat="server" />

                                    </td>
                                </tr>
                                <tr>
                                    <td class="label" style="width: 15%">
                                        <label class="label">Creato da:</label>
                                    </td>
                                    <td style="width: 35%">
                                        <asp:Label ID="lblRegistrationUser" CssClass="readValue" runat="server" />
                                    </td>
                                    <td class="label" style="width: 15%">
                                        <label class="label">Modificato da:</label>
                                    </td>
                                    <td style="width: 35%">
                                        <asp:Label ID="lblLastChangedUser" CssClass="readValue" runat="server" />
                                    </td>
                                </tr>
                                <tr id="trConservationStatus" runat="server">
                                    <td class="label" style="width: 15%">Stato conservazione:</td>
                                    <td style="width: 35%">
                                        <div style="margin: 5px">
                                            <asp:Image ID="imgConservationIcon" runat="server" Style="vertical-align: middle" />
                                            <asp:HyperLink ID="lblConservationStatus" runat="server" Target="_blank"></asp:HyperLink>
                                        </div>
                                    </td>
                                    <td class="label" style="width: 15%">
                                        <asp:Label ID="conservationUriLabel" Visible="false" runat="server">URI:</asp:Label>
                                    </td>
                                    <td style="width: 35%">
                                        <asp:Label ID="lblConservationUri" Visible="false" runat="server"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                            
                        </asp:Panel>
                    </telerik:LayoutColumn>
                </Columns>
            </telerik:LayoutRow>

            <%-- Dati --%>
            <telerik:LayoutRow CssClass="col-dsw-10" RowType="Container" runat="server" HtmlTag="None">
                <Columns>
                    <telerik:CompositeLayoutColumn HtmlTag="None">
                        <Content>
                            <asp:Panel runat="server" ID="rowData">
                                <usc:UDSDataInsert runat="server" ID="udsDataInsert" />
                                <usc:UDSDataFinder runat="server" ID="udsDataFinder" />
                            </asp:Panel>
                        </Content>
                    </telerik:CompositeLayoutColumn>
                </Columns>
            </telerik:LayoutRow>

            <%--Dati dinamici--%>
            <telerik:LayoutRow CssClass="col-dsw-10" RowType="Container" runat="server" HtmlTag="None">
                <Columns>
                    <telerik:CompositeLayoutColumn HtmlTag="None">
                        <Content>
                            <asp:Panel runat="server" ID="rowDynamicData">
                                <usc:UDSDynamics runat="server" ID="udsDynamicControls" />
                            </asp:Panel>

                            <%-- Motivo di annullamento --%>
                            <table id="tblAnnullamento" class="datatable" runat="server" visible="false">
                                <tr>
                                    <th>Estremi del provvedimento di annullamento del Documento
                                    </th>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Image ID="imgAnnullamento" ImageUrl="~/Comm/Images/Remove32.gif" runat="server" Height="32px" Width="32px" />
                                        <asp:Label runat="server" ID="lblCancelMotivation"></asp:Label>
                                    </td>
                                </tr>
                            </table>
                        </Content>
                    </telerik:CompositeLayoutColumn>
                </Columns>
            </telerik:LayoutRow>

            <telerik:LayoutRow CssClass="col-dsw-10" RowType="Container" runat="server" HtmlTag="None">
                <Columns>
                    <telerik:CompositeLayoutColumn HtmlTag="None">
                        <Content>
                            <table id="tblUDSMessage" runat="server" visible="false" class="datatable">
                                <tr>
                                    <th colspan="2">
                                        <asp:Literal runat="server" ID="UDSMessageTitle" />
                                        <asp:Image ID="UDSMessageWarns" runat="server" Visible="false" />
                                        <asp:Image ID="UDSMessageErrors" runat="server" Visible="false" />
                                        <asp:ImageButton CausesValidation="False" ID="btnExpandUDSMessage" CssClass="contactCount" runat="server" />
                                    </th>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Repeater ID="rptUDSMessage" runat="server" Visible="false">
                                            <HeaderTemplate>
                                                <table class="dataform">
                                                    <tbody>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <tr>
                                                    <td class="label" style="width: 15%; vertical-align: top;">Utente:
                                                    </td>
                                                    <td style="width: 85%">
                                                        <asp:Label ID="lblUDSMessageUser" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="label" style="width: 15%; vertical-align: top;">Data:
                                                    </td>
                                                    <td style="width: 85%">
                                                        <asp:Label ID="lblUDSMessageDate" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="label" style="width: 15%; vertical-align: top;">Oggetto:
                                                    </td>
                                                    <td style="width: 85%">
                                                        <a id="lblUDSMessageSubject" runat="server"></a>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="label" style="width: 15%; vertical-align: top;">Mittente:
                                                    </td>
                                                    <td style="width: 85%">
                                                        <asp:Label ID="lblUDSMessageSender" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="label" style="width: 15%; vertical-align: top;">Destinatari:
                                                    </td>
                                                    <td style="width: 85%">
                                                        <asp:Label ID="lblUDSMessageRecipient" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class="label" style="width: 15%; vertical-align: top;">Status:
                                                    </td>
                                                    <td style="width: 85%">
                                                        <asp:Image ID="imgUDSMessageStatus" runat="server"></asp:Image>
                                                        <asp:Label ID="lblUDSMessageStatus" runat="server"></asp:Label>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                            <SeparatorTemplate>
                                                <tr>
                                                    <td colspan="2">
                                                        <hr />
                                                    </td>
                                                </tr>
                                            </SeparatorTemplate>
                                            <FooterTemplate>
                                                </tbody>
                                                </table>
                                            </FooterTemplate>
                                        </asp:Repeater>
                                    </td>
                                </tr>
                            </table>
                        </Content>
                    </telerik:CompositeLayoutColumn>
                </Columns>
            </telerik:LayoutRow>           

               <telerik:LayoutRow>
                <Columns>
                    <telerik:CompositeLayoutColumn>
                        <Content>
                            <asp:Panel runat="server">
                                <usc:uscDocumentUnitReferences 
                                    Visible="false" 
                                    ID="uscDocumentUnitReferences" 
                                    runat="server" 
                                    ShowPECOutgoing="true"
                                    ShowPECIncoming="true"
                                    ShowArchiveLinks="true" 
                                    ShowProtocolLinks="true" 
                                    ShowFascicleLinks="true" 
                                    ShowWorkflowActivities="true" />
                            </asp:Panel>
                        </Content>
                    </telerik:CompositeLayoutColumn>
                </Columns>
            </telerik:LayoutRow>                          
        </rows>
    </telerik:RadPageLayout>
</asp:Panel>

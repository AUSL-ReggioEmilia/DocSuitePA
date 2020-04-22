<%@ Page Language="vb" AutoEventWireup="false" Inherits="VecompSoftware.DocSuiteWeb.Gui.TbltContenitoriGes" CodeBehind="TbltContenitoriGes.aspx.vb" MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Gestione Contenitori" %>

<%@ Register Src="~/UserControl/uscSelLocation.ascx" TagName="SelLocation" TagPrefix="usc" %>
<%@ Register TagPrefix="usc" TagName="SelCategory" Src="~/UserControl/uscClassificatore.ascx" %>

<asp:Content runat="server" ContentPlaceHolderID="cphHeader">
    <style type="text/css">
        .reToolCell, .reLeftVerticalSide, .reRightVerticalSide {
            display: none !important;
        }
    </style>
    <telerik:RadScriptBlock runat="server" ID="RadScriptBlock1" EnableViewState="false">
        <script language="javascript" type="text/javascript">
            //restituisce un riferimento alla radwindow
            function GetRadWindow() {
                var oWindow = null;
                if (window.radWindow) oWindow = window.radWindow;
                else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
                return oWindow;
            }

            function CloseWindow(containerOperation, containerName, containerID) {
                var oContainer = new Object();
                oContainer.Operation = containerOperation;
                oContainer.Name = containerName;
                oContainer.ID = containerID;

                var oWindow = GetRadWindow();
                oWindow.close(oContainer);
            }
        </script>
    </telerik:RadScriptBlock>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <div style="width: 100%">
        <table class="dataform">
            <tr>
                <td class="label" width="20%">Nome:</td>
                <td>
                    <telerik:RadTextBox ID="txtName" runat="server" MaxLength="100" Width="80%" />
                </td>
            </tr>
            <tr>
                <td class="label" width="20%">Note:</td>
                <td>
                    <telerik:RadTextBox ID="txtNote" runat="server" MaxLength="100" Width="80%" />
                </td>
            </tr>
        </table>        
        <%--Depositi Documentali--%>
        <table class="datatable" id="pnlLocation" runat="server">
            <tr>
                <th>Deposito Documentale</th>
            </tr>
            <tr>
                <td>
                    <usc:SelLocation runat="server" ID="uscDocmLocation" Caption="Dossier e Pratiche:" Tipo="Docm" Type="Docm" />
                    <usc:SelLocation runat="server" ID="uscProtLocation" Caption="Protocollo:" Tipo="Prot" />
                    <usc:SelLocation runat="server" ID="uscProtAttachLocation" Caption="Allegati Protocollo:" Tipo="ProtAttach" />
                    <usc:SelLocation runat="server" ID="uscReslLocation" Caption="Atti:" Tipo="Resl" Type="Resl" />
                    <usc:SelLocation runat="server" ID="uscDeskLocation" Caption="Tavoli:" Tipo="Prot" />
                    <usc:SelLocation runat="server" ID="uscUDSLocation" Caption="Unità documentarie:" Tipo="Prot" />
                    <usc:SelLocation runat="server" ID="uscSeriesLocation" Caption="Serie Doc.le:" Tipo="Prot" />
                    <usc:SelLocation runat="server" ID="uscSeriesAnnexedLocation" Caption="Annessi (non parte integrante) Serie Doc.le:" Tipo="Prot" />
                    <usc:SelLocation runat="server" ID="uscSeriesUnpublishedAnnexedLocation" Caption="Annessi Non Pubblicabili Serie Doc.le:" Tipo="Prot" />
                </td>
            </tr>
        </table>
        <%-- Securizzazione documenti --%>
        <table class="datatable" id="pnlSecureDocument" runat="server">
            <tr>
                <th>Securizzazione documenti</th>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="chkSecureDocument" runat="server" Text="Abilita securizzazione documenti" />
                </td>
            </tr>
        </table>
        <%--Conservazione Sostitutiva--%>
        <table class="datatable" id="pnlConservation" runat="server">
            <tr>
                <th>Conservazione Sostitutiva</th>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="chkConservation" runat="server" Text="Abilita per conservazione sostitutiva" />
                </td>
            </tr>
        </table>
          <%--Modulo Privacy--%>
        <table class="datatable" id="pnlPrivacy" runat="server">
            <tr>
                <th>Privacy</th>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="chkIsPrivacy" runat="server" OnCheckedChanged="chkIsPrivacy_CheckedChanged" AutoPostBack="true"/>
                </td>
            </tr>

              <tr id="lblPrivacyLevel">
                <td><%= String.Concat("Livello ", PRIVACY_LABEL) %>
                    <asp:DropDownList runat="server" ID="ddlPrivacyLevel" DataTextField="Description" DataValueField="Level" AppendDataBoundItems="true" />
                </td>
            </tr>
        </table>


        <%--Serie documentali--%>
        <table id="pnlDocumentSeries" runat="server" class="datatable">
            <tr>
                <th>
                    <asp:Label runat="server" ID="lblDocumentSeries" />
                </th>
            </tr>
            <tr>
                <td>Tipo archivio
                    <asp:DropDownList runat="server" ID="ddlArchive" DataTextField="Name" DataValueField="Id" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="chkZeroDoc" runat="server" Text="Permetti registrazioni senza documento" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="chkAddDocs" runat="server" Text="Permetti aggiunta documenti" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="chkSeriesPub" runat="server" Text="Abilita pubblicazione" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="chkRoles" runat="server" Text="Abilita settori" />
                </td>
            </tr>
            <tr>
                <td>
                    <asp:CheckBox ID="chkSubsection" runat="server" Text="Abilita sotto-sezioni" />
                </td>
            </tr>
            <tr>
                <td>Famiglia
                    <asp:DropDownList runat="server" ID="ddlDocumentSeriesFamily" DataTextField="Name" DataValueField="Id" />
                </td>
            </tr>
        </table>
        <%--Classificatore di default--%>
        <usc:SelCategory runat="server" ID="uscClassificatori" Type="Prot" Caption="Classificazione predefinita" Multiple="false" Required="false" Visible="False" />
    </div>
    <table id="FrontaliniPrivacy" runat="server" style="width: 100%" visible="false">
        <tr>
            <td colspan="2" style="width: 100%">
                <%-- Frontalini e privacy --%>
                <table class="datatable" width="100%">
                    <tr>
                        <th colspan="2">
                            <asp:Label runat="server" ID="lblFrontaliniHeader" />
                        </th>
                    </tr>
                </table>
                <table class="datatable" width="100%">
                    <%-- Heading Frontalino --%>
                    <tr>
                        <td class="label" style="vertical-align: middle; width: 20%;">
                            <asp:Label runat="server" ID="lblHeadingFrontalino" />
                        </td>
                        <td style="vertical-align: middle; width: 80%;">
                            <telerik:RadAjaxPanel runat="server">
                                <telerik:RadEditor ID="txtHeadingFrontalino" EditModes="Html" EnableViewState="false" NewLineBr="true" runat="server" Width="100%" Height="100px">
                                    <Tools>
                                        <telerik:EditorToolGroup>
                                        </telerik:EditorToolGroup>
                                    </Tools>
                                </telerik:RadEditor>
                            </telerik:RadAjaxPanel>
                        </td>
                    </tr>
                </table>
                <table class="datatable" width="100%">
                    <%-- Heading Letter --%>
                    <tr>
                        <td class="label" style="vertical-align: middle; width: 20%;">
                            <asp:Label runat="server" ID="lblHeadingLetter" />
                        </td>
                        <td style="vertical-align: middle; width: 80%;">
                            <telerik:RadAjaxPanel runat="server">
                                <telerik:RadEditor ID="txtHeadingLetter" runat="server" EnableViewState="false" EditModes="Html" NewLineBr="true" Width="100%" Height="50px">
                                    <Tools>
                                        <telerik:EditorToolGroup>
                                        </telerik:EditorToolGroup>
                                    </Tools>
                                </telerik:RadEditor>
                            </telerik:RadAjaxPanel>
                        </td>
                    </tr>
                </table>
                <table class="datatable" width="100%">
                    <%-- Privacy --%>
                    <tr>
                        <td class="label" style="vertical-align: middle; width: 20%;">
                            <asp:Label runat="server" ID="lblPrivacy" />
                        </td>
                        <td style="vertical-align: middle; width: 80%;">
                            <asp:CheckBox ID="chkPrivacy" runat="server" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    <asp:Button ID="btnConferma" runat="server" Text="Conferma" />
</asp:Content>

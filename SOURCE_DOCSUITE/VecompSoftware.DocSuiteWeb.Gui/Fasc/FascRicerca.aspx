<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="FascRicerca.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.FascRicerca"
    MasterPageFile="~/MasterPages/DocSuite2008.Master" Title="Fascicoli - Ricerca" %>

<%@ Register Src="../UserControl/uscFascicleFinder.ascx" TagName="uscFascicleFinder" TagPrefix="uc1" %>

<asp:Content runat="server" ContentPlaceHolderID="cphContent">
    <telerik:RadScriptBlock runat="server" ID="rsb">
        <script type="text/javascript">     
            $(document).keypress(function (e) {
                if (e.which == 13) {
                    document.getElementById("<%= btnSearch.ClientID %>").click();
                    e.preventDefault();
                }
            });

            $(document).ready(function() {
                var btnSearch = $find("<%= btnSearch.ClientID %>");
                btnSearch.add_clicked(btnSearch_onClick);
            });

            function btnSearch_onClick() {
                var ajaxModel = {};
                ajaxModel.ActionName = "Search";
                ajaxModel.Value = [];

                var uscMetadataRepositorySel = $("#<%= uscFascicleFinder.UscMetadataRepositorySelId %>").data();
                if (!jQuery.isEmptyObject(uscMetadataRepositorySel)) {
                    var metadataResult = uscMetadataRepositorySel.getMetadataFilterValues();
                    var metadataValue = metadataResult[0],
                        metadataFinderModels = metadataResult[1],
                        metadataValuesAreValid = metadataResult[2];

                    if (!metadataValuesAreValid) {
                        alert("Alcuni valori di metadati non sono validi");
                        return;
                    }                    
                    ajaxModel.Value.push(metadataValue);
                    ajaxModel.Value.push(JSON.stringify(metadataFinderModels));
                }

                $find("<%= AjaxManager.ClientID %>").ajaxRequest(JSON.stringify(ajaxModel));
            }
        </script>
    </telerik:RadScriptBlock>
    <telerik:RadPageLayout runat="server" HtmlTag="Div" ID="pageContent" Height="100%">
        <Rows>
            <telerik:LayoutRow runat="server" HtmlTag="Div" Height="100%">
                <Content>
                    <uc1:uscFascicleFinder ID="uscFascicleFinder" runat="server" />
                </Content>
            </telerik:LayoutRow>
        </Rows>
    </telerik:RadPageLayout>
</asp:Content>

<asp:Content ID="Content1" runat="server" ContentPlaceHolderID="cphFooter">
    <telerik:RadButton ID="btnSearch" Text="Ricerca" Width="150px" runat="server" TabIndex="1" AutoPostBack="false" />
</asp:Content>

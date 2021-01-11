<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscContactSearchRest.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscContactSearchRest" %>

<telerik:RadScriptBlock runat="server">
    <script type="text/javascript">
        var <%= Me.ClientID %>_uscContactSearchRest;
        require(["UserControl/uscContactSearchRest"], function (UscContactSearchRest) {
            $(function () {
                <%= Me.ClientID %>_uscContactSearchRest = new UscContactSearchRest(tenantModelConfiguration.serviceConfiguration);
                <%= Me.ClientID %>_uscContactSearchRest.rcdsContactsFinderId = "<%= rcdsContactsFinder.ClientID %>";
                <%= Me.ClientID %>_uscContactSearchRest.rsbSearchBoxId = "<%= rsbSearchBox.ClientID %>";
                <%= Me.ClientID %>_uscContactSearchRest.pnlMainContentId = "<%= pnlMainContent.ClientID %>";
                <%= Me.ClientID %>_uscContactSearchRest.toolTipManagerId = "<%= toolTipManager.ClientID %>";
                <%= Me.ClientID %>_uscContactSearchRest.applyAuthorizations = <%= ApplyAuthorizationsJString %>;
                <%= Me.ClientID %>_uscContactSearchRest.excludeRoleContacts = <%= ExcludeRoleContactsJString %>;
                <%= Me.ClientID %>_uscContactSearchRest.parentId = <%= ParentIdJString %>;
                <%= Me.ClientID %>_uscContactSearchRest.filterByParentId = <%= If(FilterByParentId.HasValue, FilterByParentId.Value, "undefined")  %>;
                <%= Me.ClientID %>_uscContactSearchRest.parentToExclude = <%= ParentToExcludeJString %>;
                <%= Me.ClientID %>_uscContactSearchRest.tenantId = "<%= IdTenant%>";
                <%= Me.ClientID %>_uscContactSearchRest.initialize();
            });
        });

        function showTooltip(target) {
                <%= Me.ClientID %>_uscContactSearchRest.showTooltip(target);
        }
    </script>
    <style>
        .searchBoxButton:hover {
            border-color: #c4ba9c;
            color: #000;
            background-color: #ffe18a;
            background-image: linear-gradient(#fffce8,#ffedb3 50%,#ffd563 50%,#ffe18a);
        }
    </style>
</telerik:RadScriptBlock>

<asp:Panel runat="server" ID="pnlMainContent">
    <telerik:RadClientDataSource runat="server" ID="rcdsContactsFinder"></telerik:RadClientDataSource>
    <telerik:RadToolTipManager runat="server" ID="toolTipManager" Width="480px" Height="227px"
        RelativeTo="Element" Animation="Fade" Position="BottomLeft" Skin="Silk" />

    <telerik:RadSearchBox runat="server" ID="rsbSearchBox" ClientDataSourceID="rcdsContactsFinder" Width="100%"
        EnableAutoComplete="true" RenderMode="Lightweight" DataValueField="Id" DataTextField="Description" EmptyMessage="Ricerca contatti...">
        <DropDownSettings Height="300">
            <ClientTemplate>
                <div style="display: none;">
                    <div id="toolTipContent_#= Id #"> 
                        <div id="parentTree_#= Id #" style="margin-bottom: 10px;"></div>
                        <hr></hr>
                        <p><b>Descrizione:</b> #= Description #</p>
                        <p><b>Codice di ricerca:</b> 
                            # if(Code){ #
                                #= Code  #
                            # }#
                        </p>
                        <p><b>Email:</b> 
                            # if(Email){ #
                                #= Email #
                            # }#
                        </p>
                        <p><b>Posta certificata:</b>
                            # if(CertifiedMail){ #
                                #= CertifiedMail #
                            # }#
                        </p>
                    </div>                    
                </div>

                <div>                    
                    <div class="details">
                        <img src="../App_Themes/DocSuite2008/imgset16/help.png" id="item_#= Id #" style="vertical-align: middle;" onmouseenter="showTooltip(#= Id #);"></img>
                        # if(ContactType == "Administration"){ #
                            <img src="../comm/images/interop/Amministrazione.gif" style="vertical-align: middle;"></img>
                        # } else if(ContactType == "AOO") { #
                            <img src="../comm/images/interop/Aoo.gif" style="vertical-align: middle;"></img>
                        # } else if(ContactType == "AO") { #
                            <img src="../comm/images/interop/Uo.gif" style="vertical-align: middle;"></img>
                        # } else if(ContactType == "Role") { #
                            <img src="../comm/images/interop/Ruolo.gif" style="vertical-align: middle;"></img>
                        # } else if(ContactType == "Group") { #
                            <img src="../comm/images/interop/Gruppo.gif" style="vertical-align: middle;"></img>
                        # } else if(ContactType == "Sector") { #
                            <img src="../App_Themes/DocSuite2008/imgset16/GroupMembers.png" style="vertical-align: middle;"></img>
                        # } else if(ContactType == "Citizen") { #
                            <img src="../App_Themes/DocSuite2008/imgset16/user.png" style="vertical-align: middle;"></img>
                        # } #                        
                        <span style="vertical-align: middle;"><b>#= Description #</b></span>
                        # if(Code || FiscalCode){#
                             # if(Code){ #
                                <span style="vertical-align: middle;"><b>(#= Code #)</b></span>
                            # } else {#
                                <span style="vertical-align: middle;"><b>(#= FiscalCode #)</b></span>
                            # } #
                        # } #
                        # if(Email || CertifiedMail){#
                            # if(CertifiedMail){ #
                                <span style="vertical-align: middle;"> - <label style="color: gray;">#= CertifiedMail #</label></span>
                            # } else {#
                                <span style="vertical-align: middle;"> - <label style="color: gray;">#= Email #</label></span>
                            # } #
                        # } #
                    </div>
                </div>               
            </ClientTemplate>
        </DropDownSettings>
        <Buttons>
            <telerik:SearchBoxButton runat="server" ImageUrl="~/App_Themes/DocSuite2008/imgset16/down_arrow.png" CommandName="searchByParent" />
        </Buttons>
    </telerik:RadSearchBox>
</asp:Panel>

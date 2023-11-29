<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscFascGrid.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscFascGrid" %>
<div class="radGridWrapper">
    <DocSuite:BindGrid AllowMultiRowSelection="true" AutoGenerateColumns="False" Width="100%" GridLines="Both" ID="gvFascicles" runat="server" ShowGroupPanel="True" ImpersonateCurrentUser="true" ClientSettings-Scrolling-AllowScroll="true" >
        <MasterTableView TableLayout="Auto" NoMasterRecordsText="Nessun fascicolo trovato" GridLines="Both" ClientDataKeyNames="Entity.UniqueId" AllowSorting="false">
            <Columns>
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="False" UniqueName="ClientSelectColumn">
                    <HeaderStyle HorizontalAlign="Center" Width="25px" />
                    <ItemStyle HorizontalAlign="Center" Width="25px" />
                    <ItemTemplate>
                        <asp:CheckBox AutoPostBack="False" ID="cbSelect" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderImageUrl="~/App_Themes/DocSuite2008/imgset16/fascicle_open.png" HeaderTooltip="Stato" UniqueName="cFascicle" AllowFiltering="false" Groupable="false">
                    <HeaderStyle HorizontalAlign="Center" CssClass="headerImage" Width="16px" />
                    <ItemStyle HorizontalAlign="Center" CssClass="cellImage" />
                    <ItemTemplate>
                        <asp:Image runat="server" ID="imgFasc" Width="16px" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderImageUrl="~/App_Themes/DocSuite2008/imgset16/fascicle_procedure.png" HeaderTooltip="Tipo" UniqueName="cFascicleType" AllowFiltering="false" Groupable="false">
                    <HeaderStyle HorizontalAlign="Center" CssClass="headerImage" Width="16px" />
                    <ItemStyle HorizontalAlign="Center" CssClass="cellImage" />
                    <ItemTemplate>
                        <asp:Image runat="server" ID="imgFascicleType" Width="16px" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderText="Fascicolo" CurrentFilterFunction="Contains" UniqueName="Title" AllowFiltering="false" Groupable="false" SortExpression="Entity.Title">
                    <HeaderStyle Width="300px"></HeaderStyle>
                    <ItemStyle></ItemStyle>
                    <ItemTemplate>
                        <div style="position: relative">
                            <asp:LinkButton ID="lnkFascicle" CommandName="ShowFasc" runat="server" />
                        </div>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridDateTimeColumn DataField="Entity.StartDate" HeaderText="Data <br> apertura" DataFormatString="{0:dd/MM/yyyy}" CurrentFilterFunction="EqualTo" UniqueName="StartDate" SortExpression="Entity.StartDate" >
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </telerik:GridDateTimeColumn>
                <telerik:GridDateTimeColumn DataField="Entity.LastChangedDate" Visible="false" HeaderText="Data dell'ultima <br> modifica" DataFormatString="{0:dd/MM/yyyy}" CurrentFilterFunction="EqualTo" UniqueName="LastChangedDate" SortExpression="Entity.LastChangedDate">
                    <HeaderStyle HorizontalAlign="Center"  />
                    <ItemStyle HorizontalAlign="Center" />
                </telerik:GridDateTimeColumn>
                <telerik:GridBoundColumn DataField="Entity.Category.Name" HeaderText="Classificazione" CurrentFilterFunction="Contains" UniqueName="CategoryName" SortExpression="Entity.Category.Name">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Left" />
                </telerik:GridBoundColumn>
                <telerik:GridTemplateColumn DataField="Entity.Manager" HeaderText="Responsabile" CurrentFilterFunction="Contains" UniqueName="Manager" SortExpression="Entity.Manager" GroupByExpression="Entity.Manager Group By Entity.Manager">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Left" />
                    <ItemTemplate>
                        <asp:Label ID="lblManager" runat="server"></asp:Label>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn DataField="Entity.TenantAOO.Name" HeaderText="Nome della AOO" CurrentFilterFunction="Contains" UniqueName="TenantAOOName" SortExpression="Entity.TenantAOO.Name">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Left" />
                </telerik:GridBoundColumn>
                <telerik:GridTemplateColumn DataField="Entity.ProcessLabel" HeaderText="Serie e volume" UniqueName="ProcessAndDossierFolderLabels" AllowFiltering="false" Groupable="true" Resizable="true" GroupByExpression="Entity.ProcessLabel Group By Entity.ProcessLabel">
                    <HeaderStyle ></HeaderStyle>
                    <ItemStyle></ItemStyle>
                    <ItemTemplate>
                        <div style="position: relative">
                            <asp:Label ID="lblProcessAdDossierFolderLabels" runat="server" />
                        </div>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
        <ClientSettings>
            <Selecting AllowRowSelect="true" />
        </ClientSettings>
    </DocSuite:BindGrid>
</div>

<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscUDFascicleGrid.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscUDFascicleGrid" %>

<div class="radGridWrapper">
    <DocSuite:BindGrid AllowMultiRowSelection="False" AutoGenerateColumns="False" GridLines="Both" ID="grdUDFascicle" runat="server" ShowGroupPanel="True">
        <MasterTableView AllowFilteringByColumn="True" GridLines="Both" NoMasterRecordsText="Nessun risultato" TableLayout="Fixed">
            <Columns>
                <telerik:GridTemplateColumn HeaderImageUrl="~/App_Themes/DocSuite2008/imgset16/StatusSecurityCritical_16x.png" HeaderTooltip="Documenti oltre la soglia prevista dalla normativa per la fascicolazione" UniqueName="colWarningIcon" AllowFiltering="false" Groupable="false">
                    <HeaderStyle HorizontalAlign="Center" CssClass="headerImage" Width="40px" />
                    <ItemStyle HorizontalAlign="Center" CssClass="cellImage" />
                     <ItemTemplate>
                       <asp:Image ID="imgWarningIcon" runat="server" />
                    </ItemTemplate>                    
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="Entity.DocumentUnitName" CurrentFilterFunction="EqualTo" SortExpression="Entity.DocumentUnitName" DataField="Entity.DocumentUnitName" HeaderText="Documento" Groupable="false" AllowFiltering="true">
                    <HeaderStyle HorizontalAlign="Center"  Width="140px" />
                    <ItemStyle HorizontalAlign="Center" />
                    <FilterTemplate>
                        <telerik:RadComboBox  runat="server" ID="cmbDocumentUnitName" DataTextField="Text" DataValueField="Value" Width="100%" AutoPostBack="True" OnSelectedIndexChanged="cmbDocumentUnitName_SelectedIndexChanged">
                        </telerik:RadComboBox>
                    </FilterTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblUDName" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="colFullNumber" HeaderText="Numero" SortExpression="Entity.Year,Entity.Number" Groupable="false">
                    <HeaderStyle HorizontalAlign="Center" Width="110px" />
                    <ItemStyle HorizontalAlign="Center" />
                    <FilterTemplate>
                        <telerik:RadMaskedTextBox RenderMode="Lightweight" ID="fullUDNumberFilter" OnTextChanged="MaskText_TextChanged" runat="server" Width="100px" Mask="####/#######" CssClass="filterBox"></telerik:RadMaskedTextBox>
                    </FilterTemplate>
                    <ItemTemplate>
                        <asp:LinkButton runat="server" ID="lbtViewUD" CommandName="ViewUD" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridDateTimeColumn UniqueName="Entity.RegistrationDate" HeaderText="Data registrazione" DataField="Entity.RegistrationDate" DataFormatString="{0:dd/MM/yyyy}" CurrentFilterFunction="EqualTo" SortExpression="Entity.RegistrationDate" Groupable="false" >
                    <HeaderStyle HorizontalAlign="Center" Wrap="false" Width="125px" />
                    <ItemStyle HorizontalAlign="Center"/>
                </telerik:GridDateTimeColumn>
                <telerik:GridTemplateColumn UniqueName="Entity.Subject" DataField="Entity.Subject" HeaderText="Oggetto" Groupable="false" AllowFiltering="true" CurrentFilterFunction="Contains">
                    <HeaderStyle HorizontalAlign="Center"/>
                    <ItemStyle HorizontalAlign="Left"/>
                    <ItemTemplate>
                        <asp:Label ID="lblUDObject" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                 <telerik:GridTemplateColumn UniqueName="Entity.Category.Name" DataField="Entity.Category.Name" CurrentFilterFunction="Contains" Groupable="False" HeaderText="Classificatore" SortExpression="Entity.Category.Name" >
                    <HeaderStyle HorizontalAlign="Center" Wrap="true"/>
                    <ItemStyle HorizontalAlign="Left" Wrap="true"/>
                    <ItemTemplate>
                        <asp:Label ID="lblCategory" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="Entity.Container.Name" CurrentFilterFunction="Contains" DataField="Entity.Container.Name" HeaderText="Contenitore" SortExpression="Entity.Container.Name">
                    <HeaderStyle HorizontalAlign="Center" Wrap="false" />
                    <ItemStyle HorizontalAlign="Left" />
                    <ItemTemplate>
                        <asp:Label ID="lblContainer" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>         

            </Columns>
        </MasterTableView>
        <ClientSettings>
            <Selecting AllowRowSelect="true" />
        </ClientSettings>
    </DocSuite:BindGrid>
</div>

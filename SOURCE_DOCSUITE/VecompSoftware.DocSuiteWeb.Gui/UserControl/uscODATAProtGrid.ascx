<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscODATAProtGrid.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscODATAProtGrid" %>

<telerik:RadScriptBlock runat="server" ID="rsbContent" EnableViewState="false">
    <script type="text/javascript">
        function OpenParerDetail(path) {
            var wnd = window.radopen(path, "parerDetailWindow");
            wnd.setSize(480, 300);
            wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close);
            wnd.set_visibleStatusbar(false);
            wnd.set_modal(true);
            wnd.center();
            return false;
        }

       
    </script>
</telerik:RadScriptBlock>

<div class="radGridWrapper">
    <DocSuite:BindGrid AllowMultiRowSelection="False" AutoGenerateColumns="False" GridLines="Both" ID="grdProtocols" runat="server" ShowGroupPanel="True">
        <MasterTableView AllowFilteringByColumn="True" GridLines="Both" NoMasterRecordsText="Nessun Documento Trovato" TableLayout="Fixed">
            <Columns>
                <telerik:GridTemplateColumn UniqueName="TenantModel.TenantName" CurrentFilterFunction="EqualTo" SortExpression="TenantModel.TenantName" DataField="TenantModel.TenantName" HeaderText="Azienda" Groupable="false" AllowFiltering="true" Visible="false" >
                    <HeaderStyle HorizontalAlign="Center"  Width="80px" />
                    <ItemStyle HorizontalAlign="Center" />
                    <FilterTemplate>
                        <telerik:RadComboBox  runat="server" ID="cmbTenantName" DataTextField="Text" DataValueField="Value" Width="100%" AutoPostBack="True" OnSelectedIndexChanged="cmbTenantName_SelectedIndexChanged">
                        </telerik:RadComboBox>
                    </FilterTemplate>
                    <ItemTemplate>
                        <asp:Label ID="lblTenantName" runat="server" />
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
                <telerik:GridTemplateColumn UniqueName="colViewDocuments" HeaderText="Tipo Documento" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/documentPreview.png" AllowFiltering="false" Groupable="false">
                    <HeaderStyle HorizontalAlign="Center" CssClass="headerImage" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:ImageButton ID="ibtViewDocuments" runat="server" CommandName="ViewDocuments" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="colUDTitle" HeaderText="Numero" SortExpression="Entity.Year,Entity.Number" Groupable="false">
                    <HeaderStyle HorizontalAlign="Center" Width="110px" />
                    <ItemStyle HorizontalAlign="Center" />
                    <FilterTemplate>
                        <telerik:RadMaskedTextBox RenderMode="Lightweight" ID="UDTitleFilter" OnTextChanged="MaskText_TextChanged" runat="server" Width="100px" Mask="####/#######" CssClass="filterBox"></telerik:RadMaskedTextBox>
                    </FilterTemplate>
                    <ItemTemplate>
                        <asp:LinkButton runat="server" ID="lbtViewUD" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridDateTimeColumn UniqueName="RegistrationDate" HeaderText="Data registrazione" DataField="Entity.RegistrationDate" DataFormatString="{0:dd/MM/yyyy}" AllowFiltering="false" SortExpression="Entity.RegistrationDate">
                    <HeaderStyle HorizontalAlign="Center" Wrap="false" Width="125px" />
                    <ItemStyle HorizontalAlign="Center" />
                </telerik:GridDateTimeColumn>
                <telerik:GridBoundColumn CurrentFilterFunction="Contains" DataField="Entity.Container.Name" HeaderText="Contenitore" SortExpression="Entity.Container.Name" UniqueName="Entity.Container.Name">
                    <HeaderStyle HorizontalAlign="Center" Wrap="false" />
                    <ItemStyle HorizontalAlign="Left" />
                </telerik:GridBoundColumn>
                <DocSuite:CompositeTemplateColumnSqlExpression CurrentFilterFunction="Contains" Groupable="False" HeaderText="Classificatore" SortExpression="Entity.Category.Name" UniqueName="Entity.Category.Name">
                    <HeaderStyle HorizontalAlign="Center" Wrap="true"/>
                    <ItemStyle HorizontalAlign="Left" Wrap="true"/>
                    <ItemTemplate>
                        <asp:Label ID="lblCategoryProjection" runat="server" />
                    </ItemTemplate>
                </DocSuite:CompositeTemplateColumnSqlExpression>
                <telerik:GridTemplateColumn UniqueName="Entity.Subject" DataField="Entity.Subject" HeaderText="Oggetto" Groupable="false" AllowFiltering="true" CurrentFilterFunction="Contains">
                    <HeaderStyle HorizontalAlign="Center"/>
                    <ItemStyle HorizontalAlign="Left"/>
                    <ItemTemplate>
                        <asp:Label ID="lblUDSubject" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>             
            </Columns>
        </MasterTableView>
        <ClientSettings>
            <Selecting AllowRowSelect="true" />
        </ClientSettings>
    </DocSuite:BindGrid>
</div>

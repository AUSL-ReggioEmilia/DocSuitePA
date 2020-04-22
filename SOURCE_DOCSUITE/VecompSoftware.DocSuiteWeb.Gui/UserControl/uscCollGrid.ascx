<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscCollGrid.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscCollGrid" %>


<div class="radGridWrapper">
    <DocSuite:BindGrid AllowFilteringByColumn="True" AutoGenerateColumns="False" Width="100%" GridLines="None" ID="gvCollaboration" runat="server" ShowGroupPanel="True">
        <MasterTableView DataKeyNames="Entity.IdCollaboration" GridLines="Horizontal" NoMasterRecordsText="Nessun documento trovato" TableLayout="Auto">
            <Columns>
                <telerik:GridBoundColumn HeaderStyle-Width="0" UniqueName="Entity.DocumentType" DataField="Entity.DocumentType"></telerik:GridBoundColumn>
                <telerik:GridBoundColumn AllowFiltering="false" DataField="Entity.IdCollaboration" Groupable="false" HeaderStyle-Width="5%" UniqueName="Entity.IdCollaboration" />
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="False" UniqueName="ClientSelectColumn">
                    <HeaderStyle HorizontalAlign="Center" Width="25px" CssClass="headerImage" />
                    <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:CheckBox AutoPostBack="False" ID="cbSelect" runat="server" />
                        </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn CurrentFilterFunction="EqualTo" UniqueName="TenantModel.TenantName" DataField="TenantModel.TenantName" Groupable="false" HeaderText="Azienda" SortExpression="TenantModel.TenantName" AllowSorting="true">
                    <HeaderStyle HorizontalAlign="Center" Width="80px" />
                    <ItemStyle HorizontalAlign="Center" Width="80px" />
                     <FilterTemplate>
                            <telerik:RadComboBox  runat="server" ID="cmbTenants" DataTextField="Text" DataValueField="Value" Width="100%" AutoPostBack="True"
                                OnSelectedIndexChanged="cmbTenants_SelectedIndexChanged">
                            </telerik:RadComboBox>
                        </FilterTemplate>
                     <ItemTemplate>
                         <asp:Label ID="lblTenantName" runat="server" />
                     </ItemTemplate>
	            </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderImageUrl="../Comm/Images/None16.gif" UniqueName="cDocType" >
                    <HeaderStyle HorizontalAlign="Center" Width="25px" CssClass="headerImage"/>
                    <ItemStyle HorizontalAlign="Center" />
		            <ItemTemplate>
                        <telerik:RadButton CommandName="Selz" Height="16px" ID="imgDocType" Runat="server" Width="16px" />
		            </ItemTemplate>
	            </telerik:GridTemplateColumn>
	            <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderImageUrl="../Comm/Images/Loop16.gif" UniqueName="cDelete" HeaderTooltip="Restituzione">
                    <HeaderStyle HorizontalAlign="Center" Width="25px" CssClass="headerImage"/>
                    <ItemStyle HorizontalAlign="Center" />
		            <ItemTemplate>
                        <asp:Image ID="imgDelete" runat="server" />
		            </ItemTemplate>
	            </telerik:GridTemplateColumn>
	            <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderImageUrl="../Comm/Images/PriorityHigh16.gif" UniqueName="cPriority">
                    <HeaderStyle HorizontalAlign="Center" Width="25px" CssClass="headerImage"/>
                    <ItemStyle HorizontalAlign="Center"/>
		            <ItemTemplate>
                        <asp:Image runat="server" ID="imgPriority" />
		            </ItemTemplate>
	            </telerik:GridTemplateColumn>
	            <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/user.png" UniqueName="cPerson">
                    <HeaderStyle HorizontalAlign="Center" Width="25px" CssClass="headerImage" />
                    <ItemStyle HorizontalAlign="Center" />
	                <ItemTemplate>
                        <asp:Image ID="imgPerson" runat="server" />
		            </ItemTemplate>
	            </telerik:GridTemplateColumn>
	            <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/documentPreview.png" UniqueName="cType">
                    <HeaderStyle HorizontalAlign="Center" Width="25px" CssClass="headerImage" />
                    <ItemStyle HorizontalAlign="Center" />
		            <ItemTemplate>
                        <telerik:RadButton ToolTip="Apertura del documento" Height="16px" ButtonType="LinkButton" ID="imgType" Runat="server" Width="16px" />
		            </ItemTemplate>
	            </telerik:GridTemplateColumn>
	            <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/card_chip_gold.png" UniqueName="cDocSign">
    	            <HeaderStyle HorizontalAlign="Center" Width="25px" CssClass="headerImage" />
	                <ItemStyle HorizontalAlign="Center" />
		            <ItemTemplate>
                        <asp:Image ID="imgDocSign" runat="server"/>
		            </ItemTemplate>
	            </telerik:GridTemplateColumn>
	            <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderImageUrl="../Comm/Images/Flag16.gif" UniqueName="cUserDocSign">
		            <HeaderStyle HorizontalAlign="Center" Width="25px" CssClass="headerImage"/>
	                <ItemStyle HorizontalAlign="Center" />
	                <ItemTemplate>
                        <asp:Image ID="imgSign" runat="server" ToolTip="Documenti in modifica" />
	                </ItemTemplate>
	            </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn AllowFiltering="false" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/text_signature.png" 
                    HeaderTooltip="Collaborazione con firma obbligatoria" DataField="IsSignedRequired" HeaderText="Firma obbligatoria" GroupByExpression="IsSignedRequired Group By IsSignedRequired" UniqueName="signDocRequired">
		            <HeaderStyle HorizontalAlign="Center" Width="25px" CssClass="headerImage"/>
	                <ItemStyle HorizontalAlign="Center" />
	                <ItemTemplate>
                        <asp:Image ID="imgSignDocRequired" runat="server" />
	                </ItemTemplate>
	            </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn AllowFiltering="false" DataField="VersioningCount" Groupable="false" HeaderText="Ver." UniqueName="VersioningCount" AllowSorting="false">
		            <HeaderStyle HorizontalAlign="Center" Width="35px" Wrap="false" />
                    <ItemStyle HorizontalAlign="Center" Width="35px" />
	                <ItemTemplate>
                        <asp:Label ID="lblVersioningCount" runat="server"></asp:Label>
	                </ItemTemplate>
	            </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="Number" HeaderText="Numero" AllowSorting="false" AllowFiltering="false" Groupable="false">
                    <HeaderStyle HorizontalAlign="Center" Width="110px" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:LinkButton runat="server" ID="lnkNumber" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridDateTimeColumn CurrentFilterFunction="EqualTo" DataField="Entity.MemorandumDate" DataFormatString="{0:dd/MM/yyyy}" HeaderText="Data Prom." SortExpression="Entity.MemorandumDate" UniqueName="Entity.MemorandumDate" AllowSorting="false">  
                    <HeaderStyle HorizontalAlign="Center" Width="125px" Wrap="false" />
                    <ItemStyle HorizontalAlign="Center" />
                </telerik:GridDateTimeColumn>
	            <telerik:GridBoundColumn CurrentFilterFunction="Contains" DataField="Entity.Subject" HeaderText="Oggetto" SortExpression="Entity.Subject" UniqueName="Entity.Subject" AllowSorting="false">
                    <HeaderStyle HorizontalAlign="Center" Width="30%" Wrap="false" />
                    <ItemStyle HorizontalAlign="Left" />
	            </telerik:GridBoundColumn>
	            <telerik:GridBoundColumn CurrentFilterFunction="Contains" DataField="Entity.Note" Groupable="False" HeaderText="Note" SortExpression="Entity.Note" UniqueName="Entity.Note" AllowSorting="false">
                    <HeaderStyle HorizontalAlign="Center" Wrap="false" Width="150px" />
                    <ItemStyle HorizontalAlign="Left" />
	            </telerik:GridBoundColumn>
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="Proponente" UniqueName="Proposer" AllowSorting="false">
                    <HeaderStyle HorizontalAlign="Center" Width="10%" Wrap="False" />
                    <ItemStyle HorizontalAlign="Center" />
		            <ItemTemplate>
                        <asp:Label ID="lblProposer" runat="server" />
		            </ItemTemplate>
	            </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="Da visionare/firmare" UniqueName="cToSign" AllowSorting="false">
                    <HeaderStyle HorizontalAlign="Center" Width="20%" Wrap="False" />
                    <ItemStyle HorizontalAlign="Left" />
		            <ItemTemplate>
                        <asp:Label ID="lblSigns" runat="server" />
		            </ItemTemplate>
	            </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="Da protocollare" UniqueName="cToProtocol" AllowSorting="false">
                    <HeaderStyle HorizontalAlign="Center" Width="15%" Wrap="False" />
	                <ItemStyle HorizontalAlign="Left" />
		            <ItemTemplate>
                        <asp:Label ID="lblToProtocol" runat="server" />
		            </ItemTemplate>
	            </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="Download" UniqueName="cDownload">
                    <HeaderStyle HorizontalAlign="Center" Wrap="False" />
	                <ItemStyle HorizontalAlign="Center" />
		            <ItemTemplate>
                        <asp:ImageButton CommandName="Down" ID="imgDownload" ImageUrl="../Comm/images/saveas.gif" runat="server" />
		            </ItemTemplate>
	            </telerik:GridTemplateColumn>
                 <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" UniqueName="cHasDocumentExtracted" Visible="false" AllowSorting="false">
		            <HeaderStyle HorizontalAlign="Center" Width="25px" CssClass="headerImage"/>
	                <ItemStyle HorizontalAlign="Center" />
	                <ItemTemplate>
                        <asp:Label ID="lblHasDocumentExtracted" runat="server" />
	                </ItemTemplate>
	            </telerik:GridTemplateColumn>                
            </Columns>
        </MasterTableView>
        <ClientSettings AllowDragToGroup="True">
            <Selecting AllowRowSelect="true" />
        </ClientSettings>
    </DocSuite:BindGrid>
</div>
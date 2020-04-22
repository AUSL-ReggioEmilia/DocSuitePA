<%@ Control Language="vb" AutoEventWireup="false" Codebehind="uscDocmGrid.ascx.vb"
    Inherits="VecompSoftware.DocSuiteWeb.Gui.uscDocmGrid" %>

<div style="overflow:hidden;width:100%;height:100%;">
    <DocSuite:BindGrid AllowCustomPaging="True" AllowFilteringByColumn="true" AllowMultiRowSelection="true" AllowSorting="True" AutoGenerateColumns="False" GridLines="Both" ID="gvDocuments" runat="server" ShowGroupPanel="True" Visible="True">
        <PagerStyle AlwaysVisible="True" Mode="NextPrevAndNumeric" Position="TopAndBottom"
            ShowPagerText="false" />
        <MasterTableView TableLayout="Auto" AllowMultiColumnSorting="True" Width="100%" NoMasterRecordsText="Nessuna Pratica Trovata">
            <Columns>
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="False" UniqueName="ClientSelectColumn">
                    <headerstyle horizontalalign="Center" width="25px" />
                    <itemstyle horizontalalign="Center" />
                    <itemtemplate>
                        <asp:CheckBox AutoPostBack="False" id="cbSelect" runat="server" />
                    </itemtemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn HeaderImageUrl="../Comm/Images/DocSuite/Pratica16.gif"
                    UniqueName="cIcon" AllowFiltering="false" Groupable="false">
                    <headerstyle horizontalalign="Center" width="30px" CssClass="headerImage" />
                    <itemstyle horizontalalign="Center" CssClass="cellImage" />
                    <itemtemplate>
                        <asp:Image ID="Image1" runat="server" />
                    </itemtemplate>
                </telerik:GridTemplateColumn>
                <DocSuite:YearNumberBoundColumn HeaderText="Pratica" SortExpression="Id"
                    CurrentFilterFunction="EqualTo" UniqueName="Id" Groupable="false">
                    <HeaderStyle HorizontalAlign="Center" width="98px" />
                    <ItemStyle HorizontalAlign="center" />
                    <ItemTemplate>
                        <asp:LinkButton runat="server" ID="lnkPratica" CommandName="ShowDocm"></asp:LinkButton>
                    </ItemTemplate>
                </DocSuite:YearNumberBoundColumn>
                <telerik:GridDateTimeColumn DataField="StartDate" HeaderText="Data" SortExpression="StartDate"
                    DataFormatString="{0:dd/MM/yyyy}" CurrentFilterFunction="EqualTo" UniqueName="StartDate" >
                    <HeaderStyle horizontalalign="Center" wrap="false" width="125px"/>
                    <ItemStyle horizontalalign="Center"/>
                </telerik:GridDateTimeColumn>
                <telerik:GridTemplateColumn HeaderText="Cartella" UniqueName="FolderName" AllowFiltering="false" Groupable="false">
                    <headerstyle horizontalalign="Center" />
                    <itemstyle horizontalalign="Center" />
                    <itemtemplate>
                        <asp:LinkButton runat="server" ID="lnkFolder" CommandName="ShowDocmFolder"></asp:LinkButton>
                    </itemtemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridDateTimeColumn DataField="FolderExpiryDate" HeaderText="Data scadenza" SortExpression="DocumentFolders.ExpiryDate"
                    DataFormatString="{0:dd/MM/yyyy}" CurrentFilterFunction="EqualTo" UniqueName="FolderExpiryDate" AllowFiltering="false" Groupable="false">
                    <HeaderStyle horizontalalign="Center" wrap="false" width="125px"/>
                    <ItemStyle horizontalalign="Center"/>
                </telerik:GridDateTimeColumn>
                <telerik:GridBoundColumn DataField="FolderExpiryDescription" HeaderText="Descrizione" UniqueName="FolderExpiryDescription" AllowFiltering="false" Groupable="false">
                    <HeaderStyle horizontalalign="Center" wrap="false" />
                    <ItemStyle horizontalalign="Left" />
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="ContainerName" HeaderText="Contenitore" SortExpression="Container.Name" CurrentFilterFunction="Contains" UniqueName="Container.Name">
                    <HeaderStyle horizontalalign="Center" wrap="false" />
	    		    <ItemStyle horizontalalign="Left" />
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="RoleName" HeaderText="Apertura" SortExpression="Role.Name" CurrentFilterFunction="Contains" UniqueName="Role.Name">
                    <HeaderStyle horizontalalign="Center" wrap="false" />
	    		    <ItemStyle horizontalalign="Left" />
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="CategoryName" HeaderText="Classificatore" SortExpression="Category.Name" CurrentFilterFunction="Contains" UniqueName="Category.Name">
                    <HeaderStyle horizontalalign="Center" wrap="false" />
	    		    <ItemStyle horizontalalign="Left" />
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="ContactDescription" HeaderText="Riferimento" AllowSorting="false" CurrentFilterFunction="Contains" UniqueName="Contact.Description">
                    <HeaderStyle horizontalalign="Left" wrap="false" />
	    		    <ItemStyle horizontalalign="Left" />
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="DocumentObjectDescription" HeaderText="Documento" SortExpression="DO.Description" CurrentFilterFunction="Contains" UniqueName="DO.Description">
                    <HeaderStyle horizontalalign="Center" wrap="false" />
	    		    <ItemStyle horizontalalign="Left" />
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="ServiceNumber" HeaderText="Num. Posiz." SortExpression="ServiceNumber" CurrentFilterFunction="Contains" UniqueName="ServiceNumber">
                    <HeaderStyle horizontalalign="Center" width="90px" />
	    		    <ItemStyle horizontalalign="Center" width="90px" />
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Name" HeaderText="Nome" CurrentFilterFunction="Contains" SortExpression="Name" UniqueName="Name">
                    <HeaderStyle horizontalalign="Center" wrap="false" />
	    		    <ItemStyle horizontalalign="Left" />
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="DocumentObject" HeaderText="Oggetto" CurrentFilterFunction="Contains"  UniqueName="DocumentObject">
                    <HeaderStyle horizontalalign="Center" wrap="false" />
	    		    <ItemStyle horizontalalign="Left" />
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn DataField="Manager" HeaderText="Responsabile" CurrentFilterFunction="Contains" UniqueName="Manager">
                    <HeaderStyle horizontalalign="Center" wrap="false" />
	    		    <ItemStyle horizontalalign="Left" />
                </telerik:GridBoundColumn>
            </Columns>
        </MasterTableView>
        <ClientSettings AllowGroupExpandCollapse="True" AllowDragToGroup="True">
            <Selecting AllowRowSelect="true" />
        </ClientSettings>
    </DocSuite:BindGrid>
</div>
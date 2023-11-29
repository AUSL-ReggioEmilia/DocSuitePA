<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscProtGrid.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscProtGrid" %>

<telerik:RadScriptBlock runat="server" ID="rsbContent" EnableViewState="false">
    <script type="text/javascript">
    </script>
</telerik:RadScriptBlock>

<div style="overflow:hidden;width:100%;height:100%;">
    <DocSuite:BindGrid AllowMultiRowSelection="true" AutoGenerateColumns="False" GridLines="Both" ID="grdProtocols" runat="server" ShowGroupPanel="True">
        <MasterTableView AllowFilteringByColumn="True" GridLines="Both"  NoMasterRecordsText="Nessun protocollo trovato" TableLayout="Fixed" >
            <Columns>
                <telerik:GridClientSelectColumn UniqueName="colSelection" runat="server" Visible="false" HeaderStyle-CssClass="headerImage" ItemStyle-CssClass="cellImage"/>
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="False" UniqueName="colClientSelect"   >
                    <HeaderStyle HorizontalAlign="Center" CssClass="headerImage" Width="30px"/>
                    <ItemStyle HorizontalAlign="Center"/>
                    <ItemTemplate>
                        <asp:CheckBox AutoPostBack="False" ID="cbSelect" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="colActions" HeaderText="Azioni" AllowFiltering="false" Groupable="false" Visible="False">
                    <HeaderStyle HorizontalAlign="Center" CssClass="headerImage"/>
                    <ItemStyle HorizontalAlign="Center" Wrap="false" />
                    <ItemTemplate>
                        <asp:LinkButton runat="server" ID="lbtRepair" Text="Ripara" CommandName="Repair" Style="margin-right: 10px;" />
                        <asp:LinkButton runat="server" ID="lbtRedo" Text="Nuovo protocollo" CommandName="Redo" Style="margin-right: 10px;" />
                        <asp:LinkButton runat="server" ID="lbtRecover" Text="Recupera" CommandName="Recover" />
                        <asp:LinkButton runat="server" ID="lbtPresaCarico" Text="Presa in carico" CommandName="Steal" />&nbsp;
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="colPartialActions" HeaderText="Azioni" AllowFiltering="false" Groupable="false" Visible="false">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" Wrap="false" />
                    <ItemTemplate>
                        <asp:LinkButton runat="server" ID="lbtCompleta" Text="Completa" CommandName="Complete" Style="margin-right: 10px;" />
                        <asp:LinkButton runat="server" ID="lbtAnnulla" Text="Annulla" CommandName="Cancel" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="colHasRead" HeaderText="Da leggere" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/mail.png" AllowFiltering="false" Groupable="false">
                    <HeaderStyle HorizontalAlign="Center" CssClass="headerImage"/>
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:Image ID="imgHasRead" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="colProtocolType" HeaderText="Tipo" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/contentType.png" AllowFiltering="false" Groupable="false">
                    <HeaderStyle HorizontalAlign="Center" CssClass="headerImage"/>
                    <ItemStyle HorizontalAlign="Center"/>
                    <ItemTemplate>
                        <asp:Image ID="imgProtocolType" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="colViewDocuments" HeaderText="Tipologia spedizione" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/documentPreview.png" AllowFiltering="false" Groupable="false">
                    <HeaderStyle HorizontalAlign="Center" CssClass="headerImage"/>
                    <ItemStyle HorizontalAlign="Center"/>
                    <ItemTemplate>
                        <asp:ImageButton ID="ibtViewDocuments" runat="server" CommandName="ViewDocuments" />
                        <asp:HiddenField runat="server" ID="hf_protocol_unique" Value="" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="colViewLinks" HeaderText="Collegamenti" HeaderImageUrl="~/Comm/Images/DocSuite/Link16.png" AllowFiltering="false" Groupable="false">
                    <HeaderStyle HorizontalAlign="Center" CssClass="headerImage"/>
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <telerik:radButton ButtonType="LinkButton" Height="16px" ID="cmdViewLinks" runat="server" Width="16px" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="ingoingPec" HeaderText="Protocollo da PEC" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/document_signature.png" AllowFiltering="false" Groupable="false">
                    <HeaderStyle HorizontalAlign="Center" CssClass="headerImage"/>
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:Image ID="imgIngoingPec" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <DocSuite:YearNumberBoundColumn UniqueName="Id" HeaderText="Protocollo" CurrentFilterFunction="EqualTo" Groupable="false" SortExpression="Id">
                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                    <ItemStyle HorizontalAlign="center"/>
                    <ItemTemplate>
                        <asp:LinkButton runat="server" ID="lbtViewProtocol" CommandName="ViewProtocol" />
                    </ItemTemplate>
                </DocSuite:YearNumberBoundColumn>
                <DocSuite:YearNumberBoundColumn UniqueName="colFullProtocolNumber" HeaderText="Protocollo" CurrentFilterFunction="EqualTo" Groupable="false" SortExpression="Id" Visible="False">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="center"/>
                    <ItemTemplate>
                        <asp:Label ID="lblFullProtocolNumber" runat="server" />
                    </ItemTemplate>
                </DocSuite:YearNumberBoundColumn>
                <DocSuite:GridDateTimeColumnEx UniqueName="RegistrationDate" HeaderText="Data registrazione" DataField="RegistrationDate" DataFormatString="{0:dd/MM/yyyy}" CurrentFilterFunction="EqualTo" SortExpression="RegistrationDate" Groupable="false" >
                    <HeaderStyle HorizontalAlign="Center" Wrap="false" Width="150px" />
                    <ItemStyle HorizontalAlign="Center"/>
                </DocSuite:GridDateTimeColumnEx>
                <DocSuite:SuggestFilteringColumn UniqueName="Type.Id" HeaderText="I/U" DataField="Type.ShortDescription" CurrentFilterFunction="EqualTo" SortExpression="Type.Description" DataType="System.Int32" Groupable="false" >
                    <HeaderStyle HorizontalAlign="Center" Width="40" />
                    <ItemStyle HorizontalAlign="Center"/>
                </DocSuite:SuggestFilteringColumn>   
                <telerik:GridTemplateColumn UniqueName="colAcceptanceRoles" HeaderText="Non accettati" AllowFiltering="false" Groupable="false" Visible="false" >
                    <HeaderStyle HorizontalAlign="Center" Width="80px" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                      <telerik:RadButton runat="server" ID="btnToEvaluateRoles" ButtonType="LinkButton" ToolTip="Settori non ancora accettati" Visible="false" Width="30px"/>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>              
                <telerik:GridBoundColumn CurrentFilterFunction="Contains" DataField="ContainerName" HeaderText="Contenitore" SortExpression="Container.Name" UniqueName="Container.Name">
                    <HeaderStyle HorizontalAlign="Center" Wrap="false" />
                    <ItemStyle HorizontalAlign="Left" />
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn AllowFiltering="False" DataField="Subject" HeaderText="Ass / Prop" HeaderTooltip="Assegnatario / Proponente" SortExpression="Subject" UniqueName="colSubject" Visible="False">
                    <HeaderStyle HorizontalAlign="Center" Wrap="false" />
                    <ItemStyle HorizontalAlign="Left" />
                </telerik:GridBoundColumn>
                <DocSuite:CompositeTemplateColumnSqlExpression CurrentFilterFunction="Contains" Groupable="False" HeaderText="Classificatore" SortExpression="Category.Name" UniqueName="Category.Name">
                    <HeaderStyle HorizontalAlign="Center" Wrap="true"/>
                    <ItemStyle HorizontalAlign="Left" Wrap="true"/>
                    <ItemTemplate>
                        <asp:Label ID="lblCategoryProjection" runat="server" />
                    </ItemTemplate>
                </DocSuite:CompositeTemplateColumnSqlExpression>
                <telerik:GridTemplateColumn UniqueName="colProtocolContact" HeaderText="Mitt/Dest" AllowFiltering="false" Groupable="false">
                    <HeaderStyle HorizontalAlign="Center" Wrap="false" />
                    <ItemStyle HorizontalAlign="Left" />
                    <ItemTemplate>
                        <asp:Label ID="lblProtocolContact" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn CurrentFilterFunction="Contains" HeaderText="Utente" SortExpression="RegistrationUser" UniqueName="RegistrationUser">
                    <HeaderStyle HorizontalAlign="Center" Wrap="false" />
                    <ItemStyle HorizontalAlign="Left" />
                    <ItemTemplate>
                        <asp:Label ID="lblRegistrationUser" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="ProtocolObject" HeaderText="Oggetto" Groupable="false" AllowFiltering="true" CurrentFilterFunction="Contains">
                    <HeaderStyle HorizontalAlign="Center"/>
                    <ItemStyle HorizontalAlign="Left"/>
                    <ItemTemplate>
                        <asp:Label ID="lblProtocolObject" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="PU.RegistrationUser" Visible="False" HeaderText="Evidenza da" Groupable="false" AllowFiltering="false">
                    <HeaderStyle HorizontalAlign="Center" Wrap="false" />
                    <ItemStyle HorizontalAlign="Left" />
                    <ItemTemplate>
                        <asp:Label ID="lblHighlightRegistrationUser" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="PU.Note" Visible="False" HeaderText="Note" Groupable="false" AllowFiltering="true" CurrentFilterFunction="Contains">
                    <HeaderStyle HorizontalAlign="Center"/>
                    <ItemStyle HorizontalAlign="Left"/>
                    <ItemTemplate>
                        <asp:Label ID="lblProtocolNote" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <DocSuite:SuggestFilteringTemplateColumn UniqueName="Status.Description" HeaderText="Stato Prot." DataField="StatusDescription" CurrentFilterFunction="Contains" SortExpression="Status.Description">
                    <HeaderStyle HorizontalAlign="Center" Wrap="false" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:Label ID="lblProtocolStatus" runat="server" />
                    </ItemTemplate>
                </DocSuite:SuggestFilteringTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="AP.AccountingSectional" HeaderText="Sezionale" Visible="false" AllowFiltering="true" CurrentFilterFunction="Contains" SortExpression="AP.AccountingSectional">
                    <HeaderStyle HorizontalAlign="Center"  Wrap="false" Width="150px"/>
                    <ItemStyle HorizontalAlign="Left" />
                    <ItemTemplate>
                        <asp:Label ID="lblSectional" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>  
                <telerik:GridTemplateColumn UniqueName="AP.InvoiceYear" HeaderText="Anno Fattura" AllowFiltering="true" AllowSorting="true" CurrentFilterFunction="EqualTo" SortExpression="AP.InvoiceYear" DataType="System.Int32" Groupable="false">
                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                    <ItemStyle HorizontalAlign="center"/>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblInvoiceYear" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="AP.InvoiceNumber" HeaderText="Numero Fattura" AllowFiltering="true" AllowSorting="true" CurrentFilterFunction="EqualTo" SortExpression="AP.InvoiceNumber" Groupable="false">
                    <HeaderStyle HorizontalAlign="Center" Wrap="false" Width="100px" />
                    <ItemStyle HorizontalAlign="center"/>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblInvoiceNumber" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn UniqueName="AP.AccountingNumber" HeaderText="Protocollo Iva" AllowFiltering="true" AllowSorting="true" CurrentFilterFunction="EqualTo" SortExpression="AP.AccountingNumber" DataType="System.Int32" Groupable="false">
                    <HeaderStyle HorizontalAlign="Center" Wrap="false" Width="100px" />
                    <ItemStyle HorizontalAlign="center"/>
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblAccountingNumber" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
        <ClientSettings>
            <Selecting AllowRowSelect="True" UseClientSelectColumnOnly="True" />
        </ClientSettings>
    </DocSuite:BindGrid>

    <telerik:RadWindowManager EnableViewState="False" ID="rwmRoles" runat="server">
        <Windows>
            <telerik:RadWindow ID="windowToEvaluateRoles"  runat="server" Title="Autorizzazioni non accettate" />
        </Windows>

    </telerik:RadWindowManager>  

</div>
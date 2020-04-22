<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscReslGrid.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscReslGrid" %>

<telerik:RadScriptBlock runat="server" ID="rsbContent" EnableViewState="false">
    <script type="text/javascript">
        function OpenParerDetail(parerDetailUrl) {
            var wnd = window.radopen(parerDetailUrl, "parerDetailWindow");
            wnd.setSize(400, 300);
            wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close);
            wnd.set_visibleStatusbar(false);
            wnd.set_modal(true);
            wnd.center();
            return false;
        }
    </script>
</telerik:RadScriptBlock>

<div style="overflow: hidden; width: 100%; height: 100%;">
    <DocSuite:BindGrid AutoGenerateColumns="False" GridLines="Both" ID="grdResolutions" runat="server" ShowGroupPanel="True">
        <MasterTableView AllowFilteringByColumn="true" NoMasterRecordsText="Nessun Atto Trovato" TableLayout="Auto">
            <Columns>
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" UniqueName="ClientSelectColumn">
                    <HeaderStyle HorizontalAlign="Center" Width="30" />
                    <ItemStyle HorizontalAlign="Center" Width="30" />
                    <ItemTemplate>
                        <asp:CheckBox AutoPostBack="false" CommandName="Selected" ID="cbSelect" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" UniqueName="PARERIcon" Visible="false">
                    <HeaderStyle HorizontalAlign="Center" CssClass="headerImage" />
                    <ItemStyle HorizontalAlign="Center" CssClass="cellImage" />
                    <ItemTemplate>
                        <asp:Image ID="imgParer" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="Stato AAchiviazione" UniqueName="PARERDescription" Visible="false">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="center" />
                    <ItemTemplate>
                        <asp:LinkButton ID="lnkParerDetail" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderImageUrl="../Resl/Images/Atto.gif" HeaderText="T" UniqueName="T">
                    <HeaderStyle HorizontalAlign="Center" CssClass="headerImage" />
                    <ItemStyle HorizontalAlign="Center" CssClass="cellImage" />
                    <ItemTemplate>
                        <asp:Image ID="imgTipoAtto" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/card_chip_gold.png" HeaderText="Firma Qualificata" UniqueName="S">
                    <HeaderStyle HorizontalAlign="Center" CssClass="headerImage" />
                    <ItemStyle HorizontalAlign="Center" CssClass="cellImage" />
                    <ItemTemplate>
                        <asp:Image ID="imgSigned" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderImageUrl="../App_Themes/DocSuite2008/imgset16/card_chip_gold.png" HeaderText="Tipo Documento" UniqueName="D">
                    <HeaderStyle HorizontalAlign="Center" CssClass="headerImage" />
                    <ItemStyle HorizontalAlign="Center" CssClass="cellImage" />
                    <ItemTemplate>
                        <asp:ImageButton CommandName="Open" ID="ibtDocumentType" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="" UniqueName="ReturnFromCollaboration" Visible="false">
                    <HeaderStyle HorizontalAlign="Center" Width="30" />
                    <ItemStyle HorizontalAlign="Center" Width="30" />
                    <ItemTemplate>
                        <asp:Image ID="imgReturnFromCollaboration" runat="server" ToolTip="Collaborazione annullata" />
                        <asp:Image ID="imgRetroStepResolution" runat="server" ToolTip="Da retrostep affari generale" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <DocSuite:SuggestFilteringTemplateColumn DataField="TypeId" DataType="System.Int16" Groupable="false" HeaderText="Documento" SortExpression="Type.Id" UniqueName="Type.Id">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Left" />
                    <ItemTemplate>
                        <asp:Label ID="lblResolutionType" runat="server" />
                    </ItemTemplate>
                </DocSuite:SuggestFilteringTemplateColumn>
                <telerik:GridBoundColumn CurrentFilterFunction="EqualTo" DataField="Year" HeaderText="Anno" SortExpression="Year" UniqueName="Year">
                    <HeaderStyle HorizontalAlign="Left" Wrap="false" Width="40" />
                    <ItemStyle HorizontalAlign="Left" Width="40" />
                </telerik:GridBoundColumn>
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="Numero" SortExpression="R.Id" UniqueName="Id">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Left" />
                    <ItemTemplate>
                        <asp:LinkButton CommandName="ShowResl" ID="lnkResolution" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridDateTimeColumn CurrentFilterFunction="EqualTo" DataField="AdoptionDate" DataFormatString="{0:dd/MM/yyyy}" HeaderText="Data Adozione" SortExpression="AdoptionDate" UniqueName="AdoptionDate">
                    <HeaderStyle HorizontalAlign="Center" Wrap="false" />
                    <ItemStyle HorizontalAlign="Center" />
                </telerik:GridDateTimeColumn>
                <telerik:GridTemplateColumn AllowFiltering="false" DataType="System.Int16" Groupable="false" HeaderText="Stato" SortExpression="Status.Id" UniqueName="Status.Id">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Left" />
                    <ItemTemplate>
                        <asp:Label ID="lblResolutionStatus" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="Data Stato" SortExpression="RW.RegistrationDate" UniqueName="Data">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:Label ID="lblRegistrationDate" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn CurrentFilterFunction="Contains" DataField="ControllerStatusAcronym" HeaderText="OC" SortExpression="ControllerStatus.Acronym" UniqueName="ControllerStatus.Acronym">
                    <HeaderStyle HorizontalAlign="Left" Wrap="false" />
                    <ItemStyle HorizontalAlign="Left" Wrap="false" />
                </telerik:GridBoundColumn>
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="Tip. C." SortExpression="TipOC" UniqueName="TipOC">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Left" />
                    <ItemTemplate>
                        <asp:Label ID="lblTipOC" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn CurrentFilterFunction="Contains" DataField="ContainerName" HeaderText="Contenitore" SortExpression="Container.Name" UniqueName="Container.Name">
                    <HeaderStyle HorizontalAlign="Left" Wrap="false" />
                    <ItemStyle HorizontalAlign="Left" />
                </telerik:GridBoundColumn>
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="Motivo Annullamento" SortExpression="DeclineNote" UniqueName="DeclineNote" Visible="False">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Left" />
                    <ItemTemplate>
                        <p>
                            <asp:Label ID="lblDeclineNoteMessage" runat="server" />
                        </p>
                        <p>
                            <asp:Label ID="lblDeclineNoteStepNumber" runat="server" />
                        </p>
                        <p>
                            <asp:Label ID="lblDeclineNoteStepName" runat="server" />
                        </p>
                        <p>
                            <asp:Label ID="lblDeclineNoteDate" runat="server" />
                        </p>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridBoundColumn CurrentFilterFunction="Contains" DataField="CategoryName" HeaderText="Classificatore" SortExpression="Category.Name" UniqueName="Category.Name">
                    <HeaderStyle HorizontalAlign="Left" Wrap="false" />
                    <ItemStyle HorizontalAlign="Left" />
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn AllowFiltering="false" DataField="ProposerCode" Groupable="false" HeaderText="Prop." UniqueName="ProposerCode">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Left" />
                </telerik:GridBoundColumn>
                <telerik:GridBoundColumn CurrentFilterFunction="Contains" DataField="ResolutionObject" HeaderText="Oggetto" SortExpression="ResolutionObject" UniqueName="ResolutionObject">
                    <HeaderStyle HorizontalAlign="Left" CssClass="ImportantRuleODG" />
                    <ItemStyle HorizontalAlign="Left" Wrap="true" />
                </telerik:GridBoundColumn>
                <telerik:GridCheckBoxColumn AllowFiltering="false" AllowSorting="false" DataField="OCRegion" Groupable="false" HeaderText="Regione" UniqueName="Regione" Visible="false">
                    <HeaderStyle HorizontalAlign="Left" Width="1px" />
                    <ItemStyle HorizontalAlign="Left" Width="1px" />
                </telerik:GridCheckBoxColumn>
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="Documenti" UniqueName="AllegatiSelectColumn">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Left" />
                    <ItemTemplate>
                        <asp:CheckBoxList DataTextField="Name" DataValueField="ID" ID="DocumentiCheckList" runat="server" CssClass="noBorder" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn AllowFiltering="false" Groupable="false" HeaderText="Controllo" UniqueName="LastReslLog" Visible="false">
                    <HeaderStyle HorizontalAlign="Left" />
                    <ItemStyle HorizontalAlign="Left" />
                    <ItemTemplate>
                        <asp:Label ID="lblLastReslLog" runat="server" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn AllowFiltering="false" Visible="false" Groupable="false" HeaderText="Stato Elaborazione" UniqueName="ResolutionTaskStatus">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:Image runat="server" ID="imgResolutionTaskStatus" Visible="false" />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>
        </MasterTableView>
        <ClientSettings>
            <Selecting AllowRowSelect="true" />
        </ClientSettings>
    </DocSuite:BindGrid>
</div>
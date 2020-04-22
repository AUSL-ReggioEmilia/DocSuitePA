<%@ Page Title="" Language="vb" AutoEventWireup="false" MasterPageFile="~/MasterPages/DocSuite2008.Master" CodeBehind="UserDiarioUnificato.aspx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.UserDiarioUnificato" %>
<%@ Import Namespace="VecompSoftware.DocSuiteWeb.Gui" %>

  
<asp:Content ContentPlaceHolderID="cphHeader" runat="server">
    <style type="text/css" media="all">
        .rddlPopup .rddlList {
            height:100px !important;
        }
    </style>

    <asp:Panel runat="server" DefaultButton="cmdRefreshgrid" ID="pnlFilters">
       <table class="datatable">
           <tr>
                <td class="label">Diario:</td>
                <td>
                    <span class="miniLabel">Da</span>
                    <telerik:RadDatePicker ID="rdpDateFrom" runat="server" />
                    <span class="miniLabel">A</span>
                    <telerik:RadDatePicker ID="rdpDateTo" runat="server" />
                </td>
            </tr>
           <tr>
                <td class="label">Tipologia:</td>
                <td>
                    <telerik:RadDropDownList ID="drpType" runat="server" Width="250px"/>
                </td>
            </tr>
           <tr>
                <td class="label">Oggetto:</td>
                <td>
                     <asp:TextBox ID="txtFinderSubject" runat="server" Width="300"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <hr />
                </td>
            </tr>
           </table>
        <asp:Button ID="cmdRefreshGrid" runat="server" Width="200px" Text="Aggiorna visualizzazione" />        
    </asp:Panel>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphContent" style="overflow:hidden;width:100%;height:100%;">
    <asp:Panel ID="pnlDiarioGrid" runat="server" style="overflow:hidden;width:100%;height:100%;">

    <DocSuite:BindGrid AllowFilteringByColumn="false" AllowMultiRowSelection="False" AutoGenerateColumns="False" GridLines="Both" ID="gvDiarioUnificato" runat="server" ShowGroupPanel="False">
        
        <MasterTableView CommandItemDisplay="None" CurrentResetPageIndexAction="SetPageIndexToFirst" DataKeyNames="Id" Dir="LTR" TableLayout="Auto"  Name="MasterGrid">
            
            <ItemStyle CssClass="Scuro" />
            <AlternatingItemStyle CssClass="Chiaro" />

            <DetailTables>       
                <telerik:GridTableView Name="ChildGrid" AllowPaging="True" GridLines="Both" AllowSorting="True"  runat="server">
                <Columns>
                            <telerik:GridTemplateColumn SortExpression="" HeaderText="Data ora" DataField="DetailDate" UniqueName="DetailDate">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDetailDate" runat="server" />&nbsp;
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn SortExpression="" HeaderText="Tipologia" DataField="DetailType" UniqueName="DetailType">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDetailType" runat="server" />&nbsp;
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                            <telerik:GridTemplateColumn SortExpression="" HeaderText="Oggetto" DataField="DetailDescription" UniqueName="DetailDescription">
                                    <ItemTemplate>
                                        <asp:Label ID="lblDetailDescription" runat="server" />&nbsp;
                                    </ItemTemplate>
                                </telerik:GridTemplateColumn>
                </Columns>

                </telerik:GridTableView>
            </DetailTables>

            <Columns>              
                <telerik:GridTemplateColumn SortExpression="" HeaderText="LOG" HeaderStyle-Width="40px" ItemStyle-Width="40px"  DataField="" UniqueName="LOG">
                    <ItemTemplate>
                        <telerik:RadButton runat="server" ID="btnLog" ButtonType="StandardButton" Enabled="false" Width="16px" Height="16px" CommandName="Log" Image-ImageUrl="<%# ImagePath.SmallLog%>"  />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                  <telerik:GridTemplateColumn SortExpression="" HeaderText="Tipologia"  DataField="Type" UniqueName="Type">
                    <ItemTemplate>
                        <asp:Label ID="lblType" runat="server" />&nbsp;
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                   <telerik:GridTemplateColumn SortExpression="" HeaderText="Data ora" DataField="LogDate"  UniqueName="LogDate" >
                    <ItemTemplate>
                        <asp:Label ID="lblLogDate" runat="server" />&nbsp;
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
              
                <telerik:GridTemplateColumn SortExpression="" HeaderText="" HeaderImageUrl="../Comm/Images/Mails/highimportance.gif" DataField="" UniqueName="" HeaderTooltip="Livello del log">
                    <ItemTemplate>
                        <asp:Image ID="imgPriority" runat="server" Visible="true" ToolTip="Attenzione evento importante" ImageUrl="~/Comm/Images/Mails/highimportance.gif"/>
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
             
                <telerik:GridTemplateColumn SortExpression="" HeaderText="Collegamento"  DataField="LogType" UniqueName="LogType">
                    <ItemTemplate>
                        <asp:HyperLink ID="lblLogType" runat="server" />&nbsp;
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn SortExpression="" HeaderText="Oggetto" DataField="LogDescription" UniqueName="LogDescription">
                    <ItemTemplate>
                        <asp:Label ID="lblLogDescription" runat="server" />&nbsp;
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
                <telerik:GridTemplateColumn SortExpression="" HeaderText=""  DataField="Id" UniqueName="Id" Visible="false">
                    <ItemTemplate>
                            <asp:HiddenField ID="hfType" runat="server" />
                            <asp:HiddenField ID="hfId" runat="server" />
                            <asp:HiddenField ID="hfYear" runat="server" />
                            <asp:HiddenField ID="hfNumber" runat="server"  />
                            <asp:HiddenField ID="hfUniqueId" runat="server"  />
                    </ItemTemplate>
                </telerik:GridTemplateColumn>
            </Columns>                
        </MasterTableView>    
    </DocSuite:BindGrid>

</asp:Panel>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="cphFooter">
    
</asp:Content>
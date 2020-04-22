<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CustomerSatisfactionAggregate.aspx.cs" MasterPageFile="~/MasterPages/DocumentSeries.Master" Inherits="AmministrazioneTrasparente.CustomerSatisfactionAggregate" %>

<asp:Content runat="server" ContentPlaceHolderID="MainPlaceHolder">
    <telerik:RadLabel runat="server" Text='DA DEFINIRE; ORA METTETE UN "TODO"' Font-Size="Larger" />
    <br />
    <telerik:RadMonthYearPicker runat="server" ID="rdpReportDate" AutoPostBack="true"
        OnSelectedDateChanged="rdpReportDate_SelectedDateChanged" />
    <br />
    <telerik:RadGrid runat="server" ID="reportGrid" AutoGenerateColumns="true"
        OnDetailTableDataBind="reportGrid_DetailTableDataBind">
        <MasterTableView CommandItemDisplay="None" AutoGenerateColumns="false" TableLayout="Fixed"
            DataKeyNames="Question">
            <DetailTables>
                <telerik:GridTableView Name="Responses" AutoGenerateColumns="false">
                    <Columns>
                        <telerik:GridBoundColumn DataField="Response" HeaderText="Risposta" />
                        <telerik:GridBoundColumn DataField="PercentageResponse" HeaderText="Percentuale" />
                    </Columns>
                </telerik:GridTableView>
            </DetailTables>
            <Columns>
                <telerik:GridBoundColumn DataField="Question" HeaderText="Domanda" />
            </Columns>
        </MasterTableView>
    </telerik:RadGrid>
</asp:Content>

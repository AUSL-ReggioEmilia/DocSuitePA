<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPages/Admin.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="AmministrazioneTrasparente.Admin.Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="content" runat="server">
    <div class="row">
        <div class="col-xs-6 col-sm-3"></div>
        <div class="col-xs-6 col-sm-6">
            <div class="panel panel-default" id="pnl-login">
                <div class="panel-heading">
                    <h3 class="panel-title">Accedi alla sezione amministrativa</h3>
                </div>
                <div class="panel-body">
                    <div class="form-horizontal" role="form">
                        <div class="form-group">
                            <label class="col-sm-2 control-label">Username:</label>
                            <div class="col-sm-10">
                                <asp:TextBox runat="server" ID="username" CssClass="form-control"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="userNameRequired" Display="Dynamic" runat="server" CssClass="validation-error" ControlToValidate="username" ErrorMessage="Il campo Username è obbligatorio."></asp:RequiredFieldValidator>
                            </div>
                        </div>
                        <div class="form-group">
                            <label class="col-sm-2 control-label">Password:</label>
                            <div class="col-sm-10">
                                <asp:TextBox runat="server" ID="password" CssClass="form-control" TextMode="Password"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="passwordRequired" Display="Dynamic" runat="server" CssClass="validation-error" ControlToValidate="password" ErrorMessage="Il campo Password è obbligatorio."></asp:RequiredFieldValidator>
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="col-sm-offset-2 col-sm-10">
                                <div class="checkbox" id="rememeber">
                                    <label>
                                        <asp:CheckBox runat="server" ID="rememberMe" Text="Ricorda accesso" />
                                    </label>
                                </div>
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="col-sm-offset-2 col-sm-10" id="login-div">
                                <asp:Button runat="server" CssClass="btn btn-primary" Text="Accedi" ID="btnLogin" OnClick="btnLogin_OnClick" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="col-xs-6 col-sm-3"></div>
    </div>
</asp:Content>

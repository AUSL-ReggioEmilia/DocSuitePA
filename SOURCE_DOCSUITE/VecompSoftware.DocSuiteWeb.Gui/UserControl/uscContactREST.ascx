<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscContactRest.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscContactRest" %>

<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<%@ Register Src="~/UserControl/uscContactSearchRest.ascx" TagPrefix="uc1" TagName="uscContactSearchRest" %>

<telerik:RadScriptBlock runat="server">
    <script type="text/javascript">
        var <%= Me.ClientID %>_uscContactRest;
        require(["UserControl/uscContactRest"], function (UscContactRest) {
            $(function () {
                <%= Me.ClientID %>_uscContactRest = new UscContactRest(tenantModelConfiguration.serviceConfiguration);
                <%= Me.ClientID %>_uscContactRest.uscContactSearchRestId = "<%= uscContactSearchRest.MainPanel.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.rowNameId = "<%= rowName.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.txtNameId = "<%= txtName.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.rowSurnameId = "<%= rowSurname.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.txtSurnameId = "<%= txtSurname.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.rowDescriptionId = "<%= rowDescription.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.txtDescriptionId = "<%= txtDescription.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.rowBirthdateId = "<%= rowBirthdate.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.rdpBirthdateId = "<%= rdpBirthdate.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.rowTitleId = "<%= rowTitle.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.rcbTitleId = "<%= rcbTitle.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.rowCertifiedMailId = "<%= rowCertifiedMail.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.txtCertifiedMailId = "<%= txtCertifiedMail.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.rowCodeId = "<%= rowCode.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.txtCodeId = "<%= txtCode.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.rowPivaId = "<%= rowPiva.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.txtPivaId = "<%= txtPiva.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.rowAddressTypeId = "<%= rowAddressType.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.rcbAddressTypeId = "<%= rcbAddressType.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.rowAddressId = "<%= rowAddress.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.txtAddressId = "<%= txtAddress.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.rowCivicNumberId = "<%= rowCivicNumber.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.txtCivicNumberId = "<%= txtCivicNumber.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.rowZipCodeId = "<%= rowZipCode.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.txtZipCodeId = "<%= txtZipCode.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.rowCityId = "<%= rowCity.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.txtCityId = "<%= txtCity.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.rowCityCodeId = "<%= rowCityCode.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.txtCityCodeId = "<%= txtCityCode.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.rowTelephoneNumberId = "<%= rowTelephoneNumber.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.txtTelephoneNumberId = "<%= txtTelephoneNumber.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.rowFAXId = "<%= rowFAX.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.txtFAXId = "<%= txtFAX.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.rowBirthplaceId = "<%= rowBirthplace.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.txtBirthplaceId = "<%= txtBirthplace.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.rowNationalityId = "<%= rowNationality.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.txtNationalityId = "<%= txtNationality.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.rowLanguageId = "<%= rowLanguage.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.rcbLanguageId = "<%= rcbLanguage.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.rowEmailId = "<%= rowEmail.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.txtEmailId = "<%= txtEmail.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.rowNoteId = "<%= rowNote.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.txtNoteId = "<%= txtNote.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.pnlMainId = "<%= pnlMain.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.pnlContentId = "<%= pnlContent.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.pnlToolbarId = "<%= pnlToolbar.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.btnCollapseInformationsId = "<%= btnCollapseInformations.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.toolbarId = "<%= toolbar.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.btnNewContactId = "<%= btnNewContact.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.ajaxLoadingPanelId = "<%= BasePage.MasterDocSuite.AjaxDefaultLoadingPanel.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.rowTreeContactId = "<%= rowTreeContact.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.clientId = "<%= Me.ClientID %>";
                <%= Me.ClientID %>_uscContactRest.callerId = "<%= Me.CallerId %>";
                <%= Me.ClientID %>_uscContactRest.spidEnabled = <%= ProtocolEnv.SpidEnabled.ToString().ToLower() %>;
                <%= Me.ClientID %>_uscContactRest.contactNationalityEnabled = <%= ProtocolEnv.ContactNationalityEnabled.ToString().ToLower() %>;
                <%= Me.ClientID %>_uscContactRest.initialize();
            });
        });
    </script>
</telerik:RadScriptBlock>

<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>
<div class="splitterWrapper">
    <telerik:RadSplitter runat="server" Height="100%" Width="100%" ResizeWithParentPane="False" Orientation="Horizontal">
        <telerik:RadPane runat="server" Height="38px" Width="100%" Scrolling="None">
            <div class="dsw-panel">
                <div class="dsw-panel-content">
                    <telerik:RadSplitter runat="server" Width="100%" Height="30px" ResizeWithParentPane="false">
                        <telerik:RadPane runat="server" Width="100%">
                            <uc1:uscContactSearchRest runat="server" ID="uscContactSearchRest" />
                        </telerik:RadPane>
                        <telerik:RadPane runat="server" Width="30px">
                            <telerik:RadButton runat="server" ID="btnNewContact" AutoPostBack="false" ToolTip="Crea nuovo contatto"
                                CausesValidation="false" Height="16px" Width="16px" Style="margin-left: 10px; margin-top: 5px;">
                                <Image ImageUrl="~/App_Themes/DocSuite2008/imgset16/add.png" />
                            </telerik:RadButton>
                        </telerik:RadPane>
                    </telerik:RadSplitter>
                </div>
            </div>
        </telerik:RadPane>
        <telerik:RadPane runat="server" Height="100%" Width="100%" Scrolling="Y">
            <asp:Panel runat="server" Height="100%" Width="100%" ID="pnlContent">
                <telerik:RadPageLayout runat="server" ID="pnlToolbar" Style="display: none; margin-bottom: 2px;">
                    <Rows>
                        <telerik:LayoutRow HtmlTag="Div">
                            <Columns>
                                <telerik:LayoutColumn Span="12" CssClass="t-col-left-padding t-col-right-padding">
                                    <telerik:RadToolBar runat="server" Width="100%" ID="toolbar" Skin="Office2010Blue">
                                        <Items>
                                            <telerik:RadToolBarButton>
                                                <ItemTemplate>
                                                    <i>Salva come:</i>
                                                </ItemTemplate>
                                            </telerik:RadToolBarButton>
                                            <telerik:RadToolBarButton Value="persistanceType">
                                                <ItemTemplate>
                                                    <telerik:RadComboBox runat="server" ID="rcbPersistanceType" Width="80px">
                                                        <Items>
                                                            <telerik:RadComboBoxItem ImageUrl="~/App_Themes/DocSuite2008/imgset16/folder.png" Text="Rubrica" Value="Rubrica" />
                                                            <telerik:RadComboBoxItem ImageUrl="~/App_Themes/DocSuite2008/imgset16/pencil.png" Text="Manuale" Value="Manuale" />
                                                        </Items>
                                                    </telerik:RadComboBox>
                                                </ItemTemplate>
                                            </telerik:RadToolBarButton>
                                            <telerik:RadToolBarButton IsSeparator="true" Style="margin: 0 10px;" />
                                            <telerik:RadToolBarButton>
                                                <ItemTemplate>
                                                    <i>Tipologia:</i>
                                                </ItemTemplate>
                                            </telerik:RadToolBarButton>
                                            <telerik:RadToolBarButton Value="contactType">
                                                <ItemTemplate>
                                                    <telerik:RadComboBox runat="server" ID="rcbContactType" Width="80px">
                                                        <Items>
                                                            <telerik:RadComboBoxItem ImageUrl="~/Comm/images/Interop/Aoo.gif" Text="AOO" Value="A" />
                                                            <telerik:RadComboBoxItem ImageUrl="~/App_Themes/DocSuite2008/imgset16/user.png" Text="Persona" Value="P" />
                                                        </Items>
                                                    </telerik:RadComboBox>
                                                </ItemTemplate>
                                            </telerik:RadToolBarButton>    
                                            <telerik:RadToolBarButton IsSeparator="true" Style="margin: 0 10px;" Value="roleContactSeparator" />
                                            <telerik:RadToolBarButton Value="roleContactLabel">
                                                <ItemTemplate>
                                                    <i>Rubrica:</i>
                                                </ItemTemplate>
                                            </telerik:RadToolBarButton>
                                            <telerik:RadToolBarButton Value="roleContact">
                                                <ItemTemplate>
                                                    <telerik:RadComboBox runat="server" ID="rcbRoleContact" Width="250px" />
                                                </ItemTemplate>
                                            </telerik:RadToolBarButton>
                                        </Items>
                                    </telerik:RadToolBar>
                                </telerik:LayoutColumn>
                            </Columns>
                        </telerik:LayoutRow>
                    </Rows>
                </telerik:RadPageLayout>
                <telerik:RadPageLayout runat="server" ID="pnlMain" Style="display: none;">
                    <telerik:LayoutRow HtmlTag="Div">
                        <Columns>
                            <telerik:LayoutColumn Span="12" CssClass="t-col-left-padding t-col-right-padding">
                                <asp:Panel runat="server" Height="100%" Width="100%">
                                    <telerik:RadPageLayout runat="server" HtmlTag="Div" CssClass="dsw-panel">
                                        <Rows>
                                            <telerik:LayoutRow runat="server" HtmlTag="Div" CssClass="dsw-panel-content" ID="rowTreeContact">
                                                <Columns>
                                                    <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                                    </telerik:LayoutColumn>
                                                    <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding">
                                                        <div id="treeContact"></div>
                                                        <hr />
                                                    </telerik:LayoutColumn>
                                                </Columns>
                                            </telerik:LayoutRow>
                                            <telerik:LayoutRow runat="server" HtmlTag="Div" CssClass="dsw-panel-content" ID="rowName">
                                                <Columns>
                                                    <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                                        <label style="vertical-align: middle;"><b>Nome:</b></label>
                                                    </telerik:LayoutColumn>
                                                    <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding">
                                                        <telerik:RadTextBox runat="server" ID="txtName" Width="100%"></telerik:RadTextBox>
                                                    </telerik:LayoutColumn>
                                                </Columns>
                                            </telerik:LayoutRow>
                                            <telerik:LayoutRow runat="server" HtmlTag="Div" CssClass="dsw-panel-content" ID="rowSurname">
                                                <Columns>
                                                    <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                                        <label style="vertical-align: middle;"><b>Cognome:</b></label>
                                                    </telerik:LayoutColumn>
                                                    <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding">
                                                        <telerik:RadTextBox runat="server" ID="txtSurname" Width="100%"></telerik:RadTextBox>
                                                    </telerik:LayoutColumn>
                                                </Columns>
                                            </telerik:LayoutRow>
                                            <telerik:LayoutRow runat="server" HtmlTag="Div" CssClass="dsw-panel-content" ID="rowDescription">
                                                <Columns>
                                                    <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                                        <label style="vertical-align: middle;"><b>Descrizione:</b></label>
                                                    </telerik:LayoutColumn>
                                                    <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding">
                                                        <telerik:RadTextBox runat="server" ID="txtDescription" Width="100%"></telerik:RadTextBox>
                                                    </telerik:LayoutColumn>
                                                </Columns>
                                            </telerik:LayoutRow>
                                            <telerik:LayoutRow runat="server" HtmlTag="Div" CssClass="dsw-panel-content" ID="rowBirthdate">
                                                <Columns>
                                                    <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                                        <label style="vertical-align: middle;"><b>Data di nascita:</b></label>
                                                    </telerik:LayoutColumn>
                                                    <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding">
                                                        <telerik:RadDatePicker runat="server" ID="rdpBirthdate"></telerik:RadDatePicker>
                                                    </telerik:LayoutColumn>
                                                </Columns>
                                            </telerik:LayoutRow>
                                            <telerik:LayoutRow runat="server" HtmlTag="Div" CssClass="dsw-panel-content" ID="rowBirthplace">
                                                <Columns>
                                                    <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                                        <label style="vertical-align: middle;"><b>Luogo di nascita:</b></label>
                                                    </telerik:LayoutColumn>
                                                    <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding">
                                                        <telerik:RadTextBox runat="server" ID="txtBirthplace"></telerik:RadTextBox>
                                                    </telerik:LayoutColumn>
                                                </Columns>
                                            </telerik:LayoutRow>
                                            <telerik:LayoutRow runat="server" HtmlTag="Div" CssClass="dsw-panel-content" ID="rowTitle">
                                                <Columns>
                                                    <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                                        <label style="vertical-align: middle;"><b>Titolo di studio:</b></label>
                                                    </telerik:LayoutColumn>
                                                    <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding">
                                                        <telerik:RadComboBox runat="server" ID="rcbTitle"></telerik:RadComboBox>
                                                    </telerik:LayoutColumn>
                                                </Columns>
                                            </telerik:LayoutRow>
                                            <telerik:LayoutRow runat="server" HtmlTag="Div" CssClass="dsw-panel-content" ID="rowCertifiedMail">
                                                <Columns>
                                                    <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                                        <label style="vertical-align: middle;"><b>Posta certificata:</b></label>
                                                    </telerik:LayoutColumn>
                                                    <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding">
                                                        <telerik:RadTextBox runat="server" ID="txtCertifiedMail" Width="300px"></telerik:RadTextBox>
                                                        <asp:RegularExpressionValidator ID="emailValidator" runat="server" Display="Dynamic"
                                                            ErrorMessage="L'indirizzo inserito non è valido"
                                                            ValidationExpression="^[\w\.\-]+@[a-zA-Z0-9\-]+(\.[a-zA-Z0-9\-]{1,})*(\.[a-zA-Z]{2,3}){1,2}$"
                                                            ControlToValidate="txtCertifiedMail" />
                                                    </telerik:LayoutColumn>
                                                </Columns>
                                            </telerik:LayoutRow>
                                            <telerik:LayoutRow runat="server" HtmlTag="Div" CssClass="dsw-panel-content" ID="rowEmail">
                                                <Columns>
                                                    <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                                        <label style="vertical-align: middle;"><b>Posta elettronica:</b></label>
                                                    </telerik:LayoutColumn>
                                                    <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding">
                                                        <telerik:RadTextBox runat="server" ID="txtEmail" Width="300px"></telerik:RadTextBox>
                                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" Display="Dynamic"
                                                            ErrorMessage="L'indirizzo inserito non è valido"
                                                            ValidationExpression="^[\w\.\-]+@[a-zA-Z0-9\-]+(\.[a-zA-Z0-9\-]{1,})*(\.[a-zA-Z]{2,3}){1,2}$"
                                                            ControlToValidate="txtEmail" />
                                                    </telerik:LayoutColumn>
                                                </Columns>
                                            </telerik:LayoutRow>
                                            <telerik:LayoutRow runat="server" HtmlTag="Div" CssClass="dsw-panel-content" ID="rowCode">
                                                <Columns>
                                                    <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                                        <label style="vertical-align: middle;"><b>Codice ricerca:</b></label>
                                                    </telerik:LayoutColumn>
                                                    <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding">
                                                        <telerik:RadTextBox runat="server" ID="txtCode"></telerik:RadTextBox>
                                                    </telerik:LayoutColumn>
                                                </Columns>
                                            </telerik:LayoutRow>
                                            <telerik:LayoutRow runat="server" HtmlTag="Div" CssClass="dsw-panel-content" ID="rowPiva">
                                                <Columns>
                                                    <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                                        <label style="vertical-align: middle;"><b>Cod. Fisc./P. IVA:</b></label>
                                                    </telerik:LayoutColumn>
                                                    <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding">
                                                        <telerik:RadTextBox runat="server" ID="txtPiva" Width="300px"></telerik:RadTextBox>
                                                    </telerik:LayoutColumn>
                                                </Columns>
                                            </telerik:LayoutRow>
                                            <telerik:LayoutRow runat="server" HtmlTag="Div" CssClass="dsw-panel-content" ID="rowAddressType">
                                                <Columns>
                                                    <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                                        <label style="vertical-align: middle;"><b>Tipo:</b></label>
                                                    </telerik:LayoutColumn>
                                                    <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding">
                                                        <telerik:RadComboBox runat="server" ID="rcbAddressType"></telerik:RadComboBox>
                                                    </telerik:LayoutColumn>
                                                </Columns>
                                            </telerik:LayoutRow>
                                            <telerik:LayoutRow runat="server" HtmlTag="Div" CssClass="dsw-panel-content" ID="rowAddress">
                                                <Columns>
                                                    <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                                        <label style="vertical-align: middle;"><b>Indirizzo:</b></label>
                                                    </telerik:LayoutColumn>
                                                    <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding">
                                                        <telerik:RadTextBox runat="server" ID="txtAddress" Width="100%"></telerik:RadTextBox>
                                                    </telerik:LayoutColumn>
                                                </Columns>
                                            </telerik:LayoutRow>
                                            <telerik:LayoutRow runat="server" HtmlTag="Div" CssClass="dsw-panel-content" ID="rowCivicNumber">
                                                <Columns>
                                                    <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                                        <label style="vertical-align: middle;"><b>Numero:</b></label>
                                                    </telerik:LayoutColumn>
                                                    <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding">
                                                        <telerik:RadTextBox runat="server" ID="txtCivicNumber"></telerik:RadTextBox>
                                                    </telerik:LayoutColumn>
                                                </Columns>
                                            </telerik:LayoutRow>
                                            <telerik:LayoutRow runat="server" HtmlTag="Div" CssClass="dsw-panel-content" ID="rowZipCode">
                                                <Columns>
                                                    <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                                        <label style="vertical-align: middle;"><b>CAP:</b></label>
                                                    </telerik:LayoutColumn>
                                                    <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding">
                                                        <telerik:RadTextBox runat="server" ID="txtZipCode"></telerik:RadTextBox>
                                                    </telerik:LayoutColumn>
                                                </Columns>
                                            </telerik:LayoutRow>
                                            <telerik:LayoutRow runat="server" HtmlTag="Div" CssClass="dsw-panel-content" ID="rowCity">
                                                <Columns>
                                                    <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                                        <label style="vertical-align: middle;"><b>Città:</b></label>
                                                    </telerik:LayoutColumn>
                                                    <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding">
                                                        <telerik:RadTextBox runat="server" ID="txtCity" Width="100%"></telerik:RadTextBox>
                                                    </telerik:LayoutColumn>
                                                </Columns>
                                            </telerik:LayoutRow>
                                            <telerik:LayoutRow runat="server" HtmlTag="Div" CssClass="dsw-panel-content" ID="rowCityCode">
                                                <Columns>
                                                    <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                                        <label style="vertical-align: middle;"><b>Provincia:</b></label>
                                                    </telerik:LayoutColumn>
                                                    <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding">
                                                        <telerik:RadTextBox runat="server" ID="txtCityCode"></telerik:RadTextBox>
                                                    </telerik:LayoutColumn>
                                                </Columns>
                                            </telerik:LayoutRow>
                                            <telerik:LayoutRow runat="server" HtmlTag="Div" CssClass="dsw-panel-content" ID="rowNationality">
                                                <Columns>
                                                    <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                                        <label style="vertical-align: middle;"><b>Nazionalità:</b></label>
                                                    </telerik:LayoutColumn>
                                                    <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding">
                                                        <telerik:RadTextBox runat="server" ID="txtNationality"></telerik:RadTextBox>
                                                    </telerik:LayoutColumn>
                                                </Columns>
                                            </telerik:LayoutRow>
                                            <telerik:LayoutRow runat="server" HtmlTag="Div" CssClass="dsw-panel-content" ID="rowLanguage">
                                                <Columns>
                                                    <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                                        <label style="vertical-align: middle;"><b>Lingua:</b></label>
                                                    </telerik:LayoutColumn>
                                                    <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding">
                                                        <telerik:RadComboBox runat="server" ID="rcbLanguage">
                                                            <Items>
                                                                <telerik:RadComboBoxItem Text="" Value="" />
                                                                <telerik:RadComboBoxItem Text="Italiano" Value="0" />
                                                                <telerik:RadComboBoxItem Text="Inglese" Value="1" />
                                                                <telerik:RadComboBoxItem Text="Tedesco" Value="2" />
                                                            </Items>
                                                        </telerik:RadComboBox>
                                                    </telerik:LayoutColumn>
                                                </Columns>
                                            </telerik:LayoutRow>
                                            <telerik:LayoutRow runat="server" HtmlTag="Div" CssClass="dsw-panel-content" ID="rowTelephoneNumber">
                                                <Columns>
                                                    <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                                        <label style="vertical-align: middle;"><b>Telefono:</b></label>
                                                    </telerik:LayoutColumn>
                                                    <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding">
                                                        <telerik:RadTextBox runat="server" ID="txtTelephoneNumber"></telerik:RadTextBox>
                                                    </telerik:LayoutColumn>
                                                </Columns>
                                            </telerik:LayoutRow>
                                            <telerik:LayoutRow runat="server" HtmlTag="Div" CssClass="dsw-panel-content" ID="rowFAX">
                                                <Columns>
                                                    <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                                        <label style="vertical-align: middle;"><b>FAX:</b></label>
                                                    </telerik:LayoutColumn>
                                                    <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding">
                                                        <telerik:RadTextBox runat="server" ID="txtFAX"></telerik:RadTextBox>
                                                    </telerik:LayoutColumn>
                                                </Columns>
                                            </telerik:LayoutRow>
                                            <telerik:LayoutRow runat="server" HtmlTag="Div" CssClass="dsw-panel-content" ID="rowNote">
                                                <Columns>
                                                    <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                                        <label style="vertical-align: middle;"><b>Note:</b></label>
                                                    </telerik:LayoutColumn>
                                                    <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding">
                                                        <telerik:RadTextBox runat="server" ID="txtNote" Width="100%"></telerik:RadTextBox>
                                                    </telerik:LayoutColumn>
                                                </Columns>
                                            </telerik:LayoutRow>
                                            <telerik:LayoutRow runat="server" HtmlTag="Div" CssClass="dsw-panel-content">
                                                <Columns>
                                                    <telerik:LayoutColumn Span="3" CssClass="dsw-text-right">
                                                    </telerik:LayoutColumn>
                                                    <telerik:LayoutColumn Span="9" CssClass="t-col-left-padding">
                                                        <hr />
                                                        <telerik:RadButton runat="server" ID="btnCollapseInformations" Text="Altri campi" CausesValidation="false" AutoPostBack="false"></telerik:RadButton>
                                                    </telerik:LayoutColumn>
                                                </Columns>
                                            </telerik:LayoutRow>
                                        </Rows>
                                    </telerik:RadPageLayout>
                                </asp:Panel>
                            </telerik:LayoutColumn>
                        </Columns>
                    </telerik:LayoutRow>
                </telerik:RadPageLayout>
            </asp:Panel>
        </telerik:RadPane>
    </telerik:RadSplitter>
</div>


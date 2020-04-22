<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscErrorNotification.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscErrorNotification" %>

<telerik:RadScriptBlock runat="server" ID="RadScriptBlock1">
    <script type="text/javascript">
        var uscErrorNotification;
        require(["UserControl/uscErrorNotification"], function (UscErrorNotification) {
            $(function () {
                uscErrorNotification = new UscErrorNotification();
                uscErrorNotification.radNotificationId = "<%= radNotification.ClientID %>";
                uscErrorNotification.radNotificationWarningId = "<%= radNotificationWarning.ClientID %>";
                uscErrorNotification.radNotificationValidationId = "<%= radNotificationValidation.ClientID %>";
                uscErrorNotification.radRotatorId = "<%= radRotator.ClientID %>";
                uscErrorNotification.messageRotatorId = "<%= messageRotator.ClientID %>";
                uscErrorNotification.exceptionMessageId = "<%= exceptionMessage.ClientID %>";
                uscErrorNotification.pageId = "<%= pageContent.ClientID %>";
                uscErrorNotification.initialize();
            });
        });

    </script>
</telerik:RadScriptBlock>

<telerik:RadPageLayout runat="server" ID="pageContent" HtmlTag="Div" Width="100%">
    <Rows>
        <telerik:LayoutRow>
            <Content>
                <telerik:RadNotification ID="radNotification" runat="server"
                    VisibleOnPageLoad="false" LoadContentOn="PageLoad" Width="400" Height="200" Animation="FlyIn"
                    EnableRoundedCorners="true" EnableShadow="true" ContentIcon="delete" TitleIcon="delete" Title="Errore pagina" AutoCloseDelay="0" Position="Center" />

                 <telerik:RadNotification ID="radNotificationWarning" runat="server"
                    VisibleOnPageLoad="false" LoadContentOn="PageLoad" Width="400" Height="200" Animation="FlyIn"
                    EnableRoundedCorners="true" EnableShadow="true" ContentIcon="warning" TitleIcon="warning" Title="Avviso pagina" AutoCloseDelay="0" Position="Center" />

                <telerik:RadNotification ID="radNotificationValidation" runat="server"
                    VisibleOnPageLoad="false" LoadContentOn="PageLoad" Width="400" Height="200" Animation="FlyIn"
                    EnableRoundedCorners="true" EnableShadow="true" TitleIcon="warning" Title="Validazione pagina" AutoCloseDelay="0" Position="Center">
                    <ContentTemplate>
                        <div id="messageRotator" runat="server">
                            <p id="exceptionMessage" runat="server" class="messageExcpetion"></p>
                            <telerik:RadRotator RenderMode="Lightweight" runat="server" ID="radRotator" CssClass="slideshow"
                                RotatorType="Buttons" Width="390" ItemWidth="337" Height="60" ItemHeight="60" ScrollDirection="Right,Left">
                                <ClientTemplate>
                                    <div style="padding:5px">
                                        <label>#= message #</label> 
                                    </div>
                                </ClientTemplate>
                            </telerik:RadRotator>
                        </div>
                    </ContentTemplate>
                </telerik:RadNotification>
            </Content>
        </telerik:LayoutRow>
    </Rows>
</telerik:RadPageLayout>

<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscGoSignFlowRest.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscGoSignFlowRest" %>

<telerik:RadScriptBlock runat="server" EnableViewState="false">
    <script type="text/javascript">        
        var uscGoSignFlowRest;
        require(["UserControl/uscGoSignFlowRest"], function (UscGoSignFlowRest) {
            $(function () {
                uscGoSignFlowRest = new UscGoSignFlowRest("<%= Me.ClientID %>", "<%= InfocertProxySignUrl %>");                
                uscGoSignFlowRest.initialize();
            });
        });

        function <%= Me.ClientID %>_startGoSignFlow(sessionId) {
            return uscGoSignFlowRest.startGoSignFlow(sessionId);
        }
    </script>
</telerik:RadScriptBlock>
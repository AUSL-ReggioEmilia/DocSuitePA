<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscMetadataRepositorySummary.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscMetadataRepositorySummary" %>
<%@ Register Src="~/UserControl/uscErrorNotification.ascx" TagName="uscErrorNotification" TagPrefix="usc" %>
<link href="../UDSDesigner/Content/bootstrap.css" rel="stylesheet" />
<link href="../UDSDesigner/Content/bootstrap-datetimepicker.min.css" rel="stylesheet" />
<link href="../UDSDesigner/Content/themes/proton/jstree-proton.min.css" rel="stylesheet" />
<link href="../UDSDesigner/Content/bootstrap-theme-custom.css" rel="stylesheet" />
<link href="../UDSDesigner/Content/jsoneditor.min.css" rel="stylesheet" type="text/css" />
<link href="../UDSDesigner/Content/css/font-awesome.min.css" rel="stylesheet" />
<link href="../UDSDesigner/Content/nprogress.css" rel="stylesheet" />
<link href="../Content/site.css" rel="stylesheet" />
<telerik:RadScriptBlock runat="server" EnableViewState="false">
    <script type="text/javascript">        var uscMetadataRepositorySummary;        require(["UserControl/uscMetadataRepositorySummary"], function (UscMetadataRepositorySummary) {            $(function () {                uscMetadataRepositorySummary = new UscMetadataRepositorySummary(tenantModelConfiguration.serviceConfiguration);                uscMetadataRepositorySummary.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID%>";                uscMetadataRepositorySummary.ajaxLoadingPanelId = "<%= BasePage.MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>";                uscMetadataRepositorySummary.componentTitleId = "<%= componentTitle.ClientID%>"                uscMetadataRepositorySummary.componentTextId = "<%= componentText.ClientID%>";                uscMetadataRepositorySummary.componentDateId = "<%= componentDate.ClientID%>";                uscMetadataRepositorySummary.componentNumberId = "<%= componentNumber.ClientID%>";                uscMetadataRepositorySummary.componentCheckBoxId = "<%= componentCheckbox.ClientID%>";                uscMetadataRepositorySummary.componentCommentId = "<%= componentComment.ClientID%>"                uscMetadataRepositorySummary.componentEnumId = "<%= componentEnum.ClientID%>"                uscMetadataRepositorySummary.pageContentId = "<%= pageContent.ClientID%>";                uscMetadataRepositorySummary.ajaxManagerId = "<%= BasePage.MasterDocSuite.DefaultWindowManager.ClientID %>";                uscMetadataRepositorySummary.setiIntegrationEnabledId = <%=ProtocolEnv.SETIIntegrationEnabled.ToString().ToLower()%>;                uscMetadataRepositorySummary.initialize();            });        });    </script>
    <style>
        .element {
            margin-left: 0px;
        }
    </style>
</telerik:RadScriptBlock>
<div style="margin-right: 5px;" id="pageContent" runat="server">
    <table class="maintable" style="margin-left: 0px!important;">
        <tr>
            <td>
                <div class="menuContent">
                    <div class="menuContent" id="menuContent">
                    </div>
                </div>
            </td>
        </tr>
    </table>
</div>
<div id="container">
    <div id="componentTitle" data-type="Title" runat="server" style="padding: 6px; margin-left: 0px;" class="element-Title">
        <div class="controls">
            <label class="control-label"></label>
            <span id="setiFieldId" style="margin-left: 5px;"></span>
        </div>
    </div>
    <div id="componentText" data-type="Text" style="padding: 6px;" runat="server" class="element">
        <div><span aria-hidden="true"><big><b>Ab</b></big></span></div>
        <div class="controls">
            <table>
                <tr>
                    <td>
                        <label class="control-label">Nome del campo: </label>
                        <label class="control-label" style="margin-left: 65px;" id="labelNameText"></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="control-label">Codice interno: </label>
                        <label class="control-label" style="margin-left: 79px;" id="labelKeynameText"></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="control-label" id="labelRequiredText" style="margin-top: 5px; align-items: center">Campo obbligatorio: </label>
                        <input type="checkbox" disabled="disabled" aria-readonly="true" name="required" id="requiredText" class="checkBox" style="margin-left: 50px;" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="control-label" id="labelVisibleInSearchText" style="margin-top: 5px; align-items: center">Visibile in ricerca: </label>
                        <input type="checkbox" disabled="disabled" aria-readonly="true" name="showInResults" id="visibleInSearchText" class="checkBox" style="margin-left: 66px;" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="control-label" id="labelHiddenFieldText" style="margin-top: 5px; align-items: center; display: none;">Campo nascosto: </label>
                        <input type="checkbox" disabled="disabled" aria-readonly="true" name="hiddenField" id="hiddenFieldText" class="checkBox" style="margin-left: 72px; display: none;" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="control-label" id="labelReadOnlyText" style="margin-top: 5px; align-items: center;">Sola lettura: </label>
                        <input type="checkbox" disabled="disabled" aria-readonly="true" name="readOnly" id="readOnlyText" class="checkBox" style="margin-left: 100px;" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="componentComment" data-type="Comment" style="padding: 6px;" runat="server" class="element">
        <div><span class="fa fa-comments fa-lg"></span></div>
        <div class="controls">
            <table>
                <tr>
                    <td>
                        <label class="control-label">Nome del campo: </label>
                        <label class="control-label" style="margin-left: 65px;" id="labelNameComment"></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="control-label">Codice interno: </label>
                        <label class="control-label" style="margin-left: 79px;" id="labelKeynameComment"></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="control-label" id="labelRequiredComment">Campo obbligatorio: </label>
                        <input type="checkbox" disabled="disabled" aria-readonly="true" name="required" id="requiredComment" class="checkBox" style="margin-left: 50px;" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="control-label" id="labelVisibleInSearchComment" style="margin-top: 5px; align-items: center">Visibile in ricerca: </label>
                        <input type="checkbox" disabled="disabled" aria-readonly="true" name="showInResults" id="visibleInSearchComment" class="checkBox" style="margin-left: 66px;" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="control-label" id="labelHiddenFieldComment" style="margin-top: 5px; align-items: center; display: none;">Campo nascosto: </label>
                        <input type="checkbox" disabled="disabled" aria-readonly="true" name="hiddenField" id="hiddenFieldComment" class="checkBox" style="margin-left: 72px; display: none;" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="control-label" id="labelReadOnlyComment" style="margin-top: 5px; align-items: center;">Sola lettura: </label>
                        <input type="checkbox" disabled="disabled" aria-readonly="true" name="readOnly" id="readOnlyComment" class="checkBox" style="margin-left: 100px;" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="componentDate" data-type="Date" style="padding: 5px;" runat="server" class="element">
        <div><span class="fa fa-calendar fa-lg" aria-hidden="true"></span></div>
        <div class="controls">
            <table>
                <tr>
                    <td>
                        <label class="control-label">Nome del campo: </label>
                        <label class="control-label" style="margin-left: 65px;" id="labelNameDate"></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="control-label">Codice interno: </label>
                        <label class="control-label" style="margin-left: 79px;" id="labelKeynameDate"></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="control-label" id="labelRequiredDate">Campo obbligatorio: </label>
                        <input type="checkbox" disabled="disabled" aria-readonly="true" name="required" id="requiredDate" class="checkBox" style="margin-left: 50px;" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="control-label" id="labelVisibleInSearchDate" style="margin-top: 5px; align-items: center">Visibile in ricerca: </label>
                        <input type="checkbox" disabled="disabled" aria-readonly="true" name="showInResults" id="visibleInSearchDate" class="checkBox" style="margin-left: 66px;" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="control-label" id="labelHiddenFieldDate" style="margin-top: 5px; align-items: center; display: none;">Campo nascosto: </label>
                        <input type="checkbox" disabled="disabled" aria-readonly="true" name="hiddenField" id="hiddenFieldDate" class="checkBox" style="margin-left: 72px; display: none;" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="control-label" id="labelReadOnlyDate" style="margin-top: 5px; align-items: center;">Sola lettura: </label>
                        <input type="checkbox" disabled="disabled" aria-readonly="true" name="readOnly" id="readOnlyDate" class="checkBox" style="margin-left: 100px;" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="componentNumber" data-type="Number" style="padding: 5px;" runat="server" class="element">
        <div><span aria-hidden="true"><big><b>1.</b></big></span></div>
        <div class="controls">
            <table>
                <tr>
                    <td>
                        <label class="control-label">Nome del campo: </label>
                        <label class="control-label" style="margin-left: 65px;" id="labelNameNumber"></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="control-label">Codice interno: </label>
                        <label class="control-label" style="margin-left: 79px;" id="labelKeynameNumber"></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="control-label" id="labelRequiredNumber">Campo obbligatorio: </label>
                        <input type="checkbox" disabled="disabled" aria-readonly="true" name="required" id="requiredNumber" class="checkBox" style="margin-left: 50px;" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="control-label" id="labelVisibleInSearchNumber" style="margin-top: 5px; align-items: center">Visibile in ricerca: </label>
                        <input type="checkbox" disabled="disabled" aria-readonly="true" name="showInResults" id="visibleInSearchNumber" class="checkBox" style="margin-left: 66px;" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="control-label" id="labelHiddenFieldNumber" style="margin-top: 5px; align-items: center; display: none;">Campo nascosto: </label>
                        <input type="checkbox" disabled="disabled" aria-readonly="true" name="hiddenField" id="hiddenFieldNumber" class="checkBox" style="margin-left: 72px; display: none;" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="control-label" id="labelReadOnlyNumber" style="margin-top: 5px; align-items: center;">Sola lettura: </label>
                        <input type="checkbox" disabled="disabled" aria-readonly="true" name="readOnly" id="readOnlyNumber" class="checkBox" style="margin-left: 100px;" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="control-label" id="labelFormatNumber" style="margin-top: 5px; align-items: center;">Formato: </label>
                    </td>
                    <td>
                        <select id="formatType" disabled="disabled" aria-readonly="true" name="format" style="margin-left: -28px;">
                            <option value="0" selected="selected">Intero</option>
                            <option value="0.00">Numero con due decimali</option>
                            <option value="0.0000">Numero con quattro decimali</option>
                        </select>
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="componentCheckbox" data-type="CheckBox" style="padding: 5px;" runat="server" class="element">
        <div><span class="fa fa-check-square-o fa-lg" aria-hidden="true"></span></div>
        <div class="controls">
            <table>
                <tr>
                    <td>
                        <label class="control-label">Nome del campo: </label>
                        <label class="control-label" style="margin-left: 65px;" id="labelNameCheckBox"></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="control-label">Codice interno: </label>
                        <label class="control-label" style="margin-left: 79px;" id="labelKeynameCheckBox"></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="control-label" id="labelRequiredCheckBox">Campo obbligatorio: </label>
                        <input type="checkbox" disabled="disabled" aria-readonly="true" name="required" id="requiredCheckbox" class="checkBox" style="margin-left: 50px;" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="control-label" id="labelVisibleInSearchCheckbox" style="margin-top: 5px; align-items: center">Visibile in ricerca: </label>
                        <input type="checkbox" disabled="disabled" aria-readonly="true" name="showInResults" id="visibleInSearchCheckBox" class="checkBox" style="margin-left: 66px;" />
                    </td>
                </tr> 
                <tr>
                    <td>
                        <label class="control-label" id="labelHiddenFieldCheckbox" style="margin-top: 5px; align-items: center; display: none;">Campo nascosto: </label>
                        <input type="checkbox" disabled="disabled" aria-readonly="true" name="hiddenField" id="hiddenFieldCheckbox" class="checkBox" style="margin-left: 72px; display: none;" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="control-label" id="labelReadOnlyCheckbox" style="margin-top: 5px; align-items: center;">Sola lettura: </label>
                        <input type="checkbox" disabled="disabled" aria-readonly="true" name="readOnly" id="readOnlyCheckbox" class="checkBox" style="margin-left: 100px;" />
                    </td>
                </tr>
            </table>
        </div>
    </div>
    <div id="componentEnum" data-type="Enum" style="padding: 5px;" runat="server" class="element">
        <div><span class="fa fa-list fa-lg" aria-hidden="true"></span></div>
        <div class="controls">
            <table>
                <tr>
                    <td>
                        <label class="control-label">Nome del campo: </label>
                        <label class="control-label" style="margin-left: 65px;" id="labelNameEnum"></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="control-label">Codice interno: </label>
                        <label class="control-label" style="margin-left: 79px;" id="labelKeynameEnum"></label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="control-label" id="labelRequiredEnum">Campo obbligatorio: </label>
                        <input type="checkbox" disabled="disabled" aria-readonly="true" name="required" id="requiredEnum" class="checkBox" style="margin-left: 50px;" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="control-label" id="labelVisibleInSearchEnum" style="margin-top: 5px; align-items: center">Visibile in ricerca: </label>
                        <input type="checkbox" disabled="disabled" aria-readonly="true" name="showInResults" id="visibleInSearchEnum" class="checkBox" style="margin-left: 66px;" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="control-label" id="labelHiddenFieldEnum" style="margin-top: 5px; align-items: center; display: none;">Campo nascosto: </label>
                        <input type="checkbox" disabled="disabled" aria-readonly="true" name="hiddenField" id="hiddenFieldEnum" class="checkBox" style="margin-left: 72px; display: none;" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="control-label" id="labelReadOnlyEnum" style="margin-top: 5px; align-items: center;">Sola lettura: </label>
                        <input type="checkbox" disabled="disabled" aria-readonly="true" name="readOnly" id="readOnlyEnum" class="checkBox" style="margin-left: 100px;" />
                    </td>
                </tr>
            </table>
        </div>
        <div class="controls" id="enumValues">
            <label class="control-label">Valori:</label>
            <ul id="labelEnum" class="ul" style="margin-left: 180px; margin-top: -20px">
            </ul>
        </div>
    </div>
</div>
<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>

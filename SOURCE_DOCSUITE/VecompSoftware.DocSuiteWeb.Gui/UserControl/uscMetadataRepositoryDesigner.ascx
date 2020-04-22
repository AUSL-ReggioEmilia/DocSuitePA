<%@ Control Language="vb" AutoEventWireup="false" CodeBehind="uscMetadataRepositoryDesigner.ascx.vb" Inherits="VecompSoftware.DocSuiteWeb.Gui.uscMetadataRepositoryDesigner" %>

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

    <script type="text/javascript">
        var uscMetadataRepositoryDesigner;
        require(["UserControl/uscMetadataRepositoryDesigner"], function (UscMetadaRepositoryDesigner) {
            $(function () {
                uscMetadataRepositoryDesigner = new UscMetadaRepositoryDesigner(tenantModelConfiguration.serviceConfiguration);
                uscMetadataRepositoryDesigner.uscNotificationId = "<%= uscNotification.PageContentDiv.ClientID%>";
                uscMetadataRepositoryDesigner.ajaxLoadingPanelId = "<%= BasePage.MasterDocSuite.AjaxDefaultLoadingPanel.ClientID%>";
                uscMetadataRepositoryDesigner.componentTitleId = "<%= componentTitle.ClientID%>"
                uscMetadataRepositoryDesigner.componentTextId = "<%= componentText.ClientID%>";
                uscMetadataRepositoryDesigner.componentDateId = "<%= componentDate.ClientID%>";
                uscMetadataRepositoryDesigner.componentNumberId = "<%= componentNumber.ClientID%>";
                uscMetadataRepositoryDesigner.componentCheckBoxId = "<%= componentCheckbox.ClientID%>";
                uscMetadataRepositoryDesigner.componentCommentId = "<%= componentComment.ClientID%>"
                uscMetadataRepositoryDesigner.componentEnumId = "<%= componentEnum.ClientID%>"
                uscMetadataRepositoryDesigner.pageContentId = "<%= pageContent.ClientID%>";
                uscMetadataRepositoryDesigner.ajaxManagerId = "<%= BasePage.MasterDocSuite.DefaultWindowManager.ClientID %>";
                uscMetadataRepositoryDesigner.initialize();
            });
        });

        function allowDrop(ev) {
            ev.preventDefault();
        }

        function drag(ev) {
            uscMetadataRepositoryDesigner.drag(ev);
        }

        function drop(ev) {
            uscMetadataRepositoryDesigner.drop(ev);
        }

        function remove(ev) {
            uscMetadataRepositoryDesigner.removeItem(ev)
        }

        function click(ev) {
            uscMetadataRepositoryDesigner.click(ev);
        }

        function addValue(ev) {
            uscMetadataRepositoryDesigner.addValue(ev);
        }

        $(function () {
            $("#Text").click(function (e) {
                click(e);
            })
        })

        $(function () {
            $("#Comment").click(function (e) {
                click(e);
            })
        })

        $(function () {
            $("#Number").click(function (e) {
                click(e);
            })
        })

        $(function () {
            $("#Date").click(function (e) {
                click(e);
            })
        })

        $(function () {
            $("#CheckBox").click(function (e) {
                click(e);
            })
        })

        $(function () {
            $("#Enumerator").click(function (e) {
                click(e);
            })
        })

    </script>
</telerik:RadScriptBlock>
<div style="margin-right: 5px;" id="pageContent" runat="server">
    <table class="maintable">
        <tr>
            <td style="width: 200px; white-space: nowrap; position: fixed">
                <div>
                    <div id="Text" class="component" data-type="Text" draggable="true" ondragstart="drag(event)"><span aria-hidden="true"><big><b>Ab</b></big></span>&nbsp; Testo</div>
                    <div id="Comment" class="component" data-type="Text" draggable="true" ondragstart="drag(event)"><span class="fa fa-comments fa-lg"></span>&nbsp; Commento</div>
                    <div id="Number" class="component" data-type="Number" draggable="true" ondragstart="drag(event)"><span aria-hidden="true"><big><b>1.</b></big></span>&nbsp; Numero</div>
                    <div id="Date" class="component" data-type="Date" draggable="true" ondragstart="drag(event)"><span class="fa fa-calendar fa-lg" aria-hidden="true"></span>&nbsp; Data</div>
                    <div id="CheckBox" class="component" data-type="Checkbox" draggable="true" ondragstart="drag(event)"><span class="fa fa-check-square-o fa-lg" aria-hidden="true"></span>&nbsp; Checkbox</div>
                    <div id="Enumerator" class="component" data-type="Enum" draggable="true" ondragstart="drag(event)"><span class="fa fa-list fa-lg" aria-hidden="true"></span>&nbsp; Scelta Multipla</div>
                </div>
            </td>
            <td>
                <div class="menuContent" ondrop="drop(event)" ondragover="allowDrop(event)">

                    <div class="menuContent" id="menuContent">
                    </div>

                </div>
            </td>
        </tr>
    </table>
</div>

<div id="container">

    <div id="componentTitle" data-type="Title" runat="server" style="padding: 6px;" class="element-Title">
        <label class="control-label">Titolo</label>
        <div class="controls">
            <input type="text" name="" id="Name_input" class="form-control" style="width: 320px;" placeholder="Scegliere il titolo del deposito di metadati">
        </div>
    </div>

    <div id="componentText" data-type="Text" style="padding: 6px;" runat="server" class="element">
        <div><span aria-hidden="true"><big><b>Ab</b></big></span></div>
        <div class="close" onclick="remove(event)" id="closeText">&times;</div>
        <div class="controls">
            <table>
                <tr>
                    <td>
                        <label class="control-label" id="labelNameText">Nome del campo: </label>
                    </td>
                    <td>
                        <input type="text" name="" id="text_input" class="form-control" style="width: 320px; margin-left: -15px;" placeholder="Scegliere il nome del campo" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="control-label" id="labelRequiredText" style="margin-top: 5px; align-items: center">Campo obbligatorio: </label>
                        <input type="checkbox" name="required" id="requiredText" class="checkBox" style="margin-left: 50px;" />
                    </td>
                </tr>
            </table>
        </div>
    </div>

    <div id="componentComment" data-type="Comment" style="padding: 6px;" runat="server" class="element">
        <div><span class="fa fa-comments fa-lg"></span></div>
        <div class="close" onclick="remove(event)" id="closeComment">&times;</div>
        <div class="controls">
            <table>
                <tr>
                    <td>
                        <label class="control-label" id="labelNameComment">Nome del campo: </label>
                    </td>
                    <td>
                        <input type="text" name="" id="comment_input" class="form-control" style="width: 320px; margin-left: -15px;" placeholder="Scegliere il nome del campo" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="control-label" id="labelRequiredComment">Campo obbligatorio: </label>
                        <input type="checkbox" name="required" id="requiredComment" class="checkBox" style="margin-left: 50px;" />
                    </td>
                </tr>
            </table>
        </div>
    </div>

    <div id="componentDate" data-type="Date" style="padding: 5px;" runat="server" class="element">
        <div><span class="fa fa-calendar fa-lg" aria-hidden="true"></span></div>
        <div class="close" onclick="remove(event)" id="closeDate">&times;</div>
        <div class="controls">
            <table>
                <tr>
                    <td>
                        <label class="control-label" id="labelNameDate">Nome del campo: </label>
                    </td>
                    <td>
                        <input type="text" name="" id="date_input" class="form-control" style="width: 320px; margin-left: -15px;" placeholder="Scegliere il nome del campo" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="control-label" id="labelRequiredDate">Campo obbligatorio: </label>
                        <input type="checkbox" name="required" id="requiredDate" class="checkBox" style="margin-left: 50px;" />
                    </td>
                </tr>
            </table>
        </div>
    </div>

    <div id="componentNumber" data-type="Number" style="padding: 5px;" runat="server" class="element">
        <div><span aria-hidden="true"><big><b>1.</b></big></span></div>
        <div class="close" onclick="remove(event)" id="closeNumber">&times;</div>
        <div class="controls">
            <table>
                <tr>
                    <td>
                        <label class="control-label" id="labelNameNumber">Nome del campo: </label>
                    </td>
                    <td>
                        <input type="text" name="" id="number_input" class="form-control" style="width: 320px; margin-left: -15px;" placeholder="Scegliere il nome del campo" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="control-label" id="labelRequiredNumber">Campo obbligatorio: </label>
                        <input type="checkbox" name="required" id="requiredNumber" class="checkBox" style="margin-left: 50px;" />
                    </td>
                </tr>
            </table>
        </div>
    </div>

    <div id="componentCheckbox" data-type="CheckBox" style="padding: 5px;" runat="server" class="element">
        <div><span class="fa fa-check-square-o fa-lg" aria-hidden="true"></span></div>
        <div class="close" onclick="remove(event)" id="closeCheckBox">&times;</div>
        <div class="controls">
            <table>
                <tr>
                    <td>
                        <label class="control-label" id="labelNameCheckBox">Nome del campo: </label>
                    </td>
                    <td>
                        <input type="text" name="checkbox" id="checkbox" class="form-control" style="width: 320px; margin-left: -15px;" placeholder="Scegliere il nome del campo" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="control-label" id="labelRequiredCheckBox">Campo obbligatorio: </label>
                        <input type="checkbox" name="required" id="requiredCheckbox" class="checkBox" style="margin-left: 50px;" />
                    </td>
                </tr>
            </table>
        </div>
    </div>

    <div id="componentEnum" data-type="Enum" style="padding: 5px;" runat="server" class="element">
        <div><span class="fa fa-list fa-lg" aria-hidden="true"></span></div>
        <div class="close" onclick="remove(event)" id="closeEnum">&times;</div>
        <div class="controls">
            <table>
                <tr>
                    <td>
                        <label class="control-label" id="labelNameEnum">Nome del campo: </label>
                    </td>
                    <td>
                        <input type="text" name="enum" id="enum" class="form-control" style="width: 320px; margin-left: -15px;" placeholder="Scegliere il nome del campo" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <label class="control-label" id="labelRequiredEnum">Campo obbligatorio: </label>
                        <input type="checkbox" name="required" id="requiredEnum" class="checkBox" style="margin-left: 50px;" />
                    </td>
                </tr>
            </table>
        </div>

        <div class="controls" style="margin-top: 8px;" id="enumValues">
            <label style="margin-right: 10px;" class="control-label" for="enum_values">Inserisci un nuovo valore: </label>
            <input type="text" name="checkbox" id="enumName" class="enumInput" style="width: 320px;" placeholder="" />
            <div class="btn btn-info" onclick="addValue(event)"><span class="fa fa-plus fa-lg" aria-hidden="true"></span></div>
            <br />
            <ul id="labelEnum" class="ul" style="margin-left: 200px;">
            </ul>
        </div>
    </div>
</div>

<usc:uscErrorNotification runat="server" ID="uscNotification"></usc:uscErrorNotification>

<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Designer.aspx.vb" EnableTheming="False" StylesheetTheme="" Theme="" Inherits="VecompSoftware.DocSuiteWeb.Gui.Designer" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>Designer degli Archivi</title>
    <link rel="shortcut icon" href="/favicon.ico" type="image/x-icon" />
    <link href="Content/bootstrap.css" rel="stylesheet" />
    <link href="Content/bootstrap-datetimepicker.min.css" rel="stylesheet" />
    <link href="Content/themes/proton/jstree-proton.min.css" rel="stylesheet" />
    <link href="Content/bootstrap-theme-custom.css" rel="stylesheet" />
    <link href="Content/jsoneditor.min.css" rel="stylesheet" type="text/css" />
    <link href="Content/css/font-awesome.min.css" rel="stylesheet" />
    <link href="Content/nprogress.css" rel="stylesheet" />
    <link href="Content/site.css" rel="stylesheet" />

    <style>
        .maintable {
            width: 100%;
        }

            .maintable td {
                vertical-align: top;
                padding: 2px;
            }

        .navbar {
            margin-bottom: 0;
        }

        #components-container {
            display: none;
        }

        .hidden {
            display: none;
        }

        .jsoneditor {
            border: none;
        }

        .component-attribute {
            font-size: 13px;
            margin-left: 5px;
            color: #777 !important;
        }


        .component-toolbar {
            float: right;
        }

        .read-only {
            color: #aaa !important;
        }
    </style>
</head>

<body>

    <div class="navbar navbar-default navbar-fixed-top">
        <div class="container-fluid">
            <!-- Brand and toggle get grouped for better mobile display -->
            <div class="navbar-header">
                <button type="button" class="navbar-toggle collapsed" data-toggle="collapse" data-target="#bs-example-navbar-collapse-1">
                    <span class="sr-only">Toggle navigation</span>
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                </button>
                <a class="navbar-brand" href="#">Designer degli Archivi</a>
            </div>

            <!-- Collect the nav links, forms, and other content for toggling -->
            <div class="collapse navbar-collapse" id="bs-example-navbar-collapse-1">
                <ul class="nav navbar-nav">
                    <li><a id="menuNew" href="#">Nuovo</a></li>
                    <li><a id="menuSave" href="#">Salva Bozza</a></li>
                    <li><a id="menuPublish" href="#">Pubblica Archivio</a></li>
                    <li><a id="menuXml" href="downloadXml.aspx">Xml</a></li>
                </ul>
            </div>
            <!-- /.navbar-collapse -->
        </div>
        <!-- /.container-fluid -->
    </div>

    <!-- Begin page content -->
    <div class="alert alert-danger alert-dismissible collapse" id="alertErrorMessage" role="alert">
        <button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>
        <strong>Errore!</strong> E' avvenuto un errore durante la richiesta: { message }
    </div>
    <div class="alert alert-success alert-dismissible collapse" id="alertSuccessMessage" role="alert">
        <button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>
        <strong>Successo!</strong> { message }
    </div>
    <div style="margin-right: 5px;">
        <table class="maintable">

            <tr>
                <td style="width: 200px; white-space: nowrap;">
                    <div>
                        <div class="component hide" data-type="Title"><span class="fa fa-square fa-lg" aria-hidden="true"></span>&nbsp; Titolo</div>
                        <div class="component hide" data-type="Header"><span class="fa fa-tasks fa-lg" aria-hidden="true"></span>&nbsp; Intestazione</div>
                        <div class="component" data-type="Text"><span aria-hidden="true"><big><b>Ab</b></big></span>&nbsp; Testo</div>
                        <div class="component" data-type="Number"><span aria-hidden="true"><big><b>1.</b></big></span>&nbsp; Numero</div>
                        <div class="component" data-type="Date"><span class="fa fa-calendar fa-lg" aria-hidden="true"></span>&nbsp; Data</div>
                        <div class="component" data-type="Checkbox"><span class="fa fa-check-square-o fa-lg" aria-hidden="true"></span>&nbsp; Checkbox</div>
                        <div class="component" data-type="Document"><span class="fa fa-file-text-o fa-lg" aria-hidden="true"></span>&nbsp; Documenti</div>
                        <div class="component" data-type="Contact"><span class="fa fa-user-plus fa-lg" aria-hidden="true"></span>&nbsp; Contatti</div>
                        <div class="component" data-type="Authorization"><span class="fa fa-arrow-circle-o-right fa-lg" aria-hidden="true"></span>&nbsp; Autorizzazione</div>
                        <div class="component" data-type="Enum"><span class="fa fa-list fa-lg" aria-hidden="true"></span>&nbsp; Scelta Multipla</div>
                        <div class="component" data-type="Lookup"><span class="fa fa-search fa-lg" aria-hidden="true"></span>&nbsp; Lookup</div>
                        <div class="component" data-type="Status"><span class="fa fa-signal fa-lg" aria-hidden="true"></span>&nbsp; Stato</div>
                    </div>
                </td>

                <td>
                    <div class="tabbable">
                        <ul class="nav nav-tabs">
                            <li class="active"><a href="#editor-tab" data-toggle="tab">Form</a></li>
                            <li><a href="#source-tab" data-toggle="tab">Sorgente</a></li>
                        </ul>

                        <label for="enabledProtocol">
                            <input type="hidden" name="dematerialisationEnabled" value="false" id="dematerialisationEnabled" runat="server" />
                            <input type="hidden" name="workflowManager" value="false" id="workflowManager" runat="server" />
                        </label>

                        <div class="tab-content">
                            <div class="tab-pane active" id="editor-tab">
                                <div id="status" class="alert alert-dismissible fade in" rv-show="status.error" rv-class-alert-danger="status.error" role="alert" style="margin-bottom: 0px !important;">
                                    <div>
                                        <span>Per poter utilizzare correttamente il modulo Archivi è necessario definire un insieme minimo di elementi e di proprietà base, tra cui :</span>
                                        <ul>
                                            <li>
                                                <span>Proprietà base 'Titolo' dell'archivio.</span>
                                            </li>
                                            <li>
                                                <span>Proprietà base 'Alias' dell'archivio.</span>
                                            </li>
                                            <li>
                                                <span>Documento Principale.</span>
                                            </li>
                                        </ul>
                                    </div>
                                    <span>Nell'attuale configurazione predisposta per poter pubblicare l'archivio è necessario verificare i seguenti punti:</span>
                                    <ul>
                                        <li rv-each-msg="status.messages">
                                            <span>{ msg }</span>
                                        </li>
                                    </ul>
                                </div>

                                <form class="form-horizontal">
                                    <div id="content"></div>
                                    <div >
                                        <table style="width: 100%; height: 100%; word-wrap:break-word;" id="tableLayoutColumns">
                                        </table>
                                    </div>
                                </form>
                            </div>

                            <div class="tab-pane" id="source-tab">
                                <div id="source"></div>
                            </div>

                        </div>
                    </div>
                </td>
            </tr>
        </table>
    </div>

    <!-- ActiveDate Modal -->
    <div class="modal fade" id="activeDate_modal" tabindex="-1" role="dialog" aria-labelledby="activeDate_modal_label" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <form class="form-horizontal">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                        <h4 class="modal-title" id="activeDate_modal_label">Proprietà controllo</h4>
                    </div>
                    <div class="modal-body" style="padding: 15px; background-color: #eee;">
                        <div class="form-group">
                            <label class="control-label col-sm-4">Data di attivazione</label>
                            <div class="input-group date col-sm-7" id="activeDateTime">
                                <input type="text" id="txtActiveDate" class="form-control" />
                                <span class="input-group-addon">
                                    <span class="glyphicon glyphicon-calendar"></span>
                                </span>
                            </div>
                            <label class="control-label col-sm-4" id="lblDuplicate">Crea una versione nuova?</label>
                            <div class="input-group date col-sm-7" id="activeDuplicate">
                                <input type="checkbox" id="chkDuplicate" />
                                <label for="chkDuplicate">&nbsp;Si</label>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <input type="submit" class="btn btn-primary" id="btnPublish" value="Pubblica" />
                        <button type="button" class="btn btn-default" data-dismiss="modal">Annulla</button>
                    </div>
                </form>
            </div>
        </div>
    </div>

    <!-- Options Modal -->
    <div class="modal fade" id="options_modal" tabindex="-1" role="dialog" aria-labelledby="options_modal_label" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <form class="form-horizontal">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                        <h4 class="modal-title" id="options_modal_label">Proprietà controllo</h4>
                    </div>

                    <div class="modal-body" style="padding: 15px; background-color: #eee;">
                    </div>
                    <div class="modal-footer">
                        <input type="submit" class="btn btn-primary" id="save_options" value="Conferma" />
                        <button type="button" class="btn btn-default" data-dismiss="modal">Annulla</button>
                    </div>
                </form>
            </div>
        </div>
    </div>

    <!-- Configuration Modal -->
    <div class="modal fade" id="configuration_modal" tabindex="-1" role="dialog" aria-labelledby="category_modal_label" aria-hidden="true"></div>



    <!--Components-->

    <div id="components-container">

        <div id="component-Title" data-type="Title">
            <div class="component-toolbar">
                <span class="label" rv-enabled="ctrl.enabledWorkflow" rv-class-label-info="ctrl.enabledWorkflow">Flusso</span>
                <span class="label" rv-enabled="ctrl.enabledProtocol" rv-class-label-info="ctrl.enabledProtocol">Protocollo</span>
                <span class="label" rv-enabled="ctrl.enabledPEC" rv-class-label-info="ctrl.enabledPEC">PEC</span>
                <span class="label" rv-enabled="ctrl.enabledCQRSSync" rv-class-label-info="ctrl.enabledCQRSSync">Fascicolabile</span>
            </div>
            <div rv-text="ctrl.label" style="margin-bottom: 10px; font-weight: bold;"></div>
            <span><small>Oggetto</small></span>
            <input type="text" name="" id="text_input" class="form-control" rv-value="ctrl.subject" style="width: 320px; min-width: 120px;">
            <div rv-class-hidden="ctrl.alias | isEmpty" style="margin-top: 10px;">
                <span><small>Alias</small></span><br />
                <label class="" rv-text="ctrl.alias" />
            </div>

            <div rv-hidden="true" id="layoutColumns" rv-text="ctrl.selectedItem"></div>
                       
            <div rv-class-hidden="ctrl.categoryName | isEmpty" style="margin-top: 10px;">
                <span><small>Classificatore</small></span><br />
                <label class="" rv-text="ctrl.categoryName" />
            </div>
            <div rv-class-hidden="ctrl.containerName | isEmpty" style="margin-top: 10px;">
                <span><small>Contenitore</small></span><br />
                <label class="" rv-text="ctrl.containerName" />
            </div>
        </div>

        <div id="component-Header" data-type="Header" style="margin-top: 10px;">
            <div rv-text="ctrl.label" style="background-color: #08c; color: #fff; padding: 5px;"></div>
        </div>

        <div id="component-Text" data-type="Text" style="padding: 6px;" rv-class-read-only="ctrl.readOnly">
            <div class="component-toolbar">
                <span class="label" rv-enabled="ctrl.readOnly" rv-class-label-info="ctrl.readOnly">Sola lettura</span>
                <span class="label" rv-enabled="ctrl.required" rv-class-label-info="ctrl.required">Richiesto</span>
                <span class="label" rv-enabled="ctrl.searchable" rv-class-label-info="ctrl.searchable">Ricercabile</span>
            </div>
            <label class="control-label" for="text_input" rv-text="ctrl.label"></label>
            <div class="controls">
                <input rv-disabled="ctrl.readOnly" type="text" name="" id="text_input" class="form-control" rv-hide="ctrl.multiLine" rv-value="ctrl.defaultValue" style="width: 320px; min-width: 120px;" />
                <textarea rv-disabled="ctrl.readOnly" name="" id="text_area" class="form-control" rv-show="ctrl.multiLine" rv-value="ctrl.defaultValue" style="width: 320px; min-width: 120px; height: 100px;"></textarea>
            </div>
        </div>

        <div id="component-Lookup" data-type="Lookup" style="padding: 6px;" rv-class-read-only="ctrl.readOnly">
            <div class="component-toolbar">
                <span class="label" rv-enabled="ctrl.required" rv-class-label-info="ctrl.required">Richiesto</span>
                <span class="label" rv-enabled="ctrl.searchable" rv-class-label-info="ctrl.searchable">Ricercabile</span>
            </div>
            <span class="fa fa-search fa-lg" aria-hidden="true"></span>
            <label class="control-label" for="text_input" rv-text="ctrl.label"></label>
        </div>

        <div id="component-Date" data-type="Date" style="padding: 5px;" rv-class-read-only="ctrl.readOnly">
            <div class="component-toolbar">
                <span class="label" rv-enabled="ctrl.readOnly" rv-class-label-info="ctrl.readOnly">Sola lettura</span>
                <span class="label" rv-enabled="ctrl.required" rv-class-label-info="ctrl.required">Richiesto</span>
                <span class="label" rv-enabled="ctrl.searchable" rv-class-label-info="ctrl.searchable">Ricercabile</span>
                <span class="label" rv-enabled="ctrl.restrictedYear" rv-class-label-info="ctrl.restrictedYear">Anno</span>
            </div>
            <label class="control-label" for="text_input" rv-text="ctrl.label"></label>
            <div class="controls">
                <input rv-disabled="ctrl.readOnly" type="text" name="" id="text_input" class="form-control" rv-value="ctrl.defaultValue" style="width: 120px;" />
            </div>
        </div>

        <div id="component-Number" data-type="Number" style="padding: 5px;" rv-class-read-only="ctrl.readOnly">
            <div class="component-toolbar">
                <span class="label" rv-enabled="ctrl.readOnly" rv-class-label-info="ctrl.readOnly">Sola lettura</span>
                <span class="label" rv-enabled="ctrl.required" rv-class-label-info="ctrl.required">Richiesto</span>
                <span class="label" rv-enabled="ctrl.searchable" rv-class-label-info="ctrl.searchable">Ricercabile</span>
            </div>
            <label class="control-label" for="text_input" rv-text="ctrl.label"></label>
            <div class="controls">
                <input rv-disabled="ctrl.readOnly" type="text" name="" id="text_input" class="form-control" rv-value="ctrl.defaultValue" style="width: 120px;" />
            </div>
        </div>

        <div id="component-Checkbox" data-type="Checkbox" style="padding: 5px 5px 5px 7px;" rv-class-read-only="ctrl.readOnly">
            <div class="component-toolbar">
                <span class="label" rv-enabled="ctrl.readOnly" rv-class-label-info="ctrl.readOnly">Sola lettura</span>
                <span class="label" rv-enabled="ctrl.required" rv-class-label-info="ctrl.required">Richiesto</span>
                <span class="label" rv-enabled="ctrl.searchable" rv-class-label-info="ctrl.searchable">Ricercabile</span>
            </div>
            <div class="controls">
                <input rv-disabled="ctrl.readOnly" type="checkbox" name="checkbox_1" id="checkbox_1" rv-checked="ctrl.defaultValue" />
                <label class="" rv-text="ctrl.label" />
            </div>
        </div>


        <div id="component-Document" data-type="Document" style="padding: 5px;" rv-class-read-only="ctrl.readOnly">
            <div class="component-toolbar">
                <span class="label" rv-enabled="ctrl.readOnly" rv-class-label-info="ctrl.readOnly">Sola lettura</span>
                <span class="label" rv-enabled="ctrl.required" rv-class-label-info="ctrl.required">Richiesto</span>
                <span class="label" rv-enabled="ctrl.searchable" rv-class-label-info="ctrl.searchable">Ricercabile</span>
                <span class="label" rv-enabled="ctrl.enableMultifile" rv-class-label-info="ctrl.enableMultifile">Multisel.</span>

                <span class="label" rv-enabled="ctrl.enableUpload" rv-class-btn-warning="ctrl.enableUpload">Upload</span>
                <span class="label" rv-enabled="ctrl.enableScanner" rv-class-btn-warning="ctrl.enableScanner">Scan</span>
                <span class="label" rv-enabled="ctrl.enableSign" rv-class-btn-warning="ctrl.enableSign">Firma</span>
                <span class="label" rv-enabled="ctrl.signRequired" rv-class-btn-warning="ctrl.signRequired">Firma Richiesta</span>
                <span class="label" rv-enabled="ctrl.copyProtocol" rv-class-btn-warning="ctrl.copyProtocol">Prot.</span>
                <span class="label" rv-enabled="ctrl.copyResolution" rv-class-btn-warning="ctrl.copyResolution">Atti</span>
                <span class="label" rv-enabled="ctrl.copySeries" rv-class-btn-warning="ctrl.copySeries">Serie</span>
                <span class="label" rv-enabled="ctrl.createBiblosArchive" rv-class-btn-warning="ctrl.createBiblosArchive">Crea archivio</span>
            </div>

            <span class="fa fa-file-text-o fa-lg" aria-hidden="true"></span>
            <span rv-text="ctrl.label" style="font-weight: bold;"></span>
            (collezione: <span rv-text="ctrl.collectionType" style="font-weight: bold;"></span>)
      <div class="component-attribute">
          archivio: <span rv-text="ctrl.archive"></span>
      </div>
        </div>


        <div id="component-Contact" data-type="Contact" style="padding: 5px; padding-bottom: 20px;">
            <div class="component-toolbar">
                <span class="label" rv-enabled="ctrl.required" rv-class-label-info="ctrl.required">Richiesto</span>
                <span class="label" rv-enabled="ctrl.searchable" rv-class-label-info="ctrl.searchable">Ricercabile</span>
                <span class="label" rv-enabled="ctrl.enableMultiContact" rv-class-label-info="ctrl.enableMultiContact">Multisel.</span>
                <span class="label" rv-enabled="ctrl.enableAD" rv-class-label-warning="ctrl.enableAD">AD</span>
                <span class="label" rv-enabled="ctrl.enableADDistribution" rv-class-label-warning="ctrl.enableADDistribution">AD-Dist</span>
                <span class="label" rv-enabled="ctrl.enableAddressBook" rv-class-label-warning="ctrl.enableAddressBook">Rubrica</span>
                <span class="label" rv-enabled="ctrl.enableManual" rv-class-label-warning="ctrl.enableManual">Manuali</span>
                <span class="label" rv-enabled="ctrl.enableExcelImport" rv-class-label-warning="ctrl.enableExcelImport">Excel</span>
            </div>
            <span class="fa fa-user-plus fa-lg" aria-hidden="true"></span>
            <span rv-text="ctrl.label" style="font-weight: bold;"></span>

        </div>


        <div id="component-Authorization" data-type="Authorization" style="padding: 5px; padding-bottom: 20px;">
            <div class="component-toolbar">
                <span class="label" rv-enabled="ctrl.required" rv-class-label-info="ctrl.required">Richiesto</span>
                <span class="label" rv-enabled="ctrl.searchable" rv-class-label-info="ctrl.searchable">Ricercabile</span>
                <span class="label" rv-enabled="ctrl.enableMultiAuth" rv-class-label-info="ctrl.enableMultiAuth">Autoriz. multipla</span>
            </div>

            <span class="fa fa-arrow-circle-o-right fa-lg" aria-hidden="true"></span>
            <span rv-text="ctrl.label" style="font-weight: bold;"></span>
        </div>

        <div id="component-Enum" data-type="Enum" style="padding: 5px;">
            <div class="component-toolbar">
                <span class="label" rv-enabled="ctrl.readOnly" rv-class-label-info="ctrl.readOnly">Sola lettura</span>
                <span class="label" rv-enabled="ctrl.required" rv-class-label-info="ctrl.required">Richiesto</span>
                <span class="label" rv-enabled="ctrl.searchable" rv-class-label-info="ctrl.searchable">Ricercabile</span>
            </div>

            <span class="fa fa-list fa-lg" aria-hidden="true"></span>
            <label class="" rv-text="ctrl.label" />
        </div>
        
        <div id="component-Status" data-type="Status" style="padding: 5px;">
            <div class="component-toolbar">
                <span class="label" rv-enabled="ctrl.readOnly" rv-class-label-info="ctrl.readOnly">Sola lettura</span>
                <span class="label" rv-enabled="ctrl.required" rv-class-label-info="ctrl.required">Richiesto</span>
                <span class="label" rv-enabled="ctrl.searchable" rv-class-label-info="ctrl.searchable">Ricercabile</span>
            </div>

            <span class="fa fa-signal fa-lg" aria-hidden="true"></span>
            <span rv-text="ctrl.label" style="font-weight: bold;"></span>
        </div>


    </div>

    <script type="text/javascript" src="../Scripts/jquery-3.3.1.min.js"></script>
    <script type="text/javascript" src="../Scripts/jquery-ui-1.11.4.min.js"></script>
    <script type="text/javascript" src="Scripts/bootstrap.min.js"></script>
    <script type="text/javascript" src="Scripts/jstree.min.js"></script>
    <script type="text/javascript" src="Scripts/rivets.bundled.min.js?v=0.9.6"></script>
    <script type="text/javascript" src="../Scripts/moment-with-locales.min.js"></script>
    <script type="text/javascript" src="Scripts/bootstrap-datetimepicker.min.js"></script>
    <script type="text/javascript" src="Scripts/nprogress.js"></script>
    <script type="text/javascript" src="Scripts/JsonEditor/jsoneditor.min.js" charset="utf-8"></script>
    <script type="text/javascript" src="Scripts/App/Service/PathService.js"></script>

    <script type="text/javascript" src="Scripts/UdsDesigner/formbuilder.js"></script>
    <script type="text/javascript" src="Scripts/UdsDesigner/controls.js"></script>
    <script type="text/javascript" src="Scripts/UdsDesigner/service.js"></script>

    <script type="text/javascript" src="Scripts/App/ViewModels/BaseTreeViewModel.js"></script>
    <script type="text/javascript" src="Scripts/App/ViewModels/CategoryViewModel.js"></script>
    <script type="text/javascript" src="Scripts/App/ViewModels/ContainerViewModel.js"></script>
    <script type="text/javascript" src="Scripts/App/Controllers/DesignerResultsController.js"></script>
    <script type="text/javascript" src="Scripts/App/Controllers/DesignerController.js"></script>

    <script type="text/javascript" src="Scripts/App/utils.js"></script>
    <script type="text/javascript" src="Scripts/App/main.js"></script>

</body>
</html>

import TbltPECMailBoxBase = require('Tblt/TbltPECMailBoxBase');
import PECMailBoxService = require('App/Services/PECMails/PECMailBoxService');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ChainType = require('App/Models/DocumentUnits/ChainType');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import PECMailBoxViewModel = require('App/ViewModels/PECMails/PECMailBoxViewModel');
import PECMailBoxRulesetModel = require('App/Models/PECMails/PECMailBoxRulesetModel');
import PECMailBoxModel = require("App/Models/PECMails/PECMailBoxModel");

class TbltPECMailBox extends TbltPECMailBoxBase {
  protected PECMailBox_TYPE_NAME = "PECMailBox";
  private _serviceConfigurations: ServiceConfiguration[];

  ajaxLoadingPanelId: string;
  uscNotificationId: string;
  splitterMainId: string;
  ToolBarSearchId: string;
  rtvPECMailBoxesId: string;
  rtvResult: PECMailBoxViewModel[];
  rpbDetailsId: string;
  lblPECMailBoxIdId: string;
  lblMailBoxRecipientId: string;
  lblIncomingServerId: string;
  lblOutgoingServerId: string;
  lblRulesetNameId: string;
  lblRulesetConditionId: string;
  lblRulesetTypeId: string;
  btnPECMailBoxSetRuleId: string;
  windowSetRuleId: string;
  txtRulesetNameId: string;
  txtSpecifySenderId: string;
  rlbSpecifyPECMailBoxId: string;
  btnPECMAilBoxSaveId: string;
  cmdUpdatePECId: string;
  cmdAddPECMailBoxId: string;
  uscPECMailBoxSettingsId: string;
  hfPECMailBoxIdId:string;

  private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
  private _toolbarSearch: Telerik.Web.UI.RadToolBar;
  private _rtvPECMailBoxes: Telerik.Web.UI.RadTreeView;
  private _toolbarItemSearchName: Telerik.Web.UI.RadToolBarItem;
  private _txtSearchName: Telerik.Web.UI.RadTextBox;
  private _toolbarItemButtonSearch: Telerik.Web.UI.RadToolBarItem;
  private _rpbDetails: Telerik.Web.UI.RadPanelBar;
  private _lblPECMailBoxId: HTMLLabelElement;
  private _lblMailBoxRecipient: HTMLLabelElement;
  private _lblIncomingServer: HTMLLabelElement;
  private _lblOutgoingServer: HTMLLabelElement;
  private _lblRulesetName: HTMLLabelElement;
  private _lblRulesetCondition: HTMLLabelElement;
  private _lblRulesetType: HTMLLabelElement;
  private _btnPECMailBoxSetRule: Telerik.Web.UI.RadButton;
  private _windowSetRule: Telerik.Web.UI.RadWindow;
  private _txtRulesetName: Telerik.Web.UI.RadTextBox;
  private _txtSpecifySender: Telerik.Web.UI.RadTextBox;
  private _rlbSpecifyPECMailBox: Telerik.Web.UI.RadDropDownList;
  private _btnPECMailBoxSave: Telerik.Web.UI.RadButton;
  private _cmdUpdatePEC: HTMLButtonElement;
  private _cmdAddPECMailBox: HTMLButtonElement;
  private _windowPECMailBoxSettings: Telerik.Web.UI.RadWindow;

  constructor(serviceConfigurations: ServiceConfiguration[]) {
    super(ServiceConfigurationHelper.getService(serviceConfigurations, TbltPECMailBoxBase.PECMailBox_TYPE_NAME));
    this._serviceConfigurations = serviceConfigurations;
    $(document).ready(() => {

    });
  }

  initialize() {
    super.initialize();
    this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
    this._toolbarSearch = <Telerik.Web.UI.RadToolBar>$find(this.ToolBarSearchId);
    this._toolbarSearch.add_buttonClicking(this.toolbarSearch_onClick);
    this._rtvPECMailBoxes = <Telerik.Web.UI.RadTreeView>$find(this.rtvPECMailBoxesId);
    this._rtvPECMailBoxes.add_nodeClicking(this.rtvPECMailBoxes_onClick);
    this._toolbarItemSearchName = this._toolbarSearch.findItemByValue("searchName");
    this._txtSearchName = <Telerik.Web.UI.RadTextBox>this._toolbarItemSearchName.findControl("txtName");
    this._toolbarItemButtonSearch = <Telerik.Web.UI.RadToolBarItem>this._toolbarSearch.findItemByValue("searchCommand");
    this._rpbDetails = <Telerik.Web.UI.RadPanelBar>$find(this.rpbDetailsId);
    this._rpbDetails.findItemByValue("rpiDetails").set_visible(false);
    this._lblPECMailBoxId = <HTMLLabelElement>document.getElementById(this.lblPECMailBoxIdId);
    this._lblMailBoxRecipient = <HTMLLabelElement>document.getElementById(this.lblMailBoxRecipientId);
    this._lblIncomingServer = <HTMLLabelElement>document.getElementById(this.lblIncomingServerId);
    this._lblOutgoingServer = <HTMLLabelElement>document.getElementById(this.lblOutgoingServerId);
    this._lblRulesetName = <HTMLLabelElement>document.getElementById(this.lblRulesetNameId);
    this._lblRulesetCondition = <HTMLLabelElement>document.getElementById(this.lblRulesetConditionId);
    this._lblRulesetType = <HTMLLabelElement>document.getElementById(this.lblRulesetTypeId);
    this._btnPECMailBoxSetRule = <Telerik.Web.UI.RadButton>$find(this.btnPECMailBoxSetRuleId);
    this._btnPECMailBoxSetRule.add_clicking(this.btnPECMailBoxSetRule_onClick);
    this._windowSetRule = <Telerik.Web.UI.RadWindow>$find(this.windowSetRuleId);
    this._txtRulesetName = <Telerik.Web.UI.RadTextBox>$find(this.txtRulesetNameId);
    this._txtSpecifySender = <Telerik.Web.UI.RadTextBox>$find(this.txtSpecifySenderId);
    this._rlbSpecifyPECMailBox = <Telerik.Web.UI.RadDropDownList>$find(this.rlbSpecifyPECMailBoxId);
    this._btnPECMailBoxSave = <Telerik.Web.UI.RadButton>$find(this.btnPECMAilBoxSaveId);
    this._btnPECMailBoxSave.add_clicking(this.btnPECMailBoxSave_onClick);
    this._cmdUpdatePEC = <HTMLButtonElement>document.getElementById(this.cmdUpdatePECId);
    this._cmdAddPECMailBox = <HTMLButtonElement>document.getElementById(this.cmdAddPECMailBoxId);
    this._windowPECMailBoxSettings = <Telerik.Web.UI.RadWindow>$find(this.uscPECMailBoxSettingsId);
    this._cmdUpdatePEC.onclick = (event: MouseEvent) => {
      if (this._rtvPECMailBoxes.get_selectedNode() !== null) {
        document.getElementById(this.hfPECMailBoxIdId).innerText = this._rtvPECMailBoxes.get_selectedNode().get_value();
        this._windowPECMailBoxSettings.show();
      }
      else
        alert("Nessun elemento selezionato per la modifica.");
      return false;
    }
    this._cmdAddPECMailBox.onclick = (event: MouseEvent) => {
      document.getElementById(this.hfPECMailBoxIdId).innerText = "";
      this._windowPECMailBoxSettings.show();
      return false;
    }
  }

  toolbarSearch_onClick = (sender: any, args: Telerik.Web.UI.RadToolBarCancelEventArgs) => {
    this._loadingPanel.show(this.splitterMainId);
    this.loadResults(this._txtSearchName.get_textBoxValue());
    this._rpbDetails.findItemByValue("rpiDetails").set_visible(false);
  }

  private loadResults(searchFilter: string) {
    this.pecMailBoxService.getPECMailBoxes(searchFilter,
      (data) => {
        if (!data) return;
        this.rtvResult = data;
        this._rtvPECMailBoxes.get_nodes().clear();
        let rtvNode: Telerik.Web.UI.RadTreeNode;
        rtvNode = new Telerik.Web.UI.RadTreeNode();
        rtvNode.set_text("Caselle PEC");
        this._rtvPECMailBoxes.get_nodes().add(rtvNode);
        var thisObj = this;
        $.each(this.rtvResult, function (i, value: PECMailBoxViewModel) {
          rtvNode = new Telerik.Web.UI.RadTreeNode();
          rtvNode.set_text(value.MailBoxRecipient);
          rtvNode.set_value(value.EntityShortId);
          thisObj._rtvPECMailBoxes.get_nodes().getNode(0).get_nodes().add(rtvNode);
        });
        this._rtvPECMailBoxes.get_nodes().getNode(0).expand();
        this._loadingPanel.hide(this.splitterMainId);
      },
      (exception: ExceptionDTO) => {
        this._loadingPanel.hide(this.splitterMainId);
        $("#".concat(this.rtvPECMailBoxesId)).hide();
        this.showNotificationException(this.uscNotificationId, exception);
      });
  }

  rtvPECMailBoxes_onClick = (sender: any, args: Telerik.Web.UI.RadTreeNodeCancelEventArgs) => {
    this._rpbDetails.findItemByValue("rpiDetails").set_visible(true);
    this.loadPECMailBoxDetails(args.get_node().get_value());
  }

  loadPECMailBoxDetails(pecMailBoxId: number) {
    this._btnPECMailBoxSetRule.set_enabled(true);
    let pecMailBox: PECMailBoxViewModel = this.rtvResult.filter(function (x) {
      return x.EntityShortId === pecMailBoxId
    })[0];
    this._lblPECMailBoxId.innerText = pecMailBox.EntityShortId.toString();
    this._lblMailBoxRecipient.innerText = pecMailBox.MailBoxRecipient;
    this._lblIncomingServer.innerText = pecMailBox.IncomingServer;
    this._lblOutgoingServer.innerText = pecMailBox.OutgoingServer;
    if (pecMailBox.RulesetDefinition !== null) {
      let ruleset: PECMailBoxRulesetModel = JSON.parse(pecMailBox.RulesetDefinition);
      this._lblRulesetName.innerText = ruleset.Name === undefined ? "" : ruleset.Name;
      this._lblRulesetCondition.innerText = ruleset.Condition === undefined ? "" : ruleset.Condition;
      this._lblRulesetType.innerText = ruleset.Rule === undefined ? "" : ruleset.Rule;
    } else {
      this._lblRulesetName.innerText = "";
      this._lblRulesetCondition.innerText = "";
      this._lblRulesetType.innerText = "";
    }
  }

  btnPECMailBoxSetRule_onClick = (sender: any, args: Telerik.Web.UI.ButtonEventArgs) => {
    this._windowSetRule.show();
    this._loadingPanel.show(this.windowSetRuleId);
    this.pecMailBoxService.getPECMailBoxes("",
      (data) => {
        if (!data) return;
        let pecMailBoxesExceptSelected = data.filter(x => x.EntityShortId !== +this._lblPECMailBoxId.innerText);
        this._rlbSpecifyPECMailBox.get_items().clear();
        let rlbItem: Telerik.Web.UI.DropDownListItem;
        var thisObj = this;
        $.each(pecMailBoxesExceptSelected,
          function (i, value: PECMailBoxViewModel) {
            rlbItem = new Telerik.Web.UI.DropDownListItem();
            rlbItem.set_text(value.MailBoxRecipient);
            rlbItem.set_value(value.EntityShortId.toString());
            thisObj._rlbSpecifyPECMailBox.get_items().add(rlbItem);
          });
        this._loadingPanel.hide(this.windowSetRuleId);
      },
      (exception: ExceptionDTO) => {
        this._loadingPanel.hide(this.windowSetRuleId);
        $("#".concat(this.rtvPECMailBoxesId)).hide();
        this.showNotificationException(this.uscNotificationId, exception);
      });
  }

  btnPECMailBoxSave_onClick = (sender: any, args: Telerik.Web.UI.ButtonEventArgs) => {
    let thisObj = this;
    let pecMailBox: PECMailBoxModel = <PECMailBoxModel>this.rtvResult.filter(function (x) {
      return x.EntityShortId === +thisObj._lblPECMailBoxId.innerText
    })[0];
    let ruleset: PECMailBoxRulesetModel = pecMailBox.RulesetDefinition === null ? new PECMailBoxRulesetModel() : JSON.parse(pecMailBox.RulesetDefinition);
    ruleset.Name = this._txtRulesetName.get_value();
    pecMailBox.MailBoxRecipient = this._txtSpecifySender.get_value();
    let selectedPECMailBoxId: number = +this._rlbSpecifyPECMailBox.get_selectedItem().get_value();
    let selectedPECMailBox: PECMailBoxModel = <PECMailBoxModel>this.rtvResult.filter(function (x) {
      return x.EntityShortId === selectedPECMailBoxId
    })[0];
    ruleset.Reference = selectedPECMailBox;
    pecMailBox.RulesetDefinition = JSON.stringify(ruleset);
    this.updateCallback(pecMailBox);
  }

  updateCallback(pecMailBoxModel: PECMailBoxModel): void {
    this.pecMailBoxService.updatePECMailBox(pecMailBoxModel,
      (data: any) => {
        if (!data) return;
        this._loadingPanel.show(this.splitterMainId);
        this.loadResults(this._txtSearchName.get_textBoxValue());
        this._windowSetRule.close();
        this.loadPECMailBoxDetails(pecMailBoxModel.EntityShortId);
      },
      (exception: ExceptionDTO) => {
        this.showNotificationException(this.uscNotificationId, exception);
      });
  }
}

export = TbltPECMailBox;

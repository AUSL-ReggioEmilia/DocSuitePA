import ProcessNodeType = require("Models/Processes/ProcessNodeType");

interface TreeNodeModel {
    text: string;
    value: string;
    icon: string;
    cssClass: string;
    nodeType: ProcessNodeType;
}

export = TreeNodeModel;
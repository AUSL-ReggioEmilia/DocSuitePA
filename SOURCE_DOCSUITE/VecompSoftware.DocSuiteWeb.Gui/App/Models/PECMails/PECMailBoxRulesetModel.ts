import PecMailBoxModel = require("./PECMailBoxModel");

class PECMailBoxRulesetModel {
  Name: string;
  Rule: string;
  Condition: string;
  Reference: PecMailBoxModel;
}

export = PECMailBoxRulesetModel;
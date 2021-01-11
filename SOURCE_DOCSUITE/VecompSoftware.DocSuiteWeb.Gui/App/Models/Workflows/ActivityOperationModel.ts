import ActivityArea = require("./ActivityArea");
import ActivityAction = require("./ActivityAction");

interface ActivityOperation {
    Action: ActivityAction;
    Area: ActivityArea;
}

export = ActivityOperation;
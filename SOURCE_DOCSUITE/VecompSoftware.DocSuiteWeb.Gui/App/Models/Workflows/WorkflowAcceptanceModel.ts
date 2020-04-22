import RoleModel = require('App/Models/Commons/RoleModel');
import AcceptanceStatus = require('App/Models/Workflows/AcceptanceStatus');

interface WorkflowAcceptanceModel {
    Owner: string;
    Executor: string;
    ProposedRole: RoleModel;
    Status: AcceptanceStatus;
    AcceptanceDate: Date;
    AcceptanceReason: string;
}
export = WorkflowAcceptanceModel;
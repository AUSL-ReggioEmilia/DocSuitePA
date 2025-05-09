﻿import ActivityType = require('App/Models/Workflows/ActivityType');
import WorkflowStatus = require('App/Models/Workflows/WorkflowStatus');
import WorkflowProperty = require('App/Models/Workflows/WorkflowProperty');
import WorkflowAuthorization = require('App/Models/Workflows/WorkflowAuthorizationModel');
import WorkflowInstance = require('App/Models/Workflows/WorkflowInstanceModel');
import ActivityAction = require('App/Models/Workflows/ActivityAction');
import ActivityArea = require('App/Models/Workflows/ActivityArea');
import WorkflowActivityLogModel = require('./WorkflowActivityLogModel');
import TenantModel = require('App/Models/Tenants/TenantModel');

interface WorkflowActivityModel {
    UniqueId: string;
    Name: string;
    ActivityType: ActivityType;
    Status: WorkflowStatus;
    DueDate?: Date;
    Subject: string;
    Note: string;
    ActivityTypeDescription: string;
    StatusDescription: string;
    RegistrationUser: string;
    RegistrationDate: Date;
    RegistrationDateFormatted: string;
    Priority: string;
    Documents: string;
    IdArchiveChain: string;
    ActivityAction: ActivityAction;
    ActivityArea: ActivityArea;
    Tenant: TenantModel;

    WorkflowProperties: WorkflowProperty[];
    WorkflowAuthorizations: WorkflowAuthorization[];
    WorkflowInstance: WorkflowInstance;
    WorkflowActivityLogs: WorkflowActivityLogModel[];
}

export = WorkflowActivityModel;
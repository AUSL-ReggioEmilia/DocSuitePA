﻿namespace VecompSoftware.DocSuiteWeb.Entity.Tasks
{
    public enum TaskHeaderSendingProcessStatus : short
    {
        Todo = 1,
        InProgress = 2 * Todo,
        Complete = 2 * InProgress
    }
}

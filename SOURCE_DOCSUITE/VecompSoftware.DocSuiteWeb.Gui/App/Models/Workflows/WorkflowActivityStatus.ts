
enum WorkflowActivityStatus {
    Todo = 1,
    Handling = 2,
    Rejected = 2 * Handling,
    Done = 2 * Rejected
}

export = WorkflowActivityStatus;
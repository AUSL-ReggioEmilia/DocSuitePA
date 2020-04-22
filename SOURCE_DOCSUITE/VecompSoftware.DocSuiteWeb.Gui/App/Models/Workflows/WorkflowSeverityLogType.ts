enum WorkflowSeverityLogType {
    Off = 1,
    Fatal = 2,
    Error = 2 * Fatal,
    Warning = 2 * Error,
    Info = 2 * Warning,
    Debug = 2 * Info,
    Trace = 2 * Debug
}

export = WorkflowSeverityLogType;
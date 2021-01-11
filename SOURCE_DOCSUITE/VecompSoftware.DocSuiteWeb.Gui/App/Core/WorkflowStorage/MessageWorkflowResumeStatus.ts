
//Mapping the enum response that comes from ResumeStatus channel
abstract class MessageWorkflowResumeStatus {
    public static Resumed: number = 0;
    public static DidNotResume: number = 1;
}

export = MessageWorkflowResumeStatus;
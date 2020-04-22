Public Enum POLRequestStatusEnum
    RequestQueued = 0
    RequestSent = 1
    NeedConfirm = 2
    Confirmed = 3
    Executed = 4
    [Error] = -1
End Enum
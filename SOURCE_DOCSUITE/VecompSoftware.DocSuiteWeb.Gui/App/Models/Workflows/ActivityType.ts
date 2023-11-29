
enum ActivityType {
    Undefined = -1,
    AutomaticActivity = 0,
    ProtocolCreate = 1,
    PecToProtocol = 2,
    UDSCreate = 3,
    UDSToProtocol = 4,
    UDSToPEC = 5,
    CollaborationCreate = 6,
    CollaborationSign = 7,
    CollaborationToProtocol = 8,
    Assignment = 9,
    DematerialisationStatement = 11,
    SecureDocumentCreate = 12,
    BuildAchive = 13,
    BuildProtocol = 14,
    BuildPECMail = 15,
    BuildMessages = 16,
    DocumentUnitIntoFascicle = 17,
    DocumentUnitLinks = 18,
    GenericActivity = 19,
    ProtocolUpdate = 20
}

export = ActivityType;
export enum ChainType{
    Miscellanea = -1,
    UnitContainer = 0,
    MainChain = 1,
    AttachmentsChain = MainChain * 2,
    AnnexedChain = AttachmentsChain * 2,
    UnpublishedAnnexedChain = AnnexedChain * 2,
    ProposalChain = UnpublishedAnnexedChain * 2,
    ControllerChain = ProposalChain * 2,
    AssumedProposalChain = ControllerChain * 2,
    FrontespizioChain = AssumedProposalChain * 2,
    PrivacyAttachmentChain = FrontespizioChain * 2,
    FrontalinoRitiroChain = PrivacyAttachmentChain * 2,
    PrivacyPublicationDocumentChain = FrontalinoRitiroChain * 2,
    MainOmissisChain = PrivacyPublicationDocumentChain * 2,
    AttachmentOmissisChain = MainOmissisChain * 2,
    UltimaPaginaChain = AttachmentOmissisChain * 2,
    SupervisoryBoardChain = UltimaPaginaChain * 2
}
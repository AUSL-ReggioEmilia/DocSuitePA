define(["require", "exports"], function (require, exports) {
    var ChainType;
    (function (ChainType) {
        ChainType[ChainType["Miscellanea"] = -1] = "Miscellanea";
        ChainType[ChainType["UnitContainer"] = 0] = "UnitContainer";
        ChainType[ChainType["MainChain"] = 1] = "MainChain";
        ChainType[ChainType["AttachmentsChain"] = 2] = "AttachmentsChain";
        ChainType[ChainType["AnnexedChain"] = 4] = "AnnexedChain";
        ChainType[ChainType["UnpublishedAnnexedChain"] = 8] = "UnpublishedAnnexedChain";
        ChainType[ChainType["ProposalChain"] = 16] = "ProposalChain";
        ChainType[ChainType["ControllerChain"] = 32] = "ControllerChain";
        ChainType[ChainType["AssumedProposalChain"] = 64] = "AssumedProposalChain";
        ChainType[ChainType["FrontespizioChain"] = 128] = "FrontespizioChain";
        ChainType[ChainType["PrivacyAttachmentChain"] = 256] = "PrivacyAttachmentChain";
        ChainType[ChainType["FrontalinoRitiroChain"] = 512] = "FrontalinoRitiroChain";
        ChainType[ChainType["PrivacyPublicationDocumentChain"] = 1024] = "PrivacyPublicationDocumentChain";
        ChainType[ChainType["MainOmissisChain"] = 2048] = "MainOmissisChain";
        ChainType[ChainType["AttachmentOmissisChain"] = 4096] = "AttachmentOmissisChain";
        ChainType[ChainType["UltimaPaginaChain"] = 8192] = "UltimaPaginaChain";
        ChainType[ChainType["SupervisoryBoardChain"] = 16384] = "SupervisoryBoardChain";
        ChainType[ChainType["DematerialisationChain"] = 32768] = "DematerialisationChain";
    })(ChainType || (ChainType = {}));
    return ChainType;
});
//# sourceMappingURL=ChainType.js.map
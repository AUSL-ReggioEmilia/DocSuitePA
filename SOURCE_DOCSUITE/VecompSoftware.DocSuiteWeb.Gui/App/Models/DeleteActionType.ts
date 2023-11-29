
enum DeleteActionType {
    DematerialisationLogDelete = 0,
    SecureDocumentLogDelete = 1,
    DeleteCategory = SecureDocumentLogDelete * 2,
    CancelFascicle = DeleteCategory * 2,
    DeleteProtocol = CancelFascicle * 2,
    DeleteCategoryFascicle = DeleteProtocol * 2,
    CancelProcess = DeleteCategoryFascicle * 2,
    DeleteDocumentSeriesItem = CancelProcess * 2,
    DeleteUDSFieldList = DeleteDocumentSeriesItem * 2
  }

export = DeleteActionType;
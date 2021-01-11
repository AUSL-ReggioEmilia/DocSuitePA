
enum DeleteActionType {
    DematerialisationLogDelete = 0,
    SecureDocumentLogDelete = 1,
    DeleteCategory = SecureDocumentLogDelete * 2,
    CancelFascicle = DeleteCategory * 2,
  }

export = DeleteActionType;
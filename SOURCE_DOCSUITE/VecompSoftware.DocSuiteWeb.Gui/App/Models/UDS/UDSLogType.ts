enum UDSLogType {
    Insert = 0,
    Modify = 1,
    AuthorizationModify = Modify * 2,
    DocumentModify = AuthorizationModify * 2,
    ObjectModify = DocumentModify * 2,
    Delete = ObjectModify * 2,
    DocumentView = Delete * 2,
    SummaryView = DocumentView * 2,
    AuthorizationInsert = SummaryView * 2,
    AuthorizationDelete = AuthorizationInsert * 2
}
export = UDSLogType;
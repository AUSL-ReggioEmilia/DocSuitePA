enum WorkflowReferenceType {
    Json = 1,
    RelationGuid = 2,
    RelationInt = 2 * RelationGuid,
    PropertyString = 2 * RelationInt,
    PropertyInt = 2 * PropertyString,
    PropertyDate = 2 * PropertyInt,
    PropertyDouble = 2 * PropertyDate,
    PropertyBoolean = 2 * PropertyDouble,
    PropertyGuid = 2 * PropertyBoolean
}
export = WorkflowReferenceType;
enum WorkflowEvaluationPropertyType {
    Boolean = 1,
    Double = 2,
    Integer = 2 * Double,
    String = 2 * Integer,
    Json = 2 * String,
    Guid = 2 * Json,
    Date = 2 * Guid,
    RelationGuid = 2 * Date,
    RelationInt = 2 * RelationGuid
}
export = WorkflowEvaluationPropertyType;
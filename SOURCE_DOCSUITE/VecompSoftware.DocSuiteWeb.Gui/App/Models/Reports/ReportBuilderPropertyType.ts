
enum ReportBuilderPropertyType {
    String = 1,
    Int = 2,
    Short = Int * 2,
    Double = Short * 2,
    Bool = Double * 2,
    Guid = Bool * 2,
    DateTime = Guid * 2,
    DateTimeOffset = DateTime * 2,

    /** Metadata Repository */
    MetadataText = DateTimeOffset * 2,
    MetadataBool = MetadataText * 2,
    MetadataDateTime = MetadataBool * 2,
    MetadataNumber = MetadataDateTime * 2,
    MetadataEnum = MetadataNumber * 2,
    MetadataDiscussion = MetadataEnum * 2,
    /** */

    /** Commons */
    Contact = MetadataDiscussion * 2,
    Container = Contact * 2,
    Category = Container * 2
    /** */
}

export = ReportBuilderPropertyType;
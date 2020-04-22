define(["require", "exports"], function (require, exports) {
    var ReportBuilderPropertyType;
    (function (ReportBuilderPropertyType) {
        ReportBuilderPropertyType[ReportBuilderPropertyType["String"] = 1] = "String";
        ReportBuilderPropertyType[ReportBuilderPropertyType["Int"] = 2] = "Int";
        ReportBuilderPropertyType[ReportBuilderPropertyType["Short"] = 4] = "Short";
        ReportBuilderPropertyType[ReportBuilderPropertyType["Double"] = 8] = "Double";
        ReportBuilderPropertyType[ReportBuilderPropertyType["Bool"] = 16] = "Bool";
        ReportBuilderPropertyType[ReportBuilderPropertyType["Guid"] = 32] = "Guid";
        ReportBuilderPropertyType[ReportBuilderPropertyType["DateTime"] = 64] = "DateTime";
        ReportBuilderPropertyType[ReportBuilderPropertyType["DateTimeOffset"] = 128] = "DateTimeOffset";
        /** Metadata Repository */
        ReportBuilderPropertyType[ReportBuilderPropertyType["MetadataText"] = 256] = "MetadataText";
        ReportBuilderPropertyType[ReportBuilderPropertyType["MetadataBool"] = 512] = "MetadataBool";
        ReportBuilderPropertyType[ReportBuilderPropertyType["MetadataDateTime"] = 1024] = "MetadataDateTime";
        ReportBuilderPropertyType[ReportBuilderPropertyType["MetadataNumber"] = 2048] = "MetadataNumber";
        ReportBuilderPropertyType[ReportBuilderPropertyType["MetadataEnum"] = 4096] = "MetadataEnum";
        ReportBuilderPropertyType[ReportBuilderPropertyType["MetadataDiscussion"] = 8192] = "MetadataDiscussion";
        /** */
        /** Commons */
        ReportBuilderPropertyType[ReportBuilderPropertyType["Contact"] = 16384] = "Contact";
        ReportBuilderPropertyType[ReportBuilderPropertyType["Container"] = 32768] = "Container";
        ReportBuilderPropertyType[ReportBuilderPropertyType["Category"] = 65536] = "Category";
        /** */
    })(ReportBuilderPropertyType || (ReportBuilderPropertyType = {}));
    return ReportBuilderPropertyType;
});
//# sourceMappingURL=ReportBuilderPropertyType.js.map
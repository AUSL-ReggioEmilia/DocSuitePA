define(["require", "exports"], function (require, exports) {
    var MetadataFinderType;
    (function (MetadataFinderType) {
        MetadataFinderType[MetadataFinderType["Text"] = 0] = "Text";
        MetadataFinderType[MetadataFinderType["Number"] = 1] = "Number";
        MetadataFinderType[MetadataFinderType["Date"] = 2] = "Date";
        MetadataFinderType[MetadataFinderType["Bool"] = 3] = "Bool";
        MetadataFinderType[MetadataFinderType["Enum"] = 4] = "Enum";
        MetadataFinderType[MetadataFinderType["Discussion"] = 5] = "Discussion";
        MetadataFinderType[MetadataFinderType["Contact"] = 6] = "Contact";
    })(MetadataFinderType || (MetadataFinderType = {}));
    return MetadataFinderType;
});
//# sourceMappingURL=MetadataFinderType.js.map
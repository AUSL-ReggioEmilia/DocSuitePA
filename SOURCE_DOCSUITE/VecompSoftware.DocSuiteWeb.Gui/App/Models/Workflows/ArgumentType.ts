enum ArgumentType {
        Json = 1,
        RelationGuid = Json*2,
        RelationInt = RelationGuid*2,
        PropertyString = RelationInt*2,
        PropertyInt = PropertyString*2,
        PropertyDate = PropertyInt*2,
        PropertyDouble = PropertyDate*2,
        PropertyBoolean = PropertyDouble*2,
        PropertyGuid = PropertyBoolean*2,
}

export = ArgumentType;
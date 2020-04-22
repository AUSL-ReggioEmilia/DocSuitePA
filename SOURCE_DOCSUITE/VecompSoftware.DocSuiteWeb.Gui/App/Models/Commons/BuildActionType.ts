
enum BuildActionType {
    None = 0,
    Build = 1,
    Director = Build * 2,
    Evaluate = Director * 2,
    Synchronize = Evaluate * 2,
    Destroy = Synchronize * 2
}

export = BuildActionType;

enum FascicleType {
    Legacy = -1,
    SubFascicle = 0,
    Procedure = 1,
    Period = Procedure*2,
    Activity = Period*2
}

export = FascicleType
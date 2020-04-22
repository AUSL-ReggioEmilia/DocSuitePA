export enum ContactType {

    Invalid = 0,
    Administration = 1,
    AOO = 2,
    AO = AOO * 2,
    Role = AO * 2,
    Group = Role * 2,
    Sector = Group * 2,
    Citizen = Sector * 2,
    IPA = Citizen * 2,
    CitizenManual = IPA * 2,
    AOOManual = CitizenManual * 2,

}
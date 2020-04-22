export enum DocumentUnitType {

    Invalid = 0,
    Protocol = 1,
    Resolution = 2,
    DocumentSeries = Resolution * 2,
    Archive = DocumentSeries * 2

}
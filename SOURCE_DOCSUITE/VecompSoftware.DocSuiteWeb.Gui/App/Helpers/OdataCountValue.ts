// usefull for queries like
// `$filter=startswith(FolderPath, '${node.FolderPath}') and FolderPath ne '${node.FolderPath}' &$count=true&$top=0`;
class OdataCountValue {
    private _count: number;

    public get Count(): number {
        return this._count;
    }

    constructor(source: any) {
        if (source === null || source === undefined) {
            throw new Error('OdataCountValue requires an odata response');
        }

        if (!source.hasOwnProperty('@odata.count')) {
            throw new Error('OdataCountValue requires an odata response with a $count query');
        }

        this._count = source['@odata.count'];
    }
}

export = OdataCountValue;
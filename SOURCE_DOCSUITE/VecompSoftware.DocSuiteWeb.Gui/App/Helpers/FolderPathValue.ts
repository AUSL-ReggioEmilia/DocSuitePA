// TODO: this class needs refactoring
// The method GetTopFolder will return the first, non root, path. E.G. From /1/2/3 -> /1/
// We should refactor this to accomodate for root and empty paths and GetTopFolder should be GetLevel(1)
class FolderPathValue {
    private static path_delimiter = '/';
    // pattern strictly to check /<number>/
    private static pattern_root = /^(\/[0-9]+\/){1}$/;
    // pattern strictly to check /<number>/<number>/.../
    private static pattern_path = /^(\/[0-9]+)+\/$/;

    public get Path(): string {
        return this.path;
    }

    constructor(private path: string) {
        if (path === null || path === undefined) {
            throw new Error('The path must be defined');
        }
        this.NormalizePath();
        if (this.path !== FolderPathValue.path_delimiter) {
            if (!new RegExp(FolderPathValue.pattern_path).test(this.path)) {
                throw new Error('The path is not valid');
            }
        }
    }

    private NormalizePath() {
        if (!this.path.endsWith(FolderPathValue.path_delimiter)) {
            this.path += FolderPathValue.path_delimiter;
        }
        if (!this.path.startsWith(FolderPathValue.path_delimiter)) {
            this.path = FolderPathValue.path_delimiter + this.path;
        }
    }

    public GetTopFolder(): string {
        let paths = this.path.split(FolderPathValue.path_delimiter);
        return `${FolderPathValue.path_delimiter}${paths[1]}${FolderPathValue.path_delimiter}`
    }

    public IsRoot(): boolean {
        return this.path === FolderPathValue.path_delimiter;
    }
}

export = FolderPathValue;
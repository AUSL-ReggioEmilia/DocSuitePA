interface IClientStorage {
    Set(key: string, value: string): void;
    Get(key: string): string | null;
    Remove(key: string): void;
}

export = IClientStorage;
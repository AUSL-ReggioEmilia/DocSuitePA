import IClientStorage = require("App/Core/WorkflowStorage/IClientStorage");

class ClientSessionStorage implements IClientStorage {

    /**
     * Adds a key value pair to the session storage
     * @param key The key of the session item to store
     * @param value The value of the session item to store
     */
    public Set(key: string, value: string): void {
        if (!sessionStorage) {
            throw "This client does not support session storage";
        }
        sessionStorage.setItem(key, value);
    }

    /**
     * Returns an item from the session storage having the provided key
     * @param key The key of the item to get from session storage
     */
    public Get(key: string): string | null {
        if (!sessionStorage) {
            throw "This client does not support session storage";
        }
        return sessionStorage.getItem(key);
    }

    /**
     * Remove an item from session storage having the provided key
     * @param key The key of the item to remove from session storage
     */
    public Remove(key: string): void {
        if (!sessionStorage) {
            throw "This client does not support session storage";
        }
        sessionStorage.removeItem(key);
    }
}

export = ClientSessionStorage;
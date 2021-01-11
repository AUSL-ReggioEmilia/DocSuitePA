import IClientStorage = require("App/Core/WorkflowStorage/IClientStorage");

class ClientLocalStorage implements IClientStorage {

    /**
     * Adds a key value pair to the local storage
     * @param key The key of the local storage item to store
     * @param value The value of the local storage item to store
     */
    public Set(key: string, value: string): void {
        if (!localStorage) {
            throw "This client does not support local storage";
        }
        localStorage.setItem(key, value);
    }

    /**
     * Returns an item from the local storage having the provided key
     * @param key The key of the item to get from local storage
     */
    public Get(key: string): string | null {
        if (!localStorage) {
            throw "This client does not support local storage";
        }
        return localStorage.getItem(key);
    }

    /**
     * Remove an item from local storage having the provided key
     * @param key The key of the item to remove from local storage
     */
    public Remove(key: string): void {
        if (!localStorage) {
            throw "This client does not support local storage";
        }
        localStorage.removeItem(key);
    }
}

export = ClientLocalStorage;
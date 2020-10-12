define(["require", "exports"], function (require, exports) {
    var ClientSessionStorage = /** @class */ (function () {
        function ClientSessionStorage() {
        }
        /**
         * Adds a key value pair to the session storage
         * @param key The key of the session item to store
         * @param value The value of the session item to store
         */
        ClientSessionStorage.prototype.Set = function (key, value) {
            if (!sessionStorage) {
                throw "This client does not support session storage";
            }
            sessionStorage.setItem(key, value);
        };
        /**
         * Returns an item from the session storage having the provided key
         * @param key The key of the item to get from session storage
         */
        ClientSessionStorage.prototype.Get = function (key) {
            if (!sessionStorage) {
                throw "This client does not support session storage";
            }
            return sessionStorage.getItem(key);
        };
        /**
         * Remove an item from session storage having the provided key
         * @param key The key of the item to remove from session storage
         */
        ClientSessionStorage.prototype.Remove = function (key) {
            if (!sessionStorage) {
                throw "This client does not support session storage";
            }
            sessionStorage.removeItem(key);
        };
        return ClientSessionStorage;
    }());
    return ClientSessionStorage;
});
//# sourceMappingURL=ClientSessionStorage.js.map
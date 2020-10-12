define(["require", "exports"], function (require, exports) {
    var ClientLocalStorage = /** @class */ (function () {
        function ClientLocalStorage() {
        }
        /**
         * Adds a key value pair to the local storage
         * @param key The key of the local storage item to store
         * @param value The value of the local storage item to store
         */
        ClientLocalStorage.prototype.Set = function (key, value) {
            if (!localStorage) {
                throw "This client does not support local storage";
            }
            localStorage.setItem(key, value);
        };
        /**
         * Returns an item from the local storage having the provided key
         * @param key The key of the item to get from local storage
         */
        ClientLocalStorage.prototype.Get = function (key) {
            if (!localStorage) {
                throw "This client does not support local storage";
            }
            return localStorage.getItem(key);
        };
        /**
         * Remove an item from local storage having the provided key
         * @param key The key of the item to remove from local storage
         */
        ClientLocalStorage.prototype.Remove = function (key) {
            if (!localStorage) {
                throw "This client does not support local storage";
            }
            localStorage.removeItem(key);
        };
        return ClientLocalStorage;
    }());
    return ClientLocalStorage;
});
//# sourceMappingURL=ClientLocalStorage.js.map
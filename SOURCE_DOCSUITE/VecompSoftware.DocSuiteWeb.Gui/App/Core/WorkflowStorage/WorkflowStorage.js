define(["require", "exports", "App/Core/WorkflowStorage/ClientLocalStorage", "App/Core/WorkflowStorage/ClientSessionStorage"], function (require, exports, ClientLocalStorage, ClientSessionStorage) {
    var WorkflowStorage = /** @class */ (function () {
        function WorkflowStorage() {
            this.IsValid = true;
            if (!!localStorage) {
                this.clientStorage = new ClientLocalStorage();
            }
            else if (!!sessionStorage) {
                this.clientStorage = new ClientSessionStorage();
            }
            else {
                this.IsValid = false;
            }
        }
        /**
         * If storage is not valid,the flag IsValid is set and checked in UDSInvoisesUpload. If for some reason (dev mistake)
         * a method is called this will throw a console exception
         **/
        WorkflowStorage.prototype.EnsureStorage = function () {
            if (this.IsValid === false) {
                throw "Cannot use Workflow storage because the client does not support local or session storage";
            }
        };
        WorkflowStorage.prototype.HasKey = function () {
            this.EnsureStorage();
            return !!this.clientStorage.Get(WorkflowStorage.STORAGE_CORRELATIONID);
        };
        /**
         * Stores the correlationId and the chainId locally.
         * @param correlationId The correlationId to store
         * @param chainId The chainId to store
         */
        WorkflowStorage.prototype.Set = function (correlationId, chainId) {
            this.EnsureStorage();
            //just store the correlation id to mark it as active
            this.clientStorage.Set(WorkflowStorage.STORAGE_CORRELATIONID, correlationId);
            this.clientStorage.Set(WorkflowStorage.STORAGE_CHAINID, chainId);
        };
        WorkflowStorage.prototype.GetCorrelationId = function () {
            this.EnsureStorage();
            return this.clientStorage.Get(WorkflowStorage.STORAGE_CORRELATIONID);
        };
        WorkflowStorage.prototype.GetCorrelatedChainId = function () {
            this.EnsureStorage();
            return this.clientStorage.Get(WorkflowStorage.STORAGE_CHAINID);
        };
        WorkflowStorage.prototype.Unset = function () {
            this.EnsureStorage();
            this.clientStorage.Remove(WorkflowStorage.STORAGE_CORRELATIONID);
            this.clientStorage.Remove(WorkflowStorage.STORAGE_CHAINID);
        };
        WorkflowStorage.STORAGE_CORRELATIONID = "WORKFLOW_RUNNING_CORRELATIONID";
        WorkflowStorage.STORAGE_CHAINID = "WORKFLOW_RUNNING_CHAINID";
        return WorkflowStorage;
    }());
    return WorkflowStorage;
});
//# sourceMappingURL=WorkflowStorage.js.map
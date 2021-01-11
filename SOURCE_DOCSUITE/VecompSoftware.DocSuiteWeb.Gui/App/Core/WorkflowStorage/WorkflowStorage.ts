import IClientStorage = require("App/Core/WorkflowStorage/IClientStorage");
import ClientLocalStorage = require("App/Core/WorkflowStorage/ClientLocalStorage");
import ClientSessionStorage = require("App/Core/WorkflowStorage/ClientSessionStorage");

class WorkflowStorage {

    private clientStorage: IClientStorage;
    private static STORAGE_CORRELATIONID: string = "WORKFLOW_RUNNING_CORRELATIONID";
    private static STORAGE_CHAINID: string = "WORKFLOW_RUNNING_CHAINID";
    public IsValid: boolean;

    constructor() {
        this.IsValid = true;
        if (!!localStorage) {
            this.clientStorage = new ClientLocalStorage();
        } else if (!!sessionStorage) {
            this.clientStorage = new ClientSessionStorage();
        } else {
            this.IsValid = false;
        }
    }

    /**
     * If storage is not valid,the flag IsValid is set and checked in UDSInvoisesUpload. If for some reason (dev mistake)
     * a method is called this will throw a console exception
     **/
    private EnsureStorage(): void {
        if (this.IsValid === false) {
            throw "Cannot use Workflow storage because the client does not support local or session storage";
        }
    }

    public HasKey(): boolean {
        this.EnsureStorage();

        return !!this.clientStorage.Get(WorkflowStorage.STORAGE_CORRELATIONID);
    }

    /**
     * Stores the correlationId and the chainId locally.
     * @param correlationId The correlationId to store
     * @param chainId The chainId to store
     */
    public Set(correlationId: string, chainId: string): void {
        this.EnsureStorage();

        //just store the correlation id to mark it as active
        this.clientStorage.Set(WorkflowStorage.STORAGE_CORRELATIONID, correlationId);
        this.clientStorage.Set(WorkflowStorage.STORAGE_CHAINID, chainId);
    }

    public GetCorrelationId(): string {
        this.EnsureStorage();

        return this.clientStorage.Get(WorkflowStorage.STORAGE_CORRELATIONID);
    }

    public GetCorrelatedChainId(): string {
        this.EnsureStorage();

        return this.clientStorage.Get(WorkflowStorage.STORAGE_CHAINID);
    }

    public Unset() {
        this.EnsureStorage();

        this.clientStorage.Remove(WorkflowStorage.STORAGE_CORRELATIONID);
        this.clientStorage.Remove(WorkflowStorage.STORAGE_CHAINID);
    }
}

export = WorkflowStorage
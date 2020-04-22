define(["require", "exports"], function (require, exports) {
    var InvoiceStatusEnum;
    (function (InvoiceStatusEnum) {
        InvoiceStatusEnum[InvoiceStatusEnum["None"] = 0] = "None";
        InvoiceStatusEnum[InvoiceStatusEnum["PAInvoiceSent"] = 1] = "PAInvoiceSent";
        InvoiceStatusEnum[InvoiceStatusEnum["PAInvoiceNotified"] = 2] = "PAInvoiceNotified";
        InvoiceStatusEnum[InvoiceStatusEnum["PAInvoiceAccepted"] = 3] = "PAInvoiceAccepted";
        InvoiceStatusEnum[InvoiceStatusEnum["PAInvoiceSdiRefused"] = 4] = "PAInvoiceSdiRefused";
        InvoiceStatusEnum[InvoiceStatusEnum["PAInvoiceRefused"] = 5] = "PAInvoiceRefused";
        InvoiceStatusEnum[InvoiceStatusEnum["InvoiceWorkflowStarted"] = 6] = "InvoiceWorkflowStarted";
        InvoiceStatusEnum[InvoiceStatusEnum["InvoiceWorkflowCompleted"] = 7] = "InvoiceWorkflowCompleted";
        InvoiceStatusEnum[InvoiceStatusEnum["InvoiceWorkflowError"] = 8] = "InvoiceWorkflowError";
        InvoiceStatusEnum[InvoiceStatusEnum["InvoiceSignRequired"] = 9] = "InvoiceSignRequired";
        InvoiceStatusEnum[InvoiceStatusEnum["InvoiceLookingMetadata"] = 10] = "InvoiceLookingMetadata";
        InvoiceStatusEnum[InvoiceStatusEnum["InvoiceAccounted"] = 11] = "InvoiceAccounted";
    })(InvoiceStatusEnum || (InvoiceStatusEnum = {}));
    return InvoiceStatusEnum;
});
//# sourceMappingURL=InvoiceStatusEnum.js.map
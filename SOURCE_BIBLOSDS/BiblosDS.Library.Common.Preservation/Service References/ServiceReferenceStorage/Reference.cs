﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BiblosDS.Library.Common.Preservation.ServiceReferenceStorage {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServiceReferenceStorage.IServiceDocumentStorage")]
    public interface IServiceDocumentStorage {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServiceDocumentStorage/AddDocument", ReplyAction="http://tempuri.org/IServiceDocumentStorage/AddDocumentResponse")]
        System.Guid AddDocument(BiblosDS.Library.Common.Objects.Document Document);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IServiceDocumentStorage/AddDocument", ReplyAction="http://tempuri.org/IServiceDocumentStorage/AddDocumentResponse")]
        System.IAsyncResult BeginAddDocument(BiblosDS.Library.Common.Objects.Document Document, System.AsyncCallback callback, object asyncState);
        
        System.Guid EndAddDocument(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServiceDocumentStorage/GetDocument", ReplyAction="http://tempuri.org/IServiceDocumentStorage/GetDocumentResponse")]
        BiblosDS.Library.Common.Objects.Document GetDocument(BiblosDS.Library.Common.Objects.Document Document);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IServiceDocumentStorage/GetDocument", ReplyAction="http://tempuri.org/IServiceDocumentStorage/GetDocumentResponse")]
        System.IAsyncResult BeginGetDocument(BiblosDS.Library.Common.Objects.Document Document, System.AsyncCallback callback, object asyncState);
        
        BiblosDS.Library.Common.Objects.Document EndGetDocument(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServiceDocumentStorage/DeleteDocument", ReplyAction="http://tempuri.org/IServiceDocumentStorage/DeleteDocumentResponse")]
        void DeleteDocument(System.Guid IdDocument);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IServiceDocumentStorage/DeleteDocument", ReplyAction="http://tempuri.org/IServiceDocumentStorage/DeleteDocumentResponse")]
        System.IAsyncResult BeginDeleteDocument(System.Guid IdDocument, System.AsyncCallback callback, object asyncState);
        
        void EndDeleteDocument(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServiceDocumentStorage/RestoreAttribute", ReplyAction="http://tempuri.org/IServiceDocumentStorage/RestoreAttributeResponse")]
        void RestoreAttribute(System.Guid IdDocument);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IServiceDocumentStorage/RestoreAttribute", ReplyAction="http://tempuri.org/IServiceDocumentStorage/RestoreAttributeResponse")]
        System.IAsyncResult BeginRestoreAttribute(System.Guid IdDocument, System.AsyncCallback callback, object asyncState);
        
        void EndRestoreAttribute(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServiceDocumentStorage/WriteAttribute", ReplyAction="http://tempuri.org/IServiceDocumentStorage/WriteAttributeResponse")]
        void WriteAttribute(BiblosDS.Library.Common.Objects.Document Document);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IServiceDocumentStorage/WriteAttribute", ReplyAction="http://tempuri.org/IServiceDocumentStorage/WriteAttributeResponse")]
        System.IAsyncResult BeginWriteAttribute(BiblosDS.Library.Common.Objects.Document Document, System.AsyncCallback callback, object asyncState);
        
        void EndWriteAttribute(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServiceDocumentStorage/CheckIntegrity", ReplyAction="http://tempuri.org/IServiceDocumentStorage/CheckIntegrityResponse")]
        bool CheckIntegrity(BiblosDS.Library.Common.Objects.Document Document);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IServiceDocumentStorage/CheckIntegrity", ReplyAction="http://tempuri.org/IServiceDocumentStorage/CheckIntegrityResponse")]
        System.IAsyncResult BeginCheckIntegrity(BiblosDS.Library.Common.Objects.Document Document, System.AsyncCallback callback, object asyncState);
        
        bool EndCheckIntegrity(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IServiceDocumentStorage/IsAlive", ReplyAction="http://tempuri.org/IServiceDocumentStorage/IsAliveResponse")]
        bool IsAlive();
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/IServiceDocumentStorage/IsAlive", ReplyAction="http://tempuri.org/IServiceDocumentStorage/IsAliveResponse")]
        System.IAsyncResult BeginIsAlive(System.AsyncCallback callback, object asyncState);
        
        bool EndIsAlive(System.IAsyncResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IServiceDocumentStorageChannel : BiblosDS.Library.Common.Preservation.ServiceReferenceStorage.IServiceDocumentStorage, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class AddDocumentCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public AddDocumentCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public System.Guid Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((System.Guid)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GetDocumentCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public GetDocumentCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public BiblosDS.Library.Common.Objects.Document Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((BiblosDS.Library.Common.Objects.Document)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class CheckIntegrityCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public CheckIntegrityCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public bool Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class IsAliveCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public IsAliveCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public bool Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((bool)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ServiceDocumentStorageClient : System.ServiceModel.ClientBase<BiblosDS.Library.Common.Preservation.ServiceReferenceStorage.IServiceDocumentStorage>, BiblosDS.Library.Common.Preservation.ServiceReferenceStorage.IServiceDocumentStorage {
        
        private BeginOperationDelegate onBeginAddDocumentDelegate;
        
        private EndOperationDelegate onEndAddDocumentDelegate;
        
        private System.Threading.SendOrPostCallback onAddDocumentCompletedDelegate;
        
        private BeginOperationDelegate onBeginGetDocumentDelegate;
        
        private EndOperationDelegate onEndGetDocumentDelegate;
        
        private System.Threading.SendOrPostCallback onGetDocumentCompletedDelegate;
        
        private BeginOperationDelegate onBeginDeleteDocumentDelegate;
        
        private EndOperationDelegate onEndDeleteDocumentDelegate;
        
        private System.Threading.SendOrPostCallback onDeleteDocumentCompletedDelegate;
        
        private BeginOperationDelegate onBeginRestoreAttributeDelegate;
        
        private EndOperationDelegate onEndRestoreAttributeDelegate;
        
        private System.Threading.SendOrPostCallback onRestoreAttributeCompletedDelegate;
        
        private BeginOperationDelegate onBeginWriteAttributeDelegate;
        
        private EndOperationDelegate onEndWriteAttributeDelegate;
        
        private System.Threading.SendOrPostCallback onWriteAttributeCompletedDelegate;
        
        private BeginOperationDelegate onBeginCheckIntegrityDelegate;
        
        private EndOperationDelegate onEndCheckIntegrityDelegate;
        
        private System.Threading.SendOrPostCallback onCheckIntegrityCompletedDelegate;
        
        private BeginOperationDelegate onBeginIsAliveDelegate;
        
        private EndOperationDelegate onEndIsAliveDelegate;
        
        private System.Threading.SendOrPostCallback onIsAliveCompletedDelegate;
        
        public ServiceDocumentStorageClient() {
        }
        
        public ServiceDocumentStorageClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ServiceDocumentStorageClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServiceDocumentStorageClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServiceDocumentStorageClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public event System.EventHandler<AddDocumentCompletedEventArgs> AddDocumentCompleted;
        
        public event System.EventHandler<GetDocumentCompletedEventArgs> GetDocumentCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> DeleteDocumentCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> RestoreAttributeCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> WriteAttributeCompleted;
        
        public event System.EventHandler<CheckIntegrityCompletedEventArgs> CheckIntegrityCompleted;
        
        public event System.EventHandler<IsAliveCompletedEventArgs> IsAliveCompleted;
        
        public System.Guid AddDocument(BiblosDS.Library.Common.Objects.Document Document) {
            return base.Channel.AddDocument(Document);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginAddDocument(BiblosDS.Library.Common.Objects.Document Document, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginAddDocument(Document, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.Guid EndAddDocument(System.IAsyncResult result) {
            return base.Channel.EndAddDocument(result);
        }
        
        private System.IAsyncResult OnBeginAddDocument(object[] inValues, System.AsyncCallback callback, object asyncState) {
            BiblosDS.Library.Common.Objects.Document Document = ((BiblosDS.Library.Common.Objects.Document)(inValues[0]));
            return this.BeginAddDocument(Document, callback, asyncState);
        }
        
        private object[] OnEndAddDocument(System.IAsyncResult result) {
            System.Guid retVal = this.EndAddDocument(result);
            return new object[] {
                    retVal};
        }
        
        private void OnAddDocumentCompleted(object state) {
            if ((this.AddDocumentCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.AddDocumentCompleted(this, new AddDocumentCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void AddDocumentAsync(BiblosDS.Library.Common.Objects.Document Document) {
            this.AddDocumentAsync(Document, null);
        }
        
        public void AddDocumentAsync(BiblosDS.Library.Common.Objects.Document Document, object userState) {
            if ((this.onBeginAddDocumentDelegate == null)) {
                this.onBeginAddDocumentDelegate = new BeginOperationDelegate(this.OnBeginAddDocument);
            }
            if ((this.onEndAddDocumentDelegate == null)) {
                this.onEndAddDocumentDelegate = new EndOperationDelegate(this.OnEndAddDocument);
            }
            if ((this.onAddDocumentCompletedDelegate == null)) {
                this.onAddDocumentCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnAddDocumentCompleted);
            }
            base.InvokeAsync(this.onBeginAddDocumentDelegate, new object[] {
                        Document}, this.onEndAddDocumentDelegate, this.onAddDocumentCompletedDelegate, userState);
        }
        
        public BiblosDS.Library.Common.Objects.Document GetDocument(BiblosDS.Library.Common.Objects.Document Document) {
            return base.Channel.GetDocument(Document);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginGetDocument(BiblosDS.Library.Common.Objects.Document Document, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginGetDocument(Document, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public BiblosDS.Library.Common.Objects.Document EndGetDocument(System.IAsyncResult result) {
            return base.Channel.EndGetDocument(result);
        }
        
        private System.IAsyncResult OnBeginGetDocument(object[] inValues, System.AsyncCallback callback, object asyncState) {
            BiblosDS.Library.Common.Objects.Document Document = ((BiblosDS.Library.Common.Objects.Document)(inValues[0]));
            return this.BeginGetDocument(Document, callback, asyncState);
        }
        
        private object[] OnEndGetDocument(System.IAsyncResult result) {
            BiblosDS.Library.Common.Objects.Document retVal = this.EndGetDocument(result);
            return new object[] {
                    retVal};
        }
        
        private void OnGetDocumentCompleted(object state) {
            if ((this.GetDocumentCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.GetDocumentCompleted(this, new GetDocumentCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void GetDocumentAsync(BiblosDS.Library.Common.Objects.Document Document) {
            this.GetDocumentAsync(Document, null);
        }
        
        public void GetDocumentAsync(BiblosDS.Library.Common.Objects.Document Document, object userState) {
            if ((this.onBeginGetDocumentDelegate == null)) {
                this.onBeginGetDocumentDelegate = new BeginOperationDelegate(this.OnBeginGetDocument);
            }
            if ((this.onEndGetDocumentDelegate == null)) {
                this.onEndGetDocumentDelegate = new EndOperationDelegate(this.OnEndGetDocument);
            }
            if ((this.onGetDocumentCompletedDelegate == null)) {
                this.onGetDocumentCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnGetDocumentCompleted);
            }
            base.InvokeAsync(this.onBeginGetDocumentDelegate, new object[] {
                        Document}, this.onEndGetDocumentDelegate, this.onGetDocumentCompletedDelegate, userState);
        }
        
        public void DeleteDocument(System.Guid IdDocument) {
            base.Channel.DeleteDocument(IdDocument);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginDeleteDocument(System.Guid IdDocument, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginDeleteDocument(IdDocument, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public void EndDeleteDocument(System.IAsyncResult result) {
            base.Channel.EndDeleteDocument(result);
        }
        
        private System.IAsyncResult OnBeginDeleteDocument(object[] inValues, System.AsyncCallback callback, object asyncState) {
            System.Guid IdDocument = ((System.Guid)(inValues[0]));
            return this.BeginDeleteDocument(IdDocument, callback, asyncState);
        }
        
        private object[] OnEndDeleteDocument(System.IAsyncResult result) {
            this.EndDeleteDocument(result);
            return null;
        }
        
        private void OnDeleteDocumentCompleted(object state) {
            if ((this.DeleteDocumentCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.DeleteDocumentCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void DeleteDocumentAsync(System.Guid IdDocument) {
            this.DeleteDocumentAsync(IdDocument, null);
        }
        
        public void DeleteDocumentAsync(System.Guid IdDocument, object userState) {
            if ((this.onBeginDeleteDocumentDelegate == null)) {
                this.onBeginDeleteDocumentDelegate = new BeginOperationDelegate(this.OnBeginDeleteDocument);
            }
            if ((this.onEndDeleteDocumentDelegate == null)) {
                this.onEndDeleteDocumentDelegate = new EndOperationDelegate(this.OnEndDeleteDocument);
            }
            if ((this.onDeleteDocumentCompletedDelegate == null)) {
                this.onDeleteDocumentCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnDeleteDocumentCompleted);
            }
            base.InvokeAsync(this.onBeginDeleteDocumentDelegate, new object[] {
                        IdDocument}, this.onEndDeleteDocumentDelegate, this.onDeleteDocumentCompletedDelegate, userState);
        }
        
        public void RestoreAttribute(System.Guid IdDocument) {
            base.Channel.RestoreAttribute(IdDocument);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginRestoreAttribute(System.Guid IdDocument, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginRestoreAttribute(IdDocument, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public void EndRestoreAttribute(System.IAsyncResult result) {
            base.Channel.EndRestoreAttribute(result);
        }
        
        private System.IAsyncResult OnBeginRestoreAttribute(object[] inValues, System.AsyncCallback callback, object asyncState) {
            System.Guid IdDocument = ((System.Guid)(inValues[0]));
            return this.BeginRestoreAttribute(IdDocument, callback, asyncState);
        }
        
        private object[] OnEndRestoreAttribute(System.IAsyncResult result) {
            this.EndRestoreAttribute(result);
            return null;
        }
        
        private void OnRestoreAttributeCompleted(object state) {
            if ((this.RestoreAttributeCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.RestoreAttributeCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void RestoreAttributeAsync(System.Guid IdDocument) {
            this.RestoreAttributeAsync(IdDocument, null);
        }
        
        public void RestoreAttributeAsync(System.Guid IdDocument, object userState) {
            if ((this.onBeginRestoreAttributeDelegate == null)) {
                this.onBeginRestoreAttributeDelegate = new BeginOperationDelegate(this.OnBeginRestoreAttribute);
            }
            if ((this.onEndRestoreAttributeDelegate == null)) {
                this.onEndRestoreAttributeDelegate = new EndOperationDelegate(this.OnEndRestoreAttribute);
            }
            if ((this.onRestoreAttributeCompletedDelegate == null)) {
                this.onRestoreAttributeCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnRestoreAttributeCompleted);
            }
            base.InvokeAsync(this.onBeginRestoreAttributeDelegate, new object[] {
                        IdDocument}, this.onEndRestoreAttributeDelegate, this.onRestoreAttributeCompletedDelegate, userState);
        }
        
        public void WriteAttribute(BiblosDS.Library.Common.Objects.Document Document) {
            base.Channel.WriteAttribute(Document);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginWriteAttribute(BiblosDS.Library.Common.Objects.Document Document, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginWriteAttribute(Document, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public void EndWriteAttribute(System.IAsyncResult result) {
            base.Channel.EndWriteAttribute(result);
        }
        
        private System.IAsyncResult OnBeginWriteAttribute(object[] inValues, System.AsyncCallback callback, object asyncState) {
            BiblosDS.Library.Common.Objects.Document Document = ((BiblosDS.Library.Common.Objects.Document)(inValues[0]));
            return this.BeginWriteAttribute(Document, callback, asyncState);
        }
        
        private object[] OnEndWriteAttribute(System.IAsyncResult result) {
            this.EndWriteAttribute(result);
            return null;
        }
        
        private void OnWriteAttributeCompleted(object state) {
            if ((this.WriteAttributeCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.WriteAttributeCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void WriteAttributeAsync(BiblosDS.Library.Common.Objects.Document Document) {
            this.WriteAttributeAsync(Document, null);
        }
        
        public void WriteAttributeAsync(BiblosDS.Library.Common.Objects.Document Document, object userState) {
            if ((this.onBeginWriteAttributeDelegate == null)) {
                this.onBeginWriteAttributeDelegate = new BeginOperationDelegate(this.OnBeginWriteAttribute);
            }
            if ((this.onEndWriteAttributeDelegate == null)) {
                this.onEndWriteAttributeDelegate = new EndOperationDelegate(this.OnEndWriteAttribute);
            }
            if ((this.onWriteAttributeCompletedDelegate == null)) {
                this.onWriteAttributeCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnWriteAttributeCompleted);
            }
            base.InvokeAsync(this.onBeginWriteAttributeDelegate, new object[] {
                        Document}, this.onEndWriteAttributeDelegate, this.onWriteAttributeCompletedDelegate, userState);
        }
        
        public bool CheckIntegrity(BiblosDS.Library.Common.Objects.Document Document) {
            return base.Channel.CheckIntegrity(Document);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginCheckIntegrity(BiblosDS.Library.Common.Objects.Document Document, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginCheckIntegrity(Document, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public bool EndCheckIntegrity(System.IAsyncResult result) {
            return base.Channel.EndCheckIntegrity(result);
        }
        
        private System.IAsyncResult OnBeginCheckIntegrity(object[] inValues, System.AsyncCallback callback, object asyncState) {
            BiblosDS.Library.Common.Objects.Document Document = ((BiblosDS.Library.Common.Objects.Document)(inValues[0]));
            return this.BeginCheckIntegrity(Document, callback, asyncState);
        }
        
        private object[] OnEndCheckIntegrity(System.IAsyncResult result) {
            bool retVal = this.EndCheckIntegrity(result);
            return new object[] {
                    retVal};
        }
        
        private void OnCheckIntegrityCompleted(object state) {
            if ((this.CheckIntegrityCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.CheckIntegrityCompleted(this, new CheckIntegrityCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void CheckIntegrityAsync(BiblosDS.Library.Common.Objects.Document Document) {
            this.CheckIntegrityAsync(Document, null);
        }
        
        public void CheckIntegrityAsync(BiblosDS.Library.Common.Objects.Document Document, object userState) {
            if ((this.onBeginCheckIntegrityDelegate == null)) {
                this.onBeginCheckIntegrityDelegate = new BeginOperationDelegate(this.OnBeginCheckIntegrity);
            }
            if ((this.onEndCheckIntegrityDelegate == null)) {
                this.onEndCheckIntegrityDelegate = new EndOperationDelegate(this.OnEndCheckIntegrity);
            }
            if ((this.onCheckIntegrityCompletedDelegate == null)) {
                this.onCheckIntegrityCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnCheckIntegrityCompleted);
            }
            base.InvokeAsync(this.onBeginCheckIntegrityDelegate, new object[] {
                        Document}, this.onEndCheckIntegrityDelegate, this.onCheckIntegrityCompletedDelegate, userState);
        }
        
        public bool IsAlive() {
            return base.Channel.IsAlive();
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginIsAlive(System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginIsAlive(callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public bool EndIsAlive(System.IAsyncResult result) {
            return base.Channel.EndIsAlive(result);
        }
        
        private System.IAsyncResult OnBeginIsAlive(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return this.BeginIsAlive(callback, asyncState);
        }
        
        private object[] OnEndIsAlive(System.IAsyncResult result) {
            bool retVal = this.EndIsAlive(result);
            return new object[] {
                    retVal};
        }
        
        private void OnIsAliveCompleted(object state) {
            if ((this.IsAliveCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.IsAliveCompleted(this, new IsAliveCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void IsAliveAsync() {
            this.IsAliveAsync(null);
        }
        
        public void IsAliveAsync(object userState) {
            if ((this.onBeginIsAliveDelegate == null)) {
                this.onBeginIsAliveDelegate = new BeginOperationDelegate(this.OnBeginIsAlive);
            }
            if ((this.onEndIsAliveDelegate == null)) {
                this.onEndIsAliveDelegate = new EndOperationDelegate(this.OnEndIsAlive);
            }
            if ((this.onIsAliveCompletedDelegate == null)) {
                this.onIsAliveCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnIsAliveCompleted);
            }
            base.InvokeAsync(this.onBeginIsAliveDelegate, null, this.onEndIsAliveDelegate, this.onIsAliveCompletedDelegate, userState);
        }
    }
}

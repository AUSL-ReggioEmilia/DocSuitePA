using Microsoft.Practices.Unity;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Common.Test;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Data.EF;
using VecompSoftware.DocSuiteWeb.Service.EF.Test.FakeRepositories;
using VecompSoftware.DocSuiteWeb.Service.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Messages;

namespace VecompSoftware.DocSuiteWeb.Service.EF.Test
{
    [TestClass]
    public class UnitTestMessageServiceException
    {
        #region [ Fields ]

        private bool _disposed = false;
        private IDisposable _shimContext;
        private IUnityContainer _unityContainer;
        #endregion

        #region [ Constructor ]

        public UnitTestMessageServiceException() { }
        #endregion

        #region [ Dispose ]

        ~UnitTestMessageServiceException()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _shimContext.Dispose();
                    _unityContainer.Dispose();
                }

                _shimContext = null;
                _disposed = true;
            }
        }
        #endregion

        [TestInitialize]
        public void TestInitialize()
        {
            _shimContext = ShimsContext.Create();
            _unityContainer = new UnityContainer();
            _unityContainer.RegisterType<IDataUnitOfWork, DataUnitOfWork>();
            _unityContainer.RegisterType<IDSWDataContext, DswFakeDbContext>();
            _unityContainer.RegisterType<ILogger, FakeLogger>();
            _unityContainer.RegisterType<IValidatorService, ValidatorService>();
            _unityContainer.RegisterType<IMessageRuleset, MessageRulesetDefinition>();
            _unityContainer.RegisterType<IMessageService, MessageService>();
        }

        [TestMethod,
        TestCategory("VecompSoftware.DocSuiteWeb.Service"),
        TestCategory("Message Insert")]
        public void TestMethod_Insert_NullEntity_Throws()
        {
            IMessageService service = _unityContainer.Resolve<IMessageService>();
            Action act = () => service.Create(null);
            AssertExtension.AssertInnerThrows<NullReferenceException>(act, "Nessuna eccezione di ritorno. Era attesa una NullReferenceException");
        }

        [TestMethod,
        TestCategory("VecompSoftware.DocSuiteWeb.Service"),
        TestCategory("Message Update")]
        public void TestMethod_Update_NullEntity_Throws()
        {
            IMessageService service = _unityContainer.Resolve<IMessageService>();
            Action act = () => service.Update(null);
            AssertExtension.AssertInnerThrows<NullReferenceException>(act, "Nessuna eccezione di ritorno. Era attesa una NullReferenceException");
        }

        [TestMethod,
        TestCategory("VecompSoftware.DocSuiteWeb.Service"),
        TestCategory("Message Delete")]
        public void TestMethod_Delete_NullEntity_Throws()
        {
            IMessageService service = _unityContainer.Resolve<IMessageService>();
            Action act = () => service.Delete(null);
            AssertExtension.AssertInnerThrows<NullReferenceException>(act, "Nessuna eccezione di ritorno. Era attesa una NullReferenceException");
        }

        [TestMethod,
        TestCategory("VecompSoftware.DocSuiteWeb.Service"),
        TestCategory("Message Delete Async")]
        public void TestMethod_DeleteAsync_NullEntity_Throws()
        {
            IMessageService service = _unityContainer.Resolve<IMessageService>();
            Action act = () => service.Delete(null);
            AssertExtension.AssertInnerThrows<NullReferenceException>(act, "Nessuna eccezione di ritorno. Era attesa una NullReferenceException");
        }
    }
}

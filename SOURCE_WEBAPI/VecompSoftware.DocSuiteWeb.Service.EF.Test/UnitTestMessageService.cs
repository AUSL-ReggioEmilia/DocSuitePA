using Microsoft.Practices.Unity;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;
using VecompSoftware.DocSuiteWeb.Common.Loggers;
using VecompSoftware.DocSuiteWeb.Common.Test;
using VecompSoftware.DocSuiteWeb.Data;
using VecompSoftware.DocSuiteWeb.Data.EF;
using VecompSoftware.DocSuiteWeb.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Service.EF.Test.FakeRepositories;
using VecompSoftware.DocSuiteWeb.Service.Entity.Messages;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Messages;

namespace VecompSoftware.DocSuiteWeb.Service.EF.Test
{
    [TestClass]
    public class UnitTestMessageService
    {
        #region [ Fields ]

        private bool _disposed = false;
        private IDisposable _shimContext;
        private IUnityContainer _unityContainer;
        #endregion

        #region [ Constructor ]

        public UnitTestMessageService() { }
        #endregion

        #region [ Dispose ]

        ~UnitTestMessageService()
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
        public async Task TestMethod_Insert_NotThrows()
        {
            IMessageService service = _unityContainer.Resolve<IMessageService>();
            IDataUnitOfWork unitOfWork = _unityContainer.Resolve<IDataUnitOfWork>();

            Message message = CustomElements.CreateMessageModel();

            service.Create(message);
            await unitOfWork.SaveChangesAsync();
            Message messageSaved = service.Queryable().FirstOrDefault(f => f.EntityId == CustomElements.MessageId);

            Assert.IsNotNull(messageSaved);
        }

        [TestMethod,
        TestCategory("VecompSoftware.DocSuiteWeb.Service"),
        TestCategory("Message Update")]
        public async Task TestMethod_Update_NotThrows()
        {
            IMessageService service = _unityContainer.Resolve<IMessageService>();
            IDataUnitOfWork unitOfWork = _unityContainer.Resolve<IDataUnitOfWork>();

            Message message = CustomElements.CreateMessageModel();

            service.Create(message);
            await unitOfWork.SaveChangesAsync();
            Message messageToUpdate = service.Queryable().FirstOrDefault(f => f.EntityId == CustomElements.MessageId);

            messageToUpdate.Status = MessageStatus.Error;
            service.Update(messageToUpdate);
            await unitOfWork.SaveChangesAsync();

            Message messageModified = service.Queryable().FirstOrDefault(f => f.EntityId == CustomElements.MessageId);

            Assert.IsNotNull(messageModified);
            Assert.AreEqual(MessageStatus.Error, messageModified.Status);
        }

        [TestMethod,
        TestCategory("VecompSoftware.DocSuiteWeb.Service"),
        TestCategory("Message Delete")]
        public async Task TestMethod_Delete_NotThrows()
        {
            IMessageService service = _unityContainer.Resolve<IMessageService>();
            IDataUnitOfWork unitOfWork = _unityContainer.Resolve<IDataUnitOfWork>();

            Message message = CustomElements.CreateMessageModel();

            service.Create(message);
            await unitOfWork.SaveChangesAsync();
            Message messageToDelete = service.Queryable().FirstOrDefault(f => f.EntityId == CustomElements.MessageId);

            service.Delete(messageToDelete);
            await unitOfWork.SaveChangesAsync();

            Message messageDeleted = service.Queryable().FirstOrDefault(f => f.EntityId == CustomElements.MessageId);

            Assert.IsNull(messageDeleted);
        }

        [TestMethod,
        TestCategory("VecompSoftware.DocSuiteWeb.Service"),
        TestCategory("Message DeleteAsync")]
        public async Task TestMethod_DeleteAsync_NotThrows()
        {
            IMessageService service = _unityContainer.Resolve<IMessageService>();
            IDataUnitOfWork unitOfWork = _unityContainer.Resolve<IDataUnitOfWork>();

            Message message = CustomElements.CreateMessageModel();

            service.Create(message);
            await unitOfWork.SaveChangesAsync();
            Message messageToDelete = service.Queryable().FirstOrDefault(f => f.EntityId == CustomElements.MessageId);

            await service.DeleteAsync(messageToDelete);
            await unitOfWork.SaveChangesAsync();

            Message messageDeleted = service.Queryable().FirstOrDefault(f => f.EntityId == CustomElements.MessageId);

            Assert.IsNull(messageDeleted);
        }

    }
}

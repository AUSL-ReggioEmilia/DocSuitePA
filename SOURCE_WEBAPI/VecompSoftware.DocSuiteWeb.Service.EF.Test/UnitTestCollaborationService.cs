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
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Service.EF.Test.FakeRepositories;
using VecompSoftware.DocSuiteWeb.Service.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Collaborations;

namespace VecompSoftware.DocSuiteWeb.Service.EF.Test
{
    [TestClass]
    public class UnitTestCollaborationService
    {
        #region [ Fields ]

        private bool _disposed = false;
        private IDisposable _shimContext;
        private IUnityContainer _unityContainer;
        #endregion

        #region [ Constructor ]

        public UnitTestCollaborationService() { }
        #endregion

        #region [ Dispose ]

        ~UnitTestCollaborationService()
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
            _unityContainer.RegisterType<ICollaborationRuleset, CollaborationRuleset>();
            _unityContainer.RegisterType<ICollaborationService, CollaborationService>();
        }

        [TestMethod,
        TestCategory("VecompSoftware.DocSuiteWeb.Service"),
        TestCategory("Collaboration"),
        TestCategory("Insert")]
        public async Task TestMethod_Insert_NotThrows()
        {
            ICollaborationService service = _unityContainer.Resolve<ICollaborationService>();
            IDataUnitOfWork unitOfWork = _unityContainer.Resolve<IDataUnitOfWork>();

            Collaboration collaboration = CustomElements.CreateCollaborationModel();

            collaboration = service.Create(collaboration);
            await unitOfWork.SaveChangesAsync();
            Collaboration collaborationSaved = service.Queryable().FirstOrDefault(f => f.EntityId == CustomElements.CollaborationId);

            Assert.IsNotNull(collaborationSaved);
        }

        [TestMethod,
        TestCategory("VecompSoftware.DocSuiteWeb.Service"),
        TestCategory("Collaboration Update")]
        public async Task TestMethod_Update_NotThrows()
        {
            ICollaborationService service = _unityContainer.Resolve<ICollaborationService>();
            IDataUnitOfWork unitOfWork = _unityContainer.Resolve<IDataUnitOfWork>();

            Collaboration collaboration = CustomElements.CreateCollaborationModel();

            collaboration = service.Create(collaboration);
            await unitOfWork.SaveChangesAsync();
            Collaboration collaborationToUpdate = service.Queryable().FirstOrDefault(f => f.EntityId == CustomElements.CollaborationId);

            collaborationToUpdate.Note = "Unit Test - Modifica";
            service.Update(collaborationToUpdate);
            await unitOfWork.SaveChangesAsync();

            Collaboration collaborationModified = service.Queryable().FirstOrDefault(f => f.EntityId == CustomElements.CollaborationId);

            Assert.IsNotNull(collaborationModified);
            Assert.AreEqual("Unit Test - Modifica", collaborationModified.Note);
        }

        [TestMethod,
        TestCategory("VecompSoftware.DocSuiteWeb.Service"),
        TestCategory("Collaboration Delete")]
        public async Task TestMethod_Delete_NotThrows()
        {
            ICollaborationService service = _unityContainer.Resolve<ICollaborationService>();
            IDataUnitOfWork unitOfWork = _unityContainer.Resolve<IDataUnitOfWork>();

            Collaboration collaboration = CustomElements.CreateCollaborationModel();

            collaboration = service.Create(collaboration);
            await unitOfWork.SaveChangesAsync();
            Collaboration collaborationToDelete = service.Queryable().FirstOrDefault(f => f.EntityId == CustomElements.CollaborationId);

            service.Delete(collaborationToDelete);
            await unitOfWork.SaveChangesAsync();

            Collaboration collaborationDeleted = service.Queryable().FirstOrDefault(f => f.EntityId == CustomElements.CollaborationId);

            Assert.IsNull(collaborationDeleted);
        }

        [TestMethod,
        TestCategory("VecompSoftware.DocSuiteWeb.Service"),
        TestCategory("Collaboration DeleteAsync")]
        public async Task TestMethod_DeleteAsync_NotThrows()
        {
            ICollaborationService service = _unityContainer.Resolve<ICollaborationService>();
            IDataUnitOfWork unitOfWork = _unityContainer.Resolve<IDataUnitOfWork>();

            Collaboration collaboration = CustomElements.CreateCollaborationModel();

            service.Create(collaboration);
            await unitOfWork.SaveChangesAsync();
            Collaboration collaborationToDelete = service.Queryable().FirstOrDefault(f => f.EntityId == CustomElements.CollaborationId);

            await service.DeleteAsync(collaborationToDelete);
            await unitOfWork.SaveChangesAsync();

            Collaboration collaborationDeleted = service.Queryable().FirstOrDefault(f => f.EntityId == CustomElements.CollaborationId);

            Assert.IsNull(collaborationDeleted);
        }

    }
}

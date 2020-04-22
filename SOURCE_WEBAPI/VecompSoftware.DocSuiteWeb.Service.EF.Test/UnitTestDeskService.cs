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
using VecompSoftware.DocSuiteWeb.Entity.Desks;
using VecompSoftware.DocSuiteWeb.Service.EF.Test.FakeRepositories;
using VecompSoftware.DocSuiteWeb.Service.Entity.Desks;
using VecompSoftware.DocSuiteWeb.Validation;
using VecompSoftware.DocSuiteWeb.Validation.RulesetDefinitions.Entities.Desks;

namespace VecompSoftware.DocSuiteWeb.Service.EF.Test
{
    [TestClass]
    public class UnitTestDeskService
    {
        #region [ Fields ]

        private bool _disposed = false;
        private IDisposable _shimContext;
        private IUnityContainer _unityContainer;
        #endregion

        #region [ Constructor ]

        public UnitTestDeskService() { }
        #endregion

        #region [ Dispose ]

        ~UnitTestDeskService()
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
            _unityContainer.RegisterType<IDeskRuleset, DeskRulesetDefinition>();
            _unityContainer.RegisterType<IDeskService, DeskService>();
        }

        [TestMethod,
        TestCategory("VecompSoftware.DocSuiteWeb.Service"),
        TestCategory("Desk"),
        TestCategory("Insert")]
        public async Task TestMethod_Insert_NotThrows()
        {
            IDeskService service = _unityContainer.Resolve<IDeskService>();
            IDataUnitOfWork unitOfWork = _unityContainer.Resolve<IDataUnitOfWork>();

            Desk desk = CustomElements.CreateDeskModel();

            service.Create(desk);
            await unitOfWork.SaveChangesAsync();
            Desk deskSaved = service.Queryable().FirstOrDefault(f => f.UniqueId == CustomElements.DeskId);

            Assert.IsNotNull(deskSaved);
        }

        [TestMethod,
        TestCategory("VecompSoftware.DocSuiteWeb.Service"),
        TestCategory("Desk"),
        TestCategory("Update")]
        public async Task TestMethod_Update_NotThrows()
        {
            IDeskService service = _unityContainer.Resolve<IDeskService>();
            IDataUnitOfWork unitOfWork = _unityContainer.Resolve<IDataUnitOfWork>();

            Desk desk = CustomElements.CreateDeskModel();

            service.Create(desk);
            await unitOfWork.SaveChangesAsync();
            Desk deskToUpdate = service.Queryable().FirstOrDefault(f => f.UniqueId == CustomElements.DeskId);

            deskToUpdate.Description = "Unit Test - Modifica";
            service.Update(deskToUpdate);
            await unitOfWork.SaveChangesAsync();

            Desk deskModified = service.Queryable().FirstOrDefault(f => f.UniqueId == CustomElements.DeskId);

            Assert.IsNotNull(deskModified);
            Assert.AreEqual("Unit Test - Modifica", deskModified.Description);
        }

        [TestMethod,
        TestCategory("VecompSoftware.DocSuiteWeb.Service"),
        TestCategory("Desk"),
        TestCategory("Delete")]
        public async Task TestMethod_Delete_NotThrows()
        {
            IDeskService service = _unityContainer.Resolve<IDeskService>();
            IDataUnitOfWork unitOfWork = _unityContainer.Resolve<IDataUnitOfWork>();

            Desk desk = CustomElements.CreateDeskModel();

            await service.CreateAsync(desk);
            await unitOfWork.SaveChangesAsync();
            Desk deskToDelete = service.Queryable().FirstOrDefault(f => f.UniqueId == CustomElements.DeskId);

            await service.DeleteAsync(deskToDelete);
            await unitOfWork.SaveChangesAsync();

            Desk deskDeleted = service.Queryable().FirstOrDefault(f => f.UniqueId == CustomElements.DeskId);

            Assert.IsNull(deskDeleted);
        }

        [TestMethod,
        TestCategory("VecompSoftware.DocSuiteWeb.Service"),
        TestCategory("Desk"),
        TestCategory("DeleteAsync")]
        public async Task TestMethod_DeleteAsync_NotThrows()
        {
            IDeskService service = _unityContainer.Resolve<IDeskService>();
            IDataUnitOfWork unitOfWork = _unityContainer.Resolve<IDataUnitOfWork>();

            Desk desk = CustomElements.CreateDeskModel();

            service.Create(desk);
            await unitOfWork.SaveChangesAsync();
            Desk deskToDelete = service.Queryable().FirstOrDefault(f => f.UniqueId == CustomElements.DeskId);

            await service.DeleteAsync(deskToDelete);
            await unitOfWork.SaveChangesAsync();

            Desk deskDeleted = service.Queryable().FirstOrDefault(f => f.UniqueId == CustomElements.DeskId);

            Assert.IsNull(deskDeleted);
        }
    }
}

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BiblosDS.UnitTest.WCF.PreservationSvc;
using System.Threading;
using System.Diagnostics;

namespace BiblosDS.UnitTest.WCF
{
    [TestClass]
    public class Preservation_Test : IServicePreservationCallback
    {
        private const string ARCHIVE_NAME = "X";

        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        public static ServicePreservationClient GetClient(object cbk)
        {
            return new PreservationSvc.ServicePreservationClient(new System.ServiceModel.InstanceContext(cbk));
        }

        [TestMethod]
        public void TestGetPreservationTaskTypesAndPreservationScheduleTaskTypes()
        {
            using (var svc = GetClient(this))
            {
                //9
                var ret = svc.GetPreservationTaskTypesAndPreservationScheduleTaskTypes(new Guid("2a9bc100-c781-4ee2-ac4e-f5cb05ef911d"), ARCHIVE_NAME);
            }
        }

        [TestMethod]
        public void TestGetPreservationTaskGroupsList()
        {
            using (var svc = GetClient(this))
            {
                var lista = svc.GetPreservationTaskGroup(ARCHIVE_NAME);
            }
        }

        [TestMethod]
        public void TestGetPreservationInfo()
        {
            using (var svc = GetClient(this))
            {
                try
                {
                    var ret = svc.GetPreservationInfo("LEXAUSL", new Guid("571fb7c8-ffc9-4373-8306-37f3fe9eeecc"));
                }
                catch (Exception ex)
                {
                    ex.ToString();
                }
            }
        }

        [TestMethod]
        public void TestCreatePreservation()
        {
            using (var svc = GetClient(this))
            {
                bool finished = false;

                svc.CreatePreservationCompleted += (a, ò) =>
                {
                    try
                    {
                        Debugger.Break();
                        var ret = ò.Result;
                        finished = true;
                    }
                    catch (Exception ex)
                    {
                        TestContext.WriteLine(ex.Message);
                    }
                };

                
                svc.InnerChannel.OperationTimeout = new TimeSpan(1, 0, 0);
                svc.InnerDuplexChannel.OperationTimeout = new TimeSpan(1, 0, 0);
                
                Thread.BeginCriticalRegion();

                svc.CreatePreservationAsync("LEXAUSL", @"VAIO\Massimiliano", new Guid("571FB7C8-FFC9-4373-8306-37F3FE9EEECC"), new DateTime(2010, 9, 8, 11, 11, 48), new DateTime(2012, 9, 8, 11, 11, 48), true);

                while (!finished) { }

                Thread.EndCriticalRegion();
            }
        }

        #region IServicePreservationCallback Members

        public void Pulse(string source, string message, int progressPercentage)
        {
        }

        public IAsyncResult BeginPulse(string source, string message, int progressPercentage, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndPulse(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

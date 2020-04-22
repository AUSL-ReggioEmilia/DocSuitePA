using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel;
using BiblosDS.UnitTest.PublicWCF.ServiceReferencePreservation;
using System.IO;

namespace BiblosDS.UnitTest.PublicWCF
{
    [TestClass]
    public class PreservationTest
    {
        [TestMethod]
        public void RemovePendigPreservation()
        {
            using (var client = new PreservationClient())
            {
                client.RemovePendigPreservation(new Guid("3E7F47D7-3EB8-4221-91C9-0515CEAB551E"));               
            }
        }
        
        [TestMethod]
        public void CreateDocumentTest()
        {
            int anno = 2012;
            int startMonth = 1;
            int startDay = 1;
            int addDay = 5;
            string fileName = @"C:\Lavori\Docs\BiblosDS\prima installazione biblosDS2010.txt";
            DateTime selDate = new DateTime(anno, startMonth, startDay);
            int progressivo = 1;
            while (selDate.Year == anno)
            {
                var attributeValues = new BindingList<ServiceReferenceDocument.AttributeValue>();
                attributeValues.Add(new ServiceReferenceDocument.AttributeValue
                {
                    Attribute = new ServiceReferenceDocument.Attribute() { IdAttribute = new Guid("2bc9838f-d458-4245-8960-dcafd64d0a26"), Name = "Anno" },
                    Value = anno
                });
                attributeValues.Add(new ServiceReferenceDocument.AttributeValue
                {
                    Attribute = new ServiceReferenceDocument.Attribute() { IdAttribute = new Guid("0ad2e4f6-fefa-4172-b71d-3ae76f2461fe"), Name = "Numero" },
                    Value = progressivo
                });
                attributeValues.Add(new ServiceReferenceDocument.AttributeValue
                {
                    Attribute = new ServiceReferenceDocument.Attribute() { IdAttribute = new Guid("e2527f16-762d-4e26-a765-5b22efbfe91e"), Name = "Serie" },
                    Value = "IVA"
                });
                attributeValues.Add(new ServiceReferenceDocument.AttributeValue
                {
                    Attribute = new ServiceReferenceDocument.Attribute() { IdAttribute = new Guid("0f78a2eb-8c5f-4d13-b609-5c3c4f0527ff"), Name = "DataDocumento" },
                    Value = selDate
                });               

                ServiceReferenceDocument.DocumentsClient client = new ServiceReferenceDocument.DocumentsClient();
                var doc = client.AddDocumentToChain(
                    new ServiceReferenceDocument.Document
                    {
                        Archive = new ServiceReferenceDocument.Archive()
                        {
                            IdArchive = new Guid("fa7d9ba8-30bb-44ee-bcdc-ec9d98b1b691")
                        },
                        AttributeValues = attributeValues,
                        Name = Path.GetFileName(fileName),
                        Content = new ServiceReferenceDocument.Content() { Blob = File.ReadAllBytes(fileName) }
                    }, null, ServiceReferenceDocument.ContentFormat.Binary);
                client.ConfirmChain(doc.IdDocument);
                selDate = selDate.AddDays(addDay);
                progressivo += 1;
            }                       
        }

        [TestMethod]
        public void CreatePreservationTask()
        {
            using (var client = new ServiceReferencePreservation.PreservationClient())
            {
                var tasks = new BindingList<PreservationTask>();

                var toCreate = new PreservationTask
                {
                    Archive = new Archive { IdArchive = new Guid("9FDC7E5B-BFAA-4DEF-8A78-BF033820C593"), },
                    TaskType = new PreservationTaskType { Type = PreservationTaskTypes.Verify },
                    CorrelatedTasks = new BindingList<PreservationTask>(),
                    EstimatedDate = new DateTime(2013, 2, 1),
                    StartDocumentDate = new DateTime(2012, 1, 1),
                    EndDocumentDate = new DateTime(2012, 12, 31, 23, 59, 59),
                };

                toCreate.CorrelatedTasks.Add(new PreservationTask
                {
                    TaskType = new PreservationTaskType { Type = PreservationTaskTypes.Preservation },
                    EstimatedDate = new DateTime(2013, 3, 1),
                    StartDocumentDate = new DateTime(2012, 1, 1),
                    EndDocumentDate = new DateTime(2012, 12, 31, 23, 59, 59),
                });

                tasks.Add(toCreate);

                tasks = client.CreatePreservationTask(tasks);
            }
        }

        [TestMethod]
        public void GetPreservationTask()
        {
            using (var client = new ServiceReferencePreservation.PreservationClient())
            {
                var archivi = new BindingList<Archive>();
                archivi.Add(new Archive { IdArchive = new Guid("9FDC7E5B-BFAA-4DEF-8A78-BF033820C593") });
                var ret = client.GetPreservationTasksByArchive(archivi, 0, 5);
            }
        }

        [TestMethod]
        public void UpdateTasks()
        {
            using (var client = new PreservationClient())
            {
                var tsk = client.GetPreservationTask(new Guid("0E2B02FD-AA49-408F-85FF-3A4697645FCF"));
                tsk.Enabled = true;
                client.UpdatePreservationTask(new BindingList<PreservationTask>(new[] { tsk }), false);
            }
        }

        [TestMethod]
        public void EnableTaskByPin() 
        {
            using (var client = new PreservationClient())
            {
                var tsk = client.GetPreservationTask(new Guid("3260CA5C-45E9-42EB-A7EA-0A4FBFC4CD5A"));
                client.EnablePreservationTaskByActivationPin(tsk.IdPreservationTask, new Guid("f753cf2e-6baa-4737-a8c1-dc70851e5c79"), 1);
            }
        }
    }
}

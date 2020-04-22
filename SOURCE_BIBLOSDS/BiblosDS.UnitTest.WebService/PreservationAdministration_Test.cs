using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BiblosDS.UnitTest.WebService.PreservationAdministrationSvc;
using BiblosDS.Library.Common.Objects;
using System.ComponentModel;
using System.Configuration;

namespace BiblosDS.UnitTest.WebService
{
    [TestClass]
    public class PreservationAdministration_Test
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

        public static PreservationAdministrationClient GetClient()
        {
            return new PreservationAdministrationClient();
        }

        [TestMethod]
        public void TestDeletePreservationUser()
        {
            using (var svc = GetClient())
            {
                svc.DeletePreservationUser(new Guid("1A581D87-3C25-475E-B57E-BE0BFAE12C3C"));
            }
        }

        [TestMethod]
        public void TestAddPreservationSchedule()
        {
            using (var svc = GetClient())
            {
                var sched = new PreservationSchedule
                {
                    IdPreservationSchedule = Guid.NewGuid(),
                    Name = "Test",
                    ValidWeekDays = "1",
                    Period = "1",
                    FrequencyType = 1,
                    Active = true,
                    PreservationScheduleTaskTypes = new System.ComponentModel.BindingList<PreservationScheduleTaskType>(),
                };

                sched.PreservationScheduleTaskTypes.Add(new PreservationScheduleTaskType
                {
                    TaskType = new PreservationTaskType { IdPreservationTaskType = new Guid("D28D8695-B074-42FE-BD90-6CBC555736D2") },
                    Offset = 0,
                });

                svc.AddPreservationSchedule(sched, ARCHIVE_NAME);
            }
        }

        [TestMethod]
        public void TestAddPreservationAlertType()
        {
            using (var svc = GetClient())
            {
                var toAdd = new PreservationAlertType
                {
                    AlertText = "123stella",
                    Offset = 1,
                    Role = new PreservationRole { IdPreservationRole = new Guid("56787f89-34be-4b99-8558-5795dbcb6594") },
                    TaskTypes = new BindingList<PreservationTaskType>(),
                };

                toAdd.TaskTypes.Add(new PreservationTaskType { IdPreservationTaskType = new Guid("00a6d4ed-915b-4fec-bdd7-21ec90a3c7c0") });

                var ret = svc.AddPreservationAlertType(toAdd, "X");
            }
        }

        [TestMethod]
        public void TestUpdatePreservationUser()
        {
            var utente = new PreservationUser
            {
                IdPreservationUser = new Guid("6f935497-98e1-44d2-8612-bcff76807730"),
                DomainUser = "NO",
                Name = "XXX",
                Surname = "XXX",
                FiscalId = "TNNLCU76L10L781S",
                Address = "NO",
                EMail = "NO",
                Enabled = true,
                ArchiveName = ARCHIVE_NAME,
                UserRoles = new BindingList<PreservationUserRole>(),
            };

            utente.UserRoles.Add(new PreservationUserRole
            {
                PreservationRole = new PreservationRole { IdPreservationRole = new Guid("56787f89-34be-4b99-8558-5795dbcb6594") },
                PreservationUser = new PreservationUser { IdPreservationUser = utente.IdPreservationUser }
            });

            utente.UserRoles.Add(new PreservationUserRole
            {
                PreservationRole = new PreservationRole { IdPreservationRole = new Guid("0eae6a78-bb7b-41e6-bedb-6d2057fcd943") },
                PreservationUser = new PreservationUser { IdPreservationUser = utente.IdPreservationUser }
            });

            utente.UserRoles.Add(new PreservationUserRole
            {
                PreservationRole = new PreservationRole { IdPreservationRole = new Guid("e42ee96f-1fed-4ae8-b733-c92060087ce5") },
                PreservationUser = new PreservationUser { IdPreservationUser = utente.IdPreservationUser }
            });

            using (var svc = GetClient())
            {
                svc.UpdatePreservationUser(utente, ARCHIVE_NAME);
            }
        }

        [TestMethod]
        public void TestAddPreservationUser()
        {
            var utente = new PreservationUser
            {
                DomainUser = "VAIO\\Massimiliano",
                Name = "Michael",
                Surname = "Saguaro",
                FiscalId = "TERIBBILE81",
                Address = "India",
                EMail = "sandokan@malesia.eu",
                Enabled = true,
                ArchiveName = ARCHIVE_NAME,
                UserRoles = new BindingList<PreservationUserRole>(),
            };

            utente.UserRoles.Add(new PreservationUserRole
            {
                PreservationRole = new PreservationRole { IdPreservationRole = new Guid("56787f89-34be-4b99-8558-5795dbcb6594") },
                PreservationUser = new PreservationUser { IdPreservationUser = utente.IdPreservationUser }
            });

            utente.UserRoles.Add(new PreservationUserRole
            {
                PreservationRole = new PreservationRole { IdPreservationRole = new Guid("0eae6a78-bb7b-41e6-bedb-6d2057fcd943") },
                PreservationUser = new PreservationUser { IdPreservationUser = utente.IdPreservationUser }
            });

            using (var svc = GetClient())
            {
                svc.AddPreservationUser(utente, ARCHIVE_NAME);
            }
        }

        [TestMethod]
        public void TestUpdatePreservationRole()
        {
            using (var svc = GetClient())
            {
                var rol = new PreservationRole
                {
                    IdPreservationRole = new Guid("56787F89-34BE-4B99-8558-5795DBCB6594"),
                    AlertEnabled = true,
                    Enabled = true,
                    Name = "Resp. Conservazione Sostitutiva",
                    UserRoles = new BindingList<PreservationUserRole>(),
                };

                rol.UserRoles.Add(new PreservationUserRole
                {
                    Archive = new DocumentArchive { Name = ARCHIVE_NAME },
                    PreservationRole = new PreservationRole { IdPreservationRole = rol.IdPreservationRole },
                    PreservationUser = new PreservationUser { IdPreservationUser = new Guid("6F935497-98E1-44D2-8612-BCFF76807730") }
                });

                svc.UpdatePreservationRole(rol, ARCHIVE_NAME);
            }
        }

        [TestMethod]
        public void TestGetPreservationsFromArchive()
        {
            using (var svc = GetClient())
            {
                var pres = svc.GetPreservationsFromArchive(ARCHIVE_NAME);
            }
        }

        [TestMethod]
        public void TestDeletePreservationAlertType()
        {
            using (var svc = GetClient())
            {
                svc.DeletePreservationAlertType(new Guid("C187334A-8C91-4F97-A1B0-E56BD650FADA"), ARCHIVE_NAME);
            }
        }

        [TestMethod]
        public void TestUpdatePreservationAlertType()
        {
            using (var svc = GetClient())
            {
                var toAdd = new PreservationAlertType
                {
                    IdPreservationAlertType = new Guid("95c83c12-6056-40c6-a4c6-452fb17b9302"),
                    TaskTypes = new BindingList<PreservationTaskType>(),
                    AlertText = "TESTUPDATE",
                    Offset = (short)1.0,
                    Role = new PreservationRole { IdPreservationRole = new Guid("56787f89-34be-4b99-8558-5795dbcb6594") }
                };

                toAdd.TaskTypes.Add(new PreservationTaskType { IdPreservationTaskType = new Guid("00a6d4ed-915b-4fec-bdd7-21ec90a3c7c0") });

                svc.UpdatePreservationAlertType(toAdd, ARCHIVE_NAME);
            }
        }

        [TestMethod]
        public void TestDeletePreservationTaskGroup()
        {
            using (var svc = GetClient())
            {
                svc.DeletePreservationTaskGroup(new Guid("80ed478c-ac11-4055-b9e2-503f45d4cdd2"), ARCHIVE_NAME);
                svc.DeletePreservationTaskGroup(new Guid("8b247e92-692e-479d-982c-6673cc081bd4"), ARCHIVE_NAME);
                svc.DeletePreservationTaskGroup(new Guid("e8381067-a5dc-47fe-b974-a95d5a375dbb"), ARCHIVE_NAME);
                svc.DeletePreservationTaskGroup(new Guid("c1293429-aff0-43f8-91e2-db7345ee140f"), ARCHIVE_NAME);
                svc.DeletePreservationTaskGroup(new Guid("a4603bcd-0fec-412d-8332-b8d7e39d2743"), ARCHIVE_NAME);
            }
        }

        [TestMethod]
        public void TestGetPreservationAlertTypesByTaskType()
        {
            using (var svc = GetClient())
            {
                var ret = svc.GetPreservationAlertTypesByTaskType(null, new Guid("2a9bc100-c781-4ee2-ac4e-f5cb05ef911d"), ARCHIVE_NAME);
            }
        }

        [TestMethod]
        public void TestGetAdministratedArchives()
        {
            using (var svc = GetClient())
            {
                var ret = svc.GetAdministratedArchives(true);
            }
        }

        [TestMethod]
        public void TestDeleteJournaling()
        {
            using (var svc = GetClient())
            {
                svc.DeletePreservationJournaling(new Guid("8288fdbe-ee06-4e15-b251-a49d9c7200d3"), "X");
            }
        }
    }
}

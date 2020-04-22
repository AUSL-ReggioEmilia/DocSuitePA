using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using VecompSoftware.DocSuite.Public.Core.Models.Commands.Workflows;
using VecompSoftware.DocSuite.Public.Core.Models.ContentTypes.Workflows;
using VecompSoftware.DocSuite.Public.Core.Models.Customs.Workflows.Parameters;
using VecompSoftware.DocSuite.Public.Core.Models.Securities;
using VecompSoftware.DocSuite.Public.Core.Models.Workflows;
using VecompSoftware.DocSuite.Public.Core.Models.Workflows.Parameters;
using VecompSoftware.DocSuite.Public.Helpers.Json;

namespace VecompSoftware.DocSuite.Public
{
    [TestClass]
    public class StartWorkflowCommandUnitTest
    {
        //utente del gestionale che sta avviando il workflow
        private const string accountName = "domain.local\\fabrizio.lazzarotto";
        private const string tenantName = "CustomerTenant";
        private Guid tenantId = Guid.Parse("8BF7342D-FA1B-4EF9-9B44-84E66AFB77EA");

        [TestMethod]
        public void IdentityContext_IdentityNotNull()
        {
            IdentityModel identityModel = new IdentityModel(accountName, AuthorizationType.NTLM);
            IdentityContext identityContext = new IdentityContext(identityModel);
            Assert.IsNotNull(identityContext.Identity);
        }

        [TestMethod]
        public void IdentityContext_RoleNotNull()
        {
            IdentityContext identityContext = new IdentityContext(null);
            Assert.IsNotNull(identityContext.Roles);
        }


        [TestMethod]
        public void IdentityContext_RoleExist()
        {
            IdentityContext identityContext = new IdentityContext(null);
            RoleModel roleModel = new RoleModel(AuthorizationType.NTLM, externalTagIdentifier: "IP4D");
            identityContext.Roles.Add(roleModel);
            Assert.IsTrue(identityContext.Roles.Count > 0);
        }

        [TestMethod]
        public void WorkflowModel_WorkflowParametersNotNull()
        {
            WorkflowModel workflowModel = new WorkflowModel("DSW_Start");
            Assert.IsNotNull(workflowModel.WorkflowParameters);
        }

        [TestMethod]
        public void StartWorkflowCommand()
        {
            IdentityModel identityModel = new IdentityModel(accountName, AuthorizationType.NTLM);
            IdentityContext identityContext = new IdentityContext(identityModel);
            RoleModel roleModel = new RoleModel(AuthorizationType.NTLM, externalTagIdentifier: "IP4D");
            identityContext.Roles.Add(roleModel);
            WorkflowModel workflowModel = new WorkflowModel("DSW_Start");
            WorkflowParameterModel workflowParameterModel = new WorkflowParameterModel(new SignerModel(0, true, SignerType.AD, identityModel), WorkflowParameterNames.CollaborationNames.SIGNER); ;
            workflowModel.WorkflowParameters.Add(workflowParameterModel);
            StartWorkflowContentType startWorkflowContentType = new StartWorkflowContentType(workflowModel, accountName, Guid.NewGuid());
            StartWorkflowCommand startWorkflowCommand = new StartWorkflowCommand(Guid.NewGuid(), tenantName, tenantId, identityContext, startWorkflowContentType);
            Assert.IsNotNull(startWorkflowCommand.MessageName);
        }

        [TestMethod]
        public void SerializeModel_StartWorkflowCommand()
        {
            IdentityModel identityModel = new IdentityModel(accountName, AuthorizationType.NTLM);
            IdentityContext identityContext = new IdentityContext(identityModel);
            RoleModel roleModel = new RoleModel(AuthorizationType.NTLM, externalTagIdentifier: "IP4D");
            identityContext.Roles.Add(roleModel);
            WorkflowModel workflowModel = new WorkflowModel("DSW_Start");
            WorkflowParameterModel workflowParameterModel = new WorkflowParameterModel(new SignerModel(0, true, SignerType.AD, identityModel), WorkflowParameterNames.CollaborationNames.SIGNER); ;
            workflowModel.WorkflowParameters.Add(workflowParameterModel);
            StartWorkflowContentType startWorkflowContentType = new StartWorkflowContentType(workflowModel, accountName, Guid.NewGuid());
            StartWorkflowCommand startWorkflowCommand = new StartWorkflowCommand(Guid.NewGuid(), tenantName, tenantId, identityContext, startWorkflowContentType);
            string json = ManagerHelper.SerializeModel(startWorkflowCommand);
            Assert.IsNotNull(json);
        }

        [TestMethod]
        public void DeserializeModel_StartWorkflowCommand_NotNull()
        {
            IdentityModel identityModel = new IdentityModel(accountName, AuthorizationType.NTLM);
            IdentityContext identityContext = new IdentityContext(identityModel);
            RoleModel roleModel = new RoleModel(AuthorizationType.DocSuiteSecurity, Guid.NewGuid());
            identityContext.Roles.Add(roleModel);
            WorkflowModel workflowModel = new WorkflowModel("DSW_StartCollaboration");
            SignerModel singner1 = new SignerModel(1, true, SignerType.AD, identityModel);
            WorkflowParameterModel workflowParameterModel1 = new WorkflowParameterModel(singner1, WorkflowParameterNames.CollaborationNames.SIGNER);
            workflowModel.WorkflowParameters.Add(workflowParameterModel1);
            SignerModel singner2 = new SignerModel(2, true, SignerType.DSWRole, role: roleModel);
            WorkflowParameterModel workflowParameterModel2 = new WorkflowParameterModel(singner2, WorkflowParameterNames.CollaborationNames.SIGNER);
            workflowModel.WorkflowParameters.Add(workflowParameterModel2);
            StartWorkflowContentType startWorkflowContentType = new StartWorkflowContentType(workflowModel, accountName, Guid.NewGuid());
            StartWorkflowCommand startWorkflowCommand = new StartWorkflowCommand(Guid.NewGuid(), tenantName, tenantId, identityContext, startWorkflowContentType);
            string json = ManagerHelper.SerializeModel(startWorkflowCommand);
            StartWorkflowCommand deJson = ManagerHelper.DeserializeModel<StartWorkflowCommand>(json);
            Assert.IsNotNull(deJson);
        }

        [TestMethod]
        public void DeserializeModel_StartWorkflowCommand_CorrelationId()
        {
            IdentityModel identityModel = new IdentityModel(accountName, AuthorizationType.NTLM);
            IdentityContext identityContext = new IdentityContext(identityModel);
            RoleModel roleModel = new RoleModel(AuthorizationType.DocSuiteSecurity, Guid.NewGuid());
            identityContext.Roles.Add(roleModel);
            WorkflowModel workflowModel = new WorkflowModel("DSW_StartCollaboration");
            SignerModel singner1 = new SignerModel(1, true, SignerType.AD, identityModel);
            WorkflowParameterModel workflowParameterModel1 = new WorkflowParameterModel(singner1, WorkflowParameterNames.CollaborationNames.SIGNER);
            workflowModel.WorkflowParameters.Add(workflowParameterModel1);
            SignerModel singner2 = new SignerModel(2, true, SignerType.DSWRole, role: roleModel);
            WorkflowParameterModel workflowParameterModel2 = new WorkflowParameterModel(singner2, WorkflowParameterNames.CollaborationNames.SIGNER);
            workflowModel.WorkflowParameters.Add(workflowParameterModel2);
            StartWorkflowContentType startWorkflowContentType = new StartWorkflowContentType(workflowModel, accountName, Guid.NewGuid());
            StartWorkflowCommand startWorkflowCommand = new StartWorkflowCommand(Guid.NewGuid(), tenantName, tenantId, identityContext, startWorkflowContentType)
            {
                CorrelationId = Guid.NewGuid()
            };
            string json = ManagerHelper.SerializeModel(startWorkflowCommand);
            StartWorkflowCommand deJson = ManagerHelper.DeserializeModel<StartWorkflowCommand>(json);
            Assert.AreEqual<Guid>(startWorkflowCommand.CorrelationId.Value, deJson.CorrelationId.Value);
        }

        [TestMethod]
        public void DeserializeModel_StartWorkflowCommand_MessageId()
        {
            IdentityModel identityModel = new IdentityModel(accountName, AuthorizationType.NTLM);
            IdentityContext identityContext = new IdentityContext(identityModel);
            RoleModel roleModel = new RoleModel(AuthorizationType.DocSuiteSecurity, Guid.NewGuid());
            identityContext.Roles.Add(roleModel);
            WorkflowModel workflowModel = new WorkflowModel("DSW_StartCollaboration");
            SignerModel singner1 = new SignerModel(1, true, SignerType.AD, identityModel);
            WorkflowParameterModel workflowParameterModel1 = new WorkflowParameterModel(singner1, WorkflowParameterNames.CollaborationNames.SIGNER);
            workflowModel.WorkflowParameters.Add(workflowParameterModel1);
            SignerModel singner2 = new SignerModel(2, true, SignerType.DSWRole, role: roleModel);
            WorkflowParameterModel workflowParameterModel2 = new WorkflowParameterModel(singner2, WorkflowParameterNames.CollaborationNames.SIGNER);
            workflowModel.WorkflowParameters.Add(workflowParameterModel2);
            StartWorkflowContentType startWorkflowContentType = new StartWorkflowContentType(workflowModel, accountName, Guid.NewGuid());
            StartWorkflowCommand startWorkflowCommand = new StartWorkflowCommand(Guid.NewGuid(), tenantName, tenantId, identityContext, startWorkflowContentType);
            string json = ManagerHelper.SerializeModel(startWorkflowCommand);
            StartWorkflowCommand deJson = ManagerHelper.DeserializeModel<StartWorkflowCommand>(json);
            Assert.AreEqual(startWorkflowCommand.Id, deJson.Id);
        }

        [TestMethod]
        public void DeserializeModel_StartWorkflowCommand_MessageDate()
        {
            IdentityModel identityModel = new IdentityModel(accountName, AuthorizationType.NTLM);
            IdentityContext identityContext = new IdentityContext(identityModel);
            RoleModel roleModel = new RoleModel(AuthorizationType.DocSuiteSecurity, Guid.NewGuid());
            identityContext.Roles.Add(roleModel);
            WorkflowModel workflowModel = new WorkflowModel("DSW_StartCollaboration");
            SignerModel singner1 = new SignerModel(1, true, SignerType.AD, identityModel);
            WorkflowParameterModel workflowParameterModel1 = new WorkflowParameterModel(singner1, WorkflowParameterNames.CollaborationNames.SIGNER);
            workflowModel.WorkflowParameters.Add(workflowParameterModel1);
            SignerModel singner2 = new SignerModel(2, true, SignerType.DSWRole, role: roleModel);
            WorkflowParameterModel workflowParameterModel2 = new WorkflowParameterModel(singner2, WorkflowParameterNames.CollaborationNames.SIGNER);
            workflowModel.WorkflowParameters.Add(workflowParameterModel2);
            StartWorkflowContentType startWorkflowContentType = new StartWorkflowContentType(workflowModel, accountName, Guid.NewGuid());
            StartWorkflowCommand startWorkflowCommand = new StartWorkflowCommand(Guid.NewGuid(), tenantName, tenantId, identityContext, startWorkflowContentType);
            string json = ManagerHelper.SerializeModel(startWorkflowCommand);
            StartWorkflowCommand deJson = ManagerHelper.DeserializeModel<StartWorkflowCommand>(json);
            Assert.AreEqual(startWorkflowCommand.MessageDate, deJson.MessageDate);
        }

        [TestMethod]
        public void DeserializeModel_StartWorkflowCommand_CustomProperties()
        {
            IdentityModel identityModel = new IdentityModel(accountName, AuthorizationType.NTLM);
            IdentityContext identityContext = new IdentityContext(identityModel);
            RoleModel roleModel = new RoleModel(AuthorizationType.DocSuiteSecurity, Guid.NewGuid());
            identityContext.Roles.Add(roleModel);
            WorkflowModel workflowModel = new WorkflowModel("DSW_StartCollaboration");
            SignerModel singner1 = new SignerModel(1, true, SignerType.AD, identityModel);
            WorkflowParameterModel workflowParameterModel1 = new WorkflowParameterModel(singner1, WorkflowParameterNames.CollaborationNames.SIGNER);
            workflowModel.WorkflowParameters.Add(workflowParameterModel1);
            SignerModel singner2 = new SignerModel(2, true, SignerType.DSWRole, role: roleModel);
            WorkflowParameterModel workflowParameterModel2 = new WorkflowParameterModel(singner2, WorkflowParameterNames.CollaborationNames.SIGNER);
            workflowModel.WorkflowParameters.Add(workflowParameterModel2);
            StartWorkflowContentType startWorkflowContentType = new StartWorkflowContentType(workflowModel, accountName, Guid.NewGuid());
            StartWorkflowCommand startWorkflowCommand = new StartWorkflowCommand(Guid.NewGuid(), tenantName, tenantId, identityContext, startWorkflowContentType);
            string json = ManagerHelper.SerializeModel(startWorkflowCommand);
            StartWorkflowCommand deJson = ManagerHelper.DeserializeModel<StartWorkflowCommand>(json);
            Assert.IsTrue(startWorkflowCommand.CustomProperties.All(f => deJson.CustomProperties.Any(x => f.Key == x.Key)));
        }

        [TestMethod]
        public void DeserializeModel_StartWorkflowCommand_Archive()
        {
            IdentityModel identityModel = new IdentityModel(accountName, AuthorizationType.NTLM);
            IdentityContext identityContext = new IdentityContext(identityModel);
            RoleModel roleModel = new RoleModel(AuthorizationType.DocSuiteSecurity, Guid.NewGuid());
            identityContext.Roles.Add(roleModel);
            WorkflowModel workflowModel = new WorkflowModel("Archivia Fattura");

            ArchiveModel archiveFattura = new ArchiveModel("Archivio Fatture");
            MetadataModel modelAnnoIva = new MetadataModel("Anno Iva", 2016.ToString());
            archiveFattura.Metadatas.Add(modelAnnoIva);
            WorkflowParameterModel workflowParameterModel1 = new WorkflowParameterModel(archiveFattura, WorkflowParameterNames.DocumentUnitNames.METADATA);
            workflowModel.WorkflowParameters.Add(workflowParameterModel1);

            ContactModel modelFornitore = new ContactModel(ContactType.Administration, archiveSection: "Fornitori")
            {
                Description = "Vecomp Software S.r.l.",
                LanguageCode = "IT"
            };
            WorkflowParameterModel workflowParameterModel2 = new WorkflowParameterModel(modelFornitore, WorkflowParameterNames.ArchiveNames.CONTACT);
            workflowModel.WorkflowParameters.Add(workflowParameterModel2);

            DocumentModel modelDocumento = new DocumentModel("fatture_1234.pdf", new byte[] { 0x00 }, DocumentType.Main, "Fattura del fornitore");
            WorkflowParameterModel workflowParameterModel3 = new WorkflowParameterModel(modelDocumento, WorkflowParameterNames.ArchiveNames.MAIN_DOCUMENT);
            workflowModel.WorkflowParameters.Add(workflowParameterModel3);

            StartWorkflowContentType startWorkflowContentType = new StartWorkflowContentType(workflowModel, accountName, Guid.NewGuid());
            StartWorkflowCommand startWorkflowCommand = new StartWorkflowCommand(Guid.NewGuid(), tenantName, tenantId, identityContext, startWorkflowContentType);
            string json = ManagerHelper.SerializeModel(startWorkflowCommand);
            StartWorkflowCommand deJson = ManagerHelper.DeserializeModel<StartWorkflowCommand>(json);
            Assert.IsNotNull(deJson);
        }

        [TestMethod]
        public void SerializeCherwell_Valid()
        {
            CherwellContactModel cherwellContactModel = new CherwellContactModel
            {
                busObId = Guid.NewGuid().ToString(),
                busObPublicId = Guid.NewGuid().ToString(),
                busObRecId = Guid.NewGuid().ToString()
            };
            cherwellContactModel.fields.Add(new CherwellMetadataModel() { dirty = true, displayName = "test", fieldId = Guid.NewGuid().ToString(), name = "test 2", value = "test valore" });
            string json = JsonConvert.SerializeObject(cherwellContactModel);
            Assert.IsNotNull(json);
        }

        [TestMethod]
        public void DeserializeModel_StartWorkflowCommand_Protocol()
        {
            IdentityModel identityModel = new IdentityModel(accountName, AuthorizationType.NTLM);
            IdentityContext identityContext = new IdentityContext(identityModel);
            RoleModel roleModel = new RoleModel(AuthorizationType.DocSuiteSecurity, Guid.NewGuid());
            identityContext.Roles.Add(roleModel);
            WorkflowModel workflowModel = new WorkflowModel("SkyDoc-IP4D - Protocolla Semplice");

            ContactModel modelFornitore = new ContactModel(ContactType.Administration, archiveSection: "Fornitori")
            {
                Description = "Vecomp Software S.r.l.",
                LanguageCode = "IT"
            };
            WorkflowParameterModel workflowParameterModel2 = new WorkflowParameterModel(modelFornitore, WorkflowParameterNames.ArchiveNames.CONTACT);
            workflowModel.WorkflowParameters.Add(workflowParameterModel2);

            DocumentModel modelDocumento = new DocumentModel("fatture_1234.pdf", new byte[] { 0x00 }, DocumentType.Main, "Fattura del fornitore");
            WorkflowParameterModel workflowParameterModel3 = new WorkflowParameterModel(modelDocumento, WorkflowParameterNames.ArchiveNames.MAIN_DOCUMENT);
            workflowModel.WorkflowParameters.Add(workflowParameterModel3);

            StartWorkflowContentType startWorkflowContentType = new StartWorkflowContentType(workflowModel, accountName, Guid.NewGuid());
            StartWorkflowCommand startWorkflowCommand = new StartWorkflowCommand(Guid.NewGuid(), tenantName, tenantId, identityContext, startWorkflowContentType);
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri("http://localhost/PublicDSW.WebAPI/")
            };
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.PostAsJsonAsync("api/StartWorkflow", startWorkflowCommand).Result;

            Assert.IsTrue(response.EnsureSuccessStatusCode().IsSuccessStatusCode);
        }
    }


}

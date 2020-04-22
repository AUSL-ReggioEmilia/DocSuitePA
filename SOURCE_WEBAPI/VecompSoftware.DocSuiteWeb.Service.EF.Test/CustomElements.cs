using System;
using VecompSoftware.DocSuiteWeb.Entity.Collaborations;
using VecompSoftware.DocSuiteWeb.Entity.Desks;
using VecompSoftware.DocSuiteWeb.Entity.Messages;

namespace VecompSoftware.DocSuiteWeb.Service.EF.Test
{
    public class CustomElements
    {
        #region [ Properties ]
        public static readonly Guid DeskId = Guid.Parse("FCB8755C-7F0C-499A-95DD-3D22C0614E00");
        public static readonly Guid DeskRoleUserId = Guid.Parse("4D30CCD3-5EA7-4C1D-85F3-F5E3A120C316");
        public static readonly int CollaborationId = -1;
        public static readonly Guid CollaborationUserId = Guid.Parse("80C34E11-7351-4775-B823-52A36D22DA92");
        public static readonly int MessageId = -1;
        public static readonly int MessageEmailId = -1;
        #endregion

        #region [ Constructor ]
        #endregion

        #region [ Methods ]

        public static Desk CreateDeskModel()
        {
            Desk desk = new Desk(DeskId)
            {
                Description = "Descrizione da Unit Test",
                Name = "Nome da Unit Test",
                ExpirationDate = DateTime.UtcNow,
                Status = DeskState.Open
            };

            desk.DeskRoleUsers.Add(new DeskRoleUser(DeskRoleUserId)
            {
                Desk = desk,
                AccountName = "Unit.Test",
                PermissionType = DeskPermissionType.Admin
            });

            return desk;
        }

        public static Collaboration CreateCollaborationModel()
        {
            Collaboration collaboration = new Collaboration()
            {
                EntityId = CollaborationId,
                IdStatus = CollaborationStatusType.Insert,
                Note = "Unit Test Note",
                Subject = "Collaborazione di Test"
            };

            collaboration.CollaborationUsers.Add(new CollaborationUser()
            {
                UniqueId = CollaborationUserId,
                Collaboration = collaboration,
                Incremental = 0,
                DestinationName = "Unit.Test"
            });

            return collaboration;
        }

        public static Message CreateMessageModel()
        {
            Message message = new Message()
            {
                EntityId = MessageId,
                Status = MessageStatus.Draft
            };

            message.MessageEmails.Add(new MessageEmail()
            {
                EntityId = MessageEmailId,
                Subject = "Unit Test Message",
                Message = message,
                Body = "Unit Test Body Message"
            });

            return message;
        }
        #endregion
    }
}

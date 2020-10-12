using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VecompSoftware.BPM.Integrations.Modules.VSW.PayableInvoiceFileSystem.Models;
using VecompSoftware.DocSuiteWeb.Model.Entities.Messages;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.PayableInvoiceFileSystem.Helpers
{
    internal static class MessageHelpers
    {
        public static MessageBuildModel CreateMessageBuildModel(ModuleConfigurationModel moduleConfiguration, string message)
        {
            return new MessageBuildModel
            {
                Message = new MessageModel
                {
                    MessageType = MessageType.Email,
                    MessageContacts = new List<MessageContactModel>
                    {
                        new MessageContactModel
                        {
                            ContactType = MessageContactType.User,
                            ContactPosition = MessageContantTypology.Recipient,
                            Description = string.Empty,
                            MessageContactEmail = moduleConfiguration.MessageConfiguration.EmailRecipients.Select(email=>
                                new MessageContactEmailModel
                                {
                                     Description = string.Empty,
                                     Email = email,
                                     User = email,
                                }).ToList(),
                            }
                        },

                    MessageEmails = new List<MessageEmailModel> {
                        new MessageEmailModel
                        {
                            Priority = "0",
                            Body = message,
                            Subject = moduleConfiguration.MessageConfiguration.Subject,
                            SentDate = DateTime.UtcNow
                        }
                    }
                }
            };
        }
    }
}

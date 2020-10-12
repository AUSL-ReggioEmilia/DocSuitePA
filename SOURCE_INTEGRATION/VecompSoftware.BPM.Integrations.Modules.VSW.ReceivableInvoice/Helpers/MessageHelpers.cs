using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VecompSoftware.BPM.Integrations.Modules.VSW.ReceivableInvoice.Models;
using VecompSoftware.DocSuiteWeb.Model.Entities.Messages;

namespace VecompSoftware.BPM.Integrations.Modules.VSW.ReceivableInvoice.Helpers
{
    internal static class MessageHelpers
    {
        public static MessageBuildModel CreateMessageBuildModel(ModuleConfigurationModel moduleConfiguration, ProcessSummary processSummary)
        {
            //create body
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Total number of invoices: {processSummary.InitialCount}");
            stringBuilder.AppendLine($"Total processed count : {processSummary.ZipEntrySummaries.Count(x => x.Processed)}");
            stringBuilder.AppendLine($"Total number or errors: {processSummary.ZipEntrySummaries.Count(x => !(x.ErrorMessage is null))}");

            foreach(ZipEntryProcessSummary entry in processSummary.ZipEntrySummaries)
            {
                if (!string.IsNullOrWhiteSpace(entry.InvoiceFileName))
                {
                    stringBuilder.Append($"Invoice \'{entry.InvoiceFileName}\': ");
                }
                else
                {
                    stringBuilder.Append($"Zip Metadata Name \'{entry.ZipMetadataName}\': ");
                }
                
                if (!(entry.ErrorMessage is null))
                {
                    stringBuilder.AppendLine($"processed with errors. Error: \'{entry.ErrorMessage}\'");
                }
                else if (!entry.Processed)
                {
                    stringBuilder.AppendLine("was not processed");

                }
                else
                {
                    stringBuilder.AppendLine("processed succesfully.");
                }
            }

            return new MessageBuildModel
            {
                Message = new MessageModel
                {
                    MessageType = VecompSoftware.DocSuiteWeb.Model.Entities.Messages.MessageType.Email,
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
                            Body =stringBuilder.ToString(),
                            Subject = moduleConfiguration.MessageConfiguration.Subject,
                            SentDate = DateTime.UtcNow
                        }
                    }
                }
            };
        }
    }
}

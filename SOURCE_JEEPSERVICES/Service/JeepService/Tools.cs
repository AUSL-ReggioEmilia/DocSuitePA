using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Resources;
using System.Text.RegularExpressions;
using System.Xml;
using VecompSoftware.GenericMailSender;
using VecompSoftware.JeepService.Common;
using VecompSoftware.Services.Logging;

namespace JeepService
{
    public static class Tools
    {
        #region [ Fields ]

        private const string Logger = "Application";

        #endregion

        /// <summary> Builder per il caricamento dinamico dei moduli. </summary>
        /// <param name="path">Path della dll</param>
        /// <param name="type">Nome della classe da caricare</param>
        /// <param name="parameters">Dictionary dei parametri da passare al modulo</param>
        /// <param name="name"> </param>
        /// <returns>Restituisce l'oggetto IJeepModule creato</returns>
        public static IJeepModule ModuleBuilder(string path, string type, List<Parameter> parameters, string name)
        {
            FileLogger.Debug(Logger, "Load " + path);
            Assembly myAssembly = Assembly.LoadFile(path);
            FileLogger.Debug(Logger, string.Format("Create Instance {0}", type));
            IJeepModule item = (IJeepModule)myAssembly.CreateInstance(type, false, BindingFlags.CreateInstance, null, null, null, null);
            if (item == null) throw new MissingSatelliteAssemblyException(String.Format("Impossibile caricare il modulo {0}", myAssembly.FullName));
            item.Name = name;
            item.Version = FileVersionInfo.GetVersionInfo(myAssembly.Location).FileVersion;
            FileLogger.Debug(Logger, string.Format("Item Created {0}", name));
            item.Message += OnItemMessage;
            return item;
        }

        private static void OnItemMessage(object sender, MessageEventArgs args)
        {
            var module = (IJeepModule)sender;
            SendMessage(String.Format("{0} v.{1}", module.Name, module.Version), args);
        }

        /// <summary> Spedisce il messaggio del modulo alla mail indicata e al log. </summary>
        /// <param name="sender"> Nome del modulo. </param>
        /// <param name="args"> Messaggio. </param>
        public static void SendMessage(string sender, MessageEventArgs args)
        {
            FileLogger.Debug(Logger, string.Format("MESSAGGIO da modulo {0}: {1}", sender, args.Message));

            // Chiamata ricevuta da un modulo, spedisco l'email
            if (!bool.Parse(ConfigurationManager.AppSettings["NotificationMailEnabled"]))
            {
                FileLogger.Error(Logger, "Messaggio da modulo non inviato,\"NotificationMailEnabled\" disabilitato.");
                return;
            }

            try
            {
                //Istanzio un client
                var client = new MailClient(
                    ConfigurationManager.AppSettings["NotificationMailType"],
                    ConfigurationManager.AppSettings["NotificationMailServer"],
                    !string.IsNullOrEmpty(ConfigurationManager.AppSettings["NotificationMailServerPort"]) ? int.Parse(ConfigurationManager.AppSettings["NotificationMailServerPort"]) : 25,
                    !String.IsNullOrEmpty(ConfigurationManager.AppSettings["NotificationMailServerAuthenticationType"])
                        ? (MailClient.AuthenticationType)Enum.Parse(typeof(MailClient.AuthenticationType), ConfigurationManager.AppSettings["NotificationMailServerAuthenticationType"], true)
                        : MailClient.AuthenticationType.Plain,
                    ConfigurationManager.AppSettings["NotificationMailUserName"],
                    ConfigurationManager.AppSettings["NotificationMailUserPassword"],
                    ConfigurationManager.AppSettings["NotificationMailDomain"]
                    );
                var subject = string.Format("Chiamata da modulo {0} su {1}", sender, ConfigurationManager.AppSettings["Customer"]);
                var body = CleanInvalidXmlChars(string.Format("MODULO: {0}{3}CLIENTE: {1}{3}MESSAGGIO: {2}", sender, ConfigurationManager.AppSettings["Customer"], args.Message, Environment.NewLine));
                var message = new MailMessage
                {
                    From = new MailAddress(ConfigurationManager.AppSettings["NotificationMailFrom"]),
                    Subject = subject,
                    Body = body
                };
                if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["NotificationMailTo"]))
                {
                    foreach (var to in ConfigurationManager.AppSettings["NotificationMailTo"].Split(';'))
                    {
                        message.To.Add(to);
                    }

                    // Aggiungo altri eventuali destinatari alla segnalazione di errore.
                    // Questo per poter permettere, ad esempio, al modulo di invio mail di notificare, oltre al supporto tecnico, anche il mittente del mancato invio.
                    args.Recipients.ForEach(r => message.To.Add(r));
                }
                if (!string.IsNullOrWhiteSpace(ConfigurationManager.AppSettings["NotificationMailCc"]))
                {
                    foreach (var cc in ConfigurationManager.AppSettings["NotificationMailCc"].Split(';'))
                    {
                        message.CC.Add(cc);
                    }
                }
                client.Send(message);
                FileLogger.Info(Logger, "Spedizione CHIAMATA eseguita con successo");
            }
            catch (Exception exc)
            {
                FileLogger.Error(Logger, "Errore in fase di invio messaggio", exc);
            }
        }

        public static string CleanInvalidXmlChars(string text)
        {
            // From xml spec valid chars: 
            // #x9 | #xA | #xD | [#x20-#xD7FF] | [#xE000-#xFFFD] | [#x10000-#x10FFFF]     
            // any Unicode character, excluding the surrogate blocks, FFFE, and FFFF. 
            const string re = @"[^\x09\x0A\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF]";
            return Regex.Replace(text, re, "");
        }

        /// <summary> Builder che restituisce l'elenco dei parametri di un modulo. </summary>
        /// <param name="parameterRoot">Nodo XML che contiene i parametri serializzati</param>
        /// <returns>Dictionary chiave/valore dei parametri</returns>
        public static Dictionary<string, string> ParameterBuilder(XmlNode parameterRoot)
        {
            var items = parameterRoot.SelectNodes("./PARAMETER");
            return items.Cast<XmlNode>().ToDictionary(item => item.Attributes["key"].Value, item => item.Attributes["value"].Value);
        }

        public static bool CheckSignalTime(DateTime signal, string from, DateTime? lastExecution)
        {
            int hourToCheck = int.Parse(from.Split(':')[0]);
            int minuteToCheck = int.Parse(from.Split(':')[1]);

            if ((signal.Hour <= hourToCheck) && (signal.Hour != hourToCheck || signal.Minute < minuteToCheck))
            {
                return false;
            }

            if (!lastExecution.HasValue)
            {
                return true; // Non è mai stato eseguito
            }

            // Sono nella fascia di esecuzione
            if (lastExecution.Value.Year < signal.Year || lastExecution.Value.DayOfYear < signal.DayOfYear)
            {
                return true;
            }
            return false;
        }

        public static bool CheckSignalTime(DateTime signal, string from, string to)
        {
            int hourFromCheck = int.Parse(from.Split(':')[0]);
            int minuteFromCheck = int.Parse(from.Split(':')[1]);
            int hourToCheck = int.Parse(to.Split(':')[0]);
            int minuteToCheck = int.Parse(to.Split(':')[1]);

            if ((signal.Hour <= hourFromCheck) && (signal.Hour != hourFromCheck || signal.Minute < minuteFromCheck))
            {
                return false;
            }

            if ((signal.Hour < hourToCheck) || (signal.Hour == hourToCheck && signal.Minute <= minuteToCheck))
            {
                return true;
            }

            return false;
        }

        public static DateTime GetNextExecution(DateTime signal, string from)
        {
            int hourToCheck = int.Parse(from.Split(':')[0]);
            int minuteToCheck = int.Parse(from.Split(':')[1]);

            return DateTime.Today.AddDays(1).AddHours(hourToCheck).AddMinutes(minuteToCheck);
        }

        public static DateTime GetNextExecution(DateTime signal, string from, string to, int milliseconds)
        {
            int hourFromCheck = int.Parse(from.Split(':')[0]);
            int minuteFromCheck = int.Parse(from.Split(':')[1]);

            DateTime test = signal.AddMilliseconds(milliseconds);
            if (CheckSignalTime(signal, from, to))
            {
                return test;
            }
            return DateTime.Today.AddDays(1).AddHours(hourFromCheck).AddMinutes(minuteFromCheck);
        }

        public static DateTime GetNextExecution(DateTime signal, int milliseconds)
        {
            return signal.AddMilliseconds(milliseconds);
        }


    }
}

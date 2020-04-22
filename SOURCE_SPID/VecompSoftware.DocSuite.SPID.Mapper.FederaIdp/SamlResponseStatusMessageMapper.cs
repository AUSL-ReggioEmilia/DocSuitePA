using VecompSoftware.DocSuite.SPID.Mapper.SAML;

namespace VecompSoftware.DocSuite.SPID.Mapper.FederaIdp
{
    public class SamlResponseStatusMessageMapper : ISamlResponseStatusMessageMapper
    {
        public string Map(string source)
        {
            switch (source)
            {
                case "ErrorCode nr08":
                    return "Formato della richiesta non conforme alle specifiche SAML.";
                case "ErrorCode nr09":
                    return "Parametro version non presente, malformato o diverso da 2.0.";
                case "ErrorCode nr11":
                    return "ID (Identificatore richiesta) non presente, malformato o non conforme.";
                case "ErrorCode nr12":
                    return "RequestAuthnContext non presente, malformato o non previsto da SPID.";
                case "ErrorCode nr13":
                    return "IssueInstant non presente, malformato o non coerente con l'orario di arrivo della richiesta.";
                case "ErrorCode nr14":
                    return "Destination non presente, malformata o non coincidente con ill Gestore delle identità ricevente la richiesta.";
                case "ErrorCode nr15":
                    return "Attributo isPassive presente e attualizzato al valore true.";
                case "ErrorCode nr16":
                    return "AssertionConsumerService non correttamente valorizzato.";
                case "ErrorCode nr17":
                    return "Attributo Format dell'elemento NameIDPolicy assente o non valorizzato secondo specifica.";
                case "ErrorCode nr18":
                    return "AttributeConsumerServiceIndex malformato o che riferisce a un valore non registrato nei metadati di SP.";
                case "ErrorCode nr19":
                    return "Autenticazione fallita per ripetuta sottomissione di credenziali errate (superato numero tentativi secondo le policy adottate).";
                case "ErrorCode nr20":
                    return "Utente privo di credenziali compatibili con il livello richiesto dal fornitore del servizio.";
                case "ErrorCode nr21":
                    return "Timeout durante l’autenticazione utente.";
                case "ErrorCode nr22":
                    return "Utente nega il consenso all’invio di dati al SP in caso di sessione vigente.";
                case "ErrorCode nr23":
                    return "Utente con identità sospesa/revocata o con credenziali bloccate.";
                default:
                    return source;
            }
        }
    }
}

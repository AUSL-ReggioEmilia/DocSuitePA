Sono stati modificati i contract della sezione services del file di configurazione del servizio.
Le nuove voci devono essere modificate solo ed esclusivamente nella sezione services del file di configurazione del servizio WCF
e non nei vari client.

I singoli contract sono così modificati:
- Da "BiblosDS.WCF.Interface.IAdministration" A "VecompSoftware.ServiceContract.BiblosDS.Documents.IAdministration"
- Da "BiblosDS.WCF.Interface.Public.ILog" A "VecompSoftware.ServiceContract.BiblosDS.Logs.ILog"
- Da "BiblosDS.WCF.Interface.IContentSearch" A "VecompSoftware.ServiceContract.BiblosDS.Documents.IContentSearch"
- Da "BiblosDS.WCF.Interface.IDigitalSignature" A "VecompSoftware.ServiceContract.BiblosDS.Signs.IServiceDigitalSign"
- Da "BiblosDS.WCF.Interface.IDocuments" A "VecompSoftware.ServiceContract.BiblosDS.Documents.IDocuments"
- Da "BiblosDS.WCF.Interface.IPreservation" A "VecompSoftware.ServiceContract.BiblosDS.Preservations.IPreservation"
- Da "BiblosDS.WCF.Interface.ITransit" A "VecompSoftware.ServiceContract.BiblosDS.Documents.ITransit"
- Da "BiblosDS.WCF.Interface.IPreservationAdministration" A "VecompSoftware.ServiceContract.BiblosDS.Preservations.IPreservationAdministration"

Esempio di modifica:
--- Da
<service behaviorConfiguration="ServiceBehavior" name="Administration">
    <endpoint address="" binding="basicHttpBinding" bindingConfiguration="BasicHttpStreamedBinding" contract="BiblosDS.WCF.Interface.IAdministration" />
</service>

---- A
<service behaviorConfiguration="ServiceBehavior" name="Administration">
    <endpoint address="" binding="basicHttpBinding" bindingConfiguration="BasicHttpStreamedBinding" contract="VecompSoftware.ServiceContract.BiblosDS.Documents.IAdministration" />
</service>

Se nella sezione services del WCF installato mancano una o più voci dei contract sopra indicati procedere ad aggiungerli nel file di configurazione prendendo come
riferimento il file wcfservices.system.servicemodel.services.config del pacchetto di installazione.

Per qualsiasi dubbio chiedere al reparto di Sviluppo.

#AC
##########################################
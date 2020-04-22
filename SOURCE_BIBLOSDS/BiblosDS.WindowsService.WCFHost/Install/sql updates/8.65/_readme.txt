Sono stati modificati i contract degli endpoint della sezione services del file di configurazione del servizio.
Le nuove voci devono essere modificate solo ed esclusivamente nella sezione services del file di configurazione del servizio WCFHost
e non nei vari client.

I singoli contract sono così modificati:    

- Da "BiblosDS.WCF.Interface.IServicePreservation" A "VecompSoftware.ServiceContract.BiblosDS.Documents.IServicePreservation"
- Da "BiblosDS.WCF.Interface.IServiceDocumentStorage" A "VecompSoftware.ServiceContract.BiblosDS.Documents.IServiceDocumentStorage"
- Da "BiblosDS.WCF.Interface.IDocuments" A "VecompSoftware.ServiceContract.BiblosDS.Documents.IDocuments"

Esempio di modifica:
--- Da
<endpoint address="net.tcp://localhost:9090"
              binding="netTcpBinding"
              bindingConfiguration="NetTcpBufferedBinding"
              contract="BiblosDS.WCF.Interface.IServicePreservation">
</endpoint>

---- A
<endpoint address="net.tcp://localhost:9090"
              binding="netTcpBinding"
              bindingConfiguration="NetTcpBufferedBinding"
              contract="VecompSoftware.ServiceContract.BiblosDS.Documents.IServicePreservation">
</endpoint>

Se nella sezione services del WCFHost installato mancano una o più voci dei contract sopra indicati procedere ad aggiungerli nel file di configurazione prendendo come
riferimento il file wcfhost.system.servicemodel.services.config del pacchetto di installazione.

Per qualsiasi dubbio chiedere al reparto di Sviluppo.

#AC
##########################################
Per il corretto funzionamento della release 8.60 di DocSuite devono essere aggiornati i seguenti applicativi:

- Service BUS receiver 8.60
- BiblosDS2010 8.60

#AC
############################################################################

ATTENZIONE: 
QUESTO AGGIORNAMENTO ESEGUIRA' LA RIMOZIONE FISICA DELLA COLONNA ConservationYear DELLA TABELLA Category,IN QUANTO LA 
SUA FUNZIONALITA' E' STATA GESTITA DALL'INTRODUZIONE DEL MASSIMARIO DI SCARTO.

#GN
############################################################################

Per risolvere il problema della dimensione dei file delle UDS è necessario applicare i seguenti passaggi sul server dove è installato il prodotto Micorosoft Service Bus.

 - Aprire il file C:\Program Files\Service Bus\1.1\Microsoft.ServiceBus.Gateway.exe.config
 - Cercare i l binding col nome netMessagingProtocolHead
 - Cambiare i valori dei maxReceivedMessageSize="157286400" e maxBufferSize="157286400"
 - Riavviare tutti i servizi di windows del service bus services
 
 il risultato dopo la modifica della sezione del config è la seguente:
 
		<binding name="netMessagingProtocolHead" receiveTimeout="24:00:00" listenBacklog="64" maxConnections="64" maxReceivedMessageSize="157286400" maxBufferSize="157286400">
          <readerQuotas maxArrayLength="5242880" maxDepth="64" maxStringContentLength="5242880" maxBytesPerRead="4096" maxNameTableCharCount="16384" />
          <security>
            <transport clientCredentialType="None"></transport>
          </security>
        </binding>
		
#FL
############################################################################

Il Windows Fabric Host è un prodotto Microsoft che viene installato insieme al Service Bus, ma genera dei file di trace molto corposi nel tempo.
Per tutti i clienti che hanno attivo il Service Bus è necessario eseguire il sequente comando nel promnt dei comandi su tutti nodi della farm del Service Bus.

Logman update trace FabricLeaseLayerTraces -ow –-cnf

Questo commando sovrascriverebbe il file ogni volta che si raggiungono i 128 Mb

#FL
############################################################################

[AUSL-RE]
E' stato introdotto il parametro booleamo UnifiedCollFunctionDesignerEnabled.
Se = true unifica l'alberatura di disegno di funzione in un'unica alberatura col nome di Funzioni con sottonodi Dirigenti, Vicario, Segreteria e Responsabile di Procedimento
Se = false l'alberatura avrà i nodi Fascicoli e Collaborazioni divisi

#SZ
############################################################################

E' stato introdotto un file di configurazione json per la pagina di Ricerca Flusso.
Nella cartella Config è presente il file RicercaFlussoConfig.json, il quale dovrà essere rivisto e modificato in modo tale da avere un file di configurazione specifico per ogni cliente.
Per il momento le due possibili configurazioni sono

Per [ASL-TO]:
{
  //Determina
  "0": {
    "0": "Adozione",
    "1": "Lettera di avvenuta Adozione",
    "3": "Collegamento protocollo trasmissione agli Organi di Controllo",
    "10": "Pubblicazione",
    "6": "Esecutività",
    "11": "Ultima Pagina (Dematerializzazione) Firma Digitale"
  },
  //Delibera
  "1": {
    "0": "Adozione",
    "9": "Invio Adozione Collegio Sindacale Firma Digitale",
    "3": "Collegamento protocollo trasmissione agli Organi di Controllo",
    "10": "Pubblicazione",
    "6": "Esecutività",
    "11": "Ultima Pagina (Dematerializzazione) Firma Digitale"
  }
}

Per [AUSL-PC]:
{
  //Determina
  "0": {
   "11": "Ultima Pagina (Dematerializzazione) Firma Digitale"
  },
  //Delibera
  "1": {
    "11": "Ultima Pagina (Dematerializzazione) Firma Digitale"
  }
}

Il contenuto del file va quindi modificato in base al cliente in questione.

NB: dal momento che in ambiente di sviluppo i test sul flusso atti non possono essere completi, si consiglia di testare il giro atti in formazione (utilizzando la pagina Ricerca Flusso).

#SDC
############################################################################

TUTTI I CLIENTI CON UDS ATTIVI
chiedere il supporto a sviluppo per la modifica manuale dei vari XML delle UDS di produzione

#FL
############################################################################

E' stata rilasciata una versione del Preservation Portal 8.60

In questa versione sono stati risolti:
•	Gestione dell’AutoInc per il bug trovato durante la conservazione delle PEC in ***REMOVED***
•	Gestione della data di fine periodo del PreservationTask (campo EndDocumentDate) per recuperare anche i documenti inseriti nell’ultimo giorno del periodo di conservazione del Task (bug CTT)

Per velocizzare il processo di conservazione del PreservationPortal è necessario riavviare il Pool ogni volta che si finisce l’esecuzione di un Task.

#SZ
############################################################################

E' stato introdotto un file di configurazione json per la definizione dei controlli di default da utilizzare nella ricerca di Protocollo (vedi parametro ProtocolDefaultAdaptiveSearch).
Nella cartella Config è presente il file ProtocolAdaptiveSearchMapping.json, il quale definisce la mappatura del control id con il rispettivo valore letterale.
La configurazione è standard per tutti i clienti.

#AC
############################################################################

Attenzione! Il parametro OmniBusApplicationKey di ParameterEnv è da configurare per AUSL-RE (valore di default da impostare VEC#2017ON1104).
Tale parametro necessita la configurazione dell'endpoint relativo al WS di integrazione Omnibus come 
nell'esempio seguente:

<endpoint address="http://smv-sinfo-srv2.asmn.net/omnibusnet/OmnibusWs.asmx"
          binding="basicHttpBinding" bindingConfiguration="BasicHttpStreamedBinding"
          contract="OmnibusWsSoap" name="OmnibusWsSoap" />

L'endpoint va aggiunto nel file di configurazione docsuite.system.servicemodel.client.config.

#AC
############################################################################

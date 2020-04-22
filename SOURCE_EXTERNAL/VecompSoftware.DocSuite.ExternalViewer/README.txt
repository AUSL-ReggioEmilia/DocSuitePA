Il file dynamic.setting.ts, che si trova al percorso app\config\ contiene alcuni parametri da configurare per l'applicazione.
ATTENZIONE: mai configurare un url con localhost!

I primi parametri configurano le chiamate alle API pubbliche:
	La proprietà 'apiOdataAddress' definisce l'indirizzo delle API per le chiamate ODATA.
	La proprietà 'apiAuthAddress' definisce l'indirizzo delle API per l'autenticazione all'avvio dell'applicazione (attenzione a mantenere la rotta /Auth/Token).
	Il dizionario 'apiOdataControllers' contiene l'elenco dei possibili controllers delle API; ad ognuno è associato un valore booleano TRUE o FALSE.
	Impostando a TRUE il valore, si rende disponibile il controller corrispondente.
	Impostanto a FALSE il valore, si spegne il controller corrispondente.

'documentHandler' definisce l'handler per la visualizzazione dei documenti del protocollo.
'toastLife' definisce il tempo massimo di visualizzazione delle toast notification, che appaiono per comunicare errori relativi alle chiamate alle WebAPI o alla composizione dell'URL.
'applicationName' permette di indicare il nome dell'applicazione che verrà mostrato nella barra di navigazione in alto.
'withAuth' definisce se le chiamate alle WebAPI per il sommario di protocollo richiedono token di autenticazione (***REMOVED***=true) oppure no (AUSL-RE=false).
'introduction' definisce il testo che si mostra nella home page dell'applicazione; se non si setta nulla di default si mostra la frase
			   'Per visualizzare un protocollo, comporre l\'URL indicando l\'identificativo o la coppia anno e numero.' (***REMOVED***='', va impostato per AUSL-RE)

Il parametro OAUTH_USERNAME è una lista strutturata come 'Guid' : 'stringa', dove il Guid identifica l'id del flusso da cui deriva l'External Viewer e la stringa lo username associato all'azione.
Questi valori sono specifici per ogni cliente, quindi si fornisce la lista dei valori possibili:

***REMOVED***:
	{
		"BDBD85B9-FCC4-487F-9EB6-CBB9C11A5D04":"***REMOVED***.IP4D",
		"19CE9681-F52C-42E6-B0F7-ADFF541E88A5":"***REMOVED***.ERP_CONTRATTI"
	}

L'elenco completo si può trovare anche nel file app\settings\static.setting.ts.

#SDC
#############################################################################################

Per abilitare il production mode, decommentare la riga 9 del file main.ts nella cartella app.
Per modificare il logo del prodotto DocSuite, visualizzato in home page, inserire il nuovo file nella cartella app\images con il nome 'vecompLogo.gif', 
andando quindi a sovrascrivere quello presente.

#SDC

#############################################################################################

L'URL per visualizzare un protocollo è il seguente:

http://<DNS>/ExternalViewer/#/Protocollo/anno/<ANNO>/numero/<NUMERO>

dove al posto di <ANNO> e <NUMERO> vanno inseriti i valori specifici del protocollo da visualizzare.
Sono presenti poi due parametri facoltativi che possono essere aggiunti all'url nel modo seguente

http://<DNS>/ExternalViewer/#/Protocollo/anno/<ANNO>/numero/<NUMERO>;token=<TOKEN>;authrule=<AUTHRULE>

dove <TOKEN> deve essere di tipo stringa e <AUTHRULE> di tipo GUID. Mostriamo in dettaglio come funziona l’integrazione. 

A fronte dell’url cotenuto nell’evento DocSuite, https://<DNS>/ExternalViewer/#/Protocollo/anno/<ANNO>/numero/<NUMERO> 
per poterlo utilizzare è necessario eseguire i seguenti passaggi per effettuare l’autenticazione (ad ogni singolo accesso):  

1)	Invocare lato server il controller “api/TokenSecurities” WebAPI REST (verbo POST) https://<DNS>/DSW.PublicWebAPI/api/TokenSecurities?authenticationId=<chiave di sicurezza dell’integrazione>,
La chiamata vi permetterà di generare un token di sicurezza temporale con scadenza impostabile in fase di integrazione (generalmente 50 secondi). 

2)	Attraverso il TOKEN di sicurezza è possibile restituire al browser dell’utente il seguente URL così formattato (in evidenza le parti specifiche che vanno modificate)

http://<DNS>/ExternalViewer/#/Auth/appId/<chiave di sicurezza dell’integrazione>/authrule/token/kind/Protocol/param/<token=<valore del token generato dal servizio REST>|user=<nome utente>|year=>anno di protocollo>|number=<numero protocollo> tutto va codificato con encoding html>/redirectUrl/<url ricevuto nel messaggio della DocSuite> va codificato con encoding html

In dettaglio il significato dei vai punti evidenziati:
o	Chiave di sicurezza dell’integrazione: valore statico GUID che verrà fornita al fornitore
o	Token: valore statico che determina il tipo di autenticazione OAuth basta su Token.
o	Protocol: valore statico per la visualizzazione di protocollo (oggi unico utilizzabile)
o	<url ricevuto con encoding HTLM applicato> : e’ l’url dell’externalviewer, applicato con un encoding html in modo tale che i caratteri dell’url relativo non vengano interpretati dal browser
o	<token=valore del token …..>: tale valore deve essere codificato con encoding htnl e contiene le informazioni di sicurezza, quali :
•	Token: da leggere mediante il controller rest TokenSecurities
•	User: nome utente dell’applicativo del fornitore
•	Year: anno di protocollo
•	Number: numero del protocollo
•	essere applicato con un encoding html

Mostriamo un esempio per comprendere al meglio il meccanismo di codifica e composizione l’url in considerazione del fatto che in alcune sezioni va applicato l’econding HTML.
L’accesso è a fronte di una richiesta per il protocollo 2018/120 da parte dell’utente ‘giordano.colasanti’:

https://<DNS>/ExternalViewer/#/Auth/appId/49DC7004-EFC1-454A-9174-CEA12D06061E/authrule/token/kind/Protocol/param/token%3d08b636C6-0445-4C92-acf2-924cfc1e806e%7cuser%3dgiordano.colasanti%7cyear%3d2018%7cnumber%3d120/redirectUrl/https%3A%2F%2F<DNS>%2FExternalViewer%2F%23%2FProtocollo%2Fanno%2F2018%2Fnumero%2F120

#FL
#############################################################################################

[AUSL-RE]
Per la funzionalità I miei documenti autorizzati, vanno settati nel file dynamic.setting.ts:
	- withAuth = false
	- applicationName = nome dell'applicazione che si visualizza nella barra di navigazione in alto a sinistra
	- introduction = 'Per visualizzare \'I miei documenti autorizzati\' comporre l'URL indicando la rotta DocumentiAutorizzati.'

Va inoltre modificato nel file index.html il <title> che si default è settato per ***REMOVED***.

L'url da chiamare è http://<DNS>/ExternalViewer/#/DocumentiAutorizzati

#SDC
#############################################################################################
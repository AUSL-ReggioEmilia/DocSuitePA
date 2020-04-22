Ordine di esecuzione degli script SQL:

1) ProtocolloDB_migrations.sql
2) PraticheDB_migrations.sql
3) AttiDB_migrations.sql
4) ATTENZIONE! Lo script ProtocolloDB_production.sql và eseguito solo ed esclusivamente quando si sta
installando in produzione.
Se si sta installando in Stage tale script non deve essere lanciato in quanto renderebbe incompatibile
la versione installata in produzione.

#SZ
######################################################################################################

[ASL-TO]
E' stata introdotta una funzionalità che permette ai settori autorizzati al protocollo di accettare o rifiutare le autorizzazioni al protocollo. 
Il parametro 'RefusedProtocolsGroups' indica i gruppi di utenti che possono visualizzati i protocolli respinti e i protocolli in attesa di essere valutati.
E' legato ai parametri 'RefusedProtocolAuthorizationEnabled' e 'IsSecurityGroupEnabled'.
Quindi i gruppi verranno letti solo se entrambi i parametri indicati sono attivi.

#SDC
###################################################################################################

[ASL-TO]
E' stato modificato il DocSuiteMenuConfig.json. Nella sezione di scrivania di Protocollo vanno aggiunte le voci

		  "SecondNode12": { "Name": "Da Accettare" },
          "SecondNode13": { "Name": "Autorizzati Non Accettati" },
          "SecondNode14": {"Name": "Respinti"}

#SDC
###################################################################################################

[ENPACL]
E' stato introdotto un nuovo parametro che permette di configurare la CheckBox nelle maschere di invio PEC su invio originale/Copia conforme, e quindi di 
abilitare o meno la CheckBox.
Tale parametro è 'SendPECDocumentEnabled'.
Se [SendPECDocumentEnabled] = false permette di configurare la CheckBox,quindi non abilita la CheckBox nelle maschere di invio PEC su invio
 originale/Copia conforme.        
Se [SendPECDocumentEnabled] = true non rende configurabile tale checkbox e quindi permette la selezione sia dei documenti originali che quelli in 
Copia conforme.
Per ENPACL = false

#GN
###################################################################################################

[ENPACL]
E' stata introdotto un nuovo parametro per ENPACL "AssignProtocolEnabled" il quale permette all'utente che ha diritti di modifica 
di poter modificare solo l'assegnatario e lo stato del protocollo in visualizzazione di Protocollo.
[AssignProtocolEnabled = true] permette la visualizzazione del pulsante 'Assegna'
[AssignProtocolEnabled = false ] non permette tale visualizzazione di 'Assegna'

#GN
###################################################################################################

[ENPACL]
E' stata introdotto un nuovo parametro per ENPACL "PECWithErrorFilterEnabled" il quale permette di 
filtrare le PEC con Esito Invio con errore o ricevute di ritardo nella consegna, nella visualizzazione PEC, invio massivo
[PECWithErrorFilterEnabled = true] abilita il filtro delle PEC per l'Esito Invio con errore
[PECWithErrorFilterEnabled = false ] non abilita tale filtro.

#GN
###################################################################################################

[ASL-TO]
E' stato introdotto un nuovo parametro per ASL-TO "GroupsWithSearchProtocolRoleRestrictionNone" che indica i gruppi i cui utenti possono filtrare la ricerca di protocollo
anche per settori di cui non fanno parte.

#SZ
###################################################################################################

[ASL-TO]
E' stato introdotto un nuovo parametro per ASL-TO "SecurityGroupAdmin" che indica i gruppi i cui utenti possono gestire i gruppi
ed i relativi utenti

#SZ
###################################################################################################

[ASL-TO]
E' stato introdotto un nuovo parametro per ASL-TO "SecurityGroupPowerUser" che indica i gruppi i cui utenti possono gestire gli utenti dei gruppi

#SZ
###################################################################################################

[ENPACL]
E' stata introdotto un nuovo parametro per ENPACL "SendProtocolMessageFromViewerEnabled" il quale permette di 
rendere visibile tre pulsanti:Invia PEC, Invia Mail con protocollo e Invia PEC con protocollo, nella visualizzazione multipla di 
protocolli("Visualizza documenti")
[SendProtocolMessageFromViewerEnabled = true] abilita la visibilità del pulsanti
[SendProtocolMessageFromViewerEnabled = false ] non abilita tale visibilità
[ENPACL = true]

#GN
###################################################################################################

E' stata introdotta una nuova chiave di AppSettings per il WS WSSeries.

<add key="ConsultationIncludeDocumentStream" value="true" />

[AUSL-RE, ASMN-RE = true]
[Altri clienti = false]

Tale chiave gestisce la fase di consultazione di una serie documentale pubblicata.
Se value = true, la consultazione oltre ai metadati della serie includerà anche lo stream dei documenti.
Se value = false, la consultazione recupererà solo i metadati della serie (lo stream dovrà essere gestito successivamente).

Riassumendo, la chiave dovrà essere abilitata per quei clienti che non utilizzano il sito di AmministrazioneTrasparente (es. Sharepoint).

#AC
###################################################################################################

[ASL-TO]
E' stato introdotto un nuovo parametro per ASL-TO "AuthorizInsertProposerRoles" che se attivo 
nella fase di inserimento di un atto se si aggiunge un proponente, allora automaticamente si
aggiunge l'autorizzazione al settore del proponente 

[ASL-TO = 1]

#SZ
###################################################################################################

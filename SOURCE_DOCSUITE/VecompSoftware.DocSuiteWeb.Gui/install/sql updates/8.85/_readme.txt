Per un corretto funzionamento è necessario allineare i seguenti prodotti:
	- JeepService 8.85
	- WebAPI 8.85
	- ServiceBus Listeners 8.85
	- Workflow Integrations 8.85
	- FastProtocolImporter 8.85
	- Compilazione libreria dinamica UDS alla 8.85 (soli per i clienti che hanno già adottato il modulo UDS)
	- Eseguire la nuova versione dell'UDSMigrations 8.82

#FL
######################################################################################################
La procedura di migrazione del database prevede un ordine preciso di esecuzione degli script SQL. 
L'ordine da rispettare è il seguente:
	
	1. script DocSuite
		01. ProtocolloDB_migrations.sql
		02. PraticheDB_migrations.sql
		03. AttiDB_migrations.sql
	
	2. script WebAPI
	
#FL
######################################################################################################

Sono state apportate molte modifiche alle code e alle topic del service bus localizzate nel file SB01.Docsuitenamespace_Entities.xml.


In ricordo di Vittorio che spesso mi diceva “Assistenza non può permettesi di dipendere da Fabrizio per le attività di Deploy altrimenti se le fa Fabrizio”,
ho voluto risolvere i due BUG presenti nella versione del ServiceBus 2.3.1.0. 
Abbiamo crato una nuova versione “GED – ServiceBus Explorer 2.1.3.1” battezzandola “GED Edition”.
Con la versione “GED Edition”, potete procedere senza problemi, e dunque senza “dipendere da Fabrizio” come avrebbe voluto Vittorio.

#FL
######################################################################################################
E' stata creata una nuova componente "Rubrica smart" per la selezione/gestione dei contatti da Rubrica
in stile Google.
La componente si abilita tramite il parametro di ProtocolEnv "ContactSmartEnabled", il quale visualizzerà
nel controllo relativo ai contatti delle pagine di inserimento e modifica Protocollo una nuova icona
specifica.
Il parametro deve essere abilitato per il cliente AGENAS.

#AC
######################################################################################################

Introdotto nuovo parametro di ProtocolEnv 'FascicleRoleLabels' per la gestione delle etichette del disegno di funzione dei fascicoli

Esempio: 
Indicare per la singola funzione il nome più idoneo dal cliente (es. configurazione per standard)  
	{
		"ProdecureRoleName": "Responsabile di procedimento",
		"SecretaryRoleName": "Segreteria di procedimento"
	}

Per il cliente CNOCDL (Consiglio nazionale ordine dei consulenti del lavoro) impostare come segue:
        {
        "ProdecureRoleName": "Responsabile di funzione",
        "SecretaryRoleName": "Segreteria di funzione"
        }
	
#FL
######################################################################################################
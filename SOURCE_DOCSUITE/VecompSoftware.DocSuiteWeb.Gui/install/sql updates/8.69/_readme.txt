Per un corretto funzionamento è necessario allineare i seguenti prodotti:
    - WSProt 8.69
	- WebAPI 8.69
	- ServiceBus Listeners 8.69
	- Workflow Integrations 8.69
	
#FL
######################################################################################################

La procedura di migrazione del database prevede un ordine preciso di esecuzione degli script SQL. 
L'ordine da rispettare è il seguente:
	
	1. script DocSuite
		01. ProtocolloDB_migrations.sql
		02. PraticheDB_migrations.sql
		03. AttiDB_migrations.sql

	2. script WebAPI

#SZ
######################################################################################################

E' stata introdotta una nuova possibilità di comporre le segnature: se non si desidera anteporre il nome dell'azienda
in testa alla segnatura, occorre modificare i seguenti parametri: 

AnnexedSignatureFormat         
AttachmentSignatureFormat     
ProtocolSignatureFormat       

sostituendo la dicitura: {0:Short}, o {0:Complete} con {0:None}

Qualora tale parametro non sia presente, allora occorre aggiungere il parametro, impostandolo come segue

AnnexedSignatureFormat       {0:None}/{1:Number:0000000}/{1:Date:dd/MM/yyyy}/{1:Direction:Short}/{1:Container:Name}
AttachmentSignatureFormat    {0:None}/{1:Number:0000000}/{1:Date:dd/MM/yyyy}/{1:Direction:Short}/{1:Container:Name}&lt;{2:DocumentType:Short}.{2:DocumentNumber}&gt;
ProtocolSignatureFormat      {0:None}/{1:Number:0000000}/{1:Date:dd/MM/yyyy}/{1:Direction:Short}/{1:Container:Name}&lt;{2:DocumentType:Short}/AA.{2:AttachmentsCount}&gt;

#MM
######################################################################################################
E' stata aggiornato il menu, in particolare è stata aggiunta la voce "Avvia fascicoli periodici" al menù dei fascicoli

  "Menu7": {
    "Name": "Fascicoli",
    "Nodes": {
      "FirstNode1": { "Name": "Inserimento" },
      "FirstNode2": { "Name": "Ricerca" },
      "FirstNode3": {
        "Name": "Scrivania",
        "Nodes": {
          "SecondNode1": { "Name": "Documenti da Fascicolare" }
        }
      },
      "FirstNode4": { "Name": "Avvia fascicoli periodici" }
    }

#MM
######################################################################################################
E' possibile che per problemi di collation emergano errori durante l'inserimento di un fascicolo di procedimento.
Se si è già selezionato un classificatore e si cerca di inserire un contatto, allora potrebbe esserci un errore di collation.
Per risolverlo basta modificare la collation delle colonne EmailAddress e CertifydMail.

Di seguito un esempio dello script per correggere l'errore:

ALTER TABLE dbo.Contact ALTER COLUMN EMailAddress
            nvarchar(256)COLLATE Latin1_General_CI_AS NOT NULL;
GO

#SZ
######################################################################################################

Sono state apportate molte modifiche alle code e alle topic del service bus.
E' necessario procecede come segue:
	- aprire il Tool Service Bus Explorer
	- specificare la ConnectionString del service bus del cliente
	- Eliminare la topic di nome "entity_event", situata nel nodo principale "Topics"
	- dal menu Actions selezionare la voce Import Entries, nella finestra importare il file SB01.Docsuitenamespace_Entity_Event_Topic.xml

Se da errori in fase di import, contattare sviluppo. Il prodotto senza tali configurazioni potrebbe creare anomalie critiche. 
#FL
######################################################################################################
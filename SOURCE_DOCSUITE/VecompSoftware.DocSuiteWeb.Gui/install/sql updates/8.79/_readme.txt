Per un corretto funzionamento è necessario allineare i seguenti prodotti:
	- WebAPI 8.79
	- BiblosDS 8.79
	- ServiceBus Listeners 8.79
	- Workflow Integrations 8.79 - Per i soli clienti che hanno attivato il modulo di fatturazione elettronica
	- Compilazione libreria dinamica UDS alla 8.79 (soli per i clienti che hanno già adottato il modulo UDS)

#FL
######################################################################################################
La procedura di migrazione del database prevede un ordine preciso di esecuzione degli script SQL. 
L'ordine da rispettare è il seguente:
	
	1. script DocSuite
		01. ProtocolloDB_migrations.sql (N.B prima di lanciare lo script, verificare che esista almeno un SecurityGroup con AllUsers = 1)
		02. PraticheDB_migrations.sql
		03. AttiDB_migrations.sql
	
	2. script WebAPI

	3. Bonificare a mano tutti gli XML aggiungendo l'attributo RequiredRevisionUDSRepository="true" nel blocco principale del tag UnitaDocumentariaSpecifica 
	
#FL
######################################################################################################
Sono state apportate molte modifiche alle code e alle topic del service bus localizzate nel file SB01.Docsuitenamespace_Entities.xml.

Considerando la criticità del modello di deploy mediante tool Service Bus Explorer, è neccessario delegare l'attività a Fabrizio Lazzarotto. 
Le attività possono essere svolte anche dopo il deploy a servizi attivi.

#FL
######################################################################################################

PER AUSLRE: Post aggiornamento è necessario aggiornare il parametro di protocollo "ExternalViewerMyDocuments" col seguente valore:

https://mydocsuite.ausl.re.it/#/Protocollo/identificativo/{0}/Sommario

#FL
######################################################################################################

PER AFOL: Disattivare il nuovo parametro PraticheEnabled
#FL
######################################################################################################

PER AUSL-PC: Prendere accordi per la migrazione del valore da attribuire ai recordi esistenti sulla colonna Status ProtocolRoleUser post migrazione.
E' necessario associare il valore 1 alla colonna Status quando l'utente assegnatario del protocollo ha letto il protocollo.
Genericamente tutti i ProtocolRoleUser del mese precedente dalla data di migrazione possono essere bonificati mettendo il valore 1, 
diversamente va verificata l'esistenza del Log "Visualizzazione" per determinare se il protocollo è stato lavorato.

#FL
######################################################################################################
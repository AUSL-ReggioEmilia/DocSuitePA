Per un corretto funzionamento è necessario allineare i seguenti prodotti:
	- WebAPI 8.75
	- ServiceBus Listeners 8.75
	- Workflow Integrations 8.75
	- Compilazione libreria dinamica UDS alla 8.75 (soli per i clienti che hanno già adottato il modulo UDS)

#FL
######################################################################################################
La procedura di migrazione del database prevede un ordine preciso di esecuzione degli script SQL. 
L'ordine da rispettare è il seguente:
	
	1. script DocSuite
		01. ProtocolloDB_migrations.sql
		02. PraticheDB_migrations.sql
		03. AttiDB_migrations.sql
	
#FL
######################################################################################################
Per risolvere le problematiche di visualizzazione documenti PDF in chrome, è necessario aggiornare il contenuto
della cartella Help con quello fornito nel pacchetto di installazione.

Se su qualche postazione client, chrome non visualizza i file PDF all'interno del browser, verificare le seguenti
impostazioni:
	1) Andare nelle impostazioni di chrome col il pulsante in alto a destra.
	2) In basso cliccare su "Advanced settings".
	3) Sotto a privacy cliccare su "Content settings".
	4) Sotto a PDF Documents verificare che il flag "Download PDF files instead of automatically opening them in Chrome" sia disabilitato.

#AC
######################################################################################################
ATTENZIONE! Si ricorda che nella proprietà DomainAddress del parametro TenantModel 
non va mai indicato il nome del dominio, ma solamente il nome DNS per esteso o IP del domain controller.
Nel caso il server non sia in dominio, specificare solamente il nome del server.

#AC
######################################################################################################
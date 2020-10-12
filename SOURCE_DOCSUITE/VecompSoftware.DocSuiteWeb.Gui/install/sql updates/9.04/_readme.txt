ATTENZIONE LA BUILD RICHIEDE IL FRAMEWORK .NET 4.8. 
PRIMA DI PROCEDERE ALL'INSTALLAZIONE E' NECESSARIO CANCELLARE IL CONTENUTO DELLA CARTELLA BIN

#FL
######################################################################################################
Per un corretto funzionamento è necessario allineare i seguenti prodotti:
	- WebAPI 9.04
	- Utilizzare il tool di migrazione dei Metadati DSWMetadataMigrator.

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
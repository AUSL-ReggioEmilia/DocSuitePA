Per un corretto funzionamento è necessario allineare i seguenti prodotti:
	- WebAPI 8.66
	- ServiceBus Listeners 8.66

#FL
############################################################################################################################################################################################################
La procedura di migrazione del database prevede un ordine preciso di esecuzione degli script SQL. 
L'ordine da rispettare è il seguente:
	
	1. script DocSuite
		01. ProtocolloDB_migrations.sql
		02. PraticheDB_migrations.sql
		03. AttiDB_migrations.sql

	2. script WebAPI

#FL
######################################################################################################
In questa versione è stata introdotta nei Dossier la gerarchia delle cartelle, quindi la possibilità di creare una struttura di sottocartelle figlie partendo da una cartella.
Prima di lanciare lo script 01.ProtocolloDB_migrations, accertarsi che le tabelle dei Dossiers siano vuote. Se così non fosse cancellare tutti i Dossier presenti 
con le relative relazioni (DossierComments, DossierContacts, DossierDocuemnts, DossierFolderRoles, DossierFolders, DossierLogs, DossierMessages, DossierRoles).

#SDC
###########################################################################################
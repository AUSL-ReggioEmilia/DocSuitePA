Per un corretto funzionamento è necessario allineare i seguenti prodotti:
	- WebAPI 8.78
	- ServiceBus Listeners 8.78
	- Workflow Integrations 8.78
	- Compilazione libreria dinamica UDS alla 8.78 (soli per i clienti che hanno già adottato il modulo UDS)
	- DocumentUnitMigrator 8.78
	- BiblosDS 8.76 (vedi specifico readme fornito nel pacchetto di installazione)

#FL
######################################################################################################

In questa versione è stata definitvamente dismessa la componente ActiveX (ThinClient2003 del pacchetto DSViewWeb.msi) in favore delle componenti ActiveX native in Microsoft
per la gestione del checkin/checkout di Office.

E’ necessario essere sicuri che le impostazioni di sicurezza di IE siano in regola coi seguenti punti:
	- le componenti Microsoft Office installate sulle postazioni 
	- Il sito DNS ufficale della docsuite sia riconosciuto come “Local Intranet” da IE
	- Il sito DNS  docsuite NON sia riconosciuta da IE in compatibilità
    - Nelle impostazioni di sicurezza avanzate della zona “Local Intranet” sia abilitato il parametro "Initialize and script active x controls is not marked safe for scripting" 

#FL
######################################################################################################

Per un bug introdotto nella versione 8.74 è necessario rilasciare il tool DSWHashGeneratorMigrator 8.78 fornito in questo rilascio.

#FL
######################################################################################################
Una collaborazione restituita al Proponente è modificabile (modifica e aggiunta documenti) se sono attivi
i parametri di ParameterEnv CollDocumentEditable (documento principale) e CollAttachmentEditable (allegati e annessi).
Questa configurazione è da impostare in ENPACL e per tutti i clienti che lo richiedano.

#AC
######################################################################################################
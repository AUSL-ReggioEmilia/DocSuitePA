Per far sì che la data di Adozione non sia modificabile è necessario che il campo ManagedWorkflowData
della tabella TabWorkflow per lo step di Adozione NON abbia il valore INS nella parte relativa alla Date.
 
Esempio
Date[.INS] = data modificable dall'utente
Date[] = data non modificabile dall'utente

Per ASL-TO se si vuole che le deliberazioni abbiano un comportamento diverso dalle determinazioni sarà necessario definire due workflow diversi.

#SZ
###########################################################

Per far sì che la data di adozione assuma il valore della data del giorno corrente è necessario impostare
il campo ManagedWorkflowData della tabella TabWorkflow per lo step di Adozione col valore TODAY nella parte relativa a Date

Esempio Date[.TODAY]

Per ASL-TO se si vuole che le deliberazioni abbiano un comportamento diverso dalle determinazioni sarà necessario definire due workflow diversi.

#SZ
###########################################################

[AUSL-PC]
Per attivare la funzionalità di adozione massiva delle Delibere a PC è necessario modificare il file RicercaFlussoConfig.json nel seguente modo

{
  //Determina
  "0": {    
    "11": "Ultima Pagina (Dematerializzazione) Firma Digitale"
  },
  //Delibera
  "1": {
    "0": "Adozione",
    "11": "Ultima Pagina (Dematerializzazione) Firma Digitale"
  }
}

#SZ
###########################################################
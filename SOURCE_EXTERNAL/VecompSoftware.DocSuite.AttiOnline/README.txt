
ATTENZIONE: Prima di procedere, assicurarsi che il DocumentHandler sia installato in IIS.
            L'Albo pretorio necessita delle WebAPI 8.61 per funzionare.

Le librerie necessarie all'applicazione saranno fornite in uno zip nell'ambiente di TFS build.

Terminati gli step di installazione, va configurato il file presente al percorso VecompSoftware.DocSuite.AttiOnline\app\config\settings.ts indicando 
	1. in 'apiOdataAddress' l'url delle WebAPI PUBBLICHE per le chiamate OData
	2. in 'executiveDocumentHandlerUrl' l'url dell'handler dei documenti per gli atti esecutivi
    3. in 'publishedDocumentHandlerUrl' l'url dell'handler dei documenti per gli atti pubblicati non esecutivi
    4. in 'alboPretorioDocumentHandlerUrl' l'url dell'handler dei documenti per l'albo pretorio
	5. in 'gridItemsNumber' il numero di elementi da mostrare in ogni pagina della griglia (di default 10)
	6. in 'toastLife' il tempo massimo (in millisecondi) di visualizzazione delle toast notification, che appaiono per comunicare errori relativi alle chiamate alle WebAPI o alla composizione dell'URL (di default 5000)
    7. in 'alboPretorioServiceColumnVisibility' va settata la visibilità della colonna che mostra il Servizio dell'atto nella griglia dell'Albo pretorio (impostare a True o False)
       [AUSL-RE] = true
       [ASL-TO] = false

    8. in 'alboPretorioProposerColumnVisibility' va settata la visibilità della colonna che mostra il Proponente dell'atto nella griglia dell'Albo pretorio (impostare a True o False)
       [AUSL-RE] = false
       [ASL-TO] = true

    9. in 'executiveConsultationServiceColumnVisibility' va settata la visibilità della colonna che mostra il Servizio dell'atto nella griglia di consultazione atti esecutivi (impostare a True o False)
       [AUSL-RE] = true
       [ASL-TO] = false

    10. in 'executiveConsultationProposerColumnVisibility' va settata la visibilità della colonna che mostra il Proponente dell'atto nella griglia di consultazione atti esecutivi (impostare a True o False)
        [AUSL-RE] = false
        [ASL-TO] = true

    11. in 'publishedConsultationServiceColumnVisibility' va settata la visibilità della colonna che mostra il Servizio dell'atto nella griglia di consultazione atti pubblicati non esecutivi (impostare a True o False)
        [AUSL-RE] = true
        [ASL-TO] = false

    12. in 'publishedConsultationProposerColumnVisibility' va settata la visibilità della colonna che mostra il Proponente dell'atto nella griglia di consultazione atti pubblicati non esecutivi (impostare a True o False)
        [AUSL-RE] = false
        [ASL-TO] = true

    13. in 'attiGroupableGridEnabled' va settata la possibilità di raggruppare gli atti per Servizio nella griglia dell'Albo pretorio (impostare a True o False)
        [AUSL-RE] = true
        [ASL-TO] = false

    14. in 'attiPublicationDateSorting' va settato il tipo di ordinamento con cui visualizzare gli atti nella griglia dell'Albo Pretorio (impostare 'asc' per cresecente o 'desc' per decrescente)
        [AUSL-RE] = "asc"

    15. in 'deliberePublicationDateSorting' va settato il tipo di ordinamento con cui visualizzare le delibere nella griglia dell'Albo Pretorio (impostare 'asc' per cresecente o 'desc' per decrescente)
        [AUSL-RE] = "desc"

    16. in 'activeRoutes' sono indicate le possibili rotte dell'applicazione da attivare con la seguente struttura 
                                   activeRoutes: {
                                     executiveConsultation: true,
                                     publishedConsultation: true,
                                     alboPretorio: true
                                   }
       executiveConsultation -> rotta che permette di visualizzare tutti gli atti esecutivi
       publishedConsultation -> rotta che permette di visualizzare tutti gli atti pubblicati non ancora esecutivi
       alboPretorio -> rotta che permette di visualizzare tutti gli atti pubblicati sull'albo pretorio
    17. in 'delibere' va impostato il nome che compare nell'applicazione per indicare le delibere (cambia in base al cliente)
    18. in 'determine' va impostato il nome che compare nell'applicazione per indicare le determine (cambia in base al cliente)
    19. in 'headerUrl' va impostato l'url dell'header del cliente specifico
    20. in 'footerUrl' va impostato l'url del footer del cliente specifico


#SDC
##############################################################################################################


''' <summary> Interfaccia che permette la verifica della storicizzazione logica degli oggetti Contact e Role </summary>
''' <remarks> Permette di verificare se un oggetto è stato cambiato o no. </remarks>
Public Interface ISupportChanged
    ''' <summary> Indica se l'oggetto è cambiato o meno </summary>
    Property IsChanged As Short
End Interface


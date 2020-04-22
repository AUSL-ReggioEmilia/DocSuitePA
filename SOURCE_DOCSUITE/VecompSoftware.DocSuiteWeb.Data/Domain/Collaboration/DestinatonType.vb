''' <summary>
''' Enumeratore che identifica la tipologia di destinatario protocollatore
''' </summary>
''' <remarks></remarks>
Public Enum DestinatonType
    ''' <summary>
    ''' Persona
    ''' In tal caso destinationUser conterrà l'account dell'utente che sarà incaricato di protocollare la collaborazione
    '''  </summary>
    P
    ''' <summary>
    ''' Settore
    ''' In tal caso DestinationUser conterrà l'id del settore che sarà incaricato di protocollare la collaborazione
    ''' </summary>
    S
End Enum
''' <summary> Interfaccia per indicare la validità del dato </summary>
''' <remarks> 
''' TODO: da inserire in ISupportLogicDelete?
''' </remarks>
Public Interface ISupportRangeDelete
    ''' <summary> Data dalla quale è valido il dato </summary>
    Property ActiveFrom() As DateTime?
    ''' <summary> Data fino alla quale è valido il dato </summary>
    Property ActiveTo() As DateTime?
    ''' <summary> Indica se è correntemente attivo in data odierna. </summary>
    Function IsActiveRange() As Boolean
End Interface

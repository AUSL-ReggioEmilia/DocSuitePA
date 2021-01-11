
Imports System.ComponentModel
''' <summary>
''' Enumeratore dei nomi delle <see>SessionFactory</see> corrispondenti agli ambienti
''' </summary>
''' <remarks>
''' Per ottenere la stringa corrispondente:
''' <example>
''' System.Enum.GetName(GetType(EnvironmentDataCode), EnvironmentDataCode.ProtDB) 
''' </example>.
''' Da usare al posto delle costanti <see>ProtDB</see>, <see>DocmDB</see> e <see>ReslDB</see>
''' </remarks>
Public Enum EnvironmentDataCode
    ''' <summary>Protocollo</summary>
    <Description("ProtDB")>
    ProtDB
    ''' <summary>Atti</summary>
    <Description("ReslDB")>
    ReslDB
    ''' <summary>Pratiche</summary>
    <Description("DocmDB")>
    DocmDB
End Enum
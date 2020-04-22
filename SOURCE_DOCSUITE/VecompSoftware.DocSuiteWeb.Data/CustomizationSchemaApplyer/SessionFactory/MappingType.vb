Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Xml

Public Structure MappingType

    Public Property Mappings As ICollection(Of XmlDocument)

    Public Property AssemblyNames As ICollection(Of String)
End Structure
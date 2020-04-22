
Public Class ProtocolImportFactory
	Public Shared Function GetProtocolImporter(ByVal protocolImportClassName As String) As IProtocolImporter
		Dim myType As Type = Type.GetType(protocolImportClassName, True, False)
		Dim o As Object = Activator.CreateInstance(myType)

		If TypeOf (o) Is IProtocolImporter Then
			Return CType(o, IProtocolImporter)
		Else
            Throw New Exception(String.Format("L'oggetto '{0}' caricato dinamicamente non implementa l'interfaccia 'IProtocolImporter'."))
		End If
	End Function
End Class

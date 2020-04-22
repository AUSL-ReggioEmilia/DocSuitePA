Imports System.Runtime.CompilerServices
Imports VecompSoftware.DocSuiteWeb.Data

Namespace ExtensionMethods

    Public Module PecMailBoxEx

        <Extension()>
        Public Function PecHandleEnabled(ByRef pecMailBox As PECMailBox) As Boolean
            Return DocSuiteContext.Current.ProtocolEnv.PECHandlerEnabled AndAlso _
                pecMailBox.IsHandleEnabled.HasValue AndAlso _
                pecMailBox.IsHandleEnabled.Value
        End Function

    End Module

End Namespace
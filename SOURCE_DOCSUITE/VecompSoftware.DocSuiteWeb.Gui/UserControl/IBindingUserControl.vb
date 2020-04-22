Imports System.Collections.Generic

Public Interface IBindingUserControl(Of TModel)
    Property DataSource() As IList(Of TModel)
    Sub DataBind()
End Interface

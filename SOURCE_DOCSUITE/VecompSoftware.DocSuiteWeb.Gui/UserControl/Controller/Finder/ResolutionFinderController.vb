
Imports VecompSoftware.DocSuiteWeb.Data

Public Class ResolutionFinderController
    Inherits BaseResolutionFinderController

#Region "Constructor"
    Public Sub New(ByRef uscControl As uscResolutionFinder)
        MyBase.New(uscControl)
    End Sub
#End Region

#Region "Initialize"
    Public Overrides Sub Initialize()
        _uscFinder.ControlProposerInterop.Visible = Facade.ResolutionFacade.IsManagedProperty("Proposer", ResolutionType.IdentifierDetermina, "CONTACT")
        _uscFinder.ControlRecipientInterop.Visible = Facade.ResolutionFacade.IsManagedProperty("Recipient", ResolutionType.IdentifierDetermina, "CONTACT")
        _uscFinder.ControlAssigneeInterop.Visible = Facade.ResolutionFacade.IsManagedProperty("Assignee", ResolutionType.IdentifierDetermina, "CONTACT")
        _uscFinder.ControlManagerInterop.Visible = Facade.ResolutionFacade.IsManagedProperty("Manager", ResolutionType.IdentifierDetermina, "CONTACT")

        _uscFinder.ControlRecipient.ButtonsVisible = Facade.ResolutionFacade.IsManagedProperty("Recipient", ResolutionType.IdentifierDetermina, "BO")
        _uscFinder.ControlProposer.ButtonsVisible = Facade.ResolutionFacade.IsManagedProperty("Proposer", ResolutionType.IdentifierDetermina, "BO")
        _uscFinder.ControlAssignee.ButtonsVisible = Facade.ResolutionFacade.IsManagedProperty("Assignee", ResolutionType.IdentifierDetermina, "BO")
        _uscFinder.ControlManager.ButtonsVisible = Facade.ResolutionFacade.IsManagedProperty("Manager", ResolutionType.IdentifierDetermina, "BO")

        _uscFinder.VisibleServiceNumber = Facade.ResolutionFacade.IsManagedProperty("ServiceNumber", ResolutionType.IdentifierDetermina)
        _uscFinder.VisibleNumber = Facade.ResolutionFacade.IsManagedProperty("Number", ResolutionType.IdentifierDelibera)
        _uscFinder.VisibleIdResolution = Facade.ResolutionFacade.IsManagedProperty("idResolution", ResolutionType.IdentifierDelibera)
        _uscFinder.VisibleCategory = Facade.ResolutionFacade.IsManagedProperty("Category", ResolutionType.IdentifierDelibera)

        _uscFinder.VisibleImmediatelyExecutive = False
        _uscFinder.VisibleAdoptionDate = False
        _uscFinder.VisibleProposerDate = False
        _uscFinder.VisibleOCList = False
        _uscFinder.VisibleStatusCancel = False
        _uscFinder.VisibleOnlytatusCancel = False

        _uscFinder.VisibleStatoContabilita = DocSuiteContext.Current.ResolutionEnv.ResolutionAccountingEnabled

        _uscFinder.VisibilePrivacyPublication = DocSuiteContext.Current.ResolutionEnv.WebPublicationPrint
    End Sub
#End Region

End Class

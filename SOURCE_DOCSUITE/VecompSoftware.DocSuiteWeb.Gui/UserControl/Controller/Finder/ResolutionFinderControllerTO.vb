Imports VecompSoftware.DocSuiteWeb.Data

Public Class ResolutionFinderControllerTO
    Inherits BaseResolutionFinderController

#Region "Constructor"
    Public Sub New(ByRef uscControl As uscResolutionFinder)
        MyBase.New(uscControl)
    End Sub
#End Region

    Public Overrides Sub Initialize()
        _uscFinder.ControlProposerInterop.ContactRoot = DocSuiteContext.Current.ResolutionEnv.ProposerContact
        _uscFinder.ControlProposerInterop.Visible = Facade.ResolutionFacade.IsManagedProperty("Proposer", ResolutionType.IdentifierDetermina, "CONTACT")
        _uscFinder.ControlRecipientInterop.Visible = Facade.ResolutionFacade.IsManagedProperty("Recipient", ResolutionType.IdentifierDetermina, "CONTACT")
        _uscFinder.ControlAssigneeInterop.Visible = Facade.ResolutionFacade.IsManagedProperty("Assignee", ResolutionType.IdentifierDetermina, "CONTACT")
        _uscFinder.ControlManagerInterop.Visible = Facade.ResolutionFacade.IsManagedProperty("Manager", ResolutionType.IdentifierDetermina, "CONTACT")

        _uscFinder.ControlRecipient.ButtonsVisible = Facade.ResolutionFacade.IsManagedProperty("Recipient", ResolutionType.IdentifierDetermina, "BO")
        _uscFinder.ControlProposer.ButtonsVisible = Facade.ResolutionFacade.IsManagedProperty("Proposer", ResolutionType.IdentifierDetermina, "BO")
        _uscFinder.ControlAssignee.ButtonsVisible = Facade.ResolutionFacade.IsManagedProperty("Assignee", ResolutionType.IdentifierDetermina, "BO")
        _uscFinder.ControlManager.ButtonsVisible = Facade.ResolutionFacade.IsManagedProperty("Manager", ResolutionType.IdentifierDetermina, "BO")

        _uscFinder.VisibleServiceNumber = Facade.ResolutionFacade.IsManagedProperty("ServiceNumber", ResolutionType.IdentifierDelibera)
        _uscFinder.VisibleNumber = Facade.ResolutionFacade.IsManagedProperty("Number", ResolutionType.IdentifierDelibera)
        _uscFinder.VisibleIdResolution = Facade.ResolutionFacade.IsManagedProperty("idResolution", ResolutionType.IdentifierDelibera)
        _uscFinder.VisibleCategory = Facade.ResolutionFacade.IsManagedProperty("Category", ResolutionType.IdentifierDelibera)

        _uscFinder.VisibleImmediatelyExecutive = True
        _uscFinder.VisibleAdoptionDate = True
        _uscFinder.VisibleProposerDate = True
        _uscFinder.VisibleOCList = True
        _uscFinder.VisibleStatusCancel = True
        _uscFinder.VisibleOnlytatusCancel = True

        _uscFinder.VisibleWorkflowStep = False
        _uscFinder.VisibleOC = False
        _uscFinder.VisibileOC_Management = False
        _uscFinder.VisibleDateStep = False
        _uscFinder.VisibleAssigneeContact = False
        _uscFinder.VisibleManagerContact = False
        _uscFinder.VisibleRecipientContact = False
    End Sub
End Class

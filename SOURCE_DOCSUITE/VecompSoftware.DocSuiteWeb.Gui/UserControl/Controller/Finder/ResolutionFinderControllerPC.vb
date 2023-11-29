Imports VecompSoftware.DocSuiteWeb.Data

Public Class ResolutionFinderControllerPC
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

        _uscFinder.VisibleServiceNumber = False
        _uscFinder.VisibleNumber = False
        _uscFinder.VisibleAUSLPCNumber = True 'EF 20120126 Attivazione della ricerca mista Delibere/Determine per numero
        _uscFinder.VisibleIdResolution = False
        _uscFinder.VisibleCategory = Facade.ResolutionFacade.IsManagedProperty("Category", ResolutionType.IdentifierDelibera)

        _uscFinder.VisibleImmediatelyExecutive = DocSuiteContext.Current.ResolutionEnv.ImmediatelyExecutiveEnabled
        _uscFinder.VisibleAdoptionDate = True
        _uscFinder.VisibleProposerDate = True

        'Gestione atti annullati
        _uscFinder.VisibleStatusCancel = True
        _uscFinder.VisibleOnlytatusCancel = True

        _uscFinder.VisibleWorkflowStep = False
        _uscFinder.VisibleOC = False
        _uscFinder.VisibleDateStep = False
        _uscFinder.VisibleAssigneeContact = False
        _uscFinder.VisibleManagerContact = False
        _uscFinder.VisibleRecipientContact = False

        _uscFinder.VisibleOCComment = False
        _uscFinder.VisibleOCOpinion = False
        _uscFinder.VisibleProposerContact = True

        'Gestione organi di Controllo
        _uscFinder.VisibleOCList = True
        _uscFinder.VisibileOC_SupervisoryBoard = Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDelibera, "CS") Or Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDetermina, "CS")
        _uscFinder.VisibileOC_ConfSind = Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDelibera, "CONFSIND") Or Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDetermina, "CONFSIND")
        _uscFinder.VisibileOC_CorteConti = Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDelibera, "CC") Or Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDetermina, "CC")
        _uscFinder.VisibileOC_Management = Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDelibera, "GEST") Or Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDetermina, "GEST")
        _uscFinder.VisibileOC_Other = Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDelibera, "ALTRO") Or Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDetermina, "ALTRO")
        _uscFinder.VisibileOC_Region = Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDelibera, "REG") Or Facade.ResolutionFacade.IsManagedProperty("OCData", ResolutionType.IdentifierDetermina, "REG")

        _uscFinder.VisibilePrivacyPublication = DocSuiteContext.Current.ResolutionEnv.WebPublicationPrint

        _uscFinder.VisibleStatoContabilita = DocSuiteContext.Current.ResolutionEnv.ResolutionAccountingEnabled
    End Sub
End Class

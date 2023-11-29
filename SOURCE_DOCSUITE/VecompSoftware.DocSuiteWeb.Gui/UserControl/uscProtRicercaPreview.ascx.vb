Imports Telerik.Web.UI
Imports VecompSoftware.Helpers.ExtensionMethods
Imports VecompSoftware.Helpers.Web.ExtensionMethods

Public Class uscProtRicercaPreview
    Inherits DocSuite2008BaseControl

#Region " Events "
    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Not Page.IsPostBack Then
            Initialize()
        End If
    End Sub
#End Region

#Region " Methods "
    Private Sub Initialize()
        If ProtocolEnv.AutocompleteContainer Then
            rcbContainer.Style.Remove("display")
        Else
            ddlContainer.Style.Remove("display")
        End If
        sett.SetDisplay(ProtocolEnv.RolesUserProfileEnabled)
        If ProtocolEnv.ProtParzialeEnabled Then
            rowIncomplete.Style.Add("display", "")
        Else
            rowIncomplete.Style.Add("display", "none")
        End If

        If ProtocolEnv.CorporateAcronym.Contains("ENPACL") Then
            chbCategoryChild.Checked = True
        End If
        cbOnlyMyProt.Checked = False

        SetSpecificFilterVisibility()
    End Sub

    Private Sub SetSpecificFilterVisibility()

        rowClaim.Visible = ProtocolEnv.IsClaimEnabled
        rowInterop.SetDisplay(ProtocolEnv.IsInteropEnabled)
        rowInteropExtended.Style.Add("display", "none")
        chbCategoryChild.Visible = False

        rowIdDocType.Visible = ProtocolEnv.IsTableDocTypeEnabled
        rowStatusSearch.Visible = ProtocolEnv.IsStatusEnabled
        rowPackage.Visible = ProtocolEnv.IsPackageEnabled

        rowInvoice.Visible = ProtocolEnv.IsInvoiceEnabled
        rowInvoice2.Visible = ProtocolEnv.IsInvoiceEnabled
        rowInvoice3.Visible = ProtocolEnv.IsInvoiceEnabled

        rowPerson.Visible = ProtocolEnv.IsProtSearchTitleEnabled
        rowPerson2.Visible = ProtocolEnv.IsProtSearchTitleEnabled
        rowPerson3.Visible = ProtocolEnv.IsProtSearchTitleEnabled

        rowLogType.Visible = ProtocolEnv.IsLogStatusEnabled

        rowPec.Visible = ProtocolEnv.IsPECEnabled
        rowDistribution.Visible = ProtocolEnv.IsDistributionEnabled
        rowHighlight.Visible = ProtocolEnv.ProtocolHighlightEnabled

        InitializeProtocolContactTextSearch()
    End Sub

    Private Sub InitializeProtocolContactTextSearch()
        Select Case ProtocolEnv.ProtocolContactTextSearchMode
            Case 0
                pnlDescriptionSearchBehaviour.Visible = False
                pnlLegacyDescriptionSearchBehaviour.Visible = True
                chkRecipientContains.Visible = True
            Case 1
                pnlLegacyDescriptionSearchBehaviour.Visible = False
                pnlDescriptionSearchBehaviour.Visible = True
                If pnlDescriptionSearchBehaviour.Visible Then
                    rblTextMatchMode.SelectedValue = ProtocolEnv.DefaultTextMatchMode
                End If
            Case 2
                pnlDescriptionSearchBehaviour.Visible = False
                pnlLegacyDescriptionSearchBehaviour.Visible = True
                chkRecipientContains.Visible = False
        End Select
    End Sub

    Public Sub ChangeControlEnable(controlId As String, enable As Boolean)
        Dim item As Control = searchTable.FindControl(controlId)
        If item IsNot Nothing Then
            Select Case True
                Case TypeOf item Is RadTextBox
                    Dim casted As RadTextBox = DirectCast(item, RadTextBox)
                    Select Case casted.ID
                        Case txtObjectProtocol.ID
                            casted.Enabled = enable
                            rblClausola.Enabled = enable

                        Case txtRecipient.ID,
                            txtContactDescription.ID
                            casted.Enabled = enable
                            Select Case ProtocolEnv.ProtocolContactTextSearchMode
                                Case 0
                                    chkRecipientContains.Enabled = enable
                                Case 1
                                    rblAtLeastOne.Enabled = enable
                                    rblTextMatchMode.Enabled = enable
                                Case 2
                                    chkRecipientContains.Enabled = enable
                            End Select

                        Case Else
                            casted.Enabled = enable
                    End Select

                Case TypeOf item Is CheckBox
                    Dim casted As CheckBox = DirectCast(item, CheckBox)
                    casted.Enabled = enable

                Case TypeOf item Is DropDownList
                    Dim casted As DropDownList = DirectCast(item, DropDownList)
                    casted.Enabled = enable

                Case TypeOf item Is RadioButtonList
                    Dim casted As RadioButtonList = DirectCast(item, RadioButtonList)
                    casted.Enabled = enable

                Case TypeOf item Is RadNumericTextBox
                    Dim casted As RadNumericTextBox = DirectCast(item, RadNumericTextBox)
                    casted.Enabled = enable

                Case TypeOf item Is RadDropDownList
                    Dim casted As RadDropDownList = DirectCast(item, RadDropDownList)
                    casted.Enabled = enable

                Case TypeOf item Is uscContattiSel
                    Dim casted As uscContattiSel = DirectCast(item, uscContattiSel)
                    casted.TreeViewControl.Visible = enable


                Case TypeOf item Is uscClassificatore
                    Dim casted As uscClassificatore = DirectCast(item, uscClassificatore)
                    casted.ReadOnly = Not enable

                Case TypeOf item Is uscSettori
                    Dim casted As uscSettori = DirectCast(item, uscSettori)
                    casted.ReadOnly = Not enable
                    casted.Initialize()


                Case TypeOf item Is RadDatePicker
                    Dim casted As RadDatePicker = DirectCast(item, RadDatePicker)
                    Select Case casted.ID
                        Case txtRegistrationDateFrom.ID,
                             txtRegistrationDateTo.ID
                            txtRegistrationDateFrom.Enabled = enable
                            txtRegistrationDateTo.Enabled = enable

                        Case InvoiceDateFrom.ID,
                            InvoiceDateTo.ID
                            InvoiceDateFrom.Enabled = enable
                            InvoiceDateTo.Enabled = enable

                        Case txtDocumentDateFrom.ID,
                        txtDocumentDateTo.ID
                            txtDocumentDateFrom.Enabled = enable
                            txtDocumentDateTo.Enabled = enable

                        Case Else
                            casted.Enabled = enable
                    End Select

                Case TypeOf item Is RadComboBox
                    Dim casted As RadComboBox = DirectCast(item, RadComboBox)
                    If casted.ID.Eq(rcbContainer.ID) OrElse casted.ID.Eq(ddlContainer.ID) Then
                        If ProtocolEnv.AutocompleteContainer Then
                            rcbContainer.Enabled = enable
                        Else
                            ddlContainer.Enabled = enable
                        End If
                    Else
                        casted.Enabled = enable
                    End If
            End Select
        End If
    End Sub
#End Region

End Class
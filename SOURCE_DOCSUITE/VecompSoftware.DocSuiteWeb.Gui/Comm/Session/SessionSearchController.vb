Imports System.Web

Public Class SessionSearchController

    Public Enum SessionFinderType
        DocmFinderType = 0
        ProtFinderType = 1
        ReslFinderType = 2
        FascFinderType = 3
        ReslFlussoFinderType = 4
        CommProtFascFinderType = 5
        CommDocmFascFinderType = 6
        ProtocolParerFinderType = 8
        ResolutionParerFinderType = 9
        PecInFinderType = 10
        PecOutFinderType = 11
        DocumentSeriesFinderType = 12
        ProtocolBoxFinderType = 13
        MessageEmailFinderType = 14
        FastProtocolSender = 15
        FastPecSender = 16
        PosteOnlineRequestFinder = 17
        FastPOLSender = 18
        DeskStoryBoardFinder = 19
    End Enum

    Private Const FINDER_ALIAS As String = "_sessionFinder"
    Private Const SETTINGS_ALIAS As String = "_sessionSetting"

    Public Shared Sub SaveSessionFinder(ByVal finder As Object, ByVal uniqueId As SessionFinderType)
        HttpContext.Current.Session(uniqueID.ToString() & FINDER_ALIAS) = finder
    End Sub

    Public Shared Function LoadSessionFinder(ByVal uniqueId As SessionFinderType) As Object
        Return HttpContext.Current.Session(uniqueID.ToString() & FINDER_ALIAS)
    End Function

    Public Shared Sub SaveGridSettings(ByVal grid As BindGrid)
        grid.SaveSettings()
    End Sub

    Public Shared Function LoadGridSettings(ByRef grid As BindGrid) As BindGrid
        grid.LoadSettings(CType(HttpContext.Current.Session(grid.UniqueID & SETTINGS_ALIAS), String))
        Return grid
    End Function

End Class

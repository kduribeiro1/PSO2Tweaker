Imports Microsoft.Win32
Imports System

Public Class RegKey
    Public Const AlwaysOnTop = "AlwaysOnTop"
    Public Const Backup = "Backup"
    Public Const CloseAfterLaunch = "CloseAfterLaunch"
    Public Const Color = "Color"
    Public Const Dllmd5 = "DLLMD5"
    Public Const EnPatchAfterInstall = "ENPatchAfterInstall"
    Public Const EnPatchVersion = "ENPatchVersion"
    Public Const FontColor = "FontColor"
    Public Const LargeFilesAfterInstall = "LargeFilesAfterInstall"
    Public Const LargeFilesVersion = "LargeFilesVersion"
    Public Const LatestStoryBase = "LatestStoryBase"
    Public Const Locale = "Locale"
    Public Const NewEnVersion = "NewENVersion"
    Public Const NewLargeFilesVersion = "NewLargeFilesVersion"
    Public Const NewStoryVersion = "NewStoryVersion"
    Public Const NewVersionTemp = "NewVersionTemp"
    Public Const OldX = "OldX"
    Public Const OldY = "OldY"
    Public Const Pso2Dir = "PSO2Dir"
    Public Const Pso2PatchlistMd5 = "PSO2PatchlistMD5"
    Public Const Pso2PrecedeVersion = "PSO2PrecedeVersion"
    Public Const Pso2RemoteVersion = "PSO2RemoteVersion"
    Public Const Pastebin = "Pastebin"
    Public Const PatchServer = "PatchServer"
    Public Const PreDownloadedRar = "PreDownloadedRAR"
    Public Const ProxyEnabled = "ProxyEnabled"
    Public Const RemoveCensor = "RemoveCensor"
    Public Const SeenDownloadMessage = "SeenDownloadMessage"
    Public Const SeenFuckSegaMessage = "SeenFuckSEGAMessage"
    Public Const SidebarEnabled = "SidebarEnabled"
    Public Const StoryPatchAfterInstall = "StoryPatchAfterInstall"
    Public Const StoryPatchVersion = "StoryPatchVersion"
    Public Const Style = "Style"
    Public Const TextBoxBgColor = "TextBoxBGColor"
    Public Const TextBoxColor = "TextBoxColor"
    Public Const Uid = "UID"
    Public Const UseIcsHost = "UseItemICSHost"
    Public Const UseItemTranslation = "UseItemTranslation"
    Public Const SteamMode = "SteamMode"
    Public Const ImageLocation = "ImageLocation"
    Public Const ORBLocation = "ORBLocation"
    Public Const ChecksVisible = "ChecksVisible"
    Public Const PSO2DirVisible = "PSO2DirVisible"
    Public Const StatusLabelColor = "StatusLabelColor"
    Public Const PBTextColor = "PBTextColor"
    Public Const PBFill1 = "PBFill1"
    Public Const PBFill2 = "PBFill2"
    Public Const GetProxyStats = "GetProxyStats"
    Public Const ProxyStatsURL = "ProxyStatsURL"
    Public Const StatsLastChecked = "StatsLastChecked"
    Public Const CachedStats = "CachedStats"

    Private Shared ReadOnly RegistryCache As Dictionary(Of String, Object) = New Dictionary(Of String, Object)
    Private Shared ReadOnly RegistrySubKey As RegistryKey = Registry.CurrentUser.OpenSubKey("Software\AIDA", True)

    Public Shared Function GetValue(Of T)(key As String) As T
        Try
            Dim returnValue As Object = Nothing
            If RegistryCache.TryGetValue(key, returnValue) Then Return DirectCast(Convert.ChangeType(returnValue, GetType(T)), T)

            returnValue = RegistrySubKey.GetValue(key, Nothing)
            If returnValue IsNot Nothing Then RegistryCache.Add(key, returnValue)

            Return DirectCast(Convert.ChangeType(returnValue, GetType(T)), T)
        Catch
            Return Nothing
        End Try
    End Function

    Public Shared Sub SetValue(Of T)(key As String, value As T)
        My.Computer.Registry.SetValue("HKEY_CURRENT_USER\Software\AIDA", key, value)
        RegistryCache(key) = value
    End Sub
End Class
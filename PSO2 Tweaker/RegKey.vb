Imports Microsoft.Win32

Public Class RegKey
    Public Const AlwaysOnTop = "AlwaysOnTop"
    Public Const Backup = "Backup"
    Public Const CloseAfterLaunch = "CloseAfterLaunch"
    Public Const Color = "Color"
    Public Const DLLMD5 = "DLLMD5"
    Public Const ENPatchAfterInstall = "ENPatchAfterInstall"
    Public Const ENPatchVersion = "ENPatchVersion"
    Public Const FontColor = "FontColor"
    Public Const FontColors = "FontColor"
    Public Const LargeFilesAfterInstall = "LargeFilesAfterInstall"
    Public Const LargeFilesVersion = "LargeFilesVersion"
    Public Const LatestStoryBase = "LatestStoryBase"
    Public Const Locale = "Locale"
    Public Const NewENVersion = "NewENVersion"
    Public Const NewLargeFilesVersion = "NewLargeFilesVersion"
    Public Const NewStoryVersion = "NewStoryVersion"
    Public Const NewVersionTemp = "NewVersionTemp"
    Public Const OldX = "OldX"
    Public Const OldY = "OldY"
    Public Const PSO2Dir = "PSO2Dir"
    Public Const PSO2PatchlistMD5 = "PSO2PatchlistMD5"
    Public Const PSO2PrecedeVersion = "PSO2PrecedeVersion"
    Public Const PSO2RemoteVersion = "PSO2RemoteVersion"
    Public Const Pastebin = "Pastebin"
    Public Const PatchServer = "PatchServer"
    Public Const PreDownloadedRAR = "PreDownloadedRAR"
    Public Const ProxyEnabled = "ProxyEnabled"
    Public Const RemoveCensor = "RemoveCensor"
    Public Const SeenDownloadMessage = "SeenDownloadMessage"
    Public Const SeenFuckSEGAMessage = "SeenFuckSEGAMessage"
    Public Const SidebarEnabled = "SidebarEnabled"
    Public Const SteamUID = "SteamUID"
    Public Const StoryPatchAfterInstall = "StoryPatchAfterInstall"
    Public Const StoryPatchVersion = "StoryPatchVersion"
    Public Const Style = "Style"
    Public Const TextBoxBGColor = "TextBoxBGColor"
    Public Const TextBoxColor = "TextBoxColor"
    Public Const UID = "UID"
    Public Const UseItemTranslation = "UseItemTranslation"

    Private Shared RegistryCache As Dictionary(Of String, Object) = New Dictionary(Of String, Object)
    Private Shared RegistrySubKey As RegistryKey = Registry.CurrentUser.OpenSubKey("Software\AIDA", True)


    Public Shared Function GetValue(Of T)(Key As String) As T
        Try
            Dim returnValue As Object = Nothing
            If RegistryCache.TryGetValue(Key, returnValue) Then Return DirectCast(returnValue, T)

            returnValue = RegistrySubKey.GetValue(Key, Nothing)
            If returnValue IsNot Nothing Then RegistryCache.Add(Key, returnValue)

            Return DirectCast(returnValue, T)
        Catch
            Return Nothing
        End Try
    End Function

    Public Shared Sub SetValue(Of T)(ByRef Key As String, ByRef Value As T)
        My.Computer.Registry.SetValue("HKEY_CURRENT_USER\Software\AIDA", Key, Value)
        RegistryCache(Key) = Value
    End Sub
End Class
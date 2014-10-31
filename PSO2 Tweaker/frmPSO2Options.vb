Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Collections.Generic

Public Class frmPSO2Options
    Dim Documents As String = (System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\")
    Dim usersettingsfile As String = (Documents & "SEGA\PHANTASYSTARONLINE2\user.pso2")
    Shared INICache As New Dictionary(Of String, String)

    Private Declare Function EnumDisplaySettings Lib "user32" Alias "EnumDisplaySettingsA" (ByVal lpszDeviceName As String, ByVal iModeNum As Integer, ByRef lpDevMode As DEVMODE) As Boolean

    <StructLayout(LayoutKind.Sequential)> Public Structure DEVMODE
        <MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst:=32)> Public dmDeviceName As String
        Public dmSpecVersion As Short
        Public dmDriverVersion As Short
        Public dmSize As Short
        Public dmDriverExtra As Short
        Public dmFields As Integer
        Public dmOrientation As Short
        Public dmPaperSize As Short
        Public dmPaperLength As Short
        Public dmPaperWidth As Short
        Public dmScale As Short
        Public dmCopies As Short
        Public dmDefaultSource As Short
        Public dmPrintQuality As Short
        Public dmColor As Short
        Public dmDuplex As Short
        Public dmYResolution As Short
        Public dmTTOption As Short
        Public dmCollate As Short
        <MarshalAsAttribute(UnmanagedType.ByValTStr, SizeConst:=32)> Public dmFormName As String
        Public dmUnusedPadding As Short
        Public dmBitsPerPel As Short
        Public dmPelsWidth As Integer
        Public dmPelsHeight As Integer
        Public dmDisplayFlags As Integer
        Public dmDisplayFrequency As Integer
    End Structure

    Public Sub frmPSO2Settings_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            If File.Exists(usersettingsfile) = False Then
                MsgBox("Please launch PSO2 to generate a new user configuration file!")
                Me.Visible = False
                Me.Close()
                Exit Sub
            End If

            Select Case RegKey.GetValue(Of String)(RegKey.Style)
                Case "Blue"
                    StyleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Office2007Blue
                Case "Black"
                    StyleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Office2007Black
                Case "Silver"
                    StyleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Office2007Silver
                Case "Vista Glass"
                    StyleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Office2007VistaGlass
                Case "2010 Silver"
                    StyleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Office2010Silver
                Case "Windows 7 Blue"
                    StyleManager1.ManagerStyle = DevComponents.DotNetBar.eStyle.Windows7Blue
            End Select

            Dim backColor = Me.BackColor

            TabControlPanel1.Style.BackColor1.Color = backColor
            TabControlPanel1.Style.BackColor2.Color = backColor
            TabControlPanel2.Style.BackColor1.Color = backColor
            TabControlPanel2.Style.BackColor2.Color = backColor
            TabControlPanel3.Style.BackColor1.Color = backColor
            TabControlPanel3.Style.BackColor2.Color = backColor
            TabControlPanel4.Style.BackColor1.Color = backColor
            TabControlPanel4.Style.BackColor2.Color = backColor
            TabControlPanel1.StyleMouseDown.BackColor1.Color = backColor
            TabControlPanel1.StyleMouseDown.BackColor2.Color = backColor
            TabControlPanel2.StyleMouseDown.BackColor1.Color = backColor
            TabControlPanel2.StyleMouseDown.BackColor2.Color = backColor
            TabControlPanel3.StyleMouseDown.BackColor1.Color = backColor
            TabControlPanel3.StyleMouseDown.BackColor2.Color = backColor
            TabControlPanel4.StyleMouseDown.BackColor1.Color = backColor
            TabControlPanel4.StyleMouseDown.BackColor2.Color = backColor
            TabControlPanel1.StyleMouseOver.BackColor1.Color = backColor
            TabControlPanel1.StyleMouseOver.BackColor2.Color = backColor
            TabControlPanel2.StyleMouseOver.BackColor1.Color = backColor
            TabControlPanel2.StyleMouseOver.BackColor2.Color = backColor
            TabControlPanel3.StyleMouseOver.BackColor1.Color = backColor
            TabControlPanel3.StyleMouseOver.BackColor2.Color = backColor
            TabControlPanel4.StyleMouseOver.BackColor1.Color = backColor
            TabControlPanel4.StyleMouseOver.BackColor2.Color = backColor
            Dim DevM As DEVMODE
            DevM.dmDeviceName = New String(Chr(0), 32)
            DevM.dmFormName = New String(Chr(0), 32)
            DevM.dmSize = CShort(Marshal.SizeOf(GetType(DEVMODE)))

            Dim modeIndex As Integer = 0
            ' 0 = The first mode
            While EnumDisplaySettings(Nothing, modeIndex, DevM)
                ' Mode found
                If ComboBoxEx5.Items.Contains(DevM.dmPelsWidth & "x" & DevM.dmPelsHeight) = False Then ComboBoxEx5.Items.Add(DevM.dmPelsWidth & "x" & DevM.dmPelsHeight)

                ' The next mode
                modeIndex += 1
            End While
            Slider1.Value = Convert.ToInt32(ReadINISetting("DrawLevel"))
            ComboBoxEx1.SelectedIndex = Convert.ToInt32(ReadINISetting("TextureResolution"))
            ComboBoxEx7.SelectedIndex = Convert.ToInt32(ReadINISetting("InterfaceSize"))
            ComboBoxEx6.Text = ReadINISetting("FrameKeep") & " FPS"
            If ComboBoxEx6.Text = "0 FPS" Then ComboBoxEx6.Text = "Unlimited FPS"
            If ReadINISetting("ShaderQuality") = "true" Then ComboBoxEx2.SelectedIndex = 0
            If ReadINISetting("ShaderQuality") = "false" Then ComboBoxEx2.SelectedIndex = 1
            If ReadINISetting("MoviePlay") = "true" Then ComboBoxEx3.SelectedIndex = 0
            If ReadINISetting("MoviePlay") = "false" Then ComboBoxEx3.SelectedIndex = 1
            If ReadINISetting("FullScreen") = "false" Then
                ComboBoxEx4.SelectedIndex = 0
            End If
            If ReadINISetting("FullScreen") = "true" Then
                ComboBoxEx4.SelectedIndex = 1
            End If
            If ReadINISetting("VirtualFullScreen") = "true" Then
                ComboBoxEx4.SelectedIndex = 2
                'Disable resolution thingie
            End If
            ComboBoxEx5.Text = ReadINISetting("Width", 240) & "x" & ReadINISetting("Height", 240)
            If ComboBoxEx5.Items.Contains(ComboBoxEx5.Text) = False Then ComboBoxEx5.SelectedIndex = 0
            CheckBoxX1.Checked = False
            If ReadINISetting("Y") = "99999" Then
                If ReadINISetting("X") = "99999" Then
                    CheckBoxX1.Checked = True
                End If
            End If
        Catch ex As Exception
            frmMain.Log(ex.Message)
            frmMain.WriteDebugInfo(My.Resources.strERROR & ex.Message)
        End Try
    End Sub

    Public Function ReadINISetting(ByRef SettingToRead As String, Optional ByVal LineToStartAt As Integer = 0) As String
        Try
            Dim returnValue = ""
            If INICache.TryGetValue(SettingToRead, returnValue) Then Return returnValue

            Dim TextLines As String() = File.ReadAllLines(usersettingsfile)
            For i As Integer = LineToStartAt To (TextLines.Length - 1)
                If Not String.IsNullOrEmpty(TextLines(i)) Then
                    If TextLines(i).Contains(" " & SettingToRead & " ") Then
                        Dim strLine As String = TextLines(i).Replace(vbTab, "")
                        Dim strReturn As String() = strLine.Split("="c)
                        Dim FinalString As String = strReturn(1).Replace("""", "").Replace(","c, "").Replace(" "c, "")
                        If FinalString IsNot Nothing Then INICache.Add(SettingToRead, FinalString)
                        Return FinalString
                    End If
                End If
            Next
        Catch ex As Exception
            frmMain.Log(ex.Message)
            frmMain.WriteDebugInfo(My.Resources.strERROR & ex.Message)
        End Try
        Return ""
    End Function

    Public Sub SaveINISetting(ByRef SettingToSave As String, ByRef Value As String)
        Try
            INICache(SettingToSave) = Value

            TextBoxX1.Text = ""
            Dim SettingString As String = File.ReadAllText(usersettingsfile)
            Dim TextLines As String() = SettingString.Split(Environment.NewLine.ToCharArray, System.StringSplitOptions.RemoveEmptyEntries)
            Dim i As Integer
            Dim j As Integer
            For i = 0 To (TextLines.Length - 1)
                If TextLines(i).Contains(" " & SettingToSave & " ") Then
                    Dim strLine As String = TextLines(i).Replace(vbTab, "")
                    Dim strReturn As String() = strLine.Split("="c)
                    Dim FinalString As String = strReturn(1).Replace("""", "").Replace(","c, "")
                    TextLines(i) = TextLines(i).Replace(FinalString, (" " & Value))
                    For j = 0 To TextLines.Length
                        If j + 1 = TextLines.Length Then
                            TextBoxX1.AppendText("}")
                            File.Delete(usersettingsfile)
                            File.WriteAllText(usersettingsfile, TextBoxX1.Text)
                            Exit Sub
                        End If
                        TextBoxX1.AppendText(TextLines(j) & vbCrLf)
                    Next j
                End If
            Next i
        Catch ex As Exception
            frmMain.Log(ex.Message)
            frmMain.WriteDebugInfo(My.Resources.strERROR & ex.Message)
        End Try
    End Sub

    Public Sub SaveResolutionHeight(ByRef Value As String)
        Try
            TextBoxX1.Text = ""
            Dim SettingString As String = File.ReadAllText(usersettingsfile)
            Dim TextLines As String() = SettingString.Split(Environment.NewLine.ToCharArray, System.StringSplitOptions.RemoveEmptyEntries)
            Dim i As Integer
            Dim j As Integer
            For i = 0 To (TextLines.Length - 1)
                If TextLines(i).Contains("Windows = {") Then
                    For x = 1 To 9
                        If TextLines(i + x).Contains("Height =") Then
                            i = i + x
                            Exit For
                        End If
                    Next x

                    Dim strLine As String = TextLines(i).Replace(vbTab, "")
                    Dim strReturn As String() = strLine.Split("="c)
                    Dim FinalString As String = strReturn(1).Replace("""", "").Replace(","c, "")
                    TextLines(i) = TextLines(i).Replace(FinalString, (" " & Value))
                    For j = 0 To TextLines.Length
                        If j + 1 = TextLines.Length Then
                            TextBoxX1.AppendText("}")
                            File.Delete(usersettingsfile)
                            File.WriteAllText(usersettingsfile, TextBoxX1.Text)
                            Exit Sub
                        End If
                        TextBoxX1.AppendText(TextLines(j) & vbCrLf)
                    Next j
                End If
            Next i
        Catch ex As Exception
            frmMain.Log(ex.Message)
            frmMain.WriteDebugInfo(My.Resources.strERROR & ex.Message)
        End Try
    End Sub

    Public Sub SaveResolutionWidth(ByRef Value As String)
        Try
            TextBoxX1.Text = ""
            Dim SettingString As String = File.ReadAllText(usersettingsfile)
            Dim TextLines As String() = SettingString.Split(Environment.NewLine.ToCharArray, System.StringSplitOptions.RemoveEmptyEntries)
            Dim i As Integer
            Dim j As Integer
            For i = 0 To (TextLines.Length - 1)
                If TextLines(i).Contains("Windows = {") Then
                    For x = 1 To 9
                        If TextLines(i + x).Contains("Width =") Then
                            i = i + x
                            Exit For
                        End If
                    Next x

                    Dim strLine As String = TextLines(i).Replace(vbTab, "")
                    Dim strReturn As String() = strLine.Split("="c)
                    Dim FinalString As String = strReturn(1).Replace("""", "").Replace(","c, "")
                    TextLines(i) = TextLines(i).Replace(FinalString, (" " & Value))

                    For j = 0 To TextLines.Length
                        If j + 1 = TextLines.Length Then
                            TextBoxX1.AppendText("}")
                            File.Delete(usersettingsfile)
                            File.WriteAllText(usersettingsfile, TextBoxX1.Text)
                            Exit Sub
                        End If
                        TextBoxX1.AppendText(TextLines(j) & vbCrLf)
                    Next j

                End If
            Next i
        Catch ex As Exception
            frmMain.Log(ex.Message)
            frmMain.WriteDebugInfo(My.Resources.strERROR & ex.Message)
        End Try
    End Sub

    Private Sub btnSaveSettings_Click(sender As Object, e As EventArgs) Handles btnSaveSettings.Click
        Try
            frmMain.Log("Saving Draw Level...")
            SaveINISetting("DrawLevel", Slider1.Value.ToString())
            frmMain.Log("Saving Texture Resolution...")
            SaveINISetting("TextureResolution", ComboBoxEx1.SelectedIndex.ToString())
            frmMain.Log("Saving Interface Size...")
            SaveINISetting("InterfaceSize", ComboBoxEx7.SelectedIndex.ToString())
            frmMain.Log("Saving Shader Quality...")
            If ComboBoxEx2.SelectedIndex = 0 Then SaveINISetting("ShaderQuality", "true")
            If ComboBoxEx2.SelectedIndex = 1 Then SaveINISetting("ShaderQuality", "false")
            frmMain.Log("Saving Movie Play...")
            If ComboBoxEx3.SelectedIndex = 0 Then SaveINISetting("MoviePlay", "true")
            If ComboBoxEx3.SelectedIndex = 1 Then SaveINISetting("MoviePlay", "false")

            frmMain.Log("Saving Window Mode (Windowed)...")
            If ComboBoxEx4.SelectedIndex = 0 Then
                SaveINISetting("FullScreen", "false")
                SaveINISetting("VirtualFullScreen", "false")
            End If

            frmMain.Log("Saving Window Mode (Fullscreen)...")
            If ComboBoxEx4.SelectedIndex = 1 Then
                SaveINISetting("FullScreen", "true")
                SaveINISetting("VirtualFullScreen", "false")
            End If

            frmMain.Log("Saving Window Mode (Virtual Fullscreen)...")
            If ComboBoxEx4.SelectedIndex = 2 Then
                SaveINISetting("FullScreen", "false")
                SaveINISetting("VirtualFullScreen", "true")
            End If

            If ComboBoxEx5.Items.Contains(ComboBoxEx5.Text) = False Then
                MsgBox("Please select a supported resolution!")
                Exit Sub
            End If

            frmMain.Log("Saving Resolution...")
            If ComboBoxEx5.SelectedText <> "x" Then
                Dim StrResolution As String = ComboBoxEx5.SelectedItem.ToString()

                Dim RealResolution As String() = StrResolution.Split("x"c)
                SaveResolutionWidth(RealResolution(0))
                SaveResolutionHeight(RealResolution(1))
            End If

            Dim FPS As String = ComboBoxEx6.SelectedItem.ToString().Replace(" FPS", "").Replace("Unlimited", "0")

            frmMain.Log("Saving FPS...")
            SaveINISetting("FrameKeep", FPS)
            frmMain.Log("Disabling Interface...")

            If CheckBoxX1.Checked Then
                If ReadINISetting("X") <> "99999" Then
                    If ReadINISetting("Y") <> "99999" Then
                        RegKey.SetValue(Of String)(RegKey.OldX, ReadINISetting("X"))
                        RegKey.SetValue(Of String)(RegKey.OldY, ReadINISetting("Y"))
                        SaveINISetting("X", "99999")
                        SaveINISetting("Y", "99999")
                    End If
                End If
            End If

            frmMain.Log("Enabling Interface...")
            If CheckBoxX1.Checked = False Then
                If ReadINISetting("X") = "99999" Then
                    If ReadINISetting("Y") = "99999" Then
                        SaveINISetting("X", RegKey.GetValue(Of String)(RegKey.OldX))
                        SaveINISetting("Y", RegKey.GetValue(Of String)(RegKey.OldY))
                    End If
                End If
            End If

            MsgBox("Settings saved!")
        Catch ex As Exception
            frmMain.Log(ex.Message)
            frmMain.WriteDebugInfo(My.Resources.strERROR & ex.Message)
        End Try
    End Sub

    Private Sub ComboBoxEx4_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBoxEx4.SelectedIndexChanged
        Try
            'If ComboBoxEx4.SelectedIndex = 2 Then ComboBoxEx5.Enabled = False
            'If ComboBoxEx4.SelectedIndex = 1 Then ComboBoxEx5.Enabled = True
            'If ComboBoxEx4.SelectedIndex = 0 Then ComboBoxEx5.Enabled = True

            ComboBoxEx5.Enabled = (ComboBoxEx4.SelectedIndex < 2)

        Catch ex As Exception
            frmMain.Log(ex.Message)
            frmMain.WriteDebugInfo(My.Resources.strERROR & ex.Message)
        End Try
    End Sub
End Class

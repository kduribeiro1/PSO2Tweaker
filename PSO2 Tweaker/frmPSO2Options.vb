Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Collections.Generic
Imports DevComponents.DotNetBar

Public Class frmPSO2Options
    Dim Documents As String = (System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) & "\")
    Dim usersettingsfile As String = (Documents & "SEGA\PHANTASYSTARONLINE2\user.pso2")
    'Shared INICache As New Dictionary(Of String, String)

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
            If Not File.Exists(usersettingsfile) Then
                File.WriteAllText(usersettingsfile, frmMain.txtPSO2DefaultINI.Text)
                frmMain.WriteDebugInfo("Generating new PSO2 Settings file... Done!")
            End If

            Me.SuspendLayout()

            Dim backColor = Me.BackColor
            TabControlPanel1.Style.BackColor1.Color = backColor
            TabControlPanel1.Style.BackColor2.Color = backColor
            TabControlPanel1.StyleMouseOver.BackColor1.Color = backColor
            TabControlPanel1.StyleMouseOver.BackColor2.Color = backColor
            TabControlPanel1.StyleMouseDown.BackColor1.Color = backColor
            TabControlPanel1.StyleMouseDown.BackColor2.Color = backColor

            TabControlPanel2.Style.BackColor1.Color = backColor
            TabControlPanel2.Style.BackColor2.Color = backColor
            TabControlPanel2.StyleMouseOver.BackColor1.Color = backColor
            TabControlPanel2.StyleMouseOver.BackColor2.Color = backColor
            TabControlPanel2.StyleMouseDown.BackColor1.Color = backColor
            TabControlPanel2.StyleMouseDown.BackColor2.Color = backColor

            TabControlPanel3.Style.BackColor1.Color = backColor
            TabControlPanel3.Style.BackColor2.Color = backColor
            TabControlPanel3.StyleMouseOver.BackColor1.Color = backColor
            TabControlPanel3.StyleMouseOver.BackColor2.Color = backColor
            TabControlPanel3.StyleMouseDown.BackColor1.Color = backColor
            TabControlPanel3.StyleMouseDown.BackColor2.Color = backColor

            TabControlPanel4.Style.BackColor1.Color = backColor
            TabControlPanel4.Style.BackColor2.Color = backColor
            TabControlPanel4.StyleMouseOver.BackColor1.Color = backColor
            TabControlPanel4.StyleMouseOver.BackColor2.Color = backColor
            TabControlPanel4.StyleMouseDown.BackColor1.Color = backColor
            TabControlPanel4.StyleMouseDown.BackColor2.Color = backColor

            TabControlPanel5.Style.BackColor1.Color = backColor
            TabControlPanel5.Style.BackColor2.Color = backColor
            TabControlPanel5.StyleMouseOver.BackColor1.Color = backColor
            TabControlPanel5.StyleMouseOver.BackColor2.Color = backColor
            TabControlPanel5.StyleMouseDown.BackColor1.Color = backColor
            TabControlPanel5.StyleMouseDown.BackColor2.Color = backColor

            Dim DevM As DEVMODE
            DevM.dmDeviceName = New String(Chr(0), 32)
            DevM.dmFormName = New String(Chr(0), 32)
            DevM.dmSize = CShort(Marshal.SizeOf(GetType(DEVMODE)))

            Dim modeIndex As Integer = 0
            ' 0 = The first mode
            While EnumDisplaySettings(Nothing, modeIndex, DevM)
                ' Mode found
                If Not ComboBoxEx5.Items.Contains(DevM.dmPelsWidth & "x" & DevM.dmPelsHeight) Then ComboBoxEx5.Items.Add(DevM.dmPelsWidth & "x" & DevM.dmPelsHeight)

                ' The next mode
                modeIndex += 1
            End While

            Dim CurrentHeight As String = ReadINISetting("Height3d")
            Dim CurrentWidth As String = ReadINISetting("Width3d")

            Dim FullRes As String = CurrentWidth & "x" & CurrentHeight

            ComboBoxEx5.Text = FullRes
            Slider1.Value = Convert.ToInt32(ReadINISetting("DrawLevel"))
            SBGM.Value = Convert.ToInt32(ReadINISetting("Bgm"))
            SSE.Value = Convert.ToInt32(ReadINISetting("Se"))
            SVOICE.Value = Convert.ToInt32(ReadINISetting("Voice"))
            SIGM.Value = Convert.ToInt32(ReadINISetting("Movie"))
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
            'ComboBoxEx5.Text = ReadINISetting("Width", 240) & "x" & ReadINISetting("Height", 240)
            If Not ComboBoxEx5.Items.Contains(ComboBoxEx5.Text) Then ComboBoxEx5.SelectedIndex = 0
            CheckBoxX1.Checked = False
            If ReadINISetting("Y") = "99999" Then
                If ReadINISetting("X") = "99999" Then
                    CheckBoxX1.Checked = True
                End If
            End If
        Catch ex As Exception
            frmMain.Log(ex.Message)
            frmMain.WriteDebugInfo(My.Resources.strERROR & ex.Message)
        Finally
            Me.ResumeLayout(False)
        End Try
    End Sub

    Public Function ReadINISetting(ByRef SettingToRead As String, Optional ByVal LineToStartAt As Integer = 0) As String
        Try
            'Dim returnValue = ""
            'If INICache.TryGetValue(SettingToRead, returnValue) Then Return returnValue

            Dim TextLines As String() = File.ReadAllLines(usersettingsfile)
            For i As Integer = LineToStartAt To (TextLines.Length - 1)
                If Not String.IsNullOrEmpty(TextLines(i)) Then
                    If TextLines(i).Contains(" " & SettingToRead & " ") Then
                        Dim strLine As String = TextLines(i).Replace(vbTab, "")
                        Dim strReturn As String() = strLine.Split("="c)
                        Dim FinalString As String = strReturn(1).Replace("""", "").Replace(","c, "").Replace(" "c, "")
                        'If FinalString IsNot Nothing Then INICache.Add(SettingToRead, FinalString)
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
            'INICache(SettingToSave) = Value

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
            Dim Contains As Boolean = False
            For i = 0 To (TextLines.Length - 1)
                If TextLines(i).Contains("Windows = {") Then
                    For x As Integer = 1 To 9
                        If TextLines(i + x).Contains("Height =") Then
                            i += x
                            Contains = True
                            Exit For
                        End If
                    Next x

                    If Contains = False Then
                        frmMain.WriteDebugInfo("Couldn't find Height in user settings. This is OKAY. If you notice your resolution not changing, try resetting your PSO2 Settings to default. If everything works, feel free to ignore this error.")
                        Exit Sub
                    End If

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
            Dim Contains As Boolean = False
            For i = 0 To (TextLines.Length - 1)
                If TextLines(i).Contains("Windows = {") Then
                    For x As Integer = 1 To 9
                        If TextLines(i + x).Contains("Width =") Then
                            i += x
                            Contains = True
                            Exit For
                        End If
                    Next x

                    If Contains = False Then
                        frmMain.WriteDebugInfo("Couldn't find Width in user settings. This is OKAY. If you notice your resolution not changing, try resetting your PSO2 Settings to default. If everything works, feel free to ignore this error.")
                        Exit Sub
                    End If

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
    Public Sub SaveResolutionHeight3D(ByRef Value As String)
        Try
            TextBoxX1.Text = ""
            Dim SettingString As String = File.ReadAllText(usersettingsfile)
            Dim TextLines As String() = SettingString.Split(Environment.NewLine.ToCharArray, System.StringSplitOptions.RemoveEmptyEntries)
            Dim i As Integer
            Dim j As Integer
            Dim Contains As Boolean = False
            For i = 0 To (TextLines.Length - 1)
                If TextLines(i).Contains("Windows = {") Then
                    For x As Integer = 1 To 9
                        If TextLines(i + x).Contains("Height3d =") Then
                            i += x
                            Contains = True
                            Exit For
                        End If
                    Next x

                    If Contains = False Then
                        frmMain.WriteDebugInfo("Couldn't find Height3D in user settings. This is OKAY. If you notice your resolution not changing, try resetting your PSO2 Settings to default. If everything works, feel free to ignore this error.")
                        Exit Sub
                    End If

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


    Public Sub SaveResolutionWidth3D(ByRef Value As String)
        Try
            TextBoxX1.Text = ""
            Dim SettingString As String = File.ReadAllText(usersettingsfile)
            Dim TextLines As String() = SettingString.Split(Environment.NewLine.ToCharArray, System.StringSplitOptions.RemoveEmptyEntries)
            Dim i As Integer
            Dim j As Integer
            Dim Contains As Boolean = False
            For i = 0 To (TextLines.Length - 1)
                If TextLines(i).Contains("Windows = {") Then
                    For x As Integer = 1 To 9
                        If TextLines(i + x).Contains("Width3d =") Then
                            i += x
                            Contains = True
                            Exit For
                        End If
                    Next x

                    If Contains = False Then
                        frmMain.WriteDebugInfo("Couldn't find Width3D in user settings. This is OKAY. If you notice your resolution not changing, try resetting your PSO2 Settings to default. If everything works, feel free to ignore this error.")
                        Exit Sub
                    End If

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
        'Try
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

        If ComboBoxEx4.SelectedIndex = 0 Then
            frmMain.Log("Saving Window Mode (Windowed)...")
            SaveINISetting("FullScreen", "false")
            SaveINISetting("VirtualFullScreen", "false")
        End If

        If ComboBoxEx4.SelectedIndex = 1 Then
            frmMain.Log("Saving Window Mode (Fullscreen)...")
            SaveINISetting("FullScreen", "true")
            SaveINISetting("VirtualFullScreen", "false")
        End If

        If ComboBoxEx4.SelectedIndex = 2 Then
            frmMain.Log("Saving Window Mode (Virtual Fullscreen)...")
            SaveINISetting("FullScreen", "false")
            SaveINISetting("VirtualFullScreen", "true")
        End If

        If Not ComboBoxEx5.Items.Contains(ComboBoxEx5.Text) Then
            MsgBox("Please select a supported resolution!")
            Exit Sub
        End If

        frmMain.Log("Saving Resolution...")
        'If ComboBoxEx5.SelectedText <> "x" Then
        Dim StrResolution As String = ComboBoxEx5.SelectedItem.ToString()

        Dim RealResolution As String() = StrResolution.Split("x"c)
        SaveResolutionWidth(RealResolution(0))
        SaveResolutionHeight(RealResolution(1))
        SaveResolutionWidth3D(RealResolution(0))
        SaveResolutionHeight3D(RealResolution(1))
        'End If

        Dim FPS As String = ComboBoxEx6.SelectedItem.ToString().Replace(" FPS", "").Replace("Unlimited", "0")

        frmMain.Log("Saving FPS...")
        SaveINISetting("FrameKeep", FPS)

        frmMain.Log("Saving Volume...")
        SaveINISetting("Bgm", SBGM.Value.ToString())
        SaveINISetting("Voice", SVOICE.Value.ToString())
        SaveINISetting("Movie", SIGM.Value.ToString())
        SaveINISetting("Se", SSE.Value.ToString())

        If CheckBoxX1.Checked Then
            frmMain.Log("Disabling Interface...")
            If ReadINISetting("X") <> "99999" Then
                If ReadINISetting("Y") <> "99999" Then
                    RegKey.SetValue(Of String)(RegKey.OldX, ReadINISetting("X"))
                    RegKey.SetValue(Of String)(RegKey.OldY, ReadINISetting("Y"))
                    SaveINISetting("X", "99999")
                    SaveINISetting("Y", "99999")
                End If
            End If
        End If

        If Not CheckBoxX1.Checked Then
            frmMain.Log("Enabling Interface...")
            If ReadINISetting("X") = "99999" Then
                If ReadINISetting("Y") = "99999" Then
                    SaveINISetting("X", RegKey.GetValue(Of String)(RegKey.OldX))
                    SaveINISetting("Y", RegKey.GetValue(Of String)(RegKey.OldY))
                End If
            End If
        End If

        MsgBox("Settings saved!")
        'Catch ex As Exception
        'frmMain.Log(ex.Message)
        'frmMain.WriteDebugInfo(My.Resources.strERROR & ex.Message)
        'End Try
    End Sub

    Private Sub SBGM_ValueChanged(sender As Object, e As EventArgs) Handles SBGM.ValueChanged
        SBGM.Text = "Background Music Volume - " & SBGM.Value
    End Sub

    Private Sub SSE_ValueChanged(sender As Object, e As EventArgs) Handles SSE.ValueChanged
        SSE.Text = "Sound Effect Volume - " & SSE.Value
    End Sub

    Private Sub SVOICE_ValueChanged(sender As Object, e As EventArgs) Handles SVOICE.ValueChanged
        SVOICE.Text = "Character Voice Volume - " & SVOICE.Value
    End Sub

    Private Sub SIGM_ValueChanged(sender As Object, e As EventArgs) Handles SIGM.ValueChanged
        SIGM.Text = "In-Game Movie Volume - " & SIGM.Value
    End Sub
End Class

Imports System.Windows.Forms
Imports System.IO
Public Class GUI
    Dim fulltext As String = ""
    Public filepath As String = "NULL"
    Private Sub Btnopen_Click(ByVal sender As System.Object,
   ByVal e As System.EventArgs) Handles BtnOpen.Click
        If OpenFileDialog1.ShowDialog() = System.Windows.Forms.DialogResult.OK Then
            filepath = OpenFileDialog1.FileName
            fulltext = ""
            Using sr As System.IO.StreamReader = New StreamReader(OpenFileDialog1.FileName)
                Do Until sr.EndOfStream
                    Dim line As String = sr.ReadLine
                    fulltext = fulltext & line & Chr(10)
                Loop
            End Using
            textbox.Text = fulltext
        End If
    End Sub

    Private Sub BtnSave_Click(sender As Object, e As EventArgs) Handles BtnSave.Click
        savefile()
    End Sub
    Sub savefile()
        Dim nooflines As Integer = 0
        Using file As StreamWriter = New StreamWriter(filepath) 'change file location
            For Each line In textbox.Lines
                If line = "" Then
                Else
                    file.WriteLine(line)
                    nooflines = nooflines + 1
                End If
            Next
        End Using
        Dim fullfile(nooflines) As String
        Using file As StreamReader = New StreamReader(filepath)
            Dim count As Integer = 0
            Do Until file.EndOfStream
                Dim individualline = file.ReadLine()
                If individualline <> "" Then
                    fullfile(count) = individualline
                End If
                count = count + 1
            Loop
        End Using
        Using file As StreamWriter = New StreamWriter(filepath) 'change file location
            For i = 0 To nooflines - 1
                file.WriteLine(fullfile(i))
            Next
        End Using
    End Sub

    Private Sub BtnRun_Click(sender As Object, e As EventArgs) Handles BtnRun.Click
        If filepath <> "NULL" Then
            savefile()
            guiexecute(filepath)
            'vv this code should work, is doesn't vv
            'AllocConsole()
            'Console.Title = "HAL/S Interpreter"
            'setworkingfile(filepath)
            'Parsefile(False)
            'Console.ReadKey()
            'FreeConsole()
        Else
            MsgBox("Please save or open a program before running it", MsgBoxStyle.DefaultButton1, "I'm sorry Dave, I can't let you do that")
        End If
    End Sub

    Private Sub BtnSaveAs_Click(sender As Object, e As EventArgs) Handles BtnSaveAs.Click
        Dim saveFileDialog1 As New SaveFileDialog()
        saveFileDialog1.ShowDialog()
        If saveFileDialog1.FileName <> "" Then
            filepath = saveFileDialog1.FileName
        End If
        Dim fs As FileStream = File.Create(filepath)
        fs.Close()
        savefile()
    End Sub
    Public Sub startopenfile()

        Using file As StreamReader = New StreamReader("temp.tmp")
            filepath = file.ReadLine()
        End Using
        Dim fulltext As String = ""
        Using sr As System.IO.StreamReader = New StreamReader(filepath)
            Do Until sr.EndOfStream
                Dim line As String = sr.ReadLine
                fulltext = fulltext & line & Chr(10)
            Loop
        End Using
        textbox.Text = fulltext
        setworkingfile(filepath)
        Parsefile(False)
    End Sub
    Private Sub BtnManual_Click(sender As Object, e As EventArgs) Handles BtnManual.Click
        Process.Start("http://www.brouhaha.com/~eric/nasa/hal-s/programming_in_hal-s.pdf")
    End Sub
    Private Sub BtnHelp_Click(sender As Object, e As EventArgs) Handles BtnHelp.Click
        Dim message As String =
"HAL/S Interpreter V0.2α
By Thomas Curry

Every other run, the program will restart to resolve an as yet unsolved error.
The file will only save properly if you have opened it, or used 'save as' first.

Please read the help file (hal-s interpreter.docx), and the manual (button to the left of the help button which is conveniently marked 'manual')."
        MsgBox(message, MsgBoxStyle.Information, "Help")
    End Sub
    Private Sub BtnNew_Click(sender As Object, e As EventArgs) Handles BtnNew.Click
        textbox.Text = ""
    End Sub
    Private Sub uppercase(sender As Object, e As System.Windows.Forms.KeyPressEventArgs) Handles textbox.KeyPress
        If Char.IsLower(e.KeyChar) Then
            e.KeyChar =
                UCase(e.KeyChar)
        End If
    End Sub

    Private Sub cbStrictSyntax_CheckedChanged(sender As Object, e As EventArgs) Handles cbStrictSyntax.CheckedChanged
        If cbStrictSyntax.Checked = True Then
            StrictSyntax = True
        ElseIf cbStrictSyntax.Checked = False Then
            StrictSyntax = False
        End If
    End Sub
End Class
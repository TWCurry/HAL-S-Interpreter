Imports System.IO
Imports System.Math
Imports System.Windows.Forms
Module Module1
    Dim ProgramDefinedVariables As New Dictionary(Of String, String)
    Dim ProgramDefinedvariablesTypes As New Dictionary(Of String, String)
    Dim variablenametocreate, variablevalue, operationchoice, variabletoread, variabletype, workingfile, Line, Keyword, lastopenedfile, mainprogramname, activefunction As String
    Dim LineNo As Integer = 1
    Dim ErrorLineNo As Integer = 0
    Dim multipleerrors As Boolean = False
    Dim condition As Boolean
    Dim linenotoloopto As Integer = 0
    Dim loopvariablename As String
    Dim forloopset As Boolean = False
    Dim exponents(255) As Integer
    Dim exponentno As Integer
    Dim exponentline As Boolean = False
    Dim functiontriggered As Boolean = False
    Dim linetomovetoafterfunctionexecution, firstparameterposshift As Integer
    Dim functionoperandvalues(50) As String
    Public StrictSyntax As Boolean = False
    Dim gui As New HAL_Interpreter.GUI
    Dim procedurelocations(300) 'length of string is 3 times amount of procedures allowed in program (realtime)
    Declare Function AllocConsole Lib "kernel32" () As Int32
    Declare Function FreeConsole Lib "kernel32" () As Int32
    Dim builtinfunctiongenerated As Boolean = False
    Dim defaultfunctions(10) As String
    Sub Main()
        LineNo = 1
        'DisplayGUI() 'new gui
        oldmain() '"classic" mode
        'realtime()
    End Sub
    Sub oldmain()
        Console.Title = "HAL/S Interpreter"
        Logo()
        workingfile = "TRIG2.HAL"
        ' realtime()
        Readfromfile()
        Parsefile(True)
        Parsefile(False)
        Console.ReadKey()
        '##prompt for future##
        'Console.WriteLine("Default file is test.txt")
        'Dim quit As Boolean = False
        'Dim prompt As String
        'Do Until quit = True
        '    Console.WriteLine()
        '    Console.WriteLine("Enter what type of operation you want to perfom. Options are: (1) View Annotations, (2) Compile and Run, (3) View Code, (f) change file, (c) Credits and (q) Quit")
        '    prompt = Console.ReadLine
        '    If prompt = "1" Then
        '        parsefile(True)
        '    ElseIf prompt = "2" Then
        '        parsefile(False)
        '    ElseIf prompt = "3" Then
        '        readfromfile()
        '    ElseIf prompt = "c" Then
        '        tribute()
        '    ElseIf prompt = "f" Then
        '        workingfile = Console.ReadLine()
        '        If My.Computer.FileSystem.FileExists("C:\Users\Thomas\Documents\Visual Studio 2017\Projects\HAL Interpreter\HAL Interpreter\bin\Debug\" & workingfile) Then
        '        Else
        '            Console.WriteLine("File does not exist")
        '            workingfile = "test.txt"
        '        End If
        '    ElseIf prompt = "q" Then
        '            quit = True
        '    End If
        'Loop
    End Sub
    Sub realtime()
        Dim currentarraypos As Integer = 3
        Using file As New StreamReader(workingfile)
            Do Until file.EndOfStream
                Line = file.ReadLine
                Dim components() As String = Line.Split(New Char() {" "c})
                If Line.Contains("SIMPLE:") Then
                    procedurelocations(0) = Removesemicolon(components(3)) 'check if this is the right position (should be name of program)
                    procedurelocations(1) = Convert.ToInt64(LineNo)
                End If
                If Line.Contains("CLOSE SIMPLE") Then procedurelocations(2) = Convert.ToInt64(LineNo)
                LineNo = LineNo + 1
            Loop
        End Using
        LineNo = 1
        Using file As New StreamReader(workingfile)
            Do Until file.EndOfStream
                Line = file.ReadLine
                Dim components() As String = Line.Split(New Char() {" "c})
                Try
                    If components(3).Contains("FUNCTION(") Then
                        procedurelocations(currentarraypos) = components(2)
                        Dim currentfunction = components(2)
                        currentarraypos = currentarraypos + 1
                        procedurelocations(currentarraypos) = Convert.ToInt64(LineNo)
                        currentarraypos = currentarraypos + 1
                    End If
                Catch
                End Try
                Try
                    If components(2) = "CLOSE" Then
                        procedurelocations(currentarraypos) = Convert.ToInt64(LineNo)
                        currentarraypos = currentarraypos + 1
                    End If
                Catch
                End Try
                LineNo = LineNo + 1
            Loop
        End Using
        Dim Thread1 As New System.Threading.Thread(AddressOf mainthreadsub)
        Thread1.Start()
        LineNo = 1
    End Sub
    Sub mainthreadsub()
        LineNo = procedurelocations(1)
        Parsefile(False)
        Console.ReadKey()
    End Sub
    Sub DisplayGUI()
        Console.Title = "Hal Interpreter"
        FreeConsole() 'hide console
        'gui.Size = New System.Drawing.Size(My.Computer.Screen.Bounds.Width, My.Computer.Screen.Bounds.Height)
        GUI.Size = SystemInformation.PrimaryMonitorSize
        gui.Location = New Drawing.Point(0, 0)
        gui.WindowState = FormWindowState.Maximized
        Try
            If File.Exists("D:\owin31.wav") Then
                'My.Computer.Audio.Play("D:\owin31.wav")
            End If
        Catch
        End Try
        If File.Exists("temp.tmp") Then
            Try
                gui.startopenfile()
            Catch
                File.Delete("temp.tmp")
            End Try
            If System.IO.File.Exists("temp.tmp") = True Then
                System.IO.File.Delete("temp.tmp")
            End If
        End If
        gui.ShowDialog()
    End Sub
    Sub guiexecute(filepath)
        workingfile = filepath
        AllocConsole()
        Console.Title = "HAL/S Interpreter"
        Try
            Console.WriteLine("") 'if it crashes on this line its probably because the console is not properly opened
            Parsefile(False)
            Console.ReadKey()
            FreeConsole()
        Catch ex As Exception
            If TypeOf ex Is NullReferenceException Then
                MsgBox("Please remove all blank lines and properly finish programs and procedures (CLOSE not END). (Try adding 'CLOSE SIMPLE;') Line: " & LineNo, MsgBoxStyle.Critical, "NullReferenceError")
            Else
                If TypeOf ex Is FormatException Then
                    MsgBox("Incorrect format (Are your EMSCD's right?) Line: " & LineNo, MsgBoxStyle.Critical, "FormatException")
                ElseIf TypeOf ex Is IO.IOException Then
                    ' MsgBox("System.Console Output Error:" & Chr(10) & ex.ToString() & Chr(10) & "Please save your work and restart the program.", MsgBoxStyle.Critical, "Error")
                    'savelastopenedfile
                    Dim fs As FileStream = File.Create("temp.tmp")
                    fs.Close()
                    Using file As StreamWriter = New StreamWriter("temp.tmp")
                        file.Write(filepath)
                    End Using
                    Application.Restart()
                    'gui.Close()
                    'guiexecute(filepath)
                Else
                End If
            End If
            FreeConsole()
        End Try
    End Sub
    Sub setworkingfile(filenametosetto)
        workingfile = filenametosetto
    End Sub
    Sub Readfromfile()
        Dim displaylineno As Integer = 1
        'Console.WriteLine("Enter the filename to open")
        'workingfile = Console.ReadLine()
        Console.WriteLine("Contents of file '" & workingfile & "':")
        Console.WriteLine()
        Using file As New StreamReader(workingfile)
            'output contents of workingfile (to check the file reads properly)
            Do Until file.EndOfStream = True
                Line = file.ReadLine()
                'output line no
                Console.ForegroundColor = ConsoleColor.DarkMagenta
                Console.Write(displaylineno & "  ")
                Console.ForegroundColor = ConsoleColor.Gray
                displaylineno = displaylineno + 1
                Dim components() As String = Line.Split(New Char() {" "c})
                For i As Integer = 0 To (components.Length() - 1)
                    Console.Write(components(i) & " ")
                Next
                Console.WriteLine()
            Loop
        End Using
    End Sub
    Sub Parsefile(DisplayInterpreterComments)
        Dim linetype As String
        LineNo = 1 'not for real time
        Dim endofprogram As Boolean = False
        Console.WriteLine()
        Console.WriteLine("Interpreter output:")
        Console.WriteLine()
        'check for semicolons - mean syntax mode
        If StrictSyntax = True Then
            Dim semicolonline As Integer = 0
            Using f As New StreamReader(workingfile)
                Do Until f.EndOfStream = True
                    semicolonline = semicolonline + 1
                    Dim newline As String = f.ReadLine()
                    If Not (newline.Contains(";")) Then
                        If Not newline.Contains("C ") Then
                            MsgBox($"Semicolon missing on line {semicolonline}.", MsgBoxStyle.Critical, "Syntax Error.")
                            Exit Sub
                            Exit Sub
                        End If
                    End If
                Loop
            End Using
        End If
        'move to correct line
        endofprogram = False
        Do Until endofprogram = True
            Using file As New StreamReader(workingfile)
                For i = 1 To LineNo
                    Using fileline As New StreamReader(workingfile)
                        Line = file.ReadLine()
                    End Using
                Next
                'output line number
                If DisplayInterpreterComments = True Then
                    Console.ForegroundColor = ConsoleColor.DarkMagenta
                    Console.Write(LineNo & "  ")
                    Console.ForegroundColor = ConsoleColor.Gray
                End If
                Dim components() As String
                Try
                    components = Line.Split(New Char() {" "c})
                Catch
                    Errorupdate(LineNo)
                End Try
                linetype = Getlinetype(Line)
                If linetype = "NORMAL" Then
                    Keyword = Getkeyword(Line)
                ElseIf linetype = "EXPONENT" Then
                    Keyword = "EXPONENT"
                ElseIf linetype = "SUBSCRIPT" Then
                    Keyword = "SUBSCRIPT"
                ElseIf linetype = "MAIN" Then
                    Keyword = "ASSIGNMENT"
                Else
                    Keyword = "SPECIAL"
                End If
                If DisplayInterpreterComments = True Then
                    For i As Integer = 0 To (components.Length() - 1)
                        'Console.Write(components(i) & " ")
                        If components(i).Contains(";") Then
                            'newline
                            Console.Write(Removesemicolon(Line))
                        Else
                            Console.Write(components(i) & " ")
                        End If
                    Next
                End If
                If Keyword = "ERROR" Then
                    If DisplayInterpreterComments = True Then
                        Console.ForegroundColor = ConsoleColor.DarkYellow
                        Console.Write(" " & linetype)
                        Console.ForegroundColor = ConsoleColor.Red
                        Console.Write(" " & Keyword)
                        Console.ForegroundColor = ConsoleColor.Gray
                    End If
                    If ErrorLineNo = 0 Then
                        ErrorLineNo = LineNo
                    Else
                        multipleerrors = True
                    End If
                ElseIf Keyword = "N/A" Then
                    If DisplayInterpreterComments = True Then
                        Console.ForegroundColor = ConsoleColor.DarkYellow
                        Console.Write(" " & linetype)
                        Console.ForegroundColor = ConsoleColor.Yellow
                        Console.Write(" " & Keyword)
                        Console.ForegroundColor = ConsoleColor.Gray
                    End If
                Else
                    If DisplayInterpreterComments = True Then
                        Console.ForegroundColor = ConsoleColor.DarkYellow
                        Console.Write(" " & linetype)
                        Console.ForegroundColor = ConsoleColor.Green
                        Console.Write(" " & Keyword)
                        Console.ForegroundColor = ConsoleColor.Gray
                    End If
                End If
                If DisplayInterpreterComments = True Then
                    Console.WriteLine()
                End If
                '### interpret line of code ###
                If DisplayInterpreterComments = False And (linetype = "NORMAL" Or linetype = "EXPONENT" Or linetype = "SUBSCRIPT" Or linetype = "MAIN") Then
                    Try
                        SendLineToSub(Keyword)
                    Catch e As Exception
                        MsgBox($"The interpreter encountered an error. The interpreter may not function correctly.
Line: {LineNo}
Error: {e}", MsgBoxStyle.Exclamation, "Interpreter Error")

                    End Try
                End If
                LineNo = LineNo + 1
                If Keyword = "END OF PROGRAM" Then
                    endofprogram = True
                End If
            End Using
        Loop
        Console.WriteLine()
        Console.WriteLine("Error Summary:")
        If ErrorLineNo <> 0 And multipleerrors = False Then
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("Syntax error; Line: " & ErrorLineNo)
            Console.ForegroundColor = ConsoleColor.Gray
        ElseIf ErrorLineNo <> 0 And multipleerrors = True Then
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("Multiple Errors. First Error: Line: " & ErrorLineNo)
            Console.ForegroundColor = ConsoleColor.Gray
        ElseIf ErrorLineNo = 0 Then
            Console.ForegroundColor = ConsoleColor.Green
            Console.WriteLine("No Errors.")
            Console.ForegroundColor = ConsoleColor.Gray
        End If
    End Sub
    Sub SetLineNo(line)
        LineNo = line
    End Sub
    Sub SendLineToSub(Keyword)
        'setup components
        Dim components() As String = Line.Split(New Char() {" "c})
        For i As Integer = 0 To (components.Length() - 1)
            'Console.Write(components(i) & " ")
            If components(i).Contains(";") Then
                components(i) = (Removesemicolon(Line))
            End If
        Next
        'split brackets
        For i As Integer = 0 To (components.Length() - 1)
            Try
                If components(i).Contains("(") Then
                    Dim tempvalue(components.Length + 2) As String
                    For x As Integer = i To (components.Length - 1)
                        tempvalue(x + 1) = components(x)
                    Next
                    If ReturnOperator(Line, "(") <> Nothing Then
                        ReDim Preserve components(components.Length + 1) 'this was originally on line 193
                        components(i) = (ReturnOperator(Line, "("))
                        components(i + 1) = ReturnOperand(Line)

                        For x As Integer = i + 2 To (components.Length - 1)
                            components(x) = tempvalue(x)
                            x = x + 1
                        Next
                    Else
                        components(i) = (ReturnOperand(Line))
                    End If
                End If
            Catch 'do nothing
            End Try
        Next
        Select Case Keyword
            Case "DECLARATION"
                'string
                Dim debugvariablename As String = components(3) 'only for a watch (code watch, not a timepiece!)
                Try
                    If components(5).Contains("'") Then 'turn this into a proper sub once it works
                        Dim noofparts = (components.Length) - 5
                        Dim declaredstring As String = ""
                        For i = 0 To noofparts - 1
                            declaredstring = declaredstring & " " & components(i + 5)
                        Next
                        Dim declaredstringchars() As Char = declaredstring.ToCharArray()
                        For i = 0 To declaredstringchars.Length - 3
                            declaredstringchars(i) = declaredstringchars(i + 2)
                        Next
                        declaredstring = ""
                        For i = 0 To declaredstringchars.Length - 3
                            declaredstring = declaredstring & declaredstringchars(i)
                        Next
                        DeclareHALVariable(components(3), "STRING", declaredstring)
                    End If
                Catch
                End Try
                'notstring
                Try
                    If components(5) <> Nothing Then
                        Dim possiblefunctionname As String = components(5)
                        Dim functionexists As Boolean = False
                        Dim functionoutput As String
                        Try
                            If ProgramDefinedvariablesTypes(possiblefunctionname) = "FUNCTION" Then
                                functionexists = True
                                If functiontriggered = False Then
                                    dofunction(possiblefunctionname, 5)
                                    firstparameterposshift = 5
                                    linetomovetoafterfunctionexecution = linetomovetoafterfunctionexecution - 1
                                    Console.ForegroundColor = ConsoleColor.DarkCyan
                                    Console.WriteLine("Function '" & possiblefunctionname & "'")
                                    Console.ForegroundColor = ConsoleColor.Gray
                                Else
                                    functionoutput = ProgramDefinedVariables(possiblefunctionname)
                                    DeclareHALVariable(components(3), components(4), functionoutput)
                                End If
                            End If
                        Catch
                            'function does not exist
                        End Try
                        If functionexists = False Then
                            DeclareHALVariable(components(3), components(4), components(5))
                        End If
                        functionexists = False
                    Else
                        DeclareHALVariable(components(3), components(4), "0")
                    End If
                    If components(3) = "" Then
                        If multipleerrors = False Then
                            ErrorLineNo = LineNo
                        Else
                            multipleerrors = True
                        End If
                    End If
                Catch
                    Try
                        DeclareHALVariable(components(3), components(4), "0")
                    Catch
                        If multipleerrors = False Then
                            ErrorLineNo = LineNo
                        Else
                            multipleerrors = True
                        End If
                    End Try
                End Try
            Case "READ"
                Try
                    If ProgramDefinedvariablesTypes(components(4)) <> "CONSTANT" Then
                        Console.WriteLine("Channel " & components(3) & " requires input")
                        Dim inputstring As String = Console.ReadLine()
                        ProgramDefinedVariables(components(4)) = inputstring
                        Console.WriteLine("Variable '" & components(4) & "' set to " & ProgramDefinedVariables(components(4)))
                    Else
                        Console.ForegroundColor = ConsoleColor.Red
                        Console.WriteLine("Variable " & components(4) & " cannot be changed as it is a constant.")
                        Console.ForegroundColor = ConsoleColor.Gray
                        If ErrorLineNo = 0 Then
                            ErrorLineNo = LineNo
                        Else
                            multipleerrors = True
                        End If
                    End If
                Catch
                    Console.ForegroundColor = ConsoleColor.Red
                    Console.WriteLine("Variable " & components(4) & " does not exist")
                    Console.ForegroundColor = ConsoleColor.Gray
                    If ErrorLineNo = 0 Then
                        ErrorLineNo = LineNo
                    Else
                        multipleerrors = True
                    End If
                End Try
            Case "WRITE"
                Dim newcomponents() As String = Line.Split(New Char() {" "c})
                If newcomponents(3).Contains("'") Then
                    Dim noofparts = (newcomponents.Length) - 3
                    Dim declaredstring As String = ""
                    For i = 0 To (noofparts - 1)
                        If newcomponents(i + 3).Contains(";") Then newcomponents(i + 3) = Removesemicolon(newcomponents(i + 3))
                        declaredstring = declaredstring & " " & newcomponents(i + 3)
                    Next
                    Dim declaredstringchars() As Char = declaredstring.ToCharArray()
                    For i = 0 To declaredstringchars.Length - 3
                        declaredstringchars(i) = declaredstringchars(i + 2)
                    Next
                    declaredstring = ""
                    For i = 0 To declaredstringchars.Length - 3
                        declaredstring = declaredstring & declaredstringchars(i)
                    Next
                    If declaredstring.Contains("'") Then
                        Dim declaredstringchararray As Char() = declaredstring.ToCharArray()
                        declaredstring = ""
                        For i = 0 To declaredstringchararray.Length - 2
                            declaredstring = declaredstring & declaredstringchararray(i)
                        Next
                    End If
                    HALwrite(components(3), declaredstring, "string")
                Else
                    Try
                        Dim writecomponents() As String = Line.Split(New Char() {" "c})
                        HALwrite(components(3), Newarithmetic(Removesemicolon(writecomponents(3))), "normal")
                    Catch
                        Dim writecomponents() As String = Line.Split(New Char() {" "c})
                        Dim argumentno As Integer = 0
                        For i As Integer = 3 To writecomponents.Length - 1
                            If writecomponents(i) <> Nothing Then
                                argumentno = argumentno + 1
                            End If
                        Next
                        If argumentno = 1 Then
                            HALwrite(components(3), components(4), "normal")
                        ElseIf argumentno = 0 Then
                            If ErrorLineNo = 0 Then
                                ErrorLineNo = LineNo
                            Else
                                multipleerrors = True
                            End If
                        Else
                            'convert to literal //Not Needed anymore
                        End If
                    End Try
                End If
            Case "ASSIGNMENT"
                Try
                    Dim assignmentcomponents() As String = Line.Split(New Char() {" "c})
                    ' original line: ProgramDefinedVariables(assignmentcomponents(2)) = Removesemicolon(assignmentcomponents(4))
                    assignmentcomponents(4) = Removesemicolon(assignmentcomponents(4))
                    Try
                        ProgramDefinedVariables(assignmentcomponents(2)) = Newarithmetic(assignmentcomponents(4)) ' was = Newarithmetic(ProgramDefinedVariables(assignmentcomponents(2)))
                        Console.WriteLine("Variable '" & assignmentcomponents(2) & "' set to '" & ProgramDefinedVariables(assignmentcomponents(2)) & "'")
                    Catch
                    End Try
                Catch
                    Errorupdate(LineNo)
                End Try
            Case "CONDITIONAL EXECUTION"
                Dim elseexists As Boolean = True
                Conditionalexecution(components(3), components(5), components(4))
                Console.WriteLine("Comparison '" & components(3) & " " & components(4) & " " & components(5) & "' = " & condition)
                If condition = True Then
                ElseIf condition = False Then
                    Dim lineamounttochange As Integer = LineNo
                    Dim linecontainselse As Boolean = False
                    Using fileline As New StreamReader(workingfile)
                        Do Until linecontainselse
                            Dim line As String = "placeholder" 'sort out null reference
                            For i = 1 To lineamounttochange
                                line = fileline.ReadLine() ' sort out loop
                            Next
                            Try
                                If line.Contains("ELSE;") Then
                                    linecontainselse = True
                                    LineNo = LineNo + 1
                                Else
                                    lineamounttochange = 1
                                End If
                            Catch
                                elseexists = False
                                Exit Do
                            End Try
                        Loop
                    End Using
                    If elseexists = False Then movelinetokeyword("END")
                End If
            Case "ELSE"
                If condition = True Then
                    'move to "END;"
                    Dim lineamounttochange As Integer = LineNo
                    Dim linecontainsend As Boolean = False
                    Dim totallineschanged As Integer = 0
                    Using fileline As New StreamReader(workingfile)
                        Do Until linecontainsend
                            Dim line As String = "placeholder" 'stop null reference exceptions
                            For i = 1 To lineamounttochange
                                line = fileline.ReadLine() ' sort out loop
                            Next
                            If line.Contains("END;") Then
                                linecontainsend = True
                                LineNo = LineNo + totallineschanged
                            Else
                                lineamounttochange = 1
                                totallineschanged = totallineschanged + 1
                            End If
                        Loop
                    End Using
                End If
            Case "DO"
                linenotoloopto = LineNo
                If components(3) = "FOR" Then 'do for loop
                    If forloopset = False Then
                        ProgramDefinedVariables(components(4)) = components(6)
                        loopvariablename = components(4)
                        forloopset = True
                    End If
                    If ProgramDefinedVariables(components(4)) = components(8) Then
                        linenotoloopto = 0
                        Dim lineamounttochange As Integer = LineNo       '##### For 1 To 10 would generate i = 1 To 9, but still sdet i To 10 aafterwards
                        Dim linecontainsend As Boolean = False
                        Dim totallineschanged As Integer = 0
                        Using fileline As New StreamReader(workingfile)
                            Do Until linecontainsend
                                Dim line As String = "placeholder" 'stop null reference exception
                                For i = 1 To lineamounttochange
                                    line = fileline.ReadLine() ' sort out loop
                                Next
                                If line.Contains("END;") Then
                                    linecontainsend = True
                                    LineNo = LineNo + totallineschanged
                                    linenotoloopto = LineNo
                                Else
                                    lineamounttochange = 1
                                    totallineschanged = totallineschanged + 1
                                End If
                            Loop
                        End Using
                    Else
                        If forloopset = True Then
                            ProgramDefinedVariables(components(4)) = ProgramDefinedVariables(components(4)) + 1
                        End If
                    End If
                End If
            Case "END"
                If condition = False Then
                    If linenotoloopto = 0 Then
                        LineNo = LineNo + 1
                    Else
                        LineNo = linenotoloopto - 1
                    End If
                End If
                linenotoloopto = 0
            Case "EXPONENT"
                Exponentiate(Line)
            Case "FUNCTION"
                If functiontriggered = False Then 'create namespace for function
                    If ProgramDefinedVariables.ContainsKey(Removechar(components(2), ":")) Then
                    Else
                        DeclareHALVariable(Removechar(components(2), ":"), "FUNCTION", "NULL")
                    End If 'move to end of function
                    movelinetokeyword("CLOSE")
                Else
                    Try 'if there are parameters this won't catch
                        Dim variablestype As String = components(components.Length - 2)
                        Dim parameters As String = components(4)
                        Dim functionoperands() As String = parameters.Split(",")
                        If functionoperands.Length > 1 Then
                            For i = 0 To functionoperands.Length - 1
                                Try
                                    functionoperandvalues(i + firstparameterposshift) = Convert.ToInt64(functionoperandvalues(i + firstparameterposshift))
                                Catch
                                End Try
                                DeclareHALVariable(functionoperands(i), variablestype, functionoperandvalues(i + firstparameterposshift))
                            Next
                        Else
                            Try
                                functionoperandvalues(0 + firstparameterposshift) = Convert.ToDouble(functionoperandvalues(0 + firstparameterposshift))
                            Catch
                            End Try
                            DeclareHALVariable(functionoperands(0), variablestype, functionoperandvalues(0 + firstparameterposshift))
                        End If
                    Catch 'no parameters for this function
                    End Try
                End If 'else don't do anything (skip to next line ignoring this one)
            Case "FUNCTION INVOCATION"
                dofunction(components(2), 2)
                firstparameterposshift = 2
                Console.ForegroundColor = ConsoleColor.DarkCyan
                Console.WriteLine("Function '" & components(2) & "'")
                Console.ForegroundColor = ConsoleColor.Gray
            Case "END OF FUNCTION"
                If functiontriggered = True Then
                    LineNo = linetomovetoafterfunctionexecution
                    Console.ForegroundColor = ConsoleColor.DarkCyan
                    Console.WriteLine("End Of Function")
                    Console.ForegroundColor = ConsoleColor.Gray
                End If
            Case "FUNCTION RETURN"
                Try
                    ProgramDefinedVariables(activefunction) = ProgramDefinedVariables(components(3)) 'returning value of a variable
                Catch
                    Try
                        ProgramDefinedVariables(activefunction) = Convert.ToDouble(components(3)) 'returning a number
                    Catch
                        Try
                            ProgramDefinedVariables(activefunction) = Newarithmetic(components(3)) 'returning an arithmetic operation
                        Catch
                            ProgramDefinedVariables(activefunction) = components(3) ' returning a string
                        End Try
                    End Try
                End Try
                Console.WriteLine("Value of function '" & activefunction & "' set to " & ProgramDefinedVariables(activefunction))
            Case "SCHEDULE"
                'Console.Write("Scheduling is currently in Beta.")
                Dim schedulecomponents() As String = Line.Split(New Char() {" "c})
                Dim schedulepriority = Convert.ToInt64(ReturnOperand(Removechar(schedulecomponents(4), ",")))
                Dim repeattime As Double = 1 / Convert.ToDouble(Removechar(schedulecomponents(7), ";"))
                Dim functionnamearraypos As Integer
                Dim functionnamefound As Boolean = False
                Dim x As Integer = 0
                Try
                    Do Until functionnamefound
                        If procedurelocations(x) = schedulecomponents(3) Then
                            functionnamearraypos = x
                        End If
                        x = x + 1
                    Loop
                Catch
                    Console.WriteLine("Realtime mode not initialised.")
                    Errorupdate(LineNo)
                End Try
            Case "PROGRAM DECLARATION"
                Console.ForegroundColor = ConsoleColor.DarkBlue
                Console.WriteLine("Program '" & mainprogramname & "'")
                Console.ForegroundColor = ConsoleColor.Gray
            Case "END OF PROGRAM"
                Console.ForegroundColor = ConsoleColor.DarkBlue
                Console.WriteLine("End Of Program")
                Console.ForegroundColor = ConsoleColor.Gray
            Case Else
                'do nothing
        End Select
    End Sub
    Sub BuiltInFunctions()
        builtinfunctiongenerated = True
        defaultfunctions(0) = "SIN"
        defaultfunctions(1) = "COS"
        defaultfunctions(2) = "TAN"
        For i = 0 To defaultfunctions.Length - 1
            If defaultfunctions(i) <> Nothing Then
                DeclareHALVariable(defaultfunctions(i), "FUNCTION", "NULL")
            End If
        Next
    End Sub
    Function Newarithmetic(inputstring)
        If builtinfunctiongenerated = False Then
            BuiltInFunctions()
        End If
        Dim chararray() As Char = inputstring.ToCharArray()
        'convert from hal/s to standard notation (** -> ^)
        Dim charnotoremove As Integer = 0 'chars to remove from end
        For i = 0 To chararray.Length - 2
            If chararray(i) = "*" And chararray(i + 1) = "*" Then
                chararray(i) = "^"
                For x = (i + 2) To chararray.Length - 1
                    chararray(x - 1) = chararray(x)
                Next
                charnotoremove = charnotoremove + 1
            End If
        Next
        ReDim Preserve chararray(chararray.Length - (1 + charnotoremove))
        'cluster operands together i.e. from ("3", ".", "1", "4", "+", "3", "3") to ("3.14", "+", "33")
        Dim linecomponentsarray(chararray.Length) As String
        Dim firstoperandlength As Integer
        'firstoperand
        If Not (chararray(0) = "+" Or chararray(0) = "-" Or chararray(0) = "*" Or chararray(0) = "/" Or chararray(0) = "^" Or chararray(0) = "(") Then
            Dim reachedterminator As Boolean = False
            Dim firstoperand As String = ""
            Dim x As Integer = 0
            Do Until reachedterminator
                Try
                    If chararray(x) = "+" Or chararray(x) = "-" Or chararray(x) = "*" Or chararray(x) = "/" Or chararray(x) = "^" Or chararray(x) = "(" Or x = chararray.Length Then 'or x = chararray.Length -1

                        reachedterminator = True
                    Else
                        firstoperand = firstoperand & chararray(x)
                        x = x + 1
                    End If
                Catch 'end of inputstring
                    reachedterminator = True
                End Try
            Loop
            firstoperandlength = x
            linecomponentsarray(0) = firstoperand
        Else
            linecomponentsarray(0) = chararray(0)
        End If
        'all other operands (and operators)
        For i = (firstoperandlength) To chararray.Length - 1
            Dim reachedterminator As Boolean = False
            Dim operand As String = ""
            Dim x As Integer = 0
            If chararray(i) = "+" Or chararray(i) = "-" Or chararray(i) = "*" Or chararray(i) = "/" Or chararray(i) = "^" Or chararray(i) = "(" Or chararray(i) = ")" Then
                x = i + 1
                Do Until reachedterminator
                    Try
                        If chararray(x) = "+" Or chararray(x) = "-" Or chararray(x) = "*" Or chararray(x) = "/" Or chararray(x) = "^" Or chararray(x) = "(" Or chararray(x) = ")" Then
                            reachedterminator = True
                        Else
                            operand = operand & chararray(x)
                            x = x + 1
                        End If
                    Catch
                        reachedterminator = True
                    End Try
                Loop
                linecomponentsarray(i) = chararray(i)
                If operand <> "" Then
                    linecomponentsarray(i + 1) = operand
                End If
                i = x - 1
            Else
                linecomponentsarray(i) = chararray(i)
            End If
        Next
        'remove blank spaces in array
        Dim noofdata As Integer = 0
        For i As Integer = 0 To linecomponentsarray.Length - 1
            If linecomponentsarray(i) <> Nothing Then
                noofdata = noofdata + 1
            End If
        Next
        Dim dataarray(noofdata) As String
        For i As Integer = 0 To linecomponentsarray.Length - 1
            If linecomponentsarray(i) <> Nothing Then
                For x = 0 To noofdata - 1
                    If dataarray(x) = Nothing Then
                        dataarray(x) = linecomponentsarray(i)
                        Exit For
                    End If
                Next
            End If
        Next
        ReDim Preserve dataarray(noofdata - 1)
        ReDim linecomponentsarray(noofdata - 1)
        For i = 0 To noofdata - 1
            linecomponentsarray(i) = dataarray(i)
        Next
        '###CONVERT VARIABLE NAMES TO NUMBERS HERE### (in hal/s interpreter)
        For i = 0 To linecomponentsarray.Length - 1
            If ProgramDefinedVariables.ContainsKey(linecomponentsarray(i)) Then
                If ProgramDefinedvariablesTypes(linecomponentsarray(i)) <> "FUNCTION" Then
                    linecomponentsarray(i) = ProgramDefinedVariables(linecomponentsarray(i))
                Else
                    'function
                End If
            End If
        Next
        '###ADD FUNCTION SUPPORT HERE
        Dim functionno As Integer
        'create list of function to reset later (and no of functions to do)
        Dim linefunctions As New List(Of String)
        For i = 0 To linecomponentsarray.Length - 1
            If ProgramDefinedVariables.ContainsKey(linecomponentsarray(i)) Then
                If ProgramDefinedvariablesTypes(linecomponentsarray(i)) = "FUNCTION" Then
                    functionno = functionno + 1
                    linefunctions.Add(linecomponentsarray(i))
                End If
            End If
        Next
        Do Until functionno = 0
            Try
                For i = 0 To linecomponentsarray.Length - 1
                    If ProgramDefinedVariables.ContainsKey(linecomponentsarray(i)) Then
                        If ProgramDefinedvariablesTypes(linecomponentsarray(i)) = "FUNCTION" Then
                            If ProgramDefinedVariables(linecomponentsarray(i)) = "NULL" Then
                                doarithmeticfunction(linecomponentsarray(i), i, linecomponentsarray) '###Need to send the value of the function back to this thread.
                                Console.ForegroundColor = ConsoleColor.DarkCyan
                                Console.WriteLine("Function '" & linecomponentsarray(i) & "'")
                                Console.ForegroundColor = ConsoleColor.Gray
                                functionno = functionno - 1
                                Exit Do
                            Else
                                linecomponentsarray(i) = ProgramDefinedVariables(linecomponentsarray(i))
                                For x As Integer = i + 1 To linecomponentsarray.Length - 4
                                    linecomponentsarray(x) = linecomponentsarray(x + 3)
                                Next
                                functionno = functionno - 1
                                ReDim Preserve linecomponentsarray(linecomponentsarray.Length - 4)
                            End If
                        End If
                    End If
                Next
            Catch
            End Try
        Loop
        For Each item In linefunctions
            ProgramDefinedVariables(item) = "NULL"
        Next
        '###Shunting Yard Algorithm###
        Dim input(linecomponentsarray.Length - 1) As String
        'change linecomponents over to input (to make debugging easier)
        For i = 0 To linecomponentsarray.Length - 1
            input(i) = linecomponentsarray(i)
        Next
        Dim token As String
        Dim output As New List(Of String)
        Dim operatorstack As Stack = New Stack
        Dim operatorprecedence As Integer
        Dim lastoperatorprecendence As Integer
        Dim spacejunk As New List(Of String)
        For i = 0 To input.Length - 1
            token = input(i)
            Dim isoperator As Boolean
            Try
                token = Convert.ToDouble(token)
                isoperator = False
            Catch
                isoperator = True
            End Try
            If isoperator = False Then output.Add(token)
            If isoperator = True Then
                Try
                    operatorprecedence = getoperatorprecedence(token)
                Catch
                    Console.WriteLine("Invalid algebraic string")
                End Try
                Dim stoploop As Boolean = False
                Dim lastoperatorassociativity As String
                Do Until stoploop
                    Try
                        lastoperatorprecendence = getoperatorprecedence(operatorstack(0))
                        lastoperatorassociativity = getassociation(operatorstack(0))
                    Catch 'no operators
                        lastoperatorprecendence = -1 'not 0 as that would mess up '('
                        lastoperatorassociativity = "L"
                    End Try
                    If lastoperatorprecendence >= operatorprecedence And operatorstack(0) <> "(" And operatorstack(0) <> ")" And lastoperatorassociativity = "L" Then
                        output.Add(operatorstack.Pop())
                    ElseIf token = "(" Then
                        operatorstack.Push(token)
                        stoploop = True
                    ElseIf token = ")" Then
                        While operatorstack(0) <> "("
                            output.Add(operatorstack.Pop())
                        End While
                        spacejunk.Add(operatorstack.Pop())
                        stoploop = True
                    ElseIf lastoperatorprecendence < operatorprecedence And token = "^" Then
                        operatorstack.Push(token)
                        stoploop = True
                    ElseIf lastoperatorprecendence >= operatorprecedence And operatorstack(0) <> "(" And operatorstack(0) <> ")" Then
                        output.Add(operatorstack.Pop())
                    Else
                        operatorstack.Push(token)
                        stoploop = True
                    End If
                Loop
            End If
        Next
        'move remaining operators
        Do Until operatorstack(0) = Nothing
            output.Add(operatorstack.Pop())
        Loop
        'Console.Write("RPN: ") 'output rpn
        'For Each item In output
        '    Console.Write(item & ", ")
        'Next
        'work out value of rpn 
        Dim RPNstack(output.Count - 1)
        Dim transferloopcount As Integer = 0
        Dim operationpoint As Integer = 0
        For Each item In output
            Select Case item
                Case "+"
                    RPNstack(transferloopcount - 2) = Convert.ToDouble(RPNstack(transferloopcount - 2)) + Convert.ToDouble(RPNstack(transferloopcount - 1))
                    RPNstack(transferloopcount - 1) = Nothing
                    operationpoint = transferloopcount
                    transferloopcount = transferloopcount - 1
                Case "-"
                    RPNstack(transferloopcount - 2) = Convert.ToDouble(RPNstack(transferloopcount - 2)) - Convert.ToDouble(RPNstack(transferloopcount - 1))
                    RPNstack(transferloopcount - 1) = Nothing
                    operationpoint = transferloopcount
                    transferloopcount = transferloopcount - 1
                Case "/"
                    RPNstack(transferloopcount - 2) = Convert.ToDouble(RPNstack(transferloopcount - 2)) / Convert.ToDouble(RPNstack(transferloopcount - 1))
                    RPNstack(transferloopcount - 1) = Nothing
                    operationpoint = transferloopcount
                    transferloopcount = transferloopcount - 1
                Case "*"
                    RPNstack(transferloopcount - 2) = Convert.ToDouble(RPNstack(transferloopcount - 2)) * Convert.ToDouble(RPNstack(transferloopcount - 1))
                    RPNstack(transferloopcount - 1) = Nothing
                    operationpoint = transferloopcount
                    transferloopcount = transferloopcount - 1
                Case "^"
                    RPNstack(transferloopcount - 2) = Convert.ToDouble(RPNstack(transferloopcount - 2)) ^ Convert.ToDouble(RPNstack(transferloopcount - 1))
                    RPNstack(transferloopcount - 1) = Nothing
                    operationpoint = transferloopcount
                    transferloopcount = transferloopcount - 1
                Case Else
                    RPNstack(transferloopcount) = item
                    transferloopcount = transferloopcount + 1
            End Select
            For i = operationpoint To RPNstack.Length - 1
                If RPNstack(i) = Nothing Then
                    Try
                        RPNstack(i) = RPNstack(i + 1)
                    Catch
                    End Try
                End If
            Next
        Next
        Console.WriteLine("Arithmetic Operation Returned: " & RPNstack(0))
        Return RPNstack(0)
    End Function
    Function getoperatorprecedence(token)
        Select Case token
            Case "+"
                Return 2
            Case "-"
                Return 2
            Case "/"
                Return 3
            Case "*"
                Return 3
            Case "^"
                Return 4
            Case "("
                Return 9
            Case ")"
                Return 0
            Case Else
                Return "ERROR"
        End Select
    End Function
    Function getassociation(token)
        If token = "^" Then
            Return "R"
        Else
            Return "L"
        End If
    End Function
    Sub movelinetokeyword(keywordtofind)
        Dim lineamounttochange As Integer = LineNo
        Dim linecontainsend As Boolean = False
        Dim totallineschanged As Integer = 0
        Using fileline As New StreamReader(workingfile)
            Do Until linecontainsend
                Dim line As String = "placeholder" 'stop null reference exceptions
                For i = 1 To lineamounttochange
                    line = fileline.ReadLine() ' sort out loop
                Next
                If line.Contains(keywordtofind) Then
                    linecontainsend = True
                    LineNo = LineNo + totallineschanged
                Else
                    lineamounttochange = 1
                    totallineschanged = totallineschanged + 1
                End If
            Loop
        End Using
    End Sub
    Sub Conditionalexecution(value1, value2, comparator)
        Try
            value1 = Newarithmetic(value1)
        Catch
            value1 = ProgramDefinedVariables(value1)
        End Try
        Try
            value2 = Newarithmetic(value1)
        Catch
            value2 = ProgramDefinedVariables(value2)
        End Try
        If comparator = "=" Then
            If value1 = value2 Then
                condition = True
            Else
                condition = False
            End If
        ElseIf comparator = ">" Then
            If value1 > value2 Then
                condition = True
            Else
                condition = False
            End If
        ElseIf comparator = "<" Then
            If value1 < value2 Then
                condition = True
            Else
                condition = False
            End If
        ElseIf comparator = ">=" Then
            If value1 >= value2 Then
                condition = True
            Else
                condition = False
            End If
        ElseIf comparator = "<=" Then
            If value1 <= value2 Then
                condition = True
            Else
                condition = False
            End If
        End If
    End Sub
    Sub HALwrite(channel As Integer, identifier As String, type As String)
        Try
            If identifier <> Nothing Then
                Console.WriteLine("Channel " & channel & ": ")
                If type = "string" Then Console.ForegroundColor = ConsoleColor.DarkMagenta
                Console.WriteLine(identifier)
                Console.ForegroundColor = ConsoleColor.Gray
            End If
        Catch
            If ErrorLineNo = 0 Then
                ErrorLineNo = LineNo
            Else
                multipleerrors = True
            End If
            Console.ForegroundColor = ConsoleColor.Red
            Console.WriteLine("Variable '" & identifier & "' has not been declared or has null value")
            Console.ForegroundColor = ConsoleColor.Gray
        End Try
    End Sub
    Function Trig(expression, trigfunction)
        Select Case trigfunction
            Case "SIN"
                Return Math.Sin(expression)
            Case "COS"
                Return Math.Cos(expression)
            Case "TAN"
                Return Math.Tan(expression)
            Case Else
                Return "ERROR"
        End Select
    End Function
    Sub Exponentiate(line)
        exponentline = True
        line = line.ToString
        Dim components() As Char = line.tochararray()
        exponentno = 0
        Dim tempvalue As Integer
        For i = 0 To components.Length - 1
            If components(i) <> " " And components(i) <> "E" Then
                tempvalue = Convert.ToInt16(components(i).ToString)
                exponents(i) = tempvalue
                exponentno = exponentno + 1
            End If
        Next
    End Sub
    Sub DeclareHALVariable(identifier, type, value)
        Dim isdefaultFunction As Boolean = False
        For i = 0 To defaultfunctions.Length - 1
            If defaultfunctions(i) = identifier Then isdefaultFunction = True
        Next
        If value <> "" Then
            Try
                If value.contains(";") Then value = Removesemicolon(value)
                value = Newarithmetic(value)
            Catch
            End Try
            ProgramDefinedVariables(identifier) = value
            ProgramDefinedvariablesTypes(identifier) = type
            If type = "STRING" Then
                Console.WriteLine("Variable '" & identifier & "' of type '" & ProgramDefinedvariablesTypes(identifier) & "' created with value '" & ProgramDefinedVariables(identifier) & "'")
            Else
                If isdefaultFunction = False Then
                    Console.WriteLine("Variable '" & identifier & "' of type '" & ProgramDefinedvariablesTypes(identifier) & "' created with value " & ProgramDefinedVariables(identifier))
                End If
            End If
        Else
            ProgramDefinedVariables(identifier) = "0"
            ProgramDefinedvariablesTypes(identifier) = type
            If type <> "" Then
                Console.WriteLine("Variable '" & identifier & "' of type '" & ProgramDefinedvariablesTypes(identifier) & "' created and unassigned.")
            End If
        End If
    End Sub
    Function Removesemicolon(line)
        Dim lastword As String = ""
        Dim components() As String = line.Split(New Char() {" "c})
        For i As Integer = 0 To (components.Length - 1)
            If components(i).Contains(";") Then
                Dim charArray() As Char = components(i).ToCharArray
                For x As Integer = 0 To (charArray.Length - 2)
                    lastword = lastword & charArray(x)
                Next
            End If
        Next
        Return lastword
    End Function
    Function Removechar(line, chartoremove)
        Dim lastword As String = ""
        Dim components() As String = line.Split(New Char() {" "c})
        For i As Integer = 0 To (components.Length - 1)
            If components(i).Contains(chartoremove) Then
                Dim charArray() As Char = components(i).ToCharArray
                For x As Integer = 0 To (charArray.Length - 2)
                    lastword = lastword & charArray(x)
                Next
            End If
        Next
        Return lastword
    End Function
    Sub dofunction(functionname, firstparameterpos)
        activefunction = functionname
        linetomovetoafterfunctionexecution = LineNo
        LineNo = -1
        movelinetokeyword(functionname)
        functiontriggered = True
        Dim newcomponents() As String = Line.Split(New Char() {" "c})
        For i As Integer = 0 To (newcomponents.Length() - 1)
            'Console.Write(components(i) & " ")
            If newcomponents(i).Contains(";") Then
                newcomponents(i) = (Removesemicolon(Line))
            End If
        Next
        newcomponents(firstparameterpos) = ReturnOperand(newcomponents(firstparameterpos) & ")") 'trick returnoperand into allowing the broken input
        If newcomponents(firstparameterpos).Contains(",") Then newcomponents(firstparameterpos) = Removechar(newcomponents(firstparameterpos), ",")
        If ProgramDefinedVariables.ContainsKey(newcomponents(firstparameterpos)) Then
            functionoperandvalues(firstparameterpos) = ProgramDefinedVariables(newcomponents(firstparameterpos))
        Else
            functionoperandvalues(firstparameterpos) = newcomponents(firstparameterpos)
        End If
        For i = (firstparameterpos + 1) To newcomponents.Length - 1 'sort out so it allocates the operands for the function ########### 
            If newcomponents(i).Contains(",") Then
                Try
                    functionoperandvalues(i) = ProgramDefinedVariables(Removechar(newcomponents(i), ","))
                Catch
                    functionoperandvalues(i) = Removechar(newcomponents(i), ",")
                End Try
            ElseIf newcomponents(i).Contains(")") Then
                Try
                    functionoperandvalues(i) = ProgramDefinedVariables(Removechar(newcomponents(i), ")"))
                Catch
                    functionoperandvalues(i) = Removechar(newcomponents(i), ")")
                End Try
            Else
                Try
                    functionoperandvalues(i) = ProgramDefinedVariables(newcomponents(i))
                Catch
                    functionoperandvalues(i) = newcomponents(i)
                End Try
            End If
        Next
    End Sub
    Sub doarithmeticfunction(functionname, functionpos, linecomponentsarray())
        activefunction = functionname
        Dim isdefaultfunction As Boolean = False
        For j = 0 To defaultfunctions.Length - 1
            If activefunction = defaultfunctions(j) Then isdefaultfunction = True
        Next
        If isdefaultfunction = False Then
            linetomovetoafterfunctionexecution = LineNo - 1
            LineNo = -1
            movelinetokeyword(functionname)
            functiontriggered = True
            Dim parameterstring As String = linecomponentsarray(functionpos + 2)
            Dim charparameters() As Char = parameterstring.ToCharArray()
            Dim commasremoved As Integer = 0
            Dim parameters As New List(Of String)
            Dim currentoperand As String = ""
            For i = 0 To charparameters.Length - 1
                If charparameters(i) = "," Then
                    parameters.Add(currentoperand)
                    currentoperand = ""
                    commasremoved = commasremoved + 1
                Else
                    currentoperand = currentoperand & charparameters(i)
                End If
            Next
            parameters.Add(currentoperand)
            If parameters.Count > 1 Then
                Dim finalparameters(parameters.Count - 1)
                For i = 0 To parameters.Count - commasremoved
                    Try
                        finalparameters(i) = parameters(i)
                    Catch
                    End Try
                Next
                For i = 0 To finalparameters.Length - 1
                    functionoperandvalues(i) = finalparameters(i)
                Next
            Else
                functionoperandvalues(0) = parameters(0)
            End If
        Else 'default functions
            LineNo = LineNo - 1

        End If
    End Sub
    Function ReturnOperator(line, charactertoremove)
        Dim operatorhalf As String = Nothing
        Dim components() As String = line.Split(New Char() {" "c})
        For i As Integer = 0 To (components.Length - 1)
            If components(i).Contains(charactertoremove) Then
                Dim charArray() As Char = components(i).ToCharArray
                Dim leftbracket As Boolean = False
                Dim x As Integer = 0
                Do Until leftbracket = True
                    If charArray(x) <> charactertoremove Then
                        operatorhalf = operatorhalf & charArray(x)
                    Else
                        leftbracket = True
                    End If
                    x = x + 1
                Loop
            End If
        Next
        Return operatorhalf
    End Function
    Function ReturnOperand(line)
        Dim operand As String = ""
        Dim components() As String = line.Split(New Char() {" "c})
        For i As Integer = 0 To (components.Length - 1)
            If components(i).Contains("(") Then
                Dim charArray() As Char = components(i).ToCharArray
                Dim leftbracket As Boolean = False
                Dim rightbracket As Boolean = False
                Dim x As Integer = 0
                Do Until leftbracket = True
                    If charArray(x) = "(" Then
                        Do Until rightbracket = True
                            If charArray(x) <> ")" And charArray(x) <> "(" Then
                                operand = operand & charArray(x)
                            ElseIf charArray(x) = ")" Then
                                rightbracket = True
                            End If 'fix so it doesn't output write6
                            If charArray(x) = ")" Then
                                rightbracket = True
                            End If
                            x = x + 1
                        Loop
                        leftbracket = True
                    End If
                    x = x + 1
                Loop
            End If
        Next
        Return operand
    End Function
    Function Getkeyword(line)
        Dim firstpart As String = "NULL"
        Dim components() As String = line.Split(New Char() {" "c})
        For i = 0 To components.Length - 1
            If components(i).Contains("(") Then
                components(i) = ReturnOperator(line, "(")
            End If
        Next
        Try
            If components(2).Contains(":") And components(3) = "FUNCTION" Then Return "FUNCTION"
        Catch
        End Try
        Dim istext As Boolean = False
        Dim x As Integer = -1
        Do Until istext
            x = x + 1
            If components(x) <> Nothing Then
                istext = True
            End If
        Loop
        firstpart = components(x)
        Try
            Select Case firstpart
            'E, M, S, C, D
                'Case "E" 'ignore
                'Case "M"
                'Case "S"
                'Case "C"
                '   Return "COMMENT"
                'Case "D"
                Case "DECLARE"
                    Return "DECLARATION"
                Case "SIMPLE:"
                    mainprogramname = Removesemicolon(components(3))
                    Return "PROGRAM DECLARATION"
                Case "CLOSE"
                    If Removesemicolon(components(3)) = "SIMPLE" Then
                        Return "END OF PROGRAM"
                    Else
                        Return "END OF FUNCTION"
                        functiontriggered = False
                    End If
                Case "READ"
                    Return "READ"
                Case "WRITE"
                    Return "WRITE"
                Case "IF"
                    Return "CONDITIONAL EXECUTION"
                Case "ELSE;"
                    Return "ELSE"
                Case "END"
                    Return "END"
                Case "END;"
                    Return "END"
                Case "DO"
                    Return "DO"
                Case ProgramDefinedVariables.ContainsKey(firstpart)
                    'Console.WriteLine("assignment")
                    Return "ASSIGNMENT"
                Case "FUNCTION"
                    Return "FUNCTION PARAMETERS"
                Case "RETURN"
                    Return "FUNCTION RETURN"
                Case "SCHEDULE"
                    Return "SCHEDULE"
                Case Else
                    If ProgramDefinedVariables.ContainsKey(firstpart) Then
                        'Console.WriteLine("assignment")
                        If ProgramDefinedvariablesTypes(firstpart) = "FUNCTION" Then
                            Return "FUNCTION INVOCATION"
                        Else
                            Return "ASSIGNMENT"
                        End If
                    Else
                            Return "N/A"
                    End If
            End Select
        Catch
            Return "Error"
        End Try
    End Function
    Function Getlinetype(line)
        Dim components() As String = line.Split(New Char() {" "c})
        Select Case components(0)
            'line is marked
            Case "E"
                Return "EXPONENT"
            Case "M"
                Return "MAIN"
            Case "S"
                Return "SUBSCRIPT"
            Case "C"
                Return "COMMENT"
            Case "D"
                Return "OTHER"
            Case Else
                Return "NORMAL"
        End Select
    End Function
    Sub Userinput()
        Dim quit As Boolean = False
        Do Until quit
            Console.WriteLine("Do you want To view(v), create(c) variables Or quit(q)")
            operationchoice = Console.ReadLine()
            If operationchoice = "c" Then
                Console.WriteLine("Enter a variable name")
                variablenametocreate = Console.ReadLine()
                Console.WriteLine("Enter the variable type")
                variabletype = Console.ReadLine()
                Console.WriteLine("Enter variable value (variable Is a " & variabletype & ")")
                variablevalue = Console.ReadLine()
                ProgramDefinedVariables(variablenametocreate) = variablevalue
                ProgramDefinedvariablesTypes(variablenametocreate) = variabletype
            ElseIf operationchoice = "v" Then
                Console.WriteLine("Enter a variable To read")
                variabletoread = Console.ReadLine()
                Console.WriteLine("Showing variable '" & variabletoread & "' of type " & ProgramDefinedvariablesTypes(variabletoread))
                Try
                    If ProgramDefinedVariables(variabletoread) = "" Then
                        Console.WriteLine("Value: Null Value")
                    Else
                        Console.WriteLine("Value: " & ProgramDefinedVariables(variabletoread))
                    End If
                Catch
                    Console.WriteLine("Value: Null Value")
                End Try
            ElseIf operationchoice = "q" Then
                quit = True
            End If
        Loop

    End Sub
    Sub Errorupdate(lineno)
        If ErrorLineNo = 0 Then
            ErrorLineNo = lineno
        Else
            multipleerrors = True
        End If
    End Sub
    Sub Logo()
        Console.WriteLine(" _    _     __    __         __   ____")
        Console.WriteLine("| |__| |   //\\   | |       / /  |  __|")
        Console.WriteLine("|  __  |  //__\\  | |__    / /   |__  |")
        Console.WriteLine("|_|  |_| //    \\ |____|  /_/    |____|")
        Console.WriteLine()
    End Sub
    Sub Tribute()
        Console.WriteLine("Written entirely by Thomas Curry with extensive reference to NASA's 'Programming in HAL /S' (1978) By Michael J Ryer.")
        Console.WriteLine("Dedicated to the crews of STS-51-L Challenger and STS-107 Columbia.")
        Console.WriteLine("STS-51-L:")
        Console.WriteLine("Commander Dick Scobee")
        Console.WriteLine("Pilot Michael Smith")
        Console.WriteLine("Gregory Jarvis")
        Console.WriteLine("Elison Onazuka")
        Console.WriteLine("Judith Resnick")
        Console.WriteLine("Ronald Mcnair")
        Console.WriteLine("Christa Mcaulife")
        Console.WriteLine("STS-107:")
        Console.WriteLine("Commander Rick Husband")
        Console.WriteLine("Pilot William McCool")
        Console.WriteLine("David Brown")
        Console.WriteLine("Kalpana Chawla")
        Console.WriteLine("Michael Anderson")
        Console.WriteLine("Laurel Clark")
        Console.WriteLine("Ilan Ramon")
    End Sub
End Module

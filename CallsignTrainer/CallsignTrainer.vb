Imports System.IO

Module CallsignTrainer
    Sub Main()
        Dim rnd = New Random(Now.Ticks Mod Integer.MaxValue)
        Dim sp = New Speech.Synthesis.SpeechSynthesizer()
        Dim phoneticDic As Dictionary(Of Char, String())
        Dim callbook As String() = Nothing

        Dim testMode As Boolean
        Dim engMode As Boolean
        Dim extendedMode As Boolean
        Dim talkMode As Boolean
        While True
            ClearScr(True)

            Console.WriteLine()
            Console.WriteLine("Режим:")
            Console.WriteLine("0 - обучение (RU)")
            Console.WriteLine("1 - обучение (EN)")
            Console.WriteLine("2 - обучение (RU), расширенное")
            Console.WriteLine("3 - обучение (EN), расширенное")
            Console.WriteLine("4 - самотестирование (RU)")
            Console.WriteLine("5 - самотестирование (EN)")
            Console.WriteLine("6 - самотестирование (RU), расширенное")
            Console.WriteLine("7 - самотестирование (EN), расширенное")
            Console.WriteLine("8 - озвучивание (RU)")
            Console.WriteLine("9 - озвучивание (EN)")

            Dim modeStr = Console.ReadLine().Trim()

            Select Case modeStr
                Case "0"
                    testMode = False
                    engMode = False
                    extendedMode = False
                    talkMode = False
                Case "1"
                    testMode = False
                    engMode = True
                    extendedMode = False
                    talkMode = False
                Case "2"
                    testMode = False
                    engMode = False
                    extendedMode = True
                    talkMode = False
                Case "3"
                    testMode = False
                    engMode = True
                    extendedMode = True
                    talkMode = False
                Case "4"
                    testMode = True
                    engMode = False
                    extendedMode = False
                    talkMode = False
                Case "5"
                    testMode = True
                    engMode = True
                    extendedMode = False
                    talkMode = False
                Case "6"
                    testMode = True
                    engMode = False
                    extendedMode = True
                    talkMode = False
                Case "7"
                    testMode = True
                    engMode = True
                    extendedMode = True
                    talkMode = False
                Case "8"
                    testMode = False
                    engMode = False
                    extendedMode = False
                    talkMode = True
                Case "9"
                    testMode = False
                    engMode = True
                    extendedMode = False
                    talkMode = True
                Case Else
                    Continue While
            End Select
            Exit While
        End While

        If Not talkMode Then
            Try
                callbook = File.ReadAllLines("Callbook.txt")
            Catch ex As Exception
                Console.WriteLine()
                Console.WriteLine("Не могу найти файл 'callbook.txt' со списком позывных")
                Console.WriteLine("Нажмите любую клавишу для выхода из программы...")
                Console.ReadKey()
                Return
            End Try
        End If

        Dim voiceName As String = String.Empty
        If Not engMode Then
            voiceName = GetInstalledVoiceName(False)
            If String.IsNullOrEmpty(voiceName) Then
                Console.WriteLine()
                Console.WriteLine("Не найден диктор 'RU'")
                Console.WriteLine("Нажмите любую клавишу для выхода из программы...")
                Console.ReadKey()
            End If
            phoneticDic = GetPhoneticDicRU()
        Else
            voiceName = GetInstalledVoiceName(True)
            If String.IsNullOrEmpty(voiceName) Then
                Console.WriteLine()
                Console.WriteLine("Не найден диктор 'EN'")
                Console.WriteLine("Нажмите любую клавишу для выхода из программы...")
                Console.ReadKey()
            End If
            phoneticDic = GetPhoneticDicEN()
        End If
        sp.SelectVoice(voiceName)

        ClearScr(False)

        Dim repeat = False
        Dim talkCallsign As String = String.Empty
        While True
            ClearScr(False)

            If Not repeat Then
                If Not talkMode Then
                    talkCallsign = callbook(rnd.Next Mod callbook.Length)
                Else
                    Console.Write("ПОЗЫВНОЙ:")
                    talkCallsign = Console.ReadLine().ToUpper()
                End If
            Else
                repeat = False
            End If

            If Not testMode Then
                Console.WriteLine(talkCallsign)
                Console.WriteLine()
            End If

            Dim callsignStr = PhoneticConvert(phoneticDic, talkCallsign, extendedMode, rnd)
            sp.Speak(callsignStr)

            Dim choice As ConsoleKeyInfo
            Do
                If Not testMode Then
                    Console.WriteLine("ENTER - продолжение (ESC - повтор, Ctrl + C - выход)...")
                Else
                    Console.WriteLine("ENTER - ввод позывного (ESC - повтор, Ctrl + C - выход)...")
                End If
                choice = Console.ReadKey()
                ClearScr(False)
            Loop While choice.Key <> ConsoleKey.Enter AndAlso choice.Key <> ConsoleKey.Escape

            If choice.Key = ConsoleKey.Escape Then
                repeat = True
                Continue While
            End If

            If testMode Then
                Console.Write("ПОЗЫВНОЙ:")
                Dim userCall = Console.ReadLine().ToUpper()
                Console.WriteLine("ПРОВЕРКА:" + talkCallsign)
                If userCall = talkCallsign Then
                    Console.WriteLine("ВЕРНО!")
                Else
                    Console.WriteLine("ОШИБКА!")
                End If
                Console.WriteLine("Нажмите любую клавишу для продолжения (Ctrl + C - выход)...")
                Console.ReadKey()
            End If
        End While
    End Sub

    Private Function PhoneticConvert(dic As Dictionary(Of Char, String()), callsignStr As String, extendedMode As Boolean, rnd As Random) As String
        Dim callsignStrWords = String.Join(" ", callsignStr.Select(Function(c)
                                                                       If dic.ContainsKey(c) Then
                                                                           Dim words = dic(c)
                                                                           If extendedMode Then
                                                                               Return words(rnd.Next Mod words.Length)
                                                                           Else
                                                                               Return words(0) 'В обычном режиме берем только первый вариант
                                                                           End If
                                                                       Else
                                                                           Return "_"c
                                                                       End If
                                                                   End Function)).Replace("_", String.Empty)
        Return callsignStrWords
    End Function

    Private Function GetPhoneticDicRU() As Dictionary(Of Char, String())
        Dim dic = New Dictionary(Of Char, String())()
        With dic
            .Add("A", {"анна", "антон"})
            .Add("B", {"борис"})
            .Add("C", {"центр", "цапля"})
            .Add("D", {"дмитрий", "дима"})
            .Add("E", {"елена"})
            .Add("F", {"фёдор"})
            .Add("G", {"галина", "григорий"})
            .Add("H", {"харитон"})
            .Add("I", {"иван"})
            .Add("J", {"иван краткий", "йот"})
            .Add("K", {"киловатт", "константин"})
            .Add("L", {"леонид"})
            .Add("M", {"михаил", "мария"})
            .Add("N", {"николай"})
            .Add("O", {"ольга"})
            .Add("P", {"павел"})
            .Add("Q", {"щука"})
            .Add("R", {"роман", "радио"})
            .Add("S", {"сергей", "семен"})
            .Add("T", {"тамара", "татьяна"})
            .Add("U", {"ульяна"})
            .Add("V", {"жук", "женя"})
            .Add("W", {"василий"})
            .Add("X", {"знак", "икс"})
            .Add("Y", {"игрэк", "еры"})
            .Add("Z", {"зинаида", "зоя"})
            .Add("0", {"ноль"})
            .Add("1", {"один"})
            .Add("2", {"два"})
            .Add("3", {"три"})
            .Add("4", {"четыре"})
            .Add("5", {"пять"})
            .Add("6", {"шесть"})
            .Add("7", {"семь"})
            .Add("8", {"восемь"})
            .Add("9", {"девять"})
        End With
        Return dic
    End Function

    Private Function GetPhoneticDicEN() As Dictionary(Of Char, String())
        Dim dic = New Dictionary(Of Char, String())()
        With dic
            .Add("A", {"alfa", "america", "able"})
            .Add("B", {"bravo", "boston", "baker"})
            .Add("C", {"charlie", "canada"})
            .Add("D", {"delta", "denmark", "david"})
            .Add("E", {"echo", "ecuador", "england"})
            .Add("F", {"foxtrot", "florida", "frank"})
            .Add("G", {"golf", "germany", "george"})
            .Add("H", {"hotel", "henry"})
            .Add("I", {"eeetaly", "india", "item"}) 'eeetaly => italy, speech fix
            .Add("J", {"juliet", "japan"})
            .Add("K", {"kilo", "kentucky", "king"})
            .Add("L", {"lima", "london"})
            .Add("M", {"mike", "mexico", "mary"})
            .Add("N", {"november", "norway", "nancy"})
            .Add("O", {"oscar", "ontario", "ocean"})
            .Add("P", {"papa", "portugal", "peter"})
            .Add("Q", {"quebec", "queen"})
            .Add("R", {"romeo", "radio"})
            .Add("S", {"sierra", "santiago", "sugar"})
            .Add("T", {"tango", "tokyo", "texas"})
            .Add("U", {"uniform", "united"})
            .Add("V", {"victor", "victory", "victoria"})
            .Add("W", {"whiskey", "william", "washington"})
            .Add("X", {"x-ray"})
            .Add("Y", {"yankee", "yokohama", "yellow"})
            .Add("Z", {"zulu", "zebra"})
            .Add("0", {"zero"})
            .Add("1", {"one"})
            .Add("2", {"two"})
            .Add("3", {"three"})
            .Add("4", {"four"})
            .Add("5", {"five"})
            .Add("6", {"six"})
            .Add("7", {"seven"})
            .Add("8", {"eight"})
            .Add("9", {"nine"})
        End With
        Return dic
    End Function

    Private Function GetInstalledVoiceName(engMode As Boolean) As String
        Return New Speech.Synthesis.SpeechSynthesizer().
            GetInstalledVoices().
            FirstOrDefault(Function(item) item.VoiceInfo.Culture.Name = If(engMode, "en-US", "ru-RU")).
            VoiceInfo.Name
    End Function

    Private Sub ClearScr(title As Boolean)
        Console.ResetColor()
        Console.Clear()
        If title Then Console.WriteLine("Тренажер фонетического алфавита, R1QAE, 2018")
    End Sub
End Module

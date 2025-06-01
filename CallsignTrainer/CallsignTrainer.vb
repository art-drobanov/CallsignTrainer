Imports System.IO

'Three / Tree:
'Some non-native English speakers have trouble both pronouncing And understanding the "TH" sound.
'Tree Is better pronounced And better understood by all people, regardless Of accent.

'Five / Fife
'Much of aviation has a military history. On a poorly heard transmission "Five" can sound a lot Like "Fire",
'which Is both a military command To "shoot", And an aviation emergency! "Fife" avoids that ambiguity.

'Nine / Niner
'German Is a commonly spoken language, And "Nein"(pronounced Like "Nine" In English) Is "No" In German.
'To keep it clear that this Is a digit And Not a negative-reply, "Niner" Is distinct from the "Nine"

Module CallsignTrainer
    Sub Main()
        Dim rnd = New Random(Now.Ticks Mod Integer.MaxValue)
        Dim sp = New Speech.Synthesis.SpeechSynthesizer()
        Dim phoneticDic As Dictionary(Of Char, String())
        Dim callbook As String() = Nothing

        Dim testMode As Boolean
        Dim engMode As Boolean
        Dim talkMode As Boolean
        While True
            ClearScr(True)

            Console.WriteLine()
            Console.WriteLine("Режим:")
            Console.WriteLine("1 - обучение (RU)")
            Console.WriteLine("2 - обучение (EN)")
            Console.WriteLine("3 - самотестирование (RU)")
            Console.WriteLine("4 - самотестирование (EN)")
            Console.WriteLine("5 - озвучивание (RU)")
            Console.WriteLine("6 - озвучивание (EN)")

            Dim modeStr = Console.ReadLine().Trim()

            Select Case modeStr
                Case "1"
                    testMode = False
                    engMode = False
                    talkMode = False
                Case "2"
                    testMode = False
                    engMode = True
                    talkMode = False
                Case "3"
                    testMode = True
                    engMode = False
                    talkMode = False
                Case "4"
                    testMode = True
                    engMode = True
                    talkMode = False
                Case "5"
                    testMode = False
                    engMode = False
                    talkMode = True
                Case "6"
                    testMode = False
                    engMode = True
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
            sp.Rate = 1
            voiceName = GetInstalledVoiceName(False)
            If String.IsNullOrEmpty(voiceName) Then
                Console.WriteLine()
                Console.WriteLine("Не найден диктор 'RU'")
                Console.WriteLine("Нажмите любую клавишу для выхода из программы...")
                Console.ReadKey()
            End If
            phoneticDic = GetPhoneticDicRU()
        Else
            sp.Rate = 0
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

            Dim callsignStr = PhoneticConvert(phoneticDic, talkCallsign, rnd)
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

    Private Function PhoneticConvert(dic As Dictionary(Of Char, String()), callsignStr As String, rnd As Random,
                                     Optional extendedMode As Boolean = False) As String
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
            .Add("A", {"анна"})
            .Add("B", {"борис"})
            .Add("C", {"цапля"})
            .Add("D", {"дмитрий"})
            .Add("E", {"елена"})
            .Add("F", {"фёдор"})
            .Add("G", {"григорий"})
            .Add("H", {"харитон"})
            .Add("I", {"иван"})
            .Add("J", {"иван краткий"})
            .Add("K", {"константин"})
            .Add("L", {"леонид"})
            .Add("M", {"михаил"})
            .Add("N", {"николай"})
            .Add("O", {"ольга"})
            .Add("P", {"павел"})
            .Add("Q", {"щука"})
            .Add("R", {"роман"})
            .Add("S", {"семен"})
            .Add("T", {"татьяна"})
            .Add("U", {"ульяна"})
            .Add("V", {"женя"})
            .Add("W", {"василий"})
            .Add("X", {"мягкий знак"})
            .Add("Y", {"ееры"})
            .Add("Z", {"зинаида"})
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
            .Add("A", {"alfa"})
            .Add("B", {"bravo"})
            .Add("C", {"charlie"})
            .Add("D", {"delta"})
            .Add("E", {"echo"})
            .Add("F", {"foxtrot"})
            .Add("G", {"golf"})
            .Add("H", {"hotel"})
            .Add("I", {"india"})
            .Add("J", {"juliett"})
            .Add("K", {"kilo"})
            .Add("L", {"lima"})
            .Add("M", {"mike"})
            .Add("N", {"november"})
            .Add("O", {"oscar"})
            .Add("P", {"papa"})
            .Add("Q", {"quebec"})
            .Add("R", {"romeo"})
            .Add("S", {"sierra"})
            .Add("T", {"tango"})
            .Add("U", {"uniform"})
            .Add("V", {"victor"})
            .Add("W", {"whiskey"})
            .Add("X", {"x-ray"})
            .Add("Y", {"yankee"})
            .Add("Z", {"zulu"})
            .Add("0", {"zero"})
            .Add("1", {"one"})
            .Add("2", {"two"})
            .Add("3", {"tree"})
            .Add("4", {"foaer"})
            .Add("5", {"fife"})
            .Add("6", {"six"})
            .Add("7", {"seven"})
            .Add("8", {"eight"})
            .Add("9", {"niner"})
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
        Console.ForegroundColor = ConsoleColor.Gray
        Console.Clear()
        If title Then
            Console.WriteLine("Тренажер фонетического алфавита ICAO, 2025")
            Console.WriteLine()
            Console.WriteLine("Размер шрифта: Правой кн.мыши на заголовке окна -> Свойства -> Шрифт")
            Console.WriteLine()
        End If
    End Sub
End Module

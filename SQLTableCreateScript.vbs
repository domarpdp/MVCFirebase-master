Option Explicit
Dim WshShell
Set WshShell = WScript.CreateObject("WScript.Shell")
Dim max,min,rand
max = 200000
min = 50000
Do
  	Randomize
	rand = Int((max-min+1)*Rnd+min)
	WScript.Sleep rand
  	WshShell.SendKeys("%({TAB}{TAB}{TAB})")
Loop



'Option Explicit
'Dim WshShell
'Dim MyAppID
'Set WshShell = WScript.CreateObject("WScript.Shell")
'Do
    'WScript.Sleep 10000
    'WshShell.AppActivate("notepad")
    'WshShell.SendKeys "% r"

    'WScript.Sleep 10000
    'WshShell.AppActivate("google chrome")
    'WshShell.SendKeys "% r"

    'WScript.Sleep 10000
    'WshShell.AppActivate("Microsoft Visual Studio")
    'WshShell.SendKeys "% r"
	'

    'WScript.Sleep 10000
    'WshShell.AppActivate("Microsoft SQL Server Management Studio")
    'WshShell.SendKeys "% r"

    'WScript.Sleep 10000
    'WshShell.AppActivate("Microsoft Excel")
    'WshShell.SendKeys "% r"
'Loop

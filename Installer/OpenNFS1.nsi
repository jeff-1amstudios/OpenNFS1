!include LogicLib.nsh

; The name of the installer
Name "OpenNFS1"

; The file to write
OutFile "OpenNFS1_Install-v1.2.exe"

; The default installation directory
InstallDir $PROGRAMFILES\OpenNFS1

; Registry key to check for directory (so if you install again, it will 
; overwrite the old one automatically)
InstallDirRegKey HKLM "Software\OpenNFS1" "Install_Dir"

; Request application privileges for Windows Vista
RequestExecutionLevel admin

;--------------------------------

; Pages

Page components
Page directory
Page instfiles

UninstPage uninstConfirm
UninstPage instfiles

;--------------------------------

; The stuff to install
Section "OpenNFS1 (required)"

  SectionIn RO
    
  ; Set output path to the installation directory.
  SetOutPath $INSTDIR
  
  ; Include all files in the deploy folder
  File /r "deploy\*.*"
  File /r "3rdparty"
  File "readme.txt"
  
  Call CheckAndInstallDotNet
  Call InstallOAL
  
  ; Write the installation path into the registry
  WriteRegStr HKLM SOFTWARE\OpenNFS1 "Install_Dir" "$INSTDIR"
  
  ; Write the uninstall keys for Windows
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\OpenNFS1" "DisplayName" "OpenNFS1"
  WriteRegStr HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\OpenNFS1" "UninstallString" '"$INSTDIR\uninstall.exe"'
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\OpenNFS1" "NoModify" 1
  WriteRegDWORD HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\OpenNFS1" "NoRepair" 1
  WriteUninstaller "uninstall.exe"
  
SectionEnd

; Optional section (can be disabled by the user)
Section "Start Menu Shortcuts"

  CreateDirectory "$SMPROGRAMS\OpenNFS1"
  CreateShortcut "$SMPROGRAMS\OpenNFS1\Uninstall.lnk" "$INSTDIR\uninstall.exe" "" "$INSTDIR\uninstall.exe" 0
  CreateShortcut "$SMPROGRAMS\OpenNFS1\OpenNFS1.lnk" "$INSTDIR\OpenNFS1.exe" "" "$INSTDIR\OpenNFS1.exe" 0
  
SectionEnd

;--------------------------------

; Uninstaller

Section "Uninstall"
  
  ; Remove registry keys
  DeleteRegKey HKLM "Software\Microsoft\Windows\CurrentVersion\Uninstall\OpenNFS1"
  DeleteRegKey HKLM SOFTWARE\OpenNFS1
  
  ; Remove shortcuts
  Delete "$SMPROGRAMS\OpenNFS1\*.*"

  ; Remove directories used
  RMDir "$SMPROGRAMS\OpenNFS1"
  RMDir /R "$INSTDIR"

SectionEnd

Function CheckAndInstallDotNet
    ; Installer dotNetFx45_Full_setup.exe avalible from http://msdn.microsoft.com/en-us/library/5a4x27ek.aspx
    ; Magic numbers from http://msdn.microsoft.com/en-us/library/ee942965.aspx
    ClearErrors
    ReadRegDWORD $0 HKLM "SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full" "Release"
    IfErrors NotDetected
    ${If} $0 >= 978389
        DetailPrint "Microsoft .NET Framework 4.5 is installed ($0)"
    ${Else}
    NotDetected:
        MessageBox MB_YESNO|MB_ICONQUESTION ".NET Framework 4.5+ is required, \
            do you want to launch the web installer? This requires a valid internet connection." IDYES InstallDotNet IDNO Cancel 
        Cancel:
            MessageBox MB_ICONEXCLAMATION "To install, Microsoft's .NET Framework v4.5 \
                (or higher) must be installed. Cannot proceed with the installation!"
            # ${OpenURL} "${WWW_MS_DOTNET4_5}"
            RMDir /r "$INSTDIR" 
            SetOutPath "$PROGRAMFILES"
            RMDir "$INSTDIR" 
            Abort

        ; Install .NET4.5.
        InstallDotNet:
            DetailPrint "Installing Microsoft .NET Framework 4.5"
            SetDetailsPrint listonly
            ExecWait '"$INSTDIR\3rdparty\dotNetFx45_Full_setup.exe" /passive /norestart' $0
            ${If} $0 == 3010 
            ${OrIf} $0 == 1641
                DetailPrint "Microsoft .NET Framework 4.5 installer requested reboot."
                SetRebootFlag true 
            ${EndIf}
            SetDetailsPrint lastused
            DetailPrint "Microsoft .NET Framework 4.5 installer returned $0"
    ${EndIf}

FunctionEnd


Function InstallOAL
	ExecWait '"$INSTDIR\3rdparty\oalinst.exe" /s' $0
FunctionEnd
# RedPill

**A tools that uses multiple techniques to disable ASMI**

## Description

**A tools that uses multiple techniques to disable ASMI**. 

After loading the RedPill.dll, you will have access to four static methods, **PatchOpenSession**, **PatchScanBuffer**, **modifyContext** and **modifyInitFailed**. Each of these functions utilize a different methodology to disable AMSI  raging from modifying static fields  to patching the Amsi.dll. Below screenshot depicts how after invoking the PatchScanBuffer  method, powerup loaded successfully.

![test](https://user-images.githubusercontent.com/58237490/122685083-d178ad80-d222-11eb-9fae-cd1b90f82946.PNG)

## Usage

After loading the RedPill.dll using reflection, just access any of the 4 mentioned static method to disable AMSI

```powershell
[Reflection.Assembly]::LoadFile("C:\RedPill.dll") ##load the dll
[RedPill.DetectAndDisable]::PatchOpenSession() ## invoke the method to disable AMSI
```

## Tested On

.Net version 4.7.2

Windows 10

## Refrence 
https://pentestlaboratories.com/2021/05/17/amsi-bypass-methods/
https://www.cyberark.com/resources/threat-research-blog/amsi-bypass-patching-technique
https://github.com/S3cur3Th1sSh1t/Amsi-Bypass-Powershell
https://github.com/PowerShellMafia

<#
.SYNOPSIS
Build both x86 and x64 msi

.NOTES
In order for devenv.com to successfully build a vdproj project, this command must be run once
on the machine to configure Registry settings:

    C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\
        CommonExtensions\Microsoft\VSI\DisableOutOfProcBuild\DisableOutOfProcBuild.exe
#>

# CmdletBinding adds -Verbose functionality, SupportsShouldProcess adds -WhatIf
[CmdletBinding(SupportsShouldProcess = $true)]
param ()

Begin
{
    $script:devenv = ''
    $script:vdproj = ''

    function FindVisualStudio
    {
        $0 = 'C:\Program Files (x86)\Microsoft Visual Studio\2019'
        $script:devenv = Join-Path $0 'Enterprise\Common7\IDE\devenv.com'

        if (!(Test-Path $devenv))
        {
            $script:devenv = Join-Path $0 'Professional\Common7\IDE\devenv.com'
        }

        if (!(Test-Path $devenv))
        {
            Write-Host 'devenv not found' -ForegroundColor Yellow
            return $false
        }

        return $true
    }

    function PreserveVdproj
    {
        Write-Host '... preserving vdproj' -ForegroundColor DarkGray
        Copy-Item $vdproj .\vdproj.tmp -Force -Confirm:$false
    }

    function RestoreVdproj
    {
        Write-Host '... restoring vdproj' -ForegroundColor DarkGray
        Copy-Item .\vdproj.tmp $vdproj -Force -Confirm:$false
        Remove-Item .\vdproj.tmp -Force
    }

    function Configure ([int]$bitness)
    {
        $lines = @(Get-Content $vdproj)

        $productVersion = $lines | `
            Where-Object { $_ -match '"ProductVersion" = "8:(.+?)"' } | `
            ForEach-Object { $matches[1] }

        Write-Host
        Write-Host "... configuring vdproj for $bitness-bit build of $productVersion" -ForegroundColor Yellow

        '' | Out-File $vdproj -nonewline

        $lines | ForEach-Object `
        {
            if ($_ -match '"OutputFileName" = "')
            {
                # "OutputFilename" = "8:Debug\\OneMore_v_Setupx86.msi"
                $line = $_.Replace('OneMore_v_', "OneMore_$($productVersion)_")
                if ($bitness -eq 64)
                {
                    $line = $line.Replace('x86', 'x64')
                }
                $line | Out-File $vdproj -Append
            }
            elseif (($bitness -eq 64) -and ($_ -match '"TargetPlatform" = "'))
            {
                # x86 -> "3:0"
                # x64 -> "3:1"
                '"TargetPlatform" = "3:1"' | Out-File $vdproj -Append
            }
            else
            {
                $_ | Out-File $vdproj -Append
            }
        }
    }

    function Build ([int]$bitness)
    {
        Write-Host "... building $bitness-bit MSI" -ForegroundColor Yellow

        # output file cannot exist before build
        Remove-Item .\Debug\*.* -Force -Confirm:$false

        # build
        . $devenv .\OneMoreSetup.vdproj /build 'Debug|x86' /project Setup /projectconfig Debug

        # move msi to Downloads for safe-keeping and to allow next Platform build
        Move-Item .\Debug\*.msi $home\Downloads
        Write-Host "... $bitness-bit MSI copied to $home\Downloads\" -ForegroundColor DarkYellow
    }
}
Process
{
    Push-Location OneMoreSetup
    $script:vdproj = Resolve-Path .\OneMoreSetup.vdproj

    if (FindVisualStudio)
    {
        PreserveVdproj

        Configure 32
        Build 32

        Configure 64
        Build 64

        RestoreVdproj
    }

    Pop-Location
}

# install.ps1
# A  installer for Termix for Windows.
#
# Usage:
#   Install/Update (latest): iex (iwr "https://raw.githubusercontent.com/amrohan/termix/main/install.ps1")
#   Install specific version:  iex (iwr "https://raw.githubusercontent.com/amrohan/termix/main/install.ps1") -Tag v1.5.0
#   Uninstall:                 iex (iwr "https://raw.githubusercontent.com/amrohan/termix/main/install.ps1") -Uninstall

param(
    [string]$Tag = "latest",
    [switch]$Uninstall
)

$RepoOwner = "amrohan"
$RepoName = "termix"
$ExeName = "termix.exe"
$InstallDir = "$env:LOCALAPPDATA\Programs\termix"

function Write-Info($message) { Write-Host $message -ForegroundColor Gray }
function Write-Success($message) { Write-Host $message -ForegroundColor Green }
function Write-Error-Exit($message) { Write-Host "Error: $message" -ForegroundColor Red; exit 1 }
function Tildify($path) { return $path.Replace($HOME, "~") }

function Install-Termix {
    Write-Info "Starting Termix installation..."

    $Arch = $env:PROCESSOR_ARCHITECTURE
    if ($Arch -eq "AMD64") { $AssetPattern = "win-x64.zip" }
    elseif ($Arch -eq "ARM64") { $AssetPattern = "win-arm64.zip" }
    else { Write-Error-Exit "Unsupported architecture: $Arch" }

    $ApiUrl = "https://api.github.com/repos/$RepoOwner/$RepoName/releases"
    if ($Tag -eq "latest") { $ReleaseUrl = "$ApiUrl/latest" } else { $ReleaseUrl = "$ApiUrl/tags/$Tag" }
    
    Write-Info "Fetching release information for tag: $Tag"
    try {
        $ReleaseInfo = Invoke-RestMethod -Uri $ReleaseUrl
    } catch {
        Write-Error-Exit "Failed to fetch release info. Check the tag name and your internet connection."
    }

    $Asset = $ReleaseInfo.assets | Where-Object { $_.name -like "*$AssetPattern" } | Select-Object -First 1
    if (-not $Asset) {
        Write-Error-Exit "Could not find a release asset for your system ($AssetPattern) with tag '$Tag'."
    }

    $DownloadUrl = $Asset.browser_download_url
    $TempDir = Join-Path $env:TEMP ([System.Guid]::NewGuid().ToString())
    $ArchivePath = Join-Path $TempDir "termix.zip"
    $ExePath = Join-Path $InstallDir $ExeName
    New-Item -ItemType Directory -Path $TempDir -Force | Out-Null
    New-Item -ItemType Directory -Path $InstallDir -Force | Out-Null

    Write-Host "Downloading from $DownloadUrl"
    Invoke-WebRequest -Uri $DownloadUrl -OutFile $ArchivePath -UseBasicParsing

    Write-Info "Extracting archive..."
    Expand-Archive -Path $ArchivePath -DestinationPath $TempDir -Force
    
    $FoundExe = Get-ChildItem -Path $TempDir -Recurse -Filter $ExeName | Select-Object -First 1
    if (-not $FoundExe) {
        Write-Error-Exit "Could not find '$ExeName' in the downloaded archive."
    }
    
    Move-Item -Path $FoundExe.FullName -Destination $ExePath -Force

    Remove-Item -Path $TempDir -Recurse -Force
    Write-Success "✅ Termix was installed successfully to $(Tildify $ExePath)"

    $UserPath = [System.Environment]::GetEnvironmentVariable("Path", "User")
    if ($UserPath -notlike "*$InstallDir*") {
        $Choice = Read-Host "`nMay we add the Termix directory to your user PATH? (y/n)"
        if ($Choice -match '^[Yy]') {
            $NewPath = "$UserPath;$InstallDir"
            [System.Environment]::SetEnvironmentVariable("Path", $NewPath, "User")
            $env:Path += ";$InstallDir"
            Write-Success "✅ PATH was configured. Please restart your terminal for changes to take effect."
        } else {
            Write-Info "Skipping PATH configuration. You will need to add '$(Tildify $InstallDir)' to your PATH manually."
        }
    }
}

function Uninstall-Termix {
    Write-Info "Starting Termix uninstallation..."
    $ExePath = Join-Path $InstallDir $ExeName

    if (Test-Path $ExePath) {
        Remove-Item -Path $ExePath -Force
        Write-Success "Termix executable removed from $(Tildify $ExePath)"
    } else {
        Write-Info "Termix not found at $(Tildify $ExePath). Nothing to remove."
    }

    $UserPath = [System.Environment]::GetEnvironmentVariable("Path", "User")
    if ($UserPath -like "*$InstallDir*") {
        $Choice = Read-Host "`nFound a Termix entry in your user PATH. May we remove it? (y/n)"
        if ($Choice -match '^[Yy]') {
            $PathArray = $UserPath -split ';' | Where-Object { $_ -ne $InstallDir }
            $NewPath = $PathArray -join ';'
            [System.Environment]::SetEnvironmentVariable("Path", $NewPath, "User")
            Write-Success "Removed PATH entry. Please restart your terminal."
        }
    }
}

if ($Uninstall.IsPresent) {
    Uninstall-Termix
} else {
    Install-Termix
}

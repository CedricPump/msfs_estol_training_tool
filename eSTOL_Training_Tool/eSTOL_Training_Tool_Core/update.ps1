param (
    [string]$zipPath,
    [string]$appPath
)

Write-Host "zipPath: '$zipPath'"
Write-Host "appPath: '$appPath'"

if (-not $zipPath -or -not $appPath) {
    Write-Error "zipPath or appPath not provided"
    exit 1
}


# Extract the ZIP to the app path
Add-Type -AssemblyName System.IO.Compression.FileSystem
[System.IO.Compression.ZipFile]::ExtractToDirectory($zipPath, $appPath)

# Flatten the directory structure by copying files from the eSTOL_Training_Tool folder
$sourceDir = "$appPath\eSTOL_Training_Tool"
if (Test-Path $sourceDir) {
    # Copy all files from the eSTOL_Training_Tool directory to the working directory, overwriting existing ones
    Get-ChildItem -Path $sourceDir -Recurse | ForEach-Object {
        if ($_.PSIsContainer) {
            # Create directory if it's a folder
            $destination = Join-Path $appPath $_.Name
            New-Item -Path $destination -ItemType Directory -Force
        } else {
            # Copy files to the destination, overwriting any existing files
            Copy-Item $_.FullName -Destination $appPath -Force
        }
    }

    # Remove the now-empty eSTOL_Training_Tool directory
    Remove-Item $sourceDir -Recurse -Force
}

# Find and launch the updated EXE
$exePath = Get-ChildItem -Path $appPath -Recurse -Filter "eSTOL Training Tool.exe" | Select-Object -First 1 -ExpandProperty FullName

if (-not (Test-Path $exePath)) {
    Write-Error "Executable not found: $exePath"
    exit 1
}

Start-Sleep -Seconds 1
Start-Process "`"$exePath`""

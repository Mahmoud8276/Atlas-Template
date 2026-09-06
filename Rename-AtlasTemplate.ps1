<#
.SYNOPSIS
    Renames every project, namespace, and file reference in the Atlas template
    to a new project name, so you can spin up a new solution from the template
    without a giant manual find/replace.

.DESCRIPTION
    - Deletes bin/, obj/ and .vs/ folders (they're regenerated on build and
      just carry stale references to the old name anyway).
    - Auto-detects the template's current name prefix from the folder names
      under src/ (e.g. "Atlas.Template" from Atlas.Template.Api, Atlas.Template.Core, ...).
    - Replaces every occurrence of that prefix inside .cs, .csproj, .sln,
      .json, .http, .config and .gitignore files (namespaces, usings,
      ProjectReference paths, the .http host variable, etc).
    - Renames every file and folder that contains that prefix, including the
      .sln (fixes the "Altas.Template.sln" typo along the way, since the new
      name replaces it entirely).

.PARAMETER NewName
    The new project name, e.g. "NewProjectName". Projects become
    NewProjectName.Api, NewProjectName.Core, NewProjectName.Infrastructure, NewProjectName.Services.

.PARAMETER Path
    Path to the template's root folder (the one containing the .sln and src/).
    Defaults to the current directory.

.EXAMPLE
    .\Rename-AtlasTemplate.ps1 -NewName "NewProjectName" -Path "C:\Projects\Atlas-template"

.NOTES
    Things this script deliberately does NOT touch — handle these yourself
    afterward, they're one-line edits:
      - The root folder name itself (e.g. Atlas-template -> NewProjectName).
        Rename it in File Explorer / `mv` after the script finishes.
      - The DB name in appsettings.Development.json ("AtlasDb") and the
        email "SenderName" ("Atlas Template") — these are content, not code
        identity, so they're left for you to change on purpose.
      - Any git remote / repo name if this becomes its own repository.
    After it runs: reopen the .sln in Visual Studio and run `dotnet restore`.
#>

param(
    [Parameter(Mandatory = $true)]
    [string]$NewName,

    [Parameter(Mandatory = $false)]
    [string]$Path = "."
)

$ErrorActionPreference = "Stop"
$Root = Resolve-Path $Path

Write-Host "Working in: $Root" -ForegroundColor Cyan

# ---- 1. Clean generated folders (they just get rebuilt, and hold stale refs) ----
Write-Host "Removing bin/, obj/, .vs/ ..." -ForegroundColor Yellow
Get-ChildItem -Path $Root -Recurse -Directory -Include "bin", "obj", ".vs" -ErrorAction SilentlyContinue |
    ForEach-Object {
        Write-Host "  removing $($_.FullName)"
        Remove-Item -Recurse -Force $_.FullName -ErrorAction SilentlyContinue
    }

# ---- 2. Detect the old name prefix from the src/ project folders ----
$srcPath = Join-Path $Root "src"
if (-not (Test-Path $srcPath)) {
    throw "No 'src' folder found under $Root. Run this from the template root (the folder with the .sln in it)."
}

$projectDirs = Get-ChildItem -Path $srcPath -Directory
if ($projectDirs.Count -eq 0) {
    throw "No project folders found under $srcPath."
}

function Get-CommonPrefix {
    param([string[]]$Names)
    # Build the split arrays with an explicit foreach (not the pipeline) so
    # PowerShell doesn't flatten each name's tokens into one combined list.
    $splitArrays = New-Object System.Collections.Generic.List[string[]]
    foreach ($n in $Names) {
        $splitArrays.Add($n.Split('.'))
    }

    $minLen = ($splitArrays | ForEach-Object { $_.Length } | Measure-Object -Minimum).Minimum
    $common = @()
    for ($i = 0; $i -lt $minLen; $i++) {
        $valuesAtI = @()
        foreach ($arr in $splitArrays) { $valuesAtI += $arr[$i] }
        $unique = @($valuesAtI | Select-Object -Unique)
        if ($unique.Count -eq 1) {
            $common += $unique[0]
        } else {
            break
        }
    }
    return ($common -join '.')
}

$OldName = Get-CommonPrefix -Names $projectDirs.Name

if ([string]::IsNullOrWhiteSpace($OldName)) {
    throw "Couldn't detect a common project-name prefix under src/. Found: $($projectDirs.Name -join ', ')"
}

Write-Host "Detected old name: '$OldName' -> renaming to '$NewName'" -ForegroundColor Cyan

# ---- 3. Replace the old name inside file contents ----
$extensions = @("*.cs", "*.csproj", "*.sln", "*.json", "*.http", "*.config", "*.gitignore")

Write-Host "Updating file contents ..." -ForegroundColor Yellow
$files = Get-ChildItem -Path $Root -Recurse -File -Include $extensions
foreach ($file in $files) {
    $content = Get-Content -Path $file.FullName -Raw -ErrorAction SilentlyContinue
    if ($null -ne $content -and $content.Contains($OldName)) {
        $updated = $content.Replace($OldName, $NewName)
        Set-Content -Path $file.FullName -Value $updated -NoNewline -Encoding UTF8
        Write-Host "  updated $($file.FullName.Substring($Root.Path.Length + 1))"
    }
}

# ---- 4. Rename the .sln explicitly ----
# (The template's .sln is misspelled "Altas.Template.sln", so it won't match
# the "$OldName" pattern below by filename - only its contents do. Rename
# whatever .sln sits at the root to "$NewName.sln" regardless of its old name.)
$slnFile = Get-ChildItem -Path $Root -Filter "*.sln" -File | Select-Object -First 1
if ($slnFile) {
    $newSlnName = "$NewName.sln"
    if ($slnFile.Name -ne $newSlnName) {
        Rename-Item -Path $slnFile.FullName -NewName $newSlnName
        Write-Host "  $($slnFile.Name) -> $newSlnName"
    }
}

# ---- 5. Rename files, then folders (deepest paths first so renames stay valid) ----
Write-Host "Renaming files ..." -ForegroundColor Yellow
Get-ChildItem -Path $Root -Recurse -File | Where-Object { $_.Name -like "*$OldName*" } |
    ForEach-Object {
        $newFileName = $_.Name -replace [regex]::Escape($OldName), $NewName
        Rename-Item -Path $_.FullName -NewName $newFileName
        Write-Host "  $($_.Name) -> $newFileName"
    }

Write-Host "Renaming folders ..." -ForegroundColor Yellow  # (step 6)
Get-ChildItem -Path $Root -Recurse -Directory | Where-Object { $_.Name -like "*$OldName*" } |
    Sort-Object { $_.FullName.Length } -Descending |
    ForEach-Object {
        $newDirName = $_.Name -replace [regex]::Escape($OldName), $NewName
        Rename-Item -Path $_.FullName -NewName $newDirName
        Write-Host "  $($_.Name) -> $newDirName"
    }

Write-Host ""
Write-Host "Done renaming '$OldName' -> '$NewName'." -ForegroundColor Green
Write-Host "Still to do by hand:" -ForegroundColor Yellow
Write-Host "  1. Rename the root folder itself (e.g. Atlas-template -> $NewName)."
Write-Host "  2. If Visual Studio had the old .sln open, close it and reopen the new one."
Write-Host "  3. Change the DB name / email SenderName in appsettings if you want those to match too."
Write-Host "  4. Run 'dotnet restore' after reopening the solution."
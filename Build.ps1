param(
    [Parameter(Mandatory)] [string] $ComponentsRoot,
    [switch] $Launch,
    [string] $DataDirectory
)

$ErrorActionPreference = 'Stop'
(Get-Process -Id $PID).PriorityClass = 'BelowNormal'
$components = (Resolve-Path -LiteralPath $ComponentsRoot).Path
if (-not (Test-Path -LiteralPath (Join-Path $components 'lib\TableView\src\TableView.csproj'))) {
    throw 'ComponentsRoot must name the TinyTorrent2 checkout containing TableView.'
}
$project = Join-Path $PSScriptRoot 'lib\Settings\sample\SettingsSample.csproj'
$checks = Join-Path $PSScriptRoot 'lib\Settings\tests\SettingsChecks.csproj'
$artifacts = Join-Path $PSScriptRoot 'artifacts'
$properties = @("-p:ComponentsRoot=$components", "-p:ArtifactsPath=$artifacts", '-p:Platform=x64')

dotnet build $project @properties -c Debug --nologo -v minimal
if ($LASTEXITCODE -ne 0) { throw 'Settings sample build failed.' }
dotnet run --project $checks "-p:ArtifactsPath=$artifacts" -c Debug
if ($LASTEXITCODE -ne 0) { throw 'Settings contract checks failed.' }

if ($Launch) {
    $arguments = @('run', $project, '--no-build', '--no-restore', '--arch', 'x64',
        '-p', "ComponentsRoot=$components", '-p', "ArtifactsPath=$artifacts", '--detach', '--json')
    if ($DataDirectory) { $arguments += @('--args', ('--data-dir "' + $DataDirectory + '"')) }
    winapp @arguments
    if ($LASTEXITCODE -ne 0) { throw 'Settings sample launch failed.' }
}

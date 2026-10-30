param([Parameter(Mandatory)][int]$AppPid)
$ErrorActionPreference = 'Stop'
function Read-Control([string]$Key) {
    $text = & winapp ui get-property $Key -a $AppPid --json
    if ($LASTEXITCODE -ne 0) { throw "Cannot read $Key" }
    $text | ConvertFrom-Json
}
function Invoke-Control([string]$Key) {
    & winapp ui invoke $Key -a $AppPid
    if ($LASTEXITCODE -ne 0) { throw "Cannot invoke $Key" }
}
Invoke-Control category.transfers
if ((Read-Control copy.options).properties.ExpandCollapseState -eq 'Collapsed') {
    Invoke-Control copy.options
}
$before = Read-Control copy.options
$editor = Read-Control copy.transfers
$failed = $false
foreach ($state in @('On', 'Off')) {
    & winapp ui send-keys enter --target SettingsAdvanced --via send-input -a $AppPid
    if ($LASTEXITCODE -ne 0) { throw 'Cannot activate Advanced mode.' }
    $after = Read-Control copy.options
    $currentEditor = Read-Control copy.transfers
    $retained = $after.elementId -eq $before.elementId -and $currentEditor.elementId -eq $editor.elementId
    $expanded = $after.properties.ExpandCollapseState -eq 'Expanded'
    if ($state -eq 'On') {
        $disclosed = (Read-Control app.diagnostics).properties.IsOffscreen -eq 'False'
    } else {
        & winapp ui wait-for app.diagnostics -a $AppPid --gone -t 2000
        $disclosed = $LASTEXITCODE -eq 0
    }
    if (!$retained -or !$expanded -or !$disclosed) { $failed = $true }
    [pscustomobject]@{
        Advanced = $state
        ControlsRetained = $retained
        ExpanderOpen = $expanded
        DisclosureCorrect = $disclosed
        BeforeEditor = $editor.elementId
        AfterEditor = $currentEditor.elementId
    } | Format-List
}
if ($failed) { throw 'Advanced mode replaced existing controls or lost expansion.' }
'PASS: Advanced mode retains the existing expander and editor in both directions.'

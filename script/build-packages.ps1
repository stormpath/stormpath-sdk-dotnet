param([switch]$local = $false)

$sourceFolder = "{0}\..\src" -f $PSScriptRoot

Write-Host "Packaging projects..."
dnu pack ("{0}\Stormpath.SDK.Abstractions" -f $sourceFolder)
dnu pack ("{0}\Stormpath.SDK.Core" -f $sourceFolder)
dnu pack ("{0}\Stormpath.SDK.Http.SystemNetHttpClient" -f $sourceFolder)
dnu pack ("{0}\Stormpath.SDK.JsonNetSerializer" -f $sourceFolder)
dnu pack ("{0}\Stormpath.SDK.Redis" -f $sourceFolder)
dnu pack ("{0}\Stormpath.SDK.RestSharpClient" -f $sourceFolder)

if ($local) {
	Write-Host "Adding to local NuGet server..."

	Get-ChildItem $sourceFolder -Recurse -Filter *.nupkg | `
	ForEach-Object {
		nuget add $_.FullName -Source $env:local_nuget_path
	}
}

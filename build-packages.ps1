Write-Host "Packing nuspec files..."
$currentDir = (Resolve-Path .\).Path
$createResult = md -Force ($currentDir + "\nuget-pack")

Get-ChildItem $currentDir -Filter *.nuspec | `
Foreach-Object {
	nuget pack $_.FullName -OutputDirectory nuget-pack
}
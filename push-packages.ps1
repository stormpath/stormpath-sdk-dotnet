Write-Host "Pushing packages to MyGet..."

Get-ChildItem .\nuget-pack -Filter *.nupkg | `
Foreach-Object {
	nuget push $_.FullName -ApiKey $env:stormpath_myget_api_key -Source https://www.my get.org/F/stormpath-sdk/api/v2/package
}
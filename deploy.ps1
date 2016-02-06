Write-Host "Validating nuspec versions...";
$validVersions = 0;
$validVersions += .\scripts\validate-version.ps1 -spec Stormpath.SDK.nuspec -assemblyInfo Stormpath.SDK\Stormpath.SDK\Properties\AssemblyInfo.cs
$validVersions += .\scripts\validate-version.ps1 -spec Stormpath.SDK.Core.nuspec -assemblyInfo Stormpath.SDK\Stormpath.SDK\Properties\AssemblyInfo.cs
$validVersions += .\scripts\validate-version.ps1 -spec Stormpath.SDK.Cache.Redis.nuspec -assemblyInfo Stormpath.SDK\Stormpath.SDK.Redis\Properties\AssemblyInfo.cs
$validVersions += .\scripts\validate-version.ps1 -spec Stormpath.SDK.JsonNetSerializer.nuspec -assemblyInfo Stormpath.SDK\Stormpath.SDK.JsonNetSerializer\Properties\AssemblyInfo.cs
$validVersions += .\scripts\validate-version.ps1 -spec Stormpath.SDK.RestSharpClient.nuspec -assemblyInfo Stormpath.SDK\Stormpath.SDK.RestSharpClient\Properties\AssemblyInfo.cs

If ($validVersions -ne -0) {
  Write-Host "Error: Version mismatch in nuspec files. Aborting.";
  break;
}

Write-Host "Validating nuspec depedency versions...";
$validDependencyVersions = 0;
$validDependencyVersions += .\scripts\validate-dependency-versions.ps1

If ($validDependencyVersions -ne -0) {
  Write-Host "Error: Version mismatch in nuspec dependencies. Aborting.";
  break;
}

& .\buildz.ps1
If (($LASTEXITCODE -eq 0) -ne $true) {
  Write-Host "Error: Solution did not build. Aborting.";
  break;
}

$x = $host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown");

$exitCode = 0

Write-Host "Generating proxy .csproj files..."

# Import proxy generation functions
. ("{0}\doc-proxy-functions.ps1" -f $PSScriptRoot)

try
{
  Generate-Documentation-Proxy "docs\proxy\Stormpath.SDK.Abstractions\Stormpath.SDK.Abstractions.csproj" "src\Stormpath.SDK.Abstractions"
  Generate-AssemblyInfo "docs\proxy\Stormpath.SDK.Abstractions\Properties\AssemblyInfo.cs" "src\Stormpath.SDK.Abstractions\project.json"

  Generate-Documentation-Proxy "docs\proxy\Stormpath.SDK\Stormpath.SDK.csproj" "src\Stormpath.SDK"
  Generate-AssemblyInfo "docs\proxy\Stormpath.SDK\Properties\AssemblyInfo.cs" "src\Stormpath.SDK\project.json"
}
catch
{
  $exitCode = 1
  throw
}

Write-Host "Compiling documentation..."

& msbuild docs\StormpathSDKApiDocs.shfbproj /verbosity:minimal /nologo
$exitCode = $LASTEXITCODE

Write-Host "Cleaning up files..."

try
{
	Remove-Item "docs\api\SearchHelp.aspx" -ErrorAction Stop
	Remove-Item "docs\api\SearchHelp.inc.php" -ErrorAction Stop
	Remove-Item "docs\api\SearchHelp.php" -ErrorAction Stop
	Remove-Item "docs\api\LastBuild.log" -ErrorAction Stop
	Remove-Item "docs\api\Web.Config" -ErrorAction Stop
	(Get-Content "docs\api\index.html" -Raw -ErrorAction Stop).replace('html/Introduction.htm', '/dotnet/api/html/Introduction.htm') | Set-Content "docs\api\index.html" -ErrorAction Stop
}
catch
{
	$exitCode = 1
}

return $exitCode

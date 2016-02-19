$exitCode = 0

Write-Host "Generating proxy .csproj files..."

# Import proxy generation function
. ("{0}\generate-doc-proxy.ps1" -f $PSScriptRoot)

try
{
  Generate-Documentation-Proxy "docs\proxy\Stormpath.SDK.Abstractions\Stormpath.SDK.Abstractions.csproj" "src\Stormpath.SDK.Abstractions"
  Generate-Documentation-Proxy "docs\proxy\Stormpath.SDK\Stormpath.SDK.csproj" "src\Stormpath.SDK"
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
	(Get-Content "docs\api\index.html" -ErrorAction Stop).replace('html/Introduction.htm', '/dotnet/api/html/Introduction.htm') | Set-Content "docs\api\index.html" -ErrorAction Stop
}
catch
{
	$exitCode = 1
}

return $exitCode

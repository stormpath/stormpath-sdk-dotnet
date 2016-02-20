$exitCode = 0

$root = (Get-Item $PSScriptRoot).Parent.FullName

Write-Host "Building proxy solution..."
& msbuild "$($root)\doc\proxy\Stormpath.SDK.sln" /verbosity:quiet /nologo
$exitCode = $exitCode + $LASTEXITCODE

Write-Host "Building documentation..."

& msbuild "$($root)\doc\StormpathSDKApiDocs.shfbproj" /verbosity:minimal /nologo
$exitCode = $exitCode + $LASTEXITCODE

Write-Host "Cleaning up files..."
try
{
	Remove-Item "$($root)\doc\api\SearchHelp.aspx" -ErrorAction Stop
	Remove-Item "$($root)\doc\api\SearchHelp.inc.php" -ErrorAction Stop
	Remove-Item "$($root)\doc\api\SearchHelp.php" -ErrorAction Stop
	Remove-Item "$($root)\doc\api\LastBuild.log" -ErrorAction Stop
	Remove-Item "$($root)\doc\api\Web.Config" -ErrorAction Stop
	(Get-Content "$($root)\doc\api\index.html" -Raw -ErrorAction Stop).replace('html/Introduction.htm', '/dotnet/api/html/Introduction.htm') | Set-Content "$($root)\doc\api\index.html" -ErrorAction Stop
}
catch
{
	$exitCode = 1
}

return $exitCode

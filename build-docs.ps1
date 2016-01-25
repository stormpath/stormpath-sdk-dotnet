$exitCode = 0

Write-Host "Compiling documentation..."
& msbuild docs\StormpathSDKApiDocs.shfbproj /verbosity:minimal /nologo
$exitCode = $LASTEXITCODE

Write-Host "Cleaning up files..."
Try
{
	Remove-Item docs\api\SearchHelp.aspx -ErrorAction Stop
	Remove-Item docs\api\SearchHelp.inc.php -ErrorAction Stop
	Remove-Item docs\api\SearchHelp.php -ErrorAction Stop
	Remove-Item docs\api\LastBuild.log -ErrorAction Stop
	Remove-Item docs\api\Web.Config -ErrorAction Stop
	(Get-Content docs\api\index.html -ErrorAction Stop).replace('html/Introduction.htm', '/dotnet/api/html/Introduction.htm') | Set-Content docs\api\index.html -ErrorAction Stop
}
Catch
{
	$exitCode = 1
}

Return $exitCode
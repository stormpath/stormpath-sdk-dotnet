Write-Host "Compiling documentation..."
msbuild docs\StormpathSDKApiDocs.shfbproj /verbosity:quiet /nologo

Write-Host "Cleaning up files..."
Remove-Item docs\api\SearchHelp.aspx
Remove-Item docs\api\SearchHelp.inc.php
Remove-Item docs\api\SearchHelp.php
Remove-Item docs\api\LastBuild.log
Remove-Item docs\api\Web.Config

Write-Host "Done! Docs available at docs\api"
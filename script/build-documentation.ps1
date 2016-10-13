$exitCode = 0

# Source the file that includes Google Tag Manager functions
. ("{0}\add-gtm.ps1" -f $PSScriptRoot)

$root = (Get-Item $PSScriptRoot).Parent.FullName

Write-Host "Building proxy solution..."
& msbuild "$($root)\doc\proxy\Stormpath.SDK.sln" /verbosity:quiet /nologo
$exitCode = $exitCode + $LASTEXITCODE
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

Write-Host "Adding GTM scripts..."

$count = 0
foreach ($file in Get-ChildItem -Path "$($root)\doc\api\html\*.htm" -File) {
    InsertGTM $file 
    $count++
}

Write-Host "Added GTM script to $($count) files"

return $exitCode

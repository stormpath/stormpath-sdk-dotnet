$built = $false
$built = $true # remove!

if ($env:build_docs)
{
	Try
	{
		& .\build-docs.ps1
		$built = ($LASTEXITCODE -eq 0)
	}
	Catch
	{
		Write-Host "Documentation failed to build."
		$built = $false
	}
}

if ($built -ne $true)
{
	Write-Host "Documentation not built; skipping any remaining documentation deploy steps."
	break
}

$deploy_docs = $env:deploy_docs
$latestTag = ""
if (!$deploy_docs) {
	Write-Host "deploy_docs not set; checking for tagged release"
	
	$latestTag = git tag | tail -n 1
	if ((git log --decorate --oneline | head -n 1).contains("HEAD, tag: " + $latestTag)) {
		Write-Host "Tagged release $($latestTag) found"
		$deploy_docs = 1
	}
}

if ($built -and $deploy_docs) {
	Write-Host "Deploying documentation!"
	
	git clone git@github.com:stormpath/stormpath.github.io.git --branch source
	cd stormpath.github.io
	git config user.email "evangelists@stormpath.com"
	git config user.name "sdk-dotnet Auto Doc Build"
	
	$branchName = "dotnet-autodeploy-$($latestTag)"
	git checkout -b $branchName
	
	Copy-Item ..\docs\api source\dotnet\ -recurse -force
	git add --all
	git commit -m "sdk-dotnet release $($latestTag)"
	git push -u origin $branchName
}

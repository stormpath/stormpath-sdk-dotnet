$built = $true

if (!$env:build_docs)
{
	Write-Host "build_docs not set, skipping doc build."
}
else
{
	Write-Host "build_docs set, building documentation..."

	Try
	{
		& "$($PSScriptRoot)\build-docs.ps1"
		$built = ($LASTEXITCODE -eq 0)
	}
	Catch
	{
		Write-Host $_
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
$latestTag = git tag | tail -n 1
if (!$deploy_docs) {
	Write-Host "deploy_docs not set; checking for tagged release"

	if ((git log --decorate --oneline | head -n 1).contains("HEAD, tag: " + $latestTag)) {
		Write-Host "Tagged release $($latestTag) found"
		$deploy_docs = 1
	}
	else
	{
		Write-Host "Not a tagged release, will not deploy documentation."
	}
}

if ($built -and $deploy_docs) {
	Write-Host "Deploying documentation!"

	Write-Host "Cloning stormpath/stormpath.github.io"
	git clone -q git@github.com:stormpath/stormpath.github.io.git --branch source 2> $null
	cd stormpath.github.io
	git config user.email "evangelists@stormpath.com"
	git config user.name "sdk-dotnet Auto Doc Build"

	$branchName = "dotnet-autodeploy-$($latestTag)"
	Write-Host "Creating branch $($branchName)"
	git checkout -b $branchName 2> $null

	Write-Host "Copying new files and pushing to repository"
	Copy-Item "$($PSScriptRoot)\..\doc\api source\dotnet\" -recurse -force
	git add --all
	git commit -m "sdk-dotnet release $($latestTag)"
	git push -u origin $branchName
}

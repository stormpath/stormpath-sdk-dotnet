$currentBranch = git rev-parse --abbrev-ref HEAD
if ($currentBranch -ne "develop") {
	Write-Host "Not on develop branch; will not deploy documentation."
	return;
}

$deploy_docs = $env:deploy_docs
$latestTag = git tag | select -last 1
if (!$deploy_docs) {
	Write-Host "deploy_docs not set; checking for tagged release"

	if ((git log --decorate --oneline | select -first 1).contains("HEAD, tag: " + $latestTag)) {
		Write-Host "Tagged release $($latestTag) found"
		$deploy_docs = 1
	}
	else
	{
		Write-Host "Not a tagged release, will not deploy documentation."
	}
}

if ($deploy_docs) {
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

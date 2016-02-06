param (
    [string]$spec = $(throw "-spec is required."),
    [string]$assemblyInfo = $(throw "-assemblyInfo is required.")
)


function Find-Version([string]$pattern, [string]$contents)
{
    $definition = [regex]$pattern;
    $result = $definition.Match($contents);
    
    if (!$result.Success) {
        throw "Could not find version in input string.";
    }

    return $result.Groups[1].ToString();
}


$specContents = (Get-Content $spec -ErrorAction Stop);
$assemblyInfoContents = (Get-Content $assemblyInfo -ErrorAction Stop);

$expectedVersion = Find-Version "<version>(.*?)<\/version>" $specContents;

$foundAssemblyVersion = Find-Version "AssemblyVersion\(""(.*?)""\)]" $assemblyInfoContents
$foundAssemblyFileVersion = Find-Version "AssemblyFileVersion\(""(.*?)""\)]" $assemblyInfoContents

if ($expectedVersion -ne $foundAssemblyVersion) {
    Write-Error "Assembly version ($foundAssemblyVersion) does not match nuspec version ($expectedVersion)";
    return 1;
}

if ($expectedVersion -ne $foundAssemblyFileVersion) {
    Write-Error "Assembly file version ($foundAssemblyFileVersion) does not match nuspec version ($expectedVersion)";
    return 1;
}

return 0; # All good!
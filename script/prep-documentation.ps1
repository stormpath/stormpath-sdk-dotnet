$exitCode = 0

Write-Host "Generating proxy .csproj files..."

# Import proxy generation functions
. ("{0}\doc-proxy-functions.ps1" -f $PSScriptRoot)

$root = (Get-Item $PSScriptRoot).Parent.FullName

try
{
  Generate-Documentation-Proxy "$($root)\doc\proxy\Stormpath.SDK.Abstractions\Stormpath.SDK.Abstractions.csproj" "$($root)\src\Stormpath.SDK.Abstractions"
  Generate-AssemblyInfo "$($root)\doc\proxy\Stormpath.SDK.Abstractions\Properties\AssemblyInfo.cs" "$($root)\src\Stormpath.SDK.Abstractions\project.json"

  Generate-Documentation-Proxy "$($root)\doc\proxy\Stormpath.SDK\Stormpath.SDK.csproj" "$($root)\src\Stormpath.SDK.Core"
  Generate-AssemblyInfo "$($root)\doc\proxy\Stormpath.SDK\Properties\AssemblyInfo.cs" "$($root)\src\Stormpath.SDK.Core\project.json"
}
catch
{
  $exitCode = 1
  throw
}

Write-Host "Documentation is ready to compile!"

return $exitCode

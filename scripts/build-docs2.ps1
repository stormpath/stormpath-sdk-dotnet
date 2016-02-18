. ("{0}\generate-doc-proxy.ps1" -f $PSScriptRoot)

Generate-Documentation-Proxy "docs\proxy\Stormpath.SDK.Abstractions\Stormpath.SDK.Abstractions.csproj" "src\Stormpath.SDK.Abstractions"
Generate-Documentation-Proxy "docs\proxy\Stormpath.SDK\Stormpath.SDK.csproj" "src\Stormpath.SDK"

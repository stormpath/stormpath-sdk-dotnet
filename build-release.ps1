Write-Host "Compiling solution in Release mode..."
msbuild Stormpath.SDK\Stormpath.SDK.sln /t:Rebuild /p:Configuration=Release /verbosity:quiet
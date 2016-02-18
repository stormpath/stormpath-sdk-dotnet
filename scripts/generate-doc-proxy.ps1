Function Generate-Documentation-Proxy
{
    param(
        [Parameter(Mandatory = $true)]
        [string]$csproj,

        [Parameter(Mandatory = $true)]
        [string]$sourceDirectory
    )

    # Finds all the .cs files in a directory (recursively) and generates <Compile Include="" /> statements
    Function Find-Files-Recursively
    {
        param ([string]$directory, [System.Text.StringBuilder]$builder, [string]$indent)

        $count = 0

        Get-ChildItem $directory -Recurse -Filter *.cs -ErrorAction Stop | `
        ForEach-Object {
            $includeStatement = "{0}<Compile Include=""{1}"" />" -f $indent, ($_ | Resolve-Path -Relative)

            [void]$builder.AppendLine($includeStatement)
            $count++
        }

        Write-Host ("Added {0} files" -f $count)
    }

    try
    {
        $savedLocation = (Get-Location)
        $fileInfo = (Get-Item $csproj -ErrorAction Stop)
        $sourceDirectoryInfo = (Get-Item $sourceDirectory -ErrorAction Stop)

        # Relative paths will be generated relative to the .csproj file's directory
        Set-Location $fileInfo.Directory.FullName -ErrorAction Stop

        $fileReader = New-Object System.IO.StreamReader -Arg $fileInfo.FullName -ErrorAction Stop
        $output = New-Object System.Text.StringBuilder
        $inAutogenBlock = $false
        $generatedOutput = $false

        $startTag = "<!-- #startAutogen -->"
        $endTag = "<!-- #endAutogen -->"

        while ($line = $fileReader.ReadLine()) {
            if ($inAutogenBlock -and $line.Trim() -eq $endTag) {
                $inAutogenBlock = $false
            }

            if (!$inAutogenBlock) {
                [void]$output.AppendLine($line)
            }
            else
            {
                if (!$generatedOutput)
                {

                    $indent = $line.Substring(0, $line.Length - $line.TrimStart().Length)
                    [void]$output.AppendLine("{0}<ItemGroup>" -f $indent)

                    # Recursively generate include elements
                    Find-Files-Recursively $sourceDirectoryInfo.FullName $output ("{0}{0}" -f $indent)

                    [void]$output.AppendLine("{0}{0}<Compile Include=""Properties\AssemblyInfo.cs"" />" -f $indent)
                    [void]$output.AppendLine("{0}</ItemGroup>" -f $indent)

                    $generatedOutput = $true
                }
            }

            if (!$inAutogenBlock -and $line.Trim() -eq $startTag) {
                $inAutogenBlock = $true
            }
        }
    }
    finally
    {
        $fileReader.Dispose()
    }

    Set-Location $savedLocation

    Set-Content -Path ($fileInfo.FullName) -Value $output.ToString() -ErrorAction Stop
}

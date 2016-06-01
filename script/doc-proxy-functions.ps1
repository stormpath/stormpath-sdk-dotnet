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
          if (!$_.FullName.Contains("\obj\"))
          {
            $includeStatement = "{0}<Compile Include=""{1}"" />" -f $indent, ($_ | Resolve-Path -Relative)

            [void]$builder.AppendLine($includeStatement)
            $count++
          }
        }

        Write-Host ("Added {0} files" -f $count)
    }

    $generatedOutput = $false

    Write-Output ("Generating proxy {0} for project folder {1}" -f $csproj, $sourceDirectory)

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

    if ($generatedOutput) {
      Set-Content -Path ($fileInfo.FullName) -Value $output.ToString() -ErrorAction Stop
    }
    else {
      throw "Did not add any output!"
    }
}

Function Generate-AssemblyInfo
{
  param(
      [Parameter(Mandatory = $true)]
      [string]$assemblyInfoFile,

      [Parameter(Mandatory = $true)]
      [string]$projectJsonFile
  )

  Write-Host ("Patching {0} with data from {1}" -f $assemblyInfoFile, $projectJsonFile)

  # Get info from project.json
  $json = Get-Content $projectJsonFile -Raw -ErrorAction Stop | ConvertFrom-Json

  # Generate the AssemblyInfo bare essentials
  $output = New-Object System.Text.StringBuilder
  [void]$output.AppendLine("using System.Reflection;")
  [void]$output.AppendLine("");

  [void]$output.AppendLine("[assembly: AssemblyTitle(""{0}"")]" -f $json.title)
  [void]$output.AppendLine("[assembly: AssemblyDescription(""{0}"")]" -f $json.description)
  [void]$output.AppendLine("[assembly: AssemblyCompany(""{0}"")]" -f $json.packOptions.owners[0])
  [void]$output.AppendLine("[assembly: AssemblyCopyright(""{0}"")]" -f $json.copyright)

  # Sanitize version (which may be like "1.0.0-beta2")
  $safeVersion = $json.version
  if ($safeVersion.Contains("-")) {
    $safeVersion = $safeVersion.Substring(0, $safeVersion.IndexOf("-"))
  }

  [void]$output.AppendLine("[assembly: AssemblyVersion(""{0}"")]" -f $safeVersion)
  [void]$output.AppendLine("[assembly: AssemblyFileVersion(""{0}"")]" -f $safeVersion)

  Set-Content -Path (Get-Item $assemblyInfoFile).FullName -Value $output.ToString() -ErrorAction Stop
}

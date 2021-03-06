# Disable the progress bar because it slows down Invoke-WebRequest
$progressPreference = 'silentlyContinue'

# Download NuGet.exe
if (-not (Test-Path .\NuGet.exe))
{
    Invoke-WebRequest https://nuget.org/nuget.exe -OutFile NuGet.exe
}

# Restore NuGet packages
&.\NuGet.exe restore ..\Source\LogSpect.sln

# Build solution
$msbuildPath = Join-Path $env:windir Microsoft.NET\Framework\v4.0.30319\MSBuild.exe
&$msbuildPath ..\Source\LogSpect.sln /t:Rebuild /p:Configuration=Release

# Prepare output directory
if (Test-Path .\Packages)
{
    Remove-Item .\Packages\*
}
else
{
    New-Item .\Packages -ItemType Directory
}

# Create packages
&.\NuGet.exe pack .\LogSpect.nuspec -OutputDirectory Packages
&.\NuGet.exe pack .\LogSpect.Core.nuspec -OutputDirectory Packages
&.\NuGet.exe pack .\LogSpect.BasicLoggers.nuspec -OutputDirectory Packages
&.\NuGet.exe pack .\LogSpect.NLog.nuspec -OutputDirectory Packages
&.\NuGet.exe pack .\LogSpect.Log4Net.nuspec -OutputDirectory Packages
&.\NuGet.exe restore ..\Source\LogSpect.sln
&"c:\Program Files (x86)\MSBuild\12.0\Bin\amd64\MSBuild.exe" ..\Source\LogSpect.sln /t:Rebuild /p:Configuration=Release

if (Test-Path .\Packages)
{
    Remove-Item .\Packages\*
}
else
{
    New-Item .\Packages -ItemType Directory
}

&.\NuGet.exe pack .\LogSpect\LogSpect.nuspec -OutputDirectory Packages
&.\NuGet.exe pack .\LogSpect.Core\LogSpect.Core.nuspec -OutputDirectory Packages
&.\NuGet.exe pack .\LogSpect.BasicLoggers\LogSpect.BasicLoggers.nuspec -OutputDirectory Packages
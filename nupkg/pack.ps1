$Env:DOTNET_SYSTEM_NET_HTTP_USESOCKETSHTTPHANDLER = 0
$ErrorActionPreference = "Stop"

# Paths
$packFolder = (Get-Item -Path "./" -Verbose).FullName
$slnPath = Join-Path $packFolder "../"
$srcPath = Join-Path $slnPath "src"
# List of projects
$projects = (
"FclEx",
"FclEx.Http",
"FclEx.Image"
)

Remove-Item *.nupkg
Set-Location $slnPath
# Copy all nuget packages to the pack folder
foreach($project in $projects) {    
    $projectFolder = Join-Path $srcPath $project
    # Create nuget pack
    Set-Location $projectFolder
    & dotnet pack -c Release --include-symbols -v m --output $packFolder
	if ($Lastexitcode -ne 0)	{
		throw "failed with exit code $LastExitCode"
	}
}

# Go back to the pack folder
Set-Location $packFolder

$PSGallerySourceUri = 'https://www.myget.org/F/huoshan12345/api/v2/package'
$APIKey = 'fbc0486a-55ff-4760-b246-bef3e0ee952d'
& dotnet nuget push *.nupkg -k $APIKey -s $PSGallerySourceUri --timeout 30
if ($Lastexitcode -ne 0)	{
	throw "failed with exit code $LastExitCode"
}

Write-Output "Finished. Press any key to exit."
Read-Host
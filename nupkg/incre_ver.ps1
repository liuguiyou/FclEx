# Paths
$packFolder = (Get-Item -Path "./" -Verbose).FullName
$slnPath = Join-Path $packFolder "../"
$srcPath = Join-Path $slnPath "src"

# List of projects
$projects = (
"FclEx",
"FclEx.Image",
"FclEx.Http"
)

Set-Location $slnPath
foreach($project in $projects) {
    
    $projectFolder = Join-Path $srcPath $project	
	$outputDir = New-Object System.IO.DirectoryInfo -ArgumentList @($projectFolder)
	$l = $outputDir.GetFiles("*.csproj", [System.IO.SearchOption]::TopDirectoryOnly);
	foreach ($i in $l){
		$path = $i.FullName
		(Get-Content -path $path) | % { $n = [regex]::match($_,'(?<=<Version>\d+\.\d+\.)\d+(?=</Version>)').value; if ($n) {$_ -replace "$n", ([int32]$n+1)} else {$_}; } | Set-Content $path
	}
}

# Go back to the pack folder
Set-Location $packFolder

Write-Output "Finished. Press any key to exit."
Read-Host
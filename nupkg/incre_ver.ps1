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
	$reg =  [regex]"(?<=<Version>)(\d+)\.(\d+)\.(\d+)(?=</Version>)";
	$callback = {
		$m = $args[0]
		$major = [int32]$m.Groups[1].Value
		$minor = [int32]$m.Groups[2].Value
		$build = [int32]$m.Groups[3].Value
		If ($build -gt 10) { $minor++; $build = 0} Else { $build++ }
		"$major.$minor.$build"
	}
	foreach ($i in $l){
		$path = $i.FullName
        (Get-Content -Path $path) | % { $reg.Replace($_, $callback) } | Set-Content $path
	}
}

# Go back to the pack folder
Set-Location $packFolder

Write-Output "Finished. Press any key to exit."
Read-Host
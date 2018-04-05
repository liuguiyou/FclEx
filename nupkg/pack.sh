#!/bin/bash
packFolder=$PWD
slnPath="$packFolder/.."
srcPath="$slnPath/src"

projects=("FclEx" "FclEx.Image" "FclEx.Json" "FclEx.Logger")

rm -f *.nupkg
cd $slnPath

dotnet restore
dotnet clean -c Release
dotnet msbuild /t:Rebuild /p:Configuration=Release

echo "project count: ${#projects[@]}"
for project in "${projects[@]}"; do 
	echo "packing for [$project]"
	projectFolder="$srcPath/$project"
    cd $projectFolder	
	outputPath="$projectFolder/bin/Release"
	rm -rf $outputPath
	projectPackPath="$outputPath/$project.*.nupkg"
	dotnet msbuild /t:pack /p:Configuration=Release /p:IncludeSymbols=true
	cp $projectPackPath $packFolder
done

cd $packFolder

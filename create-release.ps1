$ErrorActionPreference = "Stop"

Function check-result {
    if ($LastExitCode -ne 0) { Invoke-BuildError "ERROR: Exiting with error code $LastExitCode"	}
    return $true
}

Function Invoke-BuildError {
Param(
	[parameter(Mandatory=$true)][string] $text
)
	if ($env:TEAMCITY_VERSION) {
        Write-Error "##teamcity[message text='$text']" # so number of failed tests shows in builds list instead of this text
		Write-Error "##teamcity[buildStatus status='ERROR']"
		[System.Environment]::Exit(1) 
	} else {
        Write-Error $text 
        exit
	}
}

Function Invoke-Exe {
    Param(
        [parameter(Mandatory = $true)][string] $cmd,
        [parameter(Mandatory = $true)][string] $args
	
    )
    Write-Output "Executing: `"$cmd`" --% $args"
    & "$cmd" "--%" "$args";
    $result = check-result
    if (!$result) {
        throw "ERROR executing EXE"
        exit 1
    }
}

Function Get-Version {
	if (Test-Path 'appveyor.yml') {
		Get-Content appveyor.yml | ForEach-Object { 
			# version: 1.0.{build}
			if ($_ -like 'version: *') {
				$version = $_ -replace 'version: ', ''
				$version = $version -replace '.{build}', ''
				return $version
			} else {
				#echo $_ 
			}
		}	
	} else {
		$a = Get-Content './src/version.json' -raw | ConvertFrom-Json
		return $a.version
	}
}

Function Update-Version {
	if (Test-Path 'appveyor.yml') {
		$contents = Get-Content appveyor.yml 
		$contents | ForEach-Object {
			# version: 1.0.{build}
			if ($_ -like 'version: *') {
				$version = $_ -replace 'version: ', ''
				$version = $version -replace '.{build}', ''

				$v = [version] $version
				$versionStringIncremented =  [string] [version]::new(
				  $v.Major,
				  $v.Minor+1
				)
				"version: $($versionStringIncremented).{build}"
			} else {
				$_ 
			}
		} | Set-Content appveyor.yml	
		
		git commit -m "update version" appveyor.yml
	} else {
		$a = Get-Content './src/version.json' -raw | ConvertFrom-Json
		$version = [version] $a.version
		$versionStringIncremented =  [string] [version]::new(
		  $version.Major,
		  $version.Minor+1
		)
		$a.version = $versionStringIncremented

		$a | ConvertTo-Json -depth 32| set-content './src/version.json'
		
		git commit -m "update version" ./src/version.json
	}
}

Invoke-Exe -cmd git -args "checkout master"
Invoke-Exe -cmd git -args "pull"
Invoke-Exe -cmd git -args "checkout develop"
Invoke-Exe -cmd git -args "pull"
Invoke-Exe -cmd git -args "merge master"
Invoke-Exe -cmd git -args "push"

$version = Get-Version
$branch = "release/$($version)"
echo $branch

$exists = (git ls-remote origin $branch)
if ($exists.Length -eq 0) {
	git checkout -b $branch
	$releaseNotes = ./generate-changelog.ps1
	git add CHANGELOG.md
	git commit -m "generate changelog"
	#git push
	git push --set-upstream origin $branch
	
	$remote = (git remote -v)
	if ($remote -contains "github.com") {
		gh pr create --title "Release $version" --body "$releaseNotes" --base master
	} else {
		echo "should create the pr here -- everything passed - $branch"	
		echo $body	
	}

	git checkout develop
	git merge $branch	
	Update-Version
	git push
} else {
	echo "release branch already exists"
}
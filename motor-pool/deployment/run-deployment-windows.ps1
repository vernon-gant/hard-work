param(
    [string]$ServerIP,
    [string]$ServerPassword,
    [string]$EnvFilePath,
    [string]$CygwinHomeDir,
    [string]$CygwinBatPath  # Path to the Cygwin.bat file
)

# Validate input parameters
if (-not $ServerIP -or -not $ServerPassword -or -not $EnvFilePath -or -not $CygwinHomeDir -or -not $CygwinBatPath) {
    Write-Host "Usage: run-deployment.ps1 -ServerIP 'server_ip' -ServerPassword 'server_password' -EnvFilePath 'path_to_env_file' -CygwinHomeDir 'cygwin_home_directory' -CygwinBatPath 'path_to_cygwin_bat'"
    exit
}

# Define the paths for script and environment file within Cygwin
$LocalScriptPath = "deployment.sh"
$CygwinScriptPath = "$CygwinHomeDir/deployment.sh"
$CygwinEnvFilePath = "$CygwinHomeDir/.env"

# Copy the environment file and script to the Cygwin home directory
Copy-Item -Path $EnvFilePath -Destination $CygwinEnvFilePath -Force
Copy-Item -Path $LocalScriptPath -Destination $CygwinScriptPath -Force

# Prepare the command to run in Cygwin
$ScriptCommand = "./deployment.sh $ServerIP $ServerPassword ./.env"

# Output the command to the user
Write-Host "Open Cygwin and run the following command:"
Write-Host $ScriptCommand

# Start Cygwin but do not run the script
cmd /c start "" "$CygwinBatPath"
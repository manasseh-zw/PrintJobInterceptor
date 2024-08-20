# Requires administrator privileges
#Requires -RunAsAdministrator

$serviceName = "Sybrin Printer Service"
$displayName = "Sybrin Printer Service"
$description = "Listens for print jobs and processes them"
$exePath = Join-Path $PSScriptRoot "PrintJobInterceptor.exe"

# Check if the service already exists
$service = Get-Service -Name $serviceName -ErrorAction SilentlyContinue

if ($service -eq $null) {
    Write-Host "Installing $serviceName..."
    
    # Create the service
    New-Service -Name $serviceName -BinaryPathName $exePath -DisplayName $displayName -StartupType Automatic -Description $description

    # Start the service
    Start-Service -Name $serviceName

    Write-Host "$serviceName has been installed and started."
} else {
    Write-Host "$serviceName is already installed."
}

# Ensure the service is set to start automatically
Set-Service -Name $serviceName -StartupType Automatic

# Check the status of the service
$status = (Get-Service -Name $serviceName).Status
Write-Host "Service status: $status"

if ($status -ne "Running") {
    Write-Host "Starting $serviceName..."
    Start-Service -Name $serviceName
    $status = (Get-Service -Name $serviceName).Status
    Write-Host "Service status: $status"
}

Write-Host "Installation and configuration complete."
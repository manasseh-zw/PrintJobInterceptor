# Requires administrator privileges
#Requires -RunAsAdministrator

$printerIP = "127.0.0.1"
$printerPort = "9999"
$printerPortName = "IP_127.0.0.9999"
$printerName = "PostScript Printer"
$driverName = "Microsoft PS Class Driver"

# Function to check if the printer already exists
function Test-PrinterExists {
    param ($Name)
    return [bool](Get-Printer -Name $Name -ErrorAction SilentlyContinue)
}

# Function to check if the port already exists
function Test-PortExists {
    param ($Name)
    return [bool](Get-PrinterPort -Name $Name -ErrorAction SilentlyContinue)
}

# Check if the printer already exists
if (Test-PrinterExists $printerName) {
    Write-Host "Printer '$printerName' already exists."
}
else {
    # Check if the port exists, if not, create it
    if (-not (Test-PortExists $printerPortName)) {
        Write-Host "Creating printer port..."
        Add-PrinterPort -Name $printerPortName -PrinterHostAddress $printerIP -PortNumber $printerPort
    }
    else {
        Write-Host "Printer port '$printerPortName' already exists."
    }

    # Check if the driver is available
    $driverAvailable = Get-PrinterDriver -Name $driverName -ErrorAction SilentlyContinue

    if (-not $driverAvailable) {
        Write-Host "Error: Required printer driver '$driverName' is not installed on the system." -ForegroundColor Red
        exit 1
    }

    # Add the printer
    Write-Host "Adding printer..."
    Add-Printer -Name $printerName -DriverName $driverName -PortName $printerPortName

    Write-Host "Printer '$printerName' has been successfully added."
}

# Set the printer as default (optional)
# Set-PrintConfiguration -PrinterName $printerName -PaperSize A4

Write-Host "Printer setup complete."
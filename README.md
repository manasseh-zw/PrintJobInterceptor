# Virtual PostScript Printer

## Description

This project implements a virtual PostScript printer using C#. It creates a TCP listener that acts as a network printer, intercepts print jobs, saves them as PostScript files, and converts them to PDF format using Ghostscript.NET.

## Features

- Acts as a virtual network printer on localhost:8888
- Intercepts print jobs and saves raw PostScript data
- Converts PostScript files to PDF using Ghostscript.NET
- Extracts metadata from print jobs (title, author, filename)
- Logs server activities and individual print job details

## Prerequisites

- Windows operating system
- .NET 6.0 or later
- Ghostscript installed on the system (required for Ghostscript.NET)

## Setup

### 1. Install Ghostscript

Download and install Ghostscript from the official website: https://www.ghostscript.com/releases/gsdnld.html

### 2. Install Required NuGet Packages

Open the project in your code editor and install the following NuGet package:

- Ghostscript.NET

### 3. Configure the Virtual Printer in Windows

- Run addprinter.ps1 powershell script as Admin

- Manual installation

1.  Go to 'Printers & scanners' in Windows Settings
2.  Click 'Add a printer or scanner' (and wait a sec)
3.  Choose 'The printer that I want isn't listed'
4.  Select 'Add a printer using a an IP address or hostname'
5.  Enter the following details:
    - Device type: TCP/IP Device
    - Hostname or IP address: 127.0.0.1
    - Port name: 8888
6.  Click 'Next'

- (windows will try to detect a printer on this port config ... you will get an error, ignore it and keep the setting to General Network Card and click next)\*

7. For the driver, select 'Microsoft PS Class Driver'
8. Choose a printer name (e.g., "Virtual PostScript Printer")
9. Complete the wizard

## Usage

1. Run the application. It will start listening for print jobs on localhost:8888.

2. Send a print job to the virtual printer you set up.

3. The application will:

   - Save the raw PostScript file
   - Convert the PostScript to PDF
   - Extract and log metadata
   - Save all files and logs in the 'PrintJobs' directory (by default)

4. Check the 'PrintJobs' directory for:
   - Original PostScript files (.ps)
   - Converted PDF files (.pdf)
   - Job log (job_log.txt)
   - Server log (server_log.txt)

## Installation (Run Powershell as Admin)

- Use the powershell installer script to install the service.
- Make sure it is the same directory as the compiled exe of the service

## Customization

- To change the output directory, modify the `PrintServer` constructor call in `Program.cs`.
- To adjust PDF conversion settings, modify the `ConvertPsToPdf` method in `PrintServer.cs`.

## Troubleshooting

- Ensure Ghostscript is properly installed and its bin directory is in your system PATH.
- Check that no other application is using port 8888.
- Review the server_log.txt for any error messages or issues.

## Acknowledgments

- This project uses Ghostscript.NET, which is based on Ghostscript.

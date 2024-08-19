using System.Net;
using System.Net.Sockets;
using Ghostscript.NET.Processor;

public class PrintServer
{
    private TcpListener _listener;
    private const string _host = "127.0.0.1";
    private const int _port = 8888;
    private const bool _KeepAlive = true;
    private const string _outputDirectory = "PrintJobs";

    public PrintServer()
    {
        _listener = new TcpListener(IPAddress.Parse(_host), _port);
        Directory.CreateDirectory(_outputDirectory);
    }

    public async Task Start()
    {
        try
        {
            _listener.Start();
            Logger.Log($"Server started. Listening on {_host}:{_port}....");

            while (_KeepAlive)
            {
                Logger.Log("Waiting for print jobs...");
                using TcpClient client = await _listener.AcceptTcpClientAsync();
                using NetworkStream stream = client.GetStream();
                Logger.Log("Incoming print job received");
                await ProcessPrintJob(stream);
            }
        }
        catch (System.Exception)
        {

            throw;
        }
    }

    private async Task ProcessPrintJob(NetworkStream stream)
    {
        string jobId = Guid.NewGuid().ToString("N");
        string psFilePath = Path.Combine(_outputDirectory, $"{jobId}.ps");
        string pdfFilePath = Path.Combine(_outputDirectory, $"{jobId}.pdf");

        try
        {
            using (var fs = new FileStream(psFilePath, FileMode.Create, FileAccess.Write))
            {
                await stream.CopyToAsync(fs);
            }

            Logger.Log($"Print job data saved to {psFilePath}");

            string postScriptData = File.ReadAllText(psFilePath);
            var metadata = ExtractMetadata(postScriptData);

            ConvertPsToPdf(psFilePath, pdfFilePath);

            Logger.Log($"PDF created: {pdfFilePath}");

            Logger.LogJobDetails(jobId, metadata, psFilePath, pdfFilePath);
        }
        catch (Exception ex)
        {
            Logger.Log($"Error processing job {jobId}: {ex.Message}");
        }
    }

    private static void ConvertPsToPdf(string psFilePath, string pdfFilePath)
    {
        using GhostscriptProcessor processor = new();
        var switches = new List<string>
            {
                "-dNOPAUSE", "-dBATCH", "-dSAFER",
                "-sDEVICE=pdfwrite",
                $"-sOutputFile={pdfFilePath}",
                "-dPDFSETTINGS=/prepress",
                "-dCompatibilityLevel=1.4",
                psFilePath
            };

        processor.StartProcessing(switches.ToArray(), null);
    }

    private PSMetadata ExtractMetadata(string postScriptData)
    {
        var metadata = new PSMetadata();
        var lines = postScriptData.Split('\n');
        foreach (var line in lines)
        {
            if (line.StartsWith("%%Title:")) metadata.Title = line.Substring(8).Trim();
            else if (line.StartsWith("%%Creator:")) metadata.Author = line.Substring(10).Trim();
            else if (line.StartsWith("%%DocumentName:")) metadata.FileName = line.Substring(15).Trim();
            else if (line.StartsWith("%%CreationDate:")) metadata.CreationDate = line.Substring(15).Trim();
            else if (line.StartsWith("%%Creator:")) metadata.Software = line.Substring(10).Trim();
            else if (line.StartsWith("%%DocumentID:")) metadata.DocumentID = line.Substring(13).Trim();
            else if (line.StartsWith("%%Pages:")) metadata.PageCount = line.Substring(8).Trim();
            else if (line.StartsWith("%%Language:")) metadata.Language = line.Substring(11).Trim();
            else if (line.StartsWith("%%BoundingBox:")) metadata.BoundingBox = line.Substring(14).Trim();
            else if (line.StartsWith("%%Orientation:")) metadata.Orientation = line.Substring(14).Trim();

            // Break if we've found all metadata (you might want to adjust this condition)
            if (!string.IsNullOrEmpty(metadata.Author) &&
                !string.IsNullOrEmpty(metadata.Title) &&
                !string.IsNullOrEmpty(metadata.FileName) &&
                !string.IsNullOrEmpty(metadata.CreationDate) &&
                !string.IsNullOrEmpty(metadata.Software) &&
                !string.IsNullOrEmpty(metadata.DocumentID) &&
                !string.IsNullOrEmpty(metadata.PageCount) &&
                !string.IsNullOrEmpty(metadata.Language) &&
                !string.IsNullOrEmpty(metadata.BoundingBox) &&
                !string.IsNullOrEmpty(metadata.Orientation))
            {
                break;
            }
        }

        return metadata;
    }

}

public class PSMetadata
{
    public string Author { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string CreationDate { get; set; } = string.Empty;
    public string Software { get; set; } = string.Empty;
    public string DocumentID { get; set; } = string.Empty;
    public string PageCount { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string BoundingBox { get; set; } = string.Empty;
    public string Orientation { get; set; } = string.Empty;
}


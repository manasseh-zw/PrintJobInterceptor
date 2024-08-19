using System;
using System.IO;
using Spectre.Console;

public static class Logger
{
    private const string _logsOutputDirectory = "PrintJobs";

    static Logger()
    {
        Directory.CreateDirectory(_logsOutputDirectory);
    }

    public static void LogJobDetails(string jobId, PSMetadata metadata, string psPath, string pdfPath)
    {
        var table = new Table()
            .Border(TableBorder.Rounded)
            .AddColumn("Property")
            .AddColumn("Value");

        table.AddRow("Job ID", jobId);
        table.AddRow("Title", metadata.Title);
        table.AddRow("Author", metadata.Author);
        table.AddRow("File Name", metadata.FileName);
        table.AddRow("Creation Date", metadata.CreationDate);
        table.AddRow("Software", metadata.Software);
        table.AddRow("Document ID", metadata.DocumentID);
        table.AddRow("Page Count", metadata.PageCount);
        table.AddRow("Language", metadata.Language);
        table.AddRow("Bounding Box", metadata.BoundingBox);
        table.AddRow("Orientation", metadata.Orientation);
        table.AddRow("PS File", psPath);
        table.AddRow("PDF File", pdfPath);
        table.AddRow("Timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

        var panel = new Panel(table)
            .Header("Print Job Details")
            .Expand()
            .BorderColor(Color.Blue);

        AnsiConsole.Write(panel);

        string logEntry = table.ToString();
        File.AppendAllText(Path.Combine(_logsOutputDirectory, "job_log.txt"), logEntry + Environment.NewLine);
    }

    public static void Log(string message, LogLevel level = LogLevel.Info)
    {
        Color color = level switch
        {
            LogLevel.Info => Color.Blue,
            LogLevel.Warning => Color.Yellow,
            LogLevel.Error => Color.Red,
            _ => Color.White
        };

        var logMessage = new Markup($"[{color}][[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]] {level}: {message}[/]");
        AnsiConsole.Write(logMessage);
        AnsiConsole.WriteLine();

        // Log to file
        File.AppendAllText(Path.Combine(_logsOutputDirectory, "server_log.txt"), logMessage.ToString() + Environment.NewLine);
    }

    public enum LogLevel
    {
        Info,
        Warning,
        Error
    }
}
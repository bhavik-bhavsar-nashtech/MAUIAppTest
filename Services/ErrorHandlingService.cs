using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using Microsoft.Extensions.Logging;

namespace MAUIAppTest.Services;

public class ErrorHandlingService
{
    private readonly ILogger<ErrorHandlingService> _logger;
    private readonly string _logFilePath;

    public ErrorHandlingService(ILogger<ErrorHandlingService> logger)
    {
        _logger = logger;
        var logDir = Path.Combine(FileSystem.AppDataDirectory, "logs");
        if (!Directory.Exists(logDir))
            Directory.CreateDirectory(logDir);

        _logFilePath = Path.Combine(logDir, $"app_log_{DateTime.UtcNow:yyyyMMdd}.txt");
    }

    public async Task LogExceptionAsync(Exception ex, string? context = null)
    {
        try
        {
            // Log to ILogger
            if (string.IsNullOrWhiteSpace(context))
                _logger.LogError(ex, "Unhandled exception");
            else
                _logger.LogError(ex, "Unhandled exception: {Context}", context);

            // Also write to a local file for later inspection
            var text = $"[{DateTime.UtcNow:O}] {context ?? ""} {ex}\n";
            await File.AppendAllTextAsync(_logFilePath, text);
        }
        catch
        {
            // Swallow all errors from the logging system to avoid recursive failures
            try
            {
                Debug.WriteLine(ex.ToString());
            }
            catch { }
        }
    }

    public async Task LogMessageAsync(string message)
    {
        try
        {
            _logger.LogInformation(message);
            var text = $"[{DateTime.UtcNow:O}] {message}\n";
            await File.AppendAllTextAsync(_logFilePath, text);
        }
        catch { }
    }

    // Helper to quickly test logging from UI/pages without throwing
    public async Task TestLogAsync(string message)
    {
        await LogMessageAsync(message);
    }

    // Helper to test exception logging path. Caller may observe the thrown exception.
    public async Task TestExceptionAsync()
    {
        try
        {
            throw new InvalidOperationException("Test exception from ErrorHandlingService.TestExceptionAsync");
        }
        catch (Exception ex)
        {
            await LogExceptionAsync(ex, "TestExceptionAsync");
            throw;
        }
    }
}

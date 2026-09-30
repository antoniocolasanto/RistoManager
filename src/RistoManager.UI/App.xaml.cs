using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RistoManager.Application;
using RistoManager.Core;
using RistoManager.Infrastructure;
using RistoManager.UI.ViewModels;
using RistoManager.UI.Views;
using Serilog;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace RistoManager.UI;

public partial class App : System.Windows.Application
{
    private readonly IHost _host;

    public App()
    {
        var programData = Environment.GetFolderPath(
            Environment.SpecialFolder.CommonApplicationData);

        var logDirectory = Path.Combine(
            programData,
            "RistoManager",
            "Logs");

        Directory.CreateDirectory(logDirectory);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.File(
                Path.Combine(logDirectory, "log-.txt"),
                rollingInterval: RollingInterval.Day)
            .CreateLogger();

        DispatcherUnhandledException += App_DispatcherUnhandledException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;

        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(configuration =>
            {
                configuration.SetBasePath(AppContext.BaseDirectory);
                configuration.AddJsonFile(
                    "appsettings.json",
                    optional: false,
                    reloadOnChange: true);
            })
            .UseSerilog()
            .ConfigureServices((context, services) =>
            {
                services.AddCore();
                services.AddApplication();
                services.AddInfrastructure();

                services.AddSingleton<MainWindow>();
                services.AddSingleton<MainViewModel>();
            })
            .Build();
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            Log.Information("Avvio di RistoManager in corso...");

            await _host.StartAsync();

            var mainWindow =
                _host.Services.GetRequiredService<MainWindow>();

            var mainViewModel =
                _host.Services.GetRequiredService<MainViewModel>();

            mainWindow.DataContext = mainViewModel;

            mainWindow.Show();
        }
        catch (Exception ex)
        {
            Log.Fatal(
                ex,
                "Errore fatale durante l'avvio dell'applicazione.");

            MessageBox.Show(
                "Impossibile avviare RistoManager. Controllare i log tecnici.",
                "Errore Critico",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            Current.Shutdown();
        }
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        Log.Information("Chiusura di RistoManager in corso...");

        await _host.StopAsync(TimeSpan.FromSeconds(5));

        _host.Dispose();

        Log.CloseAndFlush();

        base.OnExit(e);
    }

    private void App_DispatcherUnhandledException(
        object sender,
        System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
    {
        Log.Error(
            e.Exception,
            "Eccezione non gestita sul thread UI (Dispatcher).");

        MessageBox.Show(
            "Si è verificato un errore imprevisto nell'interfaccia. L'errore è stato registrato.",
            "Errore",
            MessageBoxButton.OK,
            MessageBoxImage.Warning);

        e.Handled = true;
    }

    private void CurrentDomain_UnhandledException(
        object sender,
        UnhandledExceptionEventArgs e)
    {
        var ex = e.ExceptionObject as Exception;

        Log.Fatal(
            ex,
            "Eccezione non gestita nell'AppDomain.");

        MessageBox.Show(
            "Si è verificato un errore critico imprevisto. L'applicazione verrà chiusa.",
            "Errore Fatale",
            MessageBoxButton.OK,
            MessageBoxImage.Error);
    }

    private void TaskScheduler_UnobservedTaskException(
        object? sender,
        UnobservedTaskExceptionEventArgs e)
    {
        Log.Error(
            e.Exception,
            "Eccezione non osservata in un Task asincrono.");

        e.SetObserved();
    }
}
using CommunityToolkit.Mvvm.ComponentModel;
using Serilog;

namespace RistoManager.UI.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _welcomeMessage = "RistoManager - Gestione professionale del ristorante";

    [ObservableProperty]
    private string _statusMessage = "Applicazione avviata correttamente.";

    public MainViewModel()
    {
        Log.Information("MainViewModel inizializzato.");
    }
}
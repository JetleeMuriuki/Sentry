using System.Windows;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace Sentry
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Collection to hold mapped process data
        public ObservableCollection<ProcessInfo> RunningProcesses { get; set; }

        private DispatcherTimer _processTimer;
        private HashSet<int> _previousProcessIds = new();

        public MainWindow()
        {
            InitializeComponent();

            RunningProcesses = new ObservableCollection<ProcessInfo>();
            ProcessGrid.ItemsSource = RunningProcesses;

            LoadProcesses();

            _previousProcessIds =
                Process.GetProcesses()
                    .Select(p => p.Id)
                    .ToHashSet();

            _processTimer = new DispatcherTimer();
            _processTimer.Interval = TimeSpan.FromSeconds(2);
            _processTimer.Tick += ProcessTimer_Tick;
            _processTimer.Start();
        }

        private void ProcessTimer_Tick(object? sender, EventArgs e)
        {
            HashSet<int> currentProcessIds =
                Process.GetProcesses()
                       .Select(p => p.Id)
                       .ToHashSet();

            var newProcesses = currentProcessIds.Except(_previousProcessIds);

            var terminatedProcesses = _previousProcessIds.Except(currentProcessIds);

            foreach (var pid in newProcesses)
            {
                try
                {
                    Process process = Process.GetProcessById(pid);

                    Debug.WriteLine(
                        $"NEW PROCESS: {process.ProcessName} ({pid})");
                }
                catch
                {
                    //skip inaccessible processes
                }
            }

            foreach (var pid in terminatedProcesses)
            {
                Debug.WriteLine(
                    $"TERMINATED PROCESS: PID {pid}");
            }

            _previousProcessIds = currentProcessIds;

            LoadProcesses();
        }

        private void LoadProcesses()
        {
            RunningProcesses.Clear(); //to avoid duplicating processes

            foreach (Process process in Process.GetProcesses())
            {
                try
                {
                    RunningProcesses.Add(new ProcessInfo
                    {
                        Id = process.Id,
                        Name = process.ProcessName,
                        Memory = $"{process.WorkingSet64 / 1024 / 1024} MB"
                    });
                }
                catch
                {
                    // Skip inaccessible processes
                }
            }
        }

        // Lightweight model class for binding
        public class ProcessInfo
        {
            public int Id { get; set; }
            public required string Name { get; set; }
            public string Memory { get; set; } = "";
        }
    }
}
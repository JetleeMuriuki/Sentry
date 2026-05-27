using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace Sentry
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Collection to hold our mapped process data
        public ObservableCollection<ProcessInfo> RunningProcesses { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            LoadProcesses();

            // Set DataContext and bind the DataGrid
            ProcessGrid.ItemsSource = RunningProcesses;
        }

        private void LoadProcesses()
        {
            RunningProcesses = new ObservableCollection<ProcessInfo>();

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
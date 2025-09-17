using System.Diagnostics;
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

namespace ProzesuakKudeatu
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Process prozesua;
        public MainWindow()
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            InitializeComponent();
        }

        private void prozesuaHasi(object sender, RoutedEventArgs e)
        {
            prozesua = Process.Start("ftp.exe");
        }

        private void prozesuaAmaitu(object sender, RoutedEventArgs e)
        {
            String izena = prozesua.ProcessName;
            int id = prozesua.Id;
            prozesua.Kill();
            label.Content = "Prozesua amitu da " + id + ":" + izena;
        }

        private void prozesuakErakutsi(object sender, RoutedEventArgs e)
        {
            combo.ItemsSource = Process.GetProcesses();
            combo.DisplayMemberPath = "ProcessName";
        }

    }
}
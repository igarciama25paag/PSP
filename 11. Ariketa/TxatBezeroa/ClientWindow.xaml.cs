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

namespace TxatBezeroa
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly object ErroreLock = new();
        private readonly object TxatLock = new();
        private readonly object BidaliLock = new();

        private Client Client;

        public MainWindow()
        {
            InitializeComponent();
            Closing += MainWindow_Closing;
            BezeroaSortu();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Client?.BezeroaItxi();
        }

        private void Konektatu(object sender, RoutedEventArgs e)
        {
            Client.Konektatu(ip.Text.Trim(), izena.Text.Trim());
        }

        private void Deskonektatu(object sender, RoutedEventArgs e)
        {
            Client.BezeroaItxi();
        }

        private void Bidali(object sender, RoutedEventArgs e)
        {
            lock (BidaliLock)
            {
                if (mezua.Text != string.Empty)
                    Client.MezuaBidali(mezua.Text.Trim());
            }
            mezua.Text = string.Empty;
        }

        private void BezeroaSortu()
        {
            Client = new Client
            {
                MessageArrivedEvent = mezua =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        lock (TxatLock)
                        {
                            txat.Items.Add(new ListBoxItem { Content = mezua });
                        }
                    });
                },

                LogSentEvent = log =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        lock (ErroreLock)
                        {
                            erroreMezua.Text = log;
                        }
                    });
                }
            };
        }
    }
}
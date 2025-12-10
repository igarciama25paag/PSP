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
            Client?.BezeroaItxi(null);
        }

        private void Konektatu(object sender, RoutedEventArgs e)
        {
            if (izena.Text != string.Empty)
                Client.Konektatu(ip.Text.Trim(), izena.Text.Trim());
            else
            {
                erroreMezua.Text = "Izena ezin da utsik utzi";
                izena.Focus();
            }
        }

        private void Deskonektatu(object sender, RoutedEventArgs e)
        {
            Client.BezeroaItxi("konexioa itxi da");
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
                            var item = new ListBoxItem { Content = mezua };
                            txat.Items.Add(item);
                            txat.ScrollIntoView(item);
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
                },

                ConnectedEvent = () =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        txat.Items.Clear();
                        konektatuButton.IsEnabled = false;
                        deskonektatuButton.IsEnabled = true;
                        txatBox.IsEnabled = true;
                    });
                },

                DisconnectedEvent = () =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        konektatuButton.IsEnabled = true;
                        deskonektatuButton.IsEnabled = false;
                        txatBox.IsEnabled = false;
                    });
                }
            };
        }
    }
}
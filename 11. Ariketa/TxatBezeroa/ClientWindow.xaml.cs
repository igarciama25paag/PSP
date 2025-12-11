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

        private void KonektatuClick(object sender, RoutedEventArgs e) => Konektatu();

        private void KonektatuEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Konektatu();
                mezua.Focus();
            }
        }

        private void Konektatu()
        {
            if (izena.Text != string.Empty)
                Client.Konektatu(ip.Text.Trim(), izena.Text.Trim());
            else
            {
                LogBerria("Izena ezin da utsik utzi", false);
                izena.Focus();
            }
        }

        private void DeskonektatuClick(object sender, RoutedEventArgs e) => Client.BezeroaItxi("Konexioa itxi da");

        private void BidaliClick(object sender, RoutedEventArgs e) => Bidali();

        private void BidaliEnter(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) Bidali();
        }

        private void Bidali()
        {
            lock (BidaliLock)
            {
                if (mezua.Text != string.Empty)
                    Client.MezuaBidali(mezua.Text.Trim());
            }
            mezua.Text = string.Empty;
        }

        private void LogBerria(string log, bool good)
        {
            lock (TxatLock)
            {
                var item = new ListBoxItem { Content = log };
                if (good) item.Foreground = Brushes.Green;
                else item.Foreground = Brushes.Red;
                txat.Items.Add(item);
                txat.ScrollIntoView(item);
            }
        }

        private void MezuBerria(string mezua)
        {
            lock (TxatLock)
            {
                var item = new ListBoxItem { Content = mezua };
                txat.Items.Add(item);
                txat.ScrollIntoView(item);
            }
        }

        private void BezeroaSortu()
        {
            Client = new Client
            {
                MessageArrivedEvent = mezua => Dispatcher.Invoke(() => MezuBerria(mezua)),

                LogSentEvent = (log, good) => Dispatcher.Invoke(() => LogBerria(log, good)),

                ConnectedEvent = () =>
                    Dispatcher.Invoke(() =>
                    {
                        konektatuButton.IsEnabled = false;
                        deskonektatuButton.IsEnabled = true;
                        txatBox.IsEnabled = true;
                    }),

                DisconnectedEvent = () =>
                    Dispatcher.Invoke(() =>
                    {
                        konektatuButton.IsEnabled = true;
                        deskonektatuButton.IsEnabled = false;
                        txatBox.IsEnabled = false;
                    })
            };
        }
    }
}
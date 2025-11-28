using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
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

namespace TxatAurreratua
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ServerWindow : Window
    {
        private readonly object LogLock = new();
        private readonly object TxatLock = new();

        public ObservableCollection<ServersideClient> Bezeroak { get; } = [];
        private Server Server;

        public ServerWindow()
        {
            DataContext = this;
            InitializeComponent();
            ZerbitzariaSortu();
            Closing += MainWindow_Closing;
        }

        private void Piztu(object sender, RoutedEventArgs e)
        {
            piztuButton.IsEnabled = false;
            itzaliButton.IsEnabled = true;
            Server.Piztu();
        }

        private void Itzali(object sender, RoutedEventArgs e)
        {
            piztuButton.IsEnabled = true;
            itzaliButton.IsEnabled = false;
            Server.Itzali();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Server.Itzali();
        }

        private void ZerbitzariaSortu()
        {
            Server = new Server
            {
                LogSentEvent = log =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            lock (LogLock)
                            {
                                logs.Items.Add(new ListBoxItem { Content = log });
                            }
                        });
                    },

                MessageSentEvent = mezua =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            lock (TxatLock)
                            {
                                txat.Items.Add(new ListBoxItem { Content = mezua });
                            }
                        });
                    },

                ClientConnectedEvent = bezero =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            Bezeroak.Add(bezero);
                        });
                    },

                ClientDisconnectedEvent = bezero =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        Bezeroak.Remove(bezero);
                    });
                }
            };
        }
    }
}
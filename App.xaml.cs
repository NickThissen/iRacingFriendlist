using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using iRacingFriendlist.Classes;
using iRacingFriendlist.Views;

namespace iRacingFriendlist
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public MainWindow MainWindow { get; set; }

        private ObservableCollection<OnlineStatusUpdate> _updateHistory;
        public ObservableCollection<OnlineStatusUpdate> UpdateHistory { get { return _updateHistory; } }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _updateHistory = new ObservableCollection<OnlineStatusUpdate>();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            iRacingFriendlist.Properties.Settings.Default.Save();
            base.OnExit(e);
        }
    }
}

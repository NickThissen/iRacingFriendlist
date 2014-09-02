using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Threading;
using iRacingFriendlist.Classes;
using iRacingFriendlist.Http;
using iRacingFriendlist.ViewModels;
using Application = System.Windows.Application;
using ContextMenu = System.Windows.Forms.ContextMenu;
using MenuItem = System.Windows.Forms.MenuItem;
using MessageBox = System.Windows.MessageBox;
using Point = System.Windows.Point;

namespace iRacingFriendlist.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string ICON_NORMAL = @"pack://application:,,,/iRacingFriendlist;component/Resources/icon_256.ico";
        private const string ICON_MUTED = @"pack://application:,,,/iRacingFriendlist;component/Resources/icon_muted.ico";

        private Properties.Settings settings;

        private int selectedDriverId;
        private bool isUpdating;

        private DriverCollection friends; 
        private ICollectionView view;
        private DispatcherTimer timer;

        private Dictionary<int, Note> notes; 

        private NotifyIcon notifyIcon;
        private MenuItem muteMenuItem;
        private bool closedFromTray;
        private bool muted;

        bool _shown;
        private bool initialized;
        private Driver _selectedDriver;

        public MainWindow()
        {
            InitializeComponent();

            settings = Properties.Settings.Default;

            this.SetupTrayIcon();
            
            friends = new DriverCollection();
            view = CollectionViewSource.GetDefaultView(friends);
            view.Filter = FilterList;
            this.SetupSorting();

            lst.ItemsSource = view;
        }

        protected override void OnContentRendered(EventArgs e)
        {
            base.OnContentRendered(e);

            if (_shown)
                return;
            _shown = true;

            timer = new DispatcherTimer(TimeSpan.FromSeconds(Util.CapUpdateFreq(settings.UpdateFreq)), DispatcherPriority.Background, TimerTicked, this.Dispatcher);
            timer.Stop();
            StartLoadingListings();
        }

        private void SetupTrayIcon()
        {
            notifyIcon = new NotifyIcon();
            using (var iconStream = Application.GetResourceStream(new Uri(ICON_NORMAL)).Stream)
            {
                notifyIcon.Icon = new Icon(iconStream);
            }
            notifyIcon.MouseDoubleClick += notifyIcon_MouseDoubleClick;
            notifyIcon.Visible = true;

            var menu = new ContextMenu();
            muteMenuItem = new MenuItem("Mute", OnMuteClicked);

            menu.MenuItems.Add(muteMenuItem);
            menu.MenuItems.Add("-");
            menu.MenuItems.Add("History", OnHistoryClicked);
            menu.MenuItems.Add("Settings", OnSettingsClicked);
            menu.MenuItems.Add("-");
            menu.MenuItems.Add("Close", OnCloseClicked);
            notifyIcon.ContextMenu = menu;
        }

        private void OnCloseClicked(object sender, EventArgs eventArgs)
        {
            closedFromTray = true;
            this.Close();
        }

        private void OnHistoryClicked(object sender, EventArgs eventArgs)
        {
            var window = new HistoryWindow();
            window.Show();
        }

        private void OnMuteClicked(object sender, EventArgs eventArgs)
        {
            if (muted)
            {
                using (var iconStream = Application.GetResourceStream(new Uri(ICON_NORMAL)).Stream)
                {
                    notifyIcon.Icon = new Icon(iconStream);
                }
                muteMenuItem.Text = "Mute";
                muted = false;
            }
            else
            {
                using (var iconStream = Application.GetResourceStream(new Uri(ICON_MUTED)).Stream)
                {
                    notifyIcon.Icon = new Icon(iconStream);
                }
                muteMenuItem.Text = "Unmute";
                muted = true;
            }
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.WindowState = WindowState.Normal;
            this.Show();
            this.ShowInTaskbar = true;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            if (!closedFromTray)
            {
                this.ShowInTaskbar = false;
                if (!settings.HasNotifiedTray)
                {
                    settings.HasNotifiedTray = true;
                    settings.Save();

                    notifyIcon.BalloonTipTitle = "iRacing Friend List";
                    notifyIcon.BalloonTipText = "iRacing Friend List has minimized to your system tray!";
                    notifyIcon.ShowBalloonTip(2000);
                }

                this.Hide();
                this.ShowInTaskbar = false;
                e.Cancel = true;
            }
            else
            {
                Note.Save(notes);

                notifyIcon.Visible = false;
                notifyIcon.Dispose();
            }
        }

        private void StartLoadingListings()
        {
            var thread = new Thread(LoadListingsThread);
            thread.Start();
        }

        private void LoadListingsThread()
        {
            Global.GetContentListings();
            this.Dispatcher.Invoke(new Action(FinishedListings));
        }

        private void FinishedListings()
        {
            // Load friendnotes
            notes = Note.Load();

            // Load friendlist
            this.StartLoading();
        }

        private bool FilterList(object o)
        {
            var driver = (Driver)o;

            if (!string.IsNullOrWhiteSpace(txtFilter.Text))
            {
                return FilterName(driver);
            }
            else
            {
                return FilterStatus(driver);
            }
        }

        private bool FilterName(Driver driver)
        {
            return (driver.Name.ToLower().Contains(txtFilter.Text.ToLower().Trim()));
        }

        private bool FilterStatus(Driver driver)
        {
            if (settings.ShowOffline) return true;
            return driver.OnlineStatus != Driver.OnlineStatuses.Offline;
        }

        private void TimerTicked(object sender, EventArgs e)
        {
            this.StartLoading();
        }

        private void StartLoading()
        {
            lblLoading.Text = "Updating...";
            timer.Stop();

            isUpdating = true;

            // Start loading in a background thread
            var t = new Thread(Loader);
            t.Start();
        }

        private void Loader()
        {
            try
            {
                string json = Global.Connection.GetFriends();
                var update = FriendlistParser.ParseJson(json);

                // Finished - invoke FinishedLoading on the UI thread
                Dispatcher.Invoke(new Action<DriverCollection>(FinishedLoading), update);
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was an error loading the friends list:\n\n" + ex.Message);
                Logging.Instance.LogError("Loading friends list.", ex);
            }

        }

        private void FinishedLoading(DriverCollection update)
        {
            lblLoading.Text = "Last updated: " + DateTime.Now.ToString("HH:mm:ss");

            if (initialized)
            {
                var statusUpdates = this.CheckUpdates(friends, update);
                this.ShowStatusUpdates(statusUpdates);
            }
            
            Note.Save(notes);

            friends.Clear();
            foreach (var friend in update)
            {
                if (friend.Helmet != null) friend.HelmetImage = friend.Helmet.DownloadImage();
                if (notes.ContainsKey(friend.CustId)) friend.NoteText = notes[friend.CustId].Text;
                friends.Add(friend);
            }

            if (friends.Contains(selectedDriverId))
            {
                lst.SelectedItem = friends.FromId(selectedDriverId);
            }

            timer.Start();
            initialized = true;
            isUpdating = false;
        }

        private IEnumerable<OnlineStatusUpdate> CheckUpdates(DriverCollection current, DriverCollection update)
        {
            var currentSession = current.InSession();
            var currentOnline = current.Online();
            var currentOffline = current.Offline();

            var updateSession = update.InSession();
            var updateOnline = update.Online();
            var updateOffline = update.Offline();

            var statusUpdates = new List<OnlineStatusUpdate>();

            var notify = (Driver.NotificationOptions) settings.NotifyOn;

            foreach (var driver in update)
            {
                var status = new OnlineStatusUpdate(driver);
                if (driver.IsInSession) status.SetSession(driver.Session);

                if (currentOffline.Contains(driver))
                {
                    // Driver was OFFLINE...

                    status.From = Driver.OnlineStatuses.Offline;
                    if (updateOnline.Contains(driver))
                    {
                        //.. and is now ONLINE
                        if (!notify.HasFlag(Driver.NotificationOptions.OfflineOnline)) continue;
                        status.To = Driver.OnlineStatuses.Online;
                    }
                    else if (updateSession.Contains(driver))
                    {
                        //.. and is now IN SESSION
                        if (!notify.HasFlag(Driver.NotificationOptions.OfflineSession)) continue;
                        status.To = Driver.OnlineStatuses.InSession;
                    }
                }
                else if (currentOnline.Contains(driver))
                {
                    // Driver was ONLINE..
                    status.From = Driver.OnlineStatuses.Online;
                    if (updateOffline.Contains(driver))
                    {
                        //.. and is now OFFLINE
                        if (!notify.HasFlag(Driver.NotificationOptions.OnlineOffline)) continue;
                        status.To = Driver.OnlineStatuses.Offline;

                    }
                    else if (updateSession.Contains(driver))
                    {
                        //.. and is now IN SESSION
                        if (!notify.HasFlag(Driver.NotificationOptions.OnlineSession)) continue;
                        status.To = Driver.OnlineStatuses.InSession;
                    }
                }
                else if (currentSession.Contains(driver))
                {
                    // Driver was IN SESSION...
                    status.From = Driver.OnlineStatuses.InSession;
                    if (updateOnline.Contains(driver))
                    {
                        //.. and is now NO LONGER IN SESSION but still ONLINE
                        if (!notify.HasFlag(Driver.NotificationOptions.SessionOnline)) continue;
                        status.To = Driver.OnlineStatuses.Online;
                    }
                    else if (updateOffline.Contains(driver))
                    {
                        //.. and is now OFFLINE
                        if (!notify.HasFlag(Driver.NotificationOptions.SessionOffline)) continue;
                        status.To = Driver.OnlineStatuses.Offline;
                    }
                }
                
                if (status.IsUpdate && statusUpdates.All(s => s.Driver.CustId != driver.CustId)) statusUpdates.Add(status);
            }

            foreach (var statusUpdate in statusUpdates)
            {
                ((App) Application.Current).UpdateHistory.Add(statusUpdate);
            }

            return statusUpdates;
        }

        private void ShowStatusUpdates(IEnumerable<OnlineStatusUpdate> updates)
        {
            if (muted) return;

            int count = 0;
            foreach (var update in updates)
            {
                var window = new NotificationWindow(update, count);
                window.Show();

                count++;
            }
        }

        private void txtFilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            view.Refresh();
        }

        private void lst_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (isUpdating) return;

            var driver = lst.SelectedItem as Driver;
            if (driver == null)
                selectedDriverId = 0;
            else
                selectedDriverId = driver.CustId;
        }

        private void OnSettingsClicked(object sender, EventArgs e)
        {
            var window = new SettingsWindow();
            if (window.ShowDialog().GetValueOrDefault())
            {
                settings = Properties.Settings.Default;

                // Refresh..
                timer.Interval = TimeSpan.FromSeconds(Util.CapUpdateFreq(settings.UpdateFreq));

                this.SetupSorting();
                this.StartLoading();
            }
        }
        
        private void SetupSorting()
        {
            view.SortDescriptions.Clear();
            if (settings.GroupStatus)
            {
                view.SortDescriptions.Add(new SortDescription("IsInSession", ListSortDirection.Descending));
                view.SortDescriptions.Add(new SortDescription("IsOnline", ListSortDirection.Descending));
            }

            if (settings.OrderBy == 0)
                view.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            else
            {
                view.SortDescriptions.Add(new SortDescription("LastActivity", ListSortDirection.Descending));
                view.SortDescriptions.Add(new SortDescription("LastOnline", ListSortDirection.Descending));
            }
        }

        private void lst_MouseRightButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Point clickPoint = e.GetPosition(lst);
            object element = lst.InputHitTest(clickPoint);
            if (element != null)
            {
                var item = GetVisualParent<ListBoxItem>(element);
                if (item != null)
                {
                    notePopup.IsOpen = true;
                    notePopup.DataContext = item.DataContext;
                }
            }
        }

        private void popupText_TextChanged(object sender, TextChangedEventArgs e)
        {
            var driver = notePopup.DataContext as Driver;
            if (driver == null) return;
            
            if (string.IsNullOrWhiteSpace(popupText.Text))
            {
                notes.Remove(driver.CustId);
            }
            else
            {
                if (!notes.ContainsKey(driver.CustId))
                    notes.Add(driver.CustId, new Note(driver.CustId, popupText.Text));
                else
                    notes[driver.CustId].Text = popupText.Text;
            }
        }

        public T GetVisualParent<T>(object childObject) where T : Visual
        {
            DependencyObject child = childObject as DependencyObject;
            while ((child != null) && !(child is T))
            {
                child = VisualTreeHelper.GetParent(child);
            }
            return child as T;
        }

    }
}

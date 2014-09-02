using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using iRacingFriendlist.Classes;
using iRacingFriendlist.ViewModels;
using Application = System.Windows.Application;

namespace iRacingFriendlist.Views
{
    /// <summary>
    /// Interaction logic for NotificationWindow.xaml
    /// </summary>
    public partial class NotificationWindow : Window
    {
        private int _count;
        private Properties.Settings settings;

        public NotificationWindow(OnlineStatusUpdate update, int count = 0)
        {
            InitializeComponent();

            settings = Properties.Settings.Default;
            this.Duration = TimeSpan.FromSeconds(settings.PopupDuration);

            this.DataContext = update.Driver;

            _count = count;
            lblNewStatus.Text = GetNewStatus(update);

            this.Loaded += NotificationWindow_Loaded;
        }

        public TimeSpan Duration
        {
            get; set;
        }

        private void NotificationWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.SetLocation();
            this.StartAnimation();
        }

        private void StartAnimation()
        {
            var startAnim = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(0.5));
            var midAnim = new DoubleAnimation(1, 1, Duration);
            var stopAnim = new DoubleAnimation(1, 0, TimeSpan.FromSeconds(1));
            
            startAnim.Completed += (s, e) => grid.BeginAnimation(OpacityProperty, midAnim);
            midAnim.Completed += (s, e) => grid.BeginAnimation(OpacityProperty, stopAnim);
            stopAnim.Completed += (s, e) => this.Close();

            grid.BeginAnimation(OpacityProperty, startAnim);
        }

        private void SetLocation()
        {
            var screen = Screen.PrimaryScreen;
            var screens = Screen.AllScreens;
            if (settings.PopupScreen >= 0 && settings.PopupScreen < screens.Length)
            {
                screen = screens[settings.PopupScreen];
            }

            var workingArea = screen.WorkingArea;
            var transform = PresentationSource.FromVisual(this).CompositionTarget.TransformFromDevice;
            var location = GetLocation(transform, workingArea);

            this.Left = location.X;
            this.Top = location.Y;
        }

        private Point GetLocation(Matrix transform, System.Drawing.Rectangle workingArea)
        {
            Point corner;
            switch ((Util.PopupLocations)settings.PopupLocation)
            {
                case Util.PopupLocations.BottomLeft:
                    corner = transform.Transform(new Point(workingArea.Left, workingArea.Bottom));
                    return new Point(corner.X, corner.Y - this.ActualHeight * (_count + 1) + _count);
                case Util.PopupLocations.BottomRight:
                    corner = transform.Transform(new Point(workingArea.Right, workingArea.Bottom));
                    return new Point(corner.X - this.ActualWidth, corner.Y - this.ActualHeight * (_count + 1) + _count);
                case Util.PopupLocations.TopLeft:
                    corner = transform.Transform(new Point(workingArea.Left, workingArea.Top));
                    return new Point(corner.X, corner.Y + this.ActualHeight * _count + _count);
                default:
                    corner = transform.Transform(new Point(workingArea.Right, workingArea.Top));
                    return new Point(corner.X - this.ActualWidth, corner.Y + this.ActualHeight * _count + _count);
            }
        }

        private string GetNewStatus(OnlineStatusUpdate update)
        {
            switch (update.To)
            {
                case Driver.OnlineStatuses.InSession:
                    return string.Format("is now IN {0} SESSION", update.Driver.Session.EventType.ToString().ToUpper());
                case Driver.OnlineStatuses.Online:
                    return "is now ONLINE";
                case Driver.OnlineStatuses.Offline:
                    return "is now OFFLINE";
            }
            return "";
        }

        private void Grid_MouseClick(object sender, MouseButtonEventArgs e)
        {
            ((App) Application.Current).MainWindow.Show();
        }
    }
}

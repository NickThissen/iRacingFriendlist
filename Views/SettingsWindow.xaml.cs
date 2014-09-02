using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using iRacingFriendlist.Classes;
using MessageBox = System.Windows.MessageBox;

namespace iRacingFriendlist.Views
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        private Properties.Settings settings;

        public SettingsWindow()
        {
            InitializeComponent();

            settings = Properties.Settings.Default;
            this.LoadDropdowns();
            this.LoadSettings();
        }

        private void LoadDropdowns()
        {
            var screens = Screen.AllScreens;
            var scr = new List<ComboItem<int>>();
            for (int i = 0; i < screens.Length; i++)
            {
                var screen = screens[i];
                scr.Add(new ComboItem<int>()
                {
                    Value = i,
                    Display = string.Format("{0} ({1})", i + 1, screen.DeviceName)
                });
            }

            cboPopupScreens.ItemsSource = scr;

            var locs = new List<ComboItem<Util.PopupLocations>>();
            locs.Add(new ComboItem<Util.PopupLocations>() { Value = Util.PopupLocations.BottomLeft, Display = "Bottom-left" });
            locs.Add(new ComboItem<Util.PopupLocations>() { Value = Util.PopupLocations.BottomRight, Display = "Bottom-right" });
            locs.Add(new ComboItem<Util.PopupLocations>() { Value = Util.PopupLocations.TopLeft, Display = "Top-left" });
            locs.Add(new ComboItem<Util.PopupLocations>() { Value = Util.PopupLocations.TopRight, Display = "Top-right" });
            cboPopupLocations.ItemsSource = locs;

            var sizes = new List<ComboItem<FontSizes>>();
            sizes.Add(new ComboItem<FontSizes>() { Value = FontSizes.Tiny, Display = "Tiny" });
            sizes.Add(new ComboItem<FontSizes>() { Value = FontSizes.Small, Display = "Small" });
            sizes.Add(new ComboItem<FontSizes>() { Value = FontSizes.Normal, Display = "Normal" });
            sizes.Add(new ComboItem<FontSizes>() { Value = FontSizes.Large, Display = "Large" });
            sizes.Add(new ComboItem<FontSizes>() { Value = FontSizes.Huge, Display = "Huge" });
            cboFontSizes.ItemsSource = sizes;
        }

        private void LoadSettings()
        {
            chkShowOffline.IsChecked = settings.ShowOffline;
            chkGroupStatus.IsChecked = settings.GroupStatus;
            if (settings.OrderBy == 0)
                rbSortName.IsChecked = true;
            else
                rbSortActivity.IsChecked = true;

            var notify = settings.NotifyOn;
            chkNotify1.IsChecked = (notify & 1) == 1;
            chkNotify2.IsChecked = (notify & 2) == 2;
            chkNotify4.IsChecked = (notify & 4) == 4;
            chkNotify8.IsChecked = (notify & 8) == 8;
            chkNotify16.IsChecked = (notify & 16) == 16;
            chkNotify32.IsChecked = (notify & 32) == 32;

            txtUpdateFreq.Text = settings.UpdateFreq.ToString();

            txtPopupDuration.Text = settings.PopupDuration.ToString();
            cboPopupScreens.SelectedValue = settings.PopupScreen;
            cboPopupLocations.SelectedValue = (Util.PopupLocations) settings.PopupLocation;
            cboFontSizes.SelectedValue = (FontSizes)settings.FontSize;
        }

        private bool SaveSettings()
        {
            int freq;
            if (!int.TryParse(txtUpdateFreq.Text, out freq))
            {
                MessageBox.Show("Update frequency must be a valid integer!",
                    "Invalid value");
                return false;
            }
            if (freq < Util.MinUpdateFreq)
            {
                MessageBox.Show("Update frequency cannot be lower than " + Util.MinUpdateFreq + " seconds!",
                    "Invalid value");
                return false;
            }

            int popup;
            if (!int.TryParse(txtPopupDuration.Text, out popup))
            {
                MessageBox.Show("Popup duration must be a valid integer!",
                    "Invalid value");
                return false;
            }
            if (popup < 1)
            {
                MessageBox.Show("Popup duration must be more than 0 seconds!",
                    "Invalid value");
                return false;
            }

            settings.ShowOffline = chkShowOffline.IsChecked.GetValueOrDefault();
            settings.GroupStatus = chkGroupStatus.IsChecked.GetValueOrDefault();
            if (rbSortName.IsChecked.GetValueOrDefault())
                settings.OrderBy = 0;
            else
                settings.OrderBy = 1;

            int notify = 0;
            if (chkNotify1.IsChecked.GetValueOrDefault()) notify |= 1;
            if (chkNotify2.IsChecked.GetValueOrDefault()) notify |= 2;
            if (chkNotify4.IsChecked.GetValueOrDefault()) notify |= 4;
            if (chkNotify8.IsChecked.GetValueOrDefault()) notify |= 8;
            if (chkNotify16.IsChecked.GetValueOrDefault()) notify |= 16;
            if (chkNotify32.IsChecked.GetValueOrDefault()) notify |= 32;
            settings.NotifyOn = notify;

            settings.UpdateFreq = Util.CapUpdateFreq(freq);

            settings.PopupDuration = popup;
            settings.PopupLocation = (int)cboPopupLocations.SelectedValue;
            settings.PopupScreen = (int)cboPopupScreens.SelectedValue;
            settings.FontSize = (int) cboFontSizes.SelectedValue;

            settings.Save();

            return true;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (this.SaveSettings())
            {
                this.DialogResult = true;
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private class ComboItem<T>
        {
            public T Value { get; set; }
            public string Display { get; set; }
        }
    }
}

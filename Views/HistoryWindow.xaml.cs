using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace iRacingFriendlist.Views
{
    /// <summary>
    /// Interaction logic for HistoryWindow.xaml
    /// </summary>
    public partial class HistoryWindow : Window
    {
        public HistoryWindow()
        {
            InitializeComponent();

            var view = CollectionViewSource.GetDefaultView(((App) Application.Current).UpdateHistory);
            view.SortDescriptions.Add(new SortDescription("Time", ListSortDirection.Descending));
            grid.ItemsSource = view;
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to clear the update history?",
                "Confirm clear",
                MessageBoxButton.YesNoCancel) == MessageBoxResult.Yes)
            {
                ((App) Application.Current).UpdateHistory.Clear();
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

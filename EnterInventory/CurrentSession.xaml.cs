/* Title:           Current Session
 * Date:            4-13-17
 * Author:          Terry Holmes
 * 
 * Desription:      This will display the current session during data Entry */

using System;
using System.Collections.Generic;
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
using NewEventLogDLL;
using System.Windows.Threading;
using InventoryWIPDLL;

namespace EnterInventory
{
    /// <summary>
    /// Interaction logic for CurrentSession.xaml
    /// </summary>
    public partial class CurrentSession : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        InventoryWIPClass TheInventoryWIPClass = new InventoryWIPClass();

        DispatcherTimer DataSetTimer = new DispatcherTimer();

        public CurrentSession()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataSetTimer.Tick += new EventHandler(UpdateGridView);
            DataSetTimer.Interval = new TimeSpan(0, 0, 10);
            DataSetTimer.Start();

        }
        private void UpdateGridView(object Sender, EventArgs e)
        {
            try
            {
                MainWindow.TheFindWIPBySessionIDDataSet = TheInventoryWIPClass.FindWIPBySessionID(MainWindow.gintSessionID);

                dgrResults.ItemsSource = MainWindow.TheFindWIPBySessionIDDataSet.FindWIPBySessionID;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Enter Inventory \\ Current Session \\ Windows Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}

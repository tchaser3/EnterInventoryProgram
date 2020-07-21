/* Title:           View Current Sesssion
 * Date:            4-14-17
 * Author:          Terry Holmes
 * 
 * Description:     This is the form to see the current session */

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
using InventoryWIPDLL;
using NewEventLogDLL;

namespace EnterInventory
{
    /// <summary>
    /// Interaction logic for ViewCurrentSession.xaml
    /// </summary>
    public partial class ViewCurrentSession : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        InventoryWIPClass TheInventoryWIPClass = new InventoryWIPClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        ProcessBatchClass TheProcessBatchClass = new ProcessBatchClass();

        public ViewCurrentSession()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void btnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            MainMenu MainMenu = new MainMenu();
            MainMenu.Show();
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                MainWindow.TheFindWIPBySessionIDDataSet = TheInventoryWIPClass.FindWIPBySessionID(MainWindow.gintSessionID);

                dgrInventoryWIP.ItemsSource = MainWindow.TheFindWIPBySessionIDDataSet.FindWIPBySessionID;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Enter Inventory \\ View Current Session \\ Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnProcessBatch_Click(object sender, RoutedEventArgs e)
        {
            //this will update the inventory tables
            TheProcessBatchClass.UpdateInventoryTables();

            MainWindow.TheFindWIPBySessionIDDataSet = TheInventoryWIPClass.FindWIPBySessionID(MainWindow.gintSessionID);
        }
    }
}

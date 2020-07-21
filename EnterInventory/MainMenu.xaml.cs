/* Title:           Main Menu
 * Date:            2-25-17
 * Author:          Terry Holmes
 * 
 * Description      This is the Main Menu */

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

namespace EnterInventory
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Window
    {
        //settin up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();

        public MainMenu()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            //this will Close The Program
            TheMessagesClass.CloseTheProgram();
        }

        private void btnAbout_Click(object sender, RoutedEventArgs e)
        {
            About About = new About();
            About.ShowDialog();
        }

        private void btnReceiveMaterial_Click(object sender, RoutedEventArgs e)
        {
            //setting variable
            MainWindow.gstrMenuSelection = "Receive";
            MainWindow.gblnFromForm = false;

            SelectWarehouse SelectWarehouse = new SelectWarehouse();
            SelectWarehouse.Show();
            this.Close();
        }

        private void btnIssueMaterial_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.gstrMenuSelection = "Issue";
            MainWindow.gblnFromForm = false;

            SelectWarehouse SelectWarehouse = new SelectWarehouse();
            SelectWarehouse.Show();
            this.Close();
        }

        private void btnBOMMaterial_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.gstrMenuSelection = "BOM";
            MainWindow.gblnFromForm = false;

            SelectWarehouse SelectWarehouse = new SelectWarehouse();
            SelectWarehouse.Show();
            this.Close();
        }

        private void btnViewCurrentSession_Click(object sender, RoutedEventArgs e)
        {
            ViewCurrentSession ViewCurrentSession = new ViewCurrentSession();
            ViewCurrentSession.Show();
            Close();
        }
    }
}

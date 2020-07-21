/* Title:           Select Warehouse
 * Date:            2-25-17
 * Author:          Terry Holmes
 * 
 * Description:     This form is for selecting the warehouse */

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

namespace EnterInventory
{
    /// <summary>
    /// Interaction logic for SelectWarehouse.xaml
    /// </summary>
    public partial class SelectWarehouse : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();

        public SelectWarehouse()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this will load up the combo box
            //setting local variables
            int intCounter;
            int intNumberOfRecords;

            try
            {
                //setting up the initial condition
                cboSelectWarehouse.Items.Add("Select Warehouse");

                intNumberOfRecords = MainWindow.TheFindPartsWarehouseDataSet.FindPartsWarehouses.Rows.Count - 1;

                //for loop
                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    cboSelectWarehouse.Items.Add(MainWindow.TheFindPartsWarehouseDataSet.FindPartsWarehouses[intCounter].FirstName);
                }

                cboSelectWarehouse.SelectedIndex = 0;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Enter Inventory Select Warehouse Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            MainMenu MainMenu = new MainMenu();
            MainMenu.Show();
            this.Close();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void cboSelectWarehouse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting local variables
            string strWarehouse;
            int intCounter;
            int intNumberOfRecords;

            strWarehouse = cboSelectWarehouse.SelectedItem.ToString();

            if(strWarehouse != "Select Warehouse")
            {
                try
                {
                    intNumberOfRecords = MainWindow.TheFindPartsWarehouseDataSet.FindPartsWarehouses.Rows.Count - 1;

                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        if (strWarehouse == MainWindow.TheFindPartsWarehouseDataSet.FindPartsWarehouses[intCounter].FirstName)
                        {
                            MainWindow.gstrWarehouseName = strWarehouse;
                            MainWindow.gintWarehouseID = MainWindow.TheFindPartsWarehouseDataSet.FindPartsWarehouses[intCounter].EmployeeID;
                        }
                    }

                    if(MainWindow.gblnFromForm == false)
                    {
                        EnterMaterial EnterMaterial = new EnterMaterial();
                        EnterMaterial.Show();
                    }
                    
                    this.Close();
                }
                catch (Exception Ex)
                {
                    TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Enter Inventory Select Warehouse cbo Select Warehouse " + Ex.Message);

                    TheMessagesClass.ErrorMessage(Ex.ToString());
                }
                
            }

        }
    }
}

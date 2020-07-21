/* Title:           Enter Material
 * Date;            2-25-17
 * Author:          Terry Holmes
 * 
 * Description:     This form is used to enter the material */

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
using InventoryWIPDLL;
using DataValidationDLL;
using NewPartNumbersDLL;
using KeyWordDLL;
using NewEmployeeDLL;
using ProjectsDLL;
using InventoryDLL;

namespace EnterInventory
{
    /// <summary>
    /// Interaction logic for EnterMaterial.xaml
    /// </summary>
    public partial class EnterMaterial : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        InventoryWIPClass TheInventoryWIPClass = new InventoryWIPClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        PartNumberClass ThePartNumberClass = new PartNumberClass();
        KeyWordClass TheKeyWordClass = new KeyWordClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        ProjectClass TheProjectClass = new ProjectClass();
        CurrentSession CurrentSession = new CurrentSession();
        ProcessBatchClass TheProcessBatchClass = new ProcessBatchClass();
        InventoryClass TheInventoryClass = new InventoryClass();

        //setting up the data
        FindEmployeesForMaterialsDataSet TheFindEmployeesForMaterialsDataSet = new FindEmployeesForMaterialsDataSet();
        DisplayPartsDataSet TheDisplayPartsDataSet = new DisplayPartsDataSet();
        FindPartByJDEPartNumberDataSet TheFindPartByJDEPartNumber = new FindPartByJDEPartNumberDataSet();
        FindPartByPartIDDataSet TheFindPartByPartIDDataSet = new FindPartByPartIDDataSet();
        FindPartByPartNumberDataSet TheFindPartByPartNumberDataSet = new FindPartByPartNumberDataSet();
        FindProjectByAssignedProjectIDDataSet TheFindProjectByAssignedProjectIDDataSet = new FindProjectByAssignedProjectIDDataSet();
        FindProjectByProjectNameDataSet TheFindProjectByProjectNameDataSet = new FindProjectByProjectNameDataSet();
        FindProjectByProjectIDDataSet TheFindProjectByProjectIDDataSet = new FindProjectByProjectIDDataSet();
        FindWarehouseInventoryPartDataSet TheFindWarehouseInventoryPartDataSet = new FindWarehouseInventoryPartDataSet();
        FindWIPByPartIDAndWarehouseIDDataSet TheFindByPartIDAndWarehouseIDDataSet = new FindWIPByPartIDAndWarehouseIDDataSet();

        //setting global variables
        int gintWarehouseID;
        int gintSelectedEmployeeID;

        public EnterMaterial()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            //this will close the program
            TheMessagesClass.CloseTheProgram();
        }

        private void btnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            MainMenu MainMenu = new MainMenu();
            CurrentSession.Close();
            MainMenu.Show();
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //setting local variables
            string strAssignedWarehouse;

            strAssignedWarehouse = "This is the " + MainWindow.gstrWarehouseName + " Warehouse";

            lblAssignedWarehouse.Content = strAssignedWarehouse;

            txtEnterLastName.Visibility = Visibility.Hidden;
            cboSelectEmployee.Visibility = Visibility.Hidden;
            lblEnterLastName.Visibility = Visibility.Hidden;
            lblSelectEmployee.Visibility = Visibility.Hidden;


            if (MainWindow.gstrMenuSelection == "Receive")
            {
                lblTitle.Content = "Enter Material Received or Returned";
            }
            else if(MainWindow.gstrMenuSelection == "Issue")
            {
                lblTitle.Content = "Enter Material Issued";
                txtEnterLastName.Visibility = Visibility.Visible;
                cboSelectEmployee.Visibility = Visibility.Visible;
                lblEnterLastName.Visibility = Visibility.Visible;
                lblSelectEmployee.Visibility = Visibility.Visible;
                txtMSRPONumber.Visibility = Visibility.Hidden;
            }
            else if(MainWindow.gstrMenuSelection == "BOM")
            {
                lblTitle.Content = "Enter Material Reported To Charter";
                txtMSRPONumber.Visibility = Visibility.Hidden;
            }

            gintWarehouseID = MainWindow.gintWarehouseID;
            
            CurrentSession.Show();

            txtProjectID.Focus();

            AddSaveRecord();
        }

        private void btnChangeWarehouse_Click(object sender, RoutedEventArgs e)
        {
            //this will allow the warehouse to be changes

            string strAssignedWarehouse;

            MainWindow.gblnFromForm = true;

            SelectWarehouse SelectWarehouse = new SelectWarehouse();
            SelectWarehouse.ShowDialog();
            
            strAssignedWarehouse = "This is the " + MainWindow.gstrWarehouseName + " Warehouse";
            gintWarehouseID = MainWindow.gintWarehouseID;

            lblAssignedWarehouse.Content = strAssignedWarehouse;
        }
        private void AddSaveRecord()
        {
            //setting local variables
            string strPartNumber;
            string strProject;
            int intPartID = 0;
            string strMSRNumber = "NOT REQUIRED";
            string strValueForValidation;
            int intQuantity = 0;
            bool blnThereIsAProblem = false;
            bool blnFatalError = false;
            string strErrorMessage = "";
            int intRecordsFound;
            bool blnProjectNotFound = false;
            bool blnPartNotFound = true;
            bool blnKeyWordNotFound;
            int intTotalQuantity;
            int intProjectID = 0;
            string strJDEPartNumber = "";
            int intEmployeeID = 0;
            int intEnterEmployeeID = 0;

            try
            {
                if (btnAdd.Content.ToString() == "Add")
                {
                    //loading controls
                    txtDateEntryComplete.Text = "NO";
                    txtDate.Text = Convert.ToString(DateTime.Now);

                    btnAdd.Content = "Save";
                }
                else
                {
                    if (MainWindow.gstrMenuSelection == "Issue")
                    {
                        if (cboSelectEmployee.SelectedIndex < 1)
                        {
                            blnFatalError = true;
                            strErrorMessage += "Employee Was Not Selected\n";
                        }

                    }

                    //data validation
                    strProject = txtProjectID.Text;
                    if (strProject == "")
                    {
                        blnFatalError = true;
                        strErrorMessage += "Project ID Was Not Entered\n";
                    }
                    else
                    {

                        TheFindProjectByAssignedProjectIDDataSet = TheProjectClass.FindProjectByAssignedProjectID(strProject);

                        intRecordsFound = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID.Rows.Count;

                        if (intRecordsFound == 0)
                        {
                            TheFindProjectByProjectNameDataSet = TheProjectClass.FindProjectByProjectName(strProject);

                            intRecordsFound = TheFindProjectByProjectNameDataSet.FindProjectByProjectName.Rows.Count;

                            if (intRecordsFound == 0)
                            {
                                blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strProject);

                                if (blnThereIsAProblem == true)
                                {
                                    blnProjectNotFound = true;
                                }
                                else
                                {
                                    TheFindProjectByProjectIDDataSet = TheProjectClass.FindProjectByProjectID(Convert.ToInt32(strProject));

                                    intRecordsFound = TheFindProjectByProjectIDDataSet.FindProjectByProjectID.Rows.Count;

                                    if (intRecordsFound == 0)
                                    {
                                        blnProjectNotFound = true;
                                    }
                                    else
                                    {
                                        intProjectID = TheFindProjectByProjectIDDataSet.FindProjectByProjectID[0].ProjectID;
                                        strProject = TheFindProjectByProjectIDDataSet.FindProjectByProjectID[0].AssignedProjectID;
                                    }
                                }
                            }
                            else
                            {
                                strProject = TheFindProjectByProjectNameDataSet.FindProjectByProjectName[0].AssignedProjectID;
                                intProjectID = TheFindProjectByProjectNameDataSet.FindProjectByProjectName[0].ProjectID;
                            }
                        }
                        else
                        {
                            intProjectID = TheFindProjectByAssignedProjectIDDataSet.FindProjectByAssignedProjectID[0].ProjectID;
                        }

                        if (blnProjectNotFound == true)
                        {
                            blnFatalError = true;
                            strErrorMessage += "Project Was Not Found\n";
                        }
                    }
                    if (MainWindow.gstrMenuSelection == "Receive")
                    {
                        strMSRNumber = txtMSRPONumber.Text;
                        if (strMSRNumber == "")
                        {
                            blnFatalError = true;
                            strErrorMessage += "MSR or PO Number Was Not Entered\n";
                        }
                        else if (strMSRNumber == "NO MSR NUMBER PROVIDED")
                        {
                            blnFatalError = true;
                            strErrorMessage += "NO MSR NUMBER PROVIDED Cannot Be Entered\n";
                        }
                    }
                    strPartNumber = txtPartNumber.Text;
                    if (strPartNumber == "")
                    {
                        blnFatalError = true;
                        strErrorMessage += "Part Number Was Not Entered\n";
                    }
                    else
                    {
                        blnPartNotFound = true;

                        blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strPartNumber);
                        if (blnThereIsAProblem == false)
                        {
                            intPartID = Convert.ToInt32(strPartNumber);

                            TheFindPartByPartIDDataSet = ThePartNumberClass.FindPartByPartID(intPartID);

                            intRecordsFound = TheFindPartByPartIDDataSet.FindPartByPartID.Rows.Count;

                            if (intRecordsFound == 1)
                            {
                                blnPartNotFound = false;
                                strPartNumber = TheFindPartByPartIDDataSet.FindPartByPartID[0].PartNumber;
                                strJDEPartNumber = TheFindPartByPartIDDataSet.FindPartByPartID[0].JDEPartNumber;
                            }
                        }

                        if (blnPartNotFound == true)
                        {
                            TheFindPartByPartNumberDataSet = ThePartNumberClass.FindPartByPartNumber(strPartNumber);

                            intRecordsFound = TheFindPartByPartNumberDataSet.FindPartByPartNumber.Rows.Count;

                            if (intRecordsFound == 1)
                            {
                                blnPartNotFound = false;
                                intPartID = TheFindPartByPartNumberDataSet.FindPartByPartNumber[0].PartID;
                                strJDEPartNumber = TheFindPartByPartNumberDataSet.FindPartByPartNumber[0].JDEPartNumber;
                            }

                            if (blnPartNotFound == true)
                            {
                                TheFindPartByJDEPartNumber = ThePartNumberClass.FindPartByJDEPartNumber(strPartNumber);

                                intRecordsFound = TheFindPartByJDEPartNumber.FindPartByJDEPartNumber.Rows.Count;

                                if (intRecordsFound == 1)
                                {
                                    blnPartNotFound = false;
                                    intPartID = TheFindPartByJDEPartNumber.FindPartByJDEPartNumber[0].PartID;
                                    strPartNumber = TheFindPartByJDEPartNumber.FindPartByJDEPartNumber[0].PartNumber;
                                    strJDEPartNumber = TheFindPartByJDEPartNumber.FindPartByJDEPartNumber[0].JDEPartNumber;
                                }
                            }
                        }

                        if (MainWindow.gstrWarehouseName != "TRAINING")
                        {
                            //checking to see if non charter number
                            blnKeyWordNotFound = TheKeyWordClass.FindKeyWord("JH", MainWindow.gstrWarehouseName);

                            if (blnKeyWordNotFound == true)
                            {
                                if (strJDEPartNumber == "NOT REQUIRED")
                                {
                                    blnFatalError = true;
                                    strErrorMessage += "Using Non Charter Part Number for Charter Warehouse\n";
                                }

                                if (strJDEPartNumber == "NOT PROVIDED")
                                {
                                    blnFatalError = true;
                                    strErrorMessage += "Using Non Charter Part Number for Charter Warehouse\n";
                                }
                            }
                        }

                        if (blnPartNotFound == true)
                        {
                            blnFatalError = true;
                            strErrorMessage += "Part Number Not Found\n";
                        }
                    }
                    
                    //validating the Quantity
                    strValueForValidation = txtQuantity.Text;
                    blnThereIsAProblem = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                    if (blnThereIsAProblem == true)
                    {
                        blnFatalError = true;
                        strErrorMessage += "The Quantity Entered is not an Integer\n";
                    }
                    else
                    {
                        intQuantity = Convert.ToInt32(strValueForValidation);
                    }

                    if (blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage(strErrorMessage);
                        return;
                    }

                    //checking quantity for issuing
                    if (MainWindow.gstrMenuSelection == "Issue")
                    {
                        TheFindWarehouseInventoryPartDataSet = TheInventoryClass.FindWarehouseInventoryPart(intPartID, MainWindow.gintWarehouseID);

                        intRecordsFound = TheFindWarehouseInventoryPartDataSet.FindWarehouseInventoryPart.Rows.Count;

                        if (intRecordsFound == 0)
                        {
                            intTotalQuantity = 0;
                        }
                        else
                        {
                            intTotalQuantity = TheFindWarehouseInventoryPartDataSet.FindWarehouseInventoryPart[0].Quantity;
                        }

                        intTotalQuantity = CalculateWIPCount(intPartID, MainWindow.gintWarehouseID, intTotalQuantity);

                        if (intQuantity > intTotalQuantity)
                        {
                            TheMessagesClass.ErrorMessage("The Quantity Issued is greater than the Quantity On The Shelf");
                            return;
                        }
                    }

                    if (MainWindow.gstrMenuSelection == "Issue")
                    {
                        intEmployeeID = gintSelectedEmployeeID;
                        intEnterEmployeeID = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID;
                    }
                    else
                    {
                        intEmployeeID = MainWindow.TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID;
                    }

                    blnFatalError = TheInventoryWIPClass.InsertDataEntryWIP(MainWindow.gintSessionID, intPartID, strPartNumber, intProjectID, strProject, strMSRNumber, MainWindow.gintWarehouseID, intQuantity, intEmployeeID, MainWindow.gstrMenuSelection.ToUpper());
                    
                    if(blnFatalError == true)
                    {
                        TheMessagesClass.ErrorMessage("There Has Been A Problem. Contact IT");
                        return;
                    }

                    ClearControls();
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Enter Inventory \\ Enter Material \\ Add Button During Save " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
            
            
        }
        private void ClearControls()
        {
            txtPartNumber.Text = "";
            txtQuantity.Text = "";
        }
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            AddSaveRecord();
        }
        private int CalculateWIPCount(int intPartID, int intWarehouseID, int intTotalQuantity)
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;

            try
            {
                //getting the records
                TheFindByPartIDAndWarehouseIDDataSet = TheInventoryWIPClass.FindWIPByPartIDAndWarehouseID(intPartID, intWarehouseID);

                intNumberOfRecords = TheFindByPartIDAndWarehouseIDDataSet.FindWIPByPartIDAndWarehouseID.Rows.Count - 1;

                if(intNumberOfRecords > -1)
                {
                    for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        if(TheFindByPartIDAndWarehouseIDDataSet.FindWIPByPartIDAndWarehouseID[intCounter].TransactionType == "RECEIVE")
                        {
                            intTotalQuantity += TheFindByPartIDAndWarehouseIDDataSet.FindWIPByPartIDAndWarehouseID[intCounter].Quantity;
                        }
                        if(TheFindByPartIDAndWarehouseIDDataSet.FindWIPByPartIDAndWarehouseID[intCounter].TransactionType == "ISSUE")
                        {
                            intTotalQuantity -= TheFindByPartIDAndWarehouseIDDataSet.FindWIPByPartIDAndWarehouseID[intCounter].Quantity;
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Enter Inventory \\ Enter Material \\ Calculate WIP Count " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            return intTotalQuantity;
        }
        private void txtEnterLastName_TextChanged(object sender, TextChangedEventArgs e)
        {
            string strLastName;
            int intNumberOfRecords;
            int intCounter;

            //getting the last name
            strLastName = txtEnterLastName.Text;

            cboSelectEmployee.Items.Clear();

            if (strLastName == "")
            {
                TheMessagesClass.ErrorMessage("Employee Name Was Not Entered");
                return;
            }

            //loading the combo box
            cboSelectEmployee.Items.Add("Select Employee");

            //getting the data set
            TheFindEmployeesForMaterialsDataSet = TheEmployeeClass.FindEmployeesForMaterials(strLastName);

            //getting the records count
            intNumberOfRecords = TheFindEmployeesForMaterialsDataSet.FindEmployeesForMaterials.Rows.Count - 1;

            if (intNumberOfRecords == -1)
            {
                TheMessagesClass.InformationMessage("Employee Not Found");
                return;
            }

            //loop
            for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
            {
                cboSelectEmployee.Items.Add(TheFindEmployeesForMaterialsDataSet.FindEmployeesForMaterials[intCounter].FullName);
            }

            cboSelectEmployee.SelectedIndex = 0;
        }

        private void cboSelectEmployee_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;
            string strEmployeeName;

            if(cboSelectEmployee.SelectedIndex > -1)
            {
                strEmployeeName = cboSelectEmployee.SelectedItem.ToString();

                if (strEmployeeName != "Select Employee")
                {
                    //getting the number of cords
                    intNumberOfRecords = TheFindEmployeesForMaterialsDataSet.FindEmployeesForMaterials.Rows.Count - 1;

                    for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                    {
                        if (strEmployeeName == TheFindEmployeesForMaterialsDataSet.FindEmployeesForMaterials[intCounter].FullName)
                        {
                            gintSelectedEmployeeID = TheFindEmployeesForMaterialsDataSet.FindEmployeesForMaterials[intCounter].EmployeeID;
                        }
                    }
                }
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnProcessBatch_Click(object sender, RoutedEventArgs e)
        {
            TheProcessBatchClass.UpdateInventoryTables();

            MainWindow.TheFindWIPBySessionIDDataSet = TheInventoryWIPClass.FindWIPBySessionID(MainWindow.gintSessionID);
        }
    }
}

/* Title:           Main Window
 * Date:            2-24-17
 * Author:          Terry Holmes
 * 
 * Description:     This form is used to log on to Enter Inventory */

 /*
  * This program will replace the Enter Inventory form that is in the WhseTrack Program
  */

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
using System.Windows.Navigation;
using System.Windows.Shapes;
using DataValidationDLL;
using NewEmployeeDLL;
using NewEventLogDLL;
using KeyWordDLL;
using InventoryWIPDLL;
using NewPartNumbersDLL;
using EmployeeDateEntryDLL;

namespace EnterInventory
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EmployeeClass TheEmployeeClass = new EmployeeClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        KeyWordClass TheKeyWordClass = new KeyWordClass();
        InventoryWIPClass TheInventoryWIPClass = new InventoryWIPClass();
        PartNumberClass ThePartNumberClass = new PartNumberClass();
        EmployeeDateEntryClass TheEmployeeDataEntryClass = new EmployeeDateEntryClass();
        
        //setting up the data
        public static VerifyLogonDataSet TheVerifyLogonDataSet = new VerifyLogonDataSet();
        public static FindPartsWarehousesDataSet TheFindPartsWarehouseDataSet = new FindPartsWarehousesDataSet();
        public static FindSessionByEmployeeIDDataSet TheFindSessionByemployeeID = new FindSessionByEmployeeIDDataSet();
        public static FindWIPBySessionIDDataSet TheFindWIPBySessionIDDataSet = new FindWIPBySessionIDDataSet();
        public static FindPartByPartIDDataSet TheFindPartByPartIDDataSEt = new FindPartByPartIDDataSet();
        public static FindPartByJDEPartNumberDataSet TheFindPartByJDEPartNumberDataSet = new FindPartByJDEPartNumberDataSet();
        public static FindPartByPartNumberDataSet TheFindPartByPartNumberDataSet = new FindPartByPartNumberDataSet();

        //setting global variables
        int gintNumberOfMisses;
        public static string gstrMenuSelection;
        public static bool gblnFromForm;
        public static int gintSessionID;
        public static int gintEmployeeID;
        public static int gintWarehouseID;
        public static string gstrWarehouseName;
        

        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            //this will close the form
            TheMessagesClass.CloseTheProgram();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //setting form conditions
                TheFindPartsWarehouseDataSet = TheEmployeeClass.FindPartsWarehouses();

                pbxEmployeeID.Focus();
                gintNumberOfMisses = 0;

            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Enter Inventory // Main Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
            
        }

        private void btnSignIn_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            string strLastName;
            bool blnThereIsAproblem = false;
            bool blnFatalError = false;
            string strValueForValidation;
            string strErrorMessage = "";
            int intRecordsReturned;
            bool blnOpenSession = false;

            TheFindPartsWarehouseDataSet = TheEmployeeClass.FindPartsWarehouses();

            try
            {
                //data validation
                strValueForValidation = pbxEmployeeID.Password;
                blnThereIsAproblem = TheDataValidationClass.VerifyIntegerData(strValueForValidation);
                if(blnThereIsAproblem == true)
                {
                    blnFatalError = true;
                    strErrorMessage = strErrorMessage + "The Employee ID is not an Integer\n";
                }
                else
                {
                    gintEmployeeID = Convert.ToInt32(strValueForValidation);
                }
                strLastName = txtLastName.Text;
                if(strLastName == "")
                {
                    blnFatalError = true;
                    strErrorMessage = strErrorMessage + "The Last Name Was Not Entered\n";
                }
                if(blnFatalError == true)
                {
                    TheMessagesClass.ErrorMessage(strErrorMessage);
                    return;
                }

                TheVerifyLogonDataSet = TheEmployeeClass.VerifyLogon(gintEmployeeID, strLastName);

                intRecordsReturned = TheVerifyLogonDataSet.VerifyLogon.Rows.Count;

                if(intRecordsReturned == 0)
                {
                    LogonFailed();
                }
                else
                {
                    if(TheVerifyLogonDataSet.VerifyLogon[0].EmployeeGroup == "USERS")
                    {
                        LogonFailed();
                    }
                    else
                    {
                        blnOpenSession = CheckForOpenSession();

                        blnFatalError = TheEmployeeDataEntryClass.InsertIntoEmployeeDateEntry(gintEmployeeID, "ENTER INVENTORY // USER LOGIN");

                        if (blnFatalError == true)
                            throw new Exception();

                        if(blnOpenSession == false)
                        {
                            MainMenu MainMenu = new EnterInventory.MainMenu();
                            MainMenu.Show();
                            this.Hide();
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry( DateTime.Now, "Enter Inventory // Main Window // Sign In Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
        private void LogonFailed()
        {
            //this routine will process if logon has failed
            gintNumberOfMisses++;

            if(gintNumberOfMisses == 3)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "There Have Been Three Attemps to Sign Into Enter Inventory");

                TheMessagesClass.ErrorMessage("There Have Been Three Attempts to Logon In\nThe Application Will Shut Down");

                Application.Current.Shutdown();
            }
            else
            {
                TheMessagesClass.InformationMessage("The Sign In Information Is Incorrect");

                return;
            }

        }
        private bool CheckForOpenSession()
        {
            //this will check for open sessions or create open sessions
            //creating local variables
            int intEmployeeID;
            int intRecordsReturned;
            bool blnOpenSession = false;
            
            try
            {
                //getting the employee id
                intEmployeeID = TheVerifyLogonDataSet.VerifyLogon[0].EmployeeID;

                //checking for open sessions
                TheFindSessionByemployeeID = TheInventoryWIPClass.FindSessionByEmployeeID(intEmployeeID);

                //getting the record count
                intRecordsReturned = TheFindSessionByemployeeID.FindSessionByEmployeeID.Rows.Count;

                if(intRecordsReturned == 0)
                {
                    TheMessagesClass.InformationMessage("No Existing WIP Transactions Found, This is a new Session");

                    TheInventoryWIPClass.InsertNewSession(intEmployeeID);

                    TheFindSessionByemployeeID = TheInventoryWIPClass.FindSessionByEmployeeID(intEmployeeID);

                    gintSessionID = TheFindSessionByemployeeID.FindSessionByEmployeeID[0].SessionID;
                }
                else if(intRecordsReturned == 1)
                {
                    gintSessionID = TheFindSessionByemployeeID.FindSessionByEmployeeID[0].SessionID;

                    TheFindWIPBySessionIDDataSet = TheInventoryWIPClass.FindWIPBySessionID(gintSessionID);

                    TheMessagesClass.InformationMessage("You Will Be Working in a Continued Session");
                }
                else
                {
                    TheMessagesClass.ErrorMessage("There is a Problem with your Session, Please Contact IT");
                    blnOpenSession = true;
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Enter Inventory Main Window Check For Open Session " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());

                blnOpenSession = true;
            }

            return blnOpenSession;

        }
    }
}

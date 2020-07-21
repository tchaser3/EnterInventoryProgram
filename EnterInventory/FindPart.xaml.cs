/* Title:           Find Part Number
 * Date:            2-28-17
 * Author:          Terry Holmes
 * 
 * Description:     This will find the part number */

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
using DataValidationDLL;
using NewPartNumbersDLL;
using KeyWordDLL;

namespace EnterInventory
{
    /// <summary>
    /// Interaction logic for FindPart.xaml
    /// </summary>
    public partial class FindPart : Window
    {
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DataValidationClass TheDataValidationClass = new DataValidationClass();
        PartNumberClass ThePartNumberClass = new PartNumberClass();
        KeyWordClass TheKeyWordClass = new KeyWordClass();

        public FindPart()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            //this will close this form
            this.Close();
        }
    }
}

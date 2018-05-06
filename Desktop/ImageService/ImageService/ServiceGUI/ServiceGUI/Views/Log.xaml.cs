using ServiceGUI.DataStructures;
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

namespace ServiceGUI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class Log
    {
        public Log()
        {
            InitializeComponent();
            ViewModels.LogViewModel logViewModel = new ViewModels.LogViewModel();
            this.logDataGrid.ItemsSource = logViewModel.LogsTable;
            this.DataContext = logViewModel;
            logViewModel.logModel.Logs.Add(new DataStructures.LogLine(MessageTypeEnum.WARNING, "hello"));
            logViewModel.logModel.Logs.Add(new DataStructures.LogLine(MessageTypeEnum.FAIL, "hi"));
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}

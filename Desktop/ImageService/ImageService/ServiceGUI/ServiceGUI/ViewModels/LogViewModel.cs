using ServiceGUI.DataStructures;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace ServiceGUI.ViewModels
{

    class LogViewModel : INotifyPropertyChanged
    {
        public ILogModel logModel;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged(string name)
        {
            if (name == "logs")
            {
                this.logsTable.Clear();
                foreach( LogLine logLine in this.logModel.Logs)
                {
                    Brush color;
                    if (logLine.Type == MessageTypeEnum.FAIL)
                    {
                        color = Brushes.Red;
                    } 
                    else if (logLine.Type == MessageTypeEnum.INFO)
                    {
                        color = Brushes.White;
                    }
                    else
                    {
                        color = Brushes.Yellow;
                    }
                    this.logsTable.Add(new DataGridLogLine(logLine.Type, logLine.Message, color));
                }
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("LogsTable"));
            }
            
        }

        private ObservableCollection<DataGridLogLine> logsTable;
        public ObservableCollection<DataGridLogLine> LogsTable
        {
            get
            {
                return this.logsTable;
            }
            set
            {
                this.logsTable = value;
            }    
            
        }

        public LogViewModel()
        {
            this.logModel = new LogModel();
            this.LogsTable = new ObservableCollection<DataGridLogLine>();
            this.logModel.PropertyChanged += 
                delegate (Object sender, PropertyChangedEventArgs e) {
                NotifyPropertyChanged(e.PropertyName);
            }; ;
        }

    }
}

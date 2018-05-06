using ServiceGUI.DataStructures;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;


 class LogModel : ILogModel
{
    public event PropertyChangedEventHandler PropertyChanged;
    private ObservableCollection<LogLine> logs;
    public ObservableCollection<LogLine> Logs
    {
        get { return this.logs; }
        set
        {
            this.logs = value;
            NotifyPropertyChanged(this, new PropertyChangedEventArgs("logs"));
        }
    }

    public LogModel()
	{
        this.logs = new ObservableCollection<LogLine>();
        this.logs.CollectionChanged += delegate (object sender, NotifyCollectionChangedEventArgs e) 
        {
            NotifyPropertyChanged(sender, new PropertyChangedEventArgs("logs"));
        };
    }

    public void NotifyPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        this.PropertyChanged?.Invoke(sender, e);
    }
}

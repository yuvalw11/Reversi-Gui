using ServiceGUI.DataStructures;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;


interface ILogModel : INotifyPropertyChanged
{
    ObservableCollection<LogLine> Logs 
    {
        get; set;
    }
}


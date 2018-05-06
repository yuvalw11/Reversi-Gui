using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.ComponentModel;

namespace ServiceGUI.DataStructures
{
    class DataGridLogLine
    {
        private MessageTypeEnum type;
        private string message;
        private Brush cellColor;

        public MessageTypeEnum Type
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
            }
        }
        public string Message
        {
            get
            {
                return this.message;
            }
            set
            {
                this.message = value;
            }
        }

        [BindableAttribute(true)]
        public Brush CellColor
        {
            get
            {
                return this.cellColor;
            }
            set
            {
                this.cellColor = value;
            }
        }

        public DataGridLogLine(MessageTypeEnum type, string message, Brush cellColor)
        {
            this.Type = type;
            this.Message = message;
            this.CellColor = cellColor;
        }
    }
}

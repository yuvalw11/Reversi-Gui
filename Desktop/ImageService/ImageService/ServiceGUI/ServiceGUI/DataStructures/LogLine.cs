using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceGUI.DataStructures
{
    class LogLine
    {
        private MessageTypeEnum type;
        private string message;

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

        public LogLine(MessageTypeEnum type, string message)
        {
            this.Type = type;
            this.Message = message;
        }
    }
}


using ImageService.Logging.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Logging
{
    public class LoggingService : ILoggingService
    {
        public event EventHandler<MessageRecievedEventArgs> MessageRecieved;

        //the function sends the log the message to present
        public void Log(string message, MessageTypeEnum type)
        {
            MessageRecievedEventArgs e = new MessageRecievedEventArgs();
            e.Message = message;
            e.Status = type;
            this.MessageRecieved?.Invoke(this, e);
        }
    }
}

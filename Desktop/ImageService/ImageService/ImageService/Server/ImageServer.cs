using ImageService.Controller;
using ImageService.Controller.Handlers;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using ImageService.Controller.Handlers;

namespace ImageService.Server
{
    public class ImageServer
    {
        #region Members
        private IImageController m_controller;
        private ILoggingService m_logging;
        private IDirectoryHandler[] handlers;
        #endregion

        #region Properties
        public event EventHandler<CommandRecievedEventArgs> CommandRecieved;          // The event that notifies about a new Command being recieved
        public event EventHandler<DirectoryCloseEventArgs> close;                     // event to notify the handler that it needs to be closed
        #endregion

        //constructor for ImageServer
        public ImageServer(IImageController controller, ILoggingService logging)
        {
            this.m_logging = logging;
            this.m_controller = controller;
            //getting the paths to all directories that need monitoring.
            string[] paths = ReadSetting("Handlers").Split(';');
            for (int i = 0; i < paths.Length; i++)
            {
                if (!System.IO.Directory.Exists(paths[i]))
                {
                    paths = paths.Where(w => w != paths[i]).ToArray();
                }
            }

            this.handlers = new IDirectoryHandler[paths.Length]; //for each dir to monitor we need to create a handler
            for (int i = 0; i < paths.Length; i++)
            {
                this.handlers[i] = new DirectoyHandler(this.m_controller, this.m_logging, this.CommandRecieved);
                CommandRecieved += this.handlers[i].OnCommandRecieved;
                this.handlers[i].DirectoryClose += new EventHandler<DirectoryCloseEventArgs>(OnCloseServer);
                this.handlers[i].StartHandleDirectory(paths[i]); //starting the handler (not working - i beleive needs threads)
                this.m_logging.Log("created handler that listens to " + paths[i], Logging.Modal.MessageTypeEnum.INFO);
            }
        }
        //reads the handlers of the directories we want to listen to from app.config
        static string ReadSetting(string key)
        {
            try
            {
                string result = ConfigurationManager.ConnectionStrings[key].ConnectionString;
                return result;
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings");
                return "";
            }
        }
        //the function closes all the handlers and inform that the server is now closed
        public void  CloseServer()
        {
            //the loop informs each handler that it needs to be closed
            for (int i=0; i<handlers.Length; i++)
            {
                string[] args = { };
                //event to close the handlers
                CommandRecievedEventArgs e = new CommandRecievedEventArgs((int)CommandEnum.CloseCommand, args, "close");
                this.CommandRecieved?.Invoke(handlers[i], e); //invoke the event in order to inforom the handler that it needs to be closed.
            }
            //after closing all the handlers, informs the log that the server is closed
            this.m_logging.Log("the server is now closed", Logging.Modal.MessageTypeEnum.INFO);
        }

        //informs the server that the handler has been closed
        public void OnCloseServer(object sender, DirectoryCloseEventArgs e)
        {
            IDirectoryHandler closeHandler = (IDirectoryHandler)sender;
            this.CommandRecieved -= closeHandler.OnCommandRecieved; //removes the delegate of the current closed handler
            //informs the current handler has been closed
            this.m_logging.Log("The handler that handles the directory " + e.DirectoryPath + " has been closed", Logging.Modal.MessageTypeEnum.INFO);
        }
    }
}

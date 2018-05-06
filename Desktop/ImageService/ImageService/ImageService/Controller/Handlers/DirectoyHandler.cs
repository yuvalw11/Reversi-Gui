using ImageService.Modal;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Logging;
using ImageService.Logging.Modal;
using System.Text.RegularExpressions;
using System.Threading;

namespace ImageService.Controller.Handlers
{
    public class DirectoyHandler : IDirectoryHandler
    {
        #region Members
        private IImageController m_controller;              // The Image Processing Controller
        private ILoggingService m_logging;
        private FileSystemWatcher[] m_dirWatchers;             // The Watcher of the Dir
        private string m_path;           // The Path of directory
        HashSet<string> workingOn = new HashSet<string>();
        #endregion

        public event EventHandler<DirectoryCloseEventArgs> DirectoryClose; // The Event That Notifies that the Directory is being closed

        public DirectoyHandler(IImageController controller, ILoggingService logging, EventHandler<CommandRecievedEventArgs> CommandRecieved)
        {
            this.m_controller = controller;
            this.m_logging = logging;
            CommandRecieved += this.OnCommandRecieved; //listen to command recieved of server
        }


        public void OnCommandRecieved(object sender, CommandRecievedEventArgs e)
        {
            if (e.RequestDirPath == this.m_path || e.RequestDirPath.Equals("close"))
            {
                if (e.CommandID == (int)CommandEnum.CloseCommand)
                this.CloseHandler();
            }

            //should remove?


            /*bool result;
            string message = this.m_controller.ExecuteCommand(e.CommandID, e.Args, out result);
            // check if command has executed succesfully and write result to the log
            if (result)
            {
                this.m_logging.Log(message, MessageTypeEnum.INFO);
            }
            else
            {
                this.m_logging.Log(message, MessageTypeEnum.FAIL);
            }*/
        }

        //the function starts to listen to a given path
        public void StartHandleDirectory(string dirPath)
        {
            this.m_path = dirPath; //setting the path of dir to listen to
            this.m_dirWatchers = new FileSystemWatcher[4]; //defining 4 watcher to each file type
            string[] types = { "*.jpg", "*.bmp", "*.gif", "*.png" };
            //for each type we define a fileSystemWatcher
            for (int i = 0; i < this.m_dirWatchers.Length; i++)
            {
                this.m_dirWatchers[i] = new FileSystemWatcher(this.m_path);
                this.m_dirWatchers[i].Filter = types[i];
                this.m_dirWatchers[i].Created += new FileSystemEventHandler(OnChanged); //when a file is added onChanged is called by event Created
                this.m_dirWatchers[i].EnableRaisingEvents = true;
                this.m_logging.Log("listens to " + this.m_path + " filter: " + types[i], MessageTypeEnum.INFO); //informing logger
            }
        }

        //called when a new file is added
        public void OnChanged(object sender, FileSystemEventArgs e)
        {
            //creating a new task to handle a new file that was added
            Task task = new Task(() => {
                this.m_logging.Log("detected file: " + e.Name, MessageTypeEnum.INFO);
                while (IsFileLocked(new FileInfo(e.FullPath))) {
                    Thread.Sleep(500);
                }
                //perform action when a new file has arrived
                string[] args = new string[1];
                args[0] = e.FullPath;
                bool result;
                this.m_logging.Log("sending command", MessageTypeEnum.INFO);
                string message = this.m_controller.ExecuteCommand((int)CommandEnum.NewFileCommand, args, out result);
                if (!result)
                {
                    //define what to do when an error occurs
                    this.m_logging.Log("file was not added - " + message, MessageTypeEnum.FAIL);
                }
                else
                {
                    this.m_logging.Log("file added", MessageTypeEnum.INFO);
                }
            });
            task.Start(); //starting the task
        }

        public void CloseHandler()
        {
            for (int i = 0; i < this.m_dirWatchers.Length; i++)
            {
                this.m_dirWatchers[i].Dispose();
            }
            //invokes server's OnCloseServer function
            this.DirectoryClose?.Invoke(this, new DirectoryCloseEventArgs(this.m_path, "handler closed"));
        }

        private bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }

    }

}


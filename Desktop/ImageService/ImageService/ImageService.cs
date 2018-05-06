using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using ImageService.Logging;
using ImageService.Logging.Modal;
using ImageService.Server;
using ImageService.Controller;
using ImageService.Modal;
using System.Configuration;

namespace ImageService
{

    public enum ServiceState
    {
        SERVICE_STOPPED = 0x00000001,
        SERVICE_START_PENDING = 0x00000002,
        SERVICE_STOP_PENDING = 0x00000003,
        SERVICE_RUNNING = 0x00000004,
        SERVICE_CONTINUE_PENDING = 0x00000005,
        SERVICE_PAUSE_PENDING = 0x00000006,
        SERVICE_PAUSED = 0x00000007,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ServiceStatus
    {
        public int dwServiceType;
        public ServiceState dwCurrentState;
        public int dwControlsAccepted;
        public int dwWin32ExitCode;
        public int dwServiceSpecificExitCode;
        public int dwCheckPoint;
        public int dwWaitHint; 
    };
    public partial class ImageService1 : ServiceBase
    {
        
        ILoggingService ils;
        ImageServiceModal modal;
        ImageController controller;
        ImageServer server;


        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool SetServiceStatus(IntPtr handle, ref ServiceStatus serviceStatus);

        private int eventId = 1;
        public ImageService1(string[] args)
        {
            InitializeComponent();
            string eventSourceName = readAppSettings("ImageServiceSource");
            string logName = readAppSettings("ImageServiceLog");
            if (args.Count() > 0)
            {
                eventSourceName = args[0];
            }
            if (args.Count() > 1)
            {
                logName = args[1];
            }
            eventLog1 = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists(eventSourceName))
            {
                System.Diagnostics.EventLog.CreateEventSource(eventSourceName, logName);
            }
            eventLog1.Source = eventSourceName;
            eventLog1.Log = logName;
        }

        //this function is called when the service begins.
        protected override void OnStart(string[] args)
        {
            eventLog1.WriteEntry("In OnStart");
            //creating logging service to send messages to the log
            this.ils = new LoggingService();
            //every time the logger receives a message MessageRecieved is invoked and onmsg function in this class is called
            this.ils.MessageRecieved += this.OnMsg;
            string outputDir = ReadSetting("OutPutDir");
            int thumbnailSize = ReadThumbnailSize("ThumbnailSize");
            if (thumbnailSize == -1) //in case of failed conversion to an int
            {
                thumbnailSize = 120;
                eventLog1.WriteEntry("warning: parsing failed, sets thumbnailsize to 120");
            }
            else
            {
                eventLog1.WriteEntry("Thumbnail size has been read successfuly");
            }
            //creating image model (thumbail size should be taken from app.config file)
             this.modal = new ImageServiceModal(outputDir, thumbnailSize);
            eventLog1.WriteEntry("output dir is: " + outputDir);
            //creating the controller and the server.
            this.controller = new ImageController(modal);
            this.server = new ImageServer(controller, this.ils);

        }

        //the function takes a key string and returns the value of it from app.config file
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
        //the function takes a key string and returns the size of the thumbnail
        static int ReadThumbnailSize(string key)
        {
            try
            {
                int result = Int32.Parse(ConfigurationManager.AppSettings[key]);
                //if given size is negative, will turn in to positive
                if (result < 0)
                {
                    result = -result;
                }
                return result;
            }
            //catch if failed to convert from string to int
            catch (FormatException)
            {
                return -1;
            }
        }

        //the function takes a key string and return the source or the log (depends on the sent key)
        static string readAppSettings(string key)
        {
            try
            {
                string result = ConfigurationManager.AppSettings[key];
                return result;
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("error reading app settings");
                return "";
            }
        }

        //executed when the service is being closed
        //the function should send a command to the server to close it and its handlers
        protected override void OnStop()
        {
            eventLog1.WriteEntry("In onStop.");
            server.CloseServer();
            this.ils.MessageRecieved -= this.OnMsg; //removes pointer to OnMsg method

        }

        protected override void OnContinue()
        {
            eventLog1.WriteEntry("In OnContinue.");
        }

        private void OnMsg(object sender, MessageRecievedEventArgs e)
        {
            this.eventLog1.WriteEntry(e.Message);

        }

        private void eventLog1_EntryWritten(object sender, EntryWrittenEventArgs e)
        {

        }
        internal void TestStartupAndStop(string[] args)
        {
            this.OnStart(args);
            Console.ReadLine();
            this.OnStop();
        }
    }
}

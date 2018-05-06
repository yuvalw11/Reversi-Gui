using ImageService.Commands;
using ImageService.Infrastructure;
using ImageService.Infrastructure.Enums;
using ImageService.Modal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageService.Controller
{
    public class ImageController : IImageController
    {
        private IImageServiceModal m_modal;                      // The Modal Object
        private Dictionary<int, ICommand> commands; //contains all the possible commands

        //c'tor for ImageController
        public ImageController(IImageServiceModal modal)
        {
            m_modal = modal;                    // Storing the Modal Of The System
            commands = new Dictionary<int, ICommand>();
            commands.Add((int)CommandEnum.NewFileCommand, new NewFileCommand(m_modal));
            
        }

        //executes a command and set resultSuccesful if the command was successful or not
        public string ExecuteCommand(int commandID, string[] args, out bool resultSuccesful)
        {
            //trying to execute a command
            if (this.commands.ContainsKey(commandID))
            {
                return this.commands[commandID].Execute(args, out resultSuccesful);
            }
            else
            {
                resultSuccesful = false;
                return "command not available";
            }
           
        }
    }
}

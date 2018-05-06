using ImageService.Infrastructure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using static System.Net.Mime.MediaTypeNames;

namespace ImageService.Modal
{
    public class ImageServiceModal : IImageServiceModal
    {
        #region Members
        private string m_OutputFolder;            // The Output Folder
        private int m_thumbnailSize;              // The Size Of The Thumbnail Size
        private static Regex r = new Regex(":");

        //image model constructor
        public ImageServiceModal(string outputFolder, int thumbnailSize)
        {
            this.m_OutputFolder = outputFolder;
            this.m_thumbnailSize = thumbnailSize;
        }

        // <summary>
        // Adds a file to the output dir by creating a thumbnail and directories
        // that represent the year and month the picture was taken from.
        // </summary>
        // <param name="path"> The path of the file to be transfered. </param>
        // <param name="result"> The result of the operation. </param>
        // <returns> Path to transfer the file if succedded, otherwise a failure message. </returns>
        public string AddFile(string path, out bool result)
        {
            if (!Directory.Exists(m_OutputFolder))
            {
                DirectoryInfo createdDir = Directory.CreateDirectory(m_OutputFolder);
                //makes the directory hidden
                createdDir.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

            }
            else
            {
                DirectoryInfo dirExist = new DirectoryInfo(m_OutputFolder);
                //if not hidden, will hide the folder
                if (!dirExist.Attributes.HasFlag(FileAttributes.Hidden))
                dirExist.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
            if (!IsPathValid(path))
            {
                result = false;
                return "path not valid";
            }

            try
            {
                //moving file to the correct dir
                string imageName = Path.GetFileName(path);
                DateTime imageDateTime = GetDateTakenFromImage(path);
                string pathToTransfer = m_OutputFolder + "\\" + imageDateTime.Year + "\\" + imageDateTime.Month;
                Directory.CreateDirectory(pathToTransfer);
                File.Move(path, pathToTransfer + "\\" + Path.GetFileName(path));

                //creating thumnail
                Directory.CreateDirectory(this.m_OutputFolder + "\\thumbnails\\" + imageDateTime.Year + "\\" + imageDateTime.Month);
                string pathToTransferThumb = m_OutputFolder + "\\thumbnails\\" + imageDateTime.Year + "\\" + imageDateTime.Month + "\\" + imageName;
                System.Drawing.Image image = System.Drawing.Image.FromFile(pathToTransfer + "\\" + imageName);
                System.Drawing.Image thumb = image.GetThumbnailImage(this.m_thumbnailSize, this.m_thumbnailSize, () => false, IntPtr.Zero);
                thumb.Save(pathToTransferThumb);

                //success
                result = true;
                return pathToTransfer;
            }
            catch (Exception e)
            {
                //no success
                result = false;
                return "could not transfer file " + Path.GetFileName(path) + " - " + e.Message + "\n" + e.StackTrace;
            }


        }
        // <summary>
        // Gets the date the picture was taken from.
        // </summary>
        // <param name="path"> The path of the file to be transfered. </param>
        // <returns> The date the picture was taken (upon success),
        // or picture creation date if an exception was caught.
        // </returns>
        private static DateTime GetDateTakenFromImage(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (System.Drawing.Image myImage = System.Drawing.Image.FromStream(fs, false, false))
            {
                //tries to take the date the picture was taken from
                try
                {
                   PropertyItem propItem = myImage.GetPropertyItem(36867);
                   string dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
                   return DateTime.Parse(dateTaken);
                }
                //in case of exception, will return the creation time of the picture
                catch (Exception)
                {
                    return File.GetCreationTime(path);
                }
            }
        }

        // <summary>
        // Checks if a the file exist in the given path, 
        // and if it's a picture. if both true, will return
        // true, otherwise will return false.
        // </summary>
        // <param name="path"> The path of the file to be transfered. </param>
        // <returns> true if successful, otherwise false. </returns>
        private static bool IsPathValid(string path)
        {
            if (!File.Exists(path))
            {
                return false;
            }
            FileInfo file = new FileInfo(path);
            if (file.Extension == ".jpg" || file.Extension == ".png" || file.Extension == ".gif" || file.Extension == ".bmp" || file.Extension == ".jpeg")
            {
                return true;
            }
            return false;
        }

        #endregion

    }
}

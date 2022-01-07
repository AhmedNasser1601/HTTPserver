using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Logger
    {
        public static void LogException(Exception ex)
        {
            // TODO: Create log file named log.txt to log exception details in it
            //Datetime:
            //message:
            // for each exception write its details associated with datetime

            string filePath = @"X:\Visual Studio 2019\HTTPserver"; //Path of Project-Folder

            // This Part is Entered only once at the Begin of the Program.
            if (!File.Exists(filePath + "\\log.txt"))
            {
                // Create file to write to.
                using (StreamWriter sw = File.CreateText(filePath + "\\log.txt"))
                {
                    sw.WriteLine("----------------------------------------");
                    sw.WriteLine("Datetime: " + DateTime.Now.ToString() + "\n");
                    sw.WriteLine("message: " + ex.ToString());
                    sw.WriteLine("----------------------------------------");
                }
            }
            else
            {
                using (StreamWriter sw = File.AppendText(filePath + "\\log.txt"))
                {
                    sw.WriteLine("Datetime: " + DateTime.Now.ToString() + "\n");
                    sw.WriteLine("message: " + ex.ToString());
                    sw.WriteLine("----------------------------------------");
                }
            }
        }
    }
}

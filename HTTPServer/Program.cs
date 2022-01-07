using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HTTPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            {
                // If Files already exists, then delete them as a Fresh~Start

                string filePath = @"X:\Visual Studio 2019\HTTPserver"; //Path of Project-Folder

                if (File.Exists(filePath + "\\log.txt"))
                    File.Delete(filePath + "\\log.txt");

                if (File.Exists(filePath + "\\redirectionRules.txt"))
                    File.Delete(filePath + "\\redirectionRules.txt");


            }
            // TODO: Call CreateRedirectionRulesFile() function to create the rules of redirection 
            CreateRedirectionRulesFile();

            //Start server
            // 1) Make server object on port 1000
            Server server = new Server(1000, "redirectionRules.txt");

            // 2) Start Server
            server.StartServer();
        }

        static void CreateRedirectionRulesFile()
        {

            // TODO: Create file named redirectionRules.txt

            string filePath = @"X:\Visual Studio 2019\HTTPserver"; //Path of Project-Folder

            // each line in the file specify a redirection rule
            // example: "aboutus.html,aboutus2.html"
            // means that when making request to aboustus.html,, it redirects me to aboutus2
            try
            {
                // This text is added only once to the file.
                if (!File.Exists(filePath + "\\redirectionRules.txt"))
                {
                    // Create file to write to.
                    using (StreamWriter sw = File.CreateText(filePath + "\\redirectionRules.txt"))
                    {
                        sw.WriteLine("aboutus.html" + "," + "aboutus2.html");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }
    }
}

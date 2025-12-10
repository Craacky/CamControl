using System;
using System.IO;
using PrinterHandler.Controllers;

namespace PrinterHandler
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var controller = new PrintController();

            // If there are command line arguments, treat the first one as JSON data
            if (args.Length > 0)
            {
                controller.HandlePrintRequest(args[0]);
            }
            // Otherwise, read from stdin
            else
            {
                string jsonData = Console.ReadLine();
                if (!string.IsNullOrEmpty(jsonData))
                {
                    controller.HandlePrintRequest(jsonData);
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;

namespace Fougerite.Patcher
{
    internal class Program
    {
        public static string Version = "1.5.1";

        private static void Main(string[] args)
        {
            bool firstPass = args.Contains("-1");
            bool secondPass = args.Contains("-2");

            Logger.Clear();

            if (!firstPass && !secondPass) 
            {
                Logger.Log("No command specified.");
                Logger.Log("Launch patcher with args: \"-1\" (fields update) or\\and \"-2\" (methods update).");
                Logger.Log("Or enter \"0\" to patch with both flags");
                string readResponse =  Console.ReadLine();
                if (readResponse == "0")
                {
                    firstPass = true;
                    secondPass = true;
                }
                else if (readResponse == "-1")
                {
                    firstPass = true;
                }
                else if (readResponse == "-2")
                {
                    secondPass = true;
                }
                else
                {
                    Logger.Log("Unknown argument.");
                    Logger.Log("Press any key to continue...");
                    Console.ReadKey();
                    return;
                }
            }

            ILPatcher patcher = new ILPatcher();

            bool result = true;
            if (firstPass) 
            {
                result = result && patcher.FirstPass();
            }

            if (secondPass) 
            {
                result = result && patcher.SecondPass();
            }

            if (result) {
                Logger.Log("The patch was applied successfully!");
            }

            //Is that really needed for anything ? It makes it harder to automate
            Logger.Log("Press any key to continue...");
            Console.ReadKey();
        }
    }
}

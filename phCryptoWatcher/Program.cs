using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Init;


namespace phCryptoWatcher
{
    class Program
    {
        public static bool killBot = false;
        public static string botname = "phBot-crypto";
        public static int checkTime = 30;
        static void Main(string[] args)
        {
            if (File.Exists(Environment.CurrentDirectory + @"\conf.txt"))
            {
                IniFile myfile = new IniFile(Environment.CurrentDirectory + @"\conf.txt");
                botname = myfile.Read("botname", "General");
                int checkTimeparsed = 0;
                if (int.TryParse(myfile.Read("checktime", "General"), out checkTimeparsed))
                    checkTime = checkTimeparsed;
            }

            Thread botController_Thread = new Thread(botController);
            botController_Thread.Start();

            Thread logController_Thread = new Thread(LogController);
            logController_Thread.Start();
        }

        public static void botController()
        {
            while (true)
            {
                bool isAlive = false;
                foreach (Process procesInfo in Process.GetProcesses())
                {
                    if (procesInfo.ProcessName == botname)
                    {
                        if (killBot)
                        {
                            killBot = false;
                            procesInfo.Kill();
                            isAlive = false;
                        }
                        else
                        {
                            isAlive = true;
                        }
                    }
                }

                if (!isAlive)
                {
                    if (File.Exists(Environment.CurrentDirectory + @"\"+botname + ".exe"))
                    {
                        MyLog(botname + " not alive. Starting...");
                        Process.Start(Environment.CurrentDirectory + @"\" + botname + ".exe");
                    }
                    else
                        MyLog(botname + ".exe not found, please place this tool right next to it...");
                }
                Thread.Sleep(checkTime * 1000);
            }
        }

        public static void LogController()
        {
            while (true)
            {
                try
                {
                    if (File.Exists("phBot.log"))
                    {
                        string lastLine = File.ReadLines("phBot.log").Last();
                        if (lastLine.Contains("[AUTH]"))
                        {
                            killBot = true;
                        }
                    }
                    else
                    {
                        MyLog("phBot.log not found...");
                    }
                }
                catch { }
                Thread.Sleep(checkTime * 10000);
            }
        }   

        public static void MyLog(string s)
        {
            Console.WriteLine("[" + DateTime.Now.ToString("dd-MM-yyyy HH:mm:sstt") + "]: " + s);
        }
    }
}

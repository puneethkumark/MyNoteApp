using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
namespace MyNoteApp
{
    //Singleton logger class
    class Logger
    {

        private static Logger loggerInstance = null;
        private static readonly object instanceLock = new object();
        private static Queue<string> logMessagesQueue;
        private static readonly object messageQueueLock = new object();

        private Logger()
        {
            logMessagesQueue = new Queue<string>();

            Thread logThread = new Thread(new ThreadStart(PushToLogFile));
            logThread.Start();
        }

        public static Logger GetInstance
        {
            get
            {
                if (loggerInstance == null)
                {
                    lock (instanceLock)
                    {
                        if (loggerInstance == null)
                        {
                            loggerInstance = new Logger();
                        }
                    }
                }
                return loggerInstance;
            }
        }

        public void LogMessage(string functionName , string logMessage)
        {
            StringBuilder message = new StringBuilder();
            message.Append(System.DateTime.Now.ToString());
            message.Append(" : ");
            message.Append(functionName);
            message.Append(" : ");
            message.Append(logMessage);

            lock (messageQueueLock)
            {
                logMessagesQueue.Enqueue(message.ToString());
            }
        }

        private void PushToLogFile()
        {
            //if (!File.Exists(Common.LogFileName))
            //{
            //    File.Create(Common.LogFileName);
            //}
            while (true)
            {
                Thread.Sleep(2000);

                StringBuilder logInfo = new StringBuilder();

                //get all the log messages into a string
                lock (messageQueueLock)
                {
                    while ( logMessagesQueue.Count > 0)
                    {
                        logInfo.AppendLine(logMessagesQueue.Dequeue());
                    }
                }

                //appendstring to log File
                if(logInfo.Length > 0)
                {
                    using (StreamWriter sw = File.AppendText(Common.LogFileName))
                    {
                        sw.Write(logInfo.ToString());
                    }
                }                
            }
        }
    }
}

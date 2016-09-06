using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
// for testing purpose syncing to local file.
// path of the local file will be mentioned in config file syncPath XML node

namespace MyNoteApp
{
    partial class XMLInteraction
    {
        // Sync with another file
        public void syncWithCloud()
        {
            while (true)
            {
                try
                {
                    bool needSync = false;
                    //sleep till sync interval specified
                    for (int i = 0; i < AppConf.SyncInteval; i++)
                    {
                        Thread.Sleep(1000);
                        lock (thisOnlineSyncLock)
                        {
                            //If user requested for force update quit sleep and sync
                            if (ForceSyncRequired)
                            {
                                needSync = true;
                                break;
                            }
                        }
                    }

                    if (!needSync)
                    {
                        // check sync required
                        lock (thisOnlineSyncLock)
                        {
                            if (OnlineSyncRequired)
                            {
                                needSync = true;
                            }
                        }
                    }

                    if (needSync)
                    {
                        Logger.GetInstance.LogMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, "Performing sync");

                        if (File.Exists(AppConf.SyncFile))
                        {
                            //lock the object such that no updates made to xml
                            lock (thisXMLLock)
                            {
                                using (Mutex mutex = new Mutex(false, AppConf.SyncFile))
                                {
                                    if (!mutex.WaitOne(0, true))
                                    {
                                        Logger.GetInstance.LogMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, 
                                            "Couldn't aquire sync file lock. Sync delayed till next cycle.");
                                    }
                                    else
                                    {

                                        XMLInteraction syncXMLInteraction = new XMLInteraction(AppConf.SyncFile);
                                        //check for common note changes and local changes
                                        foreach (XmlElement changeElement in xmlTrackChangesNode)
                                        {
                                            bool useLocalToCloud = true;

                                            XmlElement changeElementInClound = syncXMLInteraction.xmlTrackChangesNode[changeElement.Name];
                                            if (null != changeElementInClound)
                                            {
                                                DateTime localTime, cloudTime;
                                                DateTime.TryParse(changeElement.GetAttribute(Common.XmlTimeNodeString), out localTime);
                                                DateTime.TryParse(changeElementInClound.GetAttribute(Common.XmlTimeNodeString), out cloudTime);

                                                if (DateTime.Compare(localTime, cloudTime) < 0)
                                                {
                                                    useLocalToCloud = false;
                                                }
                                                else
                                                {
                                                    syncXMLInteraction.xmlTrackChangesNode.RemoveChild(changeElementInClound);
                                                }
                                            }

                                            if (useLocalToCloud)
                                            {
                                                Operation op;
                                                Enum.TryParse<Operation>(changeElement.GetAttribute(Common.XmlOperationNodeString), out op);
                                                XmlNode tempNode = xmlNotesNode[changeElement.Name];
                                                DateTime tempDateTime;
                                                DateTime.TryParse(tempNode[Common.XmlLastUpdatedNodeString].InnerText, out tempDateTime);
                                                Note tempNote = new Note(op, changeElement.Name, tempDateTime,
                                                          tempNode[Common.XmlTitleNodeString].InnerText,
                                                          tempNode[Common.XmlNoteNodeString].InnerText
                                                          );
                                                syncXMLInteraction.PushChangesToXML(tempNote, false);
                                            }
                                            else
                                            {
                                                Operation op;
                                                Enum.TryParse<Operation>(changeElementInClound.GetAttribute(Common.XmlOperationNodeString), out op);
                                                XmlNode tempNode = syncXMLInteraction.xmlNotesNode[changeElementInClound.Name];
                                                DateTime tempDateTime;
                                                DateTime.TryParse(tempNode[Common.XmlLastUpdatedNodeString].InnerText, out tempDateTime);
                                                Note tempNote = new Note(op, changeElement.Name, tempDateTime,
                                                          tempNode[Common.XmlTitleNodeString].InnerText,
                                                          tempNode[Common.XmlNoteNodeString].InnerText
                                                          );
                                                PushChangesToXML(tempNote, false);
                                            }
                                        }
                                        xmlTrackChangesNode.RemoveAll();
                                        //retrive could changes and sync
                                        foreach (XmlElement changeElementInClound in syncXMLInteraction.xmlTrackChangesNode)
                                        {
                                            Operation op;
                                            Enum.TryParse<Operation>(changeElementInClound.GetAttribute(Common.XmlOperationNodeString), out op);
                                            XmlNode tempNode = syncXMLInteraction.xmlNotesNode[changeElementInClound.Name];
                                            DateTime tempDateTime;
                                            DateTime.TryParse(tempNode[Common.XmlLastUpdatedNodeString].InnerText, out tempDateTime);
                                            Note tempNote = new Note(op, changeElementInClound.Name, tempDateTime,
                                                      tempNode[Common.XmlTitleNodeString].InnerText,
                                                      tempNode[Common.XmlNoteNodeString].InnerText
                                                      );
                                            PushChangesToXML(tempNote, false);
                                        }
                                        syncXMLInteraction.xmlTrackChangesNode.RemoveAll();

                                        //save xml files
                                        SaveXML();
                                        syncXMLInteraction.SaveXML();
                                    }
                                    mutex.ReleaseMutex();
                                }
                            }
                            //reset sync variables
                            lock (thisOnlineSyncLock)
                            {
                                ForceSyncRequired = false;
                                OnlineSyncRequired = false;
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    Logger.GetInstance.LogMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Data.ToString());
                }
            }
        }

        public void OnlineChangesObserved(object source, FileSystemEventArgs e)
        {
            ChangesObserved();
        }

        void ChangesObserved()
        {
            lock (thisOnlineSyncLock)
            {
                OnlineSyncRequired = true;
            }
        }

        public void ForceSync()
        {
            lock (thisOnlineSyncLock)
            {
                ForceSyncRequired = true;
            }
        }
    }

    class Sync
    {
        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public static void CheckSyncRequired(string file, ref XMLInteraction xmlInteraction)
        {
            string filePath = Path.GetFullPath(file);
            string fileName = Path.GetFileName(filePath);
            filePath = Path.GetDirectoryName(filePath);
            
            // Create a new FileSystemWatcher and set its properties.
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.Path = filePath;
            watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite;
            watcher.Filter = fileName;
            watcher.Changed += new FileSystemEventHandler(xmlInteraction.OnlineChangesObserved);
            watcher.EnableRaisingEvents = true;
        }

        //private static void OnChanged(object source, FileSystemEventArgs e)
        //{
        //    Console.WriteLine("File: " + e.FullPath + " " + e.ChangeType);
        //}
    }
}

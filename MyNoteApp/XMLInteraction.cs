using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Windows.Forms;
using System.IO;
using System.Threading;
namespace MyNoteApp
{
    enum Operation
    {
        Delete = 0,
        Add = 1,
        Update = 2
    }

    class Note
    {
        public string NoteID;
        public string Title;
        public string NoteContent;
        public DateTime LastUpdate;
        public Operation operation;

        public Note(Operation op, string noteid, DateTime lastUpdate, string title = "", string noteContent ="")
        {
            this.NoteID = noteid;
            this.Title = title;
            this.NoteContent = noteContent;
            this.LastUpdate = lastUpdate;
            this.operation = op;
        }
    }

    partial class XMLInteraction
    {
        #region data
        XmlDocument xmlNoteDoc = null;
        XmlNode xmlNotesNode = null;
        XmlNode xmlTrackChangesNode = null;
        ApplicationConfiguration AppConf = null;
        string DataFile;
        bool OnlineSyncRequired = false;
        bool ForceSyncRequired = false;
        Dictionary<string, Note> NotesMap = new Dictionary<string, Note>(); // save notes to map first and then process this map
        private Object thisMapLock = new Object();
        private Object thisXMLLock = new Object();
        private Object thisOnlineSyncLock = new Object();


        #endregion

        public XMLInteraction(string fileName)
        {
            DataFile = fileName;
            if (!File.Exists(DataFile))
            {
                CreateNoteFile(DataFile, Common.DefaultNotesXMLString);
            }
            
            xmlNoteDoc = new XmlDocument();
            xmlNoteDoc.Load(DataFile);
            xmlNotesNode = xmlNoteDoc.SelectSingleNode(Common.NotesNodePathInXML);
            xmlTrackChangesNode = xmlNoteDoc.SelectSingleNode(Common.TrackChangesNodePathInXML);
        }


        public XMLInteraction(ApplicationConfiguration appConfig)
        {
            AppConf = appConfig;
            DataFile = AppConf.DataFile;
            if (!File.Exists(DataFile))
            {
                CreateNoteFile(DataFile, Common.DefaultNotesXMLString);
            }
            
            if (File.Exists(DataFile))
            {
                xmlNoteDoc = new XmlDocument();
                xmlNoteDoc.Load(DataFile);
                xmlNotesNode = xmlNoteDoc.SelectSingleNode(Common.NotesNodePathInXML);
                xmlTrackChangesNode = xmlNoteDoc.SelectSingleNode(Common.TrackChangesNodePathInXML);

                Thread processmapThread = new Thread(new ThreadStart(ProcessMapAndUpdateXML));
                processmapThread.Start();
                //check sync file exists
                if (Common.DesktopApplicaiton)
                {
                    Thread syncThread = new Thread(new ThreadStart(syncWithCloud));
                    syncThread.Start();
                }
            }
        }

        #region pushChanges
        // push data to map from UI. another thread (ProcessMapAndUpdateXML) will process this map and update xml
        public void pushChanges(Operation op, string id, DateTime lastUpdated, string title = "", string note = "")
        {
            lock (thisMapLock)
            {
                NotesMap[id] = new Note(op, id, lastUpdated, title, note);
            }
        }
        #endregion

        #region PushChangesToXML
        bool PushChangesToXML(Note note, bool trackChanges = true)
        {
            bool retval = false;
            try
            {
                switch (note.operation)
                {
                    case Operation.Add:
                    case Operation.Update:
                        UpdateNote(note, trackChanges);
                        break;
                    case Operation.Delete:
                        DeleteNote(note, trackChanges);
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.GetInstance.LogMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Data.ToString());
                retval = false;
            }
            return retval;
        }
        #endregion

        #region ProcessMapAndUpdateXML
        // this method run  by thread to process map and save to xml
        public void ProcessMapAndUpdateXML()
        {
            while(true)
            {
                // update XMl on every 1 sec
                for (long i = 0; i < AppConf.DataProcessTime; i++)
                {
                    Thread.Sleep(1000);
                }    
                // lock before using xml object to update            
                lock (thisXMLLock)
                {
                    //lock before processing map
                    lock (thisMapLock)
                    {
                        if (0 < NotesMap.Count)
                        {
                            using (Mutex mutex = new Mutex(false, DataFile))
                            {
                                if (!mutex.WaitOne(0, true))
                                {
                                    Logger.GetInstance.LogMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, "Couldn't aquire file lock. map will be processed in next cycle.");
                                }
                                else
                                {
                                    foreach (Note note in NotesMap.Values)
                                    {
                                        PushChangesToXML(note);
                                    }
                                    xmlNoteDoc.Save(DataFile);
                                    NotesMap.Clear();
                                    ChangesObserved();// inform that changes made to local data xml. 
                                }
                                mutex.ReleaseMutex();
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region CreateNoteFile
        static public bool CreateNoteFile(string dataFile, string xmlContent)
        {
            bool retval = false;
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xmlContent);
                doc.Save(dataFile);
                retval = true;
            }
            catch (Exception ex)
            {
                Logger.GetInstance.LogMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Data.ToString());
                retval = false;
            }
            return retval;
        }
        #endregion

        #region GetAllNoteIds
        public bool GetAllNoteIds(ref Dictionary<string,string> nodeIds)
        {
            bool retval = false;
            try
            {
                lock (thisXMLLock)
                {
                    if (null != xmlNotesNode)
                    {
                        foreach (XmlNode note in xmlNotesNode)
                        {
                            nodeIds[note.Name] = note[Common.XmlTitleNodeString].InnerText;
                        }
                    }
                }
                lock (thisMapLock)
                {
                    foreach (Note note in NotesMap.Values)
                    {
                        if (note.operation != Operation.Delete)
                        {
                            nodeIds[note.NoteID] = note.Title;
                        }
                        else
                        {
                            nodeIds.Remove(note.NoteID);
                        }
                    }
                }
                retval = true;
            }
            catch (Exception ex)
            {
                Logger.GetInstance.LogMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Data.ToString());
                retval = false;
            }
            return retval;
        }
        #endregion

        #region GetNodeDetails
        //If UI requests for any note information, that can be retrived using GetNodeDetails
        public bool GetNodeDetails(string id, out string title, out string note, out string dateTime)
        {
            bool retval = false;
            title = "";
            note = "";
            dateTime = "";
            
            try
            {
                lock (thisMapLock)
                {
                    Note tempNote;
                    if(NotesMap.TryGetValue(id, out tempNote))
                    {
                        title = tempNote.Title;
                        note = tempNote.NoteContent;
                        dateTime = tempNote.LastUpdate.ToString();
                        retval = true;
                    }
                }
                if (!retval)
                {
                    lock (thisXMLLock)
                    {
                        if (null != xmlNotesNode && !string.IsNullOrEmpty(id))
                        {
                            XmlNode tempid = xmlNotesNode[id];
                            if (null != tempid)
                            {
                                title = tempid[Common.XmlTitleNodeString].InnerText;
                                note = tempid[Common.XmlNoteNodeString].InnerText;
                                dateTime = tempid[Common.XmlLastUpdatedNodeString].InnerText;
                                retval = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.GetInstance.LogMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Data.ToString());
                retval = false;
            }
            return retval;
        }
        #endregion

        #region AddNewNote
        bool AddNewNote(Note note, bool trackChanges = true)
        {
            bool retval = false;
            try
            {
                if (null != xmlNotesNode && !string.IsNullOrEmpty(note.NoteID))
                {

                    //Add new note
                    XmlElement tempid = xmlNoteDoc.CreateElement(note.NoteID);
           
                    XmlElement tempTitle = xmlNoteDoc.CreateElement(Common.XmlTitleNodeString);
                    tempTitle.InnerText = note.Title;
                    tempid.AppendChild(tempTitle);

                    XmlElement tempNote = xmlNoteDoc.CreateElement(Common.XmlNoteNodeString);
                    tempNote.InnerText = note.NoteContent;
                    tempid.AppendChild(tempNote);

                    XmlElement tempLastUpdated = xmlNoteDoc.CreateElement(Common.XmlLastUpdatedNodeString);
                    tempLastUpdated.InnerText = note.LastUpdate.ToString();
                    tempid.AppendChild(tempLastUpdated);

                    xmlNotesNode.AppendChild(tempid);

                    //update track changes node
                    if (trackChanges)
                    {
                        UpdateTrackChanges(note.NoteID, Operation.Add, note.LastUpdate.ToString());
                    }

                    //xmlNoteDoc.Save(DataFile);
                    retval = true;
                }
            }
            catch(Exception ex)
            {
                Logger.GetInstance.LogMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Data.ToString());
                retval = false;
            }
            return retval;
        }
        #endregion

        #region UpdateNote
        bool UpdateNote(Note note, bool trackChanges = true)
        {
            bool retval = false;
            try
            {
                if (null != xmlNotesNode && !string.IsNullOrEmpty(note.NoteID))
                {
                    XmlNode tempid = xmlNotesNode[note.NoteID];

                    if (null != tempid)
                    {
                        tempid[Common.XmlTitleNodeString].InnerText = note.Title;
                        tempid[Common.XmlNoteNodeString].InnerText = note.NoteContent;
                        tempid[Common.XmlLastUpdatedNodeString].InnerText = note.LastUpdate.ToString();

                        //update trackChanges
                        if (trackChanges)
                        {
                            UpdateTrackChanges(note.NoteID, Operation.Update, note.LastUpdate.ToString());
                        }
                        //xmlNoteDoc.Save(DataFile);
                        retval = true;
                    }
                    else
                    {
                        retval = AddNewNote(note, trackChanges);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.GetInstance.LogMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Data.ToString());
                retval = false;
            }
            return retval;
        }
        #endregion

        #region DeleteNote
        bool DeleteNote(Note note, bool trackChanges = true)
        {
            bool retval = false;
            try
            {
                if (null != xmlNotesNode && !string.IsNullOrEmpty(note.NoteID))
                {
                    XmlNode tempid = xmlNotesNode[note.NoteID];

                    if (null != tempid)
                    {
                        xmlNotesNode.RemoveChild(tempid);
                        if (trackChanges)
                        {
                            UpdateTrackChanges(note.NoteID, Operation.Delete, note.LastUpdate.ToString());
                        }
                        //xmlNoteDoc.Save(DataFile);
                        retval = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.GetInstance.LogMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Data.ToString());
                retval = false;
            }
            return retval;
        }
        #endregion

        #region UpdateTrackChanges
        public bool UpdateTrackChanges(string id, Operation op, string time)
        {
            //Add track changes
            if (null != xmlTrackChangesNode)
            {
                XmlElement tempElement = xmlTrackChangesNode[id];
                if (null == tempElement)
                {
                    XmlElement tempchangesNode = xmlNoteDoc.CreateElement(id);
                    tempchangesNode.SetAttribute("Operation", op.ToString());
                    tempchangesNode.SetAttribute("Time", time);
                    xmlTrackChangesNode.AppendChild(tempchangesNode);
                }
                else
                {
                    tempElement.SetAttribute("Operation", op.ToString());
                    tempElement.SetAttribute("Time", time);
                }
            }
            return true;
        }
        #endregion

        void SaveXML()
        {
            xmlNoteDoc.Save(DataFile);
        }
    }
}

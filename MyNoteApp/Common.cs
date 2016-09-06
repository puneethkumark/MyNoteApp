using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNoteApp
{
    public static class Common
    {
        public static string MyNoteAppConfigFile = "MyNoteAppConfig.xml";
        public static string MyNoteAppConfigCloudFile = "MyNoteAppConfig_Cloud.xml";
        public static string DefaultConfigString = @"<MyNoteApp>
	        <DataFile>Note.xml</DataFile>
	        <DataProcessTime>1</DataProcessTime>
	        <SyncFile>SyncFileNote.xml</SyncFile>
	        <SyncInterval>30</SyncInterval>
            </MyNoteApp>";
        public const string DefaultCloudConfigString = @"<MyNoteApp>
	        <DataFile>SyncFileNote.xml</DataFile>
	        <DataProcessTime>1</DataProcessTime>
	        <SyncFile></SyncFile>
	        <SyncInterval></SyncInterval>
            </MyNoteApp>";
        public const string XMLMyNoteAppNode = "/MyNoteApp";
        public const string XMLDataFileNode = "DataFile";
        public const string XMLDataProcessTimeNode = "DataProcessTime";
        public const string XMLSyncFileNode = "SyncFile";
        public const string XMLSyncIntervalNode = "SyncInterval";

        public const string MyNoteFile = "Note.xml";
        public const string NotesNodePathInXML = "/MyNote/Notes";
        public const string TrackChangesNodePathInXML = "/MyNote/TrackChanges";
        public const string SyncFileNodePathInXML = "/MyNote/SyncFile";
        public const string XmlTitleNodeString = "Title";
        public const string XmlNoteNodeString = "Note";
        public const string XmlLastUpdatedNodeString = "LastUpdated";
        public const string XmlTimeNodeString = "Time";
        public const string XmlOperationNodeString = "Operation";

        public const string DefaultNotesXMLString = @"<MyNote><Notes></Notes><TrackChanges></TrackChanges>
                </MyNote> ";
        
        public const long DefaultSyncInterval = 300000;

        //const display strings
        public const string UntitledString = "Untitled Note";

        //Logfile
        public static string LogFileName = "MyNoteApp.log";
        public static string LogFileName_cloud = "MyNoteApp_Cloud.log";

        //use this to simulate app as both destop and cloud app
        static public bool DesktopApplicaiton = true;
    }
}

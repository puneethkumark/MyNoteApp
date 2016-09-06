using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;

namespace MyNoteApp
{
    class ApplicationConfiguration
    {
        public string SyncFile = "SyncFileNote.xml";
        public long DataProcessTime = 1;
        public string DataFile = "Notes.xml";
        public long SyncInteval = 120;
        public ApplicationConfiguration()
        {
            if (!File.Exists(Common.MyNoteAppConfigFile))
            {
                XMLInteraction.CreateNoteFile(Common.MyNoteAppConfigFile, Common.DefaultConfigString);
            }
            ReadConfigFile();
        }

        void ReadConfigFile()
        {
            XmlDocument xmlConfigDoc = new XmlDocument();
            xmlConfigDoc.Load(Common.MyNoteAppConfigFile);
            XmlNode XMLMyNoteAppNode = xmlConfigDoc.SelectSingleNode(Common.XMLMyNoteAppNode);
            if (null != XMLMyNoteAppNode)
            {
                XmlNode xmlTempNode = XMLMyNoteAppNode[Common.XMLDataFileNode];
                if (null != xmlTempNode && !string.IsNullOrEmpty(xmlTempNode.InnerText))
                {
                    DataFile = xmlTempNode.InnerText;
                }
                xmlTempNode = XMLMyNoteAppNode[Common.XMLDataProcessTimeNode];
                if (null != xmlTempNode && !string.IsNullOrEmpty(xmlTempNode.InnerText))
                {
                    long.TryParse(xmlTempNode.InnerText, out DataProcessTime);
                }
                xmlTempNode = XMLMyNoteAppNode[Common.XMLSyncFileNode];
                if (null != xmlTempNode && !string.IsNullOrEmpty(xmlTempNode.InnerText))
                {
                    SyncFile = xmlTempNode.InnerText;
                }
                else
                {
                    SyncFile = "";
                }
                xmlTempNode = XMLMyNoteAppNode[Common.XMLSyncIntervalNode];
                if (null != xmlTempNode && !string.IsNullOrEmpty(xmlTempNode.InnerText))
                {
                    long.TryParse(xmlTempNode.InnerText, out SyncInteval);
                }

            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace MyNoteApp
{
    public partial class MyNote : Form
    {
        private NoteForm noteForm = null;
        XMLInteraction xmlInteraction = null;
        ApplicationConfiguration myNoteConfiguraiton = null;

        public MyNote()
        {
            Logger.GetInstance.LogMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, "Initializing Application");
            InitializeComponent();
            Initialize();
        }

        void Initialize()
        {
            myNoteConfiguraiton = new ApplicationConfiguration();
            xmlInteraction = new XMLInteraction(myNoteConfiguraiton);
            if (null != xmlInteraction && Common.DesktopApplicaiton)
            {
                Sync.CheckSyncRequired(myNoteConfiguraiton.SyncFile, ref xmlInteraction);
            }

            UpdateTreeView();

            if(treeView1.Nodes.Count > 0)
            {
                string title;
                string note;
                string lastUpdated;
                if (xmlInteraction.GetNodeDetails(treeView1.Nodes[0].Name, out title, out note, out lastUpdated))
                {
                    noteForm = new NoteForm(treeView1.Nodes[0].Name, title, note, lastUpdated);
                }
                else
                {
                    noteForm = new NoteForm();
                }
            }
            else
            {
                CreateNote();
            }
            ShowNoteForm();
        }

        void ShowNoteForm()
        {
            if (null != noteForm)
            {
                noteForm.TopLevel = false;
                splitContainer2.Panel2.Controls.Add(noteForm);
                noteForm.FormBorderStyle = FormBorderStyle.None;
                noteForm.Dock = DockStyle.Fill;
                noteForm.Show();
            }
        }

        void UpdateTreeView()
        {
            Dictionary<string, string> noteIds = new Dictionary<string, string>();
            xmlInteraction.GetAllNoteIds(ref noteIds);
            foreach (KeyValuePair<string, string> item in noteIds)
            {
                AddNodeToTreeView(item.Key, item.Value);
            }

            Logger.GetInstance.LogMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, string.Format("Updated tree view with notes [{0}]", noteIds.Count));
        }

        void AddNodeToTreeView(string key, string value )
        {  
            if (string.IsNullOrEmpty(value))
            {
                treeView1.Nodes.Add(key, Common.UntitledString);
            }
            else
            {
                treeView1.Nodes.Add(key, value);
            }
            Logger.GetInstance.LogMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, string.Format("Treeview update: Adding node [{0}]", key));
        }

        void RemoveNodeToTreeView(string id)
        {
            TreeNode[] treenode = treeView1.Nodes.Find(id, true);
            if (treenode.Length > 0)
            {
                treeView1.Nodes.Remove(treenode[0]);
            }
            Logger.GetInstance.LogMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, string.Format("Treeview update: Removing node [{0}]", id));
        }

        private void MyNote_Load(object sender, EventArgs a)
        {

        }
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string title;
            string note;
            string lastUpdated;
            if (xmlInteraction.GetNodeDetails(treeView1.SelectedNode.Name, out title, out note, out lastUpdated))
            {
                noteForm.Initalize(treeView1.SelectedNode.Name, title, note, lastUpdated);
                deleteButton.Visible = true;
                ShowNoteForm();
            }
            else
            {
                Logger.GetInstance.LogMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, string.Format("Failed to retrive note: [{0}]", treeView1.SelectedNode.Name));
            }
        }

        private void newNote_Click(object sender, EventArgs e)
        {
            if(noteForm.isDirty)
            {
                if(MessageBox.Show("current note is modified. Do you want to discard?", "Discard changes", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }
            }
            CreateNote();
        }

        private void CreateNote()
        {
            Logger.GetInstance.LogMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, string.Format("Creating new note"));
            if (null == noteForm )
            {
                noteForm = new NoteForm();
            }
            noteForm.Initalize();
            deleteButton.Visible = false;
            ShowNoteForm();
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (noteForm.isDirty)
                {
                    if (null != noteForm && null != xmlInteraction)
                    {
                        DateTime updateTime = System.DateTime.Now;
                        if (string.IsNullOrEmpty(noteForm.NoteID))
                        {
                            noteForm.NoteID = GetIdString();
                            xmlInteraction.pushChanges(Operation.Add,noteForm.NoteID, updateTime, noteForm.titleTextBox.Text, noteForm.noteTextBox.Text);
                            AddNodeToTreeView(noteForm.NoteID, noteForm.titleTextBox.Text);
                        }
                        else
                        {
                            xmlInteraction.pushChanges(Operation.Update, noteForm.NoteID, updateTime, noteForm.titleTextBox.Text, noteForm.noteTextBox.Text);
                        }
                        noteForm.SetLastUpdate(updateTime.ToString());
                        deleteButton.Visible = true;
                        noteForm.isDirty = false;
                        Logger.GetInstance.LogMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, string.Format("Push changes. note: [{0}]", noteForm.NoteID));
                    }
                }
            }
            catch(Exception ex)
            {
                Logger.GetInstance.LogMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Data.ToString());
            }
        }

        string GetIdString()
        {
            string dateTime = System.DateTime.Now.ToString();
            dateTime = dateTime.Replace('/', '_');
            dateTime = dateTime.Replace(':', '_');
            dateTime = dateTime.Replace(' ', '_');
            if (Common.DesktopApplicaiton)
            {
                return "D" + dateTime;
            }
            else
            {
                return "C" + dateTime;
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (MessageBox.Show("Are you sure to delete this note?", "Delete Confirmaiton", MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }
                Logger.GetInstance.LogMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, "Delete Node Request for note Id :" + noteForm.NoteID);
                if (null != noteForm && null != xmlInteraction)
                {
                    if (!string.IsNullOrEmpty(noteForm.NoteID))
                    {
                        xmlInteraction.pushChanges(Operation.Delete, noteForm.NoteID, System.DateTime.Now);

                        RemoveNodeToTreeView(noteForm.NoteID);
                        if (0 == treeView1.Nodes.Count)
                        {
                            CreateNote();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.GetInstance.LogMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, ex.Data.ToString());
            }
        }

        private void MailToButton_Click(object sender, EventArgs e)
        {
            if (null != noteForm)
            {
                Logger.GetInstance.LogMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, "Mail Request for note Id :" + noteForm.NoteID);
                var url = "mailto:?" + noteForm.EmailNoteInformation();
                Process.Start(url);
            }
        }

        private void SyncNotesListButton_Click(object sender, EventArgs e)
        {
            SyncNotesList();
        }

        private void SyncNotesList()
        { 
            string selectedNote = noteForm.NoteID;
            treeView1.Nodes.Clear();
            UpdateTreeView();
            if (treeView1.Nodes.ContainsKey(selectedNote))
            {
                treeView1.SelectedNode = treeView1.Nodes.Find(selectedNote, true)[0];
            }
            else
            {
                if (treeView1.Nodes.Count > 0)
                {
                    string title;
                    string note;
                    string lastUpdated;
                    if (xmlInteraction.GetNodeDetails(treeView1.Nodes[0].Name, out title, out note, out lastUpdated))
                    {
                        noteForm = new NoteForm(treeView1.Nodes[0].Name, title, note, lastUpdated);
                    }
                    else
                    {
                        noteForm = new NoteForm();
                    }
                }
                else
                {
                    CreateNote();
                }
                ShowNoteForm();
            }
        }

        private void forceSyncButton_Click(object sender, EventArgs e)
        {
            Logger.GetInstance.LogMessage(System.Reflection.MethodBase.GetCurrentMethod().Name, "Force Sync request received" );
            xmlInteraction.ForceSync();
        }
    }
}

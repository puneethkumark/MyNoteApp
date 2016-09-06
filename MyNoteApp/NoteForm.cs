using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MyNoteApp
{
    public partial class NoteForm : Form
    {
        public bool isDirty = false;
        public NoteForm()
        {
            InitializeComponent();
            Initalize();
        }

        public NoteForm(string id, string title, string note, string lastUpdated)
        {
            InitializeComponent();
            Initalize(id, title, note, lastUpdated);
        }

        public void Initalize(string id = "", string title="", string note="", string lastUpdated ="")
        {
            NoteID = id;
            titleTextBox.Text = title;
            noteTextBox.Text = note;
            SetLastUpdate(lastUpdated);
            isDirty = false;
        }

        public void SetLastUpdate(string lastUpdated)
        {
            if (string.IsNullOrEmpty(lastUpdated))
            {
                LastUpdated.Visible = false;
            }
            else
            {
                LastUpdated.Visible = true;
                LastUpdated.Text = "Last updated on " + lastUpdated;
            }
        }

        private void titleTextBox_TextChanged(object sender, EventArgs e)
        {
            isDirty = true;
        }

        private void noteTextBox_TextChanged(object sender, EventArgs e)
        {
            isDirty = true;
        }

        public string EmailNoteInformation()
        {
            string subject = "subject=Title:" + titleTextBox.Text;
            string body = " &body=Note:" + noteTextBox.Text + "%0D%0A%0D%0A-Shared from MyNote Application";
            return subject + body;
        }
    }
}

namespace MyNoteApp
{
    partial class MyNote
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.MailToButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.updateButton = new System.Windows.Forms.Button();
            this.newNote = new System.Windows.Forms.Button();
            this.SyncNotesListButton = new System.Windows.Forms.Button();
            this.forceSyncButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(685, 452);
            this.splitContainer1.SplitterDistance = 228;
            this.splitContainer1.TabIndex = 0;
            // 
            // treeView1
            // 
            this.treeView1.Location = new System.Drawing.Point(3, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(222, 449);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.forceSyncButton);
            this.splitContainer2.Panel1.Controls.Add(this.SyncNotesListButton);
            this.splitContainer2.Panel1.Controls.Add(this.MailToButton);
            this.splitContainer2.Panel1.Controls.Add(this.deleteButton);
            this.splitContainer2.Panel1.Controls.Add(this.updateButton);
            this.splitContainer2.Panel1.Controls.Add(this.newNote);
            this.splitContainer2.Panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.splitContainer2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.splitContainer2.Size = new System.Drawing.Size(453, 452);
            this.splitContainer2.SplitterDistance = 66;
            this.splitContainer2.TabIndex = 0;
            // 
            // MailToButton
            // 
            this.MailToButton.Location = new System.Drawing.Point(258, 12);
            this.MailToButton.Name = "MailToButton";
            this.MailToButton.Size = new System.Drawing.Size(75, 23);
            this.MailToButton.TabIndex = 3;
            this.MailToButton.Text = "Mail to...";
            this.MailToButton.UseVisualStyleBackColor = true;
            this.MailToButton.Click += new System.EventHandler(this.MailToButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.Location = new System.Drawing.Point(177, 12);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(75, 23);
            this.deleteButton.TabIndex = 2;
            this.deleteButton.Text = "Delete";
            this.deleteButton.UseVisualStyleBackColor = true;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // updateButton
            // 
            this.updateButton.Location = new System.Drawing.Point(96, 13);
            this.updateButton.Name = "updateButton";
            this.updateButton.Size = new System.Drawing.Size(75, 23);
            this.updateButton.TabIndex = 1;
            this.updateButton.Text = "Save";
            this.updateButton.UseVisualStyleBackColor = true;
            this.updateButton.Click += new System.EventHandler(this.updateButton_Click);
            // 
            // newNote
            // 
            this.newNote.Location = new System.Drawing.Point(15, 13);
            this.newNote.Name = "newNote";
            this.newNote.Size = new System.Drawing.Size(75, 23);
            this.newNote.TabIndex = 0;
            this.newNote.Text = "+";
            this.newNote.UseVisualStyleBackColor = true;
            this.newNote.Click += new System.EventHandler(this.newNote_Click);
            // 
            // SyncNotesListButton
            // 
            this.SyncNotesListButton.Location = new System.Drawing.Point(15, 40);
            this.SyncNotesListButton.Name = "SyncNotesListButton";
            this.SyncNotesListButton.Size = new System.Drawing.Size(124, 23);
            this.SyncNotesListButton.TabIndex = 4;
            this.SyncNotesListButton.Text = "Sync Notes List";
            this.SyncNotesListButton.UseVisualStyleBackColor = true;
            this.SyncNotesListButton.Click += new System.EventHandler(this.SyncNotesListButton_Click);
            // 
            // forceSyncButton
            // 
            this.forceSyncButton.Location = new System.Drawing.Point(145, 40);
            this.forceSyncButton.Name = "forceSyncButton";
            this.forceSyncButton.Size = new System.Drawing.Size(124, 23);
            this.forceSyncButton.TabIndex = 5;
            this.forceSyncButton.Text = "Force Sync";
            this.forceSyncButton.UseVisualStyleBackColor = true;
            this.forceSyncButton.Click += new System.EventHandler(this.forceSyncButton_Click);
            // 
            // MyNote
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(685, 452);
            this.Controls.Add(this.splitContainer1);
            this.Name = "MyNote";
            this.Text = "MyNote";
            this.Load += new System.EventHandler(this.MyNote_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button updateButton;
        private System.Windows.Forms.Button newNote;
        private System.Windows.Forms.Button MailToButton;
        private System.Windows.Forms.Button SyncNotesListButton;
        private System.Windows.Forms.Button forceSyncButton;
    }
}


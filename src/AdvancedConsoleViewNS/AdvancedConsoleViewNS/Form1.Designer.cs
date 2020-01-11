namespace AdvancedConsoleViewNS
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
            this.advancedConsoleView1 = new AdvancedConsoleViewNS.AdvancedConsoleView(this);
            this.SuspendLayout();
            // 
            // vScrollBar1
            // 
            this.vScrollBar1.Dock = System.Windows.Forms.DockStyle.Right;
            this.vScrollBar1.Location = new System.Drawing.Point(729, 0);
            this.vScrollBar1.Name = "vScrollBar1";
            this.vScrollBar1.Size = new System.Drawing.Size(17, 406);
            this.vScrollBar1.TabIndex = 1;
            // 
            // advancedConsoleView1
            // 
            this.advancedConsoleView1.BackColor = System.Drawing.Color.Black;
            this.advancedConsoleView1.ConsoleStrollBar = null;
            this.advancedConsoleView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.advancedConsoleView1.IsPrompting = true;
            this.advancedConsoleView1.Location = new System.Drawing.Point(0, 0);
            this.advancedConsoleView1.Name = "advancedConsoleView1";
            this.advancedConsoleView1.PromptInfo = null;
            this.advancedConsoleView1.Size = new System.Drawing.Size(729, 406);
            this.advancedConsoleView1.TabIndex = 0;
       
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(746, 406);
            this.Controls.Add(this.advancedConsoleView1);
            this.Controls.Add(this.vScrollBar1);
            this.Name = "Form1";
            this.Opacity = 0.9D;
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private AdvancedConsoleView advancedConsoleView1;
        private System.Windows.Forms.VScrollBar vScrollBar1;
    }
}


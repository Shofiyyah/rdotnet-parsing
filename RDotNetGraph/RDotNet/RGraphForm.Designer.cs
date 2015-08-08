namespace RDotNet
{
  partial class RGraphForm
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
      this.menuStrip2 = new System.Windows.Forms.MenuStrip();
      this.graphitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.showInWindowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.bugInCloseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.RPanel1 = new System.Windows.Forms.Panel();
      this.disposeUsingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.menuStrip2.SuspendLayout();
      this.SuspendLayout();
      // 
      // menuStrip2
      // 
      this.menuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.graphitToolStripMenuItem,
            this.showInWindowToolStripMenuItem,
            this.bugInCloseToolStripMenuItem,
            this.disposeUsingToolStripMenuItem});
      this.menuStrip2.Location = new System.Drawing.Point(0, 0);
      this.menuStrip2.Name = "menuStrip2";
      this.menuStrip2.Size = new System.Drawing.Size(594, 24);
      this.menuStrip2.TabIndex = 1;
      this.menuStrip2.Text = "menuStrip2";
      // 
      // graphitToolStripMenuItem
      // 
      this.graphitToolStripMenuItem.Name = "graphitToolStripMenuItem";
      this.graphitToolStripMenuItem.Size = new System.Drawing.Size(108, 20);
      this.graphitToolStripMenuItem.Text = "Show Embedded";
      this.graphitToolStripMenuItem.Click += new System.EventHandler(this.ShowRGraphClick);
      // 
      // showInWindowToolStripMenuItem
      // 
      this.showInWindowToolStripMenuItem.Name = "showInWindowToolStripMenuItem";
      this.showInWindowToolStripMenuItem.Size = new System.Drawing.Size(108, 20);
      this.showInWindowToolStripMenuItem.Text = "Show In Window";
      this.showInWindowToolStripMenuItem.Click += new System.EventHandler(this.ShowInWindowToolStripMenuItemClick);
      // 
      // bugInCloseToolStripMenuItem
      // 
      this.bugInCloseToolStripMenuItem.Name = "bugInCloseToolStripMenuItem";
      this.bugInCloseToolStripMenuItem.Size = new System.Drawing.Size(85, 20);
      this.bugInCloseToolStripMenuItem.Text = "Bug In Close";
      this.bugInCloseToolStripMenuItem.Click += new System.EventHandler(this.BugInCloseToolStripMenuItemClick);
      // 
      // RPanel1
      // 
      this.RPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.RPanel1.Location = new System.Drawing.Point(0, 24);
      this.RPanel1.Name = "RPanel1";
      this.RPanel1.Size = new System.Drawing.Size(594, 342);
      this.RPanel1.TabIndex = 2;
      // 
      // disposeUsingToolStripMenuItem
      // 
      this.disposeUsingToolStripMenuItem.Name = "disposeUsingToolStripMenuItem";
      this.disposeUsingToolStripMenuItem.Size = new System.Drawing.Size(90, 20);
      this.disposeUsingToolStripMenuItem.Text = "DisposeUsing";
      this.disposeUsingToolStripMenuItem.Click += new System.EventHandler(this.DisposeUsingToolStripMenuItemClick);
      // 
      // RGraphForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(594, 366);
      this.Controls.Add(this.RPanel1);
      this.Controls.Add(this.menuStrip2);
      this.Name = "RGraphForm";
      this.Text = "RDotNet Graph";
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1FormClosed);
      this.menuStrip2.ResumeLayout(false);
      this.menuStrip2.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.MenuStrip menuStrip2;
    private System.Windows.Forms.ToolStripMenuItem graphitToolStripMenuItem;
    private System.Windows.Forms.Panel RPanel1;
    private System.Windows.Forms.ToolStripMenuItem bugInCloseToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem showInWindowToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem disposeUsingToolStripMenuItem;
  }
}


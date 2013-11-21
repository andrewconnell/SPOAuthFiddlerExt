namespace SPOAuthFiddlerExt {
  partial class SPOAuthRequestControl {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      this.txtContext = new System.Windows.Forms.TextBox();
      this.dataGridView1 = new System.Windows.Forms.DataGridView();
      this.colKey = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.colValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
      this.SuspendLayout();
      // 
      // txtContext
      // 
      this.txtContext.Dock = System.Windows.Forms.DockStyle.Top;
      this.txtContext.Location = new System.Drawing.Point(0, 0);
      this.txtContext.Multiline = true;
      this.txtContext.Name = "txtContext";
      this.txtContext.Size = new System.Drawing.Size(687, 137);
      this.txtContext.TabIndex = 0;
      // 
      // dataGridView1
      // 
      this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colKey,
            this.colValue});
      this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dataGridView1.Location = new System.Drawing.Point(0, 137);
      this.dataGridView1.Name = "dataGridView1";
      this.dataGridView1.Size = new System.Drawing.Size(687, 319);
      this.dataGridView1.TabIndex = 3;
      // 
      // colKey
      // 
      this.colKey.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.colKey.HeaderText = "Key";
      this.colKey.Name = "colKey";
      this.colKey.Width = 48;
      // 
      // colValue
      // 
      this.colValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
      this.colValue.HeaderText = "Value";
      this.colValue.Name = "colValue";
      // 
      // SPOAuthRequestControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.AutoSize = true;
      this.Controls.Add(this.dataGridView1);
      this.Controls.Add(this.txtContext);
      this.Name = "SPOAuthRequestControl";
      this.Size = new System.Drawing.Size(687, 456);
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox txtContext;
    private System.Windows.Forms.DataGridView dataGridView1;
    private System.Windows.Forms.DataGridViewTextBoxColumn colKey;
    private System.Windows.Forms.DataGridViewTextBoxColumn colValue;


  }
}

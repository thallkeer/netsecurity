namespace ClientForLaba
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnSend = new System.Windows.Forms.Button();
            this.ConsoleLog = new System.Windows.Forms.RichTextBox();
            this.openFD = new System.Windows.Forms.OpenFileDialog();
            this.btnSendFile = new System.Windows.Forms.Button();
            this.btnSendKey = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnSelect
            // 
            this.btnSelect.Location = new System.Drawing.Point(0, 41);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(75, 23);
            this.btnSelect.TabIndex = 0;
            this.btnSelect.Text = "Select file";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(81, 41);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 0;
            this.btnSend.Text = "Send init";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // ConsoleLog
            // 
            this.ConsoleLog.Dock = System.Windows.Forms.DockStyle.Right;
            this.ConsoleLog.Location = new System.Drawing.Point(327, 0);
            this.ConsoleLog.Name = "ConsoleLog";
            this.ConsoleLog.Size = new System.Drawing.Size(191, 261);
            this.ConsoleLog.TabIndex = 1;
            this.ConsoleLog.Text = "";
            // 
            // openFD
            // 
            this.openFD.FileName = "openFileDialog1";
          
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(518, 261);
            this.Controls.Add(this.ConsoleLog);
            this.Controls.Add(this.btnSendKey);
            this.Controls.Add(this.btnSendFile);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.btnSelect);
            this.Name = "Form1";
            this.Text = "Client";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.RichTextBox ConsoleLog;
        private System.Windows.Forms.OpenFileDialog openFD;
        private System.Windows.Forms.Button btnSendFile;
        private System.Windows.Forms.Button btnSendKey;
    }
}


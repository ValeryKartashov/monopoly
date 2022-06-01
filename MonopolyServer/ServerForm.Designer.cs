namespace MonopolyServer
{
    partial class ServerForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ServerForm));
            this.tbLog = new System.Windows.Forms.TextBox();
            this.tsServerControl = new System.Windows.Forms.ToolStrip();
            this.btnTurnOn = new System.Windows.Forms.ToolStripButton();
            this.btnTurnOff = new System.Windows.Forms.ToolStripButton();
            this.tsServerControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbLog
            // 
            this.tbLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbLog.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.tbLog.Location = new System.Drawing.Point(0, 27);
            this.tbLog.Multiline = true;
            this.tbLog.Name = "tbLog";
            this.tbLog.ReadOnly = true;
            this.tbLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbLog.Size = new System.Drawing.Size(675, 397);
            this.tbLog.TabIndex = 0;
            // 
            // tsServerControl
            // 
            this.tsServerControl.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.tsServerControl.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tsServerControl.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsServerControl.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnTurnOn,
            this.btnTurnOff});
            this.tsServerControl.Location = new System.Drawing.Point(0, 0);
            this.tsServerControl.Name = "tsServerControl";
            this.tsServerControl.Size = new System.Drawing.Size(675, 27);
            this.tsServerControl.TabIndex = 5;
            this.tsServerControl.Text = "toolStrip1";
            // 
            // btnTurnOn
            // 
            this.btnTurnOn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnTurnOn.Image = ((System.Drawing.Image)(resources.GetObject("btnTurnOn.Image")));
            this.btnTurnOn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnTurnOn.Name = "btnTurnOn";
            this.btnTurnOn.Size = new System.Drawing.Size(80, 24);
            this.btnTurnOn.Text = "Включить";
            this.btnTurnOn.Click += new System.EventHandler(this.btnTurnOn_Click);
            // 
            // btnTurnOff
            // 
            this.btnTurnOff.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnTurnOff.AutoToolTip = false;
            this.btnTurnOff.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnTurnOff.Image = ((System.Drawing.Image)(resources.GetObject("btnTurnOff.Image")));
            this.btnTurnOff.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnTurnOff.Name = "btnTurnOff";
            this.btnTurnOff.Size = new System.Drawing.Size(298, 24);
            this.btnTurnOff.Text = "Закрыть программу / Выключить сервер";
            this.btnTurnOff.Click += new System.EventHandler(this.btnTurnOff_Click);
            // 
            // ServerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(675, 424);
            this.ControlBox = false;
            this.Controls.Add(this.tbLog);
            this.Controls.Add(this.tsServerControl);
            this.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ServerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Сервер. Состояние: включен / Сервер. Состояние: выключен";
            this.tsServerControl.ResumeLayout(false);
            this.tsServerControl.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.TextBox tbLog;
        private System.Windows.Forms.ToolStrip tsServerControl;
        private System.Windows.Forms.ToolStripButton btnTurnOn;
        private System.Windows.Forms.ToolStripButton btnTurnOff;
    }
}


namespace Monopoly_KartashovAS41
{
    partial class Connection
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Connection));
            this.port_textbox = new System.Windows.Forms.TextBox();
            this.ip_textbox = new System.Windows.Forms.TextBox();
            this.connect_button = new System.Windows.Forms.Button();
            this.IPLabel = new System.Windows.Forms.Label();
            this.portLabel = new System.Windows.Forms.Label();
            this.redPlayer_radioButton = new System.Windows.Forms.RadioButton();
            this.bluePlayer_radioButton = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.returnBtn = new System.Windows.Forms.Button();
            this.chooseRedPlayerBtn = new System.Windows.Forms.Button();
            this.chooseBluePlayerBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // port_textbox
            // 
            this.port_textbox.BackColor = System.Drawing.Color.White;
            this.port_textbox.ForeColor = System.Drawing.Color.White;
            this.port_textbox.Location = new System.Drawing.Point(299, 191);
            this.port_textbox.MaxLength = 15;
            this.port_textbox.Name = "port_textbox";
            this.port_textbox.Size = new System.Drawing.Size(144, 27);
            this.port_textbox.TabIndex = 15;
            // 
            // ip_textbox
            // 
            this.ip_textbox.BackColor = System.Drawing.Color.White;
            this.ip_textbox.ForeColor = System.Drawing.Color.White;
            this.ip_textbox.Location = new System.Drawing.Point(144, 191);
            this.ip_textbox.MaxLength = 15;
            this.ip_textbox.Name = "ip_textbox";
            this.ip_textbox.Size = new System.Drawing.Size(149, 27);
            this.ip_textbox.TabIndex = 14;
            // 
            // connect_button
            // 
            this.connect_button.BackColor = System.Drawing.Color.Transparent;
            this.connect_button.Cursor = System.Windows.Forms.Cursors.Hand;
            this.connect_button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.connect_button.ForeColor = System.Drawing.Color.White;
            this.connect_button.Location = new System.Drawing.Point(299, 351);
            this.connect_button.Name = "connect_button";
            this.connect_button.Size = new System.Drawing.Size(144, 29);
            this.connect_button.TabIndex = 13;
            this.connect_button.Text = "Подключиться";
            this.connect_button.UseVisualStyleBackColor = false;
            this.connect_button.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // IPLabel
            // 
            this.IPLabel.AutoSize = true;
            this.IPLabel.BackColor = System.Drawing.Color.Transparent;
            this.IPLabel.Location = new System.Drawing.Point(140, 168);
            this.IPLabel.Name = "IPLabel";
            this.IPLabel.Size = new System.Drawing.Size(129, 20);
            this.IPLabel.TabIndex = 11;
            this.IPLabel.Text = "IP адрес сервера:";
            // 
            // portLabel
            // 
            this.portLabel.AutoSize = true;
            this.portLabel.BackColor = System.Drawing.Color.Transparent;
            this.portLabel.Location = new System.Drawing.Point(295, 168);
            this.portLabel.Name = "portLabel";
            this.portLabel.Size = new System.Drawing.Size(108, 20);
            this.portLabel.TabIndex = 10;
            this.portLabel.Text = "Порт сервера:";
            this.portLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // redPlayer_radioButton
            // 
            this.redPlayer_radioButton.AutoSize = true;
            this.redPlayer_radioButton.BackColor = System.Drawing.Color.Transparent;
            this.redPlayer_radioButton.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.redPlayer_radioButton.Enabled = false;
            this.redPlayer_radioButton.Location = new System.Drawing.Point(204, 245);
            this.redPlayer_radioButton.Name = "redPlayer_radioButton";
            this.redPlayer_radioButton.Size = new System.Drawing.Size(89, 24);
            this.redPlayer_radioButton.TabIndex = 16;
            this.redPlayer_radioButton.TabStop = true;
            this.redPlayer_radioButton.Text = "Красный";
            this.redPlayer_radioButton.UseVisualStyleBackColor = false;
            this.redPlayer_radioButton.CheckedChanged += new System.EventHandler(this.redPlayer_radioButton_CheckedChanged);
            // 
            // bluePlayer_radioButton
            // 
            this.bluePlayer_radioButton.AutoSize = true;
            this.bluePlayer_radioButton.BackColor = System.Drawing.Color.Transparent;
            this.bluePlayer_radioButton.Enabled = false;
            this.bluePlayer_radioButton.Location = new System.Drawing.Point(299, 245);
            this.bluePlayer_radioButton.Name = "bluePlayer_radioButton";
            this.bluePlayer_radioButton.Size = new System.Drawing.Size(72, 24);
            this.bluePlayer_radioButton.TabIndex = 17;
            this.bluePlayer_radioButton.TabStop = true;
            this.bluePlayer_radioButton.Text = "Синий";
            this.bluePlayer_radioButton.UseVisualStyleBackColor = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(239, 222);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(106, 20);
            this.label1.TabIndex = 18;
            this.label1.Text = "Цвет фигурки:";
            // 
            // returnBtn
            // 
            this.returnBtn.BackColor = System.Drawing.Color.Transparent;
            this.returnBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.returnBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.returnBtn.ForeColor = System.Drawing.Color.White;
            this.returnBtn.Location = new System.Drawing.Point(144, 351);
            this.returnBtn.Name = "returnBtn";
            this.returnBtn.Size = new System.Drawing.Size(149, 29);
            this.returnBtn.TabIndex = 19;
            this.returnBtn.Text = "Назад";
            this.returnBtn.UseVisualStyleBackColor = false;
            this.returnBtn.Click += new System.EventHandler(this.returnBtn_Click);
            // 
            // chooseRedPlayerBtn
            // 
            this.chooseRedPlayerBtn.BackColor = System.Drawing.Color.Transparent;
            this.chooseRedPlayerBtn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("chooseRedPlayerBtn.BackgroundImage")));
            this.chooseRedPlayerBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chooseRedPlayerBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chooseRedPlayerBtn.Location = new System.Drawing.Point(223, 275);
            this.chooseRedPlayerBtn.Name = "chooseRedPlayerBtn";
            this.chooseRedPlayerBtn.Size = new System.Drawing.Size(70, 70);
            this.chooseRedPlayerBtn.TabIndex = 22;
            this.chooseRedPlayerBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.chooseRedPlayerBtn.UseVisualStyleBackColor = false;
            this.chooseRedPlayerBtn.Click += new System.EventHandler(this.chooseRedPlayerBtn_Click);
            // 
            // chooseBluePlayerBtn
            // 
            this.chooseBluePlayerBtn.BackColor = System.Drawing.Color.Transparent;
            this.chooseBluePlayerBtn.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("chooseBluePlayerBtn.BackgroundImage")));
            this.chooseBluePlayerBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.chooseBluePlayerBtn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chooseBluePlayerBtn.Location = new System.Drawing.Point(299, 275);
            this.chooseBluePlayerBtn.Name = "chooseBluePlayerBtn";
            this.chooseBluePlayerBtn.Size = new System.Drawing.Size(70, 70);
            this.chooseBluePlayerBtn.TabIndex = 23;
            this.chooseBluePlayerBtn.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.chooseBluePlayerBtn.UseVisualStyleBackColor = false;
            this.chooseBluePlayerBtn.Click += new System.EventHandler(this.chooseBluePlayerBtn_Click);
            // 
            // Connection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(587, 398);
            this.ControlBox = false;
            this.Controls.Add(this.chooseBluePlayerBtn);
            this.Controls.Add(this.chooseRedPlayerBtn);
            this.Controls.Add(this.returnBtn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bluePlayer_radioButton);
            this.Controls.Add(this.redPlayer_radioButton);
            this.Controls.Add(this.port_textbox);
            this.Controls.Add(this.ip_textbox);
            this.Controls.Add(this.connect_button);
            this.Controls.Add(this.IPLabel);
            this.Controls.Add(this.portLabel);
            this.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.Name = "Connection";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Подключение";
            this.Load += new System.EventHandler(this.Connection_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox port_textbox;
        private System.Windows.Forms.TextBox ip_textbox;
        private System.Windows.Forms.Button connect_button;
        private System.Windows.Forms.Label IPLabel;
        private System.Windows.Forms.Label portLabel;
        private System.Windows.Forms.RadioButton redPlayer_radioButton;
        private System.Windows.Forms.RadioButton bluePlayer_radioButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button returnBtn;
        private System.Windows.Forms.Button chooseRedPlayerBtn;
        private System.Windows.Forms.Button chooseBluePlayerBtn;
    }
}
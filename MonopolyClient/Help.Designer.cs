namespace MonopolyClient
{
    partial class Help
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Help));
            this.helpLabel = new System.Windows.Forms.Label();
            this.openRules_button = new System.Windows.Forms.Button();
            this.returnBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // helpLabel
            // 
            this.helpLabel.AutoSize = true;
            this.helpLabel.BackColor = System.Drawing.Color.Transparent;
            this.helpLabel.ForeColor = System.Drawing.Color.White;
            this.helpLabel.Location = new System.Drawing.Point(140, 177);
            this.helpLabel.Name = "helpLabel";
            this.helpLabel.Size = new System.Drawing.Size(286, 80);
            this.helpLabel.TabIndex = 0;
            this.helpLabel.Text = "Работу выполнил студент группы АС-41\r\nКарташов Валерий Сергеевич\r\n\r\nПравила игры " +
    "(ссылка активна):";
            this.helpLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // openRules_button
            // 
            this.openRules_button.BackColor = System.Drawing.Color.Transparent;
            this.openRules_button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.openRules_button.Cursor = System.Windows.Forms.Cursors.Hand;
            this.openRules_button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.openRules_button.ForeColor = System.Drawing.Color.White;
            this.openRules_button.Location = new System.Drawing.Point(120, 260);
            this.openRules_button.Name = "openRules_button";
            this.openRules_button.Size = new System.Drawing.Size(331, 29);
            this.openRules_button.TabIndex = 2;
            this.openRules_button.Text = "https://www.mosigra.ru/monopoliy_rossiy/rules/";
            this.openRules_button.UseVisualStyleBackColor = true;
            this.openRules_button.Click += new System.EventHandler(this.OpenRules_button_Click);
            // 
            // returnBtn
            // 
            this.returnBtn.BackColor = System.Drawing.Color.Transparent;
            this.returnBtn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.returnBtn.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.returnBtn.ForeColor = System.Drawing.Color.White;
            this.returnBtn.Location = new System.Drawing.Point(219, 336);
            this.returnBtn.Name = "returnBtn";
            this.returnBtn.Size = new System.Drawing.Size(135, 35);
            this.returnBtn.TabIndex = 3;
            this.returnBtn.Text = "Назад";
            this.returnBtn.UseVisualStyleBackColor = false;
            this.returnBtn.Click += new System.EventHandler(this.ReturnBtn_Click);
            // 
            // Help
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(587, 398);
            this.Controls.Add(this.returnBtn);
            this.Controls.Add(this.openRules_button);
            this.Controls.Add(this.helpLabel);
            this.Font = new System.Drawing.Font("Segoe UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Help";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Помощь и информация";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label helpLabel;
        private System.Windows.Forms.Button openRules_button;
        private System.Windows.Forms.Button returnBtn;
    }
}
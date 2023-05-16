namespace MegaMemory
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
            this.playerOneNameTextBox = new System.Windows.Forms.TextBox();
            this.playerTwoNameTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // playerOneNameTextBox
            // 
            this.playerOneNameTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.playerOneNameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.playerOneNameTextBox.Enabled = false;
            this.playerOneNameTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 32F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.playerOneNameTextBox.Location = new System.Drawing.Point(79, 143);
            this.playerOneNameTextBox.MaxLength = 10;
            this.playerOneNameTextBox.Name = "playerOneNameTextBox";
            this.playerOneNameTextBox.Size = new System.Drawing.Size(448, 37);
            this.playerOneNameTextBox.TabIndex = 0;
            this.playerOneNameTextBox.Visible = false;
            this.playerOneNameTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.setPlayerOneName);
            this.playerOneNameTextBox.Leave += new System.EventHandler(this.playerOneNameTextBox_Leave);
            // 
            // playerTwoNameTextBox
            // 
            this.playerTwoNameTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.playerTwoNameTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.playerTwoNameTextBox.Enabled = false;
            this.playerTwoNameTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 32F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.playerTwoNameTextBox.Location = new System.Drawing.Point(79, 422);
            this.playerTwoNameTextBox.MaxLength = 10;
            this.playerTwoNameTextBox.Name = "playerTwoNameTextBox";
            this.playerTwoNameTextBox.Size = new System.Drawing.Size(448, 37);
            this.playerTwoNameTextBox.TabIndex = 1;
            this.playerTwoNameTextBox.Visible = false;
            this.playerTwoNameTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.setPlayerTwoName);
            this.playerTwoNameTextBox.Leave += new System.EventHandler(this.playerTwoNameTextBox_Leave);
            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(1274, 703);
            this.Controls.Add(this.playerTwoNameTextBox);
            this.Controls.Add(this.playerOneNameTextBox);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel, ((byte)(0)));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1296, 759);
            this.MinimumSize = new System.Drawing.Size(1296, 759);
            this.Name = "Form1";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ITPR5.518 Introduction to Object Oriented Programming, Assignment 2019 S1 - Delux" +
    "e Card Game - Mega Memory, by Cliff Earl";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox playerOneNameTextBox;
        private System.Windows.Forms.TextBox playerTwoNameTextBox;
    }
}


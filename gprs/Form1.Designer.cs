﻿﻿﻿﻿﻿﻿﻿﻿
namespace gprs
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);

            _mwCaptureWrapperPro.Dispose();
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            textBox1 = new System.Windows.Forms.TextBox();
            pictureBox1 = new System.Windows.Forms.PictureBox();
            textBox2 = new System.Windows.Forms.TextBox();
            radioButton1 = new System.Windows.Forms.RadioButton();
            groupBox1 = new System.Windows.Forms.GroupBox();
            radioButton3 = new System.Windows.Forms.RadioButton();
            radioButton2 = new System.Windows.Forms.RadioButton();
            groupBox3 = new System.Windows.Forms.GroupBox();
            radioButton6 = new System.Windows.Forms.RadioButton();
            radioButton4 = new System.Windows.Forms.RadioButton();
            radioButton12 = new System.Windows.Forms.RadioButton();
            radioButton11 = new System.Windows.Forms.RadioButton();
            radioButton9 = new System.Windows.Forms.RadioButton();
            radioButton8 = new System.Windows.Forms.RadioButton();
            radioButton7 = new System.Windows.Forms.RadioButton();
            radioButton5 = new System.Windows.Forms.RadioButton();
            port_cbb = new System.Windows.Forms.ComboBox();
            groupBox4 = new System.Windows.Forms.GroupBox();
            open_btn = new System.Windows.Forms.Button();
            folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            folderBrowserDialog2 = new System.Windows.Forms.FolderBrowserDialog();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            groupBox1.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox4.SuspendLayout();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.Font = new System.Drawing.Font("Microsoft YaHei UI", 16F);
            textBox1.Location = new System.Drawing.Point(17, 338);
            textBox1.Name = "textBox1";
            textBox1.Size = new System.Drawing.Size(587, 35);
            textBox1.TabIndex = 1;
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new System.Drawing.Point(284, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new System.Drawing.Size(320, 320);
            pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 2;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click;
            // 
            // textBox2
            // 
            textBox2.Font = new System.Drawing.Font("Microsoft YaHei UI", 16F);
            textBox2.Location = new System.Drawing.Point(18, 379);
            textBox2.Name = "textBox2";
            textBox2.Size = new System.Drawing.Size(586, 35);
            textBox2.TabIndex = 3;
            textBox2.TextChanged += textBox2_TextChanged;
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Location = new System.Drawing.Point(6, 22);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new System.Drawing.Size(74, 21);
            radioButton1.TabIndex = 6;
            radioButton1.TabStop = true;
            radioButton1.Text = "穿越火线";
            radioButton1.UseVisualStyleBackColor = true;
            radioButton1.CheckedChanged += radioButton1_CheckedChanged;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(radioButton3);
            groupBox1.Controls.Add(radioButton2);
            groupBox1.Controls.Add(radioButton1);
            groupBox1.Location = new System.Drawing.Point(11, 8);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(90, 184);
            groupBox1.TabIndex = 7;
            groupBox1.TabStop = false;
            groupBox1.Text = "功能选择";
            groupBox1.Enter += groupBox1_Enter;
            // 
            // radioButton3
            // 
            radioButton3.AutoSize = true;
            radioButton3.Location = new System.Drawing.Point(6, 78);
            radioButton3.Name = "radioButton3";
            radioButton3.Size = new System.Drawing.Size(38, 21);
            radioButton3.TabIndex = 12;
            radioButton3.Text = "cs";
            radioButton3.UseVisualStyleBackColor = true;
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.Location = new System.Drawing.Point(6, 51);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new System.Drawing.Size(74, 21);
            radioButton2.TabIndex = 11;
            radioButton2.Text = "无畏契约";
            radioButton2.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(radioButton6);
            groupBox3.Controls.Add(radioButton4);
            groupBox3.Controls.Add(radioButton12);
            groupBox3.Controls.Add(radioButton11);
            groupBox3.Controls.Add(radioButton9);
            groupBox3.Controls.Add(radioButton8);
            groupBox3.Controls.Add(radioButton7);
            groupBox3.Controls.Add(radioButton5);
            groupBox3.Location = new System.Drawing.Point(109, 8);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new System.Drawing.Size(167, 184);
            groupBox3.TabIndex = 10;
            groupBox3.TabStop = false;
            groupBox3.Text = "枪械参数选择";
            // 
            // radioButton6
            // 
            radioButton6.AutoSize = true;
            radioButton6.Location = new System.Drawing.Point(90, 154);
            radioButton6.Name = "radioButton6";
            radioButton6.Size = new System.Drawing.Size(79, 21);
            radioButton6.TabIndex = 15;
            radioButton6.TabStop = true;
            radioButton6.Text = "手枪-身子";
            radioButton6.UseVisualStyleBackColor = true;
            radioButton6.CheckedChanged += radioButton6_CheckedChanged;
            // 
            // radioButton4
            // 
            radioButton4.AutoSize = true;
            radioButton4.Location = new System.Drawing.Point(6, 73);
            radioButton4.Name = "radioButton4";
            radioButton4.Size = new System.Drawing.Size(67, 21);
            radioButton4.TabIndex = 14;
            radioButton4.TabStop = true;
            radioButton4.Text = "点射-头";
            radioButton4.UseVisualStyleBackColor = true;
            radioButton4.CheckedChanged += radioButton4_CheckedChanged;
            // 
            // radioButton12
            // 
            radioButton12.AutoSize = true;
            radioButton12.Location = new System.Drawing.Point(6, 154);
            radioButton12.Name = "radioButton12";
            radioButton12.Size = new System.Drawing.Size(79, 21);
            radioButton12.TabIndex = 13;
            radioButton12.TabStop = true;
            radioButton12.Text = "连发-身子";
            radioButton12.UseVisualStyleBackColor = true;
            radioButton12.CheckedChanged += radioButton12_CheckedChanged;
            // 
            // radioButton11
            // 
            radioButton11.AutoSize = true;
            radioButton11.Location = new System.Drawing.Point(6, 22);
            radioButton11.Name = "radioButton11";
            radioButton11.Size = new System.Drawing.Size(55, 21);
            radioButton11.TabIndex = 12;
            radioButton11.TabStop = true;
            radioButton11.Text = "狙-头";
            radioButton11.UseVisualStyleBackColor = true;
            radioButton11.CheckedChanged += radioButton11_CheckedChanged;
            // 
            // radioButton9
            // 
            radioButton9.AutoSize = true;
            radioButton9.Location = new System.Drawing.Point(90, 127);
            radioButton9.Name = "radioButton9";
            radioButton9.Size = new System.Drawing.Size(67, 21);
            radioButton9.TabIndex = 11;
            radioButton9.TabStop = true;
            radioButton9.Text = "手枪-头";
            radioButton9.UseVisualStyleBackColor = true;
            radioButton9.CheckedChanged += radioButton9_CheckedChanged;
            // 
            // radioButton8
            // 
            radioButton8.AutoSize = true;
            radioButton8.Location = new System.Drawing.Point(6, 127);
            radioButton8.Name = "radioButton8";
            radioButton8.Size = new System.Drawing.Size(67, 21);
            radioButton8.TabIndex = 3;
            radioButton8.TabStop = true;
            radioButton8.Text = "连发-头";
            radioButton8.UseVisualStyleBackColor = true;
            radioButton8.CheckedChanged += radioButton8_CheckedChanged;
            // 
            // radioButton7
            // 
            radioButton7.AutoSize = true;
            radioButton7.Location = new System.Drawing.Point(6, 100);
            radioButton7.Name = "radioButton7";
            radioButton7.Size = new System.Drawing.Size(79, 21);
            radioButton7.TabIndex = 2;
            radioButton7.TabStop = true;
            radioButton7.Text = "点射-身子";
            radioButton7.UseVisualStyleBackColor = true;
            radioButton7.CheckedChanged += radioButton7_CheckedChanged;
            // 
            // radioButton5
            // 
            radioButton5.AutoSize = true;
            radioButton5.Location = new System.Drawing.Point(6, 46);
            radioButton5.Name = "radioButton5";
            radioButton5.Size = new System.Drawing.Size(67, 21);
            radioButton5.TabIndex = 0;
            radioButton5.TabStop = true;
            radioButton5.Text = "狙-身子";
            radioButton5.UseVisualStyleBackColor = true;
            radioButton5.CheckedChanged += radioButton5_CheckedChanged;
            // 
            // port_cbb
            // 
            port_cbb.FormattingEnabled = true;
            port_cbb.Location = new System.Drawing.Point(6, 22);
            port_cbb.Name = "port_cbb";
            port_cbb.Size = new System.Drawing.Size(121, 25);
            port_cbb.TabIndex = 11;
            port_cbb.DropDown += ComboBox_DropDown;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(open_btn);
            groupBox4.Controls.Add(port_cbb);
            groupBox4.Location = new System.Drawing.Point(12, 264);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new System.Drawing.Size(266, 68);
            groupBox4.TabIndex = 12;
            groupBox4.TabStop = false;
            groupBox4.Text = "groupBox4";
            // 
            // open_btn
            // 
            open_btn.Location = new System.Drawing.Point(160, 24);
            open_btn.Name = "open_btn";
            open_btn.Size = new System.Drawing.Size(75, 23);
            open_btn.TabIndex = 13;
            open_btn.Text = "打开串口";
            open_btn.UseVisualStyleBackColor = true;
            open_btn.Click += open_btn_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(612, 416);
            Controls.Add(groupBox4);
            Controls.Add(groupBox3);
            Controls.Add(groupBox1);
            Controls.Add(textBox2);
            Controls.Add(pictureBox1);
            Controls.Add(textBox1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            groupBox4.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton radioButton7;
        private System.Windows.Forms.RadioButton radioButton5;
        private System.Windows.Forms.RadioButton radioButton8;
        private System.Windows.Forms.RadioButton radioButton9;
        private System.Windows.Forms.RadioButton radioButton11;
        private System.Windows.Forms.RadioButton radioButton12;
        private System.Windows.Forms.ComboBox port_cbb;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button open_btn;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog2;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.RadioButton radioButton4;
        private System.Windows.Forms.RadioButton radioButton6;

    }
}


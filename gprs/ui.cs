﻿﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace gprs
{
    public partial class Form1 : Form
    {
        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void radioButton11_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton11.Checked)//狙-头
            {
                fireZhu = 1; fireID = 1;
            }
        }

        private void radioButton5_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton5.Checked)//狙-身子
            {
                fireZhu = 2; fireID = 1;
            }
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton4.Checked)//点射-头
            {
                fireZhu = 3; fireID = 1;
            }
        }

        private void radioButton7_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton7.Checked)//点射-身子
            {
                fireZhu = 4; fireID = 1;
            }
        }

        private void radioButton8_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton8.Checked)//连发-头
            {
                fireZhu = 5; fireID = 1;
            }
        }

        private void radioButton12_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton12.Checked)//连发-身子
            {
                fireZhu = 6; fireID = 1;
            }
        }

        private void radioButton9_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton9.Checked)//手枪-头
            {
                fireFu = 1; fireID = 2;
            }
        }


        private void radioButton6_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton6.Checked)//手枪-身子
            {
                fireFu = 2; fireID = 2;
            }
        }

        private void ComboBox_DropDown(object sender, System.EventArgs e)
        {
            GetComList();
        }

    }
}

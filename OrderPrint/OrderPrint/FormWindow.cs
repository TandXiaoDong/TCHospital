using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace OrderPrint
{
    public partial class FormWindow : Form
    {
        public string window = "1";

        public FormWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            window = "1";
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            window = "2";
            this.Close();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            window = "3";
            this.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            window = "4";
            this.Close();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            window = "5";
            this.Close();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            window = "6";
            this.Close();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            window = "7";
            this.Close();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            window = "8";
            this.Close();
        }
    }
}
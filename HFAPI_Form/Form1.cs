using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HFAPI;

namespace HFAPI_Form
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            User.Connect(txtUsername.Text, txtPassword.Text);
            richTextBox1.AppendText("Posts : " +User.Getpostcount().ToString() + Environment.NewLine);
            richTextBox1.AppendText("Refred members : "+ User.GetMemberReferedcount().ToString() + Environment.NewLine);
            richTextBox1.AppendText("Repuation : " +User.GetReputation().ToString() + Environment.NewLine);
            richTextBox1.AppendText("Date d'insctipion : " + User.GetInscriptionDate().ToString() + Environment.NewLine);
            richTextBox1.AppendText("Ratio : " + User.GetRatio() + Environment.NewLine);
            richTextBox1.AppendText("Nombres d'anciens pseudo : " + User.GetUsernameChange() + Environment.NewLine);
            richTextBox1.AppendText("Merci : " + User.GetMerci() + Environment.NewLine);
        }
    }
}

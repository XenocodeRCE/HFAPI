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
            lblPosts.Text += User.PostCount.ToString();
            lblRefered.Text += User.ReferencedMembers;
            lblReputation.Text += User.Reputation;
            lblInscription.Text += User.InscriptionDate;
            lblRatio.Text += User.Ratio;
            lblUsername.Text = User.Username;
            lblUsernameChanges.Text += User.UsernameChange;
            lblmerçi.Text += User.Merci;
            lblCredit.Text += User.Credit;
            pictureBox1.ImageLocation = User.AvatarURL;
            lblReported.Text += User.ReportedMessage;
            lblGroupe.Text += User.Groupe;

            User.Bet("test", 50, User.BetType.Public);
        }
    }
}

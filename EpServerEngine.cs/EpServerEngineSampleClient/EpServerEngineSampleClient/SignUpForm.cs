using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EpServerEngineSampleClient
{
    public partial class SignUpForm : Form
    {
        public SignUpForm()
        {
            InitializeComponent();
            Init();
        }

        public void Init()
        {
            TB_INPUT_SIGN_PWD.PasswordChar = '*';
            TB_INPUT_SIGN_PWD2.PasswordChar = '*';
        }

        private void PB_SIGN_CANCEL_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}

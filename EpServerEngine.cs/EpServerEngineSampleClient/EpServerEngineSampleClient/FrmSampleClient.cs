using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EpServerEngine.cs;
using System.Diagnostics;

using Util;
using GlobalVar;
namespace EpServerEngineSampleClient
{
    public partial class FrmSampleClient : Form,INetworkClientCallback
    {
        INetworkClient m_client = new IocpTcpClient();
        private string IPAddr = null;
        private string Port = null;
        private bool bIsConAddrAndPort = false;
        List<string> passWord = new List<string>();

        public FrmSampleClient()
        {
            InitializeComponent();

            bIsConAddrAndPort = ReadFile.ReadFileAndSetValue(Var.ReadIPandPortTextFilePath, Var.ReadIPandPortTextFileName,
                ref IPAddr, ref Port);
            

            Init();
            
        }

        public void Init()
        {
            TB_ID_INPUT.Text = "ID";
            TB_PWD_INPUT.Text = "PASSWORD";
            TB_PWD_INPUT.PasswordChar = '*';

        }
        public void OnConnected(INetworkClient client, ConnectStatus status)
        {
        }

        public void OnReceived(INetworkClient client, Packet receivedPacket)
        {
            string sendString = StringFromByteArr(receivedPacket.GetPacket()) + "\r\n";
            //AddMsg(sendString);
        }

        public void OnSent(INetworkClient client, SendStatus status)
        {
            switch (status)
            {
                case SendStatus.SUCCESS:
                    Debug.WriteLine("SEND Success");
                    break;
                case SendStatus.FAIL_CONNECTION_CLOSING:
                    Debug.WriteLine("SEND failed due to connection closing");
                    break;
                case SendStatus.FAIL_INVALID_PACKET:
                    Debug.WriteLine("SEND failed due to invalid socket");
                    break;
                case SendStatus.FAIL_NOT_CONNECTED:
                    Debug.WriteLine("SEND failed due to no connection");
                    break;
                case SendStatus.FAIL_SOCKET_ERROR:
                    Debug.WriteLine("SEND Socket Error");
                    break;
            }
        }

        public void OnDisconnect(INetworkClient client)
        {
            MessageBox.Show("Disconnected from the server!");
        }


        String StringFromByteArr(byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        byte[] BytesFromString(String str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        private void FrmSampleClient_Load(object sender, EventArgs e)
        {
            ClientOps ops = new ClientOps( this, IPAddr, Port);
            m_client.Connect(ops);

        }
        private void TB_INPUT_Enter(object sender, EventArgs e)
        {
            if ("ID" == TB_ID_INPUT.Text) TB_ID_INPUT.Text = "";
        }

        private void TB_PWD_INPUT_Enter(object sender, EventArgs e)
        {
            if ("PASSWORD" == TB_PWD_INPUT.Text) TB_PWD_INPUT.Text = "";
        }

        private void PB_ADD_USER_Click(object sender, EventArgs e)
        {
            SignUpForm signUp = new SignUpForm();
            signUp.Show();
        }

        private void PB_LOGIN_Click(object sender, EventArgs e)
        {
            // id와 패스워드를 서버에게 보낸다.
            // 서버는 해당 패킷을 읽고
            // 체크를 해준다.
            // 그다음에 로그인 성공 관련 패킷을 날린다.
            // 성공이 되면 폼은 채팅방으로 전환이 된다.

            Byte[] packet;

        }
    }
}

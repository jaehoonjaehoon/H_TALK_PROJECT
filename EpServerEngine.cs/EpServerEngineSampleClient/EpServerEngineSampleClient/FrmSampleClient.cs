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
        private string clientName = "Guest";
        private string IPAddr;
        private string Port;
        private bool bIsConAddrAndPort = false;

        public FrmSampleClient()
        {
            InitializeComponent();

            bIsConAddrAndPort = ReadFile.ReadFileAndSetValue(Var.ReadIPandPortTextFilePath, Var.ReadIPandPortTextFileName,
                ref IPAddr, ref Port);
        }

        //private void btnSend_Click(object sender, EventArgs e)
        //{
        //    string sendText = tbSend.Text.Trim();
        //    if (sendText.Length <= 0)
        //    {
        //        MessageBox.Show("Please type in something to send.");
        //    }
        //    byte[] bytes = BytesFromString(sendText);
        //    Packet packet=new Packet(bytes,bytes.Count(),false);
        //    m_client.Send(packet);
        //}

        public void OnConnected(INetworkClient client, ConnectStatus status)
        {
            MessageBox.Show("Connected to the server!");
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

        delegate void AddMsg_Involk(string message);
        public void AddMsg(string message)
        {
            //if (tbReceived.InvokeRequired)
            //{
            //    AddMsg_Involk CI = new AddMsg_Involk(AddMsg);
            //    tbReceived.Invoke(CI, message);
            //}
            //else
            //{
            //    tbReceived.Text += message + "\r\n";
            //}
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
            ClientOps ops = new ClientOps( this, clientName, Port);
            m_client.Connect(ops);
        }
    }
}

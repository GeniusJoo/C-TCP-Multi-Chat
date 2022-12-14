using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using System.Threading;
using System.Windows.Forms;


namespace Multi_Chat_Server
{
    public partial class Form1 : Form
    {
        TcpListener server = null; // ����
        TcpClient clientSocket = null; // ����
        static int counter = 0; // ����� ��
        string date; // ��¥
        public Dictionary<TcpClient, string> clientList = new Dictionary<TcpClient, string>();
        // �� Ŭ���̾�Ʈ���� ����Ʈ�� �߰�
        public Form1()
        {
            InitializeComponent();
            // ������ ����
            Thread t = new Thread(InitSocket);
            t.IsBackground = true;
            t.Start();
        }

        private void InitSocket()
        {
            server = new TcpListener(IPAddress.Any, 9999); // ���� ���� IP ��Ʈ
            clientSocket = default(TcpClient); // ���� ����
            server.Start();
            DisplayText(">> Server Start");

            while (true)
            {
                try
                {
                    counter++; // client ����
                    clientSocket = server.AcceptTcpClient(); // client socket ���� ���
                    DisplayText(">> Accept connection from client");
                      
                    NetworkStream stream = clientSocket.GetStream();
                    byte[] buffer = new byte[1024]; // ����
                    int bytes = stream.Read(buffer, 0, buffer.Length);
                    string user_name = Encoding.Unicode.GetString(buffer, 0, bytes); // client ����� ��
                    user_name = user_name.Substring(0, user_name.IndexOf("$"));
                    
                    clientList.Add(clientSocket, user_name); // client ����Ʈ�� �߰�

                    SendMessageAll(user_name + "���� �����߽��ϴ�.", "", false); // ��� client���� �޽��� ����
                    handleClient h_client = new handleClient(); // Ŭ���̾�Ʈ �߰�
                    h_client.OnReceived += new handleClient.MessageDisplayHandler(OnReceived);
                    h_client.OnDisconnected += new handleClient.DisconnectedHandler(h_client_OnDisconnected);
                    h_client.startClient(clientSocket, clientList);
                }
                catch(SocketException se ) { break; }
                catch(Exception ex) { break; }
            }
            clientSocket.Close(); //client ���� �ݱ�
            server.Stop(); // ���� ����
        }

        void h_client_OnDisconnected(TcpClient clientSocket) // client ���� ���� �ڵ鷯
        {
            if(clientList.ContainsKey(clientSocket)) clientList.Remove(clientSocket);
        }
        private void OnReceived(string message, string user_name) // client�� ���� ���� ������
        {
            if (message.Equals("leaveChat"))
            {
                string displayMessage = "leave user : " + user_name;
                DisplayText(displayMessage);
                SendMessageAll("leaveChat", user_name, true); 
            }
            else
            {
                string displayMessage = "Form client : " + user_name + " : " + message;
                DisplayText(displayMessage); // Server�ܿ� �����.
                SendMessageAll(message, user_name, true); // ��� client���� ����
            }
        }

        public void SendMessageAll(string message, string user_name, bool flag)
        {
            foreach(var pair in clientList)
            {
                date = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");// ���� ��¥ �ޱ�
                TcpClient client = pair.Key as TcpClient;
                NetworkStream stream = client.GetStream();
                byte[] buffer = null;
                if (flag)
                {
                    if (message.Equals("leaveChat")) buffer = Encoding.Unicode.GetBytes(user_name + " ���� ��ȭ���� �������ϴ�.");
                    else buffer = Encoding.Unicode.GetBytes(" [ " + date + " ] " + user_name + " : " + message);
                }
                else
                {
                    buffer = Encoding.Unicode.GetBytes(message);
                }

                stream.Write(buffer, 0, buffer.Length); // ���۾���
                stream.Flush();
            }
        }

        private void DisplayText(string text) // Server ȭ�鿡 ����
        {
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.BeginInvoke(new MethodInvoker(delegate { richTextBox1.AppendText(text + Environment.NewLine); }));
            }
            else richTextBox1.AppendText(text + Environment.NewLine);
        }
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
using System.Net.Sockets;
using System.Text;
using System;
using System.Threading;
using System.Windows.Forms;

namespace Multi_Chat_Client
{
    public partial class Form1 : Form
    {
        TcpClient clientSocket = new TcpClient(); // ����
        NetworkStream stream = default(NetworkStream);
        string message = string.Empty;
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                clientSocket.Connect("192.168.0.229", 9999); // ���� IP �� Port
                stream = clientSocket.GetStream();
            }
            catch (Exception e2)
            {
                MessageBox.Show("������ �������� �ƴմϴ�.", "���� ����!");
                Application.Exit();
            }
            message = "ä�ü����� ����Ǿ����ϴ�.";
            DisplayText(message);

            byte[] buffer = Encoding.Unicode.GetBytes("$");
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();

            Thread t_handler = new Thread(GetMessage);
            t_handler.IsBackground = true;
            t_handler.Start();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Focus();
            byte[] buffer = Encoding.Unicode.GetBytes(richTextBox1.Text + "$");
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();
            richTextBox1.Text = "";
        }

        private void GetMessage() // �޽��� �ޱ�
        {
            while (true)
            {
                stream = clientSocket.GetStream();
                int BUFFERSIZE = clientSocket.ReceiveBufferSize;
                byte[] buffer = new byte[BUFFERSIZE];
                int bytes = stream.Read(buffer, 0, buffer.Length);

                string message = Encoding.Unicode.GetString(buffer,0,bytes);
                DisplayText(message);
            }
        }

        private void DisplayText(string text) // Server�� �޽��� ���
        {
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.BeginInvoke(new MethodInvoker(delegate { richTextBox1.AppendText(text + Environment.NewLine); }));
            }
            else
            {
                richTextBox1.AppendText(text + Environment.NewLine);
            }
        }
        private void richTextBox1_KeyUp(object sender, KeyEventArgs e)
         {
             if (e.KeyCode == Keys.Enter) // ����Ű ������ ��
                 button1_Click(this, e);
         }
        private void Form1_FormClosing(object sender, FormClosedEventArgs e) // �� ���� ��
        {
            byte[] buffer = Encoding.Unicode.GetBytes("leaveChat" + "$");
            stream.Write(buffer, 0, buffer.Length);
            stream.Flush();
            Application.ExitThread();
            Environment.Exit(0);
        }
    }
}
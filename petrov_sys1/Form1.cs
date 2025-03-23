using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;


namespace petrov_sys1
{
    public partial class Form1 : Form
    {
        Process childProcess = null;
        System.Threading.EventWaitHandle stopEvent = new EventWaitHandle(false, EventResetMode.AutoReset, "StopEvent");

        System.Threading.EventWaitHandle
            startEvent = new EventWaitHandle(false, EventResetMode.AutoReset, "StartEvent");

        System.Threading.EventWaitHandle confirmEvent =
            new EventWaitHandle(false, EventResetMode.AutoReset, "ConfirmEvent");

        System.Threading.EventWaitHandle
            closeEvent = new EventWaitHandle(false, EventResetMode.AutoReset, "CloseEvent");

        System.Threading.EventWaitHandle sendEvent = new EventWaitHandle(false, EventResetMode.AutoReset, "SendEvent");

        [DllImport(@"C:\\Users\\s1ash\\source\\repos\\petrov_sys1\\x64\\Debug\\petrov_dll.dll",
            CharSet = CharSet.Unicode)]
        private static extern void SendData(int selectedThread, string text);

        public Form1()
        {
            InitializeComponent();
            this.FormClosing += Form1_Closing;
        }

        private void Form1_Closing(object sender, FormClosingEventArgs e)
        {
            if (childProcess != null && !childProcess.HasExited)
            {
                closeEvent.Set();
                childProcess = null;
            }
        }

        int session_num = 10;
        int active_sessions = 0;

        private void ChildProcess_Exited(object sender, EventArgs e)
        {
            if (InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    session_box.Items.Clear();
                    active_sessions = 0;
                    childProcess = null;
                }));
            }
            else
            {
                session_box.Items.Clear();
                active_sessions = 0;
                childProcess = null;
            }
        }

        private void button_start_Click(object sender, EventArgs e)
        {
            if (childProcess == null || childProcess.HasExited)
            {
                childProcess =
                    Process.Start("C:\\Users\\s1ash\\source\\repos\\petrov_sys1\\Debug\\petrov_sys1_cpp.exe");
                childProcess.EnableRaisingEvents = true;
                childProcess.Exited += ChildProcess_Exited;
            }

            for (int i = 0; i < session_num; i++)
            {
                startEvent.Set();
                confirmEvent.WaitOne();
                active_sessions++;
            }

            session_box.Items.Clear();
            session_box.Items.Add("Главный поток");
            session_box.Items.Add("Все потоки");

            for (int i = 1; i <= active_sessions; i++)
            {
                session_box.Items.Add($"Поток {i}");
            }

            session_box.TopIndex = session_box.Items.Count - 1;
        }

        private void button_stop_Click(object sender, EventArgs e)
        {
            if (!(childProcess == null || childProcess.HasExited || active_sessions == 0))
            {
                stopEvent.Set();
                confirmEvent.WaitOne();
                active_sessions--;
                session_box.Items.RemoveAt(active_sessions + 2);
                if (active_sessions == 0)
                {
                    session_box.Items.Clear();
                }
                else
                {
                    session_box.TopIndex = session_box.Items.Count - 1;
                }
            }
        }

        private void button_add_Click(object sender, EventArgs e)
        {
            session_num++;
            session_count.Text = $"{session_num}";
        }

        private void button_delete_Click(object sender, EventArgs e)
        {
            if (session_num > 1)
            {
                session_num--;
                session_count.Text = $"{session_num}";
            }
        }

        private void button_send_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textbox_message.Text))
            {
                MessageBox.Show("Пожалуйста, введите текст сообщения.",
                    "Сообщение пустое",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            int selectedThread = -2;
            if (session_box.SelectedItem != null)
            {
                string sel = session_box.SelectedItem.ToString();
                if (sel == "Главный поток")
                {
                    selectedThread = -1;
                }
                else if (sel != "Все потоки")
                {
                    string[] parts = sel.Split(' ');
                    if (parts.Length == 2 && int.TryParse(parts[1], out int threadNum))
                    {
                        selectedThread = threadNum - 1;
                    }
                }
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите поток для отправки сообщения из списка.",
                    "Поток не выбран",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            SendData(selectedThread, textbox_message.Text);
            sendEvent.Set();
            confirmEvent.WaitOne();
        }
    }
}

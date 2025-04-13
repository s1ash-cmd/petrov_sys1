using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.LinkLabel;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace petrov_sys1
{
    public partial class Form1 : Form
    {

        [DllImport(@"C:\\Users\\s1ash\\source\\repos\\petrov_sys1\\x64\\Debug\\petrov_dll.dll", CharSet = CharSet.Unicode)]
        private static extern void connectClient(int selectedThread, string text);

        [DllImport(@"C:\\Users\\s1ash\\source\\repos\\petrov_sys1\\x64\\Debug\\petrov_dll.dll", CharSet = CharSet.Unicode)]
        private static extern void disconnectClient(int selectedThread, string text);

        [DllImport(@"C:\\Users\\s1ash\\source\\repos\\petrov_sys1\\x64\\Debug\\petrov_dll.dll", CharSet = CharSet.Unicode)]
        private static extern void sendCommand(int selectedThread, int commandId, string message);

        public Form1()
        {
            InitializeComponent();
        }

        int session_num = 10;
        int active_sessions = 0;

        private void button_start_Click(object sender, EventArgs e)
        {

            for (int i = 0; i < session_num; i++)
            {
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
            if (session_box.Items.Count <= 2)
            {
                sendCommand(-1, 1, "Завершить все ");
            }
            else
            {
                int threadId = session_box.Items.Count - 3;
                sendCommand(threadId, 1, "Завершить поток");
                session_box.Items.RemoveAt(session_box.Items.Count - 1);
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

            if (session_box.SelectedItem != null)
            {
                int selectedThread = -2;
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

                sendCommand(selectedThread, 3, textbox_message.Text);
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите поток для отправки сообщения из списка.",
                    "Поток не выбран",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }
        }
    }
}

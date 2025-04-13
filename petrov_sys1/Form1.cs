using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace petrov_sys1
{
    public partial class Form1 : Form
    {
        [DllImport(@"C:\\Users\\s1ash\\source\\repos\\petrov_sys1\\x64\\Debug\\petrov_dll.dll", CharSet = CharSet.Unicode)]
        private static extern void sendCommand(int selectedThread, int commandId, string message);

        [DllImport(@"C:\\Users\\s1ash\\source\\repos\\petrov_sys1\\x64\\Debug\\petrov_dll.dll", CharSet = CharSet.Unicode)]
        private static extern int getSessionCount();

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, bool wParam, int lParam);

        [DllImport("user32.dll")]
        private static extern int GetScrollPos(IntPtr hWnd, int nBar);

        [DllImport("user32.dll")]
        private static extern int SetScrollPos(IntPtr hWnd, int nBar, int nPos, bool bRedraw);

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        private const int WM_SETREDRAW = 0x000B;
        private const int SB_VERT = 1;
        private const int WM_VSCROLL = 0x0115;
        private const int SB_THUMBPOSITION = 4;

        private System.Windows.Forms.Timer updateTimer;

        public Form1()
        {
            InitializeComponent();

            updateTimer = new System.Windows.Forms.Timer();
            updateTimer.Interval = 1000;
            updateTimer.Tick += UpdateSessionList;
            updateTimer.Start();
        }

        int session_num = 10;
        int active_sessions = 0;

        private void button_start_Click(object sender, EventArgs e)
        {
            if (session_box.Items.Count == 0)
            {
                session_box.Items.Add("Главный поток");
                session_box.Items.Add("Все потоки");
            }

            int current = session_box.Items.Count - 2;
            for (int i = 1; i <= session_num; i++)
            {
                int threadNum = current + i;
                sendCommand(threadNum - 1, 0, "Создать поток");
                session_box.Items.Add($"Поток {threadNum}");
            }
            active_sessions += session_num;

            session_box.TopIndex = session_box.Items.Count - 1;
        }

        private void button_stop_Click(object sender, EventArgs e)
        {
            if (active_sessions == 0)
            {
                MessageBox.Show("Нет активных потоков для остановки.",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (session_box.Items.Count <= 2)
            {
                sendCommand(-1, 1, "Завершить все");
                active_sessions = 0;
            }
            else
            {
                active_sessions--;
                session_box.Items.RemoveAt(active_sessions + 2);
                sendCommand(active_sessions, 1, "Завершить поток");
                session_box.TopIndex = session_box.Items.Count - 1;
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

                sendCommand(selectedThread, 2, textbox_message.Text);
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

        private void UpdateSessionList(object sender, EventArgs e)
        {
            try
            {
                string selectedItem = session_box.SelectedItem as string;
                int selectedIndex = session_box.SelectedIndex;
                int scrollPos = GetScrollPos(session_box.Handle, SB_VERT);

                SendMessage(session_box.Handle, WM_SETREDRAW, false, 0);

                int sessionCount = getSessionCount();

                session_box.Items.Clear();
                session_box.Items.Add("Главный поток");
                session_box.Items.Add("Все потоки");

                for (int i = 1; i <= sessionCount; i++)
                {
                    session_box.Items.Add($"Поток {i}");
                }

                if (!string.IsNullOrEmpty(selectedItem) && session_box.Items.Contains(selectedItem))
                {
                    session_box.SelectedItem = selectedItem;
                }
                else if (selectedIndex >= 0 && selectedIndex < session_box.Items.Count)
                {
                    session_box.SelectedIndex = selectedIndex;
                }

                SetScrollPos(session_box.Handle, SB_VERT, scrollPos, true);
                SendMessage(session_box.Handle, WM_VSCROLL, (SB_THUMBPOSITION + 0x10000 * scrollPos), 0);

                active_sessions = sessionCount;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при обновлении списка сессий: {ex.Message}",
                    "Ошибка",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                SendMessage(session_box.Handle, WM_SETREDRAW, true, 0);
                session_box.Refresh();
            }
        }
    }
}

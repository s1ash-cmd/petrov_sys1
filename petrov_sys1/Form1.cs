using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace petrov_sys1
{
    public partial class Form1 : Form
    {
        Process childProcess = null;
        System.Threading.EventWaitHandle stopEvent = new EventWaitHandle(false, EventResetMode.AutoReset, "StopEvent");
        System.Threading.EventWaitHandle startEvent = new EventWaitHandle(false, EventResetMode.AutoReset, "StartEvent");
        System.Threading.EventWaitHandle confirmEvent = new EventWaitHandle(false, EventResetMode.AutoReset, "ConfirmEvent");
        System.Threading.EventWaitHandle closeEvent = new EventWaitHandle(false, EventResetMode.AutoReset, "CloseEvent");

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
                childProcess = Process.Start("\"C:\\Users\\s1ash\\source\\repos\\petrov_sys1\\Debug\\petrov_sys1_cpp.exe\"");
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
                session_box.Items.Add($"Поток № {i}");
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

        private void session_box_SelectedIndexChanged(object sender, EventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}
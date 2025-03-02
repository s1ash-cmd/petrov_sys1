namespace petrov_sys1
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button_start = new System.Windows.Forms.Button();
            this.button_stop = new System.Windows.Forms.Button();
            this.session_box = new System.Windows.Forms.ListBox();
            this.button_delete = new System.Windows.Forms.Button();
            this.button_add = new System.Windows.Forms.Button();
            this.button_send = new System.Windows.Forms.Button();
            this.session_count = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button_start
            // 
            this.button_start.Location = new System.Drawing.Point(14, 447);
            this.button_start.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.button_start.Name = "button_start";
            this.button_start.Size = new System.Drawing.Size(85, 50);
            this.button_start.TabIndex = 0;
            this.button_start.Text = "Start";
            this.button_start.UseVisualStyleBackColor = true;
            this.button_start.Click += new System.EventHandler(this.button_start_Click);
            // 
            // button_stop
            // 
            this.button_stop.Font = new System.Drawing.Font("Comfortaa", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button_stop.Location = new System.Drawing.Point(109, 447);
            this.button_stop.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.button_stop.Name = "button_stop";
            this.button_stop.Size = new System.Drawing.Size(85, 50);
            this.button_stop.TabIndex = 1;
            this.button_stop.Text = "Stop";
            this.button_stop.UseVisualStyleBackColor = true;
            this.button_stop.Click += new System.EventHandler(this.button_stop_Click);
            // 
            // session_box
            // 
            this.session_box.FormattingEnabled = true;
            this.session_box.ItemHeight = 24;
            this.session_box.Location = new System.Drawing.Point(14, 12);
            this.session_box.Name = "session_box";
            this.session_box.Size = new System.Drawing.Size(453, 412);
            this.session_box.TabIndex = 2;
            // 
            // button_delete
            // 
            this.button_delete.Font = new System.Drawing.Font("Comfortaa", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button_delete.Location = new System.Drawing.Point(202, 447);
            this.button_delete.Name = "button_delete";
            this.button_delete.Size = new System.Drawing.Size(50, 50);
            this.button_delete.TabIndex = 3;
            this.button_delete.Text = "-";
            this.button_delete.UseVisualStyleBackColor = true;
            this.button_delete.Click += new System.EventHandler(this.button_delete_Click);
            // 
            // button_add
            // 
            this.button_add.Font = new System.Drawing.Font("Comfortaa", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button_add.Location = new System.Drawing.Point(324, 447);
            this.button_add.Name = "button_add";
            this.button_add.Size = new System.Drawing.Size(50, 50);
            this.button_add.TabIndex = 4;
            this.button_add.Text = "+";
            this.button_add.UseVisualStyleBackColor = true;
            this.button_add.Click += new System.EventHandler(this.button_add_Click);
            // 
            // button_send
            // 
            this.button_send.Font = new System.Drawing.Font("Comfortaa", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.button_send.Location = new System.Drawing.Point(382, 446);
            this.button_send.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.button_send.Name = "button_send";
            this.button_send.Size = new System.Drawing.Size(85, 50);
            this.button_send.TabIndex = 5;
            this.button_send.Text = "Send";
            this.button_send.UseVisualStyleBackColor = true;
            
            // 
            // session_count
            // 
            this.session_count.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.session_count.Font = new System.Drawing.Font("Comfortaa", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.session_count.Location = new System.Drawing.Point(258, 447);
            this.session_count.Name = "session_count";
            this.session_count.Size = new System.Drawing.Size(60, 50);
            this.session_count.TabIndex = 6;
            this.session_count.Text = "10";
            this.session_count.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(481, 511);
            this.Controls.Add(this.session_count);
            this.Controls.Add(this.button_send);
            this.Controls.Add(this.button_add);
            this.Controls.Add(this.button_delete);
            this.Controls.Add(this.session_box);
            this.Controls.Add(this.button_stop);
            this.Controls.Add(this.button_start);
            this.Font = new System.Drawing.Font("Comfortaa", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.Name = "Form1";
            this.Text = "Петров Дмитрий АС-22-04 ЛР1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_start;
        private System.Windows.Forms.Button button_stop;
        private System.Windows.Forms.ListBox session_box;
        private System.Windows.Forms.Button button_delete;
        private System.Windows.Forms.Button button_add;
        private System.Windows.Forms.Button button_send;
        private System.Windows.Forms.Label session_count;
    }
}


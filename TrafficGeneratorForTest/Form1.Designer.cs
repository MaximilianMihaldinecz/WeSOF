namespace TrafficGeneratorForTest
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtURL = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.nmVisitorsCount = new System.Windows.Forms.NumericUpDown();
            this.nmCtrlConverted = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.nmTestConverted = new System.Windows.Forms.NumericUpDown();
            this.btnStart = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.lblComplete = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtConvertUrl = new System.Windows.Forms.TextBox();
            this.lblCtrlConvert = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.lblTestConvert = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtCurlLoc = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtCookieLoc = new System.Windows.Forms.TextBox();
            this.btnSendSingleConverted = new System.Windows.Forms.Button();
            this.btnSendSingle = new System.Windows.Forms.Button();
            this.lblTotalVisSent = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nmVisitorsCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmCtrlConverted)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmTestConverted)).BeginInit();
            this.SuspendLayout();
            // 
            // txtURL
            // 
            this.txtURL.Location = new System.Drawing.Point(12, 28);
            this.txtURL.Name = "txtURL";
            this.txtURL.Size = new System.Drawing.Size(249, 20);
            this.txtURL.TabIndex = 0;
            this.txtURL.Text = "http://127.0.0.1:12345/";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "WeSof Address: ";
            // 
            // nmVisitorsCount
            // 
            this.nmVisitorsCount.Increment = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nmVisitorsCount.Location = new System.Drawing.Point(15, 122);
            this.nmVisitorsCount.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nmVisitorsCount.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.nmVisitorsCount.Name = "nmVisitorsCount";
            this.nmVisitorsCount.Size = new System.Drawing.Size(120, 20);
            this.nmVisitorsCount.TabIndex = 4;
            this.nmVisitorsCount.Value = new decimal(new int[] {
            50,
            0,
            0,
            0});
            this.nmVisitorsCount.ValueChanged += new System.EventHandler(this.nmVisitorsCount_ValueChanged);
            // 
            // nmCtrlConverted
            // 
            this.nmCtrlConverted.Location = new System.Drawing.Point(12, 173);
            this.nmCtrlConverted.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nmCtrlConverted.Name = "nmCtrlConverted";
            this.nmCtrlConverted.Size = new System.Drawing.Size(120, 20);
            this.nmCtrlConverted.TabIndex = 5;
            this.nmCtrlConverted.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.nmCtrlConverted.ValueChanged += new System.EventHandler(this.nmCtrlConverted_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 106);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Visitors count";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 157);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(92, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Control Converted";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(138, 157);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(80, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "Test Converted";
            // 
            // nmTestConverted
            // 
            this.nmTestConverted.Location = new System.Drawing.Point(141, 173);
            this.nmTestConverted.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.nmTestConverted.Name = "nmTestConverted";
            this.nmTestConverted.Size = new System.Drawing.Size(120, 20);
            this.nmTestConverted.TabIndex = 9;
            this.nmTestConverted.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.nmTestConverted.ValueChanged += new System.EventHandler(this.nmTestConverted_ValueChanged);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(428, 214);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(157, 45);
            this.btnStart.TabIndex = 12;
            this.btnStart.Text = "START BATCH (20)";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(462, 24);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(87, 13);
            this.label7.TabIndex = 13;
            this.label7.Text = "Completed Visits:";
            // 
            // lblComplete
            // 
            this.lblComplete.AutoSize = true;
            this.lblComplete.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblComplete.Location = new System.Drawing.Point(470, 44);
            this.lblComplete.Name = "lblComplete";
            this.lblComplete.Size = new System.Drawing.Size(0, 25);
            this.lblComplete.TabIndex = 14;
            this.lblComplete.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 13);
            this.label2.TabIndex = 16;
            this.label2.Text = "Conversion URL:";
            // 
            // txtConvertUrl
            // 
            this.txtConvertUrl.Location = new System.Drawing.Point(12, 71);
            this.txtConvertUrl.Name = "txtConvertUrl";
            this.txtConvertUrl.Size = new System.Drawing.Size(249, 20);
            this.txtConvertUrl.TabIndex = 15;
            this.txtConvertUrl.Text = "http://127.0.0.1:12345/?add-to-cart=70";
            // 
            // lblCtrlConvert
            // 
            this.lblCtrlConvert.AutoSize = true;
            this.lblCtrlConvert.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCtrlConvert.Location = new System.Drawing.Point(470, 98);
            this.lblCtrlConvert.Name = "lblCtrlConvert";
            this.lblCtrlConvert.Size = new System.Drawing.Size(0, 25);
            this.lblCtrlConvert.TabIndex = 18;
            this.lblCtrlConvert.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(447, 78);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(121, 13);
            this.label9.TabIndex = 17;
            this.label9.Text = "Completed Control CVR:";
            // 
            // lblTestConvert
            // 
            this.lblTestConvert.AutoSize = true;
            this.lblTestConvert.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTestConvert.Location = new System.Drawing.Point(470, 163);
            this.lblTestConvert.Name = "lblTestConvert";
            this.lblTestConvert.Size = new System.Drawing.Size(0, 25);
            this.lblTestConvert.TabIndex = 20;
            this.lblTestConvert.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(451, 141);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(109, 13);
            this.label10.TabIndex = 19;
            this.label10.Text = "Completed Test CVR:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 246);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(110, 13);
            this.label8.TabIndex = 22;
            this.label8.Text = "CURL binary location:";
            // 
            // txtCurlLoc
            // 
            this.txtCurlLoc.Location = new System.Drawing.Point(12, 261);
            this.txtCurlLoc.Name = "txtCurlLoc";
            this.txtCurlLoc.Size = new System.Drawing.Size(249, 20);
            this.txtCurlLoc.TabIndex = 21;
            this.txtCurlLoc.Text = "D:\\curl-7.50.1-win32-mingw\\bin\\curl.exe";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(12, 295);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(87, 13);
            this.label11.TabIndex = 24;
            this.label11.Text = "Store cookies at:";
            // 
            // txtCookieLoc
            // 
            this.txtCookieLoc.Location = new System.Drawing.Point(12, 310);
            this.txtCookieLoc.Name = "txtCookieLoc";
            this.txtCookieLoc.Size = new System.Drawing.Size(249, 20);
            this.txtCookieLoc.TabIndex = 23;
            this.txtCookieLoc.Text = "D:\\trafficgenerator\\";
            // 
            // btnSendSingleConverted
            // 
            this.btnSendSingleConverted.Location = new System.Drawing.Point(428, 310);
            this.btnSendSingleConverted.Name = "btnSendSingleConverted";
            this.btnSendSingleConverted.Size = new System.Drawing.Size(157, 23);
            this.btnSendSingleConverted.TabIndex = 25;
            this.btnSendSingleConverted.Text = "Send single converted visitor";
            this.btnSendSingleConverted.UseVisualStyleBackColor = true;
            this.btnSendSingleConverted.Click += new System.EventHandler(this.btnSendSingleConverted_Click);
            // 
            // btnSendSingle
            // 
            this.btnSendSingle.Location = new System.Drawing.Point(428, 281);
            this.btnSendSingle.Name = "btnSendSingle";
            this.btnSendSingle.Size = new System.Drawing.Size(157, 23);
            this.btnSendSingle.TabIndex = 26;
            this.btnSendSingle.Text = "Send single visitor";
            this.btnSendSingle.UseVisualStyleBackColor = true;
            this.btnSendSingle.Click += new System.EventHandler(this.btnSendSingle_Click);
            // 
            // lblTotalVisSent
            // 
            this.lblTotalVisSent.AutoSize = true;
            this.lblTotalVisSent.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalVisSent.Location = new System.Drawing.Point(10, 357);
            this.lblTotalVisSent.Name = "lblTotalVisSent";
            this.lblTotalVisSent.Size = new System.Drawing.Size(211, 25);
            this.lblTotalVisSent.TabIndex = 27;
            this.lblTotalVisSent.Text = "Total Visitors Sent: 0";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(608, 391);
            this.Controls.Add(this.lblTotalVisSent);
            this.Controls.Add(this.btnSendSingle);
            this.Controls.Add(this.btnSendSingleConverted);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.txtCookieLoc);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtCurlLoc);
            this.Controls.Add(this.lblTestConvert);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.lblCtrlConvert);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtConvertUrl);
            this.Controls.Add(this.lblComplete);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.nmTestConverted);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.nmCtrlConverted);
            this.Controls.Add(this.nmVisitorsCount);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtURL);
            this.Name = "Form1";
            this.Text = "Mock Traffic Generator for WeSOF testing";
            ((System.ComponentModel.ISupportInitialize)(this.nmVisitorsCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmCtrlConverted)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmTestConverted)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtURL;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown nmVisitorsCount;
        private System.Windows.Forms.NumericUpDown nmCtrlConverted;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown nmTestConverted;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label lblComplete;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtConvertUrl;
        private System.Windows.Forms.Label lblCtrlConvert;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label lblTestConvert;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtCurlLoc;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtCookieLoc;
        private System.Windows.Forms.Button btnSendSingleConverted;
        private System.Windows.Forms.Button btnSendSingle;
        private System.Windows.Forms.Label lblTotalVisSent;
    }
}


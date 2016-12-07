using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace TrafficGeneratorForTest
{
    public partial class Form1 : Form
    {
        int GlobalVisitCounter = 0;

        int TotalVisits;
        int CompletedVisits;
        int ctrlCVRTarget;
        int ctrlCVRStatus;       
        int testCVRTarget;
        int testCVRStatus;
        string curlLocation;
        string cookieLocation;

        string address;
        string cvrAddress;

        WebBrowser[] Browser;


        public Form1()
        {
            InitializeComponent();
        }

        private void ResetValues()
        {
            TotalVisits = (int)(nmVisitorsCount.Value);
            ctrlCVRTarget = (int)nmCtrlConverted.Value;
            testCVRTarget = (int)nmTestConverted.Value;
            ctrlCVRStatus = 0;
            testCVRStatus = 0;
            curlLocation = txtCurlLoc.Text;
            cookieLocation = txtCookieLoc.Text;

            address = txtURL.Text;
            cvrAddress = txtConvertUrl.Text;

            lblComplete.Text = "";
            lblCtrlConvert.Text = "";
            lblTestConvert.Text = "";
        }


        private void btnStart_click(object sender, EventArgs e)
        {

            ResetValues();
            lblComplete.Text = "0 /" + TotalVisits.ToString();
            lblTotalVisSent.Text = "Total Visitors Sent: in progress...";           

            
            //Create the visits
            for(CompletedVisits = 0; CompletedVisits < TotalVisits; CompletedVisits++)
            {
                MakeNewVisit(CompletedVisits + GlobalVisitCounter +  1);
                lblComplete.Text = (CompletedVisits + 1).ToString() + "/" + TotalVisits.ToString();
            }           


            //Make the conversions. Assume first assignemnt was the control and tests are AB.
            for(int i = 0; i < TotalVisits; i++)
            {
                //control
                if(((i + GlobalVisitCounter) % 2) == 0)
                {
                    if(ctrlCVRStatus < ctrlCVRTarget)
                    {
                        MakeConversionVisit(i + GlobalVisitCounter + 1);
                        ctrlCVRStatus++;
                        lblCtrlConvert.Text = ctrlCVRStatus.ToString() + "/" + ctrlCVRTarget.ToString();
                    }
                }

                //test
                if (((i + GlobalVisitCounter) % 2) == 1)
                {
                    if (testCVRStatus < testCVRTarget)
                    {
                        MakeConversionVisit(i + GlobalVisitCounter + 1);
                        testCVRStatus++;
                        lblTestConvert.Text = testCVRStatus.ToString() + "/" + testCVRTarget.ToString();
                    }
                }
            }

            GlobalVisitCounter += TotalVisits;
            UpdGlobalLabel();
        }


        private void MakeConversionVisit(int visitorId)
        {
            // Example command
            // d:\curl-7.50.1-win32-mingw\bin\curl.exe 192.168.0.14/?add-to-cart=70 -b D:\trafficgenerator\0.txt

            string command = "/C " + curlLocation + " " + cvrAddress + " -b " + cookieLocation + visitorId.ToString() + ".txt";
            StartCmdProc(command);
        }

        private void MakeNewVisit(int visitorId)
        {
            // Example command
            // d:\curl-7.50.1-win32-mingw\bin\curl.exe 192.168.0.14 -c D:\trafficgenerator\0.txt

            string command = "/C " + curlLocation + " " + address + " -c " + cookieLocation + visitorId.ToString() + ".txt";
            StartCmdProc(command);           
        }

        private void StartCmdProc(string arguments)
        {
            Process proc = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = arguments;

            proc.StartInfo = startInfo;
            proc.Start();

            proc.WaitForExit();
        }

        private void nmVisitorsCount_ValueChanged(object sender, EventArgs e)
        {
            ChkBatchValues();

            if (nmVisitorsCount.Value % 2 != 0)
                nmVisitorsCount.Value++;
            else
                btnStart.Text = "START BATCH (" + nmVisitorsCount.Value.ToString() + ")";            
        }


        private void ChkBatchValues()
        {
            if (nmCtrlConverted.Value > (nmVisitorsCount.Value / 2))
            {
                nmCtrlConverted.Value = (nmVisitorsCount.Value / 2);
            }

            if (nmTestConverted.Value > (nmVisitorsCount.Value / 2))
            {
                nmTestConverted.Value = (nmVisitorsCount.Value / 2);
            }
        }


        private void btnSendSingle_Click(object sender, EventArgs e)
        {
            ResetValues();
            MakeNewVisit(GlobalVisitCounter + 1);
            GlobalVisitCounter++;
            UpdGlobalLabel();
        }

        private void btnSendSingleConverted_Click(object sender, EventArgs e)
        {
            ResetValues();
            MakeNewVisit(GlobalVisitCounter + 1);
            MakeConversionVisit(GlobalVisitCounter + 1);
            GlobalVisitCounter++;
            UpdGlobalLabel();
        }

        private void UpdGlobalLabel()
        {
            lblTotalVisSent.Text = "Total Visitors Sent: " + GlobalVisitCounter.ToString();
        }

        private void nmCtrlConverted_ValueChanged(object sender, EventArgs e)
        {
            ChkBatchValues();
        }

        private void nmTestConverted_ValueChanged(object sender, EventArgs e)
        {
            ChkBatchValues();
        }
    }
}

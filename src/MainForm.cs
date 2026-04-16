// ================================================
// tonebridge. - easy daw loopback for bedroom musicians.
// ================================================
//
// File:        MainForm.cs
// Purpose:     Main window of the app
// Author:      Zach T. (houndslight)
// Created:     April 2026
// 
// Why this file exists:
//   This is the first thing the user will see when they open the program.
//   It checks if the VB Audio Virtual Cable is installed, 
//   shows instructions for the Scarlett Solo 1st Generation (This is just my interface one of similar age with no loopback should work similarly.)
//   and gives the user easy buttons to test loopback and to install the cable if needed.
//
// Notes for future me:
//   Keep this relatively simple, muscians dont really care about how the app works as long as it does.
// ================================================

using System;
using System.Drawing;
using System.Windows.Forms;
using NAudio.Wave;
using NAudio.CoreAudioApi;
using System.Runtime.CompilerServices;

/// Bread & Butter:
/// 
/// Namespace and Class Declaration
///     The Mainform class is in the tonebridge namespace.
/// 
/// Constructor
///     Initializes the VB Audio VC checker,
///     then checks if the cable is installed while updating UI elements acordingly.
/// 
/// VBAChecker Class
///     Provides a method to check if the VB Audio Cable is installed
/// 
/// ================================================
/// 
/// UI Elements:
/// 
/// Labels (lblStatus, lblWarning):
///     Displats the status of the VB Cable detection.
///     Shows a warning message about turning off the Direct Monitoring switch on the interface.
/// 
/// Buttons (btnTestSignal, btnOpenDownloadPage):
///     btnTestSignal: Plays a test tone to check audio setup on OBS/Discord
///     btnOpenDownloadPage: Opens the VB-Audio download page in the users default web browser if it is missing
/// 
/// ================================================
/// 
/// Event Handlers:
/// 
/// BtnTestSignal_Click:
///     Plays a short tone using NAudio for low-latency audio output suitable for the user.
///     Updates the status label to indicate that the test signal is being played.
/// 
/// BtnOpenDownloadPage_Click:
///     Opens the VB-Audio download page
/// 
/// ================================================
/// 
/// Extra personal notes:
/// 
/// the goal of this is to provide a simple and musician centered interface so far the idea is check for vb cable -> play test tone -> make sure it works in obs.
/// 
/// 

namespace tonebridge
{
    public partial class MainForm : Form
    {
        // core deps
        private readonly VBAChecker  _vbAchecker;
        private WaveOutEvent? waveOut;
        private AudioFileReader? audioFile;

        // UI
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnTestSignal;
        private System.Windows.Forms.Button btnOpenDownloadPage;
        private System.Windows.Forms.Label lblWarning;

         // constructor
        public MainForm()
        {
            InitializeComponent();

            _vbAchecker = new VBAChecker();

            if (!_vbAchecker.CheckVBACableInstalled())
            {
                lblStatus.Text = "Virtual Cable was not found!";
                btnTestSignal.Enabled = false;
                btnOpenDownloadPage.Visible = true;
                lblWarning.Visible = true;
            }
            else
            {
                lblStatus.Text = "Virtual Cable found!";
                btnOpenDownloadPage.Visible = false;
                lblWarning.Visible = false;
            }

            btnTestSignal.Click += BtnTestSignal_Click;
            btnOpenDownloadPage.Click += BtnOpenDownloadPage_Click;  // Line 112: make sure it's .Click +=
        }

        private void InitializeComponent()
        {
            // program controls
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnTestSignal = new System.Windows.Forms.Button();
            this.btnOpenDownloadPage = new System.Windows.Forms.Button();
            this.lblWarning = new System.Windows.Forms.Label();

            // form settings
            this.ClientSize = new System.Drawing.Size(600, 400);
            this.Text = "tonebridge.";

            // lblStatus
            this.lblStatus.Location = new System.Drawing.Point(20, 20);
            this.lblStatus.Size = new System.Drawing.Size(550, 30);
            this.lblStatus.Text = "Checking....";  // <-- Capital T here
            this.Controls.Add(this.lblStatus);

            // Test signal
            this.btnTestSignal.Location = new System.Drawing.Point(20, 80);
            this.btnTestSignal.Size = new System.Drawing.Size(150, 40);
            this.btnTestSignal.Text = "Test!";
            this.Controls.Add(this.btnTestSignal);

            // btnTestSignal
            this.btnTestSignal.Location = new System.Drawing.Point(20, 80);
            this.btnTestSignal.Size = new System.Drawing.Size(150, 40);
            this.btnTestSignal.Text = "Test Signal";
            this.Controls.Add(this.btnTestSignal);
            
            // btnOpenDownloadPage
            this.btnOpenDownloadPage.Location = new System.Drawing.Point(20, 140);
            this.btnOpenDownloadPage.Size = new System.Drawing.Size(200, 40);
            this.btnOpenDownloadPage.Text = "Download VB-Cable";
            this.Controls.Add(this.btnOpenDownloadPage);
            
            // lblWarning
            this.lblWarning.Location = new System.Drawing.Point(20, 200);
            this.lblWarning.Size = new System.Drawing.Size(550, 60);
            this.lblWarning.Text = "Turn OFF Direct Monitoring on your Scarlett Solo!";
            this.lblWarning.ForeColor = System.Drawing.Color.Red;
            this.Controls.Add(this.lblWarning);
        }
        
        private void BtnTestSignal_Click(object sender, EventArgs e)
        {
            // tone using NAudio
            string toneFilePath = "#.wav"; // STILL WILL REPLACE LATER LOL
            audioFile = new AudioFileReader(toneFilePath);
            waveOut = new WaveOutEvent();
            waveOut.Init(audioFile);
            waveOut.Play();

            lblStatus.Text = "Playing test signal. Please check OBS/Discord meters.";
        }

        private void BtnOpenDownloadPage_Click(object sender, EventArgs e)
        {
            // Opens VB download
            System.Diagnostics.Process.Start("https://download.vb-audio.com/Download_CABLE/VBCABLE_Driver_Pack45.zip");
        }
    }
}
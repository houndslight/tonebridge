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
using System.Threading.Tasks.Dataflow;      // needed for checks

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
        private readonly VBAChecker _vbAChecker;
        private WaveOutEvent waveOut;
        private AudioFileReader audioFile;

        // Constructor
        public MainForm()
        {
            InitializeComponent();

            // vba checker instance
            _vbAChecker = new VBAChecker();

            // vb audio install check
            if (!_vbAChecker.CheckVBACableInstalled())
            {
                lblStatus.Text = "VB-Audio Virtual Cable not found.";
                btnTestSignal.Enabled = false;
                btnOpenDownloadPage.Visible = true;
                lblWarning.Visible = true;
            }
            else
            {
                lblStatus.Text = "VB-Audio Virtual Cable found!";
                btnOpenDownloadPage.Visible = false;
                lblWarning.Visible = false;
            }

            // test signal click event handler
            btnTestSignal.Click += BtnTestSignal_Click;

            // download (if needed)
            btnOpenDownloadPage.Click += BtnOpenDownloadPage_Click;
        }

            private void BtnTestSignal_Click(object sender, EventArgs e)
            {
               // short tone using NAudio
               string toneFilePath = "#.wav"; // will replace later
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
        public class VBAChecker
        {
            // Constructor
            public VBAChecker()
            {
                // Init (if needed)
            }

            public bool CheckVBACableInstalled()
            {
                // List of all audio endpoints
                var endpointInterfaces = DeviceEnum.getDefaultAudioEndpoint(Dataflow.Render, Role.Multimedia);

                // Expected names (VB Cable)
                string[] expectedDeviceNames = { "CABLE Input", "CABLE Output" };

                // Device check
                bool VBACableInstalled = expectedDeviceNames.All(deviceName => endpointInterfacesInterfaces.Any(endpoint => endpoint.FriendlyName.Contains(deviceName)));

                return VBACableInstalled;
            }
        }
    }
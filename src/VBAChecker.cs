// ================================================
// tonebridge. - easy daw loopback for bedroom musicians.
// ================================================
//
// File:        VBAChecker.cs
// Purpose:     Detects if VB-Audio Virtual Cable is installed
// Author:      Zach T. (houndslight)
// Created:     April 2026
// 
// Why this file exists:
//   We need to bridge ASIO (low-latency DAW) → WDM (what OBS/Discord understands).
//   The VB-Audio Virtual Cable does this. This class checks if it's installed
//   and gives the user clear feedback.
//
// Notes for future me:
//   - Scarlett Solo 1st Gen users MUST turn OFF Direct Monitoring
//   - This check runs on startup so we can warn the user early
// ================================================

using NAudio.CoreAudioApi;
using System.Linq;

/// Checks whether the VB-Audio Virtual Cable is installed on the system.
/// 
/// This is critical because:
///   - Your DAW outputs to "CABLE Input" (ASIO)
///   - OBS and Discord read from "CABLE Output" (WDM)
/// 
/// Without both, the effected audio from Ableton/Reaper won't reach your stream.
/// 
/// Methods:
/// 
/// 1. Constructor: This initializes any needed properites or services.
///    We dont need to initialize anything so for the current moment this can be left empty.
/// 
/// 2. CheckVBACableInstalled: Checks if VB Cable is installed by looping for specific audio devices.
///    This method will return true if both "CABLE Input" and "CABLE Output" are found if either one of them are missing it will return false.
/// 
///


namespace tonebridge
{
    public class VBAChecker
    {
        // Constructor
        public VBAChecker()
        {
            // NOT CURRENTLY NEEDED!
        }

        // Method to check if the VB-Audio Virtual Cable is installed
        public bool CheckVBACableInstalled()
        {
            // Lists all system audio endpoints
            var endpointInterfaces = DeviceEnum.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

            // Expected names we need
            string[] expectedDeviceNames = { "CABLE Input", "CABLE Output"};

            // Device check
            bool VBACableInstalled = expectedDeviceNames.All(deviceName => endpointInterfaces.Any(endpointInterfaces => endpoint.FriendlyName.Contains(deviceName)));

            return VBACableInstalled;
        }
    }
}
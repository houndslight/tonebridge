// ================================================
// tonebridge. - easy daw loopback for bedroom musicians.
// ================================================
//
// File:        TestToneGenerator.cs
// Purpose:     Generates a 440hz test tone for audio routing testing
// Author:      Zach T. (houndslight)
// Created:     April 2026
//
// Why this file exists:
//   This is basically the sound that will play as you would check on obs and discord levels.
// ================================================
// ================================================
// Visualizing the Wave:
// One cycle of sine wave (0 to 2π):
//     ^ amplitude
//     |
// 1.0 +      *       *
//     |     * *     * *
// 0.0 +----*---*---*---*----> time/sample
//     |   *     *     *
// -1.0 + *       *
//
// Each * represents a sample at 48kHz
// 440Hz tone = 440 cycles per second
// At 48kHz: 48,000 samples/sec ÷ 440 cycles/sec = 109 samples per cycle
// 48kHz is standard for streaming because:
// - Divisible by 24, 30, 60 (video frame rates)
// - 48,000 ÷ 24 = 2000 samples per frame (clean)
// - 48,000 ÷ 30 = 1600 samples per frame (clean)
// - 48,000 ÷ 60 = 800 samples per frame (clean)
// This prevents audio drift when syncing to video

using System;
using System.Threading;
using System.Threading.Tasks;
using NAudio.Wave;
using NAudio.CoreAudioApi;

/// <summary>
/// Basic OverLook
///     Pretty much this will generate the test tone with volume control for 3 seconds per click through the specified audio device.
///     Also I'm making a sine wave because I'm kinda bored.
///     Also going to be starting playback on background threads so the UI doesnt freeze
///     Generates sine wave samples in real-time
/// </summary>

namespace tonebridge.Services
{
    public class TestToneGenerator : IDisposable
    {
        // FIX: Made nullable to satisfy compiler warnings - these get assigned in PlayTestTone
        private WaveOutEvent? _waveOut;
        private SineWaveProvider? _sineProvider;
        private CancellationTokenSource? _cancellationTokenSource;
        private bool _isPlaying;

        // starting to make the control
        public double Volume { get; set; } = 0.5; // pretty safe default to not clip and blow your eardrums out

        // 3 sec test tone to device
        public void PlayTestTone(string targetDeviceId, CancellationToken token)
        {
            if (_isPlaying)
                throw new InvalidOperationException("Test tone already playing.");

            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(token);
            _isPlaying = true;

            // sine wave gen
            _sineProvider = new SineWaveProvider(440.0, 48000) //48kHz
            {
                Volume = (float)Volume
            };

            // output device setup
            _waveOut = new WaveOutEvent
            {
                DeviceNumber = GetDeviceIndexById(targetDeviceId),
                DesiredLatency = 100 // 100ms
            };

            _waveOut.Init(_sineProvider);
            _waveOut.PlaybackStopped += OnPlaybackStopped;

            // playback
            Task.Run(() =>
            {
                try
                {
                    _waveOut.Play();

                    while (_isPlaying && !_cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        Thread.Sleep(50); // 50 ms checks
                    }
                }
                catch (OperationCanceledException)
                {
                    // ecpected user cancels
                }
                finally
                {
                    StopPlayback();
                }
            });
        }

        // test tone stop
        public void StopTestTone()
        {
            _cancellationTokenSource?.Cancel();
        }

        private void OnPlaybackStopped(object? sender, StoppedEventArgs e)
        {
            StopPlayback();
        }

        private void StopPlayback()
        {
            if (_waveOut != null)
            {
                _waveOut.Stop();
                _waveOut.Dispose();
                _waveOut = null;
            }

            // FIX: SineWaveProvider doesn't need Dispose, just null it out
            // It's a simple managed object with no unmanaged resources
            _sineProvider = null;
            
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
            _isPlaying = false;
        }

        // Getting device index by enumeration
        private int GetDeviceIndexById(string deviceId)
        {
            var enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active);

            for (int i = 0; i < devices.Count; i++)
            {
                if (devices[i].ID == deviceId)
                    return i;
            }
            throw new ArgumentException($"Device With ID '{deviceId}' not found");
        }

        public void Dispose()
        {
            StopPlayback();
        }
    }

    // FIX: SineWaveProvider doesn't implement IDisposable - it has no unmanaged resources
    // It's just a pure managed class generating samples on demand
    public class SineWaveProvider : ISampleProvider
    {
        private double _phase;
        private readonly double _frequency;
        private readonly int _sampleRate;
        private readonly double _phaseIncrement;

        public float Volume { get; set; } = 0.5f;

        public WaveFormat WaveFormat => new WaveFormat(48000, 32); // 48kHz, 32-bit float

        public SineWaveProvider(double frequency, int sampleRate)
        {
            _frequency = frequency;
            _sampleRate = sampleRate;

            // Calculate phase increment: How much to advance the phase each sample
            // Math: 2π * frequency / sampleRate
            _phaseIncrement = (2 * Math.PI * _frequency) / _sampleRate;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            for (int i = 0; i < count; i++)
            {
                // Generate sine wave sample
                // Math: sin(phase) where phase advances by 2π * frequency / sampleRate each sample
                double sampleValue = Math.Sin(_phase) * Volume;

                // Convert to 32-bit IEEE float format
                // Musician Context: 32-bit float gives us headroom before clipping
                // Unlike 16-bit which clips harshly at ±32767
                buffer[offset + i] = (float)sampleValue;

                // Advance phase (wrap around at 2π)
                _phase += _phaseIncrement;
                if (_phase >= 2 * Math.PI)
                    _phase -= 2 * Math.PI;
            }

            return count;
        }
    }
}
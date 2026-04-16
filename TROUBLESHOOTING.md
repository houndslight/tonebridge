# Troubleshooting tonebridge.

### I hear myself twice (echo/doubling)

- Make sure the physical Direct Monitor switch on your Scarlett Solo is **OFF**.
- In your DAW, do **not** have any extra monitoring enabled that routes back to the interface input.

### No signal in OBS or Discord

- Confirm your DAW output is set to **CABLE Input** (not your Scarlett).
- Check Windows Sound settings → Recording tab → CABLE Output should show green bars when you play.
- Restart OBS/Discord after changing devices.

### Crackling or high latency

- Increase buffer size to 256 samples in your DAW.
- Close browser tabs and other audio apps.
- Use a dedicated USB port (no hub).
- Make sure you're using the official Focusrite ASIO driver.

### Sample rate mismatch (weird pitch)

Set everything to 48 kHz:
- DAW
- Windows sound settings (for the virtual cable)
- OBS

More issues? Open an issue on GitHub with your Windows version, DAW, and exact symptoms.
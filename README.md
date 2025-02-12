# VRAM Temp Tracker for Salad | Auto Terminate & Email Alerts
This console application monitors VRAM temperatures in real-time using GPU-Z logs and automatically shuts down [Salad](https://salad.com/) if the VRAM temperature exceeds 100°C, preventing overheating and potential hardware damage.

**Why?**

I deploy various computers for Salad utilization (a platform that allows users to earn rewards by sharing their computing power). I manage these systems via Google Remote Desktop, and I needed a way to monitor VRAM temperatures without manually checking each PC. 

## Features
- Tracks VRAM temperature from GPU-Z log files
-  Auto-terminates Salad if VRAM temperature hits 100°C
-  Runs continuously, checking the temperature every 5 seconds

## Program Breakdown
- Reads GPU-Z log files to extract the latest VRAM temperature
- Compares it with the critical temperature threshold (100°C by default)
- If the temperature exceeds the limit, the program
  - Terminates Salad
  - Sends an email alert to notify you
- Repeats the check every 5 seconds

## Requirements
- GPU-Z logging enabled
- Update the log file path to match your system
- dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true to create an executable

## Upcoming Changes
- Going to add email notification when Salad is shutdown


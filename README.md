# StarfieldWwizard
![GitHub License](https://img.shields.io/github/license/yak3d/starfield-wwizard)

A WinUI 3 application for working with Starfield's Wwise audio files and BSA archives.

## Overview

StarfieldWwizard is a Windows desktop application that provides tools for extracting and processing audio content from Starfield. The application can read BSA archive files, extract Wwise sound bank data, and convert audio files using various audio processing tools.

## Features

- **BSA Archive Support**: Read and extract files from Starfield's BSA archive format
- **Wwise Audio Processing**: Handle Wwise sound bank files and audio streams
- **Audio Conversion**: Convert audio files using FFmpeg and VGM Stream
- **Settings Management**: Persistent application settings with automatic Starfield installation detection
- **Theme Support**: Light/Dark theme switching

## Requirements

- **Windows 10/11**: Version 1903 (build 18362) or later
- **.NET 8**: Windows Desktop Runtime
- **Starfield Installation**: Game must be installed for archive access

## Building

1. Clone the repository
2. Open `StarfieldWwizard.sln` in Visual Studio 2022
3. Restore NuGet packages
4. Build solution (supports x64 and ARM64)

## Configuration

The application automatically:
- Detects Starfield installation directory
- Creates default settings file on first run
- Downloads required audio processing dependencies
- Configures logging to both console and file

Settings and logs are stored in JSON format in the user's local application data folder.

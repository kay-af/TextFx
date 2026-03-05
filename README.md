# TextFx

## Introduction

**TextFx** is an open-source **Unity** package for animating texts, inspired by [Rive's](https://rive.app/) text animation system. It is designed to work with _TextMeshPro_ and provides a structured way to rig character properties such as position, rotation, scale, and more. **TextFx** offers a robust programmatic API, allowing you to use your favorite tweening library to create beautiful text animations.

[Preview.webm](https://github.com/user-attachments/assets/db54f5c3-356e-4b99-a27f-26aa1018223f)

## Installation

This section outlines the installation process of **TextFx**.

### Prerequisites

**TextFx** requires **TextMeshPro** which comes from `com.unity.ugui` package. Ensure **TextMeshPro** essentials are imported in your game.

### Install via OpenUPM (Recommended)

To include **TextFx** in your game via **OpenUPM**, follow these steps:

1. Setup your project to work with the **OpenUPM CLI**. Follow the docs [OpenUPM Docs](https://openupm.com/docs).

1. To add the latest version of **TextFx** using the **OpenUPM CLI**, execute the command: `openupm add me.freedee.text-fx` in the root directory of your game.

1. Open your game window in Unity. **TextFx** will be available to use in your project.

## Install via Git URL

To include **TextFx** in your game via **Git URL**, follow these steps:

1. Open the **Package Manager** in Unity via **Window > Package Manager**.

1. Click the **+** button in the top-left corner and select **Add package from git URL...**.

1. Enter the following URL, replacing `v1.0.1` with your desired version: `https://github.com/kay-af/TextFx.git#v1.0.1-git`.

1. Click **Add** and wait for Unity to download and install the package.

### Install Manually

To install **TextFx** manually, follow these steps:

1. Download the latest release from the [GitHub Releases](https://github.com/kay-af/TextFx/releases) page.

1. Extract the downloaded archive.

1. Copy the `me.freedee.text-fx` folder into the `Packages` folder of your Unity project.

1. Open your project in Unity. **TextFx** will be available to use in your project.

## Quick Start

1. Add **TextFxController** component to a **TextMeshPro** GameObject

1. Add a modifier group using the **+** button

1. Enable modifiers (Position, Rotation, Scale, etc.)

1. Animate using your preferred tweening library (e.g., [DoTween](https://dotween.demigiant.com/))

## Documentation

Refer to the full [Documentation](https://text-fx.freedee.me) for more information.

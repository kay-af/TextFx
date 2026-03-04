# Overview

Welcome to the **TextFx** documentation.

## Introduction

**TextFx** is an open-source **Unity** package for animating texts, inspired by [Rive's](https://rive.app/) text animation system. It is designed to work with _TextMeshPro_ and provides a structured way to rig character properties such as position, rotation, scale, and more. **TextFx** offers a robust programmatic API, allowing you to use your favorite tweening library to create beautiful text animations.

<video width="100%" autoplay loop muted>
  <source src="/assets/preview.webm" type="video/webm">
</video>

## Features overview

1. **Non-Destructive Workflow** – Rigs are constructed using layered modifier groups that are composable and non-destructive in nature.

1. **Control** - Precise control over which characters to target in the text and how they behave.

1. **Editor-friendly** - Editor support for visualizing the controls in the scene view.

1. **Multi-Line Support** - Multiple lines with different text alignments are supported.

1. **RTL Support** - Fully compatible with right-to-left languages.

1. **Programmatic API** – Includes a programmatic API that allows full runtime configuration and simplifies the creation of custom tools.

## Working

Rigs in **TextFx** are configured using modifier groups. Each group controls how a particular property of the text changes with fine grained control over the range of characters affected. Multiple groups can be combined to create more complex effects. This makes it possible to build anything from simple fades and typewriter-style effects to layered animations that affect characters, words, or lines differently.

> [!NOTE]
> **TextFx** itself does not provide an API for tweening parameters. It simply provides a way to rig your texts. It is recommended to use a library such as [DoTween](https://dotween.demigiant.com/) to drive the controller values. You can also use coroutines for simple animations.

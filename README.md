# Agent Animation

This repository contains a sample Unity project that demonstrates a simple command–based animation system. The project provides tools for composing scripted sequences of actions and a custom editor window for authoring timelines.

## Features

- **Command objects** – Each animation is represented by a `RobotCommand` implementation. The built‑in commands include movement, rotation, jumping and colour or transparency changes.
- **Sequences and timelines** – Commands can be executed sequentially using `RobotCommandSequence` or with explicit start times using `RobotCommandTimeline`.
- **Robot Executor** – The `RobotExecutor` component plays sequences or timelines on a chosen GameObject. It supports looping and time scaling.
- **Timeline Editor window** – A custom editor window (Window ▸ Robot Timeline Editor) allows you to visually create and preview timed commands directly inside the Unity Editor.
- **Serialization helpers** – Sequences and timelines can be saved to and loaded from JSON files using the provided utility classes.
- **Sample scene** – The `Assets/Scenes/SampleScene` scene shows a simple robot GameObject driven by these scripts.

## Requirements

- Unity **2022.3.58f1** or later. All necessary packages are listed in `Packages/manifest.json`.

## Getting Started

1. Clone or download this repository.
2. Open the project folder `Animation` with Unity Hub and launch it using the required Unity version.
3. Open `Scenes/SampleScene` to see a basic example.
4. Create or open a `RobotCommandTimeline` asset and use **Window ▸ Robot Timeline Editor** to add commands.
5. Add a `RobotExecutor` component to a GameObject and assign your sequence or timeline to preview it at runtime or from the editor.

The system is intentionally small and easy to extend – implement new `RobotCommand` classes to add more behaviours. See the `Commands` folder for examples.


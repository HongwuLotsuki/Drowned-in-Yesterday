# Code Samples

Selected scripts from **Drowned in Yesterday**, a 2D scrolling puzzle game made by a team in a game jam. I wrote all of the game's code and produced the builds. The full Unity project is not published because it contains teammates' art and other third-party assets.

> **AI assistance:** Some code was written with AI assistance. I was responsible for the design, integration and debugging.

## What to read first

| Feature | File | Highlights |
|---|---|---|
| Time record and rewind | [`Time/TimeManager.cs`](Time/TimeManager.cs), [`Time/TimeMemory.cs`](Time/TimeMemory.cs) | `F` records the position, rotation and time of every registered object, `R` rewinds them. Objects register themselves on enable, with a scene-wide fallback scan to avoid an `OnEnable` race. Rigidbodies (2D or 3D) are zeroed and put to sleep on restore. |
| Rewind meets rising water | [`Time/WaterController.cs`](Time/WaterController.cs) | Drives the water shader through a `MaterialPropertyBlock`, and reproduces the shader's wave function on the CPU in `IsUnderwater`. `TimeManager` uses it to reject a rewind whose target position was underwater at the recorded time. |
| Bell puzzle | [`Puzzles/BellPuzzleManager.cs`](Puzzles/BellPuzzleManager.cs) | Sequence puzzle with correct, wrong and solved states, audio and animation feedback, and a solved `UnityEvent`. The correct order can be set at runtime. |
| Shuffled clues | [`Puzzles/ClueManager.cs`](Puzzles/ClueManager.cs) | Fisher-Yates shuffle of the clue order, rebuilds the clue UI and syncs the new order into the bell puzzle, so every run has a different solution. |
| Ring puzzle | [`Puzzles/RingPuzzleManager.cs`](Puzzles/RingPuzzleManager.cs) | Three concentric rings rotated in 45° steps. Angle normalization with `Mathf.DeltaAngle`, per-ring color feedback, and an optional pre-solved ring for difficulty tuning. |
| Dialog system | [`Dialog/DialogManager.cs`](Dialog/DialogManager.cs) | Listens to a `ScriptableObject` event channel, looks up text by ID in a dialog database (or shows plain text), with fade in and out and an optional typewriter effect. |

## Design notes

**Rising water as a clock, rewind as a tool.** Water rises over 120 seconds and the player drowns if their head goes under. Rewinding looks like an easy escape, so it has to respect the water: a rewind is rejected if the recorded spot is underwater when the record was made. That check needs the same wave shape the shader draws, so the shader's math is duplicated on the CPU.

**Decoupled puzzles.** Every puzzle solves through a `UnityEvent` that a separate reward component listens to, so puzzle logic does not know what it unlocks. The panels follow one interaction pattern: approach, press `E`, lock player input while open, `Esc` to close.

**Event-channel dialog.** Gameplay scripts raise a dialog event and never touch the UI. The dialog manager listens to the channel and decides how to show the text.

**Known issues from my own review.** I did a static review of the project and recorded defects, for example static state (`PlayerController.InputLocked`) surviving a scene reload and causing a soft lock, and a chest that could grant its reward repeatedly. These are what I would fix first in a longer project: reset static state on restart, and guard one-time rewards with a "claimed" flag.

## Dependencies

These scripts reference classes that are not included, so they will not compile on their own: `PlayerController`, `DialogEventSO`, `DialogDatabaseSO`, plus the water shader. Treat them as reading material.

Unity 6000.3.19f1 · URP 2D

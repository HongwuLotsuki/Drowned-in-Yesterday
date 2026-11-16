# Drowned in Yesterday

A 2D scrolling puzzle game made by a team in a game jam.

**▶ Play:** [Download the Windows build from Releases](https://github.com/HongwuLotsuki/Drowned-in-Yesterday/releases/tag/1.0.0)

## Concept

Water rises through the room and you have 120 seconds before it reaches your head. Solve the bell and ring puzzles, search the closet, and collect five memory bubbles to escape. You can record your position and rewind to it, but not to a spot that was underwater when you recorded it.

Controls: `A` / `D` move, `Space` jump, `W` / `S` climb ladders, `E` interact, `F` record, `R` rewind.

## My role

All programming and builds: puzzle systems, time record and rewind, water interaction, dialog system, UI flow and release builds.

## Code samples

Selected scripts are in [`CodeSamples/`](CodeSamples/), with notes on the design. The full project contains teammates' art and third-party assets, so it is not published.

| Feature | File |
|---|---|
| Time record and rewind | [TimeManager.cs](CodeSamples/Time/TimeManager.cs) |
| Rewind vs rising water | [WaterController.cs](CodeSamples/Time/WaterController.cs) |
| Bell puzzle | [BellPuzzleManager.cs](CodeSamples/Puzzles/BellPuzzleManager.cs) |
| Event-channel dialog | [DialogManager.cs](CodeSamples/Dialog/DialogManager.cs) |

> Some code was written with AI assistance. I was responsible for the design, integration and debugging.

## Tech

Unity 6000.3.19f1 · URP 2D

## Portfolio

[HongwuLotsuki.github.io](https://hongwulotsuki.github.io/)

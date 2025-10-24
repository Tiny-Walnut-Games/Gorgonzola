# Gorgonzola: NecroHAMSTO vs Cute Snakes

A **turn-based survivors‑io prototype** built in Unity 6.2+ using the **TopDown Engine** framework.

**Game Jam Scope**: 48-hour game jam prototype.  
**Theme**: NecroHAMSTO vs Cute Snakes (isekai twist).

---

## Overview

You play as **NecroHAMSTO**, a necromantic hamster guarding a mystical **Phylactery** deep in a dungeon overrun by cute snakes. This is a turn-based tactical game where you must survive waves of enemies, spawn minions on death, and defend the Burrow at all costs.

### Core Mechanics

- **Turn-Based Combat**: Each turn has three distinct phases:
  1. **Player Phase**: Move, attack, or use ability.
  2. **Enemy Phase**: Snakes move and attack.
  3. **Minion Phase**: Ghosts/Zombies update their AI.

- **Death & Resurrection**: When NecroHAMSTO falls, they respawn at the Phylactery, leaving behind a **GhostHamsto** and spawning a **ZombieHamsto** to fight back.

- **Lose Condition**: If the Phylactery is destroyed → Game Over.

---

## Entities

| Entity | Role | Abilities |
|--------|------|-----------|
| **NecroHAMSTO** | Player character | Basic attack (sunflower seed), *Squeak of the Damned* (AoE knockback) |
| **Phylactery** | Stationary objective | Health-based; destruction = game over |
| **Ribbon Snake** | Fast enemy | Moves straight at player, low health |
| **Boa Hugger** | Crowd control | Slow, immobilizes on contact |
| **GhostHamsto** | Ally minion | Drifts randomly, blocks enemy movement |
| **ZombieHamsto** | Ally minion | Shambles toward nearest snake, bites once, decays |

---

## Project Structure

```
Assets/
├── Scripts/
│   ├── _Core/                 # Core systems (TurnManager, Bootstrap)
│   ├── Entities/              # Player, enemies, minions
│   ├── Systems/               # Turn system, damage, UI
│   └── TopDownEngine.asmdef   # Assembly definition for TopDownEngine integration
├── Scenes/
│   ├── Main.unity             # Main game scene (arena)
│   └── GameOver.unity         # Game over / isekai flavor screen
├── Prefabs/
│   ├── Player/
│   ├── Enemies/
│   ├── Minions/
│   └── Effects/
├── Art/                       # Sprites, animations (using Suriyun + Cute Series assets)
├── Audio/                     # SFX (squeak, bite, ghost wails)
└── Tests/
    ├── PlayMode/              # PlayMode test scaffold
    └── EditMode/              # EditMode test scaffold
```

---

## Quick Start

### Prerequisites
- **Unity 6.2.6+** (6000.2.6f2)
- **TopDown Engine** v4.4 (already imported)
- **C# 10+** support

### Setup
1. Clone/open this repo in Unity.
2. Open `Assets/Scenes/Main.unity`.
3. Press **Play** in the Editor.

### Development Commands
```bash
# Run tests (via Unity Test Runner)
# Window > General > Test Runner
```

---

## Design Pillars

- **Turn-Based**: No real-time pressure; strategic positioning and ability use.
- **Asymmetric Gameplay**: Mix of defensive (Phylactery) and offensive (abilities) mechanics.
- **Narrative Flavor**: Isekai twist—death is respawning, not ending.
- **Quick Iterations**: Single-screen arena, reusable snake AI patterns.

---

## Stretch Goals

- [ ] **Glitter Cobra**: Ranged charm (confusion) effect.
- [ ] **Screen Shake & SFX**: Squeak, bite, and ghostly ambience.
- [ ] **Particle Effects**: Ghostly trails, death sparkles.
- [ ] **Isekai Meta-Screen**: Flavor text hinting at future realms.
- [ ] **Score System**: Wave counter, entity kill count.

---

## Jam Notes

- **Day 1**: Core scaffolding, TurnManager, NecroHAMSTO controller, basic snake AI.
- **Day 2**: Polish, minion AI, lose condition, Game Over screen, juice.

---

## License

MIT — See `LICENSE` file.

---

**Built with ❤️ by Tiny Walnut Games**
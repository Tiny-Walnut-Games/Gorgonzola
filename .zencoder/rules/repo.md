---
description: Repository Information Overview
alwaysApply: true
---

# Gorgonzola Game Information

## Summary
Gorgonzola is a top-down game developed by Tiny Walnut Games using the Unity engine. It's built on the TopDown Engine framework (v4.4) which provides a complete solution for creating 2D and 3D top-down games with features like character controllers, enemy AI, health management, weapons systems, and more.

## Structure
- **Assets/**: Contains all game assets including scenes, scripts, models, and third-party assets
- **ProjectSettings/**: Unity project configuration files
- **Packages/**: Unity package dependencies and manifest
- **Library/**: Unity-generated cache files (not version controlled)
- **Temp/**: Temporary build files
- **obj/**: Intermediate build files

## Language & Runtime
**Engine**: Unity 6000.2.6f2
**Language**: C# 9.0
**Framework**: .NET 4.7.1
**Build System**: MSBuild
**IDE Support**: Visual Studio, Rider

## Dependencies
**Core Unity Packages**:
- Universal Render Pipeline (17.2.0)
- Input System (1.14.2)
- Cinemachine (3.1.5)
- Post-processing (3.5.1)
- Visual Scripting (1.9.9)
- AI Navigation (2.0.9)

**Third-Party Assets**:
- TopDown Engine (v4.4)
- InventoryEngine
- PolygonDungeon
- Monsters Ultimate Pack 02
- Suriyun (character assets)

## Build & Installation
The project uses standard Unity build process:
```bash
# Open in Unity Editor
# Build via Unity Editor: File > Build Settings > Build
# For development builds, enable Development Build option
```

## Main Components
**TopDown Engine**: Core framework providing character controllers, AI, weapons, and game mechanics
**InventoryEngine**: Handles item and inventory management
**Interface System**: UI framework for menus and HUD elements
**Input System**: Uses Unity's new Input System package with custom action maps

## Testing
**Framework**: Unity Test Framework (1.6.0) for unit tests, Playwright for E2E testing
**Test Location**: 
- Unit tests: Located within the project's test assemblies
- E2E tests: Located in `tests/e2e/` directory
- TopDown Engine validation: `tests/e2e/topdown-engine-validation.spec.ts`

**Key E2E Tests**:
- **Turn-based mechanics**: Player → Enemy → Minion → Resolution phases
- **Combat system**: Sunflower seed projectile weapons with health reduction
- **Special abilities**: "Squeak of the Damned" AoE knockback effects
- **Death/respawn system**: GhostHamsto and ZombieHamsto spawning mechanics
- **TopDown Engine components**: Validation of all core framework components

**Run Commands**:
```bash
# Run unit tests via Unity Test Runner window
# Window > General > Test Runner

# Run all E2E tests with Playwright
npx playwright test tests/e2e/ --reporter=line

# Run TopDown Engine validation specifically  
npx playwright test tests/e2e/topdown-engine-validation.spec.ts --reporter=line
```

## Scene Setup Scripts
**QuickPlaySceneSetup.cs**: Enhanced one-click arena setup with TopDown Engine component fallback
**TopDownEngineSceneSetup.cs**: Complete game-jam-ready scene generation with full component hierarchy

Creates complete playable scenes with:
- **Managers**: TurnManager, LevelManager, GameManager, InputManager, SoundManager
- **Arena Environment**: Ground, spawn points, lighting
- **NecroHAMSTO Player**: Full TopDown Engine component stack (TopDownController3D, CharacterMovement, Health, Weapons)
- **Snake Enemies**: RibbonSnake, BoaHugger, GlitterCobra with proper AI brains
- **Phylactery**: Game-over condition object with Health component
- **Camera System**: Cinemachine integration with player following
- **UI System**: Turn indicators and game state display
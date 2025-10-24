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
**Framework**: Unity Test Framework (1.6.0)
**Test Location**: Tests are located within the project's test assemblies
**Run Command**:
```bash
# Run tests via Unity Test Runner window
# Window > General > Test Runner
```
# ABFPSGame

A first-person shooter game with a sci-fi theme set in a space station.

## Overview

ABFPSGame is a complete Unity FPS project featuring:
- **Level 1**: Space station environment with corridors leading to a reactor core
- **Weapons**: Pistol, Shotgun, Plasma Rifle with unique characteristics
- **Enemies**: AI-controlled enemies with patrol, chase, and attack behaviors
- **UI**: Full HUD with health, ammo, minimap, and damage indicators
- **Audio**: Spatial sound effects and background music
- **Pickups**: Health packs and ammo crates

## Requirements

- **Unity Version**: 2021.3 LTS or newer
- **Platform**: Windows/Mac/Linux

## Setup Instructions

1. Clone the repository:
   ```bash
   git clone https://github.com/rahil-algobear/ABFPSGame.git
   ```

2. Open Unity Hub

3. Click "Add" and navigate to the cloned project folder

4. Select the project and open it with Unity 2021.3 LTS or newer

5. Wait for Unity to import all assets

6. Open the main scene: `Assets/Scenes/Level1.unity`

7. Press Play in the Unity Editor

## Controls

- **WASD**: Movement
- **Mouse**: Look around
- **Left Mouse Button**: Fire weapon
- **R**: Reload
- **Right Mouse Button**: Aim down sights
- **Space**: Jump
- **Left Shift**: Sprint
- **1/2/3**: Switch weapons (Pistol/Shotgun/Plasma Rifle)
- **ESC**: Pause menu

## Architecture Overview

### Core Systems

- **GameManager**: Singleton managing game state, score, and level progression
- **AudioManager**: Centralized audio playback with SFX and music channels
- **UIManager**: Handles all UI updates and transitions

### Player System

- **PlayerController**: CharacterController-based FPS movement
- **MouseLook**: Smooth camera control with sensitivity settings
- **PlayerHealth**: Health management with regeneration and damage effects

### Weapon System

- **WeaponBase**: Abstract base class for all weapons
- **WeaponData**: ScriptableObject storing weapon statistics
- **WeaponManager**: Handles weapon switching, ammo, and firing
- **Projectile**: Raycast-based hit detection with damage falloff

### Enemy System

- **EnemyBase**: Base enemy class with health and damage
- **EnemyAI**: State machine (Patrol → Alert → Chase → Attack)
- **EnemySpawner**: Wave-based enemy spawning

### Level System

- **LevelGenerator**: Procedural space station layout
- **Minimap**: Real-time player and enemy tracking

## Project Structure

```
Assets/
├── Scenes/
│   └── Level1.unity
├── Scripts/
│   ├── Core/
│   ├── Player/
│   ├── Weapons/
│   ├── Enemies/
│   ├── UI/
│   ├── Level/
│   └── Pickups/
├── Prefabs/
├── Materials/
├── ScriptableObjects/
└── Audio/
```

## Known Limitations

- Level geometry is procedurally generated using primitives
- Audio files are referenced but not included (add your own audio clips)
- Minimap uses orthographic camera overlay
- NavMesh must be baked after level generation (Window → AI → Navigation)

## Future Enhancements

- Additional weapon types
- More enemy varieties
- Multiple levels
- Save/load system
- Multiplayer support

## License

MIT License - Feel free to use and modify for your projects.

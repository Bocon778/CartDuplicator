# Cart Duplicator Mod

Cart Duplicator is a mod for duplicating carts in the game. This mod allows you to duplicate carts with a configurable delay and offset.

## Features

- Duplicate carts with a configurable delay.
- Set the offset for the duplicated cart position.

## Installation

1. Download and install [BepInEx](https://thunderstore.io/c/repo/p/BepInEx/BepInExPack/).
2. Place the `CartDuplicator.dll` file in the `BepInEx/plugins` directory.

## Configuration

After the first run, a configuration file will be generated at `BepInEx/config/Bocon.CartDuplicator.cfg`. You can edit this file to change the mod settings.

### Configuration Options

- **DuplicationAmount**: Number of additional carts to duplicate. Default is `1.0`.
- **DuplicationOffset**: Offset for the duplicated cart position. Default is `(2.0, 0.0, 0.0)`.

## Usage

The mod will automatically duplicate carts when the room generation starts. The duplicated cart will appear at the specified offset from the original cart after the specified delay.

# Name Generator Engine Demo

This console application demonstrates the key features of the NameGeneratorEngine library.

## Running the Demo

```bash
dotnet run --project demo/NameGeneratorDemo/NameGeneratorDemo.csproj
```

## What the Demo Shows

### Demo 1: Basic Name Generation
Shows how to create a generator with a seed and generate names for different entity types across all themes.

### Demo 2: Building Types
Demonstrates all 7 building types (Residential, Commercial, Industrial, Government, Entertainment, Medical, Educational).

### Demo 3: Deterministic Generation
Proves that two generators with the same seed produce identical name sequences.

### Demo 4: Uniqueness Within Session
Shows that the library prevents duplicate names within a single session.

### Demo 5: Session Reset
Demonstrates how `ResetSession()` clears tracking and allows the same sequence to be generated again.

### Demo 6: Theme Showcase
Displays sample names from all three themes (Cyberpunk, Elves, Orcs) for all entity types.

## Expected Output

The demo generates procedural names showing:
- Cyberpunk names with tech-inspired elements
- Elf names with soft, flowing syllables
- Orc names with harsh, guttural sounds
- Deterministic behavior with seed values
- Unique name generation within sessions

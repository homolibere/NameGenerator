# Name Generator Engine

A C# library for procedural name generation supporting multiple themes and entity types with deterministic, seeded generation.

## Features

- **Deterministic Generation**: Uses seed values to produce reproducible name sequences across executions
- **Multiple Themes**: Supports Cyberpunk, Elves, and Orcs themes with distinct naming conventions
- **Entity Types**: Generates names for NPCs, buildings, cities, districts, streets, and factions
- **Duplicate Prevention**: Ensures unique names within a generation session
- **Gender Support**: Generate NPC names with male, female, or neutral gender options
- **Building Types**: Generate contextually appropriate building names (residential, commercial, industrial, government, entertainment, medical, educational)
- **Multi-targeting**: Supports both .NET 8.0 and .NET 10.0
- **Clean API**: Intuitive, well-documented public interface

## Installation

Install the package via NuGet Package Manager:

```bash
dotnet add package NameGeneratorEngine
```

Or via the NuGet Package Manager Console:

```powershell
Install-Package NameGeneratorEngine
```

Or add it directly to your `.csproj` file:

```xml
<ItemGroup>
  <PackageReference Include="NameGeneratorEngine" Version="1.0.0" />
</ItemGroup>
```

## Basic Usage

```csharp
using NameGeneratorEngine;
using NameGeneratorEngine.Enums;

// Create a generator (uses random seed)
var generator = new NameGenerator();

// Generate names for different entity types
string npcName = generator.GenerateNpcName(Theme.Cyberpunk, Gender.Female);
string cityName = generator.GenerateCityName(Theme.Elves);
string buildingName = generator.GenerateBuildingName(Theme.Orcs, BuildingType.Commercial);
string districtName = generator.GenerateDistrictName(Theme.Cyberpunk);
string streetName = generator.GenerateStreetName(Theme.Elves);
string factionName = generator.GenerateFactionName(Theme.Cyberpunk);

Console.WriteLine($"NPC: {npcName}");
Console.WriteLine($"City: {cityName}");
Console.WriteLine($"Building: {buildingName}");
Console.WriteLine($"District: {districtName}");
Console.WriteLine($"Street: {streetName}");
Console.WriteLine($"Faction: {factionName}");
```

## Deterministic Generation

Use a seed value to generate reproducible name sequences:

```csharp
// Create two generators with the same seed
var generator1 = new NameGenerator(seed: 42);
var generator2 = new NameGenerator(seed: 42);

// Both will produce identical sequences
string name1 = generator1.GenerateNpcName(Theme.Cyberpunk, Gender.Male);
string name2 = generator2.GenerateNpcName(Theme.Cyberpunk, Gender.Male);

Console.WriteLine(name1 == name2); // True

// Access the current seed
Console.WriteLine($"Using seed: {generator1.Seed}");
```

This is particularly useful for:
- Game world generation that needs to be consistent across sessions
- Debugging and testing
- Sharing reproducible results with others

## Session Reset

Reset the generator to allow previously generated names to be reused:

```csharp
var generator = new NameGenerator(seed: 100);

// Generate some names
var firstBatch = new List<string>
{
    generator.GenerateCityName(Theme.Elves),
    generator.GenerateCityName(Theme.Elves),
    generator.GenerateCityName(Theme.Elves)
};

// Reset the session
generator.ResetSession();

// Generate again - may include names from firstBatch
var secondBatch = new List<string>
{
    generator.GenerateCityName(Theme.Elves),
    generator.GenerateCityName(Theme.Elves),
    generator.GenerateCityName(Theme.Elves)
};

// With the same seed, the sequences will be identical
Console.WriteLine("First batch:");
firstBatch.ForEach(Console.WriteLine);

Console.WriteLine("\nSecond batch (after reset):");
secondBatch.ForEach(Console.WriteLine);
```

## Error Handling

The library provides comprehensive error handling with descriptive messages:

### Invalid Parameters

```csharp
var generator = new NameGenerator();

try
{
    // Invalid theme value
    string name = generator.GenerateNpcName((Theme)999, Gender.Male);
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Invalid parameter: {ex.Message}");
    // Output: "Invalid theme value: 999. Expected one of: Cyberpunk, Elves, Orcs"
}
```

### Name Pool Exhaustion

```csharp
var generator = new NameGenerator(seed: 1);

try
{
    // Generate many names until pool exhaustion
    for (int i = 0; i < 10000; i++)
    {
        generator.GenerateCityName(Theme.Orcs);
    }
}
catch (NamePoolExhaustedException ex)
{
    Console.WriteLine($"Pool exhausted: {ex.Message}");
    Console.WriteLine($"Entity Type: {ex.EntityType}");
    Console.WriteLine($"Theme: {ex.Theme}");
    Console.WriteLine($"Attempts: {ex.Attempts}");
    
    // Solution: Reset the session to clear tracking
    generator.ResetSession();
    
    // Now you can generate more names
    string newName = generator.GenerateCityName(Theme.Orcs);
}
```

## Thread Safety

**Important**: The `NameGenerator` class is **NOT thread-safe**. Each instance should be used by a single thread only.

If you need to generate names from multiple threads:

```csharp
// Option 1: Create separate instances per thread
var thread1Generator = new NameGenerator(seed: 42);
var thread2Generator = new NameGenerator(seed: 43);

// Option 2: Use thread-local storage
private static ThreadLocal<NameGenerator> _generator = 
    new ThreadLocal<NameGenerator>(() => new NameGenerator());

// Access the thread-local generator
string name = _generator.Value.GenerateCityName(Theme.Cyberpunk);
```

## API Reference

### NameGenerator Class

#### Constructor

```csharp
public NameGenerator(int? seed = null)
```

Creates a new name generator instance. If no seed is provided, a random seed is generated.

#### Properties

```csharp
public int Seed { get; }
```

Gets the seed value being used for the current session.

#### Methods

```csharp
public string GenerateNpcName(Theme theme, Gender? gender = null)
```

Generates an NPC name. If gender is not specified, a random gender is selected.

```csharp
public string GenerateBuildingName(Theme theme, BuildingType? buildingType = null)
```

Generates a building name. If building type is not specified, a generic building name is generated.

```csharp
public string GenerateCityName(Theme theme)
```

Generates a city name for the specified theme.

```csharp
public string GenerateDistrictName(Theme theme)
```

Generates a district name for the specified theme.

```csharp
public string GenerateStreetName(Theme theme)
```

Generates a street name for the specified theme.

```csharp
public string GenerateFactionName(Theme theme)
```

Generates a faction name for the specified theme.

```csharp
public void ResetSession()
```

Resets the generation session, clearing all tracked names and allowing previously generated names to be reused.

### Enumerations

#### Theme

```csharp
public enum Theme
{
    Cyberpunk,  // Tech-inspired, corporate, dystopian names
    Elves,      // Melodic, flowing names with soft consonants
    Orcs        // Harsh, guttural names with hard consonants
}
```

#### Gender

```csharp
public enum Gender
{
    Male,
    Female,
    Neutral
}
```

#### BuildingType

```csharp
public enum BuildingType
{
    Residential,
    Commercial,
    Industrial,
    Government,
    Entertainment,
    Medical,
    Educational
}
```

### Exceptions

#### NamePoolExhaustedException

Thrown when the generator cannot produce a unique name after 1000 attempts.

**Properties:**
- `EntityType`: The entity type being generated
- `Theme`: The theme being used
- `Attempts`: Number of attempts made

**Solution:** Call `ResetSession()` to clear tracked names, or use a different seed.

## Theme Characteristics

### Cyberpunk
- **Style**: Tech-inspired, corporate, dystopian
- **Phonetics**: Tech terms, numbers, corporate identifiers
- **Examples**: 
  - NPCs: "Nexora", "Zerox", "Cyra"
  - Cities: "NeoCity", "TechHaven", "CyberPrime"
  - Streets: "DataLink Street", "Neural Avenue"
  - Factions: "MegaCorp Syndicate", "Quantum Collective", "OmniSystems"

### Elves
- **Style**: Melodic, flowing, nature-inspired
- **Phonetics**: Soft consonants (L, V, F, TH), flowing vowels
- **Examples**:
  - NPCs: "Thalindra", "Vaelorn", "Aelindor"
  - Cities: "Silvanthir", "Lorendell", "Faeloria"
  - Streets: "Moonwhisper Path", "Starlight Way"
  - Factions: "Silverleaf Council", "Moonstone Circle", "Starlight Order"

### Orcs
- **Style**: Harsh, guttural, territorial
- **Phonetics**: Hard consonants (K, G, R, GH), harsh vowels
- **Examples**:
  - NPCs: "Grok", "Thrak", "Urgoth"
  - Cities: "Kragmor", "Grothak", "Thrakkar"
  - Streets: "Ironjaw Road", "Bloodstone Way"
  - Factions: "Bloodfang Clan", "Ironskull Horde", "Grimrock Warband"

## Contributing

Contributions are welcome! Here are some ways you can contribute:

### Adding New Themes

1. Create a new JSON file in `src/NameGeneratorEngine/ThemeData/` following the existing structure
2. Add the theme to the `Theme` enum in `src/NameGeneratorEngine/Enums/Theme.cs`
3. Ensure the JSON file is marked as an embedded resource in the `.csproj` file
4. Add unit tests for the new theme
5. Update this README with the new theme's characteristics

### Reporting Issues

If you find a bug or have a feature request:
1. Check if the issue already exists in the issue tracker
2. Provide a clear description and reproduction steps
3. Include the library version and .NET runtime version

### Pull Requests

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Make your changes
4. Add or update tests as needed
5. Ensure all tests pass (`dotnet test`)
6. Commit your changes (`git commit -m 'Add amazing feature'`)
7. Push to the branch (`git push origin feature/amazing-feature`)
8. Open a Pull Request

## License

This project is licensed under the MIT License.

## Requirements

- .NET 8.0 or .NET 10.0
- No external dependencies (uses only System libraries)

## Support

For questions, issues, or feature requests, please open an issue on the GitHub repository.

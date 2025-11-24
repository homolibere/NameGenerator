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

## Basic Usage

### Using Built-in Themes

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

## Custom Themes

The library supports registering custom themes at runtime, allowing you to create unlimited themes beyond the built-in Cyberpunk, Elves, and Orcs themes.

### Creating Custom Themes Programmatically

Use the `ThemeDataBuilder` to construct custom themes with a fluent API:

```csharp
using NameGeneratorEngine;

// Create a custom Steampunk theme
var steampunkTheme = new ThemeDataBuilder()
    .WithNpcNames(npc => npc
        .WithMaleNames(
            prefixes: new[] { "Brass", "Copper", "Iron", "Steel", "Gear" },
            cores: new[] { "worth", "smith", "wright", "ton", "ford" },
            suffixes: new[] { "son", "ley", "by", "ham", "field" })
        .WithFemaleNames(
            prefixes: new[] { "Cog", "Spring", "Valve", "Piston", "Steam" },
            cores: new[] { "belle", "rose", "grace", "anne", "lyn" },
            suffixes: new[] { "ia", "ette", "ina", "ara", "elle" })
        .WithNeutralNames(
            prefixes: new[] { "Mech", "Auto", "Gyro", "Turbo", "Aero" },
            cores: new[] { "spark", "bolt", "wire", "coil", "spring" },
            suffixes: new[] { "ix", "ax", "ex", "on", "um" }))
    .WithBuildingNames(building => building
        .WithGenericNames(
            prefixes: new[] { "The Brass", "The Copper", "The Iron", "The Steam" },
            suffixes: new[] { "Works", "Factory", "Foundry", "Mill", "Forge" })
        .WithTypeNames(BuildingType.Commercial,
            prefixes: new[] { "Clockwork", "Steamwork", "Gearwork" },
            descriptors: new[] { "Trading", "Merchant", "Supply" },
            suffixes: new[] { "Company", "Emporium", "Exchange" }))
    .WithCityNames(
        prefixes: new[] { "New", "Old", "Upper", "Lower", "Greater" },
        cores: new[] { "Brass", "Copper", "Steam", "Gear", "Cog" },
        suffixes: new[] { "ton", "ville", "burg", "port", "haven" })
    .WithDistrictNames(
        descriptors: new[] { "Industrial", "Factory", "Foundry", "Workshop", "Mechanical" },
        locationTypes: new[] { "Quarter", "District", "Ward", "Zone", "Sector" })
    .WithStreetNames(
        prefixes: new[] { "Brass", "Copper", "Iron", "Steam", "Gear" },
        cores: new[] { "smith", "work", "forge", "mill", "foundry" },
        streetSuffixes: new[] { "Street", "Avenue", "Road", "Lane", "Way" })
    .WithFactionNames(
        prefixes: new[] { "The", "United", "Royal", "Imperial", "Grand" },
        cores: new[] { "Clockwork", "Steamwork", "Gearwork", "Mechanist", "Engineer" },
        suffixes: new[] { "Guild", "Society", "Association", "Brotherhood", "Order" })
    .Build();

// Register the custom theme
var generator = new NameGenerator();
generator.RegisterCustomTheme("steampunk", steampunkTheme);

// Generate names using the custom theme
string npcName = generator.GenerateNpcName("steampunk", Gender.Male);
string cityName = generator.GenerateCityName("steampunk");
string buildingName = generator.GenerateBuildingName("steampunk", BuildingType.Commercial);

Console.WriteLine($"NPC: {npcName}");
Console.WriteLine($"City: {cityName}");
Console.WriteLine($"Building: {buildingName}");
```

### Loading Custom Themes from JSON

Load custom themes from external JSON files:

```csharp
// Load theme from a JSON file
var customTheme = CustomThemeData.FromJson("path/to/steampunk.json");

var generator = new NameGenerator();
generator.RegisterCustomTheme("steampunk", customTheme);

// Use the loaded theme
string name = generator.GenerateNpcName("steampunk", Gender.Female);
```

JSON format example (`steampunk.json`):

```json
{
  "npcNames": {
    "male": {
      "prefixes": ["Brass", "Copper", "Iron", "Steel", "Gear"],
      "cores": ["worth", "smith", "wright", "ton", "ford"],
      "suffixes": ["son", "ley", "by", "ham", "field"]
    },
    "female": {
      "prefixes": ["Cog", "Spring", "Valve", "Piston", "Steam"],
      "cores": ["belle", "rose", "grace", "anne", "lyn"],
      "suffixes": ["ia", "ette", "ina", "ara", "elle"]
    },
    "neutral": {
      "prefixes": ["Mech", "Auto", "Gyro", "Turbo", "Aero"],
      "cores": ["spark", "bolt", "wire", "coil", "spring"],
      "suffixes": ["ix", "ax", "ex", "on", "um"]
    }
  },
  "buildingNames": {
    "generic": {
      "prefixes": ["The Brass", "The Copper", "The Iron"],
      "suffixes": ["Works", "Factory", "Foundry", "Mill"]
    },
    "residential": {
      "prefixes": ["Copper", "Brass", "Iron"],
      "descriptors": ["Heights", "Towers", "Estates"],
      "suffixes": ["Apartments", "Residences", "Dwellings"]
    }
  },
  "cityNames": {
    "prefixes": ["New", "Old", "Upper", "Lower"],
    "cores": ["Brass", "Copper", "Steam", "Gear"],
    "suffixes": ["ton", "ville", "burg", "port"]
  },
  "districtNames": {
    "descriptors": ["Industrial", "Factory", "Foundry"],
    "locationTypes": ["Quarter", "District", "Ward"]
  },
  "streetNames": {
    "prefixes": ["Brass", "Copper", "Iron"],
    "cores": ["smith", "work", "forge"],
    "streetSuffixes": ["Street", "Avenue", "Road"]
  },
  "factionNames": {
    "prefixes": ["The", "United", "Royal"],
    "cores": ["Clockwork", "Steamwork", "Gearwork"],
    "suffixes": ["Guild", "Society", "Association"]
  }
}
```

### Extending Existing Themes

Add more variety to built-in themes without replacing them entirely:

```csharp
// Extend the Cyberpunk theme with additional NPC names
var cyberpunkExtension = ThemeDataBuilder.Extend(Theme.Cyberpunk)
    .WithNpcNames(npc => npc
        .AddMalePrefixes("Neo", "Cyber", "Data", "Quantum", "Neural")
        .AddMaleCores("hack", "wire", "byte", "node", "link")
        .AddMaleSuffixes("son", "tech", "net", "core", "sys"))
    .BuildExtension();

var generator = new NameGenerator();
generator.ExtendTheme(Theme.Cyberpunk, cyberpunkExtension);

// Generated names now include both original and extended data
string name = generator.GenerateNpcName(Theme.Cyberpunk, Gender.Male);
// Could be "Neohackson" (using extension) or "Razorwire" (using original)
```

You can also extend custom themes:

```csharp
// First register a custom theme
var steampunkTheme = CustomThemeData.FromJson("steampunk.json");
generator.RegisterCustomTheme("steampunk", steampunkTheme);

// Then extend it
var steampunkExtension = ThemeDataBuilder.Extend("steampunk")
    .WithCityNames(
        prefixes: new[] { "Neo", "Ultra", "Mega" },
        cores: new[] { "Steam", "Gear", "Cog" },
        suffixes: new[] { "opolis", "city", "metropolis" })
    .BuildExtension();

generator.ExtendTheme("steampunk", steampunkExtension);
```

### Configuration-Based Initialization

Initialize the generator with all custom themes and extensions in one step:

```csharp
// Create a configuration with multiple custom themes and extensions
var config = new ThemeConfig()
    .AddTheme("steampunk", steampunkTheme)
    .AddThemeFromJson("fantasy", "path/to/fantasy.json")
    .ExtendTheme(Theme.Cyberpunk, cyberpunkExtension)
    .ExtendTheme("steampunk", steampunkExtension);

// Create generator with configuration
var generator = new NameGenerator(config, seed: 42);

// All themes are immediately available
string cyberpunkName = generator.GenerateNpcName(Theme.Cyberpunk, Gender.Male);
string steampunkName = generator.GenerateNpcName("steampunk", Gender.Female);
string fantasyName = generator.GenerateCityName("fantasy");
```

### Listing Available Themes

Get a list of all registered themes (built-in and custom):

```csharp
var generator = new NameGenerator();
generator.RegisterCustomTheme("steampunk", steampunkTheme);
generator.RegisterCustomTheme("fantasy", fantasyTheme);

var availableThemes = generator.GetAvailableThemes();
// Returns: ["Cyberpunk", "Elves", "Orcs", "steampunk", "fantasy"]

Console.WriteLine("Available themes:");
foreach (var theme in availableThemes)
{
    Console.WriteLine($"  - {theme}");
}
```

### Custom Theme Validation

The library validates all custom theme data to ensure completeness:

```csharp
try
{
    var invalidTheme = new ThemeDataBuilder()
        .WithNpcNames(npc => npc
            .WithMaleNames(
                prefixes: new[] { "Test" },
                cores: new string[] { },  // Empty array - invalid!
                suffixes: new[] { "son" }))
        .Build();
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Validation error: {ex.Message}");
    // Output: "NPC Male Cores array is empty. All arrays must contain at least one element."
}
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

#### Constructors

```csharp
public NameGenerator(int? seed = null)
```

Creates a new name generator instance. If no seed is provided, a random seed is generated.

```csharp
public NameGenerator(ThemeConfig? config, int? seed = null)
```

Creates a new name generator instance with custom theme configuration. Registers all custom themes and applies all extensions from the configuration.

#### Properties

```csharp
public int Seed { get; }
```

Gets the seed value being used for the current session.

#### Name Generation Methods

```csharp
public string GenerateNpcName(Theme theme, Gender? gender = null)
public string GenerateNpcName(string themeIdentifier, Gender? gender = null)
```

Generates an NPC name. If gender is not specified, a random gender is selected. Accepts either a built-in Theme enum or a custom theme identifier string.

```csharp
public string GenerateBuildingName(Theme theme, BuildingType? buildingType = null)
public string GenerateBuildingName(string themeIdentifier, BuildingType? buildingType = null)
```

Generates a building name. If building type is not specified, a generic building name is generated. Accepts either a built-in Theme enum or a custom theme identifier string.

```csharp
public string GenerateCityName(Theme theme)
public string GenerateCityName(string themeIdentifier)
```

Generates a city name for the specified theme. Accepts either a built-in Theme enum or a custom theme identifier string.

```csharp
public string GenerateDistrictName(Theme theme)
public string GenerateDistrictName(string themeIdentifier)
```

Generates a district name for the specified theme. Accepts either a built-in Theme enum or a custom theme identifier string.

```csharp
public string GenerateStreetName(Theme theme)
public string GenerateStreetName(string themeIdentifier)
```

Generates a street name for the specified theme. Accepts either a built-in Theme enum or a custom theme identifier string.

```csharp
public string GenerateFactionName(Theme theme)
public string GenerateFactionName(string themeIdentifier)
```

Generates a faction name for the specified theme. Accepts either a built-in Theme enum or a custom theme identifier string.

#### Custom Theme Management Methods

```csharp
public void RegisterCustomTheme(string identifier, CustomThemeData themeData)
```

Registers a custom theme with the specified identifier. The identifier is case-insensitive and cannot conflict with built-in theme names.

```csharp
public void ExtendTheme(Theme baseTheme, ThemeExtension extension)
public void ExtendTheme(string baseThemeIdentifier, ThemeExtension extension)
```

Extends an existing theme (built-in or custom) with additional data. Multiple extensions can be applied to the same theme.

```csharp
public IReadOnlyCollection<string> GetAvailableThemes()
```

Returns a list of all available theme identifiers, including both built-in themes (as strings) and registered custom themes.

#### Session Management

```csharp
public void ResetSession()
```

Resets the generation session, clearing all tracked names and allowing previously generated names to be reused.

### ThemeDataBuilder Class

Fluent builder for constructing custom theme data.

#### Creating New Themes

```csharp
public ThemeDataBuilder()
```

Creates a new builder for constructing a complete custom theme from scratch.

```csharp
public ThemeDataBuilder WithNpcNames(Action<NpcNameDataBuilder> configure)
```

Configures NPC name data for all genders (male, female, neutral).

```csharp
public ThemeDataBuilder WithBuildingNames(Action<BuildingNameDataBuilder> configure)
```

Configures building name data for generic and type-specific buildings.

```csharp
public ThemeDataBuilder WithCityNames(string[] prefixes, string[] cores, string[] suffixes)
public ThemeDataBuilder WithDistrictNames(string[] descriptors, string[] locationTypes)
public ThemeDataBuilder WithStreetNames(string[] prefixes, string[] cores, string[] streetSuffixes)
public ThemeDataBuilder WithFactionNames(string[] prefixes, string[] cores, string[] suffixes)
```

Configures name data for other entity types.

```csharp
public CustomThemeData Build()
```

Builds and validates the complete custom theme data.

#### Extending Existing Themes

```csharp
public static ThemeDataBuilder Extend(Theme baseTheme)
public static ThemeDataBuilder Extend(string baseThemeIdentifier)
```

Creates a builder for extending an existing theme (built-in or custom).

```csharp
public ThemeExtension BuildExtension()
```

Builds a theme extension that can be merged with the base theme.

### NpcNameDataBuilder Class

Builder for NPC name data with gender-specific configuration.

```csharp
public NpcNameDataBuilder WithMaleNames(string[] prefixes, string[] cores, string[] suffixes)
public NpcNameDataBuilder WithFemaleNames(string[] prefixes, string[] cores, string[] suffixes)
public NpcNameDataBuilder WithNeutralNames(string[] prefixes, string[] cores, string[] suffixes)
```

Sets complete name data for a specific gender (used when creating new themes).

```csharp
public NpcNameDataBuilder AddMalePrefixes(params string[] prefixes)
public NpcNameDataBuilder AddMaleCores(params string[] cores)
public NpcNameDataBuilder AddMaleSuffixes(params string[] suffixes)
// Similar methods for Female and Neutral
```

Adds additional syllables to existing gender data (used when extending themes).

### BuildingNameDataBuilder Class

Builder for building name data with generic and type-specific configuration.

```csharp
public BuildingNameDataBuilder WithGenericNames(string[] prefixes, string[] suffixes)
```

Sets generic building name data (used for all building types).

```csharp
public BuildingNameDataBuilder WithTypeNames(
    BuildingType type, 
    string[] prefixes, 
    string[] descriptors, 
    string[] suffixes)
```

Sets type-specific building name data (used when creating new themes).

```csharp
public BuildingNameDataBuilder AddGenericPrefixes(params string[] prefixes)
public BuildingNameDataBuilder AddGenericSuffixes(params string[] suffixes)
public BuildingNameDataBuilder AddTypePrefixes(BuildingType type, params string[] prefixes)
public BuildingNameDataBuilder AddTypeDescriptors(BuildingType type, params string[] descriptors)
public BuildingNameDataBuilder AddTypeSuffixes(BuildingType type, params string[] suffixes)
```

Adds additional data to existing building name data (used when extending themes).

### CustomThemeData Class

Immutable representation of custom theme data.

```csharp
public static CustomThemeData FromJson(string jsonFilePath)
```

Loads custom theme data from a JSON file.

```csharp
public static CustomThemeData FromJsonString(string jsonContent)
```

Loads custom theme data from a JSON string.

### ThemeExtension Class

Immutable representation of theme extension data. Created by `ThemeDataBuilder.BuildExtension()`.

### ThemeConfig Class

Configuration object for initializing NameGenerator with custom themes.

```csharp
public ThemeConfig AddTheme(string identifier, CustomThemeData themeData)
```

Adds a custom theme to the configuration.

```csharp
public ThemeConfig AddThemeFromJson(string identifier, string jsonFilePath)
```

Loads and adds a custom theme from a JSON file.

```csharp
public ThemeConfig ExtendTheme(Theme baseTheme, ThemeExtension extension)
public ThemeConfig ExtendTheme(string baseThemeIdentifier, ThemeExtension extension)
```

Adds a theme extension to the configuration.

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

### Adding New Built-in Themes

To add a new built-in theme to the library itself:

1. Create a new JSON file in `src/NameGeneratorEngine/ThemeData/` following the existing structure
2. Add the theme to the `Theme` enum in `src/NameGeneratorEngine/Enums/Theme.cs`
3. Ensure the JSON file is marked as an embedded resource in the `.csproj` file
4. Add unit tests and property tests for the new theme
5. Update this README with the new theme's characteristics

Note: For most use cases, you can create custom themes at runtime without modifying the library. See the "Custom Themes" section above.

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

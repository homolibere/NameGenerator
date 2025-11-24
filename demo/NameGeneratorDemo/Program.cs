using NameGeneratorEngine;
using NameGeneratorEngine.Enums;

Console.WriteLine("=== Name Generator Engine Demo ===\n");

// Demo 1: Basic name generation
Console.WriteLine("--- Demo 1: Basic Name Generation ---");
var generator = new NameGenerator(seed: 12345);
Console.WriteLine($"Seed: {generator.Seed}");
Console.WriteLine($"Cyberpunk NPC (Male): {generator.GenerateNpcName(Theme.Cyberpunk, Gender.Male)}");
Console.WriteLine($"Elf NPC (Female): {generator.GenerateNpcName(Theme.Elves, Gender.Female)}");
Console.WriteLine($"Orc NPC (Neutral): {generator.GenerateNpcName(Theme.Orcs, Gender.Neutral)}");
Console.WriteLine($"Cyberpunk City: {generator.GenerateCityName(Theme.Cyberpunk)}");
Console.WriteLine($"Elf District: {generator.GenerateDistrictName(Theme.Elves)}");
Console.WriteLine($"Orc Street: {generator.GenerateStreetName(Theme.Orcs)}\n");

// Demo 2: Building types
Console.WriteLine("--- Demo 2: Building Types ---");
foreach (BuildingType type in Enum.GetValues<BuildingType>())
{
    var name = generator.GenerateBuildingName(Theme.Cyberpunk, type);
    Console.WriteLine($"{type}: {name}");
}
Console.WriteLine();

// Demo 3: Deterministic generation
Console.WriteLine("--- Demo 3: Deterministic Generation ---");
var gen1 = new NameGenerator(seed: 999);
var gen2 = new NameGenerator(seed: 999);
Console.WriteLine("Generator 1 (seed 999):");
for (int i = 0; i < 3; i++)
{
    Console.WriteLine($"  {gen1.GenerateNpcName(Theme.Elves, Gender.Female)}");
}
Console.WriteLine("Generator 2 (seed 999) - Same sequence:");
for (int i = 0; i < 3; i++)
{
    Console.WriteLine($"  {gen2.GenerateNpcName(Theme.Elves, Gender.Female)}");
}
Console.WriteLine();

// Demo 4: Uniqueness within session
Console.WriteLine("--- Demo 4: Uniqueness Within Session ---");
var uniqueGen = new NameGenerator(seed: 42);
var names = new HashSet<string>();
for (int i = 0; i < 10; i++)
{
    var name = uniqueGen.GenerateCityName(Theme.Cyberpunk);
    names.Add(name);
    Console.WriteLine($"  {i + 1}. {name}");
}
Console.WriteLine($"Generated {names.Count} unique names out of 10 attempts\n");

// Demo 5: Session reset
Console.WriteLine("--- Demo 5: Session Reset ---");
var resetGen = new NameGenerator(seed: 777);
Console.WriteLine("First sequence:");
var firstNames = new List<string>();
for (int i = 0; i < 3; i++)
{
    var name = resetGen.GenerateNpcName(Theme.Orcs, Gender.Male);
    firstNames.Add(name);
    Console.WriteLine($"  {name}");
}
resetGen.ResetSession();
Console.WriteLine("After reset - Same sequence:");
for (int i = 0; i < 3; i++)
{
    var name = resetGen.GenerateNpcName(Theme.Orcs, Gender.Male);
    Console.WriteLine($"  {name} {(firstNames[i] == name ? "✓" : "✗")}");
}
Console.WriteLine();

// Demo 6: All themes showcase
Console.WriteLine("--- Demo 6: Theme Showcase ---");
foreach (Theme theme in Enum.GetValues<Theme>())
{
    var themeGen = new NameGenerator();
    Console.WriteLine($"\n{theme} Theme:");
    Console.WriteLine($"  NPC: {themeGen.GenerateNpcName(theme, Gender.Male)}");
    Console.WriteLine($"  Building: {themeGen.GenerateBuildingName(theme, BuildingType.Commercial)}");
    Console.WriteLine($"  City: {themeGen.GenerateCityName(theme)}");
    Console.WriteLine($"  District: {themeGen.GenerateDistrictName(theme)}");
    Console.WriteLine($"  Street: {themeGen.GenerateStreetName(theme)}");
}

Console.WriteLine("\n=== Demo Complete ===");

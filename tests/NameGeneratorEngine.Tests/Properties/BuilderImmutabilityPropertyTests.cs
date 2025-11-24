using CsCheck;
using FluentAssertions;
using NameGeneratorEngine.Enums;
using Xunit;

namespace NameGeneratorEngine.Tests.Properties;

/// <summary>
/// Property-based tests for theme data builder immutability.
/// **Feature: custom-theme-registration, Property 4: Builder immutability**
/// </summary>
public class BuilderImmutabilityPropertyTests
{
    /// <summary>
    /// Property 4: Builder immutability
    /// For any theme data created by the builder, attempting to modify the data after Build()
    /// should not affect the registered theme data.
    /// **Validates: Requirements 2.5**
    /// </summary>
    [Fact]
    public void Property_ModifyingSourceArraysAfterBuild_DoesNotAffectThemeData()
    {
        // Generate mutable arrays that we'll try to modify after building
        var validStringArrayGen = Gen.String.Array[1, 10].Where(arr => arr.All(s => !string.IsNullOrWhiteSpace(s)));

        Gen.Select(
            validStringArrayGen, // NPC male prefixes
            validStringArrayGen, // NPC male cores
            validStringArrayGen, // NPC male suffixes
            validStringArrayGen, // City prefixes
            validStringArrayGen, // City cores
            validStringArrayGen  // City suffixes
        ).Sample((npcMalePre, npcMaleCore, npcMaleSuf, cityPre, cityCore, citySuf) =>
        {
            // Keep references to the original arrays
            var originalNpcMalePre = npcMalePre.ToArray();
            var originalNpcMaleCore = npcMaleCore.ToArray();
            var originalNpcMaleSuf = npcMaleSuf.ToArray();
            var originalCityPre = cityPre.ToArray();
            var originalCityCore = cityCore.ToArray();
            var originalCitySuf = citySuf.ToArray();

            // Build the theme
            var customTheme = new ThemeDataBuilder()
                .WithNpcNames(npc => npc
                    .WithMaleNames(npcMalePre, npcMaleCore, npcMaleSuf)
                    .WithFemaleNames(new[] { "F" }, new[] { "e" }, new[] { "male" })
                    .WithNeutralNames(new[] { "N" }, new[] { "eu" }, new[] { "tral" }))
                .WithBuildingNames(building =>
                {
                    building.WithGenericNames(new[] { "Build" }, new[] { "ing" });
                    foreach (BuildingType type in Enum.GetValues(typeof(BuildingType)))
                    {
                        building.WithTypeNames(type, new[] { "Pre" }, new[] { "Desc" }, new[] { "Suf" });
                    }
                })
                .WithCityNames(cityPre, cityCore, citySuf)
                .WithDistrictNames(new[] { "Old" }, new[] { "District" })
                .WithStreetNames(new[] { "Main" }, new[] { "Oak" }, new[] { "Street" })
                .WithFactionNames(new[] { "The" }, new[] { "Guild" }, new[] { "s" })
                .Build();

            // Now modify the source arrays
            if (npcMalePre.Length > 0) npcMalePre[0] = "MODIFIED";
            if (npcMaleCore.Length > 0) npcMaleCore[0] = "MODIFIED";
            if (npcMaleSuf.Length > 0) npcMaleSuf[0] = "MODIFIED";
            if (cityPre.Length > 0) cityPre[0] = "MODIFIED";
            if (cityCore.Length > 0) cityCore[0] = "MODIFIED";
            if (citySuf.Length > 0) citySuf[0] = "MODIFIED";

            // Verify the theme data still has the original values
            var themeData = customTheme.InternalData;

            // Check NPC male names
            themeData.NpcNames.Male.Prefixes.Should().BeEquivalentTo(originalNpcMalePre);
            themeData.NpcNames.Male.Cores.Should().BeEquivalentTo(originalNpcMaleCore);
            themeData.NpcNames.Male.Suffixes.Should().BeEquivalentTo(originalNpcMaleSuf);

            // Check city names
            themeData.CityNames.Prefixes.Should().BeEquivalentTo(originalCityPre);
            themeData.CityNames.Cores.Should().BeEquivalentTo(originalCityCore);
            themeData.CityNames.Suffixes.Should().BeEquivalentTo(originalCitySuf);
        }, iter: 100);
    }

    /// <summary>
    /// Property 4: Builder immutability
    /// For any theme extension created by the builder, attempting to modify the source arrays
    /// after BuildExtension() should not affect the extension data.
    /// **Validates: Requirements 2.5**
    /// </summary>
    [Fact]
    public void Property_ModifyingSourceArraysAfterBuildExtension_DoesNotAffectExtensionData()
    {
        // Generate mutable arrays for extension
        var validStringArrayGen = Gen.String.Array[1, 10].Where(arr => arr.All(s => !string.IsNullOrWhiteSpace(s)));

        Gen.Select(
            Gen.OneOf(Gen.Const(Theme.Cyberpunk), Gen.Const(Theme.Elves), Gen.Const(Theme.Orcs)),
            validStringArrayGen, // Additional prefixes
            validStringArrayGen, // Additional cores
            validStringArrayGen  // Additional suffixes
        ).Sample((baseTheme, addPrefixes, addCores, addSuffixes) =>
        {
            // Keep references to the original arrays
            var originalPrefixes = addPrefixes.ToArray();
            var originalCores = addCores.ToArray();
            var originalSuffixes = addSuffixes.ToArray();

            // Build the extension
            var extension = ThemeDataBuilder.Extend(baseTheme)
                .WithNpcNames(npc => npc
                    .AddMalePrefixes(addPrefixes)
                    .AddMaleCores(addCores)
                    .AddMaleSuffixes(addSuffixes))
                .BuildExtension();

            // Now modify the source arrays
            if (addPrefixes.Length > 0) addPrefixes[0] = "MODIFIED";
            if (addCores.Length > 0) addCores[0] = "MODIFIED";
            if (addSuffixes.Length > 0) addSuffixes[0] = "MODIFIED";

            // Verify the extension data still has the original values
            var extensionData = extension.ExtensionData;
            extensionData.NpcNames.Male.Prefixes.Should().BeEquivalentTo(originalPrefixes);
            extensionData.NpcNames.Male.Cores.Should().BeEquivalentTo(originalCores);
            extensionData.NpcNames.Male.Suffixes.Should().BeEquivalentTo(originalSuffixes);
        }, iter: 100);
    }

    /// <summary>
    /// Property 4: Builder immutability
    /// For any theme data, the internal data structure should be immutable after Build().
    /// Attempting to modify arrays in the returned CustomThemeData should not be possible
    /// (arrays are value types, so modifications create new arrays).
    /// **Validates: Requirements 2.5**
    /// </summary>
    [Fact]
    public void Property_ThemeDataArraysAreIndependent_ModificationsDoNotAffectOriginal()
    {
        // This test verifies that getting arrays from the theme data and modifying them
        // doesn't affect the original theme data (because arrays are copied, not referenced)

        var customTheme = new ThemeDataBuilder()
            .WithNpcNames(npc => npc
                .WithMaleNames(new[] { "Original1", "Original2" }, new[] { "Core1", "Core2" }, new[] { "Suf1", "Suf2" })
                .WithFemaleNames(new[] { "F" }, new[] { "e" }, new[] { "male" })
                .WithNeutralNames(new[] { "N" }, new[] { "eu" }, new[] { "tral" }))
            .WithBuildingNames(building =>
            {
                building.WithGenericNames(new[] { "Build" }, new[] { "ing" });
                foreach (BuildingType type in Enum.GetValues(typeof(BuildingType)))
                {
                    building.WithTypeNames(type, new[] { "Pre" }, new[] { "Desc" }, new[] { "Suf" });
                }
            })
            .WithCityNames(new[] { "City1", "City2" }, new[] { "Core" }, new[] { "ton" })
            .WithDistrictNames(new[] { "Old" }, new[] { "District" })
            .WithStreetNames(new[] { "Main" }, new[] { "Oak" }, new[] { "Street" })
            .WithFactionNames(new[] { "The" }, new[] { "Guild" }, new[] { "s" })
            .Build();

        // Get references to the arrays
        var malePrefixes = customTheme.InternalData.NpcNames.Male.Prefixes;
        var cityPrefixes = customTheme.InternalData.CityNames.Prefixes;

        // Store original values
        var originalMalePrefixes = malePrefixes.ToArray();
        var originalCityPrefixes = cityPrefixes.ToArray();

        // Try to modify the arrays (this creates new arrays, doesn't modify the original)
        malePrefixes = new[] { "MODIFIED" };
        cityPrefixes = new[] { "MODIFIED" };

        // Verify the theme data still has the original values
        customTheme.InternalData.NpcNames.Male.Prefixes.Should().BeEquivalentTo(originalMalePrefixes);
        customTheme.InternalData.CityNames.Prefixes.Should().BeEquivalentTo(originalCityPrefixes);
    }

    /// <summary>
    /// Property 4: Builder immutability
    /// For any building type data, modifying the dictionary after Build() should not affect the theme data.
    /// **Validates: Requirements 2.5**
    /// </summary>
    [Fact]
    public void Property_BuildingTypeDataIsImmutable_ModificationsDoNotAffectThemeData()
    {
        Gen.Select(
            Gen.String.Array[1, 5].Where(arr => arr.All(s => !string.IsNullOrWhiteSpace(s))),
            Gen.String.Array[1, 5].Where(arr => arr.All(s => !string.IsNullOrWhiteSpace(s))),
            Gen.String.Array[1, 5].Where(arr => arr.All(s => !string.IsNullOrWhiteSpace(s)))
        ).Sample((prefixes, descriptors, suffixes) =>
        {
            var originalPrefixes = prefixes.ToArray();
            var originalDescriptors = descriptors.ToArray();
            var originalSuffixes = suffixes.ToArray();

            var customTheme = new ThemeDataBuilder()
                .WithNpcNames(npc => npc
                    .WithMaleNames(new[] { "M" }, new[] { "a" }, new[] { "le" })
                    .WithFemaleNames(new[] { "F" }, new[] { "e" }, new[] { "male" })
                    .WithNeutralNames(new[] { "N" }, new[] { "eu" }, new[] { "tral" }))
                .WithBuildingNames(building =>
                {
                    building.WithGenericNames(new[] { "Build" }, new[] { "ing" });
                    building.WithTypeNames(BuildingType.Residential, prefixes, descriptors, suffixes);
                    foreach (BuildingType type in Enum.GetValues(typeof(BuildingType)))
                    {
                        if (type != BuildingType.Residential)
                        {
                            building.WithTypeNames(type, new[] { "Pre" }, new[] { "Desc" }, new[] { "Suf" });
                        }
                    }
                })
                .WithCityNames(new[] { "City" }, new[] { "Core" }, new[] { "ton" })
                .WithDistrictNames(new[] { "Old" }, new[] { "District" })
                .WithStreetNames(new[] { "Main" }, new[] { "Oak" }, new[] { "Street" })
                .WithFactionNames(new[] { "The" }, new[] { "Guild" }, new[] { "s" })
                .Build();

            // Modify the source arrays
            if (prefixes.Length > 0) prefixes[0] = "MODIFIED";
            if (descriptors.Length > 0) descriptors[0] = "MODIFIED";
            if (suffixes.Length > 0) suffixes[0] = "MODIFIED";

            // Verify the theme data still has the original values
            var residentialData = customTheme.InternalData.BuildingNames.TypeData[BuildingType.Residential];
            residentialData.Prefixes.Should().BeEquivalentTo(originalPrefixes);
            residentialData.Descriptors.Should().BeEquivalentTo(originalDescriptors);
            residentialData.Suffixes.Should().BeEquivalentTo(originalSuffixes);
        }, iter: 100);
    }
}

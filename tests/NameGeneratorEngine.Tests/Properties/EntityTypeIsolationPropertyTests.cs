using CsCheck;
using FluentAssertions;
using NameGeneratorEngine.Enums;
using NameGeneratorEngine.Foundation;
using Xunit;

namespace NameGeneratorEngine.Tests.Properties;

/// <summary>
/// Property-based tests for entity type isolation in duplicate tracking.
/// </summary>
public class EntityTypeIsolationPropertyTests
{
    /// <summary>
    /// Feature: name-generator-engine, Property 3: Separate tracking per entity type
    /// For any name string, it should be possible to generate that same name for different 
    /// entity types within a single session (e.g., "Chrome District" can exist as both a 
    /// district name and a city name).
    /// Validates: Requirements 2.3
    /// </summary>
    [Fact]
    public void Property_SeparateTrackingPerEntityType()
    {
        // Test the DuplicateTracker directly to verify entity type isolation
        Gen.String.Sample(name =>
        {
            var tracker = new DuplicateTracker();

            // The same name should be unique for each entity type
            foreach (var entityType in Enum.GetValues<EntityType>())
            {
                tracker.IsUnique(entityType, name).Should().BeTrue(
                    $"the name '{name}' should be unique for {entityType} even if used by other entity types");
            }

            // Track the name for all entity types
            foreach (var entityType in Enum.GetValues<EntityType>())
            {
                tracker.Track(entityType, name);
            }

            // After tracking, the name should no longer be unique for each entity type
            foreach (var entityType in Enum.GetValues<EntityType>())
            {
                tracker.IsUnique(entityType, name).Should().BeFalse(
                    $"the name '{name}' should not be unique for {entityType} after being tracked");
            }

            // But a different name should still be unique for all entity types
            var differentName = name + "_different";
            foreach (var entityType in Enum.GetValues<EntityType>())
            {
                tracker.IsUnique(entityType, differentName).Should().BeTrue(
                    $"a different name should be unique for {entityType}");
            }
        }, iter: 100);
    }

    /// <summary>
    /// Property test that verifies entity type isolation with multiple names.
    /// </summary>
    [Fact]
    public void Property_EntityTypeIsolationWithMultipleNames()
    {
        var genNames = Gen.String.Array[5, 20];

        genNames.Sample(names =>
        {
            var tracker = new DuplicateTracker();

            // Track all names for EntityType.Npc
            foreach (var name in names)
            {
                tracker.Track(EntityType.Npc, name);
            }

            // All names should still be unique for other entity types
            foreach (var name in names)
            {
                tracker.IsUnique(EntityType.Building, name).Should().BeTrue(
                    $"name '{name}' tracked for NPC should still be unique for Building");
                tracker.IsUnique(EntityType.City, name).Should().BeTrue(
                    $"name '{name}' tracked for NPC should still be unique for City");
                tracker.IsUnique(EntityType.District, name).Should().BeTrue(
                    $"name '{name}' tracked for NPC should still be unique for District");
                tracker.IsUnique(EntityType.Street, name).Should().BeTrue(
                    $"name '{name}' tracked for NPC should still be unique for Street");
            }

            // But should not be unique for EntityType.Npc
            foreach (var name in names)
            {
                tracker.IsUnique(EntityType.Npc, name).Should().BeFalse(
                    $"name '{name}' should not be unique for NPC after being tracked");
            }
        }, iter: 100);
    }

    /// <summary>
    /// Property test that verifies cross-entity type name reuse.
    /// </summary>
    [Fact]
    public void Property_CrossEntityTypeNameReuse()
    {
        var genName = Gen.String;
        var genEntityType1 = Gen.Int[0, 4].Select(i => (EntityType)i); // EntityType has 5 values: 0-4
        var genEntityType2 = Gen.Int[0, 4].Select(i => (EntityType)i); // EntityType has 5 values: 0-4

        Gen.Select(genName, genEntityType1, genEntityType2)
            .Sample(tuple =>
            {
                var (name, entityType1, entityType2) = tuple;
                var tracker = new DuplicateTracker();

                // Track the name for entityType1
                tracker.Track(entityType1, name);

                if (entityType1 == entityType2)
                {
                    // Same entity type: name should not be unique
                    tracker.IsUnique(entityType2, name).Should().BeFalse(
                        $"name '{name}' should not be unique for {entityType2} after being tracked for the same type");
                }
                else
                {
                    // Different entity type: name should still be unique
                    tracker.IsUnique(entityType2, name).Should().BeTrue(
                        $"name '{name}' should be unique for {entityType2} even after being tracked for {entityType1}");
                }
            }, iter: 100);
    }

    /// <summary>
    /// Property test that verifies GetAttemptCount is isolated per entity type.
    /// </summary>
    [Fact]
    public void Property_AttemptCountIsolationPerEntityType()
    {
        var genNames = Gen.String.Array[1, 50];

        genNames.Sample(names =>
        {
            var tracker = new DuplicateTracker();

            // Track different numbers of names for different entity types
            var npcCount = Math.Min(names.Length, 10);
            var buildingCount = Math.Min(names.Length, 20);
            var cityCount = Math.Min(names.Length, 15);

            for (var i = 0; i < npcCount; i++)
            {
                tracker.Track(EntityType.Npc, names[i]);
            }

            for (var i = 0; i < buildingCount; i++)
            {
                tracker.Track(EntityType.Building, names[i]);
            }

            for (var i = 0; i < cityCount; i++)
            {
                tracker.Track(EntityType.City, names[i]);
            }

            // GetAttemptCount returns the number of UNIQUE names tracked (HashSet count)
            // So we need to count distinct names in each slice
            var expectedNpcCount = names.Take(npcCount).Distinct().Count();
            var expectedBuildingCount = names.Take(buildingCount).Distinct().Count();
            var expectedCityCount = names.Take(cityCount).Distinct().Count();

            // Verify counts are isolated
            tracker.GetAttemptCount(EntityType.Npc).Should().Be(expectedNpcCount,
                "NPC count should only reflect unique NPC names");
            tracker.GetAttemptCount(EntityType.Building).Should().Be(expectedBuildingCount,
                "Building count should only reflect unique Building names");
            tracker.GetAttemptCount(EntityType.City).Should().Be(expectedCityCount,
                "City count should only reflect unique City names");
            tracker.GetAttemptCount(EntityType.District).Should().Be(0,
                "District count should be 0 when no districts are tracked");
            tracker.GetAttemptCount(EntityType.Street).Should().Be(0,
                "Street count should be 0 when no streets are tracked");
        }, iter: 100);
    }
}

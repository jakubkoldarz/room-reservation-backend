using FluentAssertions;
using RoomReservation.Core.Filters;

namespace RoomReservation.Core.Tests.Filters
{
    public class SortMapTests
    {
        private sealed record Item(string Name, int Size, int Order);

        private sealed class TestFilter : PagedFilter;

        private static readonly Item[] Items =
        [
            new("b", 2, 1),
            new("a", 3, 2),
            new("c", 2, 0),
        ];

        private static readonly SortMap<Item> Sorts = new SortMap<Item>("name", defaultDescending: true)
            .Add("name", i => i.Name)
            .Add("size", i => i.Size, i => i.Order);

        private static string[] Sorted(string? sortBy, string? direction)
            => [.. Sorts.Apply(Items.AsQueryable(), new TestFilter { SortBy = sortBy, SortDirection = direction }).Select(i => i.Name)];

        [Fact]
        public void Apply_WithoutSort_UsesDefaultFieldAndDirection()
        {
            Sorted(null, null).Should().Equal("c", "b", "a");
        }

        [Fact]
        public void Apply_WithDefaultFieldOnly_KeepsDefaultDirection()
        {
            Sorted("name", null).Should().Equal("c", "b", "a");
        }

        [Fact]
        public void Apply_WithOtherFieldOnly_SortsAscending()
        {
            Sorted("size", null).Should().Equal("c", "b", "a");
        }

        [Fact]
        public void Apply_WithDirection_UsesSecondaryKeyInSameDirection()
        {
            Sorted("size", "desc").Should().Equal("a", "b", "c");
        }

        [Fact]
        public void Apply_IgnoresCaseOfFieldAndDirection()
        {
            Sorted("NAME", "ASC").Should().Equal("a", "b", "c");
        }

        [Fact]
        public void Apply_WithUnknownField_FallsBackToDefaultField()
        {
            Sorted("missing", "asc").Should().Equal("a", "b", "c");
        }
    }
}

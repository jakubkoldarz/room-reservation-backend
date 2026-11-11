using System.Linq.Expressions;

namespace RoomReservation.Core.Filters
{
    public sealed class SortMap<T>(string defaultField, bool defaultDescending = false)
    {
        public const string Descending = "desc";

        private readonly Dictionary<string, Func<IQueryable<T>, bool, IOrderedQueryable<T>>> fields =
            new(StringComparer.OrdinalIgnoreCase);

        public SortMap<T> Add<TKey>(string name, Expression<Func<T, TKey>> key)
        {
            fields[name] = (query, descending) => descending
                ? query.OrderByDescending(key)
                : query.OrderBy(key);
            return this;
        }

        public SortMap<T> Add<TKey, TThenKey>(string name, Expression<Func<T, TKey>> key, Expression<Func<T, TThenKey>> thenKey)
        {
            fields[name] = (query, descending) => descending
                ? query.OrderByDescending(key).ThenByDescending(thenKey)
                : query.OrderBy(key).ThenBy(thenKey);
            return this;
        }

        public IOrderedQueryable<T> Apply(IQueryable<T> query, PagedFilter filters)
        {
            var field = filters.SortBy is not null && fields.ContainsKey(filters.SortBy)
                ? filters.SortBy
                : defaultField;

            var descending = filters.SortDirection is null
                ? string.Equals(field, defaultField, StringComparison.OrdinalIgnoreCase) && defaultDescending
                : string.Equals(filters.SortDirection, Descending, StringComparison.OrdinalIgnoreCase);

            return fields[field](query, descending);
        }
    }
}

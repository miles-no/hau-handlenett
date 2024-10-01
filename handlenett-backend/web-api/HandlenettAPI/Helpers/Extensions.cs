using System.Reflection;

namespace HandlenettAPI.Helpers
{
    public static class Extensions
    {
        public static List<TDestination> ConvertToList<TDestination>(this IEnumerable<object> sourceList)
        where TDestination : new()
        {
            if (sourceList == null) throw new ArgumentNullException(nameof(sourceList));

            var destinationList = new List<TDestination>();

            foreach (var source in sourceList)
            {
                var destination = new TDestination();
                MapProperties(source, destination);
                destinationList.Add(destination);
            }

            return destinationList;
        }

        private static void MapProperties(object source, object destination)
        {
            var sourceProperties = source.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var destinationProperties = destination.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var sourceProperty in sourceProperties)
            {
                var destinationProperty = destinationProperties.FirstOrDefault(dp => dp.Name == sourceProperty.Name && dp.CanWrite);
                if (destinationProperty != null && sourceProperty.CanRead)
                {
                    var value = sourceProperty.GetValue(source);
                    destinationProperty.SetValue(destination, value);
                }
            }
        }

        public static TDestination ConvertTo<TDestination>(this object source)
            where TDestination : new()
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            var destination = new TDestination();
            MapProperties(source, destination);
            return destination;
        }
    }
}

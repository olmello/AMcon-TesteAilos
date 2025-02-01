namespace Questao5.Core.Extensions
{
    public static class EnumerableExtensions
    {
        public static async Task ForEachAsync<T>(this IEnumerable<T> lista, Func<T, Task> function)
        {
            foreach (var value in lista)
            {
                await function(value);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> items, Action<T> itemAction)
        {
            foreach (var item in items)
            {
                itemAction(item);
            }
        }
    }
}

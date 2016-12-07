namespace QuintensUITools
{
    /// <summary>
    /// Simple helperclass that exist also in .NET 4. It is a collection of exactly two arbitrary objects.
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public class Tuple<T1, T2>
    {
        public T1 Item1;
        public T2 Item2;
        public Tuple(T1 item1, T2 item2)
        {
            Item1 = item1; Item2 = item2;
        }
    }
}

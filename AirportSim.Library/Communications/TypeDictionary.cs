namespace AirportSim.Library.Communications;

public class TypeDictionary 
{
    private static class PerType<T> where T : class 
    {
        public static IEnumerable<T> List = Enumerable.Empty<T>();
    }
    
    public IEnumerable<T> Get<T>() where T : class 
        => PerType<T>.List; 

    public void Set<T>(IEnumerable<T> source) where T : class 
        => PerType<T>.List = source;
    
    public void Set<T>(T newItem) where T : class 
        => PerType<T>.List = new List<T> { newItem };

    public IEnumerable<T> GetLike<T>(T ignoredExample) where T : class 
        => Get<T>(); 
}
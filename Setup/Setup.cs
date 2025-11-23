using System.Runtime.Loader;

namespace AsyncEduMockUp.Setup;

internal static class Setup
{
    private static readonly Dictionary<char, string[]> _memberItems = AssemblyLoadContext.All
            .SelectMany(x => x.Assemblies)
            .SelectMany(x => x.DefinedTypes)
            .SelectMany(x => x.DeclaredMembers)
            .Select(x => x.Name!)
            .Where(x => !string.IsNullOrEmpty(x))
            .OrderBy(x => x)
            .Distinct()
            .GroupBy(x => x[0])
            .ToDictionary(x => x.Key, x => x.ToArray());

    public static int GetHashCodeFromMemberCXor(char c)
    {
        var randSleep = Random.Shared.Next(1000, 6000);

        if(!_memberItems.TryGetValue(c, out var items))
        {
            items = [];
        }

        var hashCodeSum = items.Aggregate(0, HashCode.Combine);

        Thread.Sleep(randSleep);

        return hashCodeSum;
    }
}

namespace CafeKitchen.Domain.Recipes;

/// <summary>Depth-first search that rejects a recipe containing itself, directly or indirectly (F3.3).</summary>
public static class RecipeCycleDetector
{
    /// <param name="childrenOf">Returns the child recipe ids used by a recipe.</param>
    public static bool WouldCreateCycle(int recipeId, int proposedChildId, Func<int, IEnumerable<int>> childrenOf)
    {
        if (recipeId == proposedChildId) return true;

        var visited = new HashSet<int>();
        var stack = new Stack<int>();
        stack.Push(proposedChildId);
        while (stack.Count > 0)
        {
            var current = stack.Pop();
            if (!visited.Add(current)) continue;
            foreach (var child in childrenOf(current))
            {
                if (child == recipeId) return true;
                stack.Push(child);
            }
        }
        return false;
    }
}

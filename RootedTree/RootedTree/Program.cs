using System;
using System.Collections.Generic;

class TreeNode<T>
{
    public T NodeNumber { get; }
    public int Value { get; set; }
    public TreeNode<T> Parent { get; set; }
    public List<TreeNode<T>> Children { get; } = new List<TreeNode<T>>();

    public List<TreeNode<T>> Descendants { get; private set; }

    public TreeNode(T nodeNumber, int value = 0)
    {
        NodeNumber = nodeNumber;
        Value = value;
        Descendants = new List<TreeNode<T>>();
    }

    public void AddChild(TreeNode<T> child)
    {
        child.Parent = this;
        Children.Add(child);

        foreach (var descendant in child.GetDescendants())
        {
            descendant.Descendants.Add(descendant);
        }
    }

    public int DistanceTo(TreeNode<T> target)
    {
        var path = PathTo(target);
        if (path == null)
            return -1;
        return path.Count - 1; // Subtract 1 because the path includes the start node
    }

    public List<TreeNode<T>> PathTo(TreeNode<T> target)
    {
        return PathTo(target, new HashSet<TreeNode<T>>());
    }

    private List<TreeNode<T>> PathTo(TreeNode<T> target, HashSet<TreeNode<T>> visitedNodes)
    {
        if (this == target) return new List<TreeNode<T>>() { this };

        visitedNodes.Add(this);

        // Search descendants.
        foreach (var child in Children)
        {
            if (!visitedNodes.Contains(child))
            {
                var path = child.PathTo(target, visitedNodes);
                if (path != null)
                {
                    path.Insert(0, this);
                    return path;
                }
            }
        }

        // Search ancestors.
        if (this.Parent != null && !visitedNodes.Contains(this.Parent))
        {
            var pathToParent = this.Parent.PathTo(target, visitedNodes);
            if (pathToParent != null)
            {
                pathToParent.Add(this);
                return pathToParent;
            }
        }

        return null;
    }


    public IEnumerable<TreeNode<T>> GetDescendants()
    {
        yield return this;
        foreach (var child in Children)
        {
            foreach (var descendant in child.GetDescendants())
            {
                yield return descendant;
            }
        }
    }
}

class RootedTree<T>
{
    public TreeNode<T> Root { get; private set; }

    public RootedTree(T rootNumber, int rootValue = 0)
    {
        Root = new TreeNode<T>(rootNumber, rootValue);
    }

    public IEnumerable<TreeNode<T>> GetAllDescendants()
    {
        return Root.GetDescendants();
    }
}

class Program
{
    static void Main()
    {
        RootedTree<int> tree = new RootedTree<int>(1);

        TreeNode<int> node2 = new TreeNode<int>(2);
        TreeNode<int> node3 = new TreeNode<int>(3);
        TreeNode<int> node4 = new TreeNode<int>(4);
        TreeNode<int> node5 = new TreeNode<int>(5);
        TreeNode<int> node6 = new TreeNode<int>(6);
        tree.Root.AddChild(node2);
        node2.AddChild(node3);
        node2.AddChild(node5);
        node3.AddChild(node4);
        node3.AddChild(node6);
        List<TreeNode<int>> path = node2.PathTo(node6);
        List<TreeNode<int>> path2 = tree.Root.PathTo(node4);
        if (path != null)
        {
            Console.Write("Path from Root to Node 4: ");
            foreach (var node in path)
            {
                Console.Write(node.NodeNumber + " ");
            }
            Console.WriteLine();
        }
        else
        {
            Console.WriteLine("No path found.");
        }
        var descendies = node2.GetDescendants().ToList();
        var allDescendants = tree.GetAllDescendants();
        
        Console.WriteLine("All Descendants of the node2:");
        foreach (var descendant in descendies)
        {
            Console.Write(descendant.NodeNumber + " ");
        }
        int dist24 = node2.DistanceTo(node4);
        Console.WriteLine("\n"+dist24);
        Console.ReadLine();
    }
}



using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

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
        //Descendants = new List<TreeNode<T>>();
        Descendants.Add(this);
    }

    public void AddChild(TreeNode<T> child)
    {
        child.Parent = this;
        Children.Add(child);
        Descendants.Add(child);
        TreeNode<T> dummyParent = Parent; 
        while (dummyParent != null)
        {
            dummyParent.Descendants.Add(child);
            dummyParent = dummyParent.Parent;
        }
        /*foreach (var descendant in child.GetDescendants())
        {
            descendant.Descendants.Add(descendant);
        }*/
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

class Solution
{
    static Dictionary<int, TreeNode<int>> GetNodeList(int rootNumber, int numNodes)
    {
        Dictionary<int, TreeNode<int>> nodes = new Dictionary<int, TreeNode<int>>();
        for (int i = 1; i <= numNodes; i++)
        {
            if (i != rootNumber)
            {
                nodes.Add(i, new TreeNode<int>(i));
            }
        }
        return nodes;
    }
    static RootedTree<int> CreateTree(Dictionary<int, TreeNode<int>> nodes,int[][] edges, int rootNumber)
    {
        
        RootedTree<int> tree = new RootedTree<int>(rootNumber);

        
        bool beforeRoot = true;
        for (int i =0; i <edges.Length; i++)
        {
           
            int firstNum = edges[i][0];
            int secondNum = edges[i][1];
            // -1 and -2 are for indexing of List<TreeNode<int>> nodes relative to TreeNode<int>(i)
            if (firstNum == rootNumber)
            {
                tree.Root.AddChild(nodes[secondNum]);
                beforeRoot = false;
            }
            else if ( secondNum == rootNumber )
            {
                tree.Root.AddChild(nodes[firstNum]);
                beforeRoot = false;
            }
            
            else if (!beforeRoot)
            {
                TreeNode<int> currNode = nodes[firstNum];
                currNode.AddChild(nodes[secondNum]);
            }
            else if (beforeRoot)
            {
                nodes[secondNum].AddChild(nodes[firstNum]);
            }

        }
        return tree;
    }
    static void Main(String[] args)
    {
        string[] firstMultipleInput = Console.ReadLine().TrimEnd().Split(' ');
        int numNodes = Convert.ToInt32(firstMultipleInput[0]);
        int numQueries = Convert.ToInt32(firstMultipleInput[1]);
        int rootNumber = Convert.ToInt32(firstMultipleInput[2]);

        //Create Tree
        int[][] edges = new int[numNodes-1][];
        for (int i = 0; i < numNodes-1; i++)
        {
            string[] edgeInput = Console.ReadLine().TrimEnd().Split(' ');
            int node1 = Convert.ToInt32(edgeInput[0]);
            int node2 = Convert.ToInt32(edgeInput[1]);
            int[] edge = new int[2] {node1, node2};
            edges[i]=edge;
        }

        Dictionary<int, TreeNode<int>> nodes = GetNodeList(rootNumber, numNodes);
        RootedTree<int> tree = CreateTree(nodes,edges, rootNumber);
        
        int[][] operations = new int[numQueries][];
        for(int i = 0; i < numQueries; i++)
        {
            string[] query = Console.ReadLine().TrimEnd().Split(' ');
            if (query[0]=="U")
            {
                int T = Convert.ToInt32(query[1]);
                int V = Convert.ToInt32(query[2]);
                int K = Convert.ToInt32(query[3]);
                operations[i] = new int[3] { T, V, K };
            }
            if (query[0]=="Q") 
            { 
                int A = Convert.ToInt32(query[1]);
                int B = Convert.ToInt32(query[2]);
                operations[i] = new int[2] { A, B};
            }
        }
        /*List<TreeNode<int>> nodes = new List<TreeNode<int>>();

        for (int i = 1; i <=numNodes; i++)
        {
            if (i != rootNumber)
            {
                nodes.Add(new TreeNode<int>(i));
            }
        }*/
        /*foreach (int[] edge in edges)
        {
            int parentNum = edge[0];
            int childNum = edge[1];
            if (parentNum == rootNumber)
            {
                tree.Root.AddChild(nodes[childNum - 2]);
            }
            else
            {
                nodes[parentNum - 2].AddChild(nodes[childNum - 2]);
            }
        }*/
        
        
        for (int i = 0; i< operations.Length; i++) 
        {
            if (operations[i].Length==3) 
            {
                int T = operations[i][0];
                int V = operations[i][1];
                int K = operations[i][2];

                

                List<TreeNode<int>> desc = nodes[T].Descendants;
                for (int j = 0; j < desc.Count; j++)
                {
                    int d = desc[0].DistanceTo(desc[j]);
                    desc[j].Value += (V + d * K);
                }
            }
            if (operations[i].Length==2) 
            { 
                int A = operations[i][0];
                int B = operations[i][1];
                
                List<TreeNode<int>> path = new List<TreeNode<int>>();
                if (A == rootNumber)
                {
                    path = tree.Root.PathTo(nodes[B]);
                }
                else
                {
                    path = nodes[A].PathTo(nodes[B]);
                }
                int value = 0;
                int mod = Convert.ToInt32(Math.Pow(10,9) + 7);
                foreach(TreeNode<int> node in path)
                {
                    value += node.Value % mod;
                }
                Console.WriteLine(value);

            }
        }
        Console.ReadLine();
    }
}
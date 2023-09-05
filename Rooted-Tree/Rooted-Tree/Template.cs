using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace Temp
{

class TreeNode
{
    public int NodeNumber { get; }
    public int Value { get; set; }
    public bool Passed {get; set;}
    public TreeNode Parent { get; set; }
    public List<TreeNode> Children { get; } = new List<TreeNode>();

    

    public TreeNode(int nodeNumber, int value = 0, bool passed = false)
    {
        NodeNumber = nodeNumber;
        Value = value;
        Passed = passed;
    }

    public void AddChild(TreeNode child)
    {
        child.Parent = this;
        Children.Add(child);
    }

}

class FenwickTree
{
    private int[] tree;

    public FenwickTree(int size)
    {
        tree = new int[size + 1];
    }

    public void Update(int i, int val)
    {
        i++;  // Convert 0-based index to 1-based index
        while (i < tree.Length)
        {
            tree[i] += val;
            i += i & -i;
        }
    }

    public int Query(int i)
    {
        i++;  // Convert 0-based index to 1-based index
        int sum = 0;
        while (i > 0)
        {
            sum += tree[i];
            i -= i & -i;
        }
        return sum;
    }
}
class RootedTree
{
    public TreeNode Root { get; private set; }
    private int[,] up;  // For binary lifting
    private int[] depth;  // Store the depth of each node
    private int maxLog;  // Maximum value of log2(N)
    private FenwickTree fenwickTree;
    private  List<int>[] adjMatrix;
    Dictionary<int, TreeNode> nodes;
    public int[] dfnl;
    public int[] dfnr;

    private int tick = 0;
    public RootedTree(int rootNumber, int nodeCount, int[][] edges, int rootValue = 0)
    {
        Root = new TreeNode(rootNumber, rootValue);
        nodes.Add(rootNumber, Root);
        maxLog = (int)Math.Ceiling(Math.Log(nodeCount, 2));
        up = new int[nodeCount + 1, maxLog + 1];  // Assuming 1-based node numbering
        depth = new int[nodeCount + 1];
        fenwickTree = new FenwickTree(nodeCount * 2);
        adjMatrix = new List<int>[edges.Length + 1];
        InitializeTree(edges);
        Precompute(rootNumber, Root, null);
    }
    
    private void InitializeTree(int[][] edges)
    {
        for (int i = 0; i < edges.Length + 2; i++)
        {
            adjMatrix[i] = new List<int>();
        }
        foreach (var edge in edges)
        {
            adjMatrix[edge[0]].Add(edge[1]);
            adjMatrix[edge[1]].Add(edge[0]);
        }
        
    }
    public void Precompute(int nodeNumber, TreeNode node, TreeNode parent)
    {
        dfnl[nodeNumber] = tick++;
        up[nodeNumber, 0] = parent?.NodeNumber ?? -1;
        depth[nodeNumber] = parent == null ? 0 : depth[parent.NodeNumber] + 1;
        for (int i = 1; i <= maxLog; i++)
        {
            int ancestor = up[nodeNumber, i - 1];
            up[nodeNumber, i] = ancestor == -1 ? -1 : up[ancestor, i - 1];
        }

        foreach (var adjNode in adjMatrix[nodeNumber])
        {
            if(adjNode != parent?.NodeNumber)
            {
                TreeNode child = new TreeNode(adjNode);
                node.AddChild(child);
                nodes.Add(adjNode, child);
                Precompute(child.NodeNumber, child, node);
            }
        }
        dfnr[nodeNumber] = tick;
    }
}
class Solution
{
    
    static void Update(RootedTree tree, int U, int V, int K)
    {
        
    }
    static void Query(RootedTree tree, int A, int B)
    {

    }
    /*static void Main(String[] args)
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

        
        RootedTree tree = new RootedTree(rootNumber,numNodes,edges);
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


        for (int i = 0; i < operations.Length; i++)
        {
            if (operations[i].Length == 3)
            {
                int T = operations[i][0];
                int V = operations[i][1];
                int K = operations[i][2];
                Update(tree, T, V, K);



            }
            if (operations[i].Length == 2)
            {
                int A = operations[i][0];
                int B = operations[i][1];
                Query(tree, A, B);
            }
        }
        Console.ReadLine();
    }*/
}
}


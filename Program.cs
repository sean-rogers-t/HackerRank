using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

class TreeNode
{
    public int NodeNumber { get; }
    public int Value { get; set; }
    public TreeNode Parent { get; set; }
    public List<TreeNode> Children { get; } = new List<TreeNode>();

    public List<TreeNode> Descendants { get; } = new List<TreeNode>();

    public TreeNode(int nodeNumber, int value = 0)
    {
        NodeNumber = nodeNumber;
        Value = value;
        
    }

    public void AddChild(TreeNode child)
    {
        child.Parent = this;
        Children.Add(child);
        
        
    }
       
}

class RootedTree
{
    public TreeNode Root { get; private set; }
    private int[,] up;  // For binary lifting
    private int[] depth;  // Store the depth of each node
    private int maxLog;  // Maximum value of log2(N)
    public RootedTree(int rootNumber,int nodeCount, int rootValue = 0)
    {
        Root = new TreeNode(rootNumber, rootValue);
        maxLog = (int)Math.Ceiling(Math.Log(nodeCount, 2));
        up = new int[nodeCount + 1, maxLog + 1];  // Assuming 1-based node numbering
        depth = new int[nodeCount + 1];
        Precompute(rootNumber, Root, null);
    }
    public void Precompute(int nodeNumber, TreeNode node, TreeNode parent)
    {
        up[nodeNumber, 0] = parent?.NodeNumber ?? -1;  // Initialize the parent
        depth[nodeNumber] = parent == null ? 0 : depth[parent.NodeNumber] + 1;

        // Precompute up[][] for this node
        for (int i = 1; i <= maxLog; i++)
        {
            int ancestor = up[nodeNumber, i - 1];
            up[nodeNumber, i] = ancestor == -1 ? -1 : up[ancestor, i - 1];
        }

        // Precompute for children
        foreach (var child in node.Children)
        {
            Precompute(child.NodeNumber, child, node);
        }
    }
    public int Depth(TreeNode node)
    {
        int depth = 0;
        TreeNode dummyParent = node;
        while (dummyParent != Root)
        {
            depth++;
            dummyParent = dummyParent.Parent;
        }
        return depth;
    }
    public int FindLCA(int u, int v)
    {
        if (depth[u] < depth[v]) (u, v) = (v, u);  // Ensure u is deeper

        int diff = depth[u] - depth[v];
        // Lift u to the same depth as v
        for (int i = 0; i <= maxLog; i++)
        {
            if ((diff & (1 << i)) != 0)
                //decomposies diff into powers of 2 and checks which ones aren't 0.
            {
                u = up[u, i];
            }
        }

        if (u == v) return u;  // v was an ancestor of u

        // Lift both u and v towards the root
        for (int i = maxLog; i >= 0; i--)
        {
            if (up[u, i] != up[v, i])
            {
                u = up[u, i];
                v = up[v, i];
            }
        }

        return up[u, 0];  // or up[v, 0]
    }


}

class Solution
{
    static Dictionary<int, TreeNode> GetNodeList(int rootNumber, int numNodes)
    {
        Dictionary<int, TreeNode> nodes = new Dictionary<int, TreeNode>();
        for (int i = 1; i <= numNodes; i++)
        {
            if (i != rootNumber)
            {
                nodes.Add(i, new TreeNode(i));
            }
        }
        return nodes;
    }
    static RootedTree CreateTree(Dictionary<int, TreeNode> nodes,int[][] edges, int rootNumber,int nodeCount)
    {
        
        RootedTree tree = new RootedTree(rootNumber, nodeCount);

        
        bool beforeRoot = true;
        for (int i =0; i <edges.Length; i++)
        {
           
            int firstNum = edges[i][0];
            int secondNum = edges[i][1];
            // -1 and -2 are for indexing of List<TreeNode> nodes relative to TreeNode(i)
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
                TreeNode currNode = nodes[firstNum];
                currNode.AddChild(nodes[secondNum]);
            }
            else if (beforeRoot)
            {
                nodes[secondNum].AddChild(nodes[firstNum]);
            }

        }
        return tree;
    }
    static void Update(RootedTree tree, int U, int V, int K)
    {
        
    }
    static void Query(RootedTree tree, int A, int B)
    {

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

        Dictionary<int, TreeNode> nodes = GetNodeList(rootNumber, numNodes);
        RootedTree tree = CreateTree(nodes,edges, rootNumber, numNodes);
        tree.Precompute(rootNumber, tree.Root, null);
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
    }
}
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

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
        //i++;  // Convert 0-based index to 1-based index
        while (i < tree.Length)
        {
            tree[i] += val;
            i += i & -i;
        }
    }

    public int Query(int i)
    {
        //i++;  // Convert 0-based index to 1-based index
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
    Dictionary<int, TreeNode> nodes= new Dictionary<int, TreeNode>();
    public int[] dfnl;
    public int[] dfnr;
    private Dictionary<int, int> dfsToNodeMapping = new Dictionary<int, int>();
    private int tick = 1;
    public RootedTree(int rootNumber, int nodeCount, int[][] edges, int rootValue = 0)
    {
        dfnl = new int[nodeCount+ 1];
        dfnr = new int[nodeCount+1];
        Root = new TreeNode(rootNumber, rootValue);
        nodes.Add(rootNumber, Root);
        maxLog = (int)Math.Ceiling(Math.Log(nodeCount, 2));
        up = new int[nodeCount + 1, maxLog + 1];  // Assuming 1-based node numbering
        depth = new int[nodeCount + 1];
        fenwickTree = new FenwickTree(nodeCount);
        adjMatrix = new List<int>[edges.Length + 2];
        InitializeTree(edges);
        Precompute(rootNumber, Root, null);
        int k = 1;
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
        dfsToNodeMapping[dfnl[nodeNumber]] = nodeNumber;
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
        //dfsToNodeMapping[dfnr[nodeNumber]] = nodeNumber;
    }

   
    public int FindLCA(int u, int v)
    {
        if (depth[u] < depth[v]) (u, v) = (v, u);

        int diff = depth[u] - depth[v];
        for (int i = 0; i <= maxLog; i++)
        {
            if ((diff & (1 << i)) != 0)
            {
                u = up[u, i];
            }
        }

        if (u == v) return u;

        for (int i = maxLog; i >= 0; i--)
        {
            if (up[u, i] != up[v, i])
            {
                u = up[u, i];
                v = up[v, i];
            }
        }

        return up[u, 0];
    }
    public void Update(int T, int V, int K)
    {
        // Step 1: Identify the range of descendants using dfnl and dfnr
        int left = dfnl[T];
        int right = dfnr[T];

        // Step 2: Update the range using Fenwick Tree
        for (int i = left; i < right; i++)
        {
            // Here, you might want to map 'i' back to the actual node number
            // to find its depth. Let's assume you have a method GetNodeNumber(i)
            // that does this.
            int nodeNumber = GetNodeNumber(i);

            // Calculate d, the depth difference
            int d = depth[nodeNumber] - depth[T];

            // Calculate the amount to update
            int amount = V + d * K;

            // Update Fenwick tree
            fenwickTree.Update(i, amount);
        }
    }
    public int Query(int A, int B)
    {
        // Step 1: Find the Lowest Common Ancestor (LCA) of A and B
        int lca = FindLCA(A, B);

        // Step 2: Query the path from A to LCA
        int dfsNumberOfA = dfsToNodeMapping[A];
        int dfsNumberOfLCA = dfsToNodeMapping[lca];
        int sumAtoLCA = fenwickTree.Query(dfsNumberOfA) - fenwickTree.Query(dfsNumberOfLCA - 1);

        // Step 3: Query the path from B to LCA
        int dfsNumberOfB = dfsToNodeMapping[B];
        int sumBtoLCA = fenwickTree.Query(dfsNumberOfB) - fenwickTree.Query(dfsNumberOfLCA - 1);

        // Step 4: Combine the sums and avoid double-counting the LCA
        int lcaValue = fenwickTree.Query(dfsNumberOfLCA) - fenwickTree.Query(dfsNumberOfLCA - 1);
        int totalSum = sumAtoLCA + sumBtoLCA - lcaValue;

        return totalSum;
    }

    /*public int Query(int A, int B)
    {
        // Step 1: Find the Lowest Common Ancestor (LCA) of A and B
        int lca = FindLCA(A, B);

        // Step 2: Query the path from A to LCA and from B to LCA
        int sumAtoLCA = QueryPath(A, lca);
        int sumBtoLCA = QueryPath(B, lca);

        // Step 3: Sum up the two paths, but avoid counting LCA twice
        int lcaValue = fenwickTree.Query(dfnl[lca]);  // Assuming Query returns the value at a specific index
        int totalSum = sumAtoLCA + sumBtoLCA - lcaValue;

        return totalSum;
    }*/

    private int QueryPath(int start, int end)
    {
        int sum = 0;
        int current = start;

        // Traverse upwards from 'start' to 'end', querying the Fenwick Tree at each step
        while (current != end)
        {
            sum += fenwickTree.Query(dfnl[current]);
            current = up[current, 0];  // Move to the parent, assuming up[][] is your binary lifting table
        }

        return sum;
    }
    public int GetNodeNumber(int dfsNumber)
    {
        return dfsToNodeMapping[dfsNumber];
    }



    public List<int> EulerTour()
    {
        List<int> tour = new List<int>();
        EulerTourHelper(Root, tour);
        return tour;
    }

    private void EulerTourHelper(TreeNode node, List<int> tour)
    {
        // Add the current node to the tour when you visit it
        tour.Add(node.NodeNumber);

        // Visit all children
        foreach (var child in node.Children)
        {
            EulerTourHelper(child, tour);

            // Add the current node again to the tour when you backtrack
            tour.Add(node.NodeNumber);
        }
    }
    
}


class Solution
{
    
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

        
        RootedTree tree = new RootedTree(rootNumber,numNodes,edges);
        int[][] operations = new int[numQueries][];
        for(int i = 0; i < numQueries; i++)
        {
            string[] query = Console.ReadLine().TrimEnd().Split(' ');
            if (query[0] == "U")
            {
                int T = Convert.ToInt32(query[1]);
                int V = Convert.ToInt32(query[2]);
                int K = Convert.ToInt32(query[3]);
                tree.Update(T, V, K);
            }
            if (query[0] == "Q")
            {
                int A = Convert.ToInt32(query[1]);
                int B = Convert.ToInt32(query[2]);
                int result = tree.Query(A, B);
                Console.WriteLine(result);
            }
        }
        Console.ReadLine();
    }
}
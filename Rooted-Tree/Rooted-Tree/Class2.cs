using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rooted_Tree
{
    class TreeNode
    {
        public int NodeNumber { get; }
        public int Value { get; set; }
        public bool Passed { get; set; }
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
        public int[] tree;

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
        private List<int>[] adjMatrix;
        Dictionary<int, TreeNode> nodes = new Dictionary<int, TreeNode>();
        public int[] dfnl;
        public int[] dfnr;
        private Dictionary<int, int> dfsToNodeMapping = new Dictionary<int, int>();
        private int tick = 0;
        public int[] firstOccurrence;
        public int[] lastOccurrence;
        public int[] eulerTour;
        public RootedTree(int rootNumber, int nodeCount, int[][] edges, int rootValue = 0)
        {
            dfnl = new int[nodeCount + 1];
            dfnr = new int[nodeCount + 1];
            Root = new TreeNode(rootNumber, rootValue);
            nodes.Add(rootNumber, Root);
            maxLog = (int)Math.Ceiling(Math.Log(nodeCount, 2));
            up = new int[nodeCount + 1, maxLog + 1];  // Assuming 1-based node numbering
            depth = new int[nodeCount + 1];
            fenwickTree = new FenwickTree(nodeCount * 2);
            adjMatrix = new List<int>[edges.Length + 2];
            InitializeTree(edges);
            Precompute(rootNumber, Root, null);
            eulerTour = EulerTour().ToArray();
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
                if (adjNode != parent?.NodeNumber)
                {
                    TreeNode child = new TreeNode(adjNode);
                    node.AddChild(child);
                    nodes.Add(adjNode, child);
                    Precompute(child.NodeNumber, child, node);
                }
            }
            dfnr[nodeNumber] = tick;
            dfsToNodeMapping[dfnr[nodeNumber]] = nodeNumber;
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

        

        public List<int> EulerTour()
        {
            firstOccurrence = new int[depth.Length];
            lastOccurrence = new int[depth.Length];
            List<int> tour = new List<int>();
            EulerTourHelper(Root, tour);
            return tour;
        }

        private void EulerTourHelper(TreeNode node, List<int> tour)
        {
            // Record the first occurrence of this node in the Euler Tour
            if (firstOccurrence[node.NodeNumber] == 0)  // Assuming uninitialized values are zero
            {
                firstOccurrence[node.NodeNumber] = tour.Count;
            }

            tour.Add(node.NodeNumber);

            foreach (var child in node.Children)
            {
                EulerTourHelper(child, tour);
                tour.Add(node.NodeNumber);
            }

            // Record the last occurrence of this node in the Euler Tour
            lastOccurrence[node.NodeNumber] = tour.Count - 1;
        }
        public (int, int) GetEulerTourRange(int T)
        {
            return (firstOccurrence[T], lastOccurrence[T]);
        }
        
        public int GetNodeNumberFromEulerTour(int i)
        {
            return eulerTour[i];
        }
        public void Update(int T, int V, int K)
        {
            // Step 1: Identify the Euler Tour range for the subtree rooted at T
            // Let's assume you have a method GetEulerTourRange(T) that returns the (start, end) indices
            // of the Euler Tour segment corresponding to the subtree rooted at T.
            (int start, int end) = GetEulerTourRange(T);

            // Step 2: Update the Fenwick Tree for the identified range
            for (int i = start; i <= end; i++)
            {
                // Get the actual node number corresponding to Euler Tour index i
                // Assume you have a method GetNodeNumberFromEulerTour(i) for this
                int nodeNumber = GetNodeNumberFromEulerTour(i);

                // Calculate d, the depth difference between the node and T
                int d = depth[nodeNumber] - depth[T];

                // Calculate the amount to update
                int amount = V + d * K;

                // Update Fenwick Tree
                fenwickTree.Update(i, amount);
            }
        }
        public int Query(int A, int B)
        {
            // Step 1: Find the LCA of A and B
            int lca = FindLCA(A, B);

            // Step 2: Query the Euler Tour path from A to LCA and from B to LCA
            // Use GetEulerTourRange to get the Euler Tour indices for A, B, and LCA
            (int startA, int endA) = GetEulerTourRange(A);
            (int startB, int endB) = GetEulerTourRange(B);
            (int startLCA, int endLCA) = GetEulerTourRange(lca);

            // Query these paths in the Fenwick Tree
            int sumAtoLCA = QueryEulerTourPath(startA, startLCA);
            int sumBtoLCA = QueryEulerTourPath(startB, startLCA);

            // Sum up the two path sums
            int totalSum = sumAtoLCA + sumBtoLCA;

            // Remove the LCA value (it's counted twice)
            int lcaValue = fenwickTree.Query(startLCA); // Assuming Query returns the value at a specific index
            totalSum -= lcaValue;

            return totalSum;
        }

        private int QueryEulerTourPath(int start, int end)
        {
            // Ensure start is less than or equal to end
            if (start > end)
            {
                int temp = start;
                start = end;
                end = temp;
            }

            int sum = 0;
            for (int i = start; i <= end; i++)
            {
                sum += fenwickTree.Query(i);
            }
            return sum;
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
            int[][] edges = new int[numNodes - 1][];
            for (int i = 0; i < numNodes - 1; i++)
            {
                string[] edgeInput = Console.ReadLine().TrimEnd().Split(' ');
                int node1 = Convert.ToInt32(edgeInput[0]);
                int node2 = Convert.ToInt32(edgeInput[1]);
                int[] edge = new int[2] { node1, node2 };
                edges[i] = edge;
            }


            RootedTree tree = new RootedTree(rootNumber, numNodes, edges);
            int[][] operations = new int[numQueries][];
            for (int i = 0; i < numQueries; i++)
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
}

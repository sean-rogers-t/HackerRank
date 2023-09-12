﻿using System;

namespace Tree
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
        private List<int>[] adjMatrix;
        Dictionary<int, TreeNode> nodes = new Dictionary<int, TreeNode>();
        public int[] dfnl;
        public int[] dfnr;
        private Dictionary<int, int> dfsToNodeMapping = new Dictionary<int, int>();
        private int tick = 1;
        public RootedTree(int rootNumber, int nodeCount, int[][] edges, int rootValue = 0)
        {
            dfnl = new int[nodeCount + 1];
            dfnr = new int[nodeCount + 1];
            Root = new TreeNode(rootNumber, rootValue);
            nodes.Add(rootNumber, Root);
            maxLog = (int)Math.Ceiling(Math.Log(nodeCount, 2));
            up = new int[nodeCount + 1, maxLog + 1];  // Assuming 1-based node numbering
            depth = new int[nodeCount + 1];
            fenwickTree = new FenwickTree(2 * nodeCount);
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
                if (adjNode != parent?.NodeNumber)
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
        public void DirectUpdate(int nodeNumber, int value)
        {
            nodes[nodeNumber].Value = value;
        }
        public void Update(int node, int value)
        {
            int l = dfnl[node], int r = dfnr[node];
            fenwickTree.Update(l, value);
            fenwickTree.Update(r + 1, -value);
        }
        public int Query(int u, int v)
        {
            int lca = FindLCA(u, v);
            int sum = fenwickTree.Query(u) + fenwickTree.Query(v)
                - fenwickTree.Query(lca) - fenwickTree.Query(up[lca][0]);

        }
    }
    class BSolution
    {
        static void Main(string[] args)
        {
            int[][] edges = new int[4][];
            edges[0] = new int[2] { 1, 2 };
            edges[1] = new int[2] { 2, 3 };
            edges[2] = new int[2] { 2, 4 };
            edges[3] = new int[2] { 2, 5 };
            RootedTree tree = new RootedTree(1, 5, edges);
            int[] values = new int[5] { 1, 2, 3, 4, 5 };
            foreach (int v in values)
            {
                tree.Update(v, v);
            }
            int ans = tree.Query(3, 5);
            Console.WriteLine(ans);
            Console.ReadLine();
        }
    }
}
using System;
using System.Collections.Generic;
using System.IO;

public class OSolution
{
    const int D = 17;
    const int MOD = 1_000_000_007;
    static List<int>[] e;
    static int[][] par;
    static int[][] fenwick;
    static int[] dep;
    static int[] dfnl;
    static int[] dfnr;
    static int tick = 0;

    static int LowestCommonAncestor(int u, int v)
    {
        if (dep[u] < dep[v])
        {
            int temp = u;
            u = v;
            v = temp;
        }
        for (int i = D; --i >= 0;)
        {
            if (dep[u] - (1 << i) >= dep[v])
            {
                u = par[i][u];
            }
        }
        if (u == v)
        {
            return u;
        }
        for (int i = D; --i >= 0;)
        {
            if (par[i][u] != par[i][v])
            {
                u = par[i][u];
                v = par[i][v];
            }
        }
        return par[0][u];
    }

    class Node
    {
        public int u;
        public int p;
        public bool start = true;
        public Node(int u, int p)
        {
            this.u = u;
            this.p = p;
        }
    }

    static void Dfs(int u, int p)
    {
        Stack<Node> stack = new Stack<Node>();
        stack.Push(new Node(u, p));
        while (stack.Count > 0)
        {
            Node node = stack.Peek();
            if (node.start)
            {
                dfnl[node.u] = tick++;
                foreach (var v in e[node.u])
                {
                    if (v != node.p)
                    {
                        par[0][v] = node.u;
                        dep[v] = dep[node.u] + 1;
                        stack.Push(new Node(v, node.u));
                    }
                }
                node.start = false;
            }
            else
            {
                dfnr[node.u] = tick;
                stack.Pop();
            }
        }
    }

    static void Add(int[] fenwick, int x, int v)
    {
        for (; x < fenwick.Length; x |= x + 1)
        {
            fenwick[x] = (fenwick[x] + v) % MOD;
        }
    }

    static int GetSum(int[] fenwick, int x)
    {
        int s = 0;
        for (; x > 0; x &= x - 1)
        {
            s = (s + fenwick[x - 1]) % MOD;
        }
        return s;
    }

    static int Get(int u)
    {
        long pw = 1;
        long s = 0;
        for (int i = 0; i < 3; i++)
        {
            s = (s + pw * GetSum(fenwick[i], dfnl[u] + 1)) % MOD;
            pw = (pw * dep[u]) % MOD;
        }
        return (int)(((MOD + 1L) / 2 * s) % MOD);
    }

    static int Query(int u, int v)
    {
        int w = LowestCommonAncestor(u, v);
        long s = ((long)(Get(u)) + Get(v) - Get(w)) % MOD;
        if (par[0][w] >= 0)
        {
            s = (s - Get(par[0][w])) % MOD;
        }
        return (int)s;
    }

    static void Upd(int[] fenwick, int l, int r, int v)
    {
        Add(fenwick, l, v);
        Add(fenwick, r, -v);
    }

    static void Update(int u, int x, int y)
    {
        int l = dfnl[u];
        int r = dfnr[u];
        Upd(fenwick[2], l, r, y);
        Upd(fenwick[1], l, r, (int)(((long)(1 - 2 * dep[u]) * y + 2L * x) % MOD));
        Upd(fenwick[0], l, r, (int)((dep[u] * (dep[u] - 1L) * y + 2 * (1L - dep[u]) * x) % MOD));
    }

    public static void Main(string[] args)
    {
        StreamReader reader = new StreamReader(Console.OpenStandardInput());
        StreamWriter writer = new StreamWriter(Console.OpenStandardOutput());
        string[] input = reader.ReadLine().Split(' ');
        int n = int.Parse(input[0]);
        int m = int.Parse(input[1]);
        int rt = int.Parse(input[2]) - 1;

        e = new List<int>[n];
        for (int i = 0; i < n; i++)
        {
            e[i] = new List<int>();
        }
        for (int i = 0; i < n - 1; i++)
        {
            input = reader.ReadLine().Split(' ');
            int u = int.Parse(input[0]) - 1;
            int v = int.Parse(input[1]) - 1;
            e[u].Add(v);
            e[v].Add(u);
        }

        dep = new int[n];
        par = new int[D][];
        for (int i = 0; i < D; i++)
        {
            par[i] = new int[n];
        }
        dfnl = new int[n];
        dfnr = new int[n];
        tick = 0;
        dep[rt] = 0;
        par[0][rt] = -1;
        Dfs(rt, -1);

        for (int k = 1; k < D; k++)
        {
            for (int i = 0; i < n; i++)
            {
                par[k][i] = par[k - 1][i] == -1 ? par[k - 1][i] : par[k - 1][par[k - 1][i]];
            }
        }

        fenwick = new int[3][];
        for (int i = 0; i < 3; i++)
        {
            fenwick[i] = new int[n];
        }

        while (m-- > 0)
        {
            input = reader.ReadLine().Split(' ');
            char op = input[0][0];
            int u = int.Parse(input[1]) - 1;
            int v = int.Parse(input[2]);
            if (op == 'Q')
            {
                v--;
                int result = (Query(u, v) + MOD) % MOD;
                writer.WriteLine(result);
            }
            else
            {
                int w = int.Parse(input[3]);
                Update(u, v, w);
            }
        }

        writer.Flush();
    }
}

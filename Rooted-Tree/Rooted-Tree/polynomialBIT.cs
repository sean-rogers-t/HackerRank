using System;
using System.Collections.Generic;

class PSolution
{
    const int MOD = 1000000007;
    const int N = 100010;
    const long INV_2 = 500000004L;

    static int[] dep = new int[N], enter = new int[N], leave = new int[N];
    static int[][] parent = new int[N][];
    static List<int>[] adj = new List<int>[N];

    static long NegMod(long a, long b)
    {
        if (a < 0) a += (-a / b + 1) * b;
        return a % b;
    }

    class BIT<T>
    {
        List<T> data;
        public BIT(int sz = N)
        {
            data = new List<T>(new T[sz]);
        }
        public long Query(int l, int r)
        {
            long ans = 0;
            --l;
            while (l != r)
            {
                if (l < r)
                {
                    ans += Convert.ToInt64(data[r]);
                    r &= r - 1;
                }
                else
                {
                    ans -= Convert.ToInt64(data[l]);
                    l &= l - 1;
                }
                ans = NegMod(ans, MOD);
            }
            return ans;
        }
        public void Update(int idx, T delta)
        {
            while (idx < data.Count)
            {
                data[idx] = (dynamic)data[idx] + (dynamic)delta;
                data[idx] = (dynamic)NegMod((dynamic)data[idx], MOD);
                idx += idx & -idx;
            }
        }
    }

    static BIT<long>[] bts = { new BIT<long>(), new BIT<long>(), new BIT<long>() };

    static void Dfs(int cur, int par, int d, ref int tm)
    {
        enter[cur] = ++tm;
        dep[cur] = d;
        parent[cur][0] = par;
        for (int i = 1; 1 << i <= d; ++i)
        {
            parent[cur][i] = parent[parent[cur][i - 1]][i - 1];
        }
        foreach (var i in adj[cur])
        {
            if (i != par)
            {
                Dfs(i, cur, d + 1, ref tm);
            }
        }
        leave[cur] = tm;
    }

    static int Lca(int p, int q)
    {
        if (dep[p] < dep[q]) (p, q) = (q, p);
        int h = dep[p] - dep[q];
        while (h > 0)
        {
            p = parent[p][Convert.ToInt32(Math.Log(h & -h, 2))];
            h &= h - 1;
        }
        if (p == q) return p;
        for (int i = 17; i >= 0; --i)
        {
            if (1 << i <= dep[p] && parent[p][i] != parent[q][i])
            {
                p = parent[p][i];
                q = parent[q][i];
            }
        }
        return parent[p][0];
    }

    static void Update(int n, long v, long k)
    {
        int l = enter[n], r = leave[n];
        bts[2].Update(l, k);
        bts[2].Update(r + 1, -k);
        long d = NegMod(k - 2 * k * dep[n] + 2 * v, MOD);
        bts[1].Update(l, d);
        bts[1].Update(r + 1, -d);
        d = NegMod((dep[n] - 1) * (k * dep[n] - 2 * v), MOD);
        bts[0].Update(l, d);
        bts[0].Update(r + 1, -d);
    }

    static long _Query(int p)
    {
        int idx = enter[p];
        return (dep[p] * dep[p] % MOD * bts[2].Query(1, idx) % MOD + bts[1].Query(1, idx) * dep[p] % MOD +
            bts[0].Query(1, idx)) % MOD;
    }

    static long Query(int p, int q)
    {
        int c = Lca(p, q);
        long tmp = (_Query(p) + _Query(q)) % MOD;
        return NegMod(tmp - _Query(c) - _Query(parent[c][0]), MOD) * INV_2 % MOD;
    }

    static void Main(string[] args)
    {
        for (int i = 0; i < N; ++i)
        {
            parent[i] = new int[18];
            adj[i] = new List<int>();
        }
        string[] firstMultipleInput = Console.ReadLine().TrimEnd().Split(' ');
        /*int numNodes = Convert.ToInt32(firstMultipleInput[0]);
        int numQueries = Convert.ToInt32(firstMultipleInput[1]);
        int rootNumber = Convert.ToInt32(firstMultipleInput[2]);*/

        int n = Convert.ToInt32(firstMultipleInput[0]);
        int e = Convert.ToInt32(firstMultipleInput[1]);
        int r = Convert.ToInt32(firstMultipleInput[2]);
        for (int i = 1; i < n; ++i)
        {
            string[] inputs = Console.ReadLine().Split(' ');
            int s = int.Parse(inputs[0]);
            int t = int.Parse(inputs[1]);
            adj[s].Add(t);
            adj[t].Add(s);
        }
        int tm = 0;
        Dfs(r, 0, 0, ref tm);
        for (int i = 0; i < e; ++i)
        {
            char c = Console.ReadLine()[0];
            if (c == 'U')
            {
                string[] inputs = Console.ReadLine().Split(' ');
                int t = int.Parse(inputs[0]);
                long v = long.Parse(inputs[1]);
                long k = long.Parse(inputs[2]);
                Update(t, v, k);
            }
            else
            {
                string[] inputs = Console.ReadLine().Split(' ');
                int a = int.Parse(inputs[0]);
                int b = int.Parse(inputs[1]);
                Console.WriteLine(Query(a, b));
            }
        }
    }
}


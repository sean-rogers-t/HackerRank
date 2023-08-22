using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Text;
using System;

class Result
{

    /*
     * Complete the 'dynamicArray' function below.
     *
     * The function is expected to return an INTEGER_ARRAY.
     * The function accepts following parameters:
     *  1. INTEGER n
     *  2. 2D_INTEGER_ARRAY queries
     */

    public static List<int> dynamicArray(int n, List<List<int>> queries)
    {
        List<List<int>> arr = new List<List<int>>();
        for (int i = 0; i < n; i++)
        {
            arr.Add(new List<int>());
        }
        List<int> answer = new List<int>();
        int lastAnswer = 0;
        int idx = 0;
        for (int i = 0; i < queries.Count; i++)
        {
            List<int> query = queries[i];
            int type = query[0];
            int x = query[1];
            int y = query[2];
            if (type == 1)
            {
                idx = (x ^ lastAnswer) % n;
                arr[idx].Add(y);
            }
            if (type == 2)
            {
                idx = (x ^ lastAnswer) % n;
                lastAnswer = arr[idx][y % arr[idx].Count];
                answer.Add(lastAnswer);
            }

        }
        return answer;
    }

}

class Solution
{
    public static void Main(string[] args)
    {
        string[] firstMultipleInput = Console.ReadLine().TrimEnd().Split(' ');

        int n = Convert.ToInt32(firstMultipleInput[0]);
        int q = Convert.ToInt32(firstMultipleInput[1]);

        List<List<int>> queries = new List<List<int>>();

        for (int i = 0; i < q; i++)
        {
            queries.Add(Console.ReadLine().TrimEnd().Split(' ').ToList().Select(queriesTemp => Convert.ToInt32(queriesTemp)).ToList());
        }

        List<int> result = Result.dynamicArray(n, queries);

        foreach (int value in result)
        {
            Console.WriteLine(value);
        }
        Console.ReadLine();
    }

}

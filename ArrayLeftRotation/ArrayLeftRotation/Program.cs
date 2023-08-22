﻿using System.CodeDom.Compiler;
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
     * Complete the 'rotateLeft' function below.
     *
     * The function is expected to return an INTEGER_ARRAY.
     * The function accepts following parameters:
     *  1. INTEGER d
     *  2. INTEGER_ARRAY arr
     */

    public static List<int> rotateLeft(int d, List<int> arr)
    {
        int n = arr.Count;
        List<int> answer = new List<int>();
        for (int i = 0; i < arr.Count; i++)
        {
            answer.Add(0); // You can initialize with any initial value you want
        }
        for (int i = 0; i < arr.Count; i++)
        {
            answer[i] = arr[(i + d) % n];
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

        int d = Convert.ToInt32(firstMultipleInput[1]);

        List<int> arr = Console.ReadLine().TrimEnd().Split(' ').ToList().Select(arrTemp => Convert.ToInt32(arrTemp)).ToList();

        List<int> result = Result.rotateLeft(d, arr);

        foreach(int item  in result) { Console.WriteLine(item); }
        Console.ReadLine();
    }
}


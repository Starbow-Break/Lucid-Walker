using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ListShuffler
{
    public static void Shuffle<T>(List<T> list, int shuffleCount)
    {
        for(int i = 0; i < shuffleCount; i++)
        {
            int a = Random.Range(0, list.Count);
            int b = Random.Range(0, list.Count);
            (list[a], list[b]) = (list[b], list[a]);
        }
    }
}

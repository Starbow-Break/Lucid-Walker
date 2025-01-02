using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiableRandom
{
    System.Random random;

    public InstantiableRandom() {
        random = new System.Random();
    }

    public InstantiableRandom(int seed) {
        random = new System.Random(seed);
    }

    public int Range(int minValue, int maxValue) {
        return random.Next(minValue, maxValue);
    }

    public float Range(float minValue, float maxValue) {
        return (float)random.NextDouble() * (maxValue - minValue) + minValue;
    }
}

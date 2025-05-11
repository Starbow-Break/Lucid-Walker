using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoinHelper
{
    public static int GetRaiseCoin(int baseCost, int luck) {
        int rv = Random.Range(1, 101);
        return baseCost * (rv <= luck ? 2 : 1);
    }
}

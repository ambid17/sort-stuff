using UnityEngine;

public static class RandomChance
{
    public static bool PercentCheck(float chance)
    {
        return Random.Range(0f, 1f) < chance;
    }
}

public interface IRandom
{
    void SetSeed(string seed);
    void SetSeed(int seed);
    float value(bool inclusive = true);
    float Range(float min, float max, int decimalPlaces = 0);
    int IntRange(int min, int max, bool inclusive);
}
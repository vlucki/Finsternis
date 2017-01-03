namespace Finsternis
{

    [System.Serializable]
    public struct RangeI
    {
        public int min;
        public int max;

        public RangeI(int min, int max)
        {
            this.min = min;
            this.max = max;
        }
    }

    [System.Serializable]
    public struct RangeF
    {
        public float min;
        public float max;

        public RangeF(float min, float max)
        {
            this.min = min;
            this.max = max;
        }
    }
}
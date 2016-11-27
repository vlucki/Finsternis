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
}
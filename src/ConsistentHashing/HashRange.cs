namespace ConsistentHashing
{
    public struct HashRange
    {
        public HashRange(uint start, uint end)
        {
            this.StartExclusive = start;
            this.EndInclusive = end;
        }

        public uint StartExclusive { get; }

        public uint EndInclusive { get; }
    }
}

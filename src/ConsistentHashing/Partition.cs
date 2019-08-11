namespace ConsistentHashing
{
    public struct Partition<T>
    {
        public Partition(T node, HashRange range)
        {
            this.Node = node;
            this.Range = range;
        }

        public T Node { get; }

        public HashRange Range { get; }
    }
}

namespace Finsternis
{
    [System.Serializable]
    public sealed class CardStack
    {
        public readonly Card card;

        public int Count { get; private set; }

        public bool IsEmpty { get { return this.Count == 0; } }

        public static implicit operator bool(CardStack stack)
        {
            return stack != null;
        }

        public CardStack(Card card)
        {
            this.card = card;
            AddCard();
        }

        public void AddCard()
        {
            this.Count++;
        }

        public bool RemoveCard()
        {
            if (this.Count > 0)
                this.Count--;
            return this.IsEmpty;
        }
    }
}
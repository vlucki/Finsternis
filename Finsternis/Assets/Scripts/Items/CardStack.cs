namespace Finsternis
{
    using UnityEngine.Events;
    [System.Serializable]
    public sealed class CardStack
    {
        public readonly Card card;

        public UnityEvent onCardAdded;
        public UnityEvent onCardRemoved;

        public int Count { get; private set; }

        public bool IsEmpty { get { return this.Count == 0; } }

        public static implicit operator bool(CardStack stack)
        {
            return stack != null;
        }

        public CardStack(Card card)
        {
            this.card = card;
            this.onCardAdded = new UnityEvent();
            this.onCardRemoved = new UnityEvent();
            AddCard();
        }

        public void AddCard()
        {
            this.Count++;
            onCardAdded.Invoke();
        }

        public bool RemoveCard()
        {
            if (this.Count > 0)
            {
                this.Count--;
                onCardRemoved.Invoke();
            }
            return this.IsEmpty;
        }
    }
}
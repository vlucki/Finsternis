using UnityEngine;
using System.Collections;

namespace Finsternis
{
    public static class CardFactory
    {

        public static TextAsset cardGenerationParameters;

        public static Card CreateCard()
        {
            Card c = ScriptableObject.CreateInstance<Card>();

            return c;
        }

    }
}
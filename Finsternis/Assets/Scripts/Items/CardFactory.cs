using UnityEngine;
using System.Collections.Generic;
using System;

namespace Finsternis
{
    public static class CardFactory
    {
        public static TextAsset cardGenerationParameters;

        public static List<CardName> prenames;
        public static List<CardName> postnames;
        public static List<CardName> names;

        public static Card CreateCard()
        {
            Card c = ScriptableObject.CreateInstance<Card>();

            return c;
        }

        private static void LoadParameters()
        {
            prenames = new List<CardName>();
            postnames = new List<CardName>();
            names = new List<CardName>();
        }



        public sealed class CardComparer : IComparer<CardName>
        {
            public int Compare(CardName x, CardName y)
            {
                if (x == y)
                    return 0;
                if (!x && y)
                    return -1;
                if (x && !y)
                    return 1;

                if (x.Rarity == y.Rarity)
                    return x.name.CompareTo(y.name);
                else
                    return x.Rarity.CompareTo(y.Rarity);

            }
        }
    }
}
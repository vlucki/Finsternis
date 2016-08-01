using UnityEngine;
using System.Collections.Generic;
using System;

using Random = UnityEngine.Random;

namespace Finsternis
{
    [Serializable]
    public static class CardFactory
    {
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

        public static List<CardName> prenames;
        public static List<CardName> postnames;
        public static List<CardName> names;

        private struct JSONKeys {
            public const string PreNames         = "PreNames";
            public const string Names            = "Names";
            public const string PostNames        = "PostNames";
            public const string Modifiers        = "modifiers";
            public const string AbsoluteModifier = "absolute";
            public const string RelativeModifier = "relative";
        };

        private const int MaxNameSelectionTries = 10000;

        private static bool initialized;

        public static Card MakeCard(float rarityLimit = 1)
        {
            if (!CardFactory.initialized)
                LoadParameters();

            Card c = ScriptableObject.CreateInstance<Card>();

            CardName[] fullName = new CardName[3];

            if (Random.value > 0.5)
                fullName[0] = GetRandomName(new List<CardName>(prenames), rarityLimit);

            fullName[1] = GetRandomName(new List<CardName>(names), rarityLimit, false);

            if (Random.value > 0.5)
                fullName[2] = GetRandomName(new List<CardName>(postnames), rarityLimit);

            for (int i = 0; i < fullName.Length; i++)
                if (fullName[i])
                    c.AppendName(fullName[i]);

            return c;
        }


        public static CardName GetRandomName(List<CardName> list, float rarityLimit, bool mayReturnNull = true)
        {
            CardName name;
            int tries = 0;
            do
            {
                int index = Random.Range(0, list.Count);
                name = list[index];
                list.RemoveAt(index);
            } while (++tries < MaxNameSelectionTries && list.Count > 0);

            return mayReturnNull ? null : name; //if a name is REQUIRED, return the last one found
        }

        private static List<CardName> LoadNames(string JSONKey, JSONObject JSON)
        {
            List<CardName> names = new List<CardName>();
            List<JSONObject> namesJSON = JSON.GetFieldList(JSONKeys.PreNames);
            foreach (var nameJSON in namesJSON)
            {
                CardName name = ScriptableObject.CreateInstance<CardName>();
                name.name = nameJSON.keys[0];

                JSONObject modifiersJSON = nameJSON.list[0].GetField(JSONKeys.Modifiers);
                if (modifiersJSON)
                {
                    List<JSONObject> absoluteModifierJSON = modifiersJSON.GetFieldList(JSONKeys.AbsoluteModifier);
                    if (absoluteModifierJSON != null)
                    {
                        foreach (var absoluteBuff in absoluteModifierJSON)
                        {
                            name.AddEffect(new AttributeModifier(absoluteBuff.keys[0], absoluteBuff.list[0].f));
                        }
                    }

                    List<JSONObject> relativeModifiers = modifiersJSON.GetFieldList(JSONKeys.RelativeModifier);
                    if (relativeModifiers != null)
                    {
                        foreach (var relativeModifierJSON in relativeModifiers)
                        {
                            name.AddEffect(new AttributeModifier(relativeModifierJSON.keys[0], relativeModifierJSON.list[0].f));
                        }
                    }
                }

                names.Add(name);
            }
            return CardFactory.names;
        }

        private static void LoadParameters()
        {
            TextAsset cardGenerationParametersFile = Resources.Load<TextAsset>("CardGenParameters");

            JSONObject JSONObj = new JSONObject(cardGenerationParametersFile.text, -2, false, true);
            prenames = LoadNames("PreNames", JSONObj);
            names = LoadNames("Names", JSONObj);
            postnames = LoadNames("Names", JSONObj);

            initialized = true;
        }

    }
}
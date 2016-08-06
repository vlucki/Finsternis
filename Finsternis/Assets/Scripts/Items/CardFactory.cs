using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

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

        public static MTRandom Random = new MTRandom();

        public static readonly string[] AttributeAlias = { "hp", "mp", "str", "spd", "def", "int" };

        public const int MaxEffectsPerName = 4;

        private struct JSONKeys {
            public const string PreNames    = "PreNames";
            public const string Names       = "Names";
            public const string PostNames   = "PostNames";
            public const string Effects     = "effects";
        };

        private const int MaxNameSelectionTries = 10000;

        private static bool initialized;

        public static Card MakeCard(float rarityLimit = 1)
        {
            if (!CardFactory.initialized)
                LoadParameters();

            Card c = ScriptableObject.CreateInstance<Card>();

            CardName[] fullName = new CardName[3];

            if (Random.value() > 0.5)
                fullName[0] = GetRandomName(new List<CardName>(prenames), rarityLimit);

            fullName[1] = GetRandomName(new List<CardName>(names), rarityLimit, false);

            if (Random.value() > 0.5)
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
                int index = Random.Range(0, list.Count-1);
                name = list[index];
                list.RemoveAt(index);
            } while (++tries < MaxNameSelectionTries && list.Count > 0);

            return mayReturnNull ? null : name; //if a name is REQUIRED, return the last one found
        }

        private static List<CardName> LoadNames(string JSONKey, JSONObject JSON)
        {
            List<CardName> names        = new List<CardName>();
            List<JSONObject> namesJSON  = JSON.GetFieldList(JSONKey);
            List<string> stringNames    = new List<string>();

            foreach (var nameJSON in namesJSON)
            {
                stringNames.Add(nameJSON.keys[0]);
                CardName name = GenerateName(nameJSON.keys[0], nameJSON.GetField(nameJSON.keys[0]));
                
                names.Add(name);
            }

            return names;
        }

        private static CardName GenerateName(string nameString, JSONObject nameParameters)
        {
            CardName name = ScriptableObject.CreateInstance<CardName>();
            name.name = nameString;

            Random = new MTRandom(name.name);
            AddEffects(name, nameParameters.GetField(JSONKeys.Effects).list);
            return name;
        }

        private static void AddEffects(CardName name, List<JSONObject> effects)
        {
            foreach(var effect in effects)
            {
                if (effect.IsString)
                {
                    name.AddEffect(ParseEffect(effect.str));
                }
            }
        }

        private static Effect ParseEffect(string str)
        {
            string attributeModifierTokens = "+-*/";
            char token = str[0];
            if (attributeModifierTokens.Contains(token))
            {
                switch (token)
                {
                    case '+':
                        return new AttributeModifier(str.Substring(1), Random.Range(0.1f, 10f, 1));
                    case '-':
                        return new AttributeModifier(str.Substring(1), Random.Range(-10f, -0.1f, 1));
                    case '*':
                        return new AttributeModifier(str.Substring(1), Random.Range(1f, 4f, 1), AttributeModifier.ModifierType.Relative);
                    case '/':
                        return new AttributeModifier(str.Substring(1), Random.Range(0.1f, 1f, 1), AttributeModifier.ModifierType.Relative);
                }
            }
            else
            {

            }

            return null;
        }

        public static void LoadParameters()
        {
            TextAsset cardGenerationParametersFile = Resources.Load<TextAsset>("CardGenParameters");

            JSONObject JSONObj = new JSONObject(cardGenerationParametersFile.text, -2, false, true);
            prenames = LoadNames(JSONKeys.PreNames, JSONObj);
            names = LoadNames(JSONKeys.Names, JSONObj);
            postnames = LoadNames(JSONKeys.PostNames, JSONObj);

            initialized = true;
        }

    }
}
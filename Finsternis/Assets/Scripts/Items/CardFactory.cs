using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Finsternis
{
    [Serializable]
    public static class CardFactory
    {
        internal sealed class CardComparer : IComparer<CardName>
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

        public static List<CardName>[] names;

        public static MTRandom Random;

        public static readonly string[] AttributeAlias = { "hp", "mp", "str", "spd", "def", "int" };

        public const int MaxNameSelectionTries = 4;
        private static bool initialized;

        private static List<CardName> ChooseNames(float rarityLimit)
        {
            List<CardName> chosenNames = new List<CardName>();
            //3 iterations:
            //1st: prenames
            //2nd: main name
            //3rd: postnames
            for (int nameIndex = 0; nameIndex < 3; nameIndex++)
            {
                if (nameIndex != 1)
                {
                    float nameChance = 0.5f;
                    int count = Random.Range(0, 2);
                    for (int i = 0; i < count; i++) //Try to add up to 2 pre and post names
                    {
                        if (Random.value() <= nameChance) //if they pass the test
                        {
                            chosenNames.Add(
                                GetRandomName(chosenNames, names[nameIndex], rarityLimit));
                            nameChance /= 2;
                        }
                    }
                }
                else //Add the "main" name of the weapon
                {
                    chosenNames.Add(GetRandomName(chosenNames, names[nameIndex], rarityLimit));
                }
            }

            return chosenNames;
        }

        public static Card MakeCard(float rarityLimit = 1)
        {
            if (!CardFactory.initialized)
                LoadParameters();

            Card card = ScriptableObject.CreateInstance<Card>();
            var chosenNames = ChooseNames(rarityLimit);

            HashSet<CardName> usedNames = new HashSet<CardName>();

            for(int i = 0; i < chosenNames.Count; i++)
            {
                CardName name = chosenNames[i];
                if (!usedNames.Contains(name) || name.IsStackable)
                {
                    usedNames.Add(name);
                    
                    card.AppendName(name);
                }
            }
            card.UpdateName();

            return card;
        }

        private static string GetAdditionalNameString(CardName name, List<CardName> chosenNames)
        {
            if (name.Type != CardName.NameType.MainName)
            {
                if (chosenNames.Count > 2 && chosenNames[chosenNames.Count - 2].Type == name.Type)
                    return "and";
                else if (name.Type == CardName.NameType.PostName)
                {
                    return name.prepositions[Random.Range(0, name.prepositions.Count - 1)];
                }
            }
            return null;
        }

        private static CardName GetRandomName(List<CardName> namesBeingUsed, List<CardName> availableNames, float rarityLimit)
        {
            CardName name = null;
            int tries = 0;
            HashSet<CardName> triedNames = new HashSet<CardName>(namesBeingUsed);
            do
            {
                if (name)
                    triedNames.Add(name);

                name = availableNames[Random.Range(0, availableNames.Count - 1)];

            } while (++tries < MaxNameSelectionTries
                    && !triedNames.Contains(name));
            return name;
        }

        #region Cards initialization

        private static Effect ParseEffect(JSONObject effectJSON)
        {
            if (effectJSON.IsString)
            {
                string effect = effectJSON.str;
                string attributeModifierTokens = "+-*/";
                char token = effect[0];
                if (attributeModifierTokens.Contains(token))
                {
                    switch (token)
                    {
                        case '+':
                            return new AttributeModifier(effect.Substring(1), Random.Range(0.1f, 10f, 1));
                        case '-':
                            return new AttributeModifier(effect.Substring(1), Random.Range(-10f, -0.1f, 1));
                        case '*':
                            return new AttributeModifier(effect.Substring(1), Random.Range(1f, 4f, 1), AttributeModifier.ModifierType.Relative);
                        case '/':
                            return new AttributeModifier(effect.Substring(1), Random.Range(0.1f, 1f, 1), AttributeModifier.ModifierType.Relative);
                    }
                }
                else
                {

                }
            }

            return null;
        }

        private static void AddEffects(CardName name, List<JSONObject> effects)
        {
            foreach (var effect in effects)
            {
                if (effect.IsString)
                {
                    name.AddEffect(ParseEffect(effect));
                }
            }
        }

        private static CardName GenerateName(string nameString, CardName.NameType type, JSONObject nameParameters)
        {
            CardName name = ScriptableObject.CreateInstance<CardName>();
            name.Init(nameString, type);
            Random = new MTRandom(name.name);
            AddEffects(name, nameParameters.GetField("effects").list);
            JSONObject prepositions = nameParameters.GetField("prepositions");
            if (prepositions != null)
            {
                foreach (var v in prepositions.list)
                {
                    name.prepositions.Add(v.str);
                }
            }
            return name;
        }

        private static List<CardName> LoadNames(JSONObject JSON, CardName.NameType type)
        {
            List<CardName> names = new List<CardName>();
            List<JSONObject> namesJSON = JSON.GetFieldList(type + "s");
            List<string> stringNames = new List<string>();

            foreach (var nameJSON in namesJSON)
            {
                stringNames.Add(nameJSON.keys[0]);
                CardName name = GenerateName(nameJSON.keys[0], type, nameJSON.GetField(nameJSON.keys[0]));
                names.Add(name);
            }

            return names;
        }

        public static void LoadParameters()
        {
            TextAsset cardGenerationParametersFile = Resources.Load<TextAsset>("CardGenParameters");

            JSONObject JSONObj = new JSONObject(cardGenerationParametersFile.text, -2, false, true);
            names = new List<CardName>[] {
                LoadNames(JSONObj, CardName.NameType.PreName),
                LoadNames(JSONObj, CardName.NameType.MainName),
                LoadNames(JSONObj, CardName.NameType.PostName)
            };

            initialized = true;
        }

        #endregion

    }
}
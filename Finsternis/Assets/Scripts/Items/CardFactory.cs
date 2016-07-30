using UnityEngine;
using System.Collections.Generic;
using System;

namespace Finsternis
{
    public static class CardFactory
    {
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

        private static bool initialized;

        public static Card CreateCard()
        {
            if (!CardFactory.initialized)
                LoadParameters();

            Card c = ScriptableObject.CreateInstance<Card>();

            return c;
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
            prenames = new List<CardName>();
            postnames = new List<CardName>();
            names = new List<CardName>();

            TextAsset cardGenerationParametersFile = Resources.Load<TextAsset>("CardGenParameters");

            JSONObject jsonObj = new JSONObject(cardGenerationParametersFile.text, -2, false, true);
            List<JSONObject> prenamesJSON = jsonObj.GetFieldList(JSONKeys.PreNames);

            foreach(var prenameJSON in prenamesJSON)
            {
                CardName prename = ScriptableObject.CreateInstance<CardName>();
                prename.name = prenameJSON.keys[0];
                
                JSONObject modifiersJSON = prenameJSON.list[0].GetField(JSONKeys.Modifiers);
                if (modifiersJSON)
                {
                    List<JSONObject> absoluteModifierJSON = modifiersJSON.GetFieldList(JSONKeys.AbsoluteModifier);
                    if (absoluteModifierJSON != null)
                    {
                        foreach(var absoluteBuff in absoluteModifierJSON)
                        {
                            prename.AddEffect(new AttributeModifier(absoluteBuff.keys[0], absoluteBuff.list[0].f));
                        }
                    }

                    List<JSONObject> relativeModifiers = modifiersJSON.GetFieldList(JSONKeys.RelativeModifier);
                    if (relativeModifiers != null)
                    {
                        foreach (var relativeModifierJSON in relativeModifiers)
                        {
                            prename.AddEffect(new AttributeModifier(relativeModifierJSON.keys[0], relativeModifierJSON.list[0].f));
                        }
                    }
                }

                prenames.Add(prename);
            }


            initialized = true;
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
namespace Finsternis
{
    using UnityEngine;
    using System.Collections.Generic;
    using System;
    using Random = UnityEngine.Random;
    using System.Linq;

    [Serializable]
    public class CardFactory
    {

        private class NameList : List<CardName>
        {
            public new void Add(CardName name)
            {
                if(!this.Contains(name))
                    base.Add(name);
            }

            public List<CardName> GetNamesRarerThan(float minRarity, bool includeLimit = false)
            {
                if(!includeLimit)
                    return this.FindAll(name => name.Rarity < minRarity);
                else
                    return this.FindAll(name => name.Rarity <= minRarity);
            }

            public List<CardName> GetNamesMoreCommonThan(float minRarity, bool includeLimit = false)
            {
                if (!includeLimit)
                    return this.FindAll(name => name.Rarity > minRarity);
                else
                    return this.FindAll(name => name.Rarity >= minRarity);
            }

        }

        private NameList[] names;

        public const int MaxNameSelectionTries = 4;

        public CardFactory(List<CardName> names)
        {
            this.names = new NameList[3];
            names.ForEach(name =>
            {
                this.names[(int)name.Type].Add(name);
            });
        }

        public Card MakeCard()
        {
            Card card = Card.CreateInstance<Card>();
            var chosenNames = ChooseNames(this.names);

            List<CardName> usedNames = new List<CardName>();

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

        private List<CardName> ChooseNames(List<CardName>[] names)
        {
            List<CardName> chosenNames = new List<CardName>();

            for (int nameIndex = 0; nameIndex < 3; nameIndex++)
            {
                if (nameIndex != 1)
                {
                    float nameChance = 0.5f;
                    int count = Random.Range(0, 2);
                    for (int i = 0; i < count; i++) //Try to add up to 2 pre and post names
                    {
                        if (Random.value <= nameChance) //if they pass the test
                        {
                            chosenNames.Add(
                                GetRandomName(chosenNames, names[nameIndex]));
                            nameChance /= 2;
                        }
                    }
                }
                else //Add the "main" name of the weapon
                {
                    chosenNames.Add(GetRandomName(chosenNames, names[nameIndex]));
                }
            }


            return chosenNames;
        }

        private CardName GetRandomName(List<CardName> namesBeingUsed, List<CardName> availableNames)
        {
            CardName name = null;
            int tries = 0;
            HashSet<CardName> triedNames = new HashSet<CardName>(namesBeingUsed);
            do
            {
                if (name)
                    triedNames.Add(name);

                name = availableNames[UnityEngine.Random.Range(0, availableNames.Count - 1)];

            } while (++tries < MaxNameSelectionTries
                    && !triedNames.Contains(name));
            return name;
        }
        
    }
}
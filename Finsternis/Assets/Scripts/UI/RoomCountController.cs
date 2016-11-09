namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using UnityEngine.UI;
    using System;

    public class RoomCountController : MonoBehaviour
    {
        [Serializable]
        public struct Option
        {
            public Text optionLbl;
            public int roomCount;

            public static bool operator != (Option a, Option b)
            {
                return !(a == b);
            }


            public static bool operator ==(Option a, Option b)
            {
                return a.optionLbl == b.optionLbl && a.roomCount == b.roomCount;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is Option))
                    return false;

                return this == (Option)obj;
            }

            public override int GetHashCode()
            {
                return roomCount * 13 + (this.optionLbl ? this.optionLbl.GetHashCode() : 0);
            }
        }

        [Serializable]
        public class RoomCountEvent : CustomEvent<int> { }

        [SerializeField]
        private Option[] options;

        [SerializeField]
        private int currentOptionIndex = 1;

        public RoomCountEvent onRoomCountChanged;

        private Option currentOption;

        void Awake()
        {
            this.currentOption = this.options[this.currentOptionIndex];
            UpdateDisplay(default(Option));
        }

        public void UpdateSelection(float value)
        {
            int intValue = Mathf.RoundToInt(value);
            Option lastOption = this.currentOption;
            if (intValue > 0)
            {
                if (this.currentOptionIndex < this.options.Length - 1)
                {
                    this.currentOptionIndex++;
                }
            }
            else if (intValue < 0)
            {
                if (this.currentOptionIndex > 0)
                {
                    this.currentOptionIndex--;
                }
            }
            this.currentOption = this.options[this.currentOptionIndex];

            if(this.currentOption != lastOption)
            {
                UpdateDisplay(lastOption);
                onRoomCountChanged.Invoke(this.currentOption.roomCount);
            }
        }

        private void UpdateDisplay(Option lastOption)
        {
            if (lastOption.optionLbl)
            {
                UpdateLabel(lastOption.optionLbl, .2f);
            }

            UpdateLabel(currentOption.optionLbl, 1f);
        }

        private void UpdateLabel(Text label, float alpha)
        {
            var color = label.color;
            color.a = alpha;
            label.color = color;
        }
    }
}
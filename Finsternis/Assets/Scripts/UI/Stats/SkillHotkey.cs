namespace Finsternis
{
    using System;
    using UnityEngine;
    using UnityQuery;
    using UnityEngine.UI;

    public class SkillHotkey : CustomBehaviour
    {
        private Image image;
        private Text skillNameTxt;

        private Skill skill;

        protected override void Awake()
        {
            base.Awake();
            GameManager.Instance.onPlayerSpawned.AddListener(GrabPlayer);
            this.image = transform.Find("Hotkey").GetComponent<Image>();
            this.skillNameTxt = transform.Find("Skillname").GetComponent<Text>();
        }

        private void GrabPlayer(CharController player)
        {
            GameManager.Instance.onPlayerSpawned.RemoveListener(GrabPlayer);
            this.skill = player.EquippedSkills[transform.GetSiblingIndex()];
            if (this.skill)
            {
                this.skill.onBegin.AddListener(s => this.image.color = this.image.color.WithAlpha(.3f));
                this.skill.onEnd.AddListener(s => this.image.color = this.image.color.WithAlpha(.6f));
                this.skill.onCoolDownEnd.AddListener(s => this.image.color = this.image.color.WithAlpha(1f));
                this.skillNameTxt.text = this.skill.Name;
            }
            else
            {
                this.image.color = this.image.color.WithAlpha(.1f);
                this.skillNameTxt.text = "";
            }
        }
    }
}
namespace Finsternis
{
    using Extensions;
    using UnityEngine.UI;

    public class SkillHotkey : CustomBehaviour
    {
        private Image image;
        private Text skillNameTxt;

        private Skill skill;

        private void Awake()
        {            
            this.image = transform.Find("Hotkey").GetComponent<Image>();
            this.skillNameTxt = transform.Find("Skillname").GetComponent<Text>();
            if (!GameManager.Instance.Player)
                GameManager.Instance.onPlayerSpawned += (GrabPlayer);
            else
                GrabPlayer(GameManager.Instance.Player);
        }

        private void GrabPlayer(CharController player)
        {
            GameManager.Instance.onPlayerSpawned -= (GrabPlayer);
            this.skill = player.EquippedSkills[transform.GetSiblingIndex()];
            if (this.skill)
            {
                this.skill.onBegin.AddListener(s => this.image.color = this.image.color.Set(a: .3f));
                this.skill.onEnd.AddListener(s => this.image.color = this.image.color.Set(a: .6f));
                this.skill.onCoolDownEnd.AddListener(s => this.image.color = this.image.color.Set(a: 1f));
                this.skillNameTxt.text = this.skill.Name;
            }
            else
            {
                this.image.color = this.image.color.Set(a: .1f);
                this.skillNameTxt.text = "";
            }
        }
    }
}
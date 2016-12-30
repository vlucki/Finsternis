namespace Finsternis
{
    using UnityEngine;
    using System.Collections;
    using UnityEngine.UI;

    public class ConfigMenurController : MenuController
    {
        private AudioManager audioManager;

        protected override void Awake()
        {
            base.Awake();

            var sliders = this.GetComponentsInChildren<Slider>(true);

            sliders[0].value = PlayerPrefs.GetFloat("bgmVolume", 0);
            sliders[1].value = PlayerPrefs.GetFloat("sfxVolume", 0);
        }
    }
}
namespace Finsternis {
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using UnityQuery;
    
    public class AudioRandomizer : MonoBehaviour
    {
        [SerializeField]
        private AudioPlayer player;

        [SerializeField]
        private List<AudioClip> clips;

        public void PlayRandom()
        {
            this.player.Play(clips.GetRandom(Random.Range));
        }
    }
}

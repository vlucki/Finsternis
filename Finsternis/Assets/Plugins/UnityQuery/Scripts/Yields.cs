namespace UnityQuery
{
    using UnityEngine;
    using System.Collections;

    public static class Yields
    {

        public static WaitForSeconds Seconds(float seconds)
        {
            return new WaitForSeconds(seconds);
        }

        public static WaitForEndOfFrame Frame()
        {
            return new WaitForEndOfFrame();
        }

        public static WaitForFixedUpdate Update()
        {
            return new WaitForFixedUpdate();
        }
        
    }
}
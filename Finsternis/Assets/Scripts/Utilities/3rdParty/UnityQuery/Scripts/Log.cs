// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Log.cs" company="Nick Prühs">
//   Copyright (c) Nick Prühs. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace UnityQuery
{
    using UnityEngine;

    public static class Log
    {
        #region Public Methods and Operators

        #region error
        public static void E(Object context, string s)
        {
            Debug.LogError(s.ToLogString(context), context);
        }

        public static void E(Object context, string s, params object[] args)
        {
            Debug.LogErrorFormat(context, s.ToLogString(context), args);
        }
        #endregion

        #region info
        public static void I(Object context, bool condition, string s)
        {
            if(condition)
                Debug.Log(s.ToLogString(context), context);
        }

        public static void I(Object context, string s)
        {
            Debug.Log(s.ToLogString(context), context);
        }

        public static void I(Object context, string s, params object[] args)
        {
            Debug.LogFormat(context, s.ToLogString(context), args);
        }

        public static void I(Object context, bool condition, string s, params object[] args)
        {
            if(condition)
                Debug.LogFormat(context, s.ToLogString(context), args);
        }
        #endregion

        #region warn
        public static void W(Object context, string s)
        {
            Debug.LogWarning(s.ToLogString(context), context);
        }

        public static void W(Object context, string s, params object[] args)
        {
            Debug.LogWarningFormat(context, s.ToLogString(context), args);
        }
        #endregion

        public static string WithFrame(this string s)
        {
            return string.Format("[{0:00000}] {1}", Time.frameCount, s);
        }

        public static string WithObjectName(this string s, Object o)
        {
            return string.Format("[{0}] {1}", o.name, s);
        }

        public static string WithTimestamp(this string s)
        {
            return string.Format("[{0:000.000}] {1}", Time.realtimeSinceStartup, s);
        }

        #endregion

        #region Methods

        private static string ToLogString(this string s, Object context)
        {
            return s.WithObjectName(context).WithTimestamp();
        }

        #endregion
    }
}
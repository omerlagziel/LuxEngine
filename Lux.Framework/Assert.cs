using System;
using System.Diagnostics;

namespace Lux.Framework
{
    public static class Assert
    {
        [Conditional("DEBUG")]
        public static void Fail(string message, params object[] args)
        {
            Debug.Assert(false, string.Format(message, args));
            Debugger.Break();
        }

        [Conditional("DEBUG")]
        public static void IsTrue(bool statement, string message, params object[] args)
        {
            if (!statement)
            {
                Fail(message, args);
            }
        }

        [Conditional("DEBUG")]
        public static void IsFalse(bool statement, string message, params object[] args)
        {
            if (statement)
            {
                Fail(message, args);
            }
        }

        [Conditional("DEBUG")]
        public static void IsNotNull(object obj, string message, params object[] args)
        {
            if (obj == null)
            {
                Fail(message, args);
            }
        }
    }
}

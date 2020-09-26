using System;
using System.Collections.Generic;
using System.Text;

namespace UE {
    public static class Extensions {
        public static String Left(this string input, int length) {
            var result = "";
            if ((input.Length <= 0)) return result;
            if ((length > input.Length)) {
                length = input.Length;
            }
            result = input.Substring(0, length);
            return result;
        }

        public static String Mid(this string input, int start, int length) {
            var result = "";
            if (((input.Length <= 0) || (start >= input.Length))) return result;
            if ((start + length > input.Length)) {
                length = (input.Length - start);
            }
            result = input.Substring(start, length);
            return result;
        }

        public static String Right(this string input, int length) {
            var result = "";
            if ((input.Length <= 0)) return result;
            if ((length > input.Length)) {
                length = input.Length;
            }
            result = input.Substring((input.Length - length), length);
            return result;
        }
    }
}

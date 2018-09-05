using System;
using System.Collections.Generic;
using System.Linq;

namespace PrickleParser{
    public class Utils{

        public static bool FloatsNearlyEqual(float a, float b, float epsilon){
            float absA = Math.Abs(a);
            float absB = Math.Abs(b);
            float diff = Math.Abs(a - b);

            if (a == b) { // shortcut, handles infinities
                return true;
            } 
            if (a == 0 || b == 0 || diff < float.MinValue) {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < (epsilon * float.MinValue);
            } 
            return diff / (absA + absB) < epsilon;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace FirstPersonGameEngine.Systems
{
    public class Pair<Ta, Tb>
    {
        public Ta First;
        public Tb Second;

        public Pair(Ta first, Tb second)
        {
            First = first;
            Second = second;
        }
    }
}
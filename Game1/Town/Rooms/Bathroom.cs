﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Game1.Rooms
{
    /// <summary>
    /// Child Bathroom class inherits from parent Room class. 
    /// </summary>
    class Bathroom : Room
    {

        public Bathroom(string argId, int[] argLocation, string argRoomClass) : base(argId, argLocation, argRoomClass)
        {

        }


    }
}

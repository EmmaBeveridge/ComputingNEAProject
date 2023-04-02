using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game1
{
    public class GameStates
    {

        /// <summary>
        /// Enumeration to store possible game states.
        /// </summary>
        public enum States
        {

            MainMenu,
            CharacterSelection,
            TraitSelection,
            Loading,
            Playing,
            Paused

        }


    }
}

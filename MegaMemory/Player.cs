
// Player 1.0, by Cliff Earl, Antix Development, April 2019

using System;
using System.Collections.Generic;

namespace MegaMemory
{
    class Player
    {
        public String Name;
        public int Score;

        public int PlayerType; // type of player (human, easy ai, average ai, hard ai)
        public double Intelligence; // percentage chance the player will remember a match or pair

        public Card CardA, CardB; // cards this player has overturned

        public TextField HUD;

        public List<Card> Memory;

        /// <summary>
        /// Create new Player
        /// </summary>
        public Player()
        {
            PlayerType = 0; // human by default
            Intelligence = 0;

            Name = "Annon";
            Score = 0;

            Memory = new List<Card>();
        }
        /// <summary>
        /// Remember this card
        /// </summary>
        /// <param name="card"></param>
        public void remember(Card card)
        {
            if (!Memory.Contains(card))
            {
                Memory.Add(card);
            }
        }

        /// <summary>
        /// Forget this card
        /// </summary>
        /// <param name="card"></param>
        public void forget(Card card)
        {
            if (Memory.Contains(card))
            {
                Memory.Remove(card);
            }
        }

        /// <summary>
        /// Check if memory contains a pair
        /// </summary>
        /// <param name="intelligence"></param>
        /// <returns></returns>
        public bool recallPair(double chance)
        {
            if (chance < Intelligence) // check if player remembers a pair
            {
                for (int i = Memory.Count - 1; i >= 1; i--) // outer compare loop
                {
                    for (int j = i - 1; j >= 0; j--) // inner compare loop
                    {
                        Card cardA = Memory[i]; // get 2 cards to compare
                        Card cardB = Memory[j];
                        if (cardA.FaceValue == cardB.FaceValue) // cards have same face values?
                        {
                            CardA = cardA; // set cards
                            CardB = cardB;
                            return true; // we will return "I remembered a pair" state
                        }
                    }
                }
            }
            return false; // player did not remember
        }

        /// <summary>
        /// Check if memory contains a match for this card
        /// </summary>
        /// <param name="cardA"></param>
        /// <param name="intelligence"></param>
        /// <returns></returns>
        public bool recallMatch(Card cardA, double chance)
        {
            if (chance < Intelligence) // check if the player remembers a match
            {
                foreach (Card cardB in Memory)
                {
                    if ((cardB != cardA) && (cardB.FaceValue == cardA.FaceValue))
                    {
                        CardB = cardB;
                        return true;
                    }
                }
            }
            return false; // failed intelligence test
        }

        /// <summary>
        /// Reset player
        /// </summary>
        public void reset()
        {
            Score = 0;
            Memory.Clear();
        }
    }
}

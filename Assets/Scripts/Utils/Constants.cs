using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Constants
{
    public static class Scenes
    {
        public const string MainMenu = "MainMenu";
        public const string PauseMenu = "PauseMenu";
        public const string UI = "UI";
        public const string GameWon = "GameWon";
        public const string ESN = "ESN";
    }

    public static class EndGameTexts
    {
        public const string VictoryText = "You were able to find the Vaccine and save your population.\nCongratulations!";
        public const string SupportLossText = "You weren't able to keep the population on your side and they ended up kicking you out of power.\nSHAME!";
        public const string MoneyLossText = "You weren't able to keep your finances under control and now there's nothing you can do to stop everyone from dying.\nSHAME!";
    }
}

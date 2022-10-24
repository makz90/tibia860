﻿using OpenTibia.Common.Objects;

namespace OpenTibia.Game
{
    public static class Constants
    {
        public static readonly string OnlyProtocol86Allowed = "Only protocol 8.6 allowed.";

        public static readonly string AccountNameOrPasswordIsNotCorrect = "Account name or password is not correct.";

        public static readonly string DestinationIsOutOfReach = "Destination is out of reach.";

        public static readonly string SorryNotPossible = "Sorry, not possible.";

        public static readonly string TargetLost = "Target lost.";

        public static readonly string ThereIsNotEnoughtRoom = "There is not enought room.";

        public static readonly string ThereIsNoWay = "There is no way.";

        public static readonly string ThisIsImpossible = "This is impossible.";

        public static readonly string YouCanNotMoveThisObject = "You cannot move this object.";

        public static readonly string YouCannotPutMoreObjectsInThisContainer = "You cannot put more objects in this container.";

        public static readonly string YouCanNotTakeThisObject = "You cannot take this object.";

        public static readonly string YouCanNotThrowThere = "You cannot throw there.";

        public static readonly string YouCanNotUseThere = "You cannot use there.";

        public static readonly string YouCanNotUseThisObject = "You cannot use this object.";

        public static readonly string YouMayNotAttackThisCreature = "You may not attack this creature.";


        public static string PlayerWalkSchedulerEvent(Player player) => "Player_Walk_" + player.Id;

        public static string PlayerAutomationSchedulerEvent(Player player) => "Player_Automation_" + player.Id;

        public static readonly int PlayerAutomationSchedulerEventInterval = 200;

        public static readonly int ObjectsPerPoint = 10;
    }
}
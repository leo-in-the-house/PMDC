﻿using System;
using System.Collections.Generic;
using RogueElements;
using RogueEssence;
using RogueEssence.Dungeon;
using RogueEssence.Data;

namespace PMDO.Dungeon
{
    [Serializable]
    public class AttackFoesPlan : AIPlan
    {
        //continue to the last place the enemy was found (if no other enemies can be found) before losing aggro
        private Loc? prevLoc;

        public AttackFoesPlan() { }
        public AttackFoesPlan(AIFlags iq, AttackChoice attackPattern) : base(iq, attackPattern) { }
        protected AttackFoesPlan(AttackFoesPlan other) : base(other) { }
        public override BasePlan CreateNew() { return new AttackFoesPlan(this); }
        public override void SwitchedIn() { prevLoc = null; base.SwitchedIn(); }

        public override GameAction Think(Character controlledChar, bool preThink, ReRandom rand)
        {
            if (controlledChar.CantWalk)
            {
                GameAction attack = TryAttackChoice(rand, controlledChar, null);
                if (attack.Type != GameAction.ActionType.Wait)
                    return attack;

                return null;
            }

            //path to the closest enemy
            List<Character> seenCharacters = controlledChar.GetSeenCharacters(GetAcceptableTargets());

            bool teamPartner = (IQ & AIFlags.TeamPartner) != AIFlags.None;
            if (teamPartner)
            {
                //check for statuses that may make them ineligible targets
                for (int ii = seenCharacters.Count - 1; ii >= 0; ii--)
                {
                    //NOTE: specialized AI code!
                    if (seenCharacters[ii].GetStatusEffect(1) != null || seenCharacters[ii].GetStatusEffect(3) != null)//if they're asleep or frozen, do not attack
                        seenCharacters.RemoveAt(ii);
                    else if (seenCharacters[ii].GetStatusEffect(25) == null)//last targeted by someone; NOTE: specialized AI code!
                    {
                        //don't attack certain kinds of foes that won't attack first
                        if (seenCharacters[ii].Tactic.ID == 10)//weird tree; NOTE: specialized AI code!
                            seenCharacters.RemoveAt(ii);
                        else if (seenCharacters[ii].Tactic.ID == 8)//wait attack; NOTE: specialized AI code!
                            seenCharacters.RemoveAt(ii);
                        else if (seenCharacters[ii].Tactic.ID == 18)//tit for tat; NOTE: specialized AI code!
                            seenCharacters.RemoveAt(ii);
                    }
                }
            }

            Character closestChar = null;
            List<Loc> closestPath = null;
            //iterate in increasing character indices
            foreach (Character seenChar in seenCharacters)
            {
                //try to path to each
                List<Loc> path = GetPath(controlledChar, seenChar.CharLoc, !preThink);
                //if a path can be found, check against closest character
                bool newGoal = (path[0] == seenChar.CharLoc);
                if (!teamPartner && closestChar == null || newGoal)
                {
                    closestChar = seenChar;
                    closestPath = path;
                }
                else if (closestPath != null)
                {
                    bool oldGoal = (closestPath[0] == closestChar.CharLoc);
                    if (!oldGoal && newGoal)
                    {
                        //between characters of which exist a path, and characters that don't, the ones that do win out.
                        closestChar = seenChar;
                        closestPath = path;
                    }
                    else if ((oldGoal == newGoal) && path.Count < closestPath.Count)
                    {
                        //of all characters which exist a path, the closest one wins
                        closestChar = seenChar;
                        closestPath = path;
                    }
                }
            }

            if (closestChar != null)
            {
                GameAction attack = TryAttackChoice(rand, controlledChar, closestChar);
                if (attack.Type != GameAction.ActionType.Wait)
                    return attack;

                //pursue the enemy if one is located
                prevLoc = closestChar.CharLoc;
                return SelectChoiceFromPath(controlledChar, closestPath, closestPath[0] == closestChar.CharLoc);
            }
            else if (!teamPartner && prevLoc.HasValue && prevLoc.Value != controlledChar.CharLoc)
            {
                //if no enemy is located, path to the location of the last seen enemy
                List<Loc> path = GetPath(controlledChar, prevLoc.Value, !preThink);
                if (path[path.Count - 1] == prevLoc.Value)
                    return SelectChoiceFromPath(controlledChar, path, false);
                else
                    prevLoc = null;
            }

            return null;
        }
    }
}

﻿using System;
using RogueElements;
using RogueEssence.Dungeon;
using RogueEssence;
using RogueEssence.LevelGen;
using PMDC.Dungeon;
using System.Collections.Generic;

namespace PMDC.LevelGen
{
    [Serializable]
    public class BossSealStep<T> : BaseSealStep<T> where T : ListMapGenContext
    {
        public int SealedTile;
        public int BossTile;

        public BossSealStep()
        {
            BossFilters = new List<BaseRoomFilter>();
        }

        public BossSealStep(int sealedTile, int bossTile) : base()
        {
            SealedTile = sealedTile;
            BossTile = bossTile;
            BossFilters = new List<BaseRoomFilter>();
        }

        public List<BaseRoomFilter> BossFilters { get; set; }

        protected override void PlaceBorders(T map, Dictionary<Loc, SealType> sealList)
        {
            Rect? bossRect = null;

            for (int ii = 0; ii < map.RoomPlan.RoomCount; ii++)
            {
                FloorRoomPlan plan = map.RoomPlan.GetRoomPlan(ii);
                if (!BaseRoomFilter.PassesAllFilters(plan, this.BossFilters))
                    continue;
                bossRect = plan.RoomGen.Draw;
                break;
            }

            //if there's no way to open the door, there cannot be a door; give the player the treasure unguarded
            if (bossRect == null)
                return;

            EffectTile bossEffect = null;

            for (int xx = bossRect.Value.Start.X; xx < bossRect.Value.End.X; xx++)
            {
                for (int yy = bossRect.Value.Start.Y; yy < bossRect.Value.End.Y; yy++)
                {
                    if (map.Tiles[xx][yy].Effect.ID == BossTile)
                    {
                        bossEffect = map.Tiles[xx][yy].Effect;
                        break;
                    }
                }
                if (bossEffect != null)
                    break;
            }

            if (bossEffect == null)
                return;


            List<Loc> lockList = new List<Loc>();

            foreach (Loc loc in sealList.Keys)
            {
                switch (sealList[loc])
                {
                    case SealType.Blocked:
                        map.Tiles[loc.X][loc.Y] = (Tile)map.UnbreakableTerrain.Copy();
                        break;
                    default:
                        lockList.Add(loc);
                        break;
                }
            }

            foreach (Loc loc in lockList)
            {
                map.Tiles[loc.X][loc.Y] = (Tile)map.UnbreakableTerrain.Copy();
                EffectTile newEffect = new EffectTile(SealedTile, true, loc);
                ((IPlaceableGenContext<EffectTile>)map).PlaceItem(loc, newEffect);
            }

            ResultEventState resultEvent = new ResultEventState();
            resultEvent.ResultEvents.Add(new OpenVaultEvent(lockList));

            bossEffect.TileStates.Set(resultEvent);
        }

    }
}
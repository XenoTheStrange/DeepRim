using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;
using DeepRim;
using System.Linq;

[HarmonyPatch(typeof(PawnsArrivalModeWorker_EdgeWalkIn), "Arrive")]
public static class PawnsArrivalModeWorker_EdgeWalkIn_Patch
{
    public static bool Prefix(ref List<Pawn> pawns, ref IncidentParms parms)
    {
        Map map = (Map)parms.target;
        var lift = map.listerBuildings.AllBuildingsColonistOfClass<Building_SpawnedLift>().FirstOrDefault();

        if (lift != null){
            DeepRimMod.LogMessage($"Found a lift: {lift}. Checking for valid underground entry point...");

            IEnumerable<IntVec3> validEdgeCells = CellFinder.mapEdgeCells.Where(cell => cell.Walkable(map));
            if (!validEdgeCells.Any()){
                DeepRimMod.LogMessage("Couldn't find a valid underground entrance on this layer, spawning aboveground instead.");
                map = lift.parentDrill.Map;
            }

            IntVec3 cell = CellFinder.mapEdgeCells.Where(cell => cell.Walkable(map)).RandomElement();
            for (int i = 0; i < pawns.Count; i++)
            {
                IntVec3 loc = CellFinder.RandomClosewalkCellNear(cell, map, 8);
                GenSpawn.Spawn(pawns[i], loc, map, parms.spawnRotation);
            }
        return false;
        }

    return true;
    }
}
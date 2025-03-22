using HarmonyLib;
using Photon.Pun;
using UnityEngine;
using System.Collections.Generic;

namespace CartDuplicator.Patches
{
    [HarmonyPatch(typeof(PunManager))]
    [HarmonyPatch("SpawnItem")]
    public class DuplicateCartPatch : MonoBehaviourPunCallbacks
    {
        [HarmonyPostfix]
        public static void After_SpawnItem(Item item, ItemVolume volume)
        {
            if (item == null || volume == null) return;

            if (item.itemAssetName != "Item Cart Medium") return;

            int duplicationAmount = CartDuplicator.GetDuplicationAmount();
            Vector3 duplicationOffset = CartDuplicator.GetDuplicationOffset();
            string cartPrefabName = CartDuplicator.GetSmallCartReplacement() ? "Item Cart Small" : item.prefab.name;

            // Get player count
            int playerCount = GetPlayerCount();

            // Adjust duplication amount based on player count if dynamic spawning is enabled
            if (CartDuplicator.GetDynamicSpawning())
            {
                if (playerCount >= 10)
                {
                    duplicationAmount += 2;
                }
                else if (playerCount >= 5)
                {
                    duplicationAmount += 1;
                }
            }

            // Multiplayer spawning
            if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
            {
                for (int i = 0; i < duplicationAmount; i++)
                {
                    PhotonNetwork.InstantiateRoomObject("Items/" + cartPrefabName,
                        volume.transform.position + duplicationOffset * (i + 1),
                        item.spawnRotationOffset);
                }
            }
            // Singleplayer spawning
            else if (!PhotonNetwork.IsConnected)
            {
                var prefabToSpawn = CartDuplicator.GetSmallCartReplacement()
                    ? Resources.Load<GameObject>("Items/Item Cart Small")
                    : item.prefab;

                for (int i = 0; i < duplicationAmount; i++)
                {
                    UnityEngine.Object.Instantiate(prefabToSpawn,
                        volume.transform.position + duplicationOffset * (i + 1),
                        item.spawnRotationOffset);
                }
            }
        }

        public static List<PlayerAvatar> PlayerGetAll()
        {
            return GameDirector.instance.PlayerList;
        }

        public static int GetPlayerCount()
        {
            return PlayerGetAll().Count;
        }
    }
}

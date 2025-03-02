using HarmonyLib;
using Photon.Pun;
using UnityEngine;

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

            // Multiplayer spawning
            if (PhotonNetwork.IsConnected && PhotonNetwork.IsMasterClient)
            {
                for (int i = 0; i < duplicationAmount; i++)
                {
                    PhotonNetwork.InstantiateRoomObject("Items/" + item.prefab.name,
                        volume.transform.position + duplicationOffset * (i + 1),
                        item.spawnRotationOffset);
                }
            }
            // Singleplayer spawning
            else if (!PhotonNetwork.IsConnected)
            {
                for (int i = 0; i < duplicationAmount; i++)
                {
                    UnityEngine.Object.Instantiate(item.prefab,
                        volume.transform.position + duplicationOffset * (i + 1),
                        item.spawnRotationOffset);
                }
            }
        }

        public override void OnEnable()
        {
            base.OnEnable();
            Debug.Log("Cart Duplicator: Patch enabled");
        }
    }
}

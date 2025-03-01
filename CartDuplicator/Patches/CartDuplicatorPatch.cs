using HarmonyLib;
using Photon.Pun;
using System.Collections;
using UnityEngine;

namespace CartDuplicator.Patches
{
    [HarmonyPatch(typeof(LevelGenerator))]
    class DuplicateCartPatch
    {
        [HarmonyPatch("StartRoomGeneration")]
        [HarmonyPostfix]
        private static void OnStartRoomGeneration(LevelGenerator __instance) =>
            __instance.StartCoroutine(WaitAndDuplicate());

        private static IEnumerator WaitAndDuplicate()
        {
            yield return new WaitForSeconds(CartDuplicator.GetDuplicationDelay());

            var cart = GameObject.Find("Item Cart Medium(Clone)") ?? GameObject.Find("Item Cart Medium");

            if (cart != null)
            {
                var offset = CartDuplicator.GetDuplicationOffset();
                var spawnPosition = cart.transform.position + offset;

                _ = GameManager.instance.gameMode != 0
                    ? PhotonNetwork.InstantiateRoomObject(cart.name.Replace("(Clone)", ""),
                        spawnPosition, cart.transform.rotation, 0)
                    : Object.Instantiate(cart, spawnPosition, cart.transform.rotation);
            }
        }
    }
}

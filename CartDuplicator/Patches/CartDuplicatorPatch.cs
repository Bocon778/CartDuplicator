using HarmonyLib;
using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;

namespace CartDuplicator.Patches
{
    [HarmonyPatch(typeof(LevelGenerator))]
    class DuplicateCartPatch : MonoBehaviourPun
    {
        [HarmonyPatch("StartRoomGeneration")]
        [HarmonyPostfix]
        private static void OnStartRoomGeneration(LevelGenerator __instance) =>
            __instance.StartCoroutine(WaitAndDuplicate());

        private static IEnumerator WaitAndDuplicate()
        {
            yield return new WaitForSeconds(CartDuplicator.GetDuplicationDelay());

            var cart = FindCart();
            if (cart == null)
            {
                yield break;
            }

            var offset = CartDuplicator.GetDuplicationOffset();
            var spawnPosition = cart.transform.position + offset;

            if (GameManager.instance.gameMode != 0)
            {
                InstantiateCartMultiplayer(cart, spawnPosition);
            }
            else
            {
                InstantiateCartSingleplayer(cart, spawnPosition);
            }
        }

        private static GameObject FindCart()
        {
            return GameObject.Find("Item Cart Medium(Clone)") ?? GameObject.Find("Item Cart Medium");
        }

        private static void InstantiateCartSingleplayer(GameObject cart, Vector3 spawnPosition)
        {
            try
            {
                UnityEngine.Object.Instantiate(cart, spawnPosition, cart.transform.rotation);
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to instantiate cart in singleplayer mode: {ex.Message}");
            }
        }

        private static void InstantiateCartMultiplayer(GameObject cart, Vector3 spawnPosition)
        {
            try
            {
                var newCart = UnityEngine.Object.Instantiate(cart, spawnPosition, cart.transform.rotation);

                PhotonView photonView = newCart.GetComponent<PhotonView>() ?? newCart.AddComponent<PhotonView>();
                if (PhotonNetwork.AllocateViewID(photonView))
                {
                    photonView.RPC("RPC_SynchronizeCart", RpcTarget.OthersBuffered, photonView.ViewID, spawnPosition, cart.transform.rotation);
                }
                else
                {
                    UnityEngine.Object.Destroy(newCart);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to instantiate cart in multiplayer mode: {ex.Message}");
            }
        }

        [PunRPC]
        private void RPC_SynchronizeCart(int viewID, Vector3 position, Quaternion rotation)
        {
            var cart = FindCart();
            if (cart != null)
            {
                var newCart = UnityEngine.Object.Instantiate(cart, position, rotation);
                PhotonView photonView = newCart.GetComponent<PhotonView>() ?? newCart.AddComponent<PhotonView>();
                photonView.ViewID = viewID;
            }
        }
    }
}




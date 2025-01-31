/* This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. */

using System;
using System.Linq;
using Cgs.CardGameView.Multiplayer;
using Mirror;
using UnityEngine;
using UnityExtensionMethods;

namespace Cgs.Play.Multiplayer
{
    [RequireComponent(typeof(CgsNetDiscovery))]
    public class CgsNetManager : NetworkManager
    {
        public static CgsNetManager Instance => (CgsNetManager) singleton;

        public string GameName { get; set; } = "";

        public CgsNetPlayer LocalPlayer { get; set; }

        public CgsNetDiscovery Discovery { get; private set; }

        public LRMDirectConnectModule lanConnector;

        public PlayController playController;

        private Guid _cardStackAssetId;
        private Guid _cardAssetId;
        private Guid _dieAssetId;

        public override void Start()
        {
            base.Start();

            _cardStackAssetId = playController.cardStackPrefab.GetComponent<NetworkIdentity>().assetId;
            NetworkClient.RegisterSpawnHandler(_cardStackAssetId, SpawnStack, UnSpawn);

            _cardAssetId = playController.cardModelPrefab.GetComponent<NetworkIdentity>().assetId;
            NetworkClient.RegisterSpawnHandler(_cardAssetId, SpawnCard, UnSpawnCard);

            _dieAssetId = playController.diePrefab.GetComponent<NetworkIdentity>().assetId;
            NetworkClient.RegisterSpawnHandler(_dieAssetId, SpawnDie, UnSpawn);

            Discovery = GetComponent<CgsNetDiscovery>();
            Debug.Log("[CgsNet Manager] Acquired NetworkDiscovery.");
        }

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);
            Debug.Log("[CgsNet Manager] Server adding player...");
        }

        public override void OnClientConnect(NetworkConnection connection)
        {
            base.OnClientConnect(connection);
            Debug.Log("[CgsNet Manager] Client connected!");
        }

        private GameObject SpawnStack(Vector3 position, Guid assetId)
        {
            Debug.Log("[CgsNet Manager] SpawnStack");
            Transform target = playController.playArea.transform;
            var cardStack = Instantiate(playController.cardStackPrefab, target.parent).GetComponent<CardStack>();
            cardStack.transform.SetParent(target);
            return cardStack.gameObject;
        }

        private GameObject SpawnCard(Vector3 position, Guid assetId)
        {
            Debug.Log("[CgsNet Manager] SpawnCard");
            GameObject newCard =
                Instantiate(playController.cardModelPrefab, playController.playArea.transform);
            PlayController.SetPlayActions(newCard.GetComponent<CardModel>());
            return newCard;
        }

        private GameObject SpawnDie(Vector3 position, Guid assetId)
        {
            Debug.Log("[CgsNet Manager] SpawnDie");
            Transform target = playController.playArea.transform;
            var die = Instantiate(playController.diePrefab, target.parent).GetOrAddComponent<Die>();
            die.transform.SetParent(target);
            return die.gameObject;
        }

        private static void UnSpawn(GameObject spawned)
        {
            Debug.Log("[CgsNet Manager] UnSpawn");
            Destroy(spawned);
        }

        private static void UnSpawnCard(GameObject spawnedCard)
        {
            Debug.Log("[CgsNet Manager] UnSpawnCard");
            var cardModel = spawnedCard.GetComponent<CardModel>();
            if (!cardModel.PointerDragOffsets.Any())
                Destroy(spawnedCard);
            else
                Debug.Log("[CgsNet Manager] Ignore UnSpawn - Active Card");
        }

        public void Restart()
        {
            foreach (CardStack cardStack in playController.playArea.GetComponentsInChildren<CardStack>())
                NetworkServer.UnSpawn(cardStack.gameObject);
            foreach (CardModel cardModel in playController.playArea.GetComponentsInChildren<CardModel>())
                NetworkServer.UnSpawn(cardModel.gameObject);
            foreach (Die die in playController.playArea.GetComponentsInChildren<Die>())
                NetworkServer.UnSpawn(die.gameObject);
            foreach (CgsNetPlayer player in FindObjectsOfType<CgsNetPlayer>())
                player.TargetRestart();
        }

        private void OnDisable()
        {
            NetworkClient.ClearSpawners();
        }
    }
}

﻿using System.Collections.Generic;
using System.Linq;
using Core.Scripts.Items;
using DG.Tweening;
using Lean.Pool;
using UnityEngine;

namespace Core.Scripts.Builds
{
    public class FactoryBuild : Build
    {
        [SerializeField] private Material _conveerMaterial;
        [SerializeField] private SphereCollider[] _collider;

        [SerializeField] private Transform _pointDestroyItem;

        private int _speed;

        public List<Item> itemsSword;
        public Transform[] startPoints;

        private void Update()
        {
            if (items.Count > 0)
            {
                Vector2 offset = new Vector2(_speed * Time.fixedDeltaTime, 0);
                _conveerMaterial.mainTextureOffset -= offset;
            }
            else
            {
                _speed = 0;
            }
        }

        public void OnMoveItemsToFactory()
        {
            _speed = 1;
            for (int i = items.Count - 1; i >= 0; i--)
            {
                items[i]._itemMoveType = ItemMoveType.None;
                items[i].transform.DOMove(_pointDestroyItem.position, _time * (items.Count - i)).OnComplete(() =>
                {
                    var last = items.Last();
                    LeanPool.Despawn(last);
                    items.Remove(last);

                    var sword = LeanPool.Spawn(_prefab, _pointDestroyItem.position, _prefab.transform.rotation, _parent)
                        .GetComponent<Item>();
                    sword._itemMoveType = ItemMoveType.None;
                    sword.transform.DOMove(finishPoint.position, 1).OnComplete(() =>
                        {
                            var lastPoint = itemsSword.Count == 0 ? finishPoint : itemsSword.Last().transform;
                            sword.SetPointMove(null, lastPoint);
                            sword._itemMoveType = ItemMoveType.Follow;
                            itemsSword.Add(sword);
                        })
                        .SetEase(Ease.Linear);
                }).SetEase(Ease.Linear);
            }
        }

        public FactoryColliderType ColliderСheck(Collider collider) =>
            collider == _collider[0] ? FactoryColliderType.Set : FactoryColliderType.Get;
    }
}
using System;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DefaultNamespace
{
    public class ESCSpawner : MonoBehaviour
    {
        // [SerializeField] private Entity beePrefab;
        [SerializeField] private int initialFoodCount = 10;
        [SerializeField] private int beeCount = 10;
        [SerializeField] private GameObject beePrefab;
        [SerializeField] private Material[] beeMaterials;
        [SerializeField] private GameObject foodPrefab;

        private Entity beeEntityPrefab;
        private Entity foodEntityPrefab;
        private EntityManager manager;
        private void Start()
        {
            
            World world = World.DefaultGameObjectInjectionWorld;
            manager = world.EntityManager;
            GameObjectConversionSettings conversionSettings = new GameObjectConversionSettings(world, GameObjectConversionUtility.ConversionFlags.AssignName);
            beeEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(beePrefab, conversionSettings);
            foodEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(foodPrefab, conversionSettings);

            SpawnTeam(0, new float3(-50, 0, 0));
            SpawnTeam(1, new float3( 50, 0, 0));

            SpawnFood();

        }

        private void SpawnTeam(byte teamId, float3 teamStartPosition)
        {
            for (int i = 0; i < beeCount; i++)
            {
                var entity = manager.Instantiate(beeEntityPrefab);
                
                var translation = new Translation();
                translation.Value = teamStartPosition + (float3) Random.insideUnitSphere * 10.0f;
                manager.SetComponentData(entity, translation);
                
                var team = new TeamComponent();
                team.Value = teamId;
                manager.SetComponentData(entity, team);

                var renderMesh = manager.GetSharedComponentData<RenderMesh>(entity);
                renderMesh.material = beeMaterials[teamId];
                manager.SetSharedComponentData(entity, renderMesh);
            }
        }

        private void SpawnFood()
        {
            for (int i = 0; i < initialFoodCount; i++)
            {
                Entity prefabInstance = manager.Instantiate(foodEntityPrefab);
                Translation instancePosition = new Translation();
                Vector3 position = Random.insideUnitSphere * 10.0f;
                instancePosition.Value = position;
                manager.SetComponentData(prefabInstance, instancePosition);
            }
        }
    }
}
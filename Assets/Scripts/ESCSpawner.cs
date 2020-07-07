using System;
using Unity.Entities;
using Unity.Mathematics;
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
                Entity prefabInstance = manager.Instantiate(beeEntityPrefab);
                
                Translation instancePosition = new Translation();
                Vector3 beeStartPosition = Random.insideUnitSphere * 10.0f;
                beeStartPosition.x += teamStartPosition.x;
                beeStartPosition.y += teamStartPosition.y;
                beeStartPosition.z += teamStartPosition.z;
                instancePosition.Value = beeStartPosition;
                manager.SetComponentData(prefabInstance, instancePosition);
                
                var team = new TeamComponent();
                team.Value = teamId;
                manager.SetComponentData(prefabInstance, team);
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
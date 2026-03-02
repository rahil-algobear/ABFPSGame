using UnityEngine;
using System.Collections.Generic;

namespace Game.Level
{
    /// <summary>
    /// Procedurally generates a space station level using primitives.
    /// </summary>
    public class LevelGenerator : MonoBehaviour
    {
        #region Room Settings
        [Header("Room Settings")]
        [SerializeField] private int _numberOfRooms = 8;
        [SerializeField] private Vector3 _roomSize = new Vector3(15f, 4f, 15f);
        [SerializeField] private float _corridorWidth = 3f;
        [SerializeField] private float _corridorLength = 10f;
        #endregion

        #region Materials
        [Header("Materials")]
        [SerializeField] private Material _floorMaterial;
        [SerializeField] private Material _wallMaterial;
        [SerializeField] private Material _ceilingMaterial;
        #endregion

        #region Generation
        private List<Room> _rooms = new List<Room>();
        private GameObject _levelParent;
        #endregion

        #region Room Class
        private class Room
        {
            public Vector3 position;
            public Vector3 size;
            public GameObject roomObject;

            public Room(Vector3 pos, Vector3 sz)
            {
                position = pos;
                size = sz;
            }
        }
        #endregion

        #region Unity Lifecycle
        private void Start()
        {
            GenerateLevel();
        }
        #endregion

        #region Level Generation
        /// <summary>
        /// Generate the entire level.
        /// </summary>
        public void GenerateLevel()
        {
            _levelParent = new GameObject("Level");

            CreateMaterials();
            GenerateRooms();
            ConnectRooms();
            AddLighting();
        }

        /// <summary>
        /// Create default materials if not assigned.
        /// </summary>
        private void CreateMaterials()
        {
            if (_floorMaterial == null)
            {
                _floorMaterial = new Material(Shader.Find("Standard"));
                _floorMaterial.color = new Color(0.3f, 0.3f, 0.35f);
            }

            if (_wallMaterial == null)
            {
                _wallMaterial = new Material(Shader.Find("Standard"));
                _wallMaterial.color = new Color(0.5f, 0.5f, 0.55f);
            }

            if (_ceilingMaterial == null)
            {
                _ceilingMaterial = new Material(Shader.Find("Standard"));
                _ceilingMaterial.color = new Color(0.4f, 0.4f, 0.45f);
            }
        }

        /// <summary>
        /// Generate rooms in a grid pattern.
        /// </summary>
        private void GenerateRooms()
        {
            int roomsPerRow = Mathf.CeilToInt(Mathf.Sqrt(_numberOfRooms));
            float spacing = _roomSize.x + _corridorLength;

            for (int i = 0; i < _numberOfRooms; i++)
            {
                int row = i / roomsPerRow;
                int col = i % roomsPerRow;

                Vector3 position = new Vector3(
                    col * spacing,
                    0f,
                    row * spacing
                );

                Room room = new Room(position, _roomSize);
                room.roomObject = CreateRoom(position, _roomSize);
                _rooms.Add(room);
            }
        }

        /// <summary>
        /// Create a single room.
        /// </summary>
        /// <param name="position">Room position</param>
        /// <param name="size">Room size</param>
        /// <returns>Room GameObject</returns>
        private GameObject CreateRoom(Vector3 position, Vector3 size)
        {
            GameObject room = new GameObject($"Room_{_rooms.Count}");
            room.transform.SetParent(_levelParent.transform);
            room.transform.position = position;

            // Floor
            CreateFloor(room.transform, size);

            // Walls
            CreateWalls(room.transform, size);

            // Ceiling
            CreateCeiling(room.transform, size);

            // Cover objects
            CreateCoverObjects(room.transform, size);

            return room;
        }

        /// <summary>
        /// Create floor for a room.
        /// </summary>
        private void CreateFloor(Transform parent, Vector3 size)
        {
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "Floor";
            floor.transform.SetParent(parent);
            floor.transform.localPosition = Vector3.zero;
            floor.transform.localScale = new Vector3(size.x, 0.2f, size.z);
            floor.GetComponent<Renderer>().material = _floorMaterial;
            floor.layer = LayerMask.NameToLayer("Default");
        }

        /// <summary>
        /// Create walls for a room.
        /// </summary>
        private void CreateWalls(Transform parent, Vector3 size)
        {
            float wallThickness = 0.3f;
            float wallHeight = size.y;

            // North wall
            CreateWall(parent, new Vector3(0, wallHeight / 2f, size.z / 2f),
                      new Vector3(size.x, wallHeight, wallThickness));

            // South wall
            CreateWall(parent, new Vector3(0, wallHeight / 2f, -size.z / 2f),
                      new Vector3(size.x, wallHeight, wallThickness));

            // East wall
            CreateWall(parent, new Vector3(size.x / 2f, wallHeight / 2f, 0),
                      new Vector3(wallThickness, wallHeight, size.z));

            // West wall
            CreateWall(parent, new Vector3(-size.x / 2f, wallHeight / 2f, 0),
                      new Vector3(wallThickness, wallHeight, size.z));
        }

        /// <summary>
        /// Create a single wall.
        /// </summary>
        private void CreateWall(Transform parent, Vector3 position, Vector3 scale)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.name = "Wall";
            wall.transform.SetParent(parent);
            wall.transform.localPosition = position;
            wall.transform.localScale = scale;
            wall.GetComponent<Renderer>().material = _wallMaterial;
        }

        /// <summary>
        /// Create ceiling for a room.
        /// </summary>
        private void CreateCeiling(Transform parent, Vector3 size)
        {
            GameObject ceiling = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ceiling.name = "Ceiling";
            ceiling.transform.SetParent(parent);
            ceiling.transform.localPosition = new Vector3(0, size.y, 0);
            ceiling.transform.localScale = new Vector3(size.x, 0.2f, size.z);
            ceiling.GetComponent<Renderer>().material = _ceilingMaterial;
        }

        /// <summary>
        /// Create cover objects in a room.
        /// </summary>
        private void CreateCoverObjects(Transform parent, Vector3 size)
        {
            int coverCount = Random.Range(2, 5);

            for (int i = 0; i < coverCount; i++)
            {
                Vector3 position = new Vector3(
                    Random.Range(-size.x / 3f, size.x / 3f),
                    1f,
                    Random.Range(-size.z / 3f, size.z / 3f)
                );

                GameObject cover = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cover.name = "Cover";
                cover.transform.SetParent(parent);
                cover.transform.localPosition = position;
                cover.transform.localScale = new Vector3(2f, 2f, 2f);
                cover.GetComponent<Renderer>().material = _wallMaterial;
            }
        }

        /// <summary>
        /// Connect rooms with corridors.
        /// </summary>
        private void ConnectRooms()
        {
            for (int i = 0; i < _rooms.Count - 1; i++)
            {
                CreateCorridor(_rooms[i].position, _rooms[i + 1].position);
            }
        }

        /// <summary>
        /// Create a corridor between two points.
        /// </summary>
        private void CreateCorridor(Vector3 start, Vector3 end)
        {
            GameObject corridor = new GameObject("Corridor");
            corridor.transform.SetParent(_levelParent.transform);

            Vector3 direction = (end - start).normalized;
            float distance = Vector3.Distance(start, end);
            Vector3 midpoint = (start + end) / 2f;

            // Floor
            GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "CorridorFloor";
            floor.transform.SetParent(corridor.transform);
            floor.transform.position = midpoint;
            floor.transform.localScale = new Vector3(_corridorWidth, 0.2f, distance);
            floor.transform.rotation = Quaternion.LookRotation(direction);
            floor.GetComponent<Renderer>().material = _floorMaterial;

            // Walls
            CreateCorridorWalls(corridor.transform, midpoint, direction, distance);
        }

        /// <summary>
        /// Create corridor walls.
        /// </summary>
        private void CreateCorridorWalls(Transform parent, Vector3 position, Vector3 direction, float length)
        {
            float wallHeight = _roomSize.y;
            float wallThickness = 0.3f;

            // Left wall
            GameObject leftWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            leftWall.name = "CorridorWall";
            leftWall.transform.SetParent(parent);
            leftWall.transform.position = position + Quaternion.LookRotation(direction) * Vector3.right * (_corridorWidth / 2f);
            leftWall.transform.localScale = new Vector3(wallThickness, wallHeight, length);
            leftWall.transform.rotation = Quaternion.LookRotation(direction);
            leftWall.GetComponent<Renderer>().material = _wallMaterial;

            // Right wall
            GameObject rightWall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rightWall.name = "CorridorWall";
            rightWall.transform.SetParent(parent);
            rightWall.transform.position = position + Quaternion.LookRotation(direction) * Vector3.left * (_corridorWidth / 2f);
            rightWall.transform.localScale = new Vector3(wallThickness, wallHeight, length);
            rightWall.transform.rotation = Quaternion.LookRotation(direction);
            rightWall.GetComponent<Renderer>().material = _wallMaterial;
        }

        /// <summary>
        /// Add lighting to the level.
        /// </summary>
        private void AddLighting()
        {
            // Directional light
            GameObject dirLight = new GameObject("Directional Light");
            dirLight.transform.SetParent(_levelParent.transform);
            Light light = dirLight.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 0.5f;
            dirLight.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            // Point lights in each room
            foreach (var room in _rooms)
            {
                GameObject pointLight = new GameObject("Room Light");
                pointLight.transform.SetParent(room.roomObject.transform);
                pointLight.transform.localPosition = new Vector3(0, room.size.y - 0.5f, 0);
                Light pLight = pointLight.AddComponent<Light>();
                pLight.type = LightType.Point;
                pLight.range = room.size.x;
                pLight.intensity = 1.5f;
                pLight.color = new Color(0.9f, 0.95f, 1f);
            }
        }
        #endregion
    }
}

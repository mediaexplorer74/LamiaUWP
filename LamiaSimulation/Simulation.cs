using System;
using System.IO;
using System.Reflection;
using System.Text.Json;

namespace LamiaSimulation
{
    public class Simulation : IActionReceiver, ISimulated
    {
        public static Simulation Instance
        {
            get { return _Instance ??= new Simulation(); }
        }

        private static Simulation _Instance;

        private GlobalState globalState;
        private bool started = false;

        internal static string lastID;
        public string LastID => lastID;

        private float saveTimer;
        
        public Simulation()
        {
            globalState = new GlobalState();
            var dataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Consts.FilenameDataDirectory);
            DataType.LoadDataFromJson<ResourceType>(Path.Combine(dataDir, Consts.FilenameDataResources));
            DataType.LoadDataFromJson<PopulationSpeciesType>(
                Path.Combine(dataDir, Consts.FilenameDataPopulationSpecies)
            );
            DataType.LoadDataFromJson<TaskType>(Path.Combine(dataDir, Consts.FilenameDataTasks));
            DataType.LoadDataFromJson<LocationType>(Path.Combine(dataDir, Consts.FilenameLocationTypes));
            DataType.LoadDataFromJson<BuildingType>(Path.Combine(dataDir, Consts.FilenameBuildingTypes));
            saveTimer = Consts.SaveGameTimeInterval;
            LoadGame();
        }

        public void Reset()
        {
            globalState = new GlobalState();
            started = false;
        }

        public void Start()
        {
            if (started)
                return;
            globalState.PerformAction(ClientAction.UnlockPage,  new ClientParameter<string>("population"));
            globalState.PerformAction(ClientAction.AddLocation,  new ClientParameter<string>("origin"));
            globalState.PerformAction(
                ClientAction.AddSettlementAtLocation, new ClientParameter<string>(LastID)
            );
            globalState.PerformAction(
                ClientAction.RenameSettlement, new ClientParameter<string>(LastID),
                new ClientParameter<string>(T._("A Quiet Clearing"))
            );
            globalState.PerformAction(
                ClientAction.AddPopulation,
                new ClientParameter<string>(LastID),
                new ClientParameter<string>("lamia")
            );
            globalState.PerformAction(
                ClientAction.SendMessage, new ClientParameter<string>(
                    T._("A Lamia is a six foot tall snek with arms.")
                ));
            globalState.PerformAction(
                ClientAction.SendMessage, new ClientParameter<string>(
                    T._("There's one hanging out in a clearing on the outskirts of a forest.")
                ));
            globalState.PerformAction(
                ClientAction.SendMessage, new ClientParameter<string>(
                    T._("It's tongue laps at the air lazily. It's probably hungry.")
                ));
            started = true;
        }
        
        public void SaveGame()
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var jsonString = JsonSerializer.Serialize(globalState, options);
            File.WriteAllText(Consts.FilenameSaveFile, jsonString);
        }

        public void LoadGame()
        {
            if (!File.Exists(Consts.FilenameSaveFile))
                return;
            var jsonString = File.ReadAllText(Consts.FilenameSaveFile);
            globalState = JsonSerializer.Deserialize<GlobalState>(jsonString)!;
            globalState.LodadedFromSave();
            started = true;
        }

        // ---------------------------------------------------
        // IActionReceiver
        // ---------------------------------------------------

        public void PerformAction(ClientAction action)
        {
            globalState.PerformAction(action);
        }

        public void PerformAction<T>(ClientAction action, ClientParameter<T> param1)
        {
            globalState.PerformAction(action, param1);
        }

        public void PerformAction<T1, T2>(ClientAction action, ClientParameter<T1> param1, ClientParameter<T2> param2)
        {
            globalState.PerformAction(action, param1, param2);
        }

        public void PerformAction<T1, T2, T3>(ClientAction action, ClientParameter<T1> param1, ClientParameter<T2> param2,
            ClientParameter<T3> param3)
        {
            globalState.PerformAction(action, param1, param2, param3);
        }

        // ---------------------------------------------------
        // IQueryable
        // ---------------------------------------------------

        public T Query<T>(ClientQuery query)
        {
            QueryResult<T> result = null;
            globalState.Query(ref result, query);
            if(result == null)
                throw new ClientQueryException($"Null result when querying {query.ToString()}");
            return result.Value;
        }

        public T Query<T, T1>(ClientQuery query, T1 param1)
        {
            QueryResult<T> result = null;
            var _param1 = new ClientParameter<T1>(param1);
            globalState.Query(ref result, query, _param1);
            if(result == null)
                throw new ClientQueryException($"Null result when querying {query.ToString()}, params: {_param1}");
            return result.Value;
        }

        public T Query<T, T1, T2>(ClientQuery query, T1 param1, T2 param2)
        {
            QueryResult<T> result = null;
            var _param1 = new ClientParameter<T1>(param1);
            var _param2 = new ClientParameter<T2>(param2);
            globalState.Query(ref result, query, _param1, _param2);
            if(result == null)
                throw new ClientQueryException($"Null result when querying {query.ToString()}, params: {_param1}, {_param2}");
            return result.Value;
        }

        public T Query<T, T1, T2, T3>(ClientQuery query, T1 param1, T2 param2, T3 param3)
        {
            QueryResult<T> result = null;
            var _param1 = new ClientParameter<T1>(param1);
            var _param2 = new ClientParameter<T2>(param2);
            var _param3 = new ClientParameter<T3>(param3);
            globalState.Query(ref result, query, _param1, _param2, _param3);
            if(result == null)
                throw new ClientQueryException($"Null result when querying {query.ToString()}, params: {_param1}, {_param2}, {_param3}");
            return result.Value;
        }

        // ---------------------------------------------------
        // ISimulated
        // ---------------------------------------------------
        
        public void Simulate(float deltaTime)
        {
            // Save game every so often
            saveTimer -= deltaTime;
            if (saveTimer <= 0f)
            {
                SaveGame();
                saveTimer = Consts.SaveGameTimeInterval;
            }
            // Simulate 
            globalState.Simulate(deltaTime);
        }
    }
    
}
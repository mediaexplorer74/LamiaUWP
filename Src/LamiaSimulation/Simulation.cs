﻿using System;
using System.IO;
using System.Text.Json;
using System.Runtime.CompilerServices;


[assembly: InternalsVisibleTo("Tests")]
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
        private float saveTimer;

        // The last created SimulationObject sets this to its ID in it's constructor
        internal static string lastID;
        public string LastID => lastID;
        public SimulationEvents events; 
        
        public Simulation()
        {
            events = new SimulationEvents();
            var dataDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Consts.FilenameDataDirectory);
            DataType.LoadDataFromJson<ResourceType>(Path.Combine(dataDir, Consts.FilenameDataResources));
            DataType.LoadDataFromJson<PopulationSpeciesType>(
                Path.Combine(dataDir, Consts.FilenameDataPopulationSpecies)
            );
            DataType.LoadDataFromJson<TaskType>(Path.Combine(dataDir, Consts.FilenameDataTasks));
            DataType.LoadDataFromJson<LocationType>(Path.Combine(dataDir, Consts.FilenameLocationTypes));
            DataType.LoadDataFromJson<BuildingType>(Path.Combine(dataDir, Consts.FilenameBuildingTypes));
            DataType.LoadDataFromJson<ResearchType>(Path.Combine(dataDir, Consts.FilenameResearchTypes));
            DataType.LoadDataFromJson<ResourceCategory>(Path.Combine(dataDir, Consts.FilenameResourceCategories));
            DataType.LoadDataFromJson<UpgradeType>(Path.Combine(dataDir, Consts.FilenameUpgrades));
            globalState = new GlobalState();
            saveTimer = Consts.SaveGameTimeInterval;
        }

        public void Reset()
        {
            File.Delete(Consts.FilenameSaveFile);
            events.DisconnectAllEventHandlers();
            globalState = new GlobalState();
            globalState.Init();
            started = false;
        }

        public void Start()
        {
            if (started)
                return;
            globalState.Init();
            PerformAction(ClientAction.UnlockPage, Consts.Pages.Population);
            PerformAction(ClientAction.UnlockTask, "idle");
            PerformAction(ClientAction.UnlockTask, "forage");
            PerformAction(ClientAction.AddLocation, "origin");
            PerformAction(ClientAction.AddSettlementAtLocation, LastID);
            PerformAction(ClientAction.RenameSettlement, lastID, T._("A Quiet Clearing"));
            PerformAction(ClientAction.AddPopulation, LastID, "lamia");
            foreach (var msg in Consts.InitialMessages)
                PerformAction(ClientAction.SendMessage, T._(msg));
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
            LoadedFromSave();
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

        public void PerformAction<T1>(ClientAction action, T1 param1)
        {
            var _param1 = new ClientParameter<T1>(param1);
            PerformAction(action, _param1);
        }

        public void PerformAction<T1, T2>(ClientAction action, T1 param1, T2 param2)
        {
            var _param1 = new ClientParameter<T1>(param1);
            var _param2 = new ClientParameter<T2>(param2);
            PerformAction(action, _param1, _param2);
        }
        
        public void PerformAction<T1, T2, T3>(ClientAction action, T1 param1, T2 param2, T3 param3)
        {
            var _param1 = new ClientParameter<T1>(param1);
            var _param2 = new ClientParameter<T2>(param2);
            var _param3 = new ClientParameter<T3>(param3);
            PerformAction(action, _param1, _param2, _param3);
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

        public void LoadedFromSave()
        {
            globalState.LoadedFromSave();
        }
       
    }
    
}
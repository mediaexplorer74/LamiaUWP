using System;
using System.Collections.Generic;
using System.Linq;

namespace LamiaSimulation
{

    using Text = T;

    [Serializable]
    internal class GlobalState: IActionReceiver, IQueryable, ISimulated
    {
        private List<Location> locations;
        private List<Settlement> playerSettlements;
        private List<string> unreadMessages;

        public GlobalState()
        {
            locations = new List<Location>();
            playerSettlements = new List<Settlement>();
            unreadMessages = new List<string>();
        }

        // ---------------------------------------------------
        // IActionReceiver
        // ---------------------------------------------------

        public void PerformAction(ClientAction action)
        {
            try
            {
                foreach(var location in locations)
                    location.PerformAction(action);
                foreach(var settlement in playerSettlements)
                    settlement.PerformAction(action);
            }
            catch(ClientActionException e)
            {
                unreadMessages.Add(e.Message);
            }
        }

        public void PerformAction<T>(ClientAction action, ClientParameter<T> param1)
        {
            switch(action)
            {
                // Send message to client
                case ClientAction.SendMessage:
                    unreadMessages.Add(param1.Get as string);
                    break;
                // Add new location to world
                case ClientAction.AddLocation:
                    locations.Add(new Location(param1.Get as string));
                    break;
                // Convert location to player settlement
                case ClientAction.ConvertLocationToSettlement:
                    var location = locations.Filter(
                        l => l.ID == param1.Get.ToString()
                    ).FirstOrDefault();
                    if(location == null)
                        throw new ClientActionException(Text._("Location to convert does not exist."));
                    var newSettlement = location.CreateSettlementAt(Text._("Unnamed"));
                    locations.Remove(location);
                    playerSettlements.Add(newSettlement);
                    break;
            }

            try
            {
                foreach(var location in locations)
                    location.PerformAction(action, param1);
                foreach(var settlement in playerSettlements)
                    settlement.PerformAction(action, param1);
            }
            catch(ClientActionException e)
            {
                unreadMessages.Add(e.Message);
            }
        }

        public void PerformAction<T1, T2>(ClientAction action, ClientParameter<T1> param1, ClientParameter<T2> param2)
        {
            try
            {
                foreach(var location in locations)
                    location.PerformAction(action, param1, param2);
                foreach(var settlement in playerSettlements)
                    settlement.PerformAction(action, param1, param2);
            }
            catch(ClientActionException e)
            {
                unreadMessages.Add(e.Message);
            }
        }

        public void PerformAction<T1, T2, T3>(ClientAction action, ClientParameter<T1> param1, ClientParameter<T2> param2,
            ClientParameter<T3> param3)
        {
            try
            {
                foreach(var location in locations)
                    location.PerformAction(action, param1, param2, param3);
                foreach(var settlement in playerSettlements)
                    settlement.PerformAction(action, param1, param2, param3);
            }
            catch(ClientActionException e)
            {
                unreadMessages.Add(e.Message);
            }
        }

        // ---------------------------------------------------
        // IQueryable
        // ---------------------------------------------------

        public void Query<T>(ref QueryResult<T> result, ClientQuery query)
        {
            switch(query)
            {
                // Available game pages
                case ClientQuery.AvailablePages:
                    result = new QueryResult<(string, string)[]>(AvailablePages.ToArray()) as QueryResult<T>;
                    break;
                // Unread messages
                case ClientQuery.UnreadMessages:
                    result = new QueryResult<string[]>(unreadMessages.ToArray()) as QueryResult<T>;
                    unreadMessages.Clear();
                    break;
                // Settlement IDs
                case ClientQuery.Settlements:
                    result = new QueryResult<string[]>(playerSettlements.Map(s => s.ID).ToArray()) as QueryResult<T>;
                    break;
            }

            foreach(var location in locations)
                location.Query(ref result, query);
            foreach(var settlement in playerSettlements)
                settlement.Query(ref result, query);
        }

        public void Query<T, T1>(ref QueryResult<T> result, ClientQuery query, ClientParameter<T1> param1)
        {
            switch(query)
            {
                // Species name
                case ClientQuery.SpeciesName:
                    result = new QueryResult<string>(Text._(Helpers.GetSpeciesTypeById(param1.Get as string).name)) as QueryResult<T>;
                    break;
                // Species description
                case ClientQuery.SpeciesDescription:
                    result = new QueryResult<string[]>(Text._(Helpers.GetSpeciesTypeById(param1.Get as string).description)) as QueryResult<T>;
                    break;
            }

            foreach(var location in locations)
                location.Query(ref result, query, param1);
            foreach(var settlement in playerSettlements)
                settlement.Query(ref result, query, param1);
        }

        public void Query<T, T1, T2>(ref QueryResult<T> result, ClientQuery query,
            ClientParameter<T1> param1, ClientParameter<T2> param2)
        {
            foreach(var location in locations)
                location.Query(ref result, query, param1, param2);
            foreach(var settlement in playerSettlements)
                settlement.Query(ref result, query, param1, param2);
        }

        public void Query<T, T1, T2, T3>(ref QueryResult<T> result, ClientQuery query,
            ClientParameter<T1> param1, ClientParameter<T2> param2, ClientParameter<T3> param3)
        {
            foreach(var location in locations)
                location.Query(ref result, query, param3);
            foreach(var settlement in playerSettlements)
                settlement.Query(ref result, query, param1, param2, param3);
        }

        // ---------------------------------------------------
        // ISimulated
        // ---------------------------------------------------

        public void Simulate(float deltaTime)
        {
            locations.Apply(location => location.Simulate(deltaTime));
            playerSettlements.Apply(settlement => settlement.Simulate(deltaTime));
        }

        // ---------------------------------------------------
        // Action behaviours
        // ---------------------------------------------------

        // ....

        // ---------------------------------------------------
        // Query behaviours
        // ---------------------------------------------------

        private List<(string, string)> AvailablePages
        {
            get
            {
                var pages = new List<(string, string)>
                {
                    (Consts.Pages.Population, T._("Population"))
                };
                return pages;
            }
        }
    }
}
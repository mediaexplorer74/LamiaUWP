using System;


namespace LamiaSimulation
{
    public class BuildingPurchasedEventArgs : EventArgs
    {
        public string SettlementUuid { get; set; }
        public string BuildingId { get; set; }
    }

    public class SettlementHasNewResourceEventArgs : EventArgs
    {
        public string SettlementUuid { get; set; }
        public string ResourceId { get; set; }
    }
    
    public class SettlementSpawnedNewPopulationEventArgs : EventArgs
    {
        public string SettlementUuid { get; set; }
        public string LocationUuid { get; set; }
        public string SpeciesId { get; set; }
        public string PopulationName { get; set; }
    }
    
    public class UnlockedPageEventArgs : EventArgs
    {
        public string PageId { get; set; }
    }

    public class SimulationEvents
    {
        public event EventHandler<BuildingPurchasedEventArgs> BuildingPurchasedEvent; 
        public event EventHandler<SettlementHasNewResourceEventArgs> SettlementHasNewResourceEvent; 
        public event EventHandler<SettlementSpawnedNewPopulationEventArgs> SettlementSpawnedNewPopulationEvent; 
        public event EventHandler<UnlockedPageEventArgs> UnlockedPageEvent; 

        public void OnBuildingPurchased(BuildingPurchasedEventArgs e)
        {
            BuildingPurchasedEvent?.Invoke(this, e);
        }
        
        public void OnSettlementHasNewResource(SettlementHasNewResourceEventArgs e)
        {
            SettlementHasNewResourceEvent?.Invoke(this, e);
        }

        public void OnSettlementSpawnedNewPopulationEvent(SettlementSpawnedNewPopulationEventArgs e)
        {
            SettlementSpawnedNewPopulationEvent?.Invoke(this, e);
        }
        
        public void OnUnlockedPage(UnlockedPageEventArgs e)
        {
            UnlockedPageEvent?.Invoke(this, e);
        }
        
    }
}
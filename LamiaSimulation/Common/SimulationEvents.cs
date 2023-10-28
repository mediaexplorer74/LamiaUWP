using System;


namespace LamiaSimulation
{
    public class SettlementBuildingPurchasedEventArgs : EventArgs
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
        public event EventHandler<SettlementBuildingPurchasedEventArgs> SettlementBuildingPurchasedEvent; 
        public event EventHandler<SettlementHasNewResourceEventArgs> SettlementHasNewResourceEvent; 
        public event EventHandler<SettlementSpawnedNewPopulationEventArgs> SettlementSpawnedNewPopulationEvent; 
        public event EventHandler<UnlockedPageEventArgs> UnlockedPageEvent; 

        public void OnBuildingPurchased(SettlementBuildingPurchasedEventArgs e)
        {
            SettlementBuildingPurchasedEvent?.Invoke(this, e);
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
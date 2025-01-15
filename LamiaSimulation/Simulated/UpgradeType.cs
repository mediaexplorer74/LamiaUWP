using System;
using System.Collections.Generic;

namespace LamiaSimulation
{
    public enum UpgradePrerequisiteMethod
    {
        RESEARCH_UNLOCKED,
        UPGRADE_UNLOCKED,
        HAS_BUILDING,
        HAS_RESOURCE,
    }
    
    public class UpgradePrerequisite
    {
        public UpgradePrerequisiteMethod method;
        public string id;
    }

    public enum UpgradeBehaviourMethod
    {
        TASK_SPEED_ADJUST,
        TASK_EXTRACT_AMOUNT_ADJUST,
    }
    
    public class UpgradeBehaviour
    {
        public UpgradeBehaviourMethod method;
        public string id;
        public float value;
    }
    
    public class UpgradeType: DataType
    {
        public string name;
        public string description;
        public Dictionary<string, float> cost;
        public List<UpgradePrerequisite> prerequisites;
        public List<UpgradeBehaviour> behaviour;
        public string unlockMessage;
    }
}
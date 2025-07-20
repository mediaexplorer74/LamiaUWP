using System.Linq;
using NUnit.Framework;
using LamiaSimulation;
using NUnit.Framework.Internal;

namespace Tests
{
    public class LocationTest : BaseTest
    {
        [Test]
        public void TestResources()
        {
            simulation.PerformAction(ClientAction.AddLocation, "forest");
            var locationUuid = simulation.LastID;
            var forest = Helpers.GetLocationTypeById("forest");
            Assert.AreEqual(
                forest.resources.Keys.ToArray(),
                simulation.Query<string[], string>(ClientQuery.LocationResources, locationUuid)
            );
            foreach (var resource in forest.resources)
            {
                Assert.AreEqual(
                    resource.Value,
                    simulation.Query<float, string, string>(ClientQuery.LocationResourceAmount, locationUuid, resource.Key)
                );
                simulation.PerformAction(ClientAction.SubtractResourceFromLocation, locationUuid, resource.Key, 100f);
                Assert.AreEqual(
                    resource.Value-100f,
                    simulation.Query<float, string, string>(ClientQuery.LocationResourceAmount, locationUuid, resource.Key)
                );
            }
        }
    }
}
extends PanelContainer

@onready var population_total_label = %PopulationTotal
@onready var game_controller = $/root/GameController

func _process(delta):
    var poplationNum = Query.SettlementCurrentPopulation(game_controller.currentSettlementUuid)
    var poplationMax = Query.SettlementPopulationMax(game_controller.currentSettlementUuid)
    population_total_label.text = "Total " + str(poplationNum) + "/" + str(poplationMax)

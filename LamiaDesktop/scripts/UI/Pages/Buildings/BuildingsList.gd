extends VBoxContainer

@onready var building_template = preload(
    "res://scenes/UI/Pages/Buildings/building.tscn"
    )
@onready var game_controller = $/root/GameController
@onready var buildings_container = $BuildingsContainer

var current_buildings_list = []

func _process(_delta):
    var buildings_list = Array(Query.SettlementBuildings(game_controller.currentSettlementUuid))
    
    if buildings_list == current_buildings_list:
        return
    current_buildings_list = buildings_list
    
    for n in buildings_container.get_children():
        buildings_container.remove_child(n)
        n.queue_free()
    for building_id in buildings_list:
        var new_building = building_template.instantiate() as Building
        buildings_container.add_child(new_building)
        new_building.building_id = building_id
    

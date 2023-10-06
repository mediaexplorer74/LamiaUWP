extends PanelContainer

@onready var demographic_template = preload(
    "res://scenes/UI/Pages/Population/demographic.tscn"
    )
@onready var game_controller = $/root/GameController
@onready var demographic_container = $DemographicContainer

var current_species_list = []

func _process(_delta):
    var species_list = Array(Query.SettlementPopulationSpecies(game_controller.currentSettlementUuid))
    
    if species_list == current_species_list:
        return
    current_species_list = species_list
    
    for n in demographic_container.get_children():
        demographic_container.remove_child(n)
        n.queue_free()
    for species_name in species_list:
        var new_demographic = demographic_template.instantiate() as Demographic
        demographic_container.add_child(new_demographic)
        new_demographic.species_name = species_name
    

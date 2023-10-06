extends PanelContainer
class_name Demographic

@onready var icon = %Icon
@onready var name_text = %Name
@onready var amount_text = %Amount
@onready var info_button = %Info
@onready var game_controller = $/root/GameController

@export var species_name = "thing"


func _process(_delta):
    if not species_name:
        return
    name_text.text = Query.SpeciesName(species_name)
    amount_text.text = str(
        len(
            Query.SettlementPopulationSpeciesMembers(
                game_controller.currentSettlementUuid, species_name
            )
        )
    )
    find_child("Info").get_node("TooltipShower").tooltip_label_text = "\n".join(Query.SpeciesDescription(species_name))

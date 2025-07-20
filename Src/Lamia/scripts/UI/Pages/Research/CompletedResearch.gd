extends PanelContainer
class_name CompletedResearch

@onready var name_text = %Name
@onready var info_button = %Info

@export var research_id = "thing"

func _process(_delta):
    if not research_id:
        return
    name_text.text = Query.ResearchDisplayName(research_id)
    find_child("Info").get_node("TooltipShower").tooltip_label_text = Query.ResearchDescription(research_id)

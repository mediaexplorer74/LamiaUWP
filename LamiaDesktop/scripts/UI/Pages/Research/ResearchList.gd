extends VBoxContainer

@onready var research_template = preload(
    "res://scenes/UI/Pages/Research/research.tscn"
    )
@onready var available_research_container = %AvailableResearchListContainer

var current_research_list = []

func _ready():
    for n in available_research_container.get_children():
        available_research_container.remove_child(n)
        n.queue_free()

func _process(_delta):
    var research_list = Array(Query.ResearchAvailable())
    
    if research_list == current_research_list:
        return
    current_research_list = research_list
    
    for n in available_research_container.get_children():
        available_research_container.remove_child(n)
        n.queue_free()
    for research_id in research_list:
        var new_research = research_template.instantiate()
        available_research_container.add_child(new_research)
        new_research.research_id = research_id
    

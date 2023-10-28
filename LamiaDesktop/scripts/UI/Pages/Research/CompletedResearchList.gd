extends VBoxContainer

@onready var completed_research_template = preload(
    "res://scenes/UI/Pages/Research/completed_research.tscn"
    )
@onready var completed_research_container = %CompletedResearchListContainer

var current_research_list = []

func _ready():
    for n in completed_research_container.get_children():
        completed_research_container.remove_child(n)
        n.queue_free()

func _process(_delta):
    var research_list = Array(Query.ResearchUnlocked())
    
    if research_list == current_research_list:
        return
    current_research_list = research_list
    
    for n in completed_research_container.get_children():
        completed_research_container.remove_child(n)
        n.queue_free()
    for research_id in research_list:
        var new_research = completed_research_template.instantiate()
        completed_research_container.add_child(new_research)
        new_research.research_id = research_id
    

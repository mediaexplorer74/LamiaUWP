extends VBoxContainer

@onready var page_holder = self.find_child("PageHolder")

var page_nodes = {}


func open_page(page_id):
    for n in page_holder.get_children():
        n.hide()
    if not page_id in page_nodes:
        page_nodes[page_id] = load("res://scenes/UI/Pages/"+page_id+"_page.tscn").instantiate()
        page_holder.add_child(page_nodes[page_id])
    page_nodes[page_id].show()
    

extends HBoxContainer

@onready var page_tabs_holder = self.find_child("PageTabs")
@onready var page_holder = self.find_child("PageHolder")
@onready var page_button_template = preload("res://scenes/UI/Global/page_button.tscn")

var current_available_pages = []
var page_nodes = {}


func _ready():
    create_tabs(Query.AvailablePages())


func _process(_delta):
    create_tabs(Query.AvailablePages())


func create_tabs(tabs):
    if current_available_pages != tabs:
        current_available_pages = tabs
        if not current_available_pages:
            return
        for n in page_tabs_holder.get_children():
            page_tabs_holder.remove_child(n)
            n.queue_free()
        for page_tuple in current_available_pages:
            var new_page_button = page_button_template.instantiate() as Button
            new_page_button.text = page_tuple[1]
            new_page_button.pressed.connect(
                func(): open_page(page_tuple[0])
                )
            page_tabs_holder.add_child(new_page_button)


func open_page(page_id):
    for n in page_holder.get_children():
        n.hide()
    if not page_id in page_nodes:
        page_nodes[page_id] = load("res://scenes/UI/Pages/"+page_id+"_page.tscn").instantiate()
        page_holder.add_child(page_nodes[page_id])
    page_nodes[page_id].show()
    

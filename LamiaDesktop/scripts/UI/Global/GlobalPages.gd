extends HBoxContainer

@onready var page_holder = self.find_child("PageHolder")
@onready var page_button_template = preload("res://scenes/UI/Global/global_page_button.tscn")
@onready var page_opener = $/root/GameController/PageOpener

var current_available_pages = []
var page_nodes = {}

func _ready():
    for n in self.get_children():
        self.remove_child(n)
        n.queue_free()
    create_tabs(Query.AvailableGlobalPages())


func _process(_delta):
    create_tabs(Query.AvailableGlobalPages())


func create_tabs(tabs):
    if current_available_pages != tabs:
        current_available_pages = tabs
        if not current_available_pages:
            return
        for n in self.get_children():
            self.remove_child(n)
            n.queue_free()
        for page_tuple in current_available_pages:
            var new_page_button = page_button_template.instantiate() as Button
            new_page_button.text = page_tuple[1]
            new_page_button.pressed.connect(
                func(): page_opener.open_page(page_tuple[0])
                )
            self.add_child(new_page_button)


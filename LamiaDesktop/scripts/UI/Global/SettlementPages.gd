extends HBoxContainer

@onready var page_tabs_holder = self.find_child("LocalPageTabs")
@onready var page_button_template = preload("res://scenes/UI/Global/local_page_button.tscn")
@onready var game_controller = $/root/GameController
@onready var page_opener = $/root/GameController/PageOpener


var current_available_pages = []


func get_pages():
    if not game_controller.ready:
        return []
    return Query.AvailableSettlementPages(game_controller.currentSettlementUuid)


func _ready():
    create_tabs(get_pages())


func _process(_delta):
    create_tabs(get_pages())


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
                func(): page_opener.open_page(page_tuple[0])
                )
            page_tabs_holder.add_child(new_page_button)


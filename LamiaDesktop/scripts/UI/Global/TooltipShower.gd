extends PanelContainer

signal ShowTooltip
signal HideTooltip

@export var anchor = LayoutPreset.PRESET_TOP_LEFT
@export var tooltip_label_text = "":
    set(val):
        tooltip_label_text = val

@onready var tooltip = $Tooltip
@onready var tooltip_label = %TooltipLabel

var display_tooltip = false

func _ready():
    tooltip.hide()
    display_tooltip = false

func _process(_delta):
    if display_tooltip:        
        global_position = get_viewport().get_mouse_position()
        var calc_size = get_size()
        if anchor == PRESET_TOP_RIGHT or anchor == PRESET_BOTTOM_RIGHT:
            global_position -= Vector2(calc_size[0], 0.0)
        if anchor == PRESET_BOTTOM_LEFT or anchor == PRESET_BOTTOM_RIGHT:
            global_position -= Vector2(0.0, calc_size[1])
        if anchor == PRESET_TOP_LEFT or anchor == PRESET_BOTTOM_LEFT:
            global_position += Vector2(10.0, 0.0)

func update_tooltip_display():
    tooltip_label.text = tooltip_label_text

func _on_show_tooltip():
    tooltip.show()
    update_tooltip_display()
    display_tooltip = true

func _on_hide_tooltip():
    tooltip.hide()
    display_tooltip = false

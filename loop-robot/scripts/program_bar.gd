extends HBoxContainer

var buttons: Array

func _get_drag_data(at_position: Vector2) -> Variant:
	var data = {}
	
	var control = Control.new()
	set_drag_preview(control)
	
	return data
	
func _can_drop_data(at_position: Vector2, data: Variant) -> bool:
	print("Can drop data %s", at_position)
	return true
	
func _drop_data(at_position: Vector2, data: Variant) -> void:
	var drag_texture = TextureRect.new()
	drag_texture.expand_mode = TextureRect.EXPAND_FIT_WIDTH
	drag_texture.texture = data["texture"]
	drag_texture.size = data["size"]
	drag_texture.set_script("res://scripts/program_tile.gd")
	
	var control = Control.new()
	self.add_child(control)
	control.add_child(drag_texture)
	drag_texture.get_rect().position = -drag_texture.size/2
	set_drag_preview(control)
	

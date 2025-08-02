extends TextureRect

func _get_drag_data(at_position: Vector2) -> Variant:
	var data = {}
	
	data["texture"] = self.texture
	data["size"] = self.size

	var drag_texture = TextureRect.new()
	drag_texture.expand_mode = self.expand_mode
	drag_texture.texture = self.texture
	drag_texture.size = self.size
	
	var control = Control.new()
	control.add_child(drag_texture)
	drag_texture.get_rect().position = -drag_texture.size/2
	set_drag_preview(control)
	
	return data

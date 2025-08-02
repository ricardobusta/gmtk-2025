extends TileMapLayer

class_name LevelController

@export var start_position: Node2D

func _ready() -> void:
	var game: GameController = preload("res://scenes/game.tscn").instantiate()
	game.set_map(self)

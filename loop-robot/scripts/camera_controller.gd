extends Camera2D

@export var camera_offset: Vector2 = Vector2.ZERO

@onready var target: Node2D = $"../CharacterBody2D"

func _process(delta: float) -> void:
	global_position = target.global_position + camera_offset

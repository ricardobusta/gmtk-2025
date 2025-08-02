extends Node

@onready var play_button: BaseButton = $"../CanvasLayer/UI/PlayButton"
@onready var stop_button: BaseButton = $"../CanvasLayer/UI/StopButton"
@onready var robot = $"../World/CharacterBody2D"
@onready var map = $"../World/TileMapLayer"
@onready var program_container = $"../CanvasLayer/UI/ProgramScrollContainer/PanelContainer/HBoxContainer"

func _ready() -> void:
	play_button.button_down.connect(on_play_button_clicked)
	
func on_play_button_clicked() -> void:
	print("Play")
	var commands = get_program_commands()
	for c in commands:
		print(c)

func get_program_commands() -> Array:
	var result := []

	for child in program_container.get_children():
		if child is ProgramTile:
			var tile: ProgramTile = child
			result.append(ProgramCommands.Command.keys()[tile.command])
	
	return result

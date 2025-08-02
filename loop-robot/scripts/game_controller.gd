extends Node

@onready var play_button: BaseButton = $"../CanvasLayer/UI/PlayButton"
@onready var stop_button: BaseButton = $"../CanvasLayer/UI/StopButton"
@onready var robot: CharacterBody2D = $"../World/CharacterBody2D"
@onready var map = $"../World/TileMapLayer"
@onready var program_container = $"../CanvasLayer/UI/ProgramScrollContainer/PanelContainer/HBoxContainer"

const tile_size: int = 16
const move_time: float = 0.5
const flip_time: float = 0.2
const jump_time: float = 0.5

func _ready() -> void:
	play_button.button_down.connect(on_play_button_clicked)

func on_play_button_clicked() -> void:
	print("Play")
	var commands = get_program_commands()
	for c in commands:
		match c:
			ProgramCommands.Command.MOVE:
				await execute_command_move()
			ProgramCommands.Command.FLIP:
				await execute_command_flip()
			ProgramCommands.Command.JUMP:
				await execute_command_jump()
		print(ProgramCommands.Command.keys()[c])

func get_program_commands() -> Array:
	var result := []

	for child in program_container.get_children():
		if child is ProgramTile:
			var tile: ProgramTile = child
			result.append(tile.command)

	return result

func execute_command_move() -> void:
	var target_pos = robot.position + Vector2(tile_size * robot.scale.x, 0)
	var duration = 0.2
	await animate_to_position(target_pos, duration)

func execute_command_flip() -> void:
	await get_tree().create_timer(flip_time).timeout
	robot.scale.x = -robot.scale.x
	await get_tree().create_timer(flip_time).timeout

func execute_command_jump() -> void:
	var start_pos = robot.position
	var end_pos = robot.position + Vector2(tile_size * (2 * robot.scale.x), 0)
	var jump_height = tile_size
	await animate_jump_arc(start_pos, end_pos, jump_height, jump_time)

func animate_to_position(target: Vector2, duration: float) -> void:
	var elapsed := 0.0
	var start := robot.global_position
	while elapsed < duration:
		elapsed += get_process_delta_time()
		robot.global_position = start.lerp(target, elapsed / duration)
		await get_tree().process_frame

func animate_jump_arc(start: Vector2, end: Vector2, height: float, duration: float) -> void:
	var elapsed := 0.0
	while elapsed < duration:
		elapsed += get_process_delta_time()
		var t = elapsed / duration
		var arc_y = -4 * height * t * (1 - t)
		var pos = start.lerp(end, t)
		pos.y += arc_y  # Apply arc vertically
		robot.position = pos
		await get_tree().process_frame

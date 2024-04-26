extends CanvasLayer

#自定义信号
signal start_game
# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass

func show_message(text: String):
	#设置文本
	$Message.text = text
	#显示
	$Message.show()
	$MessageTimer.start()

func show_game_over():
	show_message("Game Over")
	#等待
	await $MessageTimer.timeout
	$Message.text = "Dodge the\nCreeps!"
	$Message.show()
	#重新设置启动
	await get_tree().create_timer(1.0).timeout
	$StartButton.show()

func update_score(score: int) ->void:
	$ScoreLabel.text = str(score)
	
	
# 开始游戏按钮
func _on_start_button_pressed():
	$StartButton.hide()
	#触发自定义信号
	start_game.emit()
	
# 隐藏文字
func _on_message_timer_timeout():
	$Message.hide()

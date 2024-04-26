extends CharacterBody3D

#添加信号
signal squashed

#最小速度
@export var min_speed = 2
#最大速度
@export var max_speed = 5

func _physics_process(delta):
	move_and_slide()

#主场景调用  初始化怪方法 朝玩家移动
func initialize(start_position, player_position):
	#我们通过将mob放置在start_position来定位它  
	#并将其旋转到player_position，这样它就会看到玩家。 
	look_at_from_position(start_position, player_position, Vector3.UP)
	#在-90度和+90度的范围内随机旋转这个怪物  
	#这样它就不会直接向玩家移动。 
	rotate_y(randf_range(-PI / 4, PI / 4))
	#在指定范围创建随机数
	#var random_speed = randi_range(min_speed, max_speed)
	var random_speed = min_speed
	#我们计算一个向前的速度
	velocity = Vector3.FORWARD * random_speed
	# 基于Y轴旋转  将怪物朝向玩家
	velocity = velocity.rotated(Vector3.UP, rotation.y)
	
	

#怪物离开屏幕信号链接方法
func _on_visible_on_screen_notifier_3d_screen_exited():
	#移除当前怪物
	queue_free()
	
func squash():
	squashed.emit()
	queue_free()


func _on_speed_timer_timeout():
	min_speed += 1
	max_speed += 1

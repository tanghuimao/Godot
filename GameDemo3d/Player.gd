extends CharacterBody3D
#自定义信号
signal hit
#初始速度
@export var speed = 14
#下落速度
@export var fall_acceleration = 75
#跳跃强度
@export var jump_impulse = 20
#弹跳强度
@export var bounce_impulse = 16
#初始向量
var target_velocity = Vector3.ZERO


func _physics_process(delta):
	#初始方向
	var direction = Vector3.ZERO
	#检测移动输入
	if Input.is_action_pressed("move_left"):
		direction.x -= 1
	if Input.is_action_pressed("move_right"):
		direction.x += 1
	if Input.is_action_pressed("move_forward"):
		direction.z -= 1
	if Input.is_action_pressed("move_back"):
		direction.z += 1
		
	if direction != Vector3.ZERO:
		direction = direction.normalized()
		$Pivot.look_at(position + direction, Vector3.UP)
		
	# 地面速度
	target_velocity.x = direction.x * speed
	target_velocity.z = direction.z * speed
	
	# 垂直速度 如果不是在地面
	if !is_on_floor():
		target_velocity.y = target_velocity.y - (fall_acceleration * delta)

	#检测跳跃
	if is_on_floor() && Input.is_action_pressed("jump"):
		target_velocity.y = jump_impulse 
		
	#循环检测是否与怪物发生碰撞 遍历本帧发生的所有碰撞
	for index in range(get_slide_collision_count()):
		# 我们与玩家发生了一次碰撞
		var collision = get_slide_collision(index)
		# 如果碰撞是地面 跳过
		if (collision.get_collider() == null): continue
		#如果碰撞是怪物
		if collision.get_collider().is_in_group("mobs"):
			var mob = collision.get_collider()
			#我们检查是否从上方击中它。
			if Vector3.UP.dot(collision.get_normal()) > 0.1:
				#压扁怪物
				mob.squash()
				target_velocity.y = bounce_impulse
	# 移动角色
	velocity = target_velocity
	move_and_slide()

	
func die():
	hit.emit()
	queue_free()

func _on_mob_detector_body_entered(body):
	die()

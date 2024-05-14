shader_type canvas_item;
//导出变量到引擎
uniform vec3 color : source_color = vec3(1);
uniform float progress : hint_range(0.0, 1.0, 0.1) = 0;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	//获取当前图片的a值
	float current_a = texture(TEXTURE, UV).a;
	//混合颜色
	vec4 target_color = mix(texture(TEXTURE, UV), vec4(color, current_a), progress);
	//设置当前颜色
	COLOR = target_color;
}


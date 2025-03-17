shader_type spatial;
uniform int strokeIndex;

void fragment() {
vec3 albedo;
if(strokeIndex == 0){ // exhaust
	albedo = vec3(0,0,0);
}else if(strokeIndex == 1){
	albedo = vec3(.46,.67,1); //intake -> light blue
}else if(strokeIndex == 2){
	albedo = vec3(1,.98,.48); //compression -> light yellow
}else{
	albedo = vec3(1,0,0); // power -> red
}

ALBEDO = albedo;
}

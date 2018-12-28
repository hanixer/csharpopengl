#version 330 core

out vec4 FragColor;
uniform vec3 uniformColor;

void main() {
	FragColor = vec4(uniformColor, 1.0);
}
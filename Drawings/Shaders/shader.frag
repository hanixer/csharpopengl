#version 330 core

in vec2 TexCoord;
in vec3 ourColor;

out vec4 FragColor;

uniform sampler2D texture1;
uniform sampler2D texture2;
uniform vec3 uniformColor;

void main()
{
	FragColor = vec4(uniformColor, 1.0f);
}